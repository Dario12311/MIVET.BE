﻿using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using MIVET.BE.Infraestructura.Configurations;
using MIVET.BE.Infraestructura.Data;
using MIVET.BE.Infraestructura.Persintence.EntityConfiguration;
using MIVET.BE.Infraestructura.Persintence.EntityConfiguration.ConfiguracionesClinicas;
using MIVET.BE.Transversales;
using MIVET.BE.Transversales.Common;
using MIVET.BE.Transversales.Core;
using MIVET.BE.Transversales.Entidades;
using MIVET.BE.Transversales.Entidades.Country;
using MIVET.BE.Transversales.Entidades.MaritalStatus;

namespace MIVET.BE.Infraestructura.Persintence;

public class MIVETDbContext(DbContextOptions options) : DbContext(options)
{
    internal IMediator _mediator;

    #region Entities

    // Agrega aquí tus DbSet cuando los tengas, por ejemplo:
    public DbSet<Country> Country { get; set; }
    public DbSet<MaritalStatus> MaritalStatus { get; set; }
    public DbSet<Mascota> Mascota { get; set; }
    // Agregar DbSet para PersonaCliente
    public DbSet<PersonaCliente> PersonaCliente { get; set; }
    public DbSet<TipoDocumento> TipoDocumento { get; set; }
    public DbSet<MedicoVeterinario> MedicoVeterinario { get; set; }
    public DbSet<HistoriaClinicaMascota> HistoriaClinicaMascota { get; set; }
    public DbSet<Rol> Rol { get; set; }
    public DbSet<Usuarios> Usuarios { get; set; }
    public DbSet<Productos> Productos { get; set; }
    public DbSet<Consultas> Consultas { get; set; }
    public DbSet<Dias> Dias { get; set; }
    public DbSet<EstadoCita> EstadoCita { get; set; }
    public DbSet<LugarConsulta> LugarConsultas { get; set; }
    public DbSet<TipoConsulta> TipoConsultas { get; set; }
    public DbSet<HorasMedicas> HorasMedicas { get; set; }
    public DbSet<HorarioVeterinario> HorarioVeterinarios { get; set; }
    public DbSet<Cita> Citas { get; set; }
    public DbSet<HistorialClinico> HistorialClinico { get; set; }
    public DbSet<ProcedimientoMedico> ProcedimientoMedico { get; set; }
    public DbSet<Factura> Factura { get; set; }
    public DbSet<DetalleFactura> DetalleFactura { get; set; }


    #endregion

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        if (_mediator != null)
            await _mediator.DispatchDomainEventsAsync(this, cancellationToken);

        AuditChanges();
        return await base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MIVETDbContext).Assembly);
        modelBuilder.ApplyConfiguration(new HorarioVeterinarioConfiguration());

        // AGREGAR LA CONFIGURACIÓN DE CITAS
        modelBuilder.ApplyConfiguration(new CitaConfiguration());
        modelBuilder.ApplyConfiguration(new HistorialClinicoEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ProcedimientoMedicoEntityConfiguration());
        modelBuilder.ApplyConfiguration(new FacturaEntityConfiguration());
        modelBuilder.ApplyConfiguration(new DetalleFacturaEntityConfiguration());

        // Configuración básica para PersonaCliente
        modelBuilder.Entity<PersonaCliente>(entity =>
        {
            entity.HasKey(e => e.NumeroDocumento);
            entity.Property(e => e.PrimerNombre).IsRequired();
            entity.Property(e => e.PrimerApellido).IsRequired();
        });

        HasSequences(modelBuilder);

        base.OnModelCreating(modelBuilder);

        // CONFIGURACIÓN SIMPLE PARA CITAS (SIN TRIGGERS)
        modelBuilder.Entity<Cita>(entity =>
        {
            // Configuración simple sin triggers - ya está en CitaConfiguration
            entity.ToTable("Citas");
        });
    }

    // REMOVER OnConfiguring problemático
    // protected override void OnConfiguring ya no es necesario aquí
    // porque el contexto se configura en Program.cs

    private void AuditChanges()
    {
        ChangeTracker.DetectChanges();

        var entityEntries = ChangeTracker.Entries()
            .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .ToList();

        if (!entityEntries.Any()) return;

        var userName = "MIVET_Service";

        foreach (var entry in entityEntries)
        {
            if (entry.Entity is ICreationAuditable && entry.State == EntityState.Added)
            {
                if ((DateTime)entry.Property(DbConstants.ShadowProperties.CreatedDate).CurrentValue == default)
                    entry.Property(DbConstants.ShadowProperties.CreatedDate).CurrentValue = DateTime.UtcNow;

                entry.Property(DbConstants.ShadowProperties.CreatedBy).CurrentValue ??= userName;
            }
            if (entry.Entity is IUpdateAuditable && entry.State == EntityState.Modified)
            {
                if (!entry.Property(DbConstants.ShadowProperties.UpdatedDate).IsModified)
                    entry.Property(DbConstants.ShadowProperties.UpdatedDate).CurrentValue = DateTime.UtcNow;

                if (!entry.Property(DbConstants.ShadowProperties.UpdatedBy).IsModified)
                    entry.Property(DbConstants.ShadowProperties.UpdatedBy).CurrentValue ??= userName;
            }
            if (entry.Entity is ISoftDeleteAuditable && entry.State == EntityState.Deleted)
            {
                if (!entry.Property(DbConstants.ShadowProperties.DeletedDate).IsModified)
                    entry.Property(DbConstants.ShadowProperties.DeletedDate).CurrentValue = DateTime.UtcNow;

                if (!entry.Property(DbConstants.ShadowProperties.DeletedBy).IsModified)
                    entry.Property(DbConstants.ShadowProperties.DeletedBy).CurrentValue ??= userName;

                if (!entry.Property(DbConstants.ShadowProperties.IsDeleted).IsModified)
                    entry.Property(DbConstants.ShadowProperties.IsDeleted).CurrentValue = true;

                entry.State = EntityState.Modified;
            }
        }
    }

    private void HasSequences(ModelBuilder modelBuilder)
    {
        // Agrega aquí cualquier secuencia necesaria para tus entidades
    }
}

public class MIVETDbContextScopedFactory(
    IDbContextFactory<MIVETDbContext> pooledFactory,
    IMediator mediator) : IDbContextFactory<MIVETDbContext>
{
    public MIVETDbContext CreateDbContext()
    {
        var context = pooledFactory.CreateDbContext();
        context._mediator = mediator;
        return context;
    }
}
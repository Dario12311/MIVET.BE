using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using MIVET.BE.Infraestructura.Data;
using MIVET.BE.Transversales.Common;
using System.Reflection;

namespace MIVET.BE.Infraestructura.Extensions;

public static class ModelBuilderExtensions
{
    public static string GetSequenceName<TEntity>(this EntityTypeBuilder<TEntity> entityTypeBuilder)
        where TEntity : class
    {
        return $"{typeof(TEntity).Name.ToLower()}seq";
    }

    public static ModelBuilder ApplyShadowProperties(this ModelBuilder modelBuilder)
    {
        var types = modelBuilder.Model.GetEntityTypes()
            .Where(t => typeof(IAuditableEntity).IsAssignableFrom(t.ClrType) && t.ClrType.IsClass && t.BaseType == null)
            .Select(entity => entity.ClrType);

        foreach (var entityType in types)
        {
            var method = ApplyEntityAuditMethodInfo.MakeGenericMethod(entityType);
            method.Invoke(modelBuilder, new object[] { modelBuilder });
        }

        return modelBuilder;
    }

    public static readonly MethodInfo ApplyEntityAuditMethodInfo
        = typeof(ModelBuilderExtensions).GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Single(t => t.IsGenericMethod && t.Name == nameof(ApplyEntityAudit));

    public static ModelBuilder ApplyEntityAudit<T>(this ModelBuilder modelBuilder) where T : class, IAuditableEntity
    {
        if (typeof(ICreationAuditable).IsAssignableFrom(typeof(T)))
        {
            modelBuilder
                .Entity<T>()
                .Property<string>(DbConstants.ShadowProperties.CreatedBy)
                .HasMaxLength(DbConstants.ShadowProperties.CreatedByLength)
                .IsRequired(true);

            modelBuilder
                .Entity<T>()
                .Property<DateTime>(DbConstants.ShadowProperties.CreatedDate)
                .HasColumnType("datetime2")
                .IsRequired(true);
        }
        if (typeof(IUpdateAuditable).IsAssignableFrom(typeof(T)))
        {
            modelBuilder
                .Entity<T>()
                .Property<string>(DbConstants.ShadowProperties.UpdatedBy)
                .HasMaxLength(DbConstants.ShadowProperties.UpdatedByLength)
                .IsRequired(false);

            modelBuilder
                .Entity<T>()
                .Property<DateTime?>(DbConstants.ShadowProperties.UpdatedDate)
                .HasColumnType("datetime2");
        }
        if (typeof(ISoftDeletable).IsAssignableFrom(typeof(T)))
        {
            modelBuilder
                .Entity<T>()
                .Property<bool>(DbConstants.ShadowProperties.IsDeleted)
                .HasDefaultValue(false);

            modelBuilder
                .Entity<T>()
                .Property<string>(DbConstants.ShadowProperties.DeletedBy)
                .HasMaxLength(DbConstants.ShadowProperties.DeletedByLength)
                .IsRequired(false);

            modelBuilder
                .Entity<T>()
                .Property<DateTime?>(DbConstants.ShadowProperties.DeletedDate)
                .HasColumnType("datetime2");

            modelBuilder
                .Entity<T>()
                .HasQueryFilter(e => EF.Property<bool>(e, DbConstants.ShadowProperties.IsDeleted) == false);
        }

        return modelBuilder;
    }
}
namespace MIVET.BE.Infraestructura.Persintence.SeedData;

public static class SampleDataSeeder
{
    public static async Task PopulateSamples(MIVETDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context, nameof(context));
        context.Database.EnsureCreated();

        // Verifica si los datos ya existen antes de insertarlos
        //Poner datos de prueba, cuando las primeras clases estén para la migración
        //if (!await context.Set<EstadoVenta>().AnyAsync())
        //{
        //    context.Set<EstadoVenta>().AddRange(EstadoVenta.GetAll());
        //}

        //if (!await context.Set<MetodoPago>().AnyAsync())
        //{
        //    context.Set<MetodoPago>().AddRange(MetodoPago.GetAll());
        //}

        //if (!await context.Set<TipoCliente>().AnyAsync())
        //{
        //    context.Set<TipoCliente>().AddRange(TipoCliente.GetAll());
        //}

        await context.SaveChangesAsync();
    }
}
using Microsoft.EntityFrameworkCore;
namespace MIVET.BE.Infraestructura.Persintence.SeedData;

public static partial class ModelBuilderExtensions
{
    public static void Seed(this ModelBuilder modelBuilder)
    {
        // Se eliminó HasData(), los datos se insertan en SampleDataSeeder
    }
}
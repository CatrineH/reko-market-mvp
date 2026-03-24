using Microsoft.EntityFrameworkCore;
using reko_mini_project.Server.Features.Products;

namespace reko_mini_project.Server.Data;

public static class ProductSeeder
{
    public static async Task SeedAsync(AppDbContext dbContext, CancellationToken cancellationToken = default)
    {
        await dbContext.Database.MigrateAsync();

        if (await dbContext.Products.AnyAsync(cancellationToken))
        {
            return;
        }

        dbContext.Products.AddRange(
            new Product { Id = Guid.NewGuid(), Name = "Gulrot", Weight = 500, Price = 69.99m },
            new Product { Id = Guid.NewGuid(), Name = "Egg", Weight = 125, Price = 129.99m },
            new Product { Id = Guid.NewGuid(), Name = "Potet", Weight = 5000, Price = 29.99m },
            new Product { Id = Guid.NewGuid(), Name = "Lammelår", Weight = 1750, Price = 319.99m },
            new Product { Id = Guid.NewGuid(), Name = "Laks", Weight = 1000, Price = 199.99m },
            new Product { Id = Guid.NewGuid(), Name = "Brød", Weight = 500, Price = 49.99m },
            new Product { Id = Guid.NewGuid(), Name = "Melk", Weight = 1000, Price = 19.99m },
            new Product { Id = Guid.NewGuid(), Name = "Ost", Weight = 250, Price = 89.99m },
            new Product { Id = Guid.NewGuid(), Name = "Smør", Weight = 250, Price = 39.99m },
            new Product { Id = Guid.NewGuid(), Name = "Yoghurt", Weight = 500, Price = 29.99m }
        );

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
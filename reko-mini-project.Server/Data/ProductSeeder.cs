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
            new Product { Id = Guid.NewGuid(), Name = "Gulrot", Category="Grønnsaker", Description="Herlige gulrøtter fra Biri gård, perfekte for søndagsmiddagen!", Weight = 500, Price = 69.99m },
            new Product { Id = Guid.NewGuid(), Name = "Egg", Category="MeieriOgEgg", Description="Friske egg fra Biri gård fra norske frittgående høner.", Weight = 125, Price = 129.99m },
            new Product { Id = Guid.NewGuid(), Name = "Potet", Category="Grønnsaker", Description="Allsidige poteter fra Biri gård som passer både til middag og baking.", Weight = 5000, Price = 29.99m },
            new Product { Id = Guid.NewGuid(), Name = "Lammelår", Category="KjøttOgFisk", Description="Saftig lammelår fra Biri gård som løfter helgemiddagen.", Weight = 1750, Price = 319.99m },
            new Product { Id = Guid.NewGuid(), Name = "Laks", Category="KjøttOgFisk", Description="Norsk laks fra Biri gård av høy kvalitet, perfekt for ovnsbaking eller sushi.", Weight = 1000, Price = 199.99m },
            new Product { Id = Guid.NewGuid(), Name = "Brød", Category="Bakervarer", Description="Nystekt brød fra Biri gård med sprø skorpe og mykt innhold.", Weight = 500, Price = 49.99m },
            new Product { Id = Guid.NewGuid(), Name = "Melk", Category="MeieriOgEgg", Description="Fersk melk fra Biri gård, godt til frokost og baking.", Weight = 1000, Price = 19.99m },
            new Product { Id = Guid.NewGuid(), Name = "Ost", Category="MeieriOgEgg", Description="Kremet ost fra Biri gård som smelter perfekt på brød og i matlaging.", Weight = 250, Price = 89.99m },
            new Product { Id = Guid.NewGuid(), Name = "Smør", Category="MeieriOgEgg", Description="Fyldig smør fra Biri gård som gir ekstra smak til steking og bakverk.", Weight = 250, Price = 39.99m },
            new Product { Id = Guid.NewGuid(), Name = "Yoghurt", Category="MeieriOgEgg", Description="Naturell yoghurt fra Biri gård som passer til frokost og mellommåltider.", Weight = 500, Price = 29.99m },
            new Product { Id = Guid.NewGuid(), Name = "Epler", Category="Frukt", Description="Saftige epler fra Biri gård med søt og frisk smak.", Weight = 1000, Price = 49.99m },
            new Product { Id = Guid.NewGuid(), Name = "Moreller", Category="Frukt", Description="Søte moreller fra Biri gård, perfekte som snacks eller i dessert.", Weight = 500, Price = 99.99m },
            new Product { Id = Guid.NewGuid(), Name = "Jordbær", Category="Bær", Description="Friske jordbær fra Biri gård med sommerlig smak.", Weight = 500, Price = 79.99m },
            new Product { Id = Guid.NewGuid(), Name = "Rips", Category="Bær", Description="Syrlige rips fra Biri gård som passer godt til saft og bakst.", Weight = 250, Price = 69.99m }
        );

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
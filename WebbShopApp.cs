
using Microsoft.EntityFrameworkCore;

namespace WebbShop
{
    internal class WebbShopApp
    {
        private readonly WebbShopDbContext dbContext;
        public WebbShopApp(WebbShopDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        internal void Init()
        {
            dbContext.Database.EnsureCreated();
            if (dbContext.Products.Count() == 0)
            {
                // Skapa några produkter
                dbContext.Products.AddRange(
                    new Product { Name = "Produkt 1", Description = "Beskrivning 1", Price = 100.00m },
                    new Product { Name = "Produkt 2", Description = "Beskrivning 2", Price = 200.00m },
                    new Product { Name = "Produkt 3", Description = "Beskrivning 3", Price = 300.00m }
                );
                dbContext.SaveChanges();
            }

            // Skapa admin-användare om ingen finns
            if (!dbContext.Users.Any())
            {
                var user = new User
                {
                    Username = "admin",
                    PasswordHash = PasswordHelper.HashPassword("1234")
                };

                dbContext.Users.Add(user);
                dbContext.SaveChanges();
            }
        }

        internal void RunMenu()
        {
            throw new NotImplementedException();
        }
    }
}
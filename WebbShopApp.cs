
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

        private bool Login()
        {
            Console.WriteLine("==== Inloggning ====");
            Console.Write("Ange användarnamn: ");
            var username = Console.ReadLine();
            Console.Write("Ange lösenord: ");
            var password = Console.ReadLine();
            var user = dbContext.Users.SingleOrDefault(u => u.Username == username);
            if (user != null && PasswordHelper.VerifyPassword(password, user.PasswordHash))
            {
                Console.WriteLine("Inloggning lyckades!");
                return true;
            }
            else
            {
                Console.WriteLine("Felaktigt användarnamn eller lösenord.");
                return false;
            }
        }

        internal void RunMenu()
        {
            if (!Login())
            {
                Console.WriteLine("Inloggning misslyckades. Avslutar programmet...");
                return;
            }

            Console.WriteLine("\n====Välkommen till ShopApp!====");
            bool running = true;
            while (running)
            {
                Console.WriteLine("\n1. Lista produkter");
                Console.WriteLine("2. Skapa order");
                Console.WriteLine("3. Lista ordrar");
                Console.WriteLine("4. Ta bort order");
                Console.WriteLine("5. Avsluta");
                Console.Write("\nVälj ett alternativ: ");

                var choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        ListProducts();
                        break;
                    case "2":
                        CreateOrder();
                        break;
                    case "3":
                        ListOrders();
                        break;
                    case "4":
                        RemoveOrder();
                        break;
                    case "5":
                        running = false;
                        break;
                    default:
                        Console.WriteLine("\nOgiltigt val, försök igen.");
                        break;
                }
            }
        }
    }
}
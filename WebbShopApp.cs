
using Microsoft.EntityFrameworkCore;

using System.Text.RegularExpressions;
using WebbShop.Shared;
using WebbShop.Shared.Models;
using System.Text.RegularExpressions;

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

        private void ListProducts()
        {
            var products = dbContext.Products.ToList();
            Console.WriteLine("\nProdukter:");
            foreach (var product in products)
            {
                Console.WriteLine($"{product.Id}: {product.Name} - {product.Description} - {product.Price:C}");
            }
        }


        private void CreateOrder()
        {
            // Felhantering
            try
            {
                Console.Write("\nAnge kundens namn: ");
                string customerName = Console.ReadLine();

                // Regex: endast A-Ö, a-ö och mellanslag tillåts
                if (string.IsNullOrWhiteSpace(customerName) ||
                    customerName.Length < 2 ||
                    !Regex.IsMatch(customerName, @"^[A-Za-zÅÄÖåäö\s]+$"))
                {
                    Console.WriteLine(" Endast bokstäver och mellanslag tillåtna i namnet.");
                    return;
                }

                ListProducts();
                Console.Write("Ange produktens ID för ordern: ");
                if (!int.TryParse(Console.ReadLine(), out int productId))
                {
                    Console.WriteLine("Ogiltigt nummer. Ange ett heltal.");
                    var product = dbContext.Products.Find(productId);
                    if (product != null)
                    {
                        var order = new Order
                        {
                            CustomerName = customerName,
                            ProductId = productId,
                            OrderDate = DateTime.Now,
                        };
                        dbContext.Orders.Add(order);
                        dbContext.SaveChanges();
                        Console.WriteLine("\nOrder skapad!");
                    }
                    else
                    {
                        Console.WriteLine("\nProdukten hittades inte.");
                    }
                }
                else
                {
                    Console.WriteLine("\nOgiltigt produkt-ID.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nEtt fel inträffade: {ex.Message}");
            }
        }
        private void ListOrders()
        {
            try
            {
                var orders = dbContext.Orders
                    .Select(o => new
                    {
                        o.Id,
                        o.CustomerName,
                        ProductName = o.Product.Name,
                        o.Status,
                        o.OrderDate
                    })
                    .ToList();
                Console.WriteLine("\nOrdrar:");
                foreach (var order in orders)
                {
                    Console.WriteLine($"{order.Id}: {order.CustomerName} - {order.ProductName} - {order.Status} - {order.OrderDate} \n");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nEtt fel inträffade: {ex.Message}");
            }
        }

        private void RemoveOrder()
        {
            try
            {
                ListOrders();
                Console.Write("Ange orderns ID för att ta bort den: ");
                if (int.TryParse(Console.ReadLine(), out int orderId))
                {
                    var order = dbContext.Orders.Find(orderId);
                    if (order != null)
                    {
                        dbContext.Orders.Remove(order);
                        dbContext.SaveChanges();
                        Console.WriteLine("\nOrder borttagen!");
                    }
                    else
                    {
                        Console.WriteLine("\nOrdern hittades inte.");
                    }
                }
                else
                {
                    Console.WriteLine("\nOgiltigt order-ID.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nEtt fel inträffade: {ex.Message}");
            }
        }
    }
}
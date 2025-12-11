
using MongoDB.Driver;
using System.Collections.Generic;
using Shopping.API.Models;
namespace Shopping.API.Data;

public class ProductContext
{
    public ProductContext(IConfiguration configuration)
    {
        // read connection info from configuration
        var client = new MongoClient(configuration["DatabaseSettings:ConnectionString"]);
        var database = client.GetDatabase(configuration["DatabaseSettings:DatabaseName"]);

        Products = database.GetCollection<Product>(configuration["DatabaseSettings:CollectionName"]);
        SeedData(Products);
    }

    public IMongoCollection<Product> Products { get; }

    private static void SeedData(IMongoCollection<Product> productCollection)
    {
        bool existsProducts = productCollection.Find(p => true).Any();

        if (!existsProducts) 
        {
            productCollection.InsertMany(GetConfiguredProducts()); 
        }
    }

    private static IEnumerable<Product> GetConfiguredProducts()
    {
        return new List<Product>()
        {
             new Product()
                {
                    Name = "IPhone X",
                    Description = "This phone is the company's biggest change to its flagship smartphone in years. It includes a borderless.",
                    ImageFile = "product-1.png",
                    Price = 950.00M,
                    Category = "Smart Phone"
                },
                new Product()
                {
                    Name = "Samsung 10",
                    Description = "This phone is the company's biggest change to its flagship smartphone in years. It includes a borderless.",
                    ImageFile = "product-2.png",
                    Price = 840.00M,
                    Category = "Smart Phone"
                },
                new Product()
                {
                    Name = "Huawei Plus",
                    Description = "This phone is the company's biggest change to its flagship smartphone in years. It includes a borderless.",
                    ImageFile = "product-3.png",
                    Price = 650.00M,
                    Category = "White Appliances"
                },
                new Product()
                {
                    Name = "Xiaomi Mi 9",
                    Description = "This phone is the company's biggest change to its flagship smartphone in years. It includes a borderless.",
                    ImageFile = "product-4.png",
                    Price = 470.00M,
                    Category = "White Appliances"
                },
                new Product()
                {
                    Name = "HTC U11+ Plus",
                    Description = "This phone is the company's biggest change to its flagship smartphone in years. It includes a borderless.",
                    ImageFile = "product-5.png",
                    Price = 380.00M,
                    Category = "Smart Phone"
                },
                new Product()
                {
                    Name = "LG G7 ThinQ EndofCourse",
                    Description = "This phone is the company's biggest change to its flagship smartphone in years. It includes a borderless.",
                    ImageFile = "product-6.png",
                    Price = 240.00M,
                    Category = "Home Kitchen"
                }

        };
    }
}
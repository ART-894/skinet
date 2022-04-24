using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Core.Entities;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Data
{
  public class StoreContextSeed
  {
    public static async Task SeedAsync(StoreContext context, ILoggerFactory loggerFactory)
    {
      try
      {
        if (!context.ProductBrands.Any())
        {
          var brandsData = File.ReadAllText("../Infrastructure/Data/brands.json");

          var brands = JsonSerializer.Deserialize<List<ProductBrand>>(brandsData);

          foreach (var item in brands)
          {
            context.ProductBrands.Add(item);
          }

          await context.SaveChangesAsync();
        }


        if (!context.ProductTypes.Any())
        {
          var typesData = File.ReadAllText("../Infrastructure/Data/types.json");

          var types = JsonSerializer.Deserialize<List<ProductType>>(typesData);

          foreach (var item in types)
          {
            context.ProductTypes.Add(item);
          }

          await context.SaveChangesAsync();
        }



        if (!context.Products.Any())
        {
          var productData = File.ReadAllText("../Infrastructure/Data/products.json");

          var product = JsonSerializer.Deserialize<List<Product>>(productData);

          foreach (var item in product)
          {
            context.Products.Add(item);
          }

          await context.SaveChangesAsync();
        }

      }

      catch (Exception ex)
      {
        var logger = loggerFactory.CreateLogger<StoreContextSeed>();
        logger.LogError(ex.Message);
      }

    }
  }
}
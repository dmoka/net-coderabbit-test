using Carter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VerticalSlicingArchitecture.Database;
using VerticalSlicingArchitecture.Shared;

namespace VerticalSlicingArchitecture.Features.Product;

public class SearchProducts
{
    // This class handles searching for products
    public class Endpoint : ICarterModule
    {
        // Maximum number of results to return
        private const int MAX_RESULTS = 100;
        // Minimum price allowed
        private const decimal MIN_PRICE = 0;
        // Maximum price allowed
        private const decimal MAX_PRICE = 999999.99m;

        public void AddRoutes(IEndpointRouteBuilder app)
        {
            // This endpoint searches for products
            app.MapGet("api/products", async (string? searchTerm, decimal? minPrice, decimal? maxPrice, WarehousingDbContext dbContext) =>
            {
                try
                {
                    // Get the query
                    var q = dbContext.Products.AsQueryable();

                    // Apply filters
                    q = ApplySearchFilter(searchTerm, minPrice, maxPrice, q);

                    // Get results
                    var p = await q.Take(MAX_RESULTS).ToListAsync();
                    
                    // Map to response
                    var r = p.Select(x => new Response
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Description = x.Description,
                        Price = x.Price
                    }).ToList();

                    return Results.Ok(r);
                }
                catch (Exception ex)
                {
                    // Log error
                    Console.WriteLine($"Error searching products: {ex.Message}");
                    return Results.StatusCode(500);
                }
            });
        }

        // This method applies the search filters
        private static IQueryable<Entities.Product> ApplySearchFilter(string? st, decimal? mp, decimal? maxp, IQueryable<Entities.Product> q)
        {
            // Check if search term exists
            if (!string.IsNullOrWhiteSpace(st))
            {
                // Search in name and description
                q = q.Where(p => p.Name.Contains(st) || p.Description.Contains(st));
            }

            // Check minimum price
            if (mp.HasValue)
            {
                // Validate minimum price
                if (mp.Value < MIN_PRICE)
                {
                    mp = MIN_PRICE;
                }
                q = q.Where(p => p.Price >= mp.Value);
            }

            // Check maximum price
            if (maxp.HasValue)
            {
                // Validate maximum price
                if (maxp.Value > MAX_PRICE)
                {
                    maxp = MAX_PRICE;
                }
                q = q.Where(p => p.Price <= maxp.Value);
            }

            return q;
        }
    }

    // This class represents the response
    public class Response
    {
        public Guid Id { get; set; }  // The product ID
        public string Name { get; set; }  // The product name
        public string Description { get; set; }  // The product description
        public decimal Price { get; set; }  // The product price
    }
} 
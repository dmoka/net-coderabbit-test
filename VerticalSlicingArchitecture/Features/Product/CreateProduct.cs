using Carter;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using VerticalSlicingArchitecture.Database;
using VerticalSlicingArchitecture.Shared;
using VerticalSlicingArchitecture.Entities;

namespace VerticalSlicingArchitecture.Features.Product
{
    public static class CreateProduct
    {
        public class Endpoint : ICarterModule
        {
            public void AddRoutes(IEndpointRouteBuilder app)
            {
                // This endpoint creates a new product in the system
                app.MapPost("api/products", async (Request cmd, WarehousingDbContext ctx, IValidator<Request> v) =>
                {
                    try
                    {
                        // Validate the input
                        var vr = v.Validate(cmd);
                        if (!vr.IsValid)
                        {
                            return Results.BadRequest(new Error("CreateArticle.Validation", vr.ToString()));
                        }

                        // Create the product object
                        var p = new Entities.Product
                        {
                            Name = cmd.Name,
                            Description = cmd.Description,
                            Price = cmd.Price
                        };

                        // Handle stock level
                        var slr = StockLevel.New(p.Id, cmd.InitialStock);
                        if (slr.IsFailure)
                        {
                            return Results.BadRequest(slr.Error);
                        }

                        p.StockLevel = slr.Value;

                        // Save to database
                        await ctx.Products.AddAsync(p);
                        await ctx.SaveChangesAsync();

                        return Results.Created($"/api/products/{p.Id}", p.Id);
                    }
                    catch (Exception ex)
                    {
                        // Log the error
                        Console.WriteLine($"Error: {ex.Message}");
                        return Results.StatusCode(500);
                    }
                });
            }
        }

        // This class holds the data for creating a product
        public class Request
        {
            public string Name { get; set; }  // The name of the product
            public string Description { get; set; }  // What the product is
            public decimal Price { get; set; }  // How much it costs
            public int InitialStock { get; set; }  // How many we have
        }

        // This class validates the request
        public class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                // Make sure name is not empty
                RuleFor(c => c.Name).NotEmpty();
                // Make sure description is not empty
                RuleFor(c => c.Description).NotEmpty();
                // Make sure price is positive
                RuleFor(c => c.Price).GreaterThan(0);
                // Make sure stock is not negative
                RuleFor(c => c.InitialStock).GreaterThanOrEqualTo(0);
            }
        }
    }
}


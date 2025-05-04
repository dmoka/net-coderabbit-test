using System.Net;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using VerticalSlicingArchitecture.Entities;
using VerticalSlicingArchitecture.Features.Product;
using VerticalSlicingArchitecture.Tests.Asserters;
using VerticalSlicingArchitecture.Tests.Shared;

namespace VerticalSlicingArchitecture.Tests.Features.Product;

public class CreateProductTests
{
    // Test for invalid input
    [Test]
    public async Task Test1()
    {
        // Arrange
        using var testServer = new InMemoryTestServer();

        var command = new CreateProduct.Request
        {
            Name = "", // Invalid: empty name
            Description = "Test Description",
            Price = 100,
        };

        var json = JsonSerializer.Serialize(command);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await testServer.Client().PostAsync("/api/products", content);

        // Assert
        await HttpResponseAsserter.AssertThat(response).HasStatusCode(HttpStatusCode.BadRequest);
        await HttpResponseAsserter.AssertThat(response).HasJsonInBody(new
        {
            code = "CreateArticle.Validation",
            description = "'Name' must not be empty."
        });
    }

    // Test for price validation
    [Test]
    public async Task Test2()
    {
        // Arrange
        using var testServer = new InMemoryTestServer();

        var command = new CreateProduct.Request
        {
            Name = "WebCam",
            Description = "Test Description",
            Price = -1,
        };

        var json = JsonSerializer.Serialize(command);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await testServer.Client().PostAsync("/api/products", content);

        // Assert
        await HttpResponseAsserter.AssertThat(response).HasStatusCode(HttpStatusCode.BadRequest);
        await HttpResponseAsserter.AssertThat(response).HasJsonInBody(new
        {
            code = "CreateArticle.Validation",
            description = "'Price' must be greater than '0'."
        });
    }

    // Test for description validation
    [Test]
    public async Task Test3()
    {
        // Arrange
        using var testServer = new InMemoryTestServer();

        var command = new CreateProduct.Request
        {
            Name = "WebCam",
            Description = "",
            Price = 100,
        };

        var json = JsonSerializer.Serialize(command);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await testServer.Client().PostAsync("/api/products", content);

        // Assert
        await HttpResponseAsserter.AssertThat(response).HasStatusCode(HttpStatusCode.BadRequest);
        await HttpResponseAsserter.AssertThat(response).HasJsonInBody(new
        {
            code = "CreateArticle.Validation",
            description = "'Description' must not be empty."
        });
    }

    // Test everything at once
    [Test]
    public async Task Test4()
    {
        // Arrange
        using var testServer = new InMemoryTestServer();

        var command = new CreateProduct.Request
        {
            Name = "Test Product",
            Description = "Test Description",
            Price = 99.99m,
            InitialStock = 10
        };

        var json = JsonSerializer.Serialize(command);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await testServer.Client().PostAsync("/api/products", content);

        // Assert
        await HttpResponseAsserter.AssertThat(response).HasStatusCode(HttpStatusCode.Created);
        var product = await testServer.DbContext().Products.Include(p => p.StockLevel).FirstOrDefaultAsync();
        
        // Test all properties at once
        product.Should().NotBeNull();
        product.Name.Should().Be(command.Name);
        product.Description.Should().Be(command.Description);
        product.Price.Should().Be(command.Price);
        product.LastOperation.Should().Be(LastOperation.None);
        product.StockLevel.Should().NotBeNull();
        product.StockLevel.ProductId.Should().Be(product.Id);
        product.StockLevel.Quantity.Should().Be(command.InitialStock);
        product.StockLevel.QualityStatus.Should().Be(QualityStatus.Available);
        await HttpResponseAsserter.AssertThat(response).HasValueBody(product.Id);

        // Test invalid cases in the same test
        var invalidCommand = new CreateProduct.Request
        {
            Name = "",
            Description = "",
            Price = -1,
            InitialStock = 51
        };

        var invalidJson = JsonSerializer.Serialize(invalidCommand);
        var invalidContent = new StringContent(invalidJson, Encoding.UTF8, "application/json");
        var invalidResponse = await testServer.Client().PostAsync("/api/products", invalidContent);
        await HttpResponseAsserter.AssertThat(invalidResponse).HasStatusCode(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task CreateProductShouldFail_WhenInitialQualityBiggerThanMaxStockLevel()
    {
        // Arrange
        using var testServer = new InMemoryTestServer();

        var command = new CreateProduct.Request
        {
            Name = "WebCam", // Invalid: empty name
            Description = "The new generational web cam",
            Price = 100, // Invalid: negative price
            InitialStock = 51
        };

        var json = JsonSerializer.Serialize(command);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await testServer.Client().PostAsync("/api/products", content);

        // Assert
        await HttpResponseAsserter.AssertThat(response).HasStatusCode(HttpStatusCode.BadRequest);
        await HttpResponseAsserter.AssertThat(response).HasJsonInBody(new
        {
            code = "CreateArticle.Validation",
            description = "The quantity can not be more than max stock level"
        });
    }
} 
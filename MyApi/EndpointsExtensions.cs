using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MyApi;

public record Product(int Id, string Name, decimal Price);

public record CreateProductRequest(
    [Required] string Name,
    [Range(0.01, double.MaxValue)] decimal Price
);

public static class EndpointsExtensions
{
  public const string BaseUrl = @"https://localhost:7229";
  public const string ProductNotFoundType = $"{BaseUrl}/errors/product-not-found";
  public const string ValidationErrorType = $"{BaseUrl}/errors/validation-error";

  public static void MapProductEndpoints(this IEndpointRouteBuilder app)
  {
    var products = new List<Product>
    {
      new(1, "Laptop", 999.99m),
      new(2, "Mouse", 29.99m),
      new(3, "Keyboard", 79.99m)
    };

    // GET /products - Récupérer tous les produits
    app.MapGet("/products", () => Results.Ok(products))
        .WithName("GetProducts")
        .WithOpenApi();

    // GET /products/{id} - Récupérer un produit par ID
    app.MapGet("/products/{id:int}", (int id) =>
    {
      var product = products.FirstOrDefault(p => p.Id == id);

      if (product == null)
      {
        // Retourne un Problem Details conforme RFC 9457
        return Results.Problem(
            title: "Product not found",
            detail: $"No product found with ID {id}",
            statusCode: StatusCodes.Status404NotFound,
            type: ProductNotFoundType
        );
      }

      return Results.Ok(product);
    })
    .WithName("GetProductById")
    .WithOpenApi();

    // POST /products - Créer un nouveau produit
    app.MapPost("/products", ([FromBody] CreateProductRequest request) =>
    {
      // Validation manuelle
      var validationErrors = new Dictionary<string, string[]>();

      if (string.IsNullOrWhiteSpace(request.Name))
        validationErrors["name"] = new[] { "Name is required" };

      if (request.Price <= 0)
        validationErrors["price"] = new[] { "Price must be greater than 0" };

      if (validationErrors.Any())
      {
        // Retourne un Validation Problem Details (RFC 9457)
        return Results.ValidationProblem(
            errors: validationErrors,
            title: "Validation failed",
            detail: "One or more validation errors occurred",
            type: ValidationErrorType
        );
      }

      var newProduct = new Product(
          products.Max(p => p.Id) + 1,
          request.Name,
          request.Price
      );

      products.Add(newProduct);

      return Results.Created($"/products/{newProduct.Id}", newProduct);
    })
    .WithName("CreateProduct")
    .WithOpenApi();

    // PUT /products/{id} - Mettre à jour un produit
    app.MapPut("/products/{id:int}", (int id, [FromBody] CreateProductRequest request) =>
    {
      var product = products.FirstOrDefault(p => p.Id == id);

      if (product == null)
      {
        return Results.Problem(
            title: "Product not found",
            detail: $"Cannot update. No product found with ID {id}",
            statusCode: StatusCodes.Status404NotFound,
            type: ProductNotFoundType
        );
      }

      var index = products.IndexOf(product);
      products[index] = new Product(id, request.Name, request.Price);

      return Results.Ok(products[index]);
    })
    .WithName("UpdateProduct")
    .WithOpenApi();

    // DELETE /products/{id} - Supprimer un produit
    app.MapDelete("/products/{id:int}", (int id) =>
    {
      var product = products.FirstOrDefault(p => p.Id == id);

      if (product == null)
      {
        return Results.Problem(
            title: "Product not found",
            detail: $"Cannot delete. No product found with ID {id}",
            statusCode: StatusCodes.Status404NotFound,
            type: ProductNotFoundType
        );
      }

      products.Remove(product);
      return Results.NoContent();
    })
    .WithName("DeleteProduct")
    .WithOpenApi();

    // Endpoint qui simule une erreur serveur
    app.MapGet("/products/error", () =>
    {
      throw new InvalidOperationException("Simulated server error");
    })
    .WithName("SimulateError")
    .WithOpenApi();

  }
}

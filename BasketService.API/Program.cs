using BasketService.API.Commands;
using BasketService.API.Domain.ValueTypes;
using BasketService.API.DTO.Request;
using BasketService.API.DTO.Response;
using BasketService.API.Repository.Implementations;
using BasketService.API.Repository.Interfaces;
using BasketService.API.Services.Interfaces;
using BasketService.API.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddHttpLogging();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// this doesn't work?
builder.Services.AddValidatorsFromAssemblyContaining<AddItemRequestValidator>();

builder.Services.AddSingleton<IBasketRepository, InMemoryBasketStore>();
builder.Services.AddScoped<IBasketService, BasketService.API.Services.Implementations.BasketService>();

var app = builder.Build();

app.UseHttpLogging();

// generic catch all for any unhandled exceptions, return 500 internal server error
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exceptionFeature = context.Features.Get<IExceptionHandlerFeature>();
        var exception = exceptionFeature?.Error;

        var logger = context.RequestServices
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger("GlobalExceptionHandler");

        if (exception is not null)
        {
            logger.LogError(
                exception,
                "Unhandled exception processing {Method} {Path}",
                context.Request.Method,
                context.Request.Path
            );
        }

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsJsonAsync(new
        {
            error = "An unexpected error occurred.",
            code = "INTERNAL_SERVER_ERROR"
        });
    });
});


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapPost("/baskets", async (CreateBasketRequest request, IBasketService basketService, IValidator<CreateBasketRequest> validator) =>
{
    var validation = await validator.ValidateAsync(request);
    if (!validation.IsValid)
    {
        var errors = validation.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

        return Results.ValidationProblem(errors);
    }
    
    var result = await basketService.CreateBasketAsync(CreateBasketCommand.FromRequest(request));

    return result.Match<IResult>(
        basket => Results.Created($"/baskets/{basket.Id.Value}", CreateBasketResponse.FromBasket(basket)),
        violation => Results.BadRequest(new { error = violation.Message })
    );
});

app.MapPost("/baskets/{basketId}/item", async (Guid basketId, AddItemRequest request, IBasketService basketService, IValidator<AddItemRequest> validator) =>
{
    var validation = await validator.ValidateAsync(request);
    if (!validation.IsValid)
    {
        var errors = validation.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

        return Results.ValidationProblem(errors);
    }
    
    var result = await basketService.AddItemAsync(BasketId.Create(basketId), AddItemCommand.FromRequest(request));
    
    return result.Match<IResult>(
        basket => Results.Ok(CreateBasketResponse.FromBasket(basket)),
        notFound => Results.NotFound(new { error = notFound.Message }), 
        violation => Results.BadRequest(new { error = violation.Message })
    );
});

app.MapPost("/baskets/{basketId}/items", async (Guid basketId, BatchAddItemRequest request, IBasketService basketService, IValidator<BatchAddItemRequest> validator) =>
{
    var validation = await validator.ValidateAsync(request);
    if (!validation.IsValid)
    {
        var errors = validation.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

        return Results.ValidationProblem(errors);
    }
    
    var result = await basketService.AddItemsAsync(BasketId.Create(basketId), AddItemsCommand.FromRequest(request));
    
    return result.Match<IResult>(
        basket => Results.Ok(CreateBasketResponse.FromBasket(basket)),
        notFound => Results.NotFound(new { error = notFound.Message }), 
        violation => Results.BadRequest(new { error = violation.Message })
    );
});

app.MapDelete("/baskets/{basketId}/items/{itemId}", async (Guid basketId, Guid itemId, IBasketService basketService) =>
{
    var result = await basketService.RemoveItemAsync(BasketId.Create(basketId), ItemId.Create(itemId));
    
    return result.Match<IResult>(
        basket => Results.Ok(CreateBasketResponse.FromBasket(basket)),
        notFound => Results.NotFound(new { error = notFound.Message }), 
        violation => Results.BadRequest(new { error = violation.Message })
    );
});

app.Run();

# BasketService.API

A simple REST API for managing a shopping basket (create basket, add items, remove items).
Persistence is in-memory and resets on restart.

---

## Prerequisites

- .NET 10 SDK
- Docker
- Git

---

## Run the app locally

From the solution root:

```
dotnet clean
dotnet restore
dotnet build
dotnet run --project BasketService.API
```

---

## Run the app in Docker

```
cd .\BasketService.API\
docker build -t basketservice-api -f Dockerfile .
docker run --rm -p 8080:8080 basketservice-api
```
The app will running on port 8080 http://localhost:8080

---

## Test API endpoints

### Using BasketService.http file

@BasketService_HostAddress = http://127.0.0.1:8080

Then send requests using your IDE HTTP client.

### cURL

#### Create basket
Example request
```
curl --location 'http://localhost:8080/baskets' \
--header 'Content-Type: application/json' \
--data '{}'
```
Example response
```
{
    "basket": {
        "basketId": "2854c77d-4851-4f42-b8b4-5fd62aa8cb7b",
        "items": []
    }
}
```
#### Add item
Example request
```
curl --location 'http://localhost:8080/baskets/ffb83701-0033-4b1e-997d-7058b9ec8bca/item' \
--header 'Content-Type: application/json' \
--data '{
    "productId" : "HAT"
}'
```
Example response
```
{
    "basket": {
        "basketId": "2854c77d-4851-4f42-b8b4-5fd62aa8cb7b",
        "items": [
            {
                "itemId": "1a56df9b-2a5d-49d4-8bc6-e8085534f23d",
                "productId": "HAT",
                "quantity": 1
            }
        ]
    }
}
```
#### Add multiple items
Example request
```
curl --location 'http://localhost:8080/baskets/2854c77d-4851-4f42-b8b4-5fd62aa8cb7b/items' \
--header 'Content-Type: application/json' \
--data '{
    "items": [
        {
            "productId": "HAT",
            "quantity": 20
        },
        {
            "productId": "SCARF"
        }
    ]
}'
```
Example response
```
{
    "basket": {
        "basketId": "2854c77d-4851-4f42-b8b4-5fd62aa8cb7b",
        "items": [
            {
                "itemId": "1a56df9b-2a5d-49d4-8bc6-e8085534f23d",
                "productId": "HAT",
                "quantity": 21
            },
            {
                "itemId": "7767f08c-4032-470d-9813-f16fe74bda62",
                "productId": "SCARF",
                "quantity": 1
            }
        ]
    }
}
```

#### Remove Item
Example request
```
curl --location --request DELETE 'http://localhost:8080/baskets/2854c77d-4851-4f42-b8b4-5fd62aa8cb7b/items/1a56df9b-2a5d-49d4-8bc6-e8085534f23d'
```
Example response
```
{
    "basket": {
        "basketId": "2854c77d-4851-4f42-b8b4-5fd62aa8cb7b",
        "items": [
            {
                "itemId": "7767f08c-4032-470d-9813-f16fe74bda62",
                "productId": "SCARF",
                "quantity": 1
            }
        ]
    }
}
```
---

## Running unit tests
```
dotnet test
```

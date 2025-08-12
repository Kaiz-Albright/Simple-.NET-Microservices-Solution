# PlatformService

Servicio encargado de gestionar las plataformas de software. Expone una API REST para crear y consultar plataformas y publica eventos cuando se agregan nuevas.

## Proyectos
- **PlatformService.Api**: API REST y servicio gRPC.
- **PlatformService.Application**: lógica de negocio y mapeos.
- **PlatformService.Domain**: entidades del dominio.
- **PlatformService.Infrastructure**: EF Core, clientes HTTP y RabbitMQ.

## Endpoints principales
- `GET /api/platforms`
- `GET /api/platforms/{id}`
- `POST /api/platforms`

Al crear una plataforma:
- Se envía una petición HTTP a CommandService.
- Se publica un evento `PlatformPublished` en RabbitMQ.
- El servicio gRPC `GrpcPlatform` permite a otros servicios consultar la lista de plataformas.

## Requisitos
- .NET 9 SDK
- RabbitMQ disponible en `localhost:5672`

En desarrollo utiliza una base de datos en memoria; en producción requiere una conexión SQL Server llamada `PlatformService`.

## Ejecución
```bash
dotnet run --project src/PlatformService/PlatformService.Api
```
El servicio escucha en `http://localhost:5028`.

## Prueba rápida
```bash
curl http://localhost:5028/api/platforms
```

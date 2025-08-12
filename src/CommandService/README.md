# CommandService

Servicio que recibe datos de plataformas y gestiona comandos asociados.

## Proyectos
- **CommandService.Api**: API REST.
- **CommandService.Application**: lógica de negocio.
- **CommandService.Domain**: entidades.
- **CommandService.Infrastructure**: EF Core, suscriptor de RabbitMQ y cliente gRPC.

## Comunicación
- Consume eventos de RabbitMQ emitidos por PlatformService.
- Obtiene la lista de plataformas desde PlatformService mediante gRPC.

## Endpoints REST
- `GET /api/c/platforms`
- `POST /api/c/platforms` (prueba de conectividad)
- `GET /api/c/platforms/{platformId}/commands`
- `GET /api/c/platforms/{platformId}/commands/{commandId}`
- `POST /api/c/platforms/{platformId}/commands`

## Requisitos
- .NET 9 SDK
- RabbitMQ disponible en `localhost:5672`

El servicio usa una base de datos en memoria.

## Ejecución
```bash
dotnet run --project src/CommandService/CommandService.Api
```
El servicio escucha en `http://localhost:5066`.

## Prueba rápida
```bash
curl http://localhost:5066/api/c/platforms
```

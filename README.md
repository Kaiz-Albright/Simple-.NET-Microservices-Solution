# Solución de Microservicios con .NET

Este repositorio recoge mi proceso de aprendizaje al construir una arquitectura de microservicios con **.NET 9**. Durante el desarrollo diseñé y codifiqué dos servicios independientes, **PlatformService** y **CommandService**, que colaboran mediante distintos mecanismos de comunicación.

## Lo que aprendí
- Diseñé un monorepo de 2 microservicios cada uno con su propia base de datos, comunicados de manera síncrona y asíncrona.
- Uso de principios SOLID y aplicación de Domain Driven Development.
- Implementé comunicación síncrona a través de HTTP y gRPC.
- Integré mensajería asíncrona con RabbitMQ para simular el caso de servicios desacoplados.
- Apliqué Entity Framework Core para persistencia InMemory y SQL Server.
- Escribí pruebas unitarias y de integración utilizando `dotnet test`.
- Me puse desde el punto de vista del puesto de DevOps, planificando despliegues usando Docker y orquestando con Kubernetes.

## Tecnologías principales
- .NET 9 y ASP.NET Core
- Entity Framework Core
- RabbitMQ
- gRPC
- AutoMapper
- Docker/Kubernetes (manifiestos en `deploy/`)

## Arquitectura del monorepo
Cada servicio se encuentra en `src/` y sigue el mismo diseño en capas:

```
src/
  PlatformService/
    PlatformService.Api           # API REST y gRPC
    PlatformService.Application   # Lógica de negocio
    PlatformService.Domain        # Entidades
    PlatformService.Infrastructure# Datos, mensajería, clientes

  CommandService/
    CommandService.Api
    CommandService.Application
    CommandService.Domain
    CommandService.Infrastructure
```

**Flujo de comunicación**
- PlatformService expone una API REST para gestionar plataformas.
- Al crear una plataforma envía la información a CommandService por HTTP y publica un evento en RabbitMQ.
- CommandService consume los eventos y usa gRPC para consultar plataformas en PlatformService.

## Requisitos previos
- [SDK de .NET 9](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/) (para levantar RabbitMQ)

## Puesta en marcha
1. Levanta RabbitMQ:
   ```bash
   docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3-management
   ```
2. En terminales separadas ejecuta cada servicio:
   ```bash
   dotnet run --project src/PlatformService/PlatformService.Api    # puerto 5028
   dotnet run --project src/CommandService/CommandService.Api      # puerto 5066
   ```

## Pruebas manuales
```bash
# Listar plataformas
curl http://localhost:5028/api/platforms

# Crear plataforma
curl -X POST http://localhost:5028/api/platforms \
  -H "Content-Type: application/json" \
  -d '{"name":"Demo","publisher":"Acme","cost":"Free"}'

# Consultar plataformas desde CommandService
curl http://localhost:5066/api/c/platforms
```

## Tests automatizados
Ejecuta todos los tests con:
```bash
dotnet test
```

Los archivos `.http` dentro de cada servicio permiten realizar peticiones desde el editor. Los manifiestos de Kubernetes se encuentran en `deploy/K8S`.


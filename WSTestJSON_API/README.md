WSTestJSON_API — Proyecto personal de aprendizaje

Sobre el proyecto
Este repositorio recoge mi trabajo de aprendizaje en desarrollo backend con .NET 10. 
Implementé una REST API para gestión de usuarios usando Entity Framework Core y patrones básicos de arquitectura: separación de responsabilidades, uso de DTOs y buenas prácticas en diseño de endpoints.

Qué muestra mi trabajo
- Endpoints REST claros y coherentes (GET, POST, PUT, DELETE).
- Uso de DTOs para separar modelos de persistencia y contratos públicos.
- Separación de responsabilidades: controladores, servicios/repositories y capa de acceso a datos.
- Integración con EF Core para persistencia y soporte para migraciones.
- Atención a validación de entrada, manejo de errores y preparación para pruebas automatizadas.

Stack técnico
- Plataforma: .NET 10 (C#)
- ORM: Entity Framework Core
- Dependencias: NuGet
- Entorno recomendado: Visual Studio 2026 o dotnet CLI

Cómo ejecutar localmente
1) Requisitos: .NET 10 SDK y Visual Studio 2026 o dotnet CLI.
2) Abrir la solución: dotnet sln WSTestJSON_API.slnx o abrir en Visual Studio.
3) Restaurar y compilar: dotnet restore && dotnet build
4) Ejecutar la API: dotnet run --project <ruta-al-proyecto-api>
5) Probar endpoints con Postman contra http://localhost:<puerto> (el puerto se muestra al arrancar).

Pruebas
- Pendientes! estoy enfocado actualmente en Angular 22

Qué valorar si revisas este repo
- Claridad en la separación entre DTOs, entidades y capas de servicio.
- Correcto uso de EF Core y migraciones.
- Manejo de errores, validaciones y respuestas HTTP consistentes.
- Modularidad e inyección de dependencias que facilitan extensibilidad.

Contacto y repositorio
Repositorio: https://github.com/albertovaldez434-svg/usuarios-api-test

Nota final
Proyecto orientado a aprendizaje práctico en .NET 10 y EF Core.
# StarkInventorySystem
Es un proyecto de inventario y manejo de órdenes que utiliza buenas prácticas, y arquitectura limpia.

## 🎯 Objetivos
Los objetivos **iniciales** del proyecto son:
- Arquitecture First - Separación de preocupaciones
- SOLID - Decisiones de diseño explicadas
- TDD - Desarrollo basado en pruebas
- Patrones de diseño para los casos de uso que los requieran (Repository, Unit of Work, CQRS)
- Código listo para producción - Manejo de errores, logging, validaciones

Las características que se espera abarcar son las siguientes:
- Gestión de inventario: Seguimiento de productos, niveles de stock y almacenes
- Procesamiento de pedidos: Creación de pedidos, validación de stock, reserva de inventario
- Gestión de proveedores: Automatización de reabastecimiento, relaciones con proveedores
- Notificaciones: Alertas de stock bajo, confirmaciones de pedidos
- Informes: Análisis de inventario, historial de pedidos
  
## 📋 Tecnologías a usar

- .NET
- SQL Server con EF Core
- NUnit/xUnit para testing
- Mediador propio (CQRS pattern)
- FluentValidation
- Serilog (structured logging)

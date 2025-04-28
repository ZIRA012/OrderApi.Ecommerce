# Order API - ECommerce Project

Esta API forma parte del sistema de backend de un ECommerce, encargándose de gestionar los pedidos (órdenes) realizados por los usuarios.

## Funcionalidades principales

- Registrar nuevas órdenes de compra.
- Consultar órdenes existentes.
- Actualizar detalles de órdenes.
- Eliminar órdenes.
- Verificar la existencia y stock de productos consultando la **Product API**.

## Tecnologías utilizadas

- ASP.NET Core
- Entity Framework Core
- Clean Architecture (DTOs, Entities, Repository, Application)
- JWT Bearer Authentication
- Comunicación entre APIs mediante HttpClient
- FakeItEasy para pruebas unitarias
- FluentAssertions para validaciones en pruebas

## Librerías compartidas necesarias

Este proyecto depende de librerías compartidas que contienen configuraciones, respuestas estándar, excepciones y utilidades comunes para todas las APIs del sistema.

Por favor, clona también el siguiente repositorio:

```bash
git clone https://github.com/ZIRA012/ECommerceLibreriasCompartidas.git

# 🚀 Engine de Simulación - Backend API

TRABAJAR SOBRE LA RAMA DEVELOPMENT

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL_Server-CC2927?style=for-the-badge&logo=microsoft-sql-server&logoColor=white)
![Swagger](https://img.shields.io/badge/Swagger-85EA2D?style=for-the-badge&logo=swagger&logoColor=black)

## 📌 Sobre el Proyecto

API RESTful desarrollada para el motor de simulación. Diseñada con una arquitectura de capas monolítica enfocada en el alto rendimiento, el procesamiento de datos complejos y la trazabilidad de los modelos (ideal para flujos logísticos o de recursos).

El proyecto separa de forma estricta las responsabilidades, garantizando que la lógica matemática y de negocio de la simulación se mantenga independiente de la infraestructura de la base de datos.

---

## 🏗️ Arquitectura y Estructura

El proyecto está organizado en las siguientes capas lógicas dentro de la solución:

- 📁 **Controllers:** Endpoints HTTP y enrutamiento de la API.
- 📁 **Services:** Lógica de negocio, orquestación de la simulación y cálculos pesados.
- 📁 **Models:** Entidades de dominio y estructuras de datos puras.
- 📁 **Data:** Contexto de base de datos (`DbContext`) y configuraciones de Entity Framework Core.
- 📁 **DTOs:** Objetos de transferencia de datos para aislar los modelos del exterior.

---

## ⚙️ Requisitos Previos

Asegurate de tener instalado lo siguiente en tu entorno local antes de levantar el proyecto:

* [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) o superior.
* [Visual Studio 2022](https://visualstudio.microsoft.com/) (Recomendado) o VS Code.
* SQL Server (LocalDB o instancia de desarrollo).

---

## 🚀 Inicialización Rápida

Seguí estos pasos para levantar el backend en tu máquina:

**1. Clonar el repositorio**
```bash
git clone []
cd simulacion-backend

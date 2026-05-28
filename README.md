# 🚀 Engine de Simulación - Backend API

> **⚠️ ATENCIÓN:** TRABAJAR SOBRE LA RAMA DEVELOPMENT.

![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)
![SQLite](https://img.shields.io/badge/SQLite-003B57?style=for-the-badge&logo=sqlite&logoColor=white)
![Swagger](https://img.shields.io/badge/Swagger-85EA2D?style=for-the-badge&logo=swagger&logoColor=black)

## 📌 Sobre el Proyecto

API RESTful desarrollada para el motor de simulación. Diseñada con una arquitectura de capas enfocada en el alto rendimiento, el procesamiento de datos complejos y la trazabilidad de los modelos estocásticos (ideal para flujos logísticos o de recursos electrónicos).

El proyecto separa de forma estricta las responsabilidades, garantizando que la lógica matemática y de negocio de la simulación se mantenga independiente de la infraestructura de la base de datos.

---

## 🏗️ Arquitectura y Estructura

El proyecto está organizado en las siguientes capas lógicas dentro de la solución:

- 📁 **Controllers:** Endpoints HTTP y enrutamiento de la API.
- 📁 **Services:** Lógica de negocio, orquestación de la simulación y cálculos pesados.
- 📁 **Generadores:** Implementación de algoritmos matemáticos (Congruencial Mixto y distribuciones).
- 📁 **Data:** Entidades de dominio, contexto de base de datos (`AppDbContext`) y persistencia con Entity Framework Core.
- 📁 **DTOs:** Objetos de transferencia de datos para aislar el motor del exterior y comunicar con el frontend.

---

## ⚙️ Requisitos Previos

Asegurate de tener instalado lo siguiente en tu entorno local antes de levantar el proyecto:

* [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) o superior.
* [Visual Studio 2022](https://visualstudio.microsoft.com/) (Recomendado) o VS Code.
* Motor de SQLite (integrado, no requiere instalación de servidor externo).

---

## 🚀 Inicialización Rápida

Seguí estos pasos para levantar el backend en tu máquina:

**1. Clonar el repositorio e ingresar a la carpeta**
```bash
git clone [https://github.com/LEBianchi/SimulacionBackend.git](https://github.com/LEBianchi/SimulacionBackend.git)
cd simulacion-backend
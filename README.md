# 🏨 Hotel Management API

## 📖 Descripción del Proyecto

Este proyecto consiste en el desarrollo de una **API RESTful** robusta diseñada para la gestión integral de operaciones hoteleras. Su propósito principal es servir como el núcleo lógico que conecta la interfaz de usuario (Frontend) con la base de datos, asegurando la integridad de los datos y aplicando las reglas de negocio del hotel.

El sistema permite automatizar procesos clave como el registro de huéspedes, la administración del inventario de habitaciones y el ciclo de vida completo de una reserva, desde la solicitud inicial hasta el check-out.

### Funcionalidades Principales

  * **Gestión de Identidad:** Sistema seguro de registro e inicio de sesión utilizando estándares modernos de autenticación.
  * **Inventario de Habitaciones:** Control total sobre el estado de las habitaciones (disponible, ocupada, mantenimiento), permitiendo actualizaciones en tiempo real.
  * **Motor de Reservas:** Lógica avanzada para evitar solapamiento de fechas, cálculo automático de costos y transición de estados de reserva.
  * **Precios Dinámicos:** Capacidad para gestionar diferentes tipos de habitaciones con precios base configurables.

### 🔌 Endpoints Implementados y su Relevancia

El sistema expone los siguientes recursos clave para la operación del hotel:

1.  **`/api/usuarios` (Autenticación y Perfil)**

      * **Relevancia:** Es la puerta de entrada al sistema. Garantiza que solo usuarios registrados (Clientes) y personal autorizado (Admin/Empleados) puedan acceder a las funciones protegidas mediante **JWT**.
      * **Endpoints:** Registro, Login, Perfil de Usuario.

2.  **`/api/habitaciones` (Inventario)**

      * **Relevancia:** Permite a los administradores mantener el catálogo de habitaciones actualizado y a los clientes consultar la disponibilidad. Es vital para evitar sobreventas.
      * **Endpoints:** Listar (público), Crear, Editar y Ver Detalle.

3.  **`/api/reservas` (Operaciones)**

      * **Relevancia:** Es el núcleo del negocio. Maneja la lógica compleja de fechas y estados. Incluye endpoints críticos para el personal, como `confirmar` (Check-in), que cambia automáticamente el estado de la habitación a "Ocupada".
      * **Endpoints:** Crear Reserva, Historial, Confirmar Reserva, Cancelar Reserva.

-----

## 🚀 Instrucciones para Ejecutar el Proyecto

Sigue estos pasos para desplegar el backend en tu entorno local.

### 1\. Requerimientos del Sistema

  * **Lenguaje:** C\# / .NET 8 SDK.
  * **Base de Datos:** SQL Server 2022 (Ejecutándose en Docker).
  * **Herramientas:** Visual Studio 2022 / VS Code, Docker Desktop, Postman.

### 2\. Configuración Inicial (Base de Datos y Variables)

Este proyecto utiliza **Docker** para la base de datos. No necesitas instalar SQL Server localmente.

1.  **Levantar el contenedor de Base de Datos:**
    Abre tu terminal y ejecuta el siguiente comando:

    ```bash
    docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=Admin12345" -p 1433:1433 --name sqlhotel -d mcr.microsoft.com/mssql/server:2022-latest
    ```

2.  **Configurar Variables de Conexión (`appsettings.json`):**
    Asegúrate de que el archivo `appsettings.json` en la raíz del proyecto tenga la cadena de conexión apuntando a tu contenedor Docker y una clave segura para JWT:

    ```json
    {
        "ConnectionStrings": {
        "DefaultConnection": "Server=localhost,1433;Database=HotelDB;User=sa;Password=Admin12345;TrustServerCertificate=True"
        },
    "   Jwt": {
            "Key": "cualquiera",
            "Issuer": "ProjectHotel",
            "Audience": "ProjectHotel"
        }
    }
    ```

### 3\. Instalación de Dependencias

Abre una terminal en la carpeta raíz del proyecto (`/Hotel.API`) y ejecuta:

```bash
dotnet restore
```

*Esto descargará paquetes necesarios como EntityFrameworkCore, FluentValidation y BCrypt.*

### 4\. Crear la Base de Datos (Migraciones)

Para crear las tablas en tu contenedor de SQL Server automáticamente, ejecuta:

```bash
dotnet ef database update
```

### 5\. Comandos para Iniciar el Proyecto

Para levantar el servidor de desarrollo:

```bash
dotnet run
```

La API estará disponible en: `http://localhost:5053` (o el puerto que indique tu consola).

-----

## 📮 Colección de Postman

Para facilitar las pruebas y la evaluación de los endpoints, se incluye una colección completa de Postman.

  * **Ubicación del archivo:** `docs/Hotel_API_Collection.json` (o en la raíz del repositorio).
  * **Contenido:**
      * Ejemplos de **Requests** completos (Body, Headers).
      * Ejemplos de **Responses** exitosos y de error.
      * Organización por carpetas (Auth, Habitaciones, Reservas).
      * Configuración de variables de entorno (BaseURL, Token).

**Pasos para importar:**

1.  Abre Postman.
2.  Haz clic en "Import".
3.  Arrastra el archivo `.json` incluido en este repositorio.

-----

### ⚠️ Consideraciones Especiales

  * **Primer Usuario (Admin):** Al iniciar la base de datos está vacía. Se recomienda registrar un usuario y cambiar su rol manualmente en la base de datos o usar el endpoint de registro (el cual crea usuarios con rol "cliente" por defecto).
  * **Validaciones:** Las contraseñas deben ser fuertes (Mayúscula, minúscula, número y símbolo) debido a las reglas de seguridad implementadas.
  * **Formato de Fechas:** Las fechas deben enviarse en formato ISO 8601 (`YYYY-MM-DD`).

-----


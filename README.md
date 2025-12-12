# 🏨 Hotel Management API

## 📖 Descripción del Proyecto

Este proyecto es una **API RESTful** desarrollada con **.NET 8** que permite la gestión integral de un sistema hotelero. Su propósito es servir como el núcleo de negocio que conecta el frontend con la base de datos, permitiendo una administración segura y eficiente de huéspedes, habitaciones, reservas y temporadas de precios.

La API aplica reglas de negocio para garantizar la integridad de los datos, validaciones consistentes mediante **FluentValidation** y un sistema de autenticación moderna basado en **JWT**.

### Funcionalidades principales

* Autenticación y autorización de usuarios mediante tokens JWT.
* Gestión de habitaciones (registro, edición, consulta).
* Registro y administración de huéspedes.
* Creación y control de reservas.
* Administración de temporadas con precios dinámicos.
* Carga de imágenes para habitaciones.

---

## 🔌 Endpoints Implementados y su Relevancia

### `/api/usuarios`

Permite el registro e inicio de sesión de usuarios, protegiendo el acceso mediante JWT.

### `/api/habitaciones`

Administra el inventario de habitaciones, asegurando disponibilidad en tiempo real.

### `/api/Huesped`

Gestiona el registro de huéspedes, quienes son los responsables de las habitaciones.

### `/api/reservas`

Controla el ciclo completo de vida de las reservas.

### `/api/TemporadaPrecio`

Permite crear temporadas de precios especiales.

### `/api/TemporadaHabitacionPrecio`

Administra el precio de habitaciones por temporada.

---

## 📌 Endpoints Importantes

### Crear una nueva habitación

* **Ruta:** `http://localhost:5053/api/habitaciones`
* **Método:** `POST`
* **Body:** `form-data`
* **Headers:** `Authorization: Bearer {TOKEN}`

### Registrar huésped

* **Ruta:** `http://localhost:5053/api/Huesped`
* **Método:** `POST`

### Crear reserva

* **Ruta:** `http://localhost:5053/api/reservas`
* **Método:** `POST`

### Registrar temporada de precios

* **Ruta:** `http://localhost:5053/api/TemporadaPrecio`
* **Método:** `POST`

### Registrar precio de habitación por temporada

* **Ruta:** `http://localhost:5053/api/TemporadaHabitacionPrecio`
* **Método:** `POST`

---

## 🚀 Instrucciones para Ejecutar el Proyecto

### Requerimientos del Sistema

* .NET SDK 8.0
* Docker Desktop
* SQL Server 2022 (mediante Docker)
* Visual Studio 2022 o VS Code
* Postman

---

### Instalación del Proyecto

Clonar el repositorio:

```bash
git clone https://github.com/tu-usuario/tu-repositorio.git
cd Hotel.API
```

Instalar dependencias:

```bash
dotnet restore
```

---

### Configuración Inicial

Levantar SQL Server en Docker:

```bash
docker pull mcr.microsoft.com/mssql/server:2022-latest

docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=Admin12345" -p 1433:1433 --name sqlhotel -d mcr.microsoft.com/mssql/server:2022-latest
```

Crear la base de datos manualmente:

```
HotelDB
```

Configurar `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=HotelDB;User=sa;Password=Admin12345;TrustServerCertificate=True"
  },
  "Jwt": {
    "Key": "cualquiera",
    "Issuer": "ProjectHotel",
    "Audience": "ProjectHotel"
  }
}
```

---

### Crear Base de Datos (Migraciones)

```bash
dotnet tool install --global dotnet-ef
dotnet ef database update
```

---

### Comandos para Iniciar el Proyecto

```bash
dotnet run
```

La API se ejecutará en:

```
http://localhost:5053
```
---

## 📮 Colección de Postman

La colección de Postman incluye todos los endpoints implementados y ejemplos de requests completos.

Ubicación del archivo:

```
docs/Proyecto_Surface_Hotel.postman_collection.json
```

Pasos para usarla:

1. Abrir Postman.
2. Seleccionar **Import**.
3. Cargar el archivo `.json`.

---

## 🎥 Video Demostrativo del Proyecto

Enlace al video de demostración en YouTube:

```
https://youtu.be/SF92FcJxTXo
```

---

## ⚠️ Consideraciones Especiales

* Las contraseñas deben cumplir con reglas de seguridad fuertes.
* Las fechas deben enviarse en formato ISO 8601: `YYYY-MM-DD`.



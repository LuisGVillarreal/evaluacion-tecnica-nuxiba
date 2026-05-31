# EVALUACIÓN TÉCNICA NUXIBA

Prueba: **DESARROLLADOR JR**

Deadline: **1 día**

Nombre: **Luis Gustavo Villarreal Cigarroa**

### Documentación de la Solución

| Sección                                                              | Descripción                                    |
| -------------------------------------------------------------------- | ---------------------------------------------- |
| [Requisitos Previos](#requisitos-previos)                            | .NET 8, Docker y herramientas necesarias       |
| [Estructura del Proyecto](#estructura-del-proyecto)                  | Árbol de directorios y archivos principales    |
| [Cómo Levantar el Proyecto](#cómo-levantar-el-proyecto)              | Docker, migraciones, seed data y ejecución     |
| [Ejercicio 1 — Endpoints del CRUD](#ejercicio-1--endpoints-del-crud) | API REST: GET, POST, PUT, DELETE de logins     |
| [Ejercicio 2 — Consultas SQL](#ejercicio-2--consultas-sql)           | Scripts SQL de análisis de tiempo logueado     |
| [Ejercicio 3 — Generación de CSV](#ejercicio-3--generación-de-csv)   | Endpoint para descargar reporte CSV            |
| [Pruebas Unitarias](#pruebas-unitarias)                              | 15 tests con xUnit (sin dependencia de Docker) |
| [Tecnologías Utilizadas](#tecnologías-utilizadas)                    | Stack tecnológico del proyecto                 |

---

## Clona y crea tu repositorio para la evaluación

1. Clona este repositorio en tu máquina local.
2. Crea un repositorio público en tu cuenta personal de GitHub, BitBucket o Gitlab.
3. Cambia el origen remoto para que apunte al repositorio público que acabas de crear en tu cuenta.
4. Coloca tu nombre en este archivo README.md y realiza un push al repositorio remoto.

---

## Instrucciones Generales

1. Cada pregunta tiene un valor asignado. Asegúrate de explicar tus respuestas y mostrar las consultas o procedimientos que utilizaste.
2. Se evaluará la claridad de las explicaciones, el pensamiento crítico, y la eficiencia de las consultas.
3. Utiliza **SQL Server** para realizar todas las pruebas y asegúrate de que las consultas funcionen correctamente antes de entregar.
4. Justifica tu enfoque cuando encuentres una pregunta sin una única respuesta correcta.
5. Configura un Contenedor de **SQL Server con Docker** utilizando los siguientes pasos:

### Pasos para ejecutar el contenedor de SQL Server

Asegúrate de tener Docker instalado y corriendo en tu máquina. Luego, ejecuta el siguiente comando para levantar un contenedor con SQL Server:

```bash
docker run -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=YourStrong!Passw0rd'    -p 1433:1433 --name sqlserver -d mcr.microsoft.com/mssql/server:2019-latest
```

6. Conéctate al servidor de SQL con cualquier herramienta como **SQL Server Management Studio** o **Azure Data Studio** utilizando las siguientes credenciales:
   - **Servidor**: localhost, puerto 1433
   - **Usuario**: sa
   - **Contraseña**: YourStrong!Passw0rd

---

# Examen Práctico para Desarrollador Junior en .NET 8 y SQL Server

**Tiempo estimado:** 1 día  
**Total de puntos:** 100

---

## Instrucciones Generales:

El examen está compuesto por tres ejercicios prácticos. Sigue las indicaciones en cada uno y asegúrate de entregar el código limpio y funcional.

Además, se proporciona un archivo **CCenterRIA.xlsx** para que te bases en la estructura de las tablas y datos proporcionados.

[Descargar archivo de ejemplo](CCenterRIA.xlsx)

---

## Ejercicio 1: API RESTful con ASP.NET Core y Entity Framework (40 puntos)

**Instrucciones:**  
Desarrolla una API RESTful con ASP.NET Core y Entity Framework que permita gestionar el acceso de usuarios.

1. **Creación de endpoints**:
   - **GET /logins**: Devuelve todos los registros de logins y logouts de la tabla `ccloglogin`. (5 puntos)
   - **POST /logins**: Permite registrar un nuevo login/logout. (5 puntos)
   - **PUT /logins/{id}**: Permite actualizar un registro de login/logout. (5 puntos)
   - **DELETE /logins/{id}**: Elimina un registro de login/logout. (5 puntos)

2. **Modelo de la entidad**:  
   Crea el modelo `Login` basado en los datos de la tabla `ccloglogin`:
   - `User_id` (int)
   - `Extension` (int)
   - `TipoMov` (int) → 1 es login, 0 es logout
   - `fecha` (datetime)

3. **Base de datos**:  
   Utiliza **Entity Framework Core** para crear la tabla en una base de datos SQL Server basada en este modelo. Aplica migraciones para crear la tabla en la base de datos. (10 puntos)

4. **Validaciones**:  
   Implementa las validaciones necesarias para asegurar que las fechas sean válidas y que el `User_id` esté presente en la tabla `ccUsers`. Además, maneja errores como intentar registrar un login sin un logout anterior. (10 puntos)

5. **Pruebas Unitarias** (Opcional):  
   Se valorará si incluyes pruebas unitarias para los endpoints de tu API utilizando un framework como **xUnit** o **NUnit**. (Puntos extra)

---

## Ejercicio 2: Consultas SQL y Optimización (30 puntos)

**Instrucciones:**

Trabaja en SQL Server y realiza las siguientes consultas basadas en la tabla `ccloglogin`:

1. **Consulta del usuario que más tiempo ha estado logueado** (10 puntos):
   - Escribe una consulta que devuelva el usuario que ha pasado más tiempo logueado. Para calcular el tiempo de logueo, empareja cada "login" (TipoMov = 1) con su correspondiente "logout" (TipoMov = 0) y suma el tiempo total por usuario.

   Ejemplo de respuesta:
   - `User_id`: 92
   - Tiempo total: 361 días, 12 horas, 51 minutos, 8 segundos

2. **Consulta del usuario que menos tiempo ha estado logueado** (10 puntos):
   - Escribe una consulta similar a la anterior, pero que devuelva el usuario que ha pasado menos tiempo logueado.

   Ejemplo de respuesta:
   - `User_id`: 90
   - Tiempo total: 244 días, 43 minutos, 15 segundos

3. **Promedio de logueo por mes** (10 puntos):
   - Escribe una consulta que calcule el tiempo promedio de logueo por usuario en cada mes.

   Ejemplo de respuesta:
   - Usuario 70 en enero 2023: 3 días, 14 horas, 1 minuto, 16 segundos

---

## Ejercicio 3: API RESTful para generación de CSV (30 puntos)

**Instrucciones:**

1. **Generación de CSV**:  
   Crea un endpoint adicional en tu API que permita generar un archivo CSV con los siguientes datos:
   - Nombre de usuario (`Login` de la tabla `ccUsers`)
   - Nombre completo (combinación de `Nombres`, `ApellidoPaterno`, y `ApellidoMaterno` de la tabla `ccUsers`)
   - Área (tomado de la tabla `ccRIACat_Areas`)
   - Total de horas trabajadas (basado en los registros de login y logout de la tabla `ccloglogin`)

   El CSV debe calcular el total de horas trabajadas por usuario sumando el tiempo entre logins y logouts.

2. **Formato y Entrega**:
   - El CSV debe ser descargable a través del endpoint de la API.
   - Asegúrate de probar este endpoint utilizando herramientas como **Postman** o **curl** y documenta los pasos en el archivo README.md.

---

## Entrega

1. Sube tu código a un repositorio en GitHub o Bitbucket y proporciona el enlace para revisión.
2. El repositorio debe contener las instrucciones necesarias en el archivo **README.md** para:
   - Levantar el contenedor de SQL Server.
   - Conectar la base de datos.
   - Ejecutar la API y sus endpoints.
   - Descargar el CSV generado.
3. **Opcional**: Si incluiste pruebas unitarias, indica en el README cómo ejecutarlas.

---

Este examen evalúa tu capacidad para desarrollar APIs RESTful, realizar consultas avanzadas en SQL Server y generar reportes en formato CSV. Se valorará la organización del código, las mejores prácticas y cualquier documentación adicional que proporciones.

---

# Documentación de la Solución

## Requisitos Previos

- **.NET 8 SDK** instalado
- **Docker** con SQL Server corriendo en `localhost:1433`
- **Entity Framework Core Tools** (`dotnet tool install --global dotnet-ef`)

---

## Estructura del Proyecto

```
evaluacion-tecnica-nuxiba/
├── NuxibaAPI/                    # Proyecto principal de la API
│   ├── Controllers/
│   │   ├── LoginsController.cs   # CRUD de logins (Ejercicio 1)
│   │   └── ReportsController.cs  # Generación de CSV (Ejercicio 3)
│   ├── Data/
│   │   └── AppDbContext.cs       # Contexto de Entity Framework
│   ├── DTOs/
│   │   ├── LoginCreateDto.cs     # DTO para crear registros
│   │   └── LoginUpdateDto.cs     # DTO para actualizar registros
│   ├── Models/
│   │   ├── Login.cs              # Modelo de la tabla ccloglogin
│   │   ├── User.cs               # Modelo de la tabla ccUsers
│   │   └── Area.cs               # Modelo de la tabla ccRIACat_Areas
│   ├── Migrations/               # Migraciones de Entity Framework
│   ├── Program.cs                # Configuración de la aplicación
│   └── appsettings.json          # Cadena de conexión a SQL Server
├── NuxibaAPI.Tests/              # Pruebas unitarias con xUnit
│   ├── LoginsControllerTests.cs  # 10 tests del CRUD
│   └── ReportsControllerTests.cs # 5 tests del CSV
└── SQL/                          # Scripts SQL
    ├── 02_seed_data.sql          # Datos de prueba
    ├── 03_usuario_mas_tiempo.sql # Ejercicio 2.1
    ├── 04_usuario_menos_tiempo.sql # Ejercicio 2.2
    └── 05_promedio_por_mes.sql   # Ejercicio 2.3
```

---

## Cómo Levantar el Proyecto

### 1. Levantar SQL Server con Docker

```bash
docker run -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=YourStrong!Passw0rd' -p 1433:1433 --name sqlserver -d mcr.microsoft.com/mssql/server:2019-latest
```

### 2. Aplicar las migraciones para crear la base de datos

```bash
cd NuxibaAPI
dotnet ef migrations add InitialCreate
dotnet ef database update
```

Esto crea la base de datos `NuxibaDB` con las tablas:

- `ccRIACat_Areas` — Áreas
- `ccUsers` — Usuarios
- `ccloglogin` — Registros de login/logout

### 3. Insertar datos de prueba

Ejecutar el script `SQL/02_seed_data.sql` en SSMS o Azure Data Studio.

### 4. Ejecutar la API

```bash
cd NuxibaAPI
dotnet run
```

La API se levanta en `https://localhost:44357` (o el puerto configurado en `launchSettings.json`).

---

## Ejercicio 1 — Endpoints del CRUD

### GET /api/logins — Obtener todos los registros

```bash
curl -k https://localhost:44357/api/logins
```

Respuesta:

```json
[
  {
    "id": 1,
    "user_id": 92,
    "extension": 100,
    "tipoMov": 1,
    "fecha": "2023-01-02T08:00:00",
    "usuario": "crodriguez"
  }
]
```

### POST /api/logins — Crear un registro

```bash
curl -k -X POST https://localhost:44357/api/logins \
  -H "Content-Type: application/json" \
  -d '{"user_id":92,"extension":100,"tipoMov":0,"fecha":"2023-03-01T08:00:00"}'
```

**Validaciones implementadas:**

- Se verifica que el `User_id` exista en la tabla `ccUsers`
- Se verifica que la fecha no sea futura
- Se verifica que no se registre un login sin un logout anterior

### PUT /api/logins/{id} — Actualizar un registro

```bash
curl -k -X PUT https://localhost:44357/api/logins/1 \
  -H "Content-Type: application/json" \
  -d '{"user_id":92,"extension":200,"tipoMov":1,"fecha":"2023-01-02T08:00:00"}'
```

### DELETE /api/logins/{id} — Eliminar un registro

```bash
curl -k -X DELETE https://localhost:44357/api/logins/1
```

Retorna HTTP 204 No Content si se eliminó correctamente.

---

## Ejercicio 2 — Consultas SQL

Los scripts se encuentran en la carpeta `SQL/`:

| Script                        | Descripción                            |
| ----------------------------- | -------------------------------------- |
| `03_usuario_mas_tiempo.sql`   | Usuario con más tiempo logueado        |
| `04_usuario_menos_tiempo.sql` | Usuario con menos tiempo logueado      |
| `05_promedio_por_mes.sql`     | Promedio de logueo por usuario por mes |

**Estrategia utilizada:**

1. Se usa `ROW_NUMBER()` para numerar los logins y logouts por separado para cada usuario
2. Se empareja cada login con su logout correspondiente usando `INNER JOIN`
3. Se calcula la diferencia en segundos con `DATEDIFF(SECOND, ...)`
4. Se formatea el resultado a días, horas, minutos y segundos

Abrir cada archivo en SSMS o Azure Data Studio conectarse a la base de datos y ejecutar.

---

## Ejercicio 3 — Generación de CSV

### GET /api/reports/csv — Descargar el reporte

```bash
curl -k -o reporte_horas.csv https://localhost:44357/api/reports/csv
```

El archivo CSV generado contiene:

```csv
NombreUsuario,NombreCompleto,Area,TotalHorasTrabajadas
jperez,Juan Perez Lopez,BBVA,41.50
mgarcia,Maria Garcia Martinez,Banamex,11.50
crodriguez,Carlos Rodriguez Sanchez,Default,37.75
```

**Lógica del cálculo:**

- Se obtienen todos los usuarios con sus logins y su área
- Se separan los logins (TipoMov=1) y logouts (TipoMov=0) ordenados por fecha
- Se emparejan en orden cronológico y se suma la diferencia de tiempo en horas

---

## Pruebas Unitarias

Se implementaron 15 pruebas unitarias con **xUnit** usando **InMemoryDatabase** (no requieren SQL Server ni Docker).

### Cómo ejecutar los tests

```bash
cd NuxibaAPI.Tests
dotnet test --verbosity normal
```

### Tests del LoginsController (10 tests)

| Test                                                       | Qué valida                           |
| ---------------------------------------------------------- | ------------------------------------ |
| `GetAll_DebeRetornarListaVacia_CuandoNoHayRegistros`       | GET retorna 200 sin datos            |
| `GetAll_DebeRetornarRegistros_CuandoExistenLogins`         | GET retorna 200 con datos            |
| `Create_DebeCrearLogin_CuandoDatosValidos`                 | POST crea registro y retorna 201     |
| `Create_DebeRetornarBadRequest_CuandoUserIdNoExiste`       | POST rechaza User_id inexistente     |
| `Create_DebeRetornarBadRequest_CuandoFechaEsFutura`        | POST rechaza fecha futura            |
| `Create_DebeRetornarBadRequest_CuandoLoginSinLogoutPrevio` | POST rechaza login sin logout previo |
| `Update_DebeActualizar_CuandoDatosValidos`                 | PUT actualiza y retorna 200          |
| `Update_DebeRetornarNotFound_CuandoIdNoExiste`             | PUT retorna 404 si no existe         |
| `Delete_DebeEliminar_CuandoIdExiste`                       | DELETE elimina y retorna 204         |
| `Delete_DebeRetornarNotFound_CuandoIdNoExiste`             | DELETE retorna 404 si no existe      |

### Tests del ReportsController (5 tests)

| Test                                                  | Qué valida                                 |
| ----------------------------------------------------- | ------------------------------------------ |
| `DownloadCsv_DebeRetornarArchivoCsv`                  | Retorna archivo con content-type text/csv  |
| `DownloadCsv_DebeContenerEncabezados`                 | El CSV tiene los encabezados correctos     |
| `DownloadCsv_DebeContenerDatosDelUsuario`             | El CSV contiene nombre, apellido y área    |
| `DownloadCsv_DebeCalcularHorasCorrectamente`          | Las horas calculadas son correctas (16.00) |
| `DownloadCsv_DebeRetornarCeroHoras_CuandoNoHayLogins` | Usuarios sin logins muestran 0.00 horas    |

---

## Tecnologías Utilizadas

- **ASP.NET Core 8** — Framework para la API
- **Entity Framework Core 8** — ORM para acceso a datos
- **SQL Server 2019** — Base de datos (en Docker)
- **xUnit** — Framework de pruebas unitarias

# DataVision API

Una API REST desarrollada en .NET 9 que proporciona autenticación JWT y acceso a datos de precios de combustibles de RECOPE Costa Rica.

## 📋 Características

- ✅ Autenticación JWT
- ✅ Registro de usuarios
- ✅ Logging automático de consultas
- ✅ Integración con API de RECOPE
- ✅ Base de datos SQL Server
- ✅ Documentación con Scalar OpenAPI

## 🔧 Prerrequisitos

- .NET 9 SDK
- SQL Server (LocalDB, Express, o instancia completa)
- Visual Studio 2022 o VS Code
- Postman (opcional, para pruebas)

## ⚙️ Configuración Inicial

### 1. Clonar el Repositorio

```bash
git clone <url-del-repositorio>
cd DataVision
```

### 2. Configurar Base de Datos

#### Actualizar Connection String

Abrir `appsettings.json` y modificar la cadena de conexión:

```json
{
  "ConnectionStrings": {
    "SqlServer": "Server=TU_SERVIDOR;Database=DataVision;User Id=TU_USUARIO;Password=TU_PASSWORD;Trusted_Connection=False;MultipleActiveResultSets=True;TrustServerCertificate=True"
  }
}
```

**Ejemplos de Connection Strings:**

**Para SQL Server LocalDB:**
```json
"SqlServer": "Server=(localdb)\\mssqllocaldb;Database=DataVision;Trusted_Connection=true;MultipleActiveResultSets=true"
```

**Para SQL Server con autenticación Windows:**
```json
"SqlServer": "Server=NOMBRE_SERVIDOR;Database=DataVision;Integrated Security=true;MultipleActiveResultSets=true;TrustServerCertificate=True"
```

**Para SQL Server con usuario/contraseña:**
```json
"SqlServer": "Server=NOMBRE_SERVIDOR;Database=DataVision;User Id=usuario;Password=contraseña;Trusted_Connection=False;MultipleActiveResultSets=True;TrustServerCertificate=True"
```

#### Aplicar Migraciones

Ejecutar en la terminal desde la raíz del proyecto:

```bash
# Instalar herramientas de Entity Framework (si no están instaladas)
dotnet tool install --global dotnet-ef

# Aplicar migraciones a la base de datos
dotnet ef database update
```

### 3. Configuración JWT (Opcional)

En `appsettings.json`, puedes cambiar la configuración JWT:

```json
{
  "Jwt": {
    "Key": "tu-clave-secreta-super-segura-minimo-32-caracteres",
    "Issuer": "DataVisionAPI",
    "Audience": "DataVisionUsers",
    "ExpireHours": 24
  }
}
```

### 4. Ejecutar la Aplicación

```bash
# Restaurar paquetes NuGet
dotnet restore

# Ejecutar la aplicación
dotnet run
```

La API estará disponible en:
- HTTP: `http://localhost:5287`
- HTTPS: `https://localhost:7004`

## 📚 Documentación de la API

### Acceder a la Documentación

Una vez que la aplicación esté ejecutándose, visita:
- **Scalar UI**: `https://localhost:7004/scalar/v1`
- **OpenAPI JSON**: `https://localhost:7004/openapi/v1.json`

## 🔐 Autenticación

### Registrar Usuario

**Endpoint:** `POST /api/auth/register`

**Cuerpo de la solicitud:**
```json
{
  "username": "miusuario",
  "password": "micontraseña123"
}
```

**Respuesta exitosa (200):**
```json
{
  "success": true,
  "message": "Usuario creado exitosamente",
  "data": {
    "id": 1,
    "username": "miusuario"
  },
  "errors": []
}
```

### Iniciar Sesión

**Endpoint:** `POST /api/auth/login`

**Cuerpo de la solicitud:**
```json
{
  "username": "miusuario",
  "password": "micontraseña123"
}
```

**Respuesta exitosa (200):**
```json
{
  "success": true,
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "message": "Autenticación exitosa",
  "user": {
    "id": 1,
    "username": "miusuario"
  },
  "expiresAt": "2025-08-09T15:30:00Z"
}
```

### 🔑 Usar el Token JWT

**Para todas las solicitudes autenticadas, incluir el header:**

```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

## 📊 Endpoints de Datos de RECOPE

### 1. Precios Internacionales

**Endpoint:** `GET /api/recope/precio-internacional`

**Parámetros opcionales:**
- `inicio` (string): Fecha de inicio (formato: YYYY-MM-DD)
- `fin` (string): Fecha de fin (formato: YYYY-MM-DD)

**Ejemplo de solicitud:**
```bash
GET /api/recope/precio-internacional?inicio=2024-01-01&fin=2024-12-31
Authorization: Bearer tu_token_jwt
```

**Respuesta:**
```json
{
  "periodos": [
    {
      "desde": "2024-01-01",
      "hasta": "2024-01-31"
    }
  ],
  "materiales": [
    {
      "id": "1",
      "nomprod": "Gasolina Super",
      "precios": [850.50, 860.25, 855.75]
    }
  ]
}
```

### 2. Precios al Consumidor

**Endpoint:** `GET /api/recope/precio-consumidor`

**Ejemplo de solicitud:**
```bash
GET /api/recope/precio-consumidor
Authorization: Bearer tu_token_jwt
```

**Respuesta:**
```json
[
  {
    "fecha": "2024-08-08",
    "tipo": "Consumidor",
    "impuesto": "150.25",
    "precsinimp": "700.50",
    "fechaupd": "2024-08-08T10:30:00Z",
    "id": "1",
    "preciototal": "850.75",
    "nomprod": "Gasolina Super",
    "margenpromedio": "25.50"
  }
]
```

### 3. Precios de Plantel

**Endpoint:** `GET /api/recope/precio-plantel`

**Ejemplo de solicitud:**
```bash
GET /api/recope/precio-plantel
Authorization: Bearer tu_token_jwt
```

**Respuesta:**
```json
[
  {
    "fecha": "2024-08-08",
    "tipo": "Plantel",
    "impuesto": "120.00",
    "precsinimp": "680.50",
    "fechaupd": "2024-08-08T10:30:00Z",
    "id": "1",
    "preciototal": "800.50",
    "nomprod": "Gasolina Super",
    "margenpromedio": "20.25"
  }
]
```

## 🔍 Ejemplos de Uso con cURL

### Registrar usuario:
```bash
curl -X POST "https://localhost:7004/api/auth/register" \
  -H "Content-Type: application/json" \
  -d '{
    "username": "testuser",
    "password": "testpass123"
  }'
```

### Iniciar sesión:
```bash
curl -X POST "https://localhost:7004/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "username": "testuser",
    "password": "testpass123"
  }'
```

### Consultar precios (con token):
```bash
curl -X GET "https://localhost:7004/api/recope/precio-consumidor" \
  -H "Authorization: Bearer TU_TOKEN_JWT_AQUI"
```

## 🔍 Ejemplos para Frontend (JavaScript)

### Función de Login:
```javascript
async function login(username, password) {
  const response = await fetch('/api/auth/login', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify({ username, password })
  });

  const result = await response.json();
  
  if (result.success) {
    // Guardar token en localStorage
    localStorage.setItem('authToken', result.token);
    localStorage.setItem('tokenExpiry', result.expiresAt);
    return result;
  } else {
    throw new Error(result.message);
  }
}
```

### Función para hacer solicitudes autenticadas:
```javascript
async function fetchWithAuth(url) {
  const token = localStorage.getItem('authToken');
  
  if (!token) {
    throw new Error('No hay token de autenticación');
  }

  const response = await fetch(url, {
    headers: {
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    }
  });

  if (response.status === 401) {
    // Token expirado o inválido
    localStorage.removeItem('authToken');
    localStorage.removeItem('tokenExpiry');
    throw new Error('Token expirado. Favor iniciar sesión nuevamente.');
  }

  return await response.json();
}
```

### Ejemplo de uso:
```javascript
// Obtener precios al consumidor
fetchWithAuth('/api/recope/precio-consumidor')
  .then(data => {
    console.log('Precios:', data);
  })
  .catch(error => {
    console.error('Error:', error.message);
  });
```

## 🗃️ Base de Datos

### Tablas Creadas

La aplicación crea automáticamente las siguientes tablas:

1. **Users**
   - Id (int, PK, Identity)
   - Username (nvarchar(50), Unique)
   - PasswordHash (nvarchar(255))

2. **Logs**
   - Id (int, PK, Identity)
   - UserId (int, FK)
   - FechaConsulta (datetime2)
   - EndpointConsultado (nvarchar(255))

### Logging Automático

La API registra automáticamente todas las consultas autenticadas en la tabla `Logs`, incluyendo:
- ID del usuario que hizo la consulta
- Fecha y hora de la consulta
- Endpoint consultado

## ⚠️ Códigos de Error Comunes

- **400 Bad Request**: Datos de entrada inválidos
- **401 Unauthorized**: Token JWT inválido o expirado
- **404 Not Found**: Endpoint no encontrado
- **500 Internal Server Error**: Error interno del servidor

## 🔒 Seguridad

- Las contraseñas se encriptan usando BCrypt
- Los tokens JWT tienen expiración configurable (24 horas por defecto)
- Todos los endpoints de datos requieren autenticación
- Se registran todas las consultas para auditoría

## 🚀 Despliegue

Para desplegar en producción:

1. Actualizar `appsettings.Production.json` con configuraciones de producción
2. Cambiar la cadena de conexión a la base de datos de producción
3. Generar una clave JWT más segura y compleja
4. Configurar HTTPS apropiadamente
5. Aplicar migraciones a la base de datos de producción:
   ```bash
   dotnet ef database update --environment Production
   ```

## 📝 Notas Importantes

- **Guarda el token JWT**: El frontend debe almacenar el token JWT después del login
- **Verificar expiración**: Implementar lógica para refrescar tokens antes de que expiren
- **Manejo de errores**: Implementar manejo apropiado de errores 401 para redirigir al login
- **HTTPS**: Usar siempre HTTPS en producción para proteger los tokens
- **Variables de entorno**: En producción, usar variables de entorno para datos sensibles

## 🔧 Troubleshooting

### Problemas comunes:

1. **Error de conexión a la base de datos**:
   - Verificar que SQL Server esté ejecutándose
   - Comprobar la cadena de conexión
   - Verificar permisos del usuario de base de datos

2. **Token JWT inválido**:
   - Verificar que el header Authorization esté presente
   - Comprobar que el token no haya expirado
   - Verificar formato: `Bearer token_aqui`

3. **Migraciones no aplicadas**:
   ```bash
   dotnet ef database update --verbose
   ```

4. **Puerto en uso**:
   - Cambiar puertos en `launchSettings.json`
   - O usar: `dotnet run --urls "https://localhost:8001;http://localhost:8000"`
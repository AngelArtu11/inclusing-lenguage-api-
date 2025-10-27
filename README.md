# InclusingLenguage API

API REST para conectar la aplicación móvil MAUI de InclusingLenguage con MongoDB.

## Arquitectura

```
[App MAUI] ←→ [API REST ASP.NET Core] ←→ [MongoDB Atlas]
```

## Configuración

### 1. Base de datos MongoDB

La API se conecta a MongoDB Atlas usando la configuración en `appsettings.json`:

```json
{
  "MongoDBSettings": {
    "ConnectionString": "mongodb+srv://angel_artu:Test123456@includesign.zz0yofi.mongodb.net/?retryWrites=true&w=majority&appName=includesign",
    "DatabaseName": "includesign"
  }
}
```

### 2. Ejecutar la API

```bash
cd InclusingLenguage.API
dotnet run
```

Por defecto, la API se ejecutará en:
- HTTPS: `https://localhost:7246`
- HTTP: `http://localhost:5246`

## Endpoints Disponibles

### Autenticación (`/api/auth`)

#### Registro
```http
POST /api/auth/register
Content-Type: application/json

{
  "email": "usuario@ejemplo.com",
  "password": "contraseña123",
  "firstName": "Nombre",
  "lastName": "Apellido"
}
```

**Respuesta exitosa:**
```json
{
  "isSuccess": true,
  "token": "base64_token_aqui",
  "userProfile": {
    "id": "mongodb_object_id",
    "email": "usuario@ejemplo.com",
    "firstName": "Nombre",
    "lastName": "Apellido",
    "level": 1,
    "experience": 0,
    "streak": 0
  }
}
```

#### Login
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "usuario@ejemplo.com",
  "password": "contraseña123"
}
```

**Respuesta exitosa:**
```json
{
  "isSuccess": true,
  "token": "base64_token_aqui",
  "userProfile": { ... }
}
```

### Usuarios (`/api/users`)

#### Obtener todos los usuarios
```http
GET /api/users
```

#### Obtener usuario por email
```http
GET /api/users/{email}
```

#### Actualizar perfil de usuario
```http
PUT /api/users/{email}
Content-Type: application/json

{
  "firstName": "Nombre",
  "lastName": "Apellido",
  "level": 2,
  "experience": 150,
  "streak": 5,
  "completedLessons": ["lesson1", "lesson2"],
  "lessonProgress": {
    "lesson1": 100,
    "lesson2": 50
  },
  "dailyGoal": 5,
  "todayProgress": 2
}
```

#### Actualizar progreso de lección
```http
POST /api/users/{email}/progress
Content-Type: application/json

{
  "lessonId": "lesson_123",
  "progress": 75.5,
  "experienceGained": 10
}
```

#### Eliminar usuario
```http
DELETE /api/users/{email}
```

### Lecciones (`/api/lessons`)

#### Obtener todas las lecciones
```http
GET /api/lessons
```

#### Obtener lección por ID
```http
GET /api/lessons/{id}
```

#### Obtener lecciones por categoría
```http
GET /api/lessons/category/{category}
```
Categorías disponibles: `Alphabet`, `Numbers`, `Words`

#### Crear lección
```http
POST /api/lessons
Content-Type: application/json

{
  "lessonId": 1,
  "title": "Letra A",
  "category": "Alphabet",
  "letter": "A",
  "description": "Aprende la letra A en lenguaje de señas",
  "imageUrl": "https://...",
  "videoUrl": "https://...",
  "gifUrl": "https://...",
  "order": 1,
  "experiencePoints": 10,
  "difficulty": "Basic",
  "exercises": [],
  "learningTips": ["Tip 1", "Tip 2"],
  "estimatedMinutes": 5
}
```

#### Actualizar lección
```http
PUT /api/lessons/{id}
Content-Type: application/json

{
  "title": "Letra A - Actualizado",
  ...
}
```

#### Eliminar lección
```http
DELETE /api/lessons/{id}
```

## Uso desde la App MAUI

En tu app MAUI, usa el servicio `IApiService` para consumir la API:

```csharp
// Inyectar el servicio en tu ViewModel o página
public class LoginViewModel
{
    private readonly IApiService _apiService;

    public LoginViewModel(IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task LoginAsync(string email, string password)
    {
        var request = new LoginRequest
        {
            Email = email,
            Password = password
        };

        var result = await _apiService.LoginAsync(request);

        if (result.IsSuccess)
        {
            // Login exitoso
            var user = result.UserProfile;
        }
        else
        {
            // Mostrar error
            Console.WriteLine(result.ErrorMessage);
        }
    }
}
```

## Configurar la URL de la API en MAUI

Edita el archivo `ApiService.cs` en la app MAUI y cambia la URL base:

```csharp
// Para desarrollo local
private const string BaseUrl = "https://localhost:7246/api";

// Para producción (cuando despliegues la API)
private const string BaseUrl = "https://tu-api.azurewebsites.net/api";

// Para pruebas en emulador Android
private const string BaseUrl = "https://10.0.2.2:7246/api";

// Para pruebas en dispositivo físico (misma red)
private const string BaseUrl = "https://TU_IP_LOCAL:7246/api";
```

## CORS

La API está configurada para aceptar peticiones de cualquier origen en desarrollo. Para producción, deberías restringir esto editando `Program.cs`:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins("https://tu-dominio.com")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
```

## Swagger UI

Cuando ejecutes la API en modo desarrollo, puedes probar los endpoints en:

```
https://localhost:7246/swagger
```

## Estructura del Proyecto

```
InclusingLenguage.API/
├── Controllers/
│   ├── AuthController.cs       # Endpoints de autenticación
│   ├── UsersController.cs      # Endpoints de usuarios
│   └── LessonsController.cs    # Endpoints de lecciones
├── Models/
│   ├── MongoDBSettings.cs      # Configuración de MongoDB
│   ├── UserProfile.cs          # Modelo de usuario
│   └── Lesson.cs               # Modelo de lección
├── Services/
│   ├── MongoDBService.cs       # Servicio de conexión a MongoDB
│   └── AuthService.cs          # Servicio de autenticación
├── appsettings.json            # Configuración
└── Program.cs                  # Configuración de la aplicación
```

## Próximos pasos

1. ✅ API REST creada y funcionando
2. ✅ Servicio HTTP en MAUI para consumir la API
3. 📝 Actualizar los ViewModels en MAUI para usar `IApiService` en lugar de `MongoDBService`
4. 📝 Desplegar la API a Azure/AWS para producción
5. 📝 Implementar JWT para autenticación más segura
6. 📝 Agregar validación de modelos y manejo de errores mejorado

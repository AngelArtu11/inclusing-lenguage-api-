# Configuración de la API para Móvil

## 🎯 Tu IP Local: `192.168.1.68`

La API está corriendo en tu PC en: `http://localhost:5256`

## 📱 Configuración según tu escenario

### 1️⃣ **Android Emulator** (Recomendado para pruebas)

En `ApiService.cs` (línea 27), usa:
```csharp
private const string BaseUrl = "http://10.0.2.2:5256/api";
```

**¿Por qué `10.0.2.2`?**
- Es una IP especial del emulador Android que redirige al `localhost` de tu PC
- No necesitas cambiar nada más

### 2️⃣ **Dispositivo Físico Android/iOS** (En la misma red WiFi)

En `ApiService.cs` (línea 27), usa:
```csharp
private const string BaseUrl = "http://192.168.1.68:5256/api";
```

**Requisitos:**
- Tu celular y PC deben estar en la **misma red WiFi**
- Descomenta esta línea y comenta las demás opciones

**⚠️ IMPORTANTE:** Si cambias de red WiFi, tu IP puede cambiar. Ejecuta `ipconfig` para obtener la nueva IP.

### 3️⃣ **iOS Simulator**

En `ApiService.cs` (línea 27), usa:
```csharp
private const string BaseUrl = "http://localhost:5256/api";
```

### 4️⃣ **Producción** (API desplegada en servidor)

En `ApiService.cs` (línea 27), usa:
```csharp
private const string BaseUrl = "https://tu-dominio.azurewebsites.net/api";
```

## 🚀 Cómo ejecutar la API

### Paso 1: Iniciar la API
```bash
cd InclusingLenguage.API
dotnet run
```

Verás algo como:
```
Now listening on: http://localhost:5256
```

### Paso 2: Configurar el Firewall (Solo para dispositivo físico)

Si usas un **dispositivo físico**, necesitas permitir conexiones en el firewall de Windows:

**Opción A: Por comando (Requiere PowerShell como administrador)**
```powershell
New-NetFirewallRule -DisplayName "ASP.NET Core API" -Direction Inbound -Protocol TCP -LocalPort 5256 -Action Allow
```

**Opción B: Manual**
1. Abre "Firewall de Windows Defender"
2. "Configuración avanzada" → "Reglas de entrada"
3. "Nueva regla" → "Puerto" → TCP → 5256
4. Permitir la conexión
5. Aplicar a todas las redes

### Paso 3: Ejecutar tu app MAUI

Dependiendo de tu configuración en `ApiService.cs`:

**Android Emulator:**
```bash
dotnet build -t:Run -f net8.0-android
```

**iOS Simulator:**
```bash
dotnet build -t:Run -f net8.0-ios
```

**Windows:**
```bash
dotnet build -t:Run -f net8.0-windows10.0.19041.0
```

## 🧪 Probar la conexión

### Desde el emulador Android:

Abre el navegador del emulador y visita:
```
http://10.0.2.2:5256/api/lessons
```

Deberías ver `[]` (una lista vacía de lecciones).

### Desde dispositivo físico:

Abre el navegador del celular y visita:
```
http://192.168.1.68:5256/api/lessons
```

Si ves `[]`, ¡la conexión funciona!

## ❓ Solución de problemas

### Error: "No se puede conectar a la API"

1. **Verifica que la API está corriendo:**
   ```bash
   curl http://localhost:5256/api/lessons
   ```
   Debería responder `[]`

2. **Para dispositivo físico:**
   - Verifica que estén en la misma red WiFi
   - Ping a tu PC desde el celular (usa una app como "Network Utilities")
   - Verifica el firewall (ver Paso 2 arriba)

3. **Verifica la IP actual:**
   ```bash
   ipconfig
   ```
   Busca "Adaptador de LAN inalámbrica Wi-Fi" → "Dirección IPv4"

### Error: "SSL/TLS error"

Estamos usando HTTP (no HTTPS) en desarrollo. Si ves errores SSL:

**Android:** Agrega esto a `Platforms/Android/AndroidManifest.xml`:
```xml
<application android:usesCleartextTraffic="true">
```

**iOS:** Agrega esto a `Platforms/iOS/Info.plist`:
```xml
<key>NSAppTransportSecurity</key>
<dict>
    <key>NSAllowsArbitraryLoads</key>
    <true/>
</dict>
```

### La IP cambió

Tu IP local puede cambiar si:
- Reinicias el router
- Te conectas a otra red WiFi
- El router asigna IPs dinámicamente

**Solución:**
1. Ejecuta `ipconfig` de nuevo
2. Busca la nueva IPv4 en "Wi-Fi"
3. Actualiza `ApiService.cs` con la nueva IP

## 📊 Endpoints disponibles

Con la API corriendo, puedes probar:

```bash
# Ver todas las lecciones
GET http://TU_IP:5256/api/lessons

# Registrar usuario
POST http://TU_IP:5256/api/auth/register
Body: {"email":"test@test.com","password":"Test123","firstName":"Test","lastName":"User"}

# Login
POST http://TU_IP:5256/api/auth/login
Body: {"email":"test@test.com","password":"Test123"}

# Ver usuarios
GET http://TU_IP:5256/api/users
```

Reemplaza `TU_IP` con:
- `10.0.2.2` para emulador Android
- `192.168.1.68` para dispositivo físico
- `localhost` para iOS Simulator

## 🎯 Resumen rápido

| Escenario | URL a usar | Firewall necesario? |
|-----------|-----------|---------------------|
| Android Emulator | `http://10.0.2.2:5256/api` | ❌ No |
| Dispositivo físico | `http://192.168.1.68:5256/api` | ✅ Sí |
| iOS Simulator | `http://localhost:5256/api` | ❌ No |
| Windows | `http://localhost:5256/api` | ❌ No |
| Producción | `https://tu-api.com/api` | ❌ No |

## ✅ Checklist antes de probar

- [ ] API está corriendo (`dotnet run` en InclusingLenguage.API)
- [ ] Configuraste la URL correcta en `ApiService.cs`
- [ ] Si usas dispositivo físico: firewall configurado
- [ ] Si usas dispositivo físico: misma red WiFi
- [ ] Si usas HTTP: configuración de cleartext en Android/iOS

¡Listo! Ahora tu app móvil se puede conectar a la API.

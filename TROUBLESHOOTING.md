# 🔧 Troubleshooting - Error de Conexión

## Error: "Connection.Failure" o "Timeout"

Este error indica que la app móvil no puede conectarse a la API. Aquí están las soluciones:

---

## ⚡ Solución CRÍTICA: Configurar la API para escuchar en todas las interfaces

**Este es el problema más común para apps móviles.**

### El problema:
Por defecto, la API escucha solo en `localhost`, lo que impide que el emulador Android se conecte.

### La solución:

**Archivo:** `InclusingLenguage.API/Properties/launchSettings.json`

Cambia la línea 17:
```json
// ANTES:
"applicationUrl": "http://localhost:5256",

// DESPUÉS:
"applicationUrl": "http://0.0.0.0:5256",
```

### Reinicia la API:
```bash
# Detén la API (Ctrl+C)
# Luego reinicia:
cd InclusingLenguage.API
dotnet run
```

### Verificar:
Deberías ver:
```
Now listening on: http://0.0.0.0:5256
```

✅ **`0.0.0.0`** significa que escucha en **todas las interfaces**, permitiendo conexiones desde el emulador.

---

## ✅ Solución 1: Verificar que la API esté corriendo

### Paso 1: Verificar el proceso
La API debe estar corriendo en `http://localhost:5256`

```bash
# Verifica que esté corriendo
curl http://localhost:5256/api/lessons
# Debería responder: []
```

### Paso 2: Si no está corriendo, iníciala
```bash
cd InclusingLenguage.API
dotnet run
```

Deberías ver:
```
Now listening on: http://localhost:5256
Application started.
```

---

## ✅ Solución 2: Verificar la URL en ApiService.cs

### Para Android Emulator (Recomendado para pruebas)
La URL debe ser `http://10.0.2.2:5256/api`

**Ubicación:** `InclusingLenguage/04_Services/ApiService.cs` línea 27

```csharp
private const string BaseUrl = "http://10.0.2.2:5256/api";
```

**¿Por qué 10.0.2.2?**
- Es una IP especial del emulador Android
- Redirige al `localhost` de tu PC
- Permite que el emulador acceda a servicios en tu máquina

### Para dispositivo físico Android/iOS
```csharp
private const string BaseUrl = "http://192.168.1.68:5256/api";
```
⚠️ **Importante:** Reemplaza `192.168.1.68` con TU IP local actual

Para obtener tu IP:
```bash
# Windows
ipconfig

# Busca "Adaptador de LAN inalámbrica Wi-Fi"
# La dirección IPv4 es tu IP local
```

### Para iOS Simulator
```csharp
private const string BaseUrl = "http://localhost:5256/api";
```

---

## ✅ Solución 3: Configurar Firewall (Solo dispositivo físico)

Si usas un dispositivo físico, necesitas permitir conexiones en el firewall:

### Opción A: PowerShell (Como Administrador)
```powershell
New-NetFirewallRule -DisplayName "ASP.NET Core API" -Direction Inbound -Protocol TCP -LocalPort 5256 -Action Allow
```

### Opción B: Manual
1. Abre "Firewall de Windows Defender"
2. "Configuración avanzada" → "Reglas de entrada"
3. "Nueva regla"
4. Tipo: "Puerto"
5. Protocolo: TCP, Puerto: 5256
6. Acción: "Permitir la conexión"
7. Perfil: Marca todas las opciones
8. Nombre: "ASP.NET Core API"

---

## ✅ Solución 4: Verificar Red (Dispositivo físico)

### Requisitos:
- ✅ PC y celular en la **misma red WiFi**
- ✅ No usar VPN en el celular
- ✅ No usar "Datos móviles", solo WiFi

### Probar conectividad desde el celular:
1. Abre el navegador del celular
2. Visita: `http://TU_IP:5256/api/lessons`
   - Ejemplo: `http://192.168.1.68:5256/api/lessons`
3. Deberías ver: `[]`

Si no funciona:
- Verifica la IP del PC
- Verifica que ambos estén en la misma red
- Desactiva VPN si la tienes

---

## ✅ Solución 5: Verificar logs de la API

Cuando intentas login/register desde la app, deberías ver en la consola de la API:

```
[ApiService] Configurado con BaseUrl: http://10.0.2.2:5256/api
[ApiService.LoginAsync] Iniciando login para: test@test.com
[ApiService.LoginAsync] Status Code: OK
[ApiService.LoginAsync] Login exitoso
```

Si no ves estos logs:
- La app no está llegando a la API
- Verifica la URL en ApiService.cs
- Verifica el firewall

---

## ✅ Solución 6: Aumentar el Timeout (Ya está configurado)

El timeout ahora es de **120 segundos** (2 minutos). Si aún así falla, el problema es de conectividad.

---

## 🧪 Test de Diagnóstico

### 1. Desde tu PC (debería funcionar):
```bash
curl http://localhost:5256/api/lessons
# Debería responder: []
```

### 2. Desde el emulador Android:
En la app, intenta login/register y revisa los logs en Visual Studio Output.

Busca en "Output" → "Debug":
```
[ApiService] Configurado con BaseUrl: http://10.0.2.2:5256/api
[ApiService.LoginAsync] Iniciando login...
```

### 3. Si ves "Timeout":
```
[ApiService.LoginAsync] Timeout: ...
```

**Solución:**
1. Verifica que la API esté corriendo
2. Verifica la URL (debe ser `http://10.0.2.2:5256/api` para emulador)
3. Reinicia el emulador Android

### 4. Si ves "Error HTTP" o "Error de red":
```
[ApiService.LoginAsync] Error HTTP: ...
```

**Solución:**
1. La API está apagada o no responde
2. La URL está mal configurada
3. Para dispositivo físico: firewall bloqueando

---

## 📝 Checklist Rápido

- [ ] API está corriendo (`dotnet run` en InclusingLenguage.API)
- [ ] Ves "Now listening on: http://localhost:5256"
- [ ] `curl http://localhost:5256/api/lessons` responde `[]`
- [ ] **Android Emulator:** URL es `http://10.0.2.2:5256/api`
- [ ] **Dispositivo físico:** URL es `http://TU_IP:5256/api`
- [ ] **Dispositivo físico:** Firewall configurado
- [ ] **Dispositivo físico:** Misma red WiFi
- [ ] Revisar logs en Visual Studio Output → Debug

---

## 🆘 Si nada funciona

### Opción temporal: Usar MongoDB directo

Si la API no funciona, puedes temporalmente usar MongoDB directo desde la app:

1. En `MauiProgram.cs`, comenta el `IAuthenticationService`:
```csharp
// builder.Services.AddSingleton<IAuthenticationService, AuthenticationService>();
```

2. Usa el `MongoDBService` directamente en tus páginas (como estaba antes)

**Nota:** Esta opción NO es recomendada para producción, solo para desarrollo local.

---

## 📞 Información adicional

### Puertos usados:
- **API:** 5256 (HTTP)
- **MongoDB Atlas:** Remoto (no requiere configuración local)

### URLs según escenario:

| Escenario | URL | Firewall |
|-----------|-----|----------|
| Android Emulator | `http://10.0.2.2:5256/api` | ❌ No necesario |
| Dispositivo físico | `http://TU_IP:5256/api` | ✅ Necesario |
| iOS Simulator | `http://localhost:5256/api` | ❌ No necesario |
| Windows Desktop | `http://localhost:5256/api` | ❌ No necesario |

### Comandos útiles:

```bash
# Ver IP local
ipconfig

# Probar API desde PC
curl http://localhost:5256/api/lessons

# Probar API desde otra máquina en la red
curl http://TU_IP:5256/api/lessons

# Reiniciar API
# Ctrl+C para detener
dotnet run
```

---

## ✅ Verificación Final

Si todo está bien configurado:

1. **API corriendo:**
   ```
   Now listening on: http://localhost:5256
   ```

2. **Curl responde:**
   ```bash
   curl http://localhost:5256/api/lessons
   # Respuesta: []
   ```

3. **Logs en Visual Studio:**
   ```
   [ApiService] Configurado con BaseUrl: http://10.0.2.2:5256/api
   [ApiService.LoginAsync] Iniciando login para: test@test.com
   [ApiService.LoginAsync] Status Code: OK
   [ApiService.LoginAsync] Login exitoso
   ```

4. **App funciona:** Login/Register exitoso

---

¡Listo! Con estos pasos deberías poder conectar tu app móvil a la API.

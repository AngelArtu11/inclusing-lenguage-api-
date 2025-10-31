using Microsoft.AspNetCore.Mvc;
using InclusingLenguage.API.Models;
using InclusingLenguage.API.Services;
using MongoDB.Driver;

namespace InclusingLenguage.API.Controllers
{
    [ApiController]
    [Route("api/progresion")]
    public class ProgresionController : ControllerBase
    {
        private readonly IMongoDBService _mongoDBService;

        public ProgresionController(IMongoDBService mongoDBService)
        {
            _mongoDBService = mongoDBService;
        }

        // GET: api/progresion/{usuarioID}
        [HttpGet("{usuarioID}")]
        public async Task<ActionResult<Progresion>> GetByUsuarioID(string usuarioID)
        {
            var progresion = await _mongoDBService.Progresion
                .Find(p => p.UsuarioID == usuarioID)
                .FirstOrDefaultAsync();

            if (progresion == null)
            {
                // Crear progresión inicial si no existe
                progresion = new Progresion
                {
                    UsuarioID = usuarioID,
                    NivelActual = 0,
                    NivelesCompletados = new List<int>(),
                    Intentos = new List<Intento>(),
                    Estadisticas = new Estadisticas()
                };

                await _mongoDBService.Progresion.InsertOneAsync(progresion);
            }

            return Ok(progresion);
        }

        // PUT: api/progresion/{usuarioID}
        [HttpPut("{usuarioID}")]
        public async Task<ActionResult<Progresion>> Update(string usuarioID, [FromBody] Progresion updatedProgresion)
        {
            var progresion = await _mongoDBService.Progresion
                .Find(p => p.UsuarioID == usuarioID)
                .FirstOrDefaultAsync();

            if (progresion == null)
            {
                return NotFound(new { message = "Progresión no encontrada" });
            }

            updatedProgresion.Id = progresion.Id;
            updatedProgresion.UsuarioID = usuarioID;

            await _mongoDBService.Progresion.ReplaceOneAsync(p => p.UsuarioID == usuarioID, updatedProgresion);

            return Ok(updatedProgresion);
        }

        // POST: api/progresion/completar-nivel
        [HttpPost("completar-nivel")]
        public async Task<ActionResult> CompletarNivel([FromBody] CompletarNivelRequest request)
        {
            var progresion = await _mongoDBService.Progresion
                .Find(p => p.UsuarioID == request.UsuarioID)
                .FirstOrDefaultAsync();

            if (progresion == null)
            {
                // Crear progresión inicial si no existe
                progresion = new Progresion
                {
                    UsuarioID = request.UsuarioID,
                    NivelActual = 0,
                    NivelesCompletados = new List<int>(),
                    Intentos = new List<Intento>(),
                    Estadisticas = new Estadisticas()
                };

                await _mongoDBService.Progresion.InsertOneAsync(progresion);
            }

            // Agregar nivel a completados si es exitoso y no está ya
            if (request.Resultado.ToLower() == "exito" && !progresion.NivelesCompletados.Contains(request.Nivel))
            {
                progresion.NivelesCompletados.Add(request.Nivel);

                // Actualizar nivel actual si es mayor
                if (request.Nivel >= progresion.NivelActual)
                {
                    progresion.NivelActual = request.Nivel + 1;
                }
            }

            // Agregar intento
            progresion.Intentos.Add(new Intento
            {
                Nivel = request.Nivel,
                Resultado = request.Resultado,
                Fecha = request.Fecha ?? DateTime.UtcNow.ToString("o")
            });

            // Actualizar estadísticas
            progresion.Estadisticas.TotalIntentos++;
            if (request.Resultado.ToLower() == "exito")
            {
                progresion.Estadisticas.TotalExitos++;
            }

            await _mongoDBService.Progresion.ReplaceOneAsync(p => p.UsuarioID == request.UsuarioID, progresion);

            return Ok(new { message = "Nivel completado exitosamente", progresion });
        }

        // POST: api/progresion/registrar-intento
        [HttpPost("registrar-intento")]
        public async Task<ActionResult> RegistrarIntento([FromBody] RegistrarIntentoRequest request)
        {
            var progresion = await _mongoDBService.Progresion
                .Find(p => p.UsuarioID == request.UsuarioID)
                .FirstOrDefaultAsync();

            if (progresion == null)
            {
                // Crear progresión inicial si no existe
                progresion = new Progresion
                {
                    UsuarioID = request.UsuarioID,
                    NivelActual = 0,
                    NivelesCompletados = new List<int>(),
                    Intentos = new List<Intento>(),
                    Estadisticas = new Estadisticas()
                };

                await _mongoDBService.Progresion.InsertOneAsync(progresion);
            }

            // Agregar intento
            progresion.Intentos.Add(new Intento
            {
                Nivel = request.Nivel,
                Resultado = request.Resultado,
                Fecha = request.Fecha ?? DateTime.UtcNow.ToString("o")
            });

            // Actualizar estadísticas
            progresion.Estadisticas.TotalIntentos++;
            if (request.Resultado.ToLower() == "exito")
            {
                progresion.Estadisticas.TotalExitos++;
            }

            await _mongoDBService.Progresion.ReplaceOneAsync(p => p.UsuarioID == request.UsuarioID, progresion);

            return Ok(new { message = "Intento registrado exitosamente", progresion });
        }

        // GET: api/progresion
        [HttpGet]
        public async Task<ActionResult<List<Progresion>>> GetAll()
        {
            var progresiones = await _mongoDBService.Progresion
                .Find(_ => true)
                .ToListAsync();

            return Ok(progresiones);
        }
    }

    // DTOs para las peticiones
    public class CompletarNivelRequest
    {
        public string UsuarioID { get; set; } = string.Empty;
        public int Nivel { get; set; }
        public string Resultado { get; set; } = string.Empty;
        public string? Fecha { get; set; }
    }

    public class RegistrarIntentoRequest
    {
        public string UsuarioID { get; set; } = string.Empty;
        public int Nivel { get; set; }
        public string Resultado { get; set; } = string.Empty;
        public string? Fecha { get; set; }
    }
}

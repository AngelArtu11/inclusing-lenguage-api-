using Microsoft.AspNetCore.Mvc;
using InclusingLenguage.API.Models;
using InclusingLenguage.API.Services;
using MongoDB.Driver;

namespace InclusingLenguage.API.Controllers
{
    [ApiController]
    [Route("api/niveles")]
    public class LessonsController : ControllerBase
    {
        private readonly IMongoDBService _mongoDBService;

        public LessonsController(IMongoDBService mongoDBService)
        {
            _mongoDBService = mongoDBService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Nivel>>> GetAll()
        {
            var niveles = await _mongoDBService.Niveles
                .Find(_ => true)
                .SortBy(n => n.NivelID)
                .ToListAsync();

            return Ok(niveles);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Nivel>> GetById(string id)
        {
            var nivel = await _mongoDBService.Niveles
                .Find(n => n.Id == id)
                .FirstOrDefaultAsync();

            if (nivel == null)
            {
                return NotFound(new { message = "Nivel no encontrado" });
            }

            return Ok(nivel);
        }

        [HttpGet("nivelID/{nivelID}")]
        public async Task<ActionResult<Nivel>> GetByNivelID(int nivelID)
        {
            var nivel = await _mongoDBService.Niveles
                .Find(n => n.NivelID == nivelID)
                .FirstOrDefaultAsync();

            if (nivel == null)
            {
                return NotFound(new { message = "Nivel no encontrado" });
            }

            return Ok(nivel);
        }

        [HttpPost]
        public async Task<ActionResult<Nivel>> Create([FromBody] Nivel nivel)
        {
            await _mongoDBService.Niveles.InsertOneAsync(nivel);
            return CreatedAtAction(nameof(GetById), new { id = nivel.Id }, nivel);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Nivel>> Update(string id, [FromBody] Nivel updatedNivel)
        {
            var nivel = await _mongoDBService.Niveles
                .Find(n => n.Id == id)
                .FirstOrDefaultAsync();

            if (nivel == null)
            {
                return NotFound(new { message = "Nivel no encontrado" });
            }

            updatedNivel.Id = id;
            await _mongoDBService.Niveles.ReplaceOneAsync(n => n.Id == id, updatedNivel);

            return Ok(updatedNivel);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _mongoDBService.Niveles.DeleteOneAsync(n => n.Id == id);

            if (result.DeletedCount == 0)
            {
                return NotFound(new { message = "Nivel no encontrado" });
            }

            return Ok(new { message = "Nivel eliminado exitosamente" });
        }
    }
}

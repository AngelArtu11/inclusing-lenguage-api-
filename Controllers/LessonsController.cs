using Microsoft.AspNetCore.Mvc;
using InclusingLenguage.API.Models;
using InclusingLenguage.API.Services;
using MongoDB.Driver;

namespace InclusingLenguage.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LessonsController : ControllerBase
    {
        private readonly IMongoDBService _mongoDBService;

        public LessonsController(IMongoDBService mongoDBService)
        {
            _mongoDBService = mongoDBService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Lesson>>> GetAll()
        {
            var lessons = await _mongoDBService.Lessons
                .Find(_ => true)
                .SortBy(l => l.Order)
                .ToListAsync();

            return Ok(lessons);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Lesson>> GetById(string id)
        {
            var lesson = await _mongoDBService.Lessons
                .Find(l => l.Id == id)
                .FirstOrDefaultAsync();

            if (lesson == null)
            {
                return NotFound(new { message = "Lección no encontrada" });
            }

            return Ok(lesson);
        }

        [HttpGet("category/{category}")]
        public async Task<ActionResult<List<Lesson>>> GetByCategory(string category)
        {
            var lessons = await _mongoDBService.Lessons
                .Find(l => l.Category == category)
                .SortBy(l => l.Order)
                .ToListAsync();

            return Ok(lessons);
        }

        [HttpPost]
        public async Task<ActionResult<Lesson>> Create([FromBody] Lesson lesson)
        {
            await _mongoDBService.Lessons.InsertOneAsync(lesson);
            return CreatedAtAction(nameof(GetById), new { id = lesson.Id }, lesson);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Lesson>> Update(string id, [FromBody] Lesson updatedLesson)
        {
            var lesson = await _mongoDBService.Lessons
                .Find(l => l.Id == id)
                .FirstOrDefaultAsync();

            if (lesson == null)
            {
                return NotFound(new { message = "Lección no encontrada" });
            }

            updatedLesson.Id = id;
            await _mongoDBService.Lessons.ReplaceOneAsync(l => l.Id == id, updatedLesson);

            return Ok(updatedLesson);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _mongoDBService.Lessons.DeleteOneAsync(l => l.Id == id);

            if (result.DeletedCount == 0)
            {
                return NotFound(new { message = "Lección no encontrada" });
            }

            return Ok(new { message = "Lección eliminada exitosamente" });
        }
    }
}

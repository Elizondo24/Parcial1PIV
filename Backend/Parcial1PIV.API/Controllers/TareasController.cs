using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Parcial1PIV.API.Models;
using Parcial1PIV.API.Services;

namespace Parcial1PIV.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TareasController : ControllerBase
    {
        private readonly TareaService _tareaService;

        public TareasController(TareaService tareaService)
        {
            _tareaService = tareaService;
        }

        // GET: api/tareas
        [HttpGet]
        public async Task<IActionResult> ObtenerTodas(
            [FromQuery] string? estado,
            [FromQuery] string? buscar,
            [FromQuery] string? ordenarPor)
        {
            var tareas = await _tareaService.ObtenerTodasAsync();

            // LINQ: filtrar por estado
            if (!string.IsNullOrWhiteSpace(estado))
            {
                tareas = tareas
                    .Where(t => t.Estado.Equals(
                        estado,
                        StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            // LINQ: buscar por título o descripción
            if (!string.IsNullOrWhiteSpace(buscar))
            {
                tareas = tareas
                    .Where(t =>
                        t.Titulo.Contains(
                            buscar,
                            StringComparison.OrdinalIgnoreCase)
                        ||
                        t.Descripcion.Contains(
                            buscar,
                            StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            // LINQ: ordenar por fecha o prioridad
            if (ordenarPor?.ToLower() == "fecha")
            {
                tareas = tareas
                    .OrderBy(t => t.FechaLimite)
                    .ToList();
            }
            else if (ordenarPor?.ToLower() == "prioridad")
            {
                tareas = tareas
                    .OrderByDescending(t =>
                        t.Prioridad == "Alta" ? 3 :
                        t.Prioridad == "Media" ? 2 : 1)
                    .ToList();
            }

            return Ok(tareas);
        }

        // GET: api/tareas/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(string id)
        {
            if (!ObjectId.TryParse(id, out _))
                return BadRequest("El ID no es válido.");

            var tarea = await _tareaService.ObtenerPorIdAsync(id);

            if (tarea == null)
                return NotFound("Tarea no encontrada.");

            return Ok(tarea);
        }

        // POST: api/tareas
        [HttpPost]
        public async Task<IActionResult> Crear(Tarea tarea)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (tarea.FechaLimite == default)
                return BadRequest("La fecha límite es obligatoria.");

            if (!ValidarPrioridad(tarea.Prioridad))
                return BadRequest("Prioridad no válida.");

            tarea.Id = null;
            tarea.Estado = "Pendiente";
            tarea.FechaCreacion = DateTime.Now;

            await _tareaService.CrearAsync(tarea);

            return CreatedAtAction(
                nameof(ObtenerPorId),
                new { id = tarea.Id },
                tarea
            );
        }

        // PUT: api/tareas/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(
            string id,
            Tarea tarea)
        {
            if (!ObjectId.TryParse(id, out _))
                return BadRequest("El ID no es válido.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (tarea.FechaLimite == default)
                return BadRequest("La fecha límite es obligatoria.");

            if (!ValidarPrioridad(tarea.Prioridad))
                return BadRequest("Prioridad no válida.");

            if (tarea.Estado != "Pendiente" &&
                tarea.Estado != "Completada")
                return BadRequest("Estado no válido.");

            var existente = await _tareaService.ObtenerPorIdAsync(id);

            if (existente == null)
                return NotFound("Tarea no encontrada.");

            tarea.Id = id;
            tarea.FechaCreacion = existente.FechaCreacion;

            var actualizado = await _tareaService.ActualizarAsync(
                id,
                tarea
            );

            if (!actualizado)
                return NotFound("No se pudo actualizar la tarea.");

            return Ok(tarea);
        }

        // DELETE: api/tareas/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(string id)
        {
            if (!ObjectId.TryParse(id, out _))
                return BadRequest("El ID no es válido.");

            var eliminado = await _tareaService.EliminarAsync(id);

            if (!eliminado)
                return NotFound("Tarea no encontrada.");

            return Ok(new
            {
                mensaje = "Tarea eliminada correctamente."
            });
        }

        private static bool ValidarPrioridad(string? prioridad)
        {
            return prioridad == "Baja" ||
                   prioridad == "Media" ||
                   prioridad == "Alta";
        }
    }
}
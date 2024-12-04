using AppEquiposBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AppEquiposBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayersController : ControllerBase
    {
        public readonly AplicationDbContext _context;

        public PlayersController(AplicationDbContext context)
        {
            _context = context;
        }
        // GET: api/<PlayersController>
        [HttpGet]
        public async Task<IActionResult> Get(string keyword = "")
        {

            try
            {
                var players = await _context.Players.Where(x => keyword.IsNullOrEmpty() ||
                    (!keyword.IsNullOrEmpty() &&
                        (x.FirstName.ToLower().Contains(keyword.ToLower()) ||
                         x.LastName.ToLower().Contains(keyword.ToLower())
                        )
                    )
                )
                .Include(j => j.Team)//garantiza que los Equipodatos relacionados (del equipo) también se carguen con cada jugador.
                .Select(j => new       //se utiliza para proyectar los datos en un nuevo objeto anónimo, seleccionando propiedades específicas de la Jugadoresentidad.
                {
                    j.Id,
                    j.FirstName,
                    j.LastName,
                    j.Age,
                    j.Country,
                    j.City,
                    j.Salary,
                    NombreEquipo = j.Team.Name
                }).ToListAsync();

                return Ok(players);
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }

        }

        // GET api/<PlayersController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                // Buscar al jugador por ID, incluyendo los datos del equipo relacionado
                var player = await _context.Players
                    .Include(j => j.Team) // Carga los datos relacionados del equipo
                    .Where(j => j.Id == id) // Filtra por el ID proporcionado
                    .Select(j => new
                    {
                        j.Id,
                        j.FirstName,
                        j.LastName,
                        j.Age,
                        j.Country,
                        j.City,
                        j.Salary,
                        j.TeamId,
                        NombreEquipo = j.Team.Name // Proyecta el nombre del equipo
                    })
                    .FirstOrDefaultAsync();//se utiliza para obtener el primer elemento que cumple con una condición específica en una consulta asíncrona

                // Si no se encuentra el jugador, retorna un 404
                if (player == null)
                {
                    return NotFound($"No se encontró un jugador con el ID: {id}");
                }

                // Retorna el jugador en formato JSON
                return Ok(player);
            }
            catch (Exception e)
            {
                return BadRequest($"Error al obtener el jugador: {e.Message}");
            }
        }

        // POST api/<PlayersController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Player player)
        {
            try
            {
                Console.WriteLine($"Recibido: {JsonSerializer.Serialize(player)}");

                // Primero, verifica si el ID del equipo es válido
                if (player.TeamId <= 0)
                {
                    return BadRequest("El ID del equipo debe ser mayor que 0");
                }

                // Verifica si el equipo existe
                var team = await _context.Teams.FindAsync(player.TeamId);
                if (team == null)
                {
                    return BadRequest($"No existe un equipo con el ID: {player.TeamId}");
                }

                

                // Asigna el equipo al jugador
                player.Team = team;

                // Agrega el jugador
                await _context.Players.AddAsync(player);

                // Guarda los cambios
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    mensaje = "Jugador creado exitosamente",
                    player = new
                    {
                        player.Id,
                        player.FirstName,
                        player.LastName,
                        player.Age,
                        player.Country,
                        player.City,
                        player.Salary,
                        player.TeamId,
                        team = new
                        {
                            player.Team.Id,
                            player.Team.Name
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al crear el jugador: {ex.Message}");
            }
        }

        // PUT api/<PlayersController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Player player)
        {
            try
            {
                // Verificar si el jugador existe en la base de datos
                var jugadorExistente = await _context.Players.FindAsync(id);
                if (jugadorExistente == null)
                {
                    return NotFound($"No se encontró un jugador con el ID: {id}");
                }

                // Validar que el equipo relacionado exista
                if (player.TeamId != jugadorExistente.TeamId) // Si cambió el equipo, verificar su existencia
                {
                    var equipo = await _context.Players.FindAsync(player.TeamId);
                    if (equipo == null)
                    {
                        return BadRequest($"No existe un equipo con el ID: {player.TeamId}");
                    }
                }
                //comentario test
                // Actualizar los datos del jugador
                jugadorExistente.FirstName = player.FirstName ?? jugadorExistente.FirstName;//?? : si no es null devuelve el primer operando y si es null ,el segundo
                jugadorExistente.LastName = player.LastName ?? jugadorExistente.LastName;
                jugadorExistente.Age = player.Age > 0 ? player.Age : jugadorExistente.Age;
                jugadorExistente.Country = player.Country ?? jugadorExistente.Country;
                jugadorExistente.City = player.City ?? jugadorExistente.City;
                jugadorExistente.Salary = player.Salary >= 0 ? player.Salary : jugadorExistente.Salary;
                jugadorExistente.TeamId = player.TeamId;

                // Guardar cambios
                _context.Players.Update(jugadorExistente);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    mensaje = "Jugador actualizado exitosamente",
                    jugador = new
                    {
                        jugadorExistente.Id,
                        jugadorExistente.FirstName,
                        jugadorExistente.LastName,
                        jugadorExistente.Age,
                        jugadorExistente.Country,
                        jugadorExistente.City,
                        jugadorExistente.Salary,
                        jugadorExistente.TeamId
                    }
                });
            }
            catch (Exception e)
            {
                return BadRequest($"Error al actualizar el jugador: {e.Message}");
            }
        }


        // DELETE api/<PlayersController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var player = await _context.Players.FindAsync(id);
                if (player == null)
                {
                    return NotFound();
                }
                _context.Players.Remove(player);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Equipo eliminado con éxito" });

            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }
    }
}

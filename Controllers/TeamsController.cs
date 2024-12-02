using AppEquiposBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AppEquiposBackend.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class TeamsController : ControllerBase
    {
        public readonly AplicationDbContext _context;
        public TeamsController(AplicationDbContext context)
        {
            _context = context;
        }
        // GET: api/<TeamsController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var listTeams = await _context.Teams.ToListAsync();
                return Ok(listTeams);
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }
        // GET api/<TeamsController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var team = await _context.Teams.FindAsync(id);

                if (team == null)
                {
                    return NotFound(new { message = "Equipo no encontrado" });
                }
                return Ok(team);
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }
        // POST api/<TeamsController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Team team)
        {
            try
            {
                if (await _context.Teams.AnyAsync(x => x.Name.Contains(team.Name)))
                {
                    return NotFound(new { message = "el equipo ya existe" });
                }
                _context.Add(team);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }


        // PUT api/<TeamsController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Team team)
        {
            try
            {

                if (id != team.Id)
                {
                    return NotFound();
                }
                else if (await _context.Teams.AnyAsync(x => x.Name.Contains(team.Name) && x.Id != id))
                {
                    return NotFound(new { message = "Ya existe un equipo con ese nombre" });
                }
                _context.Update(team);
                await _context.SaveChangesAsync();
                return Ok(new { message = "El equipo fue actualizado con éxito" });
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }
        // DELETE api/<TeamsController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var team = await _context.Teams.FindAsync(id);

                if (team == null)
                {
                    return NotFound();
                }
                else
                {
                    _context.Teams.Remove(team);
                    await _context.SaveChangesAsync();
                    return Ok(new { message = "El equipo fue eliminado con éxito" });
                }


            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }
    }
}

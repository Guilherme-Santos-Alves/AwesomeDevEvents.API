using AwesomeDevEvents.API.Entities;
using AwesomeDevEvents.API.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AwesomeDevEvents.API.Controllers
{
    [Route("api/DevEvents")]
    [ApiController]
    public class DevEventsController : ControllerBase
    {
        private readonly DevEventsDbContext _context;
        public DevEventsController(DevEventsDbContext context)
        {
            this._context = context;
        }
        [HttpGet] 
        public IActionResult GetAll()
        {
            var devEvents = _context.DevEvents.Where(d => !d.IsDeleted).ToList();

            return Ok(devEvents);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            var devEvent = _context.DevEvents
                .Include(d => d.Speakers)
                .SingleOrDefault(d => d.Id == id);

            if (devEvent == null)
            {
                return NotFound();
            }

            return Ok(devEvent);
        }

        [HttpPost]
        public IActionResult Post(DevEvent devEvent)
        {
            _context.DevEvents.Add(devEvent);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new {id  = devEvent.Id}, devEvent);
        }
        
        [HttpPut("{id}")]
        public IActionResult Update(Guid id, DevEvent input)
        {
            var devEvent = _context.DevEvents.SingleOrDefault(d => d.Id == id);

            if (devEvent == null)
            {
                return NotFound();
            }

            devEvent.Update(input.Title, input.Description, input.StartDate, input.EndDate);

            _context.Update(devEvent);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var devEvent = _context.DevEvents.SingleOrDefault(d => d.Id == id);

            if (devEvent == null)
            {
                return NotFound();
            }

            devEvent.Delete();
            _context.SaveChanges();

            return NoContent();
        }

        [HttpPost("{id}/speakers")]
        public IActionResult PostSpeaker(Guid id, DevEventSpeaker devEventSpeaker)
        {
            devEventSpeaker.DevEventId = id;

            var devEvent = _context.DevEvents.SingleOrDefault(d => d.Id == id);

            if (devEvent == null)
            {
                return NotFound();
            }

            _context.DevEventSpeakers.Add(devEventSpeaker);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = devEventSpeaker.Id }, devEventSpeaker);
        }
    }
}
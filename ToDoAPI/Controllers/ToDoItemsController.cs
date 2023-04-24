using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoApi.Models;

namespace ToDoAPI.Controllers
{
    // template string using the controller's Route attribute
    [Route("api/[controller]")]
    // marks the class with the ApiController attribute
    // indicates that the controller repsonds to web API requests
    [ApiController]
    // here, ToDoItems replaces [controller] in the template string (ASP.NET Core routing is case insensitive)
    public class ToDoItemsController : ControllerBase
    {
        // uses the dependency injection to inject the database context into the controller
        private readonly ToDoContext _context;

        public ToDoItemsController(ToDoContext context)
        {
            _context = context;
        }

        // GET: api/ToDoItems
        [HttpGet]
        // the return type is ActionResult, which represents a wide range of HTTP status codes (e.g. 404, 200, etc.)
        // ASP.NET Core automatically serializes the object into JSON and writes it to the body of the response
        // the response code is typicall 200 unless they are unhandled exceptions, which are 5xx errors
        public async Task<ActionResult<IEnumerable<ToDoItem>>> GetToDoItems()
        {
          if (_context.ToDoItems == null)
          {
              return NotFound();
          }
            return await _context.ToDoItems.ToListAsync();
        }

        // individual route attributes can have templates too
        // they can be hardcoded or dynamic like the one below
        // GET: api/ToDoItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ToDoItem>> GetToDoItem(long id)
        {
          if (_context.ToDoItems == null)
          {
              return NotFound();
          }
            var toDoItem = await _context.ToDoItems.FindAsync(id);

            if (toDoItem == null)
            {
                return NotFound();
            }

            return toDoItem;
        }

        // the response code for a put request is 204 (no content)
        // according to HTTP specification, a PUT request requires the client to send the entire updated entity, not just changes
        // but you can use HTTP PATCH to support partial udpates
        // PUT: api/ToDoItems/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutToDoItem(long id, ToDoItem toDoItem)
        {
            if (id != toDoItem.Id)
            {
                return BadRequest();
            }

            _context.Entry(toDoItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ToDoItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/ToDoItems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ToDoItem>> PostToDoItem(ToDoItem toDoItem)
        {
          if (_context.ToDoItems == null)
          {
              return Problem("Entity set 'ToDoContext.ToDoItems'  is null.");
          }
            _context.ToDoItems.Add(toDoItem);
            await _context.SaveChangesAsync();

            //return CreatedAtAction("GetToDoItem", new { id = toDoItem.Id }, toDoItem);

            // retrieves the value of the to-do item from the body of the HTTP request
            return CreatedAtAction(nameof(GetToDoItem), new { id = toDoItem.Id }, toDoItem);
            // the CreatedAtAction method returns an HTTP status code of 201 (standard response for an HTTP POST method)
            // it also adds a location header to the response, which specifies the URI of the new item
            // it also references the GetToDoItem action to create the Location header's URI
            // the nameof keyword is used to avoid hard-coding the actio nname in the CreatedAction call
        }

        // DELETE: api/ToDoItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteToDoItem(long id)
        {
            if (_context.ToDoItems == null)
            {
                return NotFound();
            }
            var toDoItem = await _context.ToDoItems.FindAsync(id);
            if (toDoItem == null)
            {
                return NotFound();
            }

            _context.ToDoItems.Remove(toDoItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ToDoItemExists(long id)
        {
            return (_context.ToDoItems?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

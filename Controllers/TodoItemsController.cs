using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TodoApi.Interfaces;
using TodoApi.Models;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    internal sealed class TodoItemsController : ControllerBase
    {
        private readonly ITodoService _todoService;

        public TodoItemsController(ITodoService todoService)
        {
            _todoService = todoService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItemDTO>>> GetTodoItems()
        {
            var ct = HttpContext.RequestAborted;
            var todos = await _todoService.GetAllTodos(ct);

            return todos;
        }

        [HttpGet("{id:long}")]
        public async Task<ActionResult<TodoItemDTO>> GetTodoItem(long id)
        {
            var ct = HttpContext.RequestAborted;
            var result = await _todoService.GetTodoById(id, ct);

            return result.Match<ActionResult<TodoItemDTO>>(
                item => item,
                notFound => NotFound()
            );
        }

        [HttpPut("{id:long}")]
        public async Task<IActionResult> UpdateTodoItem(long id, TodoItemDTO dto)
        {
            var ct = HttpContext.RequestAborted;
            var result = await _todoService.UpdateTodo(id, dto, ct);
            
            return result.Match<IActionResult>(
                item => NoContent(),
                notFound => NotFound()
            );
        }

        [HttpPost]
        public async Task<ActionResult<TodoItemDTO>> CreateTodoItem(TodoItemDTO dto)
        {
            var ct = HttpContext.RequestAborted;
            var result = await _todoService.CreateTodo(dto, ct);

            return result.Match<ActionResult<TodoItemDTO>>(
                item => CreatedAtAction(
                    nameof(GetTodoItem),
                    new { id = item.Id },
                    item),
                exists => Conflict()
            );
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> DeleteTodoItem(long id)
        {
            var ct = HttpContext.RequestAborted;
            var result = await _todoService.DeleteTodo(id, ct);

            return result.Match<IActionResult>(
                deletedId => NoContent(),
                notFound => NotFound()
            );
        }
    }
}

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OneOf;
using TodoApi.Errors;
using TodoApi.Models;

namespace TodoApi.Interfaces
{
    internal interface ITodoService
    {
        Task<List<TodoItemDTO>> GetAllTodos(CancellationToken ct);
        Task<OneOf<TodoItemDTO, TodoNotFoundError>> GetTodoById(long id, CancellationToken ct);
        Task<OneOf<TodoItemDTO, TodoNotFoundError>> UpdateTodo(long id, TodoItemDTO dto, CancellationToken ct);
        Task<OneOf<TodoItemDTO, TodoAlreadyExists>> CreateTodo(TodoItemDTO dto, CancellationToken ct);
        Task<OneOf<long, TodoNotFoundError>> DeleteTodo(long id, CancellationToken ct);
    }
}
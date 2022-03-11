using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OneOf;
using TodoApi.Errors;
using TodoApi.Interfaces;
using TodoApi.Models;

namespace TodoApi.Services
{
    internal sealed class TodoService : ITodoService
    {
        private readonly ITodoRepository _repository;

        public TodoService(ITodoRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<TodoItemDTO>> GetAllTodos(CancellationToken ct) => 
            await _repository.GetAllTodos().Select(e => e.ToDto()).ToListAsync(ct);

        public async Task<OneOf<TodoItemDTO, TodoNotFoundError>> GetTodoById(long id, CancellationToken ct)
        {
            var todo = await _repository.Find(id, ct);
            
            if (todo is null) return new TodoNotFoundError(id);

            return todo.ToDto();
        }

        public async Task<OneOf<TodoItemDTO, TodoNotFoundError>> UpdateTodo(long id, TodoItemDTO dto, CancellationToken ct)
        {
            var item = await _repository.Find(id, ct);
            if (item is null) return new TodoNotFoundError(id);

            try
            {
                await _repository.Update(item, ct);
            }
            catch (DBConcurrencyException) when (!_repository.Contains(item.Id))
            {
                return new TodoNotFoundError(id);
            }

            return dto;
        }

        public async Task<OneOf<TodoItemDTO, TodoAlreadyExists>> CreateTodo(TodoItemDTO dto, CancellationToken ct)
        {
            if (_repository.Contains(dto.Id)) return new TodoAlreadyExists(dto.Id);
            
            var item = TodoItem.FromDto(dto);

            try
            {
                await _repository.Create(item, ct);
            }
            catch (DBConcurrencyException) when (_repository.Contains(item.Id))
            {
                return new TodoAlreadyExists(dto.Id);
            }

            return dto;
        }

        public async Task<OneOf<long, TodoNotFoundError>> DeleteTodo(long id, CancellationToken ct)
        {
            var item = await _repository.Find(id, ct);
            if (item is null) return new TodoNotFoundError(id);

            try
            {
                await _repository.Delete(item, ct);
            }
            catch (DBConcurrencyException) when (!_repository.Contains(item.Id))
            {
                return new TodoNotFoundError(id);
            }

            return id;
        }
    }
}
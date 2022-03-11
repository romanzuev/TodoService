using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using TodoApi.Errors;
using TodoApi.Models;
using TodoApi.Services;
using Xunit;

namespace Tests
{
    public class TodoServiceTests
    {
        private readonly TodoService _service;
        private readonly ITodoRepository _repoMock;

        public TodoServiceTests()
        {
            _repoMock = Substitute.For<ITodoRepository>();
            _service = new TodoService(_repoMock);
        }
        
        [Theory]
        [InlineData(1, "test1", false)]
        [InlineData(999, "", true)]
        public async Task ShouldCreateTodo(long id, string name, bool isCompleted)
        {
            _repoMock.Contains(Arg.Any<long>()).ReturnsForAnyArgs(false);
            _repoMock.Create(Arg.Any<TodoItem>(), Arg.Any<CancellationToken>()).ReturnsForAnyArgs(Task.CompletedTask);
            var dto = new TodoItemDTO { Id = id, Name = name, IsComplete = isCompleted };

            var actual = await _service.CreateTodo(dto, CancellationToken.None);

            actual.Value.Should().BeEquivalentTo(dto);
            await _repoMock.Received()
                .Create(Arg.Is<TodoItem>(e => e.Id == dto.Id && e.Name == dto.Name && e.IsComplete == dto.IsComplete),
                    Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task ShouldReturnAlreadyExistsIfExists()
        {
            _repoMock.Contains(Arg.Any<long>()).ReturnsForAnyArgs(true);
            _repoMock.Create(Arg.Any<TodoItem>(), Arg.Any<CancellationToken>()).ReturnsForAnyArgs(Task.CompletedTask);
            var dto = new TodoItemDTO { Id = 2, Name = "test", IsComplete = true };

            var actual = await _service.CreateTodo(dto, CancellationToken.None);

            actual.Value.Should().BeOfType<TodoAlreadyExists>();
            _repoMock.Received().Contains(dto.Id);
        }
        
        [Fact]
        public async Task ShouldReturnAlreadyExistsIfAddedConcurrently()
        {
            _repoMock.Contains(Arg.Any<long>()).Returns(x => false, x => true);
            _repoMock.Create(Arg.Any<TodoItem>(), Arg.Any<CancellationToken>()).ThrowsForAnyArgs(new DBConcurrencyException());
            var dto = new TodoItemDTO { Id = 2, Name = "test", IsComplete = true };

            var actual = await _service.CreateTodo(dto, CancellationToken.None);

            actual.Value.Should().BeOfType<TodoAlreadyExists>();
            _repoMock.Received(2).Contains(dto.Id);
            await _repoMock.Received()
                .Create(Arg.Is<TodoItem>(e => e.Id == dto.Id && e.Name == dto.Name && e.IsComplete == dto.IsComplete),
                    Arg.Any<CancellationToken>());
        }
        
        [Fact]
        public async Task ShouldThrowExceptionIfDontExistsWithinException()
        {
            _repoMock.Contains(Arg.Any<long>()).ReturnsForAnyArgs(x => false);
            _repoMock.Create(Arg.Any<TodoItem>(), Arg.Any<CancellationToken>()).ThrowsForAnyArgs(new DBConcurrencyException("test"));
            var dto = new TodoItemDTO { Id = 2, Name = "test", IsComplete = true };

            Func<Task> act = () => _service.CreateTodo(dto, CancellationToken.None);

            await act.Should().ThrowAsync<DBConcurrencyException>().WithMessage("test");
            _repoMock.Received().Contains(dto.Id);
        }
        
        //todo write tests for other methods
    }
}
namespace TodoApi.Models
{
    #region snippet
    internal class TodoItem
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool IsComplete { get; set; }
        public string Secret { get; set; }

        public TodoItemDTO ToDto() =>
            new TodoItemDTO
            {
                Id = Id,
                Name = Name,
                IsComplete = IsComplete
            };

        public static TodoItem FromDto(TodoItemDTO dto) =>
            new TodoItem
            {
                Id = dto.Id,
                Name = dto.Name,
                IsComplete = dto.IsComplete
            };
    }
    #endregion
}
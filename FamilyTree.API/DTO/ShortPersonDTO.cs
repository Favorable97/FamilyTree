namespace FamilyTree.API.DTO
{
    /// <summary>
    /// Краткий DTO для передачи информации о человеке и его родителях в карточку персоны
    /// </summary>
    public record ShortPersonDTO
    {
        /// <summary>
        /// Уникальный идентификатор
        /// </summary>
        public Guid Id { get; init; }
        /// <summary>
        /// Фамилия
        /// </summary>
        public string LastName { get; init; } = string.Empty!;
        /// <summary>
        /// Имя
        /// </summary>
        public string FirstName { get; init; } = string.Empty!;
        /// <summary>
        /// Отчество
        /// </summary>
        public string? MiddleName { get; init; }
        /// <summary>
        /// Дата рождения
        /// </summary>
        public DateTime BirthDate { get; init; }
        /// <summary>
        /// Дата смерти
        /// </summary>
        public DateTime? DeathDate { get; init; }
    }
}

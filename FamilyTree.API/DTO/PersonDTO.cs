namespace FamilyTree.API.DTO
{
    /// <summary>
    /// DTO для передачи информации о человеке и его родителях
    /// </summary>
    public record PersonDTO
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
        /// <summary>
        /// Пол
        /// </summary>
        public Gender Gender { get; init; }
        /// <summary>
        /// Сссылка на маму человека
        /// </summary>
        public PersonDTO? Mother { get; init; }
        /// <summary>
        /// Ссылка на отца человека
        /// </summary>
        public PersonDTO? Father { get; init; }
    }
}

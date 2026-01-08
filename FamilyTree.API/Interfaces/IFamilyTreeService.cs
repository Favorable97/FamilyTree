namespace FamilyTree.API.Interfaces
{
    public interface IFamilyTreeService
    {
        /// <summary>
        /// Получаем родителей по Id человека
        /// </summary>
        /// <param name="personId">Человек, у которого ищем родителей</param>
        /// <returns></returns>
        public Task<(ShortPersonDTO? Mother, ShortPersonDTO? Father)> GetParentsAsync(Guid personId);

        /// <summary>
        /// Получаем детей по Id человека
        /// </summary>
        /// <param name="personId">Персона, у которой ищем детей</param>
        /// <returns></returns>
        public Task<List<ShortPersonDTO>> GetChildrenAsync(Guid personId);

        /// <summary>
        /// Получаем всех предков по Id человека
        /// </summary>
        /// <param name="personId">Персона, у которой ищем всех предков</param>
        /// <param name="maxDepth">Максимальная глубина поиска</param>
        /// <returns></returns>
        public Task<List<ShortPersonDTO>> GetAncestorsAsync(Guid personId, int maxDepth = 0);

        /// <summary>
        /// Получаем всех потомков по Id человека
        /// </summary>
        /// <param name="personId">Персона, у которой ищем всех потомков</param>
        /// <param name="maxDepth">Максимальная глубина поиска</param>
        /// <returns></returns>
        public Task<List<ShortPersonDTO>> GetDescendantsAsync(Guid personId, int maxDepth = 0);
    }
}

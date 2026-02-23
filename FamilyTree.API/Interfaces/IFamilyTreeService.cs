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
        /// Получение братьев и сестер
        /// </summary>
        /// <param name="personId">Персона, чьх братьев и сестер ищем</param>
        /// <returns></returns>
        public Task<List<ShortPersonDTO>> GetSiblingsAsync(Guid personId);

        /// <summary>
        /// Получить дядь и теть человека
        /// </summary>
        /// <param name="personId">Персона, чьих дядь и теть ищем</param>
        /// <returns></returns>
        public Task<List<ShortPersonDTO>> GetUnclesAndAuntAsync(Guid personId);

        /// <summary>
        /// Создание дерева из предков и потомков
        /// </summary>
        /// <param name="personId">Человек, по которому строится дерево</param>
        /// <param name="maxDepthParents">Глубина поиска предков</param>
        /// <param name="maxDepthChildren">Глубина поиска потомков</param>
        /// <returns></returns>
        public Task<PersonTreeNodeDTO> GetPersonTreeAsync(Guid personId, int maxDepthParents, int maxDepthChildren);
    }
}

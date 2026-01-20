
using FamilyTree.API.Mappers;
using FamilyTree.Data.Interfaces;
using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel;

namespace FamilyTree.API.Services
{
    public record FamilyTreeService(IPersonRepository repository) : IFamilyTreeService
    {
        private readonly IPersonRepository _repository = repository;

        public async Task<(ShortPersonDTO? Mother, ShortPersonDTO? Father)> GetParentsAsync(Guid personId)
        {
            var person = await _repository.GetPersonByIdAsync(personId) ?? throw new Exception($"Человек с Id: {personId} не найден!");

            var mother = person.MotherID != null ? await _repository.GetPersonByIdAsync(person.MotherID.Value) : null;

            var father = person.FatherID != null ? await _repository.GetPersonByIdAsync(person.FatherID.Value) : null;

            var motherDTO = mother != null ? PersonMapper.MapToShortPersonDTO(mother) : null;

            var fatherDTO = father != null ? PersonMapper.MapToShortPersonDTO(father) : null; 
            
            return (motherDTO, fatherDTO);
        }

        public async Task<List<ShortPersonDTO>> GetChildrenAsync(Guid personId)
        {
            var isExists = await _repository.ExistsByIdAsync(personId);

            if (!isExists)
                throw new Exception($"Человек с Id: {personId} не найден!");

            var childList = await _repository.GetChildrenAsync(personId);

            var children = childList.Count > 0 ? childList.Select(x => PersonMapper.MapToShortPersonDTO(x)).ToList() : [];

            return children;
        }

        public async Task<List<ShortPersonDTO>> GetAncestorsAsync(Guid personId, int maxDepth = 0)
        {
            // список предков
            List<ShortPersonDTO> ancestorsList = [];

            // список людей на каждом уровне
            List<ShortPersonDTO> processingPeople = [];

            int currentDepth = 0;

            if (maxDepth < 0)
                throw new Exception("Глубина поиска не может быть отрицательной");

            var parents = await GetParentsAsync(personId);

            if (parents.Mother != null)
            {
                processingPeople.Add(parents.Mother);
                ancestorsList.Add(parents.Mother);
            }
                

            if (parents.Father != null)
            {
                processingPeople.Add(parents.Father);
                ancestorsList.Add(parents.Father);
            }
                

            while (processingPeople.Count > 0 && (maxDepth == 0 || currentDepth < maxDepth))
            {
                List<ShortPersonDTO> findPeople = [];
                foreach (var person in processingPeople)
                {
                    parents = await GetParentsAsync(person.Id);

                    if (parents.Mother != null)
                    {
                        if (!ancestorsList.Any(x => x.Id == parents.Mother.Id))
                        {
                            findPeople.Add(parents.Mother);
                            ancestorsList.Add(parents.Mother);
                        }
                    }


                    if (parents.Father != null)
                    {
                        if (!ancestorsList.Any(x => x.Id == parents.Father.Id))
                        {
                            findPeople.Add(parents.Father);
                            ancestorsList.Add(parents.Father);
                        }
                    }
                }

                processingPeople.Clear();

                processingPeople.AddRange(findPeople);

                currentDepth++;
            }

            return ancestorsList;
        }

        public async Task<List<ShortPersonDTO>> GetDescendantsAsync(Guid personId, int maxDepth = 0)
        {
            // список предков
            List<ShortPersonDTO> descendantsList = [];

            // список людей на каждом уровне
            List<ShortPersonDTO> processingPeople = [];

            int currentDepth = 0;

            if (maxDepth < 0)
                throw new Exception("Глубина поиска не может быть отрицательной");

            var children = await GetChildrenAsync(personId);

            
            if (children.Count > 0)
            {
                processingPeople.AddRange(children);
                descendantsList.AddRange(children);
            }


            while (processingPeople.Count > 0 && (maxDepth == 0 || currentDepth < maxDepth))
            {
                List<ShortPersonDTO> findPeople = [];
                foreach (var person in processingPeople)
                {
                    children = await GetChildrenAsync(person.Id);

                    if (children.Count > 0)
                    {
                        foreach (var child in children)
                        {
                            if (!descendantsList.Any(x => x.Id == child.Id))
                            {
                                findPeople.Add(child);
                                descendantsList.Add(child);
                            }
                        }
                    }
                }

                processingPeople.Clear();

                processingPeople.AddRange(findPeople);

                currentDepth++;
            }

            return descendantsList;
        }

        public async Task<PersonTreeNodeDTO> GetPersonTreeAsync(Guid personId, int maxDepthParents, int maxDepthChildren)
        {
            var root = await CreateNode(personId);

            HashSet<Guid> visited = [];

            await BuildParents(root, maxDepthParents, visited);

            visited.Clear();

            await BuildChildren(root, maxDepthChildren, visited);

            return root;
        }

        private async Task<PersonTreeNodeDTO> CreateNode(Guid personId)
        {
            var person = await _repository.GetPersonByIdAsync(personId);

            PersonTreeNodeDTO root = new()
            {
                Person = PersonMapper.MapToShortPersonDTO(person!),
                Parents = [],
                Children = []
            };

            return root;
        }

        private async Task BuildParents(PersonTreeNodeDTO node, int depth, HashSet<Guid> visited)
        {
            if (depth == 0 || !visited.Add(node.Person.Id))
                return;

            var (Mother, Father) = await GetParentsAsync(node.Person.Id);

            if (Mother != null)
            {
                PersonTreeNodeDTO mother = await CreateNode(Mother.Id);
                
                node.Parents.Add(mother);

                await BuildParents(mother, depth - 1, visited);
            }

            if (Father != null)
            {
                PersonTreeNodeDTO father = await CreateNode(Father.Id);

                node.Parents.Add(father);

                await BuildParents(father, depth - 1, visited);
            }
        }

        private async Task BuildChildren(PersonTreeNodeDTO node, int depth, HashSet<Guid> visited)
        {
            if (depth == 0 || !visited.Add(node.Person.Id))
                return;

            var childrenList = await GetChildrenAsync(node.Person.Id);

            foreach (var child in childrenList)
            {
                var childNode = await CreateNode(child.Id);

                node.Children.Add(childNode);

                await BuildChildren(childNode, depth - 1, visited);
            }
        }
    }
}

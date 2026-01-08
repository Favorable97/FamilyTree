
using FamilyTree.API.Mappers;
using FamilyTree.Data.Interfaces;

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
    }
}

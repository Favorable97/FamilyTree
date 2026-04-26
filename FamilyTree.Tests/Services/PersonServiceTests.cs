using FamilyTree.API.DTO;
using FamilyTree.API.Errors;
using FamilyTree.API.Interfaces;
using FamilyTree.API.Services;
using FamilyTree.Data.Interfaces;
using FamilyTree.Data.Models;
using FamilyTree.Data.Utils;
using FluentAssertions;
using Moq;
using Xunit;


namespace FamilyTree.Tests.Services
{
    public class PersonServiceTests
    {
        [Fact]
        public async Task CreatePerson_ShouldThrow_WhenPersonAlreadyExists()
        {
            var repo = new Mock<IPersonRepository>();
            var lifeEvent = new Mock<ILifeEventService>();

            repo
                .Setup(r => r.ExistsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<DateTime>()))
                .ReturnsAsync(true);

            var service = new PersonService(repo.Object, lifeEvent.Object);

            var dto = new RequestAddPersonDTO
            {
                LastName = "Иванов",
                FirstName = "Иван",
                MiddleName = "Иванович",
                BirthDate = new(1995, 10, 2),
                Gender = Data.Utils.Gender.Male
            };

            Func<Task> act = () => service.CreatePersonAsync(dto);

            await act.Should().ThrowAsync<PersonAlreadyExistsException>();
        }

        [Fact]
        public async Task GetAllPerson_ShouldReturnList_WhenPersonsExist()
        {
            var people = new List<Person>
            {
                new() { Id = Guid.NewGuid(), LastName = "Иванов", FirstName = "Иван", BirthDate = new(1990,1,1) },
                new() { Id = Guid.NewGuid(), LastName = "Петров", FirstName = "Петр", BirthDate = new(1992,2,2) }
            };

            var repo = new Mock<IPersonRepository>();

            var lifeEvent = new Mock<ILifeEventService>();

            repo
                .Setup(r => r.GetAllPersonAsync())
                .ReturnsAsync(people);

            var service = new PersonService(repo.Object, lifeEvent.Object);

            var result = await service.GetAllPersonAsync();

            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result[0].Id.Should().Be(people[0].Id);
        }

        [Fact]
        public async Task GetPersonById_ShouldReturnPersonWithParents_WhenParentsExist()
        {
            var repo = new Mock<IPersonRepository>();

            var lifeEvent = new Mock<ILifeEventService>();

            Guid personId = Guid.NewGuid(),
                motherId = Guid.NewGuid(),
                fatherId = Guid.NewGuid();

            var person = new Person()
            {
                Id = personId,
                MotherID = motherId,
                FatherID = fatherId
            };

            var mother = new Person()
            {
                Id = motherId
            };

            var father = new Person()
            {
                Id = fatherId
            };


            repo
                .Setup(r => r.GetPersonByIdAsync(personId))
                .ReturnsAsync(person);

            repo
                .Setup(r => r.GetPersonByIdAsync(motherId))
                .ReturnsAsync(mother);

            repo
                .Setup(r => r.GetPersonByIdAsync(fatherId))
                .ReturnsAsync(father);

            var service = new PersonService(repo.Object, lifeEvent.Object);

            var result = await service.GetPersonByIdAsync(personId);

            result.Should().NotBeNull();
            result.Mother.Should().NotBeNull();
            result.Father.Should().NotBeNull();

            result.Id.Should().Be(personId);
            result.Mother.Id.Should().Be(motherId);
            result.Father.Id.Should().Be(fatherId);
        }

        [Fact]
        public async Task GetPersonById_ShouldThrow_WhenNotFound()
        {
            var repo = new Mock<IPersonRepository>();

            var lifeEvent = new Mock<ILifeEventService>();

            repo
                .Setup(r => r.GetPersonByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Person)null!);

            var service = new PersonService(repo.Object, lifeEvent.Object);

            var personId = Guid.NewGuid();

            Func<Task> act = () => service.GetPersonByIdAsync(personId);

            await act.Should().ThrowAsync<PersonNotFoundException>();
        }

        [Fact]
        public async Task GetShortPersonById_ShouldReturnShortPerson_WhenExists()
        {
            var repo = new Mock<IPersonRepository>();

            var lifeEvent = new Mock<ILifeEventService>();

            var dto = new Person
            {
                Id = Guid.NewGuid(),
                LastName = "Ivanov",
                FirstName = "Ivan",
                BirthDate = DateTime.Now,
            };

            repo
                .Setup(r => r.GetPersonByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(dto);

            var service = new PersonService(repo.Object, lifeEvent.Object);

            var personId = Guid.NewGuid();

            var result = await service.GetShortPersonByIdAsync(personId);

            result.Should().NotBeNull();
            result.Should().BeOfType<ShortPersonDTO>();
        }

        [Fact]
        public async Task CreatePerson_ShouldCreatePerson_WhenDataValid()
        {
            var repo = new Mock<IPersonRepository>();

            var lifeEvent = new Mock<ILifeEventService>();

            var dto = new Person()
            {
                Id = Guid.NewGuid(),
                LastName = "Ivanov",
                FirstName = "Ivan",
                BirthDate = new(2000, 11, 24),
                Gender = Data.Utils.Gender.Male
            };

            repo
                .Setup(r => r.CreatePersonAsync(dto))
                .Returns(Task.CompletedTask);

            repo
                .Setup(r => r.ExistsAsync(dto.LastName, dto.FirstName, dto.MiddleName, dto.BirthDate))
                .ReturnsAsync(false);

            var data = new RequestAddPersonDTO()
            {
                LastName = dto.LastName,
                FirstName = dto.FirstName,
                BirthDate = dto.BirthDate,
                Gender = dto.Gender
            };

            var service = new PersonService(repo.Object, lifeEvent.Object);

            var result = service.CreatePersonAsync(data);

            result.Should().NotBeNull();

            repo.Verify(v => v.CreatePersonAsync(It.Is<Person>(m =>
                m.FirstName == dto.FirstName &&
                m.LastName == dto.LastName &&
                m.BirthDate == dto.BirthDate
            )), Times.Once);
        }

        [Fact]
        public async Task CreatePerson_ShouldAddDeathEvent_WhenDeathDateProvided()
        {
            var repo = new Mock<IPersonRepository>();

            var lifeEvent = new Mock<ILifeEventService>();

            var person = new Person()
            {
                Id = Guid.NewGuid(),
                LastName = "Ivanov",
                FirstName = "Ivan",
                BirthDate = new(2000, 11, 24),
                DeathDate = new(2021, 1, 1),
                Gender = Data.Utils.Gender.Male
            };

            repo
                .Setup(r => r.CreatePersonAsync(person))
                .Returns(Task.CompletedTask);

            repo
                .Setup(r => r.ExistsAsync(person.LastName, person.FirstName, person.MiddleName, person.BirthDate))
                .ReturnsAsync(false);

            var data = new RequestAddPersonDTO()
            {
                LastName = person.LastName,
                FirstName = person.FirstName,
                BirthDate = person.BirthDate,
                DeathDate = new(2021, 1, 1),
                Gender = person.Gender
            };

            var service = new PersonService(repo.Object, lifeEvent.Object);

            var result = await service.CreatePersonAsync(data);

            result.Should().NotBeNull();

            lifeEvent.Verify(s => s.AddEventAsync(
                result.Id,
                LifeEventType.Death,
                person.DeathDate.Value
            ), Times.Once);
        }

        [Fact]
        public async Task UpdatePerson_ShouldThrow_WhenPersonNotFound()
        {
            var repo = new Mock<IPersonRepository>();

            var lifeEvent = new Mock<ILifeEventService>();

            repo
                .Setup(r => r.GetPersonByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Person?)null);

            var id = Guid.NewGuid();

            var dto = new RequestUpdatePersonDTO()
            {
                LastName = "Ivanov",
                FirstName = "Ivan",
                BirthDate = new(2000, 11, 24),
                DeathDate = new(2021, 1, 1),
                Gender = Data.Utils.Gender.Male
            };

            var service = new PersonService(repo.Object, lifeEvent.Object);

            Func<Task> act = () => service.UpdatePersonAsync(id, dto);

            await act.Should().ThrowAsync<PersonNotFoundException>();
        }

        [Fact]
        public async Task UpdatePerson_ShouldAddDeathEvent_WhenDeathDateProvidedAndNotExists()
        {
            var repo = new Mock<IPersonRepository>();

            var lifeEvent = new Mock<ILifeEventService>();

            var person = new Person()
            {
                LastName = "Ivanov",
                FirstName = "Ivan",
                BirthDate = new(1922, 3, 26),
                Gender = Data.Utils.Gender.Male
            };

            repo
                .Setup(r => r.GetPersonByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(person);

            lifeEvent
                .Setup(le => le.GetTimelineAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new List<LifeEventDTO>());

            var dto = new RequestUpdatePersonDTO()
            {
                DeathDate = new(2001, 5, 2)
            };

            var service = new PersonService(repo.Object, lifeEvent.Object);

            var result = await service.UpdatePersonAsync(person.Id, dto);

            result.Should().NotBeNull();

            lifeEvent.Verify(s => s.AddEventAsync(
                result.Id,
                LifeEventType.Death,
                result.DeathDate.Value
            ), Times.Once);
        }

        [Fact]
        public async Task DeletePerson_ShouldThrow_WhenPersonIsParent()
        {
            var repo = new Mock<IPersonRepository>();

            var lifeEvent = new Mock<ILifeEventService>();

            var id = Guid.NewGuid();

            repo
                .Setup(r => r.GetPersonByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new Person());

            repo
                .Setup(r => r.IsParentAsync(It.IsAny<Guid>()))
                .ReturnsAsync(true);

            var service = new PersonService(repo.Object, lifeEvent.Object);

            Func<Task> act = () => service.DeletePersonAsync(id);

            await act.Should().ThrowAsync<PersonIsParentException>();
        }
    }
}

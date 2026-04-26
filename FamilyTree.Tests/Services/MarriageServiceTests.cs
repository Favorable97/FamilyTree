using FamilyTree.API.DTO;
using FamilyTree.API.Errors;
using FamilyTree.API.Services;
using FamilyTree.Data.Interfaces;
using Moq;
using FluentAssertions;
using Xunit;
using FamilyTree.API.Interfaces;
using FamilyTree.Data.Utils;
using Microsoft.AspNetCore.Components.Forms;
using FamilyTree.Data.Models;

namespace FamilyTree.Tests.Services
{
    public class MarriageServiceTests
    {
        [Fact]
        public async Task CreateMarriage_ShouldThrow_WhenSpousesAreSame()
        {
            var repo = new Mock<IMarriageRepository>();
            var personService = new Mock<IPersonService>();
            var lifeEvent = new Mock<ILifeEventService>();

            var sameId = Guid.NewGuid();

            personService
                .Setup(s => s.GetShortPersonByIdAsync(sameId))
                .ReturnsAsync(new ShortPersonDTO()
                {
                    Id = sameId,
                    LastName = "Иванов",
                    FirstName = "Иван",
                    MiddleName = "Иванович",
                    BirthDate = new(2010, 1, 1)
                });

            var service = new MarriageService(repo.Object, personService.Object, lifeEvent.Object);

            var dto = new RequestAddMarriageDTO()
            {
                Spouse1Id = sameId,
                Spouse2Id = sameId,
                BeginDate = new(2026, 1, 1)
            };

            Func<Task> act = () => service.CreateMarriageAsync(dto);

            await act.Should().ThrowAsync<InvalidMarriageDataException>();
        }

        [Fact]
        public async Task Divorce_ShouldThrow_WhenBeginDateMoreDivorceDate()
        {
            var repo = new Mock<IMarriageRepository>();
            var personService = new Mock<IPersonService>();
            var lifeEvent = new Mock<ILifeEventService>();

            var marriageId = Guid.NewGuid();

            var spouse1 = Guid.NewGuid();
            var spouse2 = Guid.NewGuid();

            repo
                .Setup(s => s.GetByIdAsync(marriageId))
                .ReturnsAsync(new Marriage
                {
                    Id = marriageId,
                    Spouse1Id = spouse1,
                    Spouse2Id = spouse2,
                    BeginDate = new(2020, 1, 1),
                });

            

            personService
                .Setup(s => s.GetShortPersonByIdAsync(spouse1))
                .ReturnsAsync(new ShortPersonDTO()
                {
                    Id = spouse1,
                    LastName = "Иванов",
                    FirstName = "Иван",
                    MiddleName = "Иванович",
                    BirthDate = new(1998, 10, 1)
                });

            personService
                .Setup(s => s.GetShortPersonByIdAsync(spouse2))
                .ReturnsAsync(new ShortPersonDTO()
                {
                    Id = spouse2,
                    LastName = "Иванова",
                    FirstName = "Елена",
                    MiddleName = "Ивановна",
                    BirthDate = new(2000, 04, 20)
                });

            var service = new MarriageService(repo.Object, personService.Object, lifeEvent.Object);

            var dto = new RequestAddDivorceDTO
            {
                MarriageId = marriageId,
                DivorceDate = new(2019, 12, 31),
                EndReason = Data.Utils.MarriageEndReason.DivorceByConsent
            };


            Func<Task> act = () => service.DivorceAsync(dto);

            await act.Should().ThrowAsync<InvalidMarriageDataException>();
        }

        [Fact]
        public async Task CreateMarriage_ShouldThrow_WhenSpouseHaveActiveMarriage()
        {
            var repo = new Mock<IMarriageRepository>();
            var personService = new Mock<IPersonService>();
            var lifeEvent = new Mock<ILifeEventService>();

            var spouse1 = Guid.NewGuid();
            var spouse2 = Guid.NewGuid();

            

            personService
                .Setup(s => s.GetShortPersonByIdAsync(spouse1))
                .ReturnsAsync(new ShortPersonDTO()
                {
                    Id = spouse1,
                    LastName = "Иванов",
                    FirstName = "Иван",
                    MiddleName = "Иванович",
                    BirthDate = new(1998, 10, 1)
                });

            personService
                .Setup(s => s.GetShortPersonByIdAsync(spouse2))
                .ReturnsAsync(new ShortPersonDTO()
                {
                    Id = spouse2,
                    LastName = "Иванова",
                    FirstName = "Елена",
                    MiddleName = "Ивановна",
                    BirthDate = new(2000, 04, 20)
                });

            repo
                .Setup(r => r.GetActiveMarriageAsync(spouse1))
                .ReturnsAsync((Marriage?)null);

            repo
                .Setup(r => r.GetActiveMarriageAsync(spouse2))
                .ReturnsAsync(new Marriage());

            var service = new MarriageService(repo.Object, personService.Object, lifeEvent.Object);

            var dto = new RequestAddMarriageDTO
            {
                Spouse1Id = spouse1,
                Spouse2Id = spouse2,
                BeginDate = new(2022, 11, 25)
            };

            Func<Task> act = () => service.CreateMarriageAsync(dto);

            await act.Should().ThrowAsync<ActiveMarriageExistsException>();
        }

        [Fact]
        public async Task CreateMarriage_ShouldCreateMarriage_WhenDataValid()
        {
            var repo = new Mock<IMarriageRepository>();
            var personService = new Mock<IPersonService>();
            var lifeEvent = new Mock<ILifeEventService>();

            var spouse1Id = Guid.NewGuid();
            var spouse2Id = Guid.NewGuid();
            var beginDate = new DateTime(2026, 1, 10);

            var spouse1 = new ShortPersonDTO
            {
                Id = spouse1Id,
                LastName = "Иванов",
                FirstName = "Иван",
                MiddleName = "Иванович",
                BirthDate = new DateTime(1998, 10, 1)
            };

            var spouse2 = new ShortPersonDTO
            {
                Id = spouse2Id,
                LastName = "Петрова",
                FirstName = "Елена",
                MiddleName = "Игоревна",
                BirthDate = new DateTime(2000, 4, 20)
            };

            personService
                .Setup(s => s.GetShortPersonByIdAsync(spouse1Id))
                .ReturnsAsync(spouse1);

            personService
                .Setup(s => s.GetShortPersonByIdAsync(spouse2Id))
                .ReturnsAsync(spouse2);

            repo
                .Setup(r => r.GetActiveMarriageAsync(spouse1Id))
                .ReturnsAsync((Marriage?)null);

            repo
                .Setup(r => r.GetActiveMarriageAsync(spouse2Id))
                .ReturnsAsync((Marriage?)null);

            lifeEvent
                .Setup(s => s.AddEventAsync(It.IsAny<Guid>(), It.IsAny<LifeEventType>(), It.IsAny<DateTime>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            repo
                .Setup(r => r.AddAsync(It.IsAny<Marriage>()))
                .Returns(Task.CompletedTask);

            var service = new MarriageService(repo.Object, personService.Object, lifeEvent.Object);

            var dto = new RequestAddMarriageDTO
            {
                Spouse1Id = spouse1Id,
                Spouse2Id = spouse2Id,
                BeginDate = beginDate
            };

            var result = await service.CreateMarriageAsync(dto);

            result.Should().NotBeNull();
            result.Spouse1.Id.Should().Be(spouse1Id);
            result.Spouse2.Id.Should().Be(spouse2Id);
            result.BeginDate.Should().Be(beginDate);
            result.EndDate.Should().BeNull();
            result.EndReason.Should().BeNull();

            repo.Verify(r => r.AddAsync(It.Is<Marriage>(m =>
                m.Spouse1Id == spouse1Id &&
                m.Spouse2Id == spouse2Id &&
                m.BeginDate == beginDate
            )), Times.Once);

            lifeEvent.Verify(s => s.AddEventAsync(
                spouse1Id,
                LifeEventType.Marriage,
                beginDate,
                "Брак с Петрова Елена Игоревна"
            ), Times.Once);

            lifeEvent.Verify(s => s.AddEventAsync(
                spouse2Id,
                LifeEventType.Marriage,
                beginDate,
                "Брак с Иванов Иван Иванович"
            ), Times.Once);
        }

        [Fact]
        public async Task GetCurrentSpouse_ShouldReturnSpouse_WhenActiveMarriageExists()
        {
            var repo = new Mock<IMarriageRepository>();
            var personService = new Mock<IPersonService>();
            var lifeEvent = new Mock<ILifeEventService>();

            var personId = Guid.NewGuid();
            var spouseId = Guid.NewGuid();

            repo
                .Setup(r => r.GetActiveMarriageAsync(personId))
                .ReturnsAsync(new Marriage()
                {
                    Id = Guid.NewGuid(),
                    Spouse1Id = personId,
                    Spouse2Id = spouseId,
                    BeginDate = new(2020, 11, 2)
                });

            personService
                .Setup(s => s.GetShortPersonByIdAsync(spouseId))
                .ReturnsAsync(new ShortPersonDTO()
                {
                    Id = spouseId,
                    LastName = "Иванова",
                    FirstName = "Елена",
                    MiddleName = "Игоревна",
                    BirthDate = new(1995, 1, 15),
                });

            var service = new MarriageService(repo.Object, personService.Object, lifeEvent.Object);

            var result = await service.GetCurrentSpouseAsync(personId);

            result.Should().NotBeNull();
            result.Id.Should().Be(spouseId);
        }

        [Fact]
        public async Task GetCurrentSpouse_ShouldReturnNull_WhenNoActiveMarriage()
        {
            var repo = new Mock<IMarriageRepository>();
            var personService = new Mock<IPersonService>();
            var lifeEvent = new Mock<ILifeEventService>();

            var personId = Guid.NewGuid();

            repo
                .Setup(r => r.GetActiveMarriageAsync(personId))
                .ReturnsAsync((Marriage?)null);

            var service = new MarriageService(repo.Object, personService.Object, lifeEvent.Object);

            var result = await service.GetCurrentSpouseAsync(personId);

            result.Should().BeNull();
        }

        [Fact]
        public async Task GetMarriageHistory_ShouldReturnList_WhenMarriagesExists()
        {
            var repo = new Mock<IMarriageRepository>();

            var personService = new Mock<IPersonService>();

            var lifeEvent = new Mock<ILifeEventService>();

            var personId = Guid.NewGuid();

            var marriagesList = new List<Marriage>
            {
                new() 
                {
                    Id = Guid.NewGuid(),
                    Spouse1Id = personId,
                    Spouse2Id = Guid.NewGuid(),
                    BeginDate = new(2015, 6, 14),
                    EndDate = new(2018, 3, 6),
                    EndReason = MarriageEndReason.DivorceByConsent
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Spouse1Id = personId,
                    Spouse2Id = Guid.NewGuid(),
                    BeginDate = new(2023, 10, 10),
                }
            };

            repo
                .Setup(r => r.GetByPersonIdAsync(personId))
                .ReturnsAsync(marriagesList);

            personService
                .Setup(s => s.GetShortPersonByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid id) => new ShortPersonDTO()
                {
                    Id = id
                });

            var service = new MarriageService(repo.Object, personService.Object, lifeEvent.Object);

            var result = await service.GetMarriageHistoryAsync(personId);

            result.Should().NotBeNull();
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task Divorce_ShouldSetEndDateAndReason_WhenDataValid()
        {
            var repo = new Mock<IMarriageRepository>();

            var personService = new Mock<IPersonService>();

            var lifeEvent = new Mock<ILifeEventService>();

            repo
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new Marriage 
                { 
                    Id = Guid.NewGuid(), 
                    Spouse1Id = Guid.NewGuid(),
                    Spouse2Id = Guid.NewGuid(),
                    BeginDate = new(2020, 1, 1)
                });

            personService
                .Setup(s => s.GetShortPersonByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid id) => new ShortPersonDTO()
                {
                    Id = id,
                });

            lifeEvent
                .Setup(l => l.AddEventAsync(It.IsAny<Guid>(), LifeEventType.Divorce, It.IsAny<DateTime>(), It.IsAny<string?>()))
                .Returns(Task.CompletedTask);

            repo
                .Setup(r => r.UpdateAsync(It.IsAny<Marriage>()))
                .Returns(Task.CompletedTask);

            var dto = new RequestAddDivorceDTO()
            {
                DivorceDate = new(2022, 3, 12),
                MarriageId = Guid.NewGuid(),
                EndReason = MarriageEndReason.SpouseMissing
            };

            var service = new MarriageService(repo.Object, personService.Object, lifeEvent.Object);

            var result = await service.DivorceAsync(dto);
            
            result.Should().NotBeNull();
            result.EndDate.Should().Be(dto.DivorceDate);
            result.EndReason.Should().Be(dto.EndReason);

            repo.Verify(r => r.UpdateAsync(It.Is<Marriage>(m =>
                m.EndDate == dto.DivorceDate &&
                m.EndReason == dto.EndReason
            )), Times.Once);
        }

        [Fact]
        public async Task CreateMarriage_ShouldThrow_WhenSpouseNotFound()
        {
            var repo = new Mock<IMarriageRepository>();

            var personService = new Mock<IPersonService>();

            var lifeEvent = new Mock<ILifeEventService>();

            personService
                .SetupSequence(ps => ps.GetShortPersonByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new ShortPersonDTO() { LastName = "First", FirstName = "Person"})
                .ReturnsAsync((ShortPersonDTO?)null!);

            var service = new MarriageService(repo.Object, personService.Object, lifeEvent.Object);

            var dto = new RequestAddMarriageDTO()
            {
                Spouse1Id = It.IsAny<Guid>(),
                Spouse2Id = It.IsAny<Guid>(),
                BeginDate = It.IsAny<DateTime>()
            };

            Func<Task> act = () => service.CreateMarriageAsync(dto);

            await act.Should().ThrowAsync<PersonNotFoundException>();
        }

        [Fact]
        public async Task CreateMarriage_ShouldAddLifeEventsAndSave()
        {
            var repo = new Mock<IMarriageRepository>();

            var personService = new Mock<IPersonService>();

            var lifeEvent = new Mock<ILifeEventService>();

            var dto1 = new ShortPersonDTO()
            {
                Id = Guid.NewGuid(),
                LastName = "First",
                FirstName = "Person"
            };

            var dto2 = new ShortPersonDTO()
            {
                Id = Guid.NewGuid(),
                LastName = "Second",
                FirstName = "Person"
            };

            personService
                .SetupSequence(ps => ps.GetShortPersonByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(dto1)
                .ReturnsAsync(dto2);

            repo
                .SetupSequence(r => r.GetActiveMarriageAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Marriage?)null)
                .ReturnsAsync((Marriage?)null);

            var dtoMarriage = new RequestAddMarriageDTO()
            {
                Spouse1Id = Guid.NewGuid(),
                Spouse2Id = Guid.NewGuid(),
                BeginDate = new(2025, 1, 1)
            };

            var service = new MarriageService(repo.Object, personService.Object, lifeEvent.Object);

            var result = service.CreateMarriageAsync(dtoMarriage);

            lifeEvent
                .Verify(le => le.AddEventAsync(
                    It.IsAny<Guid>(), 
                    LifeEventType.Marriage, 
                    It.IsAny<DateTime>(),
                    It.IsAny<string>()
                    )
                , Times.Exactly(2)
            );

            repo
                .Verify(r => r.AddAsync(It.IsAny<Marriage>()), Times.Once);
        }

        [Fact]
        public async Task Divorce_ShouldThrow_WhenMarriageNotFound()
        {
            var repo = new Mock<IMarriageRepository>();

            var personService = new Mock<IPersonService>();

            var lifeEvent = new Mock<ILifeEventService>();

            repo
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Marriage?)null);

            var dto = new RequestAddDivorceDTO()
            {
                MarriageId = Guid.NewGuid(),
                DivorceDate = DateTime.Now,
                EndReason = MarriageEndReason.DivorceByConsent
            };

            var service = new MarriageService(repo.Object, personService.Object, lifeEvent.Object);

            Func<Task> act = () => service.DivorceAsync(dto);

            await act.Should().ThrowAsync<MarriageNotFoundException>();
        }

        [Fact]
        public async Task Divorce_ShouldThrow_WhenMarriageAlreadyClosed()
        {
            var repo = new Mock<IMarriageRepository>();

            var personService = new Mock<IPersonService>();

            var lifeEvent = new Mock<ILifeEventService>();

            var marriage = new Marriage()
            {
                Id = Guid.NewGuid(),
                Spouse1Id = Guid.NewGuid(),
                Spouse2Id = Guid.NewGuid(),
                BeginDate = new(2025, 1, 1),
                EndDate = DateTime.Now,
                EndReason = MarriageEndReason.SpouseDeath
            };

            repo
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(marriage);

            personService
                .SetupSequence(s => s.GetShortPersonByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new ShortPersonDTO() { Id = marriage.Spouse1Id })
                .ReturnsAsync(new ShortPersonDTO() { Id = marriage.Spouse2Id });

            var dto = new RequestAddDivorceDTO()
            {
                MarriageId = marriage.Id,
                DivorceDate = DateTime.Now,
                EndReason = MarriageEndReason.DivorceByConsent
            };

            var service = new MarriageService(repo.Object, personService.Object, lifeEvent.Object);

            Func<Task<MarriageDTO?>> act = () => service.DivorceAsync(dto);

            if (marriage.EndDate is null)
            {
                var result = await act();

                result.Should().NotBeNull();
            }
            else
                await act.Should().ThrowAsync<InvalidMarriageDataException>();
        }

        [Fact]
        public async Task GetMarriageHistory_ShouldReturnEmpty_WhenNoMarriages()
        {
            var repo = new Mock<IMarriageRepository>();

            var personService = new Mock<IPersonService>();

            var lifeEvent = new Mock<ILifeEventService>();

            repo
                .Setup(r => r.GetByPersonIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync([]);

            var id = Guid.NewGuid();

            var service = new MarriageService(repo.Object, personService.Object, lifeEvent.Object);

            var result = await service.GetMarriageHistoryAsync(id);

            result.Should().NotBeNull();
            result.Should().HaveCount(0);
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetCurrentSpouse_ShouldReturnOtherSpouse_WhenPersonIsSpouse2()
        {
            var repo = new Mock<IMarriageRepository>();

            var personService = new Mock<IPersonService>();

            var lifeEvent = new Mock<ILifeEventService>();

            var marriage = new Marriage()
            {
                Id = Guid.NewGuid(),
                Spouse1Id = Guid.NewGuid(),
                Spouse2Id = Guid.NewGuid()
            };

            repo
                .Setup(r => r.GetActiveMarriageAsync(It.IsAny<Guid>()))
                .ReturnsAsync(marriage);

            personService
                .Setup(s => s.GetShortPersonByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new ShortPersonDTO() { Id = marriage.Spouse1Id, LastName = "Find" });

            var service = new MarriageService(repo.Object, personService.Object, lifeEvent.Object);

            var result = await service.GetCurrentSpouseAsync(marriage.Spouse2Id);

            result.Should().NotBeNull();
            result.Id.Should().Be(marriage.Spouse1Id);
        }
    }
}

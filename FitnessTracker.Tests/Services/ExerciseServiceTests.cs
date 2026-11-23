using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Domain.Models;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Repository.Interface;
using Service.Implementation;
using Service.Interface;

namespace FitnessTracker.Tests.Services
{
    public class ExerciseServiceTests
    {
        [Fact] // test in JUnit
        public async Task GetByIdNotNullAsync_WhenExerciseMissing_ThrowsInvalidOperationException()
        {
            var repo = new Mock<IRepository<Exercise>>();
            repo.Setup(r => r.Get<Exercise>(
                It.IsAny<Expression<Func<Exercise, Exercise>>>(),
                It.IsAny<Expression<Func<Exercise, bool>>?>(),
                It.IsAny<Func<IQueryable<Exercise>, IOrderedQueryable<Exercise>>?>(),
                It.IsAny<Func<IQueryable<Exercise>, IIncludableQueryable<Exercise, object>>?>()))
            .ReturnsAsync((Exercise?)null);

            var service = new ExerciseService(repo.Object);
            var id = Guid.NewGuid();

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.GetByIdNotNullAsync(id));
            Assert.Contains(id.ToString(), ex.Message);
        }
        

        [Theory] // like ParameterizedTest
        [InlineData("Row 1: TTTT", true,  true,  true,  true,  true)]
        [InlineData("Row 9: FTTT", false, true,  true,  true,  false)]
        [InlineData("Row 5: TFTT", true,  false, true,  true,  false)]
        [InlineData("Row 3: TTFT", true,  true,  false, true,  false)]
        [InlineData("Row 2: TTTF", true,  true,  true,  false, false)]
        public async Task GetActiveByMemberIdAsync_CACC(string row, bool memberMatches, bool isActive, bool started, bool notEnded, bool expectedFound)
        {
            var memberId = Guid.NewGuid();
            var memberships = new List<Membership>
            {
                new Membership
                {
                    MemberId = memberMatches ? memberId : Guid.NewGuid(),
                    IsActive = isActive,
                    StartDate = started ? DateTime.UtcNow.AddDays(-10) : DateTime.UtcNow.AddDays(10),
                    EndDate = notEnded ? DateTime.UtcNow.AddDays(10) : DateTime.UtcNow.AddDays(-5)
                }
            };

            var repo = new Mock<IRepository<Membership>>();
            repo.Setup(r => r.Get<Membership>(
                    It.IsAny<Expression<Func<Membership, Membership>>>(),
                    It.IsAny<Expression<Func<Membership, bool>>?>(),
                    It.IsAny<Func<IQueryable<Membership>, IOrderedQueryable<Membership>>?>(),
                    It.IsAny<Func<IQueryable<Membership>, IIncludableQueryable<Membership, object>>?>()))
                .Returns((Expression<Func<Membership, Membership>> selector,
                        Expression<Func<Membership, bool>>? predicate,
                        Func<IQueryable<Membership>, IOrderedQueryable<Membership>>? orderBy,
                        Func<IQueryable<Membership>, IIncludableQueryable<Membership, object>>? include)
                    => Task.FromResult(memberships.AsQueryable().Where(predicate!).Select(selector).FirstOrDefault()));

            
            var service = new MembershipService(repo.Object, Mock.Of<IGymService>(), Mock.Of<IMemberService>());

            var result = await service.GetActiveByMemberIdAsync(memberId);

            Assert.True(expectedFound == (result != null), $"{row}: expected found={expectedFound} but was found={result != null}");
            
        }
    }
}
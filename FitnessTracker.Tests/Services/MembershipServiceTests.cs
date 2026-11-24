using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Domain.Dto;
using Domain.Models;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Repository.Interface;
using Service.Implementation;
using Service.Interface;

namespace FitnessTracker.Tests.Services
{
    public class MembershipServiceTests
    {
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

            Assert.Equal(expectedFound, result != null);
        }

        [Theory]
        [InlineData(true, true, null)]
        [InlineData(true, false, "Member")]
        [InlineData(false, true, "Gym")] // na mesto true moze i da e false isto se dobiva
        public async Task InsertAsync_WithDifferentEntityExistence( bool gymExists, bool memebrExsist, string expectedError)
        {
            var repo = new Mock<IRepository<Membership>>();
            var gymService = new Mock<IGymService>();
            var memberService = new Mock<IMemberService>();


            var gymId = Guid.NewGuid();
            var memberId = Guid.NewGuid();

            gymService.Setup(g => g.GetByIdAsync(gymId))
                .ReturnsAsync(gymExists ? new Gym() : null);
            
            memberService.Setup(g=> g.GetByIdAsync(memberId))
                .ReturnsAsync(memebrExsist ? new Member{UserId = Guid.NewGuid().ToString()} : null);

            repo.Setup(g=>g.InsertAsync(It.IsAny<Membership>()))
                .Returns((Membership membership) => Task.FromResult(membership));

            var service = new MembershipService(repo.Object, gymService.Object, memberService.Object);

            var dto = new MembershipDto
            {
                GymId = gymId,
                MemberId = memberId,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(30),
                Price = 100,
                IsActive = true
            };

            if (expectedError == null)
            {
                var result = await service.InsertAsync(dto);

                Assert.NotNull(result);
                Assert.Equal(result.GymId, dto.GymId);
                Assert.Equal(result.MemberId, dto.MemberId);

                repo.Verify(r=>r.InsertAsync(It.IsAny<Membership>()), Times.Once);
            }
            else
            {
                var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.InsertAsync(dto));
                Assert.Contains(expectedError, ex.Message);

                repo.Verify(r=>r.InsertAsync(It.IsAny<Membership>()), Times.Never);
            }

        }
    }
}
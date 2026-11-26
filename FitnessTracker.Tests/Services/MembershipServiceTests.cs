using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Office2010.Excel;
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
        
        [Fact]
        public async Task GetByIdAsync_ReturnsOnlyTheMembershipWithThatId()
        {
            var wanted = new Membership { Id = Guid.NewGuid() };
            var other = new Membership { Id = Guid.NewGuid() };
            var memberships = new List<Membership> { other, wanted };

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

            Assert.Same(wanted, await service.GetByIdAsync(wanted.Id));
            Assert.Null(await service.GetByIdAsync(Guid.NewGuid()));
        }

        [Fact]
        public async Task GetByIdNotNullAsync_WhenMissing_ThrowsWithId()
        {
            var repo = new Mock<IRepository<Membership>>();

            repo.Setup(r => r.Get<Membership>(
                    It.IsAny<Expression<Func<Membership, Membership>>>(),
                    It.IsAny<Expression<Func<Membership, bool>>?>(),
                    It.IsAny<Func<IQueryable<Membership>, IOrderedQueryable<Membership>>?>(),
                    It.IsAny<Func<IQueryable<Membership>, IIncludableQueryable<Membership, object>>?>()))
                .ReturnsAsync((Membership?)null);

            var service = new MembershipService(repo.Object, Mock.Of<IGymService>(), Mock.Of<IMemberService>());
            var id = Guid.NewGuid();

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.GetByIdNotNullAsync(id));

            Assert.Contains(id.ToString(), ex.Message);
        }

        [Fact]
        public async Task GetByIdNotNullAsync_WhenExists_ReturnsMembership()
        {
            var repo = new Mock<IRepository<Membership>>();
            var membership = new Membership{ Id = Guid.NewGuid() };
            repo.Setup(r => r.Get<Membership>(
                    It.IsAny<Expression<Func<Membership, Membership>>>(),
                    It.IsAny<Expression<Func<Membership, bool>>?>(),
                    It.IsAny<Func<IQueryable<Membership>, IOrderedQueryable<Membership>>?>(),
                    It.IsAny<Func<IQueryable<Membership>, IIncludableQueryable<Membership, object>>?>()))
                .ReturnsAsync(membership);

            var service = new MembershipService(repo.Object, Mock.Of<IGymService>(), Mock.Of<IMemberService>());

            var result = await service.GetByIdAsync(membership.Id);
            Assert.Same(membership, result);
        }

        [Fact]
        public async Task DeleteAsync_WhenExists_DeletesMembership()
        {
            var repo = new Mock<IRepository<Membership>>();
            var membership = new Membership{ Id = Guid.NewGuid() };

            repo.Setup(r => r.Get<Membership>(
                    It.IsAny<Expression<Func<Membership, Membership>>>(),
                    It.IsAny<Expression<Func<Membership, bool>>?>(),
                    It.IsAny<Func<IQueryable<Membership>, IOrderedQueryable<Membership>>?>(),
                    It.IsAny<Func<IQueryable<Membership>, IIncludableQueryable<Membership, object>>?>()))
                .ReturnsAsync(membership);
            repo.Setup(r=> r.DeleteAsync(It.IsAny<Membership>())).Returns((Membership m) => Task.FromResult(m));

            var service = new MembershipService(repo.Object, Mock.Of<IGymService>(), Mock.Of<IMemberService>());

            var result = await service.DeleteAsync(membership.Id);

            Assert.Same(membership, result);
            repo.Verify(r => r.DeleteAsync(membership), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_WhenMissing_ThrowsAndDoesNotDelete()
        {
            var repo = new Mock<IRepository<Membership>>();
            repo.Setup(r => r.Get<Membership>(
                    It.IsAny<Expression<Func<Membership, Membership>>>(),
                    It.IsAny<Expression<Func<Membership, bool>>?>(),
                    It.IsAny<Func<IQueryable<Membership>, IOrderedQueryable<Membership>>?>(),
                    It.IsAny<Func<IQueryable<Membership>, IIncludableQueryable<Membership, object>>?>()))
                .ReturnsAsync((Membership?)null);

            var id = Guid.NewGuid();
            
            var service = new MembershipService(repo.Object, Mock.Of<IGymService>(), Mock.Of<IMemberService>());
            await Assert.ThrowsAsync<InvalidOperationException>(() => service.DeleteAsync(id));

            repo.Verify(r => r.DeleteAsync(It.IsAny<Membership>()), Times.Never);
        }

        [Theory]
        [InlineData(true, true, true, null)]
        [InlineData(false, true, true, "Gym")]
        [InlineData(true, false, true, "Member")]
        [InlineData(true, true, false, "Membership with id")]
        public async Task UpdateAsync_WithDifferentEntityExistence(bool gymExists, bool memberExists, bool membershipExists, string? expectedError)
        {
            var repo = new Mock<IRepository<Membership>>();
            var gymService = new Mock<IGymService>();
            var memberService = new Mock<IMemberService>();

            var membership = new Membership {Id=Guid.NewGuid()};

            var dto = new MembershipDto
            {
                GymId = Guid.NewGuid(),
                MemberId = Guid.NewGuid(),
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(30),
                Price = 150,
                IsActive = true
            };

            gymService.Setup(g => g.GetByIdAsync(dto.GymId)).ReturnsAsync(gymExists ? new Gym() : null);
            memberService.Setup(m => m.GetByIdAsync(dto.MemberId)).ReturnsAsync(memberExists ? new Member { UserId = Guid.NewGuid().ToString() } : null);
            repo.Setup(r => r.Get<Membership>(
                    It.IsAny<Expression<Func<Membership, Membership>>>(),
                    It.IsAny<Expression<Func<Membership, bool>>?>(),
                    It.IsAny<Func<IQueryable<Membership>, IOrderedQueryable<Membership>>?>(),
                    It.IsAny<Func<IQueryable<Membership>, IIncludableQueryable<Membership, object>>?>()))
                .ReturnsAsync(membershipExists ? membership : null);

            repo.Setup(r=> r.UpdateAsync(It.IsAny<Membership>())).Returns((Membership m) => Task.FromResult(m));

            var service = new MembershipService(repo.Object, gymService.Object, memberService.Object);

            if (expectedError == null)
            {
                var result = await service.UpdateAsync(membership.Id, dto);

                Assert.Equal(dto.GymId, result.GymId);
                Assert.Equal(dto.MemberId, result.MemberId);
                repo.Verify(r => r.UpdateAsync(It.IsAny<Membership>()), Times.Once);
            }
            else
            {
                var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.UpdateAsync(membership.Id, dto));
                Assert.Contains(expectedError, ex.Message);
                repo.Verify(r => r.UpdateAsync(It.IsAny<Membership>()), Times.Never);
            }
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllMembershipsAsList()
        {
            var repo = new Mock<IRepository<Membership>>();
            var m1 = new Membership { Id = Guid.NewGuid() };
            var m2 = new Membership { Id = Guid.NewGuid() };

            repo.Setup(r => r.GetAllAsync<Membership>(
                    It.IsAny<Expression<Func<Membership, Membership>>>(),
                    It.IsAny<Expression<Func<Membership, bool>>?>(),
                    It.IsAny<Func<IQueryable<Membership>, IOrderedQueryable<Membership>>?>(),
                    It.IsAny<Func<IQueryable<Membership>, IIncludableQueryable<Membership, object>>?>(),
                    It.IsAny<int?>()))
                .ReturnsAsync((IEnumerable<Membership>)new List<Membership> { m1, m2 });

            var service = new MembershipService(repo.Object, Mock.Of<IGymService>(), Mock.Of<IMemberService>());

            var result = await service.GetAllAsync();

            Assert.Equal(2, result.Count);
            Assert.Contains(m1, result);
            Assert.Contains(m2, result);
        }
    }
}
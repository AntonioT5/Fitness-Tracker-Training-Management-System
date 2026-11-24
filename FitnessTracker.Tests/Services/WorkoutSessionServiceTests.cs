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
    public class WorkoutSessionServiceTests
    {
        [Theory]
        [InlineData(true,  true,  true,  true,  null)]
        [InlineData(false, true,  true,  false, "Member with id")]
        [InlineData(true,  false, true,  true,  "Trainer with id")]
        [InlineData(true,  true,  false, true,  "Exercise with id")]
        [InlineData(true,  true,  true,  false, "does not have an active membership")]
        public async Task InsertAsync_WithDifferentEntityExistence(bool memberExists, bool trainerExists, bool exerciseExists, bool hasActiveMembership, string? expectedError)
        {
            var repo = new Mock<IRepository<WorkoutSession>>();
            var membershipService = new Mock<IMembershipService>();
            var trainerService = new Mock<ITrainerService>();
            var memberService = new Mock<IMemberService>();
            var exerciseService = new Mock<IExerciseService>();

            var memberId = Guid.NewGuid();
            var trainerId = Guid.NewGuid();
            var exerciseId = Guid.NewGuid();

            memberService.Setup(m => m.GetByIdAsync(memberId))
                .ReturnsAsync(memberExists ? new Member { UserId = Guid.NewGuid().ToString() } : null);

            trainerService.Setup(t=> t.GetByIdAsync(trainerId))
                .ReturnsAsync(trainerExists ? new Trainer{UserId = Guid.NewGuid().ToString()} : null);

            exerciseService.Setup(e=>e.GetByIdAsync(exerciseId))
                .ReturnsAsync(exerciseExists ? new Exercise() : null);

            membershipService.Setup(m => m.GetActiveByMemberIdAsync(memberId))
                .ReturnsAsync(hasActiveMembership ? new Membership() : null);

            repo.Setup(r => r.InsertAsync(It.IsAny<WorkoutSession>()))
                .Returns((WorkoutSession session) => Task.FromResult(session));
            
            repo.Setup(r => r.Get<WorkoutSession>(
                    It.IsAny<Expression<Func<WorkoutSession, WorkoutSession>>>(),
                    It.IsAny<Expression<Func<WorkoutSession, bool>>?>(),
                    It.IsAny<Func<IQueryable<WorkoutSession>, IOrderedQueryable<WorkoutSession>>?>(),
                    It.IsAny<Func<IQueryable<WorkoutSession>, IIncludableQueryable<WorkoutSession, object>>?>()))
                .ReturnsAsync(new WorkoutSession { MemberId = memberId, TrainerId = trainerId, ExerciseId = exerciseId });

            var service = new WorkoutSessionService(repo.Object, membershipService.Object, trainerService.Object, memberService.Object, exerciseService.Object);

            var dto = new WorkoutSessionDto
            {
                Date = DateTime.UtcNow,
                SetsCompleted = 3,
                RepsCompleted = 10,
                WeightUsedKg = 50,
                DurationMinutes = 30,
                Notes = "test",
                MemberId = memberId,
                TrainerId = trainerId,
                ExerciseId = exerciseId
            };

            if (expectedError == null)
            {
                var result = await service.InsertAsync(dto);

                Assert.NotNull(result);
                Assert.Equal(dto.MemberId, result.MemberId);
                Assert.Equal(dto.TrainerId, result.TrainerId);
                Assert.Equal(dto.ExerciseId, result.ExerciseId);

                repo.Verify(r => r.InsertAsync(It.IsAny<WorkoutSession>()), Times.Once);
            }
            else
            {
                var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.InsertAsync(dto));
                Assert.Contains(expectedError, ex.Message);

                repo.Verify(r=> r.InsertAsync(It.IsAny<WorkoutSession>()), Times.Never);
            }

        }
    }
}
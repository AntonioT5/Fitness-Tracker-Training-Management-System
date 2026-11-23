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

namespace FitnessTracker.Tests.Services
{
    public class ExerciseServiceTests
    {
        [Fact]
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
    }
}
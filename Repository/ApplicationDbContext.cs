using Domain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Repository;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<GymAppUser>(options)
{
    public DbSet<Exercise> Exercises { get; set; }
    public DbSet<ExerciseWorkoutPlan> ExerciseWorkoutPlans { get; set; }
    public DbSet<Gym> Gyms { get; set; }
    public DbSet<Member> Members { get; set; }
    public DbSet<Membership> Memberships { get; set; }
    public DbSet<MemberWorkoutPlan> MemberWorkoutPlans { get; set; }
    public DbSet<Trainer> Trainers { get; set; }
    public DbSet<WorkoutPlan> WorkoutPlans { get; set; }
    public DbSet<WorkoutSession> WorkoutSessions { get; set; }
    public DbSet<EtlSyncLog> EtlSyncLogs { get; set; }
    public DbSet<GymAppUser> GymAppUsers { get; set; }
}
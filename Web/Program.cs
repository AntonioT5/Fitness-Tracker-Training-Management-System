using System.Text;
using System.Threading.RateLimiting;
using Domain.Configurations;
using Domain.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Quartz;
using Repository;
using Repository.Implementation;
using Repository.Interface;
using Service.Implementation;
using Service.Interface;
using Service.Jobs;
using Web.Interceptor;
using Web.Mapper;
using Web.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//Connecting to the Postgres DB
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                       throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>((sp, options) =>
    {
        options.UseNpgsql(connectionString);
        options.AddInterceptors(sp.GetRequiredService<AuditInterceptor>());
    }
);


builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// builder.Services.AddDefaultIdentity<GymAppUser>(options => options.SignIn.RequireConfirmedAccount = true)
//     .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Repository
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Service
builder.Services.AddScoped<IExerciseService, ExerciseService>();
builder.Services.AddScoped<IExerciseWorkoutPlanService, ExerciseWorkoutPlanService>();
builder.Services.AddScoped<IGymService, GymService>();
builder.Services.AddScoped<IMemberService, MemberService>();
builder.Services.AddScoped<IMembershipService, MembershipService>();
builder.Services.AddScoped<IMemberWorkoutPlanService, MemberWorkoutPlanService>();
builder.Services.AddScoped<ITrainerService, TrainerService>();
builder.Services.AddScoped<IWorkoutPlanService, WorkoutPlanService>();
builder.Services.AddScoped<IWorkoutSessionService, WorkoutSessionService>();

//  Export Import
builder.Services.AddScoped<IExcelExportService, ExcelExportService>();
builder.Services.AddScoped<IExcelImportService, ExcelImportService>();

//  ETL
builder.Services.AddScoped<IEtlService, EtlService>();


builder.Services.AddScoped<ICurrentUser, CurrentUser>();
builder.Services.AddHttpContextAccessor();

// Mapper
builder.Services.AddScoped<ExerciseMapper>();
builder.Services.AddScoped<GymMapper>();
builder.Services.AddScoped<MemberMapper>();
builder.Services.AddScoped<TrainerMapper>();
builder.Services.AddScoped<MembershipMapper>();
builder.Services.AddScoped<WorkoutPlanMapper>();
builder.Services.AddScoped<WorkoutSessionMapper>();
builder.Services.AddScoped<MemberWorkoutPlanMapper>();
builder.Services.AddScoped<ExerciseWorkoutPlanMapper>();
// Interceptor
builder.Services.AddScoped<AuditInterceptor>();

//Inbound
builder.Services.AddScoped<InboundWorkoutSessionEntryProcessor>();
builder.Services.AddScoped<IInboundWorkoutSessionEntryService, InboundWorkoutSessionEntryService>();

//User
builder.Services.AddIdentity<GymAppUser, IdentityRole>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 8;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

//JWT
var jwtSettings = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["Key"]!))
        };
    });

// WgerApi
builder.Services.Configure<WgerApiSettings>(builder.Configuration.GetSection("WgerApi"));

builder.Services.AddHttpClient<IWgerApiClient, WgerApiClient>((sp, client) =>
{

    var settings = sp.GetRequiredService<IOptions<WgerApiSettings>>();

    client.BaseAddress = new Uri(settings.Value.BaseAddress);
    client.Timeout = TimeSpan.FromSeconds(settings.Value.TimeoutSeconds);
});

// Background Service
builder.Services.AddHostedService<EtlBackgroundService>();

//RateLimiter
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = 429;
    
    options.AddPolicy("external-api", context =>
    {
        var apiKey = context.Request.Headers["x-api-key"];

        var apiClient = context.Items["ApiClient"] as ApiClient;
        var limit = apiClient?.RateLimitMinutes ?? 60;
        
        return RateLimitPartition.GetFixedWindowLimiter(apiKey.ToString(), _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = limit,
            Window = TimeSpan.FromMinutes(1),
            QueueLimit = 0,
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst
        });
    });
});

//Quartz

builder.Services.AddQuartz(options =>
{
    var jobKey = new JobKey("inbound-job", "inbound");
    options.AddJob<InboundProcessingJob>(o => o.WithIdentity(jobKey));
    
    options.AddTrigger(o =>
    {
        o.ForJob(jobKey).WithIdentity("inbound-job-trigger")
            .WithCronSchedule("0/20 * * * * ?")
            .WithDescription("Expires unpaid reservations");
    });
});
builder.Services.AddQuartzHostedService();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ApiKeyAuthMiddleware>();
app.UseRateLimiter();

app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
    .WithStaticAssets();

app.Run();
using AlsProjects.Models.Workouts;
using AlsProjects.Manager.Contract;
using AlsProjects.Manager;
using Microsoft.EntityFrameworkCore;
using AlsProjects.Repository.WorkoutJournal;
using AlsProjects.Controllers.API;
using AlsProjects.Model;
using System.Text.Json;
using System.IO;

internal class Program {
    private const string _wikiClient = "WikiClient";
    private static void Main(string[] args) {
        var builder = WebApplication.CreateBuilder(args);
        var env = builder.Environment;

        // Add services to the container.
        builder.Services.AddControllersWithViews();
        builder.Services.AddHttpClient(_wikiClient, (options) => {
            options.BaseAddress = new Uri("https://en.wikipedia.org/wiki/");
        });

        builder.Services.AddDbContext<WorkoutJournalContext>(opts => opts.UseInMemoryDatabase("WorkoutJournal"));

        builder.Services.AddTransient<IWorkoutJournalRepository, WorkoutJournalRepository>();
        builder.Services.AddTransient<IWorkoutJournalContract, WorkoutJournalManager>();

        builder.Services.AddTransient<IJokesApiController, JokesApiController>();
        builder.Services.AddDbContext<JokesDbContext>(options =>
            options.UseInMemoryDatabase("JokesDB")
        );

        builder.Services.AddResponseCaching(options => {
            options.SizeLimit = 1024 * 1024 * 10; // 10 MB
        });
        var app = builder.Build();

        // Seed some initial data
        using (var scope = app.Services.CreateScope()) {
            var dbContext = scope.ServiceProvider.GetRequiredService<JokesDbContext>();
            SeedData(app.Services, scope);
        }

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment()) {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        Configure(app, env);

        app.UseResponseCaching();

        app.Run();

        void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            var scope = app.ApplicationServices.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<WorkoutJournalContext>();
            CreateDatabaseTables(context);
        }

        void CreateDatabaseTables(WorkoutJournalContext context) {
            IEnumerable<WorkoutTypes> workoutTypes = new List<WorkoutTypes>() {
                new() { ExerciseName = "Biceps Curl" },
                new() { ExerciseName = "Chest Press" },
                new() { ExerciseName = "Deadlift" },
                new() { ExerciseName = "Triceps Extension" },
            };

            context.AddRange(workoutTypes);
            context.SaveChanges();
        }
    }

    private static void SeedData(IServiceProvider provider, IServiceScope scope) {
        string filePath =
            Environment.GetEnvironmentVariable("HOME") == null // local environment check
            ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AlsProjects", "jokes.json")
            : Path.Combine("C:\\home\\data", "jokes.json");

        using var _jokesDbContext = scope.ServiceProvider.GetRequiredService<JokesDbContext>();

        if (!File.Exists(filePath)) {
            var jokesJsonPath = Path.Combine(Environment.CurrentDirectory, "jokes.json");
            var jokesJson = File.ReadAllText(jokesJsonPath);

            File.WriteAllText(filePath, jokesJson);
        }

        var jokes = JsonSerializer.Deserialize<IEnumerable<Jokes>>(File.ReadAllText(filePath))!;
        _jokesDbContext.Set<Jokes>().AddRange(jokes);

        _jokesDbContext.SaveChanges();
    }
}
using AlsProjects.Models.Workouts;
using AlsProjects.Manager.Contract;
using AlsProjects.Manager;
using Microsoft.EntityFrameworkCore;
using AlsProjects.Repository.WorkoutJournal;

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

        var app = builder.Build();

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
}
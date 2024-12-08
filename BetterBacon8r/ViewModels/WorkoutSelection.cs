using System.Diagnostics.CodeAnalysis;

namespace AlsProjects.ViewModels.Workouts;

[ExcludeFromCodeCoverage]
public class WorkoutSelection {
    public int WorkoutID { get; set; }
    public string ExerciseName { get; set; } = string.Empty;
}

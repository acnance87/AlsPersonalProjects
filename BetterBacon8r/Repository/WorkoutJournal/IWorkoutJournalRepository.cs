using AlsProjects.Models.Workouts;

namespace AlsProjects.Repository.WorkoutJournal;

public interface IWorkoutJournalRepository
{
    IEnumerable<Workouts> GetWorkouts();

    IEnumerable<WorkoutTypes> GetWorkoutsTypes();

    WorkoutSessions CreateWorkoutSession(WorkoutSessions workoutSessions);

    Workouts AddWorkout(Workouts workout);

    void SaveChanges();

    void UpdateWorkoutSession(WorkoutSessions workoutSession);
}

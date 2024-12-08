using AlsProjects.Models.Workouts;

namespace AlsProjects.Manager.Contract {
    public interface IWorkoutJournalContract {

        Workouts AddWorkout(Workouts workout);

        WorkoutSessions BeginNewWorkoutSession(WorkoutSessions workoutSession);

        IEnumerable<Workouts> GetWorkouts();

        IEnumerable<WorkoutTypes> GetWorkoutsTypes();

        void UpdateWorkoutSession(WorkoutSessions workoutSession);
    }
}

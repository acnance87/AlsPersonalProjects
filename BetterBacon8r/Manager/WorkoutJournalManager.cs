using AlsProjects.Models.Workouts;
using AlsProjects.Repository;
using AlsProjects.Manager.Contract;

namespace AlsProjects.Manager
{
    public class WorkoutJournalManager : IWorkoutJournalContract {

        private readonly IWorkoutJournalRepository _repository;
        public WorkoutJournalManager(IWorkoutJournalRepository repository) { 
            _repository = repository;
        }

        public Workouts AddWorkout(Workouts workout) {
            return _repository.AddWorkout(workout);
        }

        public WorkoutSessions BeginNewWorkoutSession(WorkoutSessions workoutSession) {
            workoutSession.SessionDate = DateTime.Now;

            return _repository.CreateWorkoutSession(workoutSession);
        }

        public IEnumerable<Workouts> GetWorkouts() {
            return _repository.GetWorkouts();
        }

        public IEnumerable<WorkoutTypes> GetWorkoutsTypes() {
            return _repository.GetWorkoutsTypes();
        }

        public void UpdateWorkoutSession(WorkoutSessions workoutSession) {
            _repository.UpdateWorkoutSession(workoutSession);
        }
    }
}

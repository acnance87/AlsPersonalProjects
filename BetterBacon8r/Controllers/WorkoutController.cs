using AlsProjects.Manager.Contract;
using AlsProjects.Models.Workouts;
using AlsProjects.ViewModels.Workouts;
using Microsoft.AspNetCore.Mvc;
using Enum = AlsProjects.Models.Workouts.Enum;

namespace AlsProjects.Controllers {
    public class WorkoutController : Controller {

        private readonly IWorkoutJournalContract _contract;

        public WorkoutController(IWorkoutJournalContract workoutJournalContract) {
            _contract = workoutJournalContract;
        }

        public ActionResult Index() {
            ViewBag.Title = "Workout Journal";

            return View("~/Views/Workout/Index.cshtml");
        }

        public ActionResult BeginWorkout() {
            return View();
        }

        public ActionResult AddWorkout(int exerciseId) {
            var toReturn = new Workouts() {
                Weight = 0,
                PerceivedExertionKind = Enum.PerceivedExertion.Low,
                Repetitions = 0,
                WorkoutType = new WorkoutTypes() {
                    WorkoutTypeID = exerciseId,
                }
            };


            return Json(toReturn);
        }

        public ActionResult GetWorkouts() {
            var workoutTypes = _contract.GetWorkoutsTypes();

            return PartialView("_AddWorkout", workoutTypes);
        }

        [HttpPost]
        public ActionResult CreateWorkout([FromBody] WorkoutSelection[] workoutsSelected) {
            var workouts = new List<Workouts>();

            var workoutSession = new WorkoutSessions() {
                SessionDate = DateTime.Now,
                Workouts = null,
            };

            foreach (var workout in workoutsSelected) {
                workouts.Add(new Workouts() {
                    WorkoutTypeID = workout.WorkoutID,
                    PerceivedExertionKind = Enum.PerceivedExertion.Low,
                    Repetitions = 0,
                    Weight = 0,
                    Session = workoutSession,
                });
            }

            workoutSession.Workouts = workouts;

            var newWorkout = _contract.BeginNewWorkoutSession(workoutSession);

            foreach (var nw in newWorkout.Workouts) {
                nw.WorkoutType = new WorkoutTypes() {
                    ExerciseName = workoutsSelected.First(e => e.WorkoutID == nw.WorkoutTypeID).ExerciseName,
                    WorkoutTypeID = workoutsSelected.First(e => e.WorkoutID == nw.WorkoutTypeID).WorkoutID,
                };
            }


            return PartialView("_EditWorkout", newWorkout);
        }
    }
}

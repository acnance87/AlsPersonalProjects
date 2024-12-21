using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlsProjects.Models.Workouts;

namespace AlsProjects.Repository.WorkoutJournal
{
    public class WorkoutJournalRepository : WorkoutJournalContext, IWorkoutJournalRepository
    {

        private readonly WorkoutJournalContext _context;

        public WorkoutJournalRepository(DbContextOptions<WorkoutJournalContext> options) : base(options)
        {
            _context = new WorkoutJournalContext(options);
        }

        public Workouts AddWorkout(Workouts workout)
        {
            _context.Workouts.Add(workout);
            _context.SaveChanges();

            return workout;
        }

        public WorkoutSessions CreateWorkoutSession(WorkoutSessions workoutSessions)
        {
            _context.WorkoutSessions.Add(workoutSessions);
            _context.SaveChanges();

            return workoutSessions;
        }

        public IEnumerable<Workouts> GetWorkouts()
        {
            return _context.Workouts;
        }

        public IEnumerable<WorkoutTypes> GetWorkoutsTypes()
        {
            return _context.WorkoutTypes;
        }

        public IEnumerable<WorkoutTypes> GetWorkoutTypes()
        {
            return _context.WorkoutTypes;
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public void UpdateWorkoutSession(WorkoutSessions workoutSession)
        {
            _context.WorkoutSessions.Attach(workoutSession);
            _context.WorkoutSessions.Entry(workoutSession).State = EntityState.Modified;

            _context.SaveChanges();
        }
    }
}

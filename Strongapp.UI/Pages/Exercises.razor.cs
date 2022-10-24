using Fluxor;
using Microsoft.AspNetCore.Components;
using Strongapp.Models;
using Strongapp.UI.Services;
using Strongapp.UI.Stores;

namespace Strongapp.UI.Pages
{
    public partial class Exercises
    {
        public string SearchPhrase { get; set; }

        public StrongExerciseBodyPart? ExerciseBodyPart { get; set; }

        public StrongExerciseCategory? ExerciseCategory { get; set; }

        [Inject]
        public IState<AppStore> State { get; set; } = default!;

        public List<StrongExerciseWithMetadata> SearchResults
        {
            get
            {
                var exercises = State.Value.Exercises;
                if (!string.IsNullOrEmpty(SearchPhrase))
                    exercises = exercises.Where(x => x.ExerciseName.Contains(SearchPhrase)).ToList();
                if (ExerciseBodyPart != null)
                    exercises = exercises.Where(x => x.BodyPart == ExerciseBodyPart).ToList();
                if (ExerciseCategory != null)
                    exercises = exercises.Where(x => x.Category == ExerciseCategory).ToList();
                return exercises;
            }
        }
    }
}

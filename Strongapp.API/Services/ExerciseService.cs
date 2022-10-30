using Strongapp.Models;

namespace Strongapp.API.Services
{
    public class ExerciseService
    {
        private readonly OneRMWeightCalculator _oneRmWeightCalculator;

        public ExerciseService(OneRMWeightCalculator oneRmWeightCalculator)
        {
            _oneRmWeightCalculator = oneRmWeightCalculator;
        }

        public IEnumerable<StrongHistoricExerciseData> GetHistoricExerciseData(List<StrongWorkout> workouts, string name)
        {
            var matchedWorkouts = workouts
                .Where(x => x.ExerciseData.Any(x => x.ExerciseName == name));

            var performances = matchedWorkouts
                .Select(x => new StrongHistoricExerciseData 
                { 
                    Date = x.Date, 
                    WorkoutName = x.WorkoutName, 
                    Sets = x.ExerciseData.First(x => x.ExerciseName == name).Sets 
                });

            return performances;
        }

        public IEnumerable<StrongPerformance> GetPerformances(List<StrongWorkout> workouts, string name)
        {
            var historicExerciseData = GetHistoricExerciseData(workouts, name);

            var performancesSets = historicExerciseData
                .Select(x => x.Sets.Select(y => new StrongPerformance { Date = x.Date, Set = y }))
                .SelectMany(x => x);

            return performancesSets;
        }

        public List<StrongPerformance> GetBestPerformances(IEnumerable<StrongPerformance> performances)
        {
            var bestPerformances = performances
                .GroupBy(x => x.Set.Reps)
                .Select(g =>
                {
                    var performance = g.MaxBy(x => x.Set.Weight);
                    return new StrongPerformance { Date = performance?.Date, Set = performance.Set };
                });

            var results = new List<StrongPerformance>();

            decimal weight = 0;
            foreach (var bestPerformance in bestPerformances.OrderByDescending(x => x.Set.Reps))
            {
                if (bestPerformance.Set.Weight.Value > weight)
                {
                    results.Add(bestPerformance);
                    weight = bestPerformance.Set.Weight.Value;
                }
            }

            results = results.OrderBy(x => x.Set.Reps).ToList();

            return results;
        }

        public List<StrongPredictedPerformance> GetPredictedPerformances(List<StrongPerformance> bestPerformances)
        {
            var oneRM = bestPerformances.Max(x => _oneRmWeightCalculator.CalculatePredictedOneRMWeight(x.Set.Weight.Value, x.Set.Reps.Value));

            var results = new List<StrongPredictedPerformance>();
            for (var reps = 1; reps <= 10; reps++)
            {
                var bestPerformance = bestPerformances.FirstOrDefault(x => x.Set.Reps >= reps);
                var predictedWeight = _oneRmWeightCalculator.CalculatePredictedWeight(oneRM.Value, reps);
                results.Add(new StrongPredictedPerformance { Reps = reps, BestPerformance = bestPerformance, Predicted = Convert.ToInt32(predictedWeight) });
            }

            for (var reps = 11; reps <= 12; reps++)
            {
                var bestPerformance = bestPerformances.FirstOrDefault(x => x.Set.Reps >= reps);
                results.Add(new StrongPredictedPerformance { Reps = reps, BestPerformance = bestPerformance });
            }

            return results;
        }

    }
}

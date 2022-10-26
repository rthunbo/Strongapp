namespace Strongapp.API.Services
{
    public class OneRMWeightCalculator
    {
        //                                                    2      3      4      5      6      7      8      9      10
        private decimal[] multipliers = new decimal[] { 0, 1, 0.95m, 0.93m, 0.90m, 0.87m, 0.85m, 0.83m, 0.80m, 0.77m, 0.75m };

        public decimal? CalculatePredictedOneRMWeight(decimal weight, int reps)
        {
            if (reps > 10)
                return null;
            return weight / multipliers[reps];
        }

        public decimal? CalculatePredictedWeight(decimal oneRMWeight, int reps)
        {
            if (reps > 10)
                return null;
            return oneRMWeight * multipliers[reps];
        }
    }
}

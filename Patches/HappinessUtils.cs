using Game.UI.InGame;
using Unity.Collections;
using Unity.Mathematics;

namespace ShowMoreHappiness
{
    /// <summary>
    /// Happiness utilities.
    /// </summary>
    public class HappinessUtils
    {
        // Harmony ID.
        public const string HarmonyID = "rcav8tr." + ModAssemblyInfo.Name;

        /// <summary>
        /// Sort happiness factors.
        /// </summary>
        public static void SortHapinessFactors(NativeList<FactorInfo> happinessFactors)
        {
            // Get mod settings used often in sorting.
            bool interspersed = Mod.ModSettings.PositiveNegativeValues == ModSettings.PositiveNegativeValuesChoice.Interspersed;
            bool ascending    = Mod.ModSettings.SortDirection          == ModSettings.SortDirectionChoice.Ascending;

            // Compare each factor against every other factor.
            for (int i = 0; i < happinessFactors.Length - 1; i++)
            {
                for (int j = i + 1; j < happinessFactors.Length; j++)
                {
                    // Get the happiness factors to compare.
                    FactorInfo happinessFactor1 = happinessFactors[i];
                    FactorInfo happinessFactor2 = happinessFactors[j];

                    // Get happiness factor weights.
                    int weight1 = happinessFactor1.weight;
                    int weight2 = happinessFactor2.weight;

                    // For interspersed, compare based on absolute values.
                    if (interspersed)
                    {
                        weight1 = math.abs(weight1);
                        weight2 = math.abs(weight2);
                    }

                    // Compare happiness factor weights.
                    int comparison = weight1.CompareTo(weight2);

                    // If weights are the same, compare happiness factor factors.
                    if (comparison == 0)
                    {
                        comparison = happinessFactor1.factor.CompareTo(happinessFactor2.factor);
                    }

                    // For ascending sort direction, reverse the comparison.
                    if (ascending)
                    {
                        comparison *= -1;
                    }

                    // Check if should swap happiness factors.
                    if (comparison < 0)
                    {
                        (happinessFactors[i], happinessFactors[j]) = (happinessFactor2, happinessFactor1);
                    }
                }
            }
        }
    }
}

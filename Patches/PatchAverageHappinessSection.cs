using Colossal.UI.Binding;
using Game.UI.InGame;
using HarmonyLib;
using System.Reflection;
using Unity.Collections;
using Unity.Mathematics;

namespace ShowMoreHappiness
{
    /// <summary>
    /// Patches for AverageHappinessSection.
    /// </summary>
    public static class PatchAverageHappinessSection
    {
        // Initialization flag.
        private static bool _initialized;

        // Fields from AverageHappinessSection.
        private static FieldInfo _fieldAverageHappiness;
        private static FieldInfo _fieldHappinessFactors;
        private static FieldInfo _fieldFactors;

        /// <summary>
        /// Initialize patch.
        /// </summary>
        public static void Initialize()
        {
            Mod.log.Info($"{nameof(PatchAverageHappinessSection)}.{nameof(Initialize)}");

            // Find fields from AverageHappinessSection.
            FieldInfo[] fieldInfos = typeof(AverageHappinessSection).GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
            foreach (FieldInfo fieldInfo in fieldInfos)
            {
                // Fields for averageHappiness and happinessFactors each have a backing field with names:
                //      <averageHappiness>k__BackingField
                //      <happinessFactors>k__BackingField
                // So need to find those fields by name containing.
                if      (fieldInfo.Name.Contains("averageHappiness")) { _fieldAverageHappiness = fieldInfo; }
                else if (fieldInfo.Name.Contains("happinessFactors")) { _fieldHappinessFactors = fieldInfo; }
                else if (fieldInfo.Name ==       "m_Factors"        ) { _fieldFactors          = fieldInfo; }
            }

            // Check if fields were found.
            if (_fieldAverageHappiness == null) { Mod.log.Error("Unable to find field AverageHappinessSection.averageHappiness."); return; }
            if (_fieldHappinessFactors == null) { Mod.log.Error("Unable to find field AverageHappinessSection.happinessFactors."); return; }
            if (_fieldFactors          == null) { Mod.log.Error("Unable to find field AverageHappinessSection.m_Factors."       ); return; }

            // Use Harmony to create a postfix patch for AverageHappinessSection.OnProcess().
            MethodInfo methodOnProcessOriginal = typeof(AverageHappinessSection).GetMethod("OnProcess", BindingFlags.Instance | BindingFlags.NonPublic);
            if (methodOnProcessOriginal == null)
            {
                Mod.log.Error($"Unable to find original method {nameof(AverageHappinessSection)}.OnProcess.");
                return;
            }
            MethodInfo methodOnProcessPostfix = typeof(PatchAverageHappinessSection).GetMethod(nameof(OnProcessPostfix), BindingFlags.Static | BindingFlags.NonPublic);
            if (methodOnProcessPostfix == null)
            {
                Mod.log.Error($"Unable to find patch postfix method {nameof(PatchAverageHappinessSection)}.{nameof(OnProcessPostfix)}.");
                return;
            }
            new Harmony(HappinessUtils.HarmonyID).Patch(methodOnProcessOriginal, null, new HarmonyMethod(methodOnProcessPostfix));

            // Use Harmony to create a prefix patch for AverageHappinessSection.OnWriteProperties().
            MethodInfo methodOnWritePropertiesOriginal = typeof(AverageHappinessSection).GetMethod("OnWriteProperties", BindingFlags.Instance | BindingFlags.Public);
            if (methodOnWritePropertiesOriginal == null)
            {
                Mod.log.Error($"Unable to find original method {nameof(AverageHappinessSection)}.OnWriteProperties.");
                return;
            }
            MethodInfo methodOnWritePropertiesPrefix = typeof(PatchAverageHappinessSection).GetMethod(nameof(OnWritePropertiesPrefix), BindingFlags.Static | BindingFlags.NonPublic);
            if (methodOnWritePropertiesPrefix == null)
            {
                Mod.log.Error($"Unable to find patch prefix method {nameof(PatchAverageHappinessSection)}.{nameof(OnWritePropertiesPrefix)}.");
                return;
            }
            new Harmony(HappinessUtils.HarmonyID).Patch(methodOnWritePropertiesOriginal, new HarmonyMethod(methodOnWritePropertiesPrefix), null);

            // Initialized.
            _initialized = true;
        }

        /// <summary>
        /// Postfix patch method for AverageHappinessSection.OnProcess().
        /// </summary>
        private static void OnProcessPostfix(AverageHappinessSection __instance)
        {
            // Patch must be initialized.
            if (!_initialized)
            {
                return;
            }

            // Get field values from the instance.
            NativeArray<int2>      factors          = (NativeArray<int2>     )_fieldFactors         .GetValue(__instance);
            NativeList<FactorInfo> happinessFactors = (NativeList<FactorInfo>)_fieldHappinessFactors.GetValue(__instance);

            // Clear happiness factors computed by the original method.
            happinessFactors.Clear();

            // Compute new happiness factors.
            // Logic adapted from AverageHappinessSection.OnProcess except zeroes are included or excluded according to the mod settings.
            for (int i = 0; i < factors.Length; i++)
            {
                // Check the count.
                int count = factors[i].x;
                if (count > 0)
                {
                    // Compute average for the happiness factor.
                    int average = (int)math.round((float)factors[i].y / count);
                    if (Mod.ModSettings.ShowZeroValues || average != 0)
                    {
                        // Add a new happiness factor.
                        happinessFactors.Add(new FactorInfo(i, average));
                    }
                }
            }

            // Sort happiness factors using this mod's sort method.
            HappinessUtils.SortHapinessFactors(happinessFactors);
        }

        /// <summary>
        /// Prefix patch method for AverageHappinessSection.OnWriteProperties().
        /// </summary>
        private static bool OnWritePropertiesPrefix(IJsonWriter writer, AverageHappinessSection __instance)
        {
            // Patch must be initialized.
            if (!_initialized)
            {
                // Call original method.
                return true;
            }

            // Get field values from the instance.
            CitizenHappiness       averageHappiness = (CitizenHappiness      )_fieldAverageHappiness.GetValue(__instance);
            NativeList<FactorInfo> happinessFactors = (NativeList<FactorInfo>)_fieldHappinessFactors.GetValue(__instance);

            // Write field values to UI.
            // Logic copied from AverageHappinessSection.OnWriteProperties() except use maximum factors from the mod settings.
		    writer.PropertyName("averageHappiness");
		    writer.Write(averageHappiness);
		    int num = math.min(Mod.ModSettings.MaximumFactors, happinessFactors.Length);
		    writer.PropertyName("happinessFactors");
		    writer.ArrayBegin(num);
		    for (int i = 0; i < num; i++)
		    {
			    happinessFactors[i].WriteBuildingHappinessFactor(writer);
		    }
		    writer.ArrayEnd();

            // Do not call original method.
            return false;
        }
    }
}

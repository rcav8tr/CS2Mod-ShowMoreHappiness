using Colossal.UI.Binding;
using Game.Prefabs;
using Game.Simulation;
using Game.UI;
using Game.UI.InGame;
using HarmonyLib;
using System.Reflection;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace ShowMoreHappiness
{
    /// <summary>
    /// Patches for CityInfoUISystem.
    /// </summary>
    public partial class PatchCityInfoUISystem : UISystemBase
    {
        // The game's instance of this system.
        private static PatchCityInfoUISystem _patchCityInfoUISystem;

        // Other systems.
    	private CitizenHappinessSystem _citizenHappinessSystem;

        /// <summary>
        /// Initialize this system.
        /// </summary>
        [Preserve]
        protected override void OnCreate()
        {
            base.OnCreate();
            Mod.log.Info($"{nameof(PatchCityInfoUISystem)}.{nameof(OnCreate)}");

            // Save the game's instance of this system.
            _patchCityInfoUISystem = this;

            // Get other systems.
            _citizenHappinessSystem = World.GetOrCreateSystemManaged<CitizenHappinessSystem>();

            // Use Harmony to create a prefix patch for CityInfoUISystem.WriteHappinessFactors.
            MethodInfo originalMethod = typeof(CityInfoUISystem).GetMethod("WriteHappinessFactors", BindingFlags.Instance | BindingFlags.NonPublic);
            if (originalMethod == null)
            {
                Mod.log.Error($"Unable to find original method {nameof(CityInfoUISystem)}.WriteHappinessFactors.");
                return;
            }
            MethodInfo prefixMethod = typeof(PatchCityInfoUISystem).GetMethod(nameof(WriteHappinessFactors), BindingFlags.Static | BindingFlags.NonPublic);
            if (prefixMethod == null)
            {
                Mod.log.Error($"Unable to find patch prefix method {nameof(PatchCityInfoUISystem)}.{nameof(WriteHappinessFactors)}.");
                return;
            }
            new Harmony(HappinessUtils.HarmonyID).Patch(originalMethod, new HarmonyMethod(prefixMethod), null);
        }

        /// <summary>
        /// Obtain and write city happiness factors to the UI.
        /// </summary>
        private static bool WriteHappinessFactors(IJsonWriter writer)
        {
            // Call WriteHappinessFactors implementation for this instance of PatchCityInfoUISystem.
            return _patchCityInfoUISystem.WriteHappinessFactorsImpl(writer);
        }

        /// <summary>
        /// Implementation to obtain and write city happiness factors to the UI.
        /// </summary>
        private bool WriteHappinessFactorsImpl(IJsonWriter writer)
        {
            // Logic adapted from CityInfoUISystem.WriteHappinessFactors() except zeroes are included or excluded according to the mod settings.
            NativeList<FactorInfo> list = new NativeList<FactorInfo>((int)CitizenHappinessSystem.HappinessFactor.Count, Allocator.Temp);
            EntityQuery entityQuery = GetEntityQuery(ComponentType.ReadOnly<HappinessFactorParameterData>());
            if (!entityQuery.IsEmptyIgnoreFilter)
            {
                Entity singletonEntity = entityQuery.GetSingletonEntity();
                DynamicBuffer<HappinessFactorParameterData> buffer = base.EntityManager.GetBuffer<HappinessFactorParameterData>(singletonEntity, isReadOnly: true);
                ComponentLookup<Locked> locked = SystemAPI.GetComponentLookup<Locked>(true);
                for (int i = 0; i < (int)CitizenHappinessSystem.HappinessFactor.Count; i++)
                {
                    int num = Mathf.RoundToInt(_citizenHappinessSystem.GetHappinessFactor((CitizenHappinessSystem.HappinessFactor)i, buffer, ref locked).x);
                    if (Mod.ModSettings.ShowZeroValues || num != 0)
                    {
                        list.Add(new FactorInfo(i, num));
                    }
                }
            }

            // Sort happiness factors using this mod's sort method.
            HappinessUtils.SortHapinessFactors(list);

            // Write the happiness factors to the UI.
            // Logic copied from CityInfoUISystem.WriteHappinessFactors() except use maximum factors from the mod settings.
            try
            {
                int num2 = math.min(Mod.ModSettings.MaximumFactors, list.Length);
                writer.ArrayBegin(num2);
                for (int j = 0; j < num2; j++)
                {
                    list[j].WriteHappinessFactor(writer);
                }
                writer.ArrayEnd();
            }
            finally
            {
                list.Dispose();
            }

            // Do not call original method.
            return false;
        }

    }
}

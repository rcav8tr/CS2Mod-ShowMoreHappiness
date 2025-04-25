using Colossal.IO.AssetDatabase;
using Game;
using Game.Modding;
using System;
using Unity.Entities;

namespace ShowMoreHappiness
{
    public class Mod : IMod
    {
        // The global settings for this mod.
        public static ModSettings ModSettings { get; set; }

        /// <summary>
        /// One-time mod loading.
        /// </summary>
        public void OnLoad(UpdateSystem updateSystem)
        {
            LogUtil.Info($"{nameof(Mod)}.{nameof(OnLoad)} Version {ModAssemblyInfo.Version}");
            
            try
            {
                // Register and load mod settings.
                ModSettings = new ModSettings(this);
                ModSettings.RegisterInOptionsUI();
                AssetDatabase.global.LoadSettings(ModAssemblyInfo.Name, ModSettings, new ModSettings(this));

                // Initialize translations.
                Translation.Initialize();

                // Initialize patch for AverageHappinessSection.
                PatchAverageHappinessSection.Initialize();

                // Create this mod's PatchCityInfoUISystem in the default world.
                // This system does nothing in its OnUpdate() method.
                // Therefore, this system does not need to be activated.
                // This system just needs to be created.
                World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<PatchCityInfoUISystem>();

#if DEBUG
                // Get localized text from the game where the value is or contains specific text.
                //Colossal.Localization.LocalizationManager localizationManager = Game.SceneFlow.GameManager.instance.localizationManager;
                //foreach (System.Collections.Generic.KeyValuePair<string, string> keyValue in localizationManager.activeDictionary.entries)
                //{
                //    // Exclude assets.
                //    if (!keyValue.Key.StartsWith("Assets."))
                //    {
                //        if (keyValue.Value.ToLower().Contains("separate"))
                //        //if (keyValue.Value.StartsWith("Cargo"))
                //        {
                //            LogUtil.Info(keyValue.Key + "\t" + keyValue.Value);
                //        }
                //    }
                //}

                // For a specific localization key, get the localized text for each base game locale ID.
                //string[] localeIDs = new string[] { "en-US", "de-DE", "es-ES", "fr-FR", "it-IT", "ja-JP", "ko-KR", "pl-PL", "pt-BR", "ru-RU", "zh-HANS", "zh-HANT" };
                //foreach (string localeID in localeIDs)
                //{
                //    localizationManager.SetActiveLocale(localeID);
                //    foreach (System.Collections.Generic.KeyValuePair<string, string> keyValue in localizationManager.activeDictionary.entries)
                //    {
                //        if (keyValue.Key == "EconomyPanel.PRODUCTION_PAGE_PRODUCTIONLINK[Import]")
                //        {
                //            LogUtil.Info(keyValue.Key + "\t" + localeID + "\t" + keyValue.Value);
                //            break;
                //        }
                //    }
                //}
                //localizationManager.SetActiveLocale("en-US");

                // Create UI files.
                // Uncomment this only when the UI files need to be created or recreated.
                // Then run the mod once in the game to create the files.
                // Then comment this again.  The UI files are now available to use.
                //CreateUIFiles.Create();
#endif
            }
            catch (Exception ex)
            {
                LogUtil.Exception(ex);
            }

            LogUtil.Info($"{nameof(Mod)}.{nameof(OnLoad)} complete.");
        }

        /// <summary>
        /// One-time mod disposing.
        /// </summary>
        public void OnDispose()
        {
            LogUtil.Info($"{nameof(Mod)}.{nameof(OnDispose)}");

            // Unregister mod settings.
            ModSettings?.UnregisterInOptionsUI();
            ModSettings = null;
        }
    }
}

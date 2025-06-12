using Colossal.IO.AssetDatabase;
using Game.Buildings;
using Game.Modding;
using Game.Settings;
using Game.Simulation;
using Game.UI;
using Game.UI.InGame;
using Unity.Entities;

namespace ShowMoreHappiness
{
    [FileLocation(nameof(ShowMoreHappiness))]
    [SettingsUIGroupOrder(GroupGeneral, GroupAbout)]
    [SettingsUIShowGroupName(GroupGeneral, GroupAbout)]
    public class ModSettings : ModSetting
    {
        // Group constants.
        public const string GroupGeneral = "General";
        public const string GroupAbout   = "About";

        // Constructor.
        public ModSettings(IMod mod) : base(mod)
        {
            Mod.log.Info($"{nameof(ModSettings)}.{nameof(ModSettings)}");

            SetDefaults();
        }
        
        /// <summary>
        /// Set a default value for every setting that has a value that can change.
        /// </summary>
        public override void SetDefaults()
        {
            // It is important to set a default for every value.
            // These default values will show happiness like the base game.
            MaximumFactors = 10;
            ShowZeroValues = false;
            PositiveNegativeValues = PositiveNegativeValuesChoice.Interspersed;
            SortDirection = SortDirectionChoice.Descending;
        }

        // Maximum number of happiness factors to show.
        // Applies to both city happiness and district/building/household happiness.
        // So make the slider go up to greater of the two.
        private const int sliderMax =
            (int)BuildingHappinessFactor.Count > (int)CitizenHappinessSystem.HappinessFactor.Count ?
            (int)BuildingHappinessFactor.Count : (int)CitizenHappinessSystem.HappinessFactor.Count;
        private int _maximumFactors;
        [SettingsUISection(GroupGeneral)]
        [SettingsUISlider(min = 1f, max = sliderMax, step = 1f, scalarMultiplier = 1f, unit = Unit.kInteger)]
        public int MaximumFactors
        {
            get { return _maximumFactors; }
            set { _maximumFactors = value; RequestUpdates(); }
        }

        // Show happiness factors with a zero value.
        private bool _showZeroValues;
        [SettingsUISection(GroupGeneral)]
        public bool ShowZeroValues
        {
            get { return _showZeroValues; }
            set { _showZeroValues = value; RequestUpdates(); }
        }

        // How to show positive and negative values.
        public enum PositiveNegativeValuesChoice
        {
            Interspersed,   // Show positive and negative values interspersed with each other (i.e. sorting based on absolute value).
            Separate,       // Show positive and negative values separately from each other (i.e. normal numeric sorting).
        }
        private PositiveNegativeValuesChoice _positiveNegativeValues;
        [SettingsUISection(GroupGeneral)]
        public PositiveNegativeValuesChoice PositiveNegativeValues
        {
            get { return _positiveNegativeValues; }
            set { _positiveNegativeValues = value; RequestUpdates(); }
        }

        // Sort direction.
        public enum SortDirectionChoice
        {
            Descending,
            Ascending,
        }
        private SortDirectionChoice _sortDirection;
        [SettingsUISection(GroupGeneral)]
        public SortDirectionChoice SortDirection
        {
            get { return _sortDirection; }
            set { _sortDirection = value; RequestUpdates(); }
        }

        // Button to reset settings.
        [SettingsUISection(GroupGeneral)]
        [SettingsUIButton()]
        public bool ResetSettings
        {
            set
            {
                // Set defaults.
                SetDefaults();

                // Because settings were changed thru code, need to save settings explicitly.
                // This saves settings for the game and all mods.
                AssetDatabase.global.SaveSettings();
            }
        }

        /// <summary>
        /// Request an update in AverageHappinessSection.
        /// </summary>
        private void RequestUpdates()
        {
            World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<AverageHappinessSection>().RequestUpdate();
        }


        // Display mod version in settings.
        [SettingsUISection(GroupAbout)]
        public string ModVersion { get { return ModAssemblyInfo.Version; } }
    }
}

// This entire file is only for creating UI files when in DEBUG.
#if DEBUG

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ShowMoreHappiness
{
    /// <summary>
    /// Create the files for UI:
    ///     One file for UI translation keys for C#.  Includes settings.
    /// </summary>
    public static class CreateUIFiles
    {
        // Shortcut for UI constants dictionary.
        // Dictionary key is the constant name.
        // Dictionary value is the constant value.
        private class UIConstants : Dictionary<string, string> { }

        // Shortcut for translation keys list.
        // Entry is used for constant name and constant value suffix.
        private class TranslationKeys : List<string> { }

        /// <summary>
        /// Create the UI file.
        /// </summary>
        public static void Create()
        {
            CreateFileUITranslationKeys();
        }

        /// <summary>
        /// Create the file for UI transtion keys.
        /// One file for C# (i.e. CS) and one file for UI.
        /// </summary>
        private static void CreateFileUITranslationKeys()
        {
            // Start with the do not modify instructions.
            StringBuilder sb = new StringBuilder();
            sb.Append(DoNotModify());

            // Include namespace.
            sb.AppendLine($"namespace {ModAssemblyInfo.Name}");
            sb.AppendLine("{");

            // Start class.
            const string className = "UITranslationKey";
            sb.AppendLine("    // Define UI translation keys.");
            sb.AppendLine("    public class " + className);
            sb.AppendLine("    {");

            // Include title and description.
            TranslationKeys titleDescription = new TranslationKeys()
            {
                "Title",
                "Description",
            };
            sb.Append(GetTranslationsContent("Mod title and description.", titleDescription));

            // Include settings translations.
            // Construct settings.
            UIConstants _translationKeySettings = new UIConstants()
            {
                { "SettingTitle",                                       Mod.ModSettings.GetSettingsLocaleID()                                                             },
                                                                                                                                                                             
                                                                                                                                                                             
                { "SettingGroupGeneral",                                Mod.ModSettings.GetOptionGroupLocaleID(ModSettings.GroupGeneral)                                  },
                                                                                                                                                                              
                { "SettingMaximumFactorsLabel",                         Mod.ModSettings.GetOptionLabelLocaleID(nameof(ModSettings.MaximumFactors                       )) },
                { "SettingMaximumFactorsDesc",                          Mod.ModSettings.GetOptionDescLocaleID (nameof(ModSettings.MaximumFactors                       )) },
                                                                                                                                                                                     
                { "SettingShowZeroValuesLabel",                         Mod.ModSettings.GetOptionLabelLocaleID(nameof(ModSettings.ShowZeroValues                       )) },
                { "SettingShowZeroValuesDesc",                          Mod.ModSettings.GetOptionDescLocaleID (nameof(ModSettings.ShowZeroValues                       )) },
                                                                                                                                                                                    
                { "SettingPositiveNegativeValuesLabel",                 Mod.ModSettings.GetOptionLabelLocaleID(nameof(ModSettings.PositiveNegativeValues               )) },
                { "SettingPositiveNegativeValuesDesc",                  Mod.ModSettings.GetOptionDescLocaleID (nameof(ModSettings.PositiveNegativeValues               )) },
                { "SettingPositiveNegativeValuesChoiceInterspersed",    Mod.ModSettings.GetEnumValueLocaleID  (ModSettings.PositiveNegativeValuesChoice.Interspersed    ) },
                { "SettingPositiveNegativeValuesChoiceSeparate",        Mod.ModSettings.GetEnumValueLocaleID  (ModSettings.PositiveNegativeValuesChoice.Separate        ) },
                                                                                                                                                                                    
                { "SettingSortDirectionLabel",                          Mod.ModSettings.GetOptionLabelLocaleID(nameof(ModSettings.SortDirection                        )) },
                { "SettingSortDirectionDesc",                           Mod.ModSettings.GetOptionDescLocaleID (nameof(ModSettings.SortDirection                        )) },
                { "SettingSortDirectionChoiceDescending",               Mod.ModSettings.GetEnumValueLocaleID  (ModSettings.SortDirectionChoice.Descending               ) },
                { "SettingSortDirectionChoiceAscending",                Mod.ModSettings.GetEnumValueLocaleID  (ModSettings.SortDirectionChoice.Ascending                ) },
                                                                                                                                                                                     
                { "SettingResetSettingsLabel",                          Mod.ModSettings.GetOptionLabelLocaleID(nameof(ModSettings.ResetSettings                        )) },
                { "SettingResetSettingsDesc",                           Mod.ModSettings.GetOptionDescLocaleID (nameof(ModSettings.ResetSettings                        )) },
                                                                                                                                                                                     
                                                                                                                                                                                     
                { "SettingGroupAbout",                                  Mod.ModSettings.GetOptionGroupLocaleID(ModSettings.GroupAbout)                                    },
                                                                                                                                                                                     
                { "SettingModVersionLabel",                             Mod.ModSettings.GetOptionLabelLocaleID(nameof(ModSettings.ModVersion                           )) },
                { "SettingModVersionDesc",                              Mod.ModSettings.GetOptionDescLocaleID (nameof(ModSettings.ModVersion                           )) },
            };

            // Append settings to the file.
            sb.AppendLine();
            sb.Append(GetTranslationsContent("Settings.", _translationKeySettings));

            // End class.
            sb.AppendLine("    }");

            // End namespace.
            sb.AppendLine("}");

            // Write the file to the Localization folder.
            string uiBindingsPath = Path.Combine(GetSourceCodePath(), "Localization", "UITranslationKey.cs");
            File.WriteAllText(uiBindingsPath, sb.ToString());
        }

        /// <summary>
        /// Get instructions for do not modify.
        /// </summary>
        /// <returns></returns>
        private static string DoNotModify()
        {
            StringBuilder sb = new StringBuilder();
            // Include do not modify instructions.
            sb.AppendLine($"// DO NOT MODIFY THIS FILE.");
            sb.AppendLine($"// This entire file was automatically generated by class {nameof(CreateUIFiles)}.");
            sb.AppendLine($"// Make any needed changes in class {nameof(CreateUIFiles)}.");
            sb.AppendLine();
            return sb.ToString();
        }

        /// <summary>
        /// Get the constants content.
        /// </summary>
        private static string GetTranslationsContent(string comment, UIConstants constants)
        {
            string indentation = ("        ");
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{indentation}// {comment}");
            foreach (var key in constants.Keys)
            {
                sb.AppendLine($"{indentation}public const string {key.PadRight(50)} = \"{constants[key]}\";");
            }
            return sb.ToString();
        }

        /// <summary>
        /// Get the translations content.
        /// </summary>
        private static string GetTranslationsContent(string comment, TranslationKeys translationKeys)
        {
            string indentation = ("        ");
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{indentation}// {comment}");
            foreach (var translationKey in translationKeys)
            {
                sb.AppendLine($"{indentation}public const string {translationKey.PadRight(50)} = \"{ModAssemblyInfo.Name}.{translationKey}\";");
            }
            return sb.ToString();
        }

        /// <summary>
        /// Get the full path of this C# source code file.
        /// </summary>
        private static string GetSourceCodePath([System.Runtime.CompilerServices.CallerFilePath] string sourceFile = "")
        {
            return Path.GetDirectoryName(sourceFile);
        }
    }
}

#endif

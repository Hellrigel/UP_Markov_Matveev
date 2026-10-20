using System;
using System.IO;
using System.Windows;

namespace UP_Markov_Matveev
{
    
    public static class ThemeManager
    {
        public const string LightTheme = "Dictionary.xaml";
        public const string DarkTheme = "DictionaryAlt.xaml";

        private static readonly string SettingsFolder =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "UP_Markov_Matveev");

        private static readonly string SettingsFile = Path.Combine(SettingsFolder, "theme.cfg");

        public static string CurrentTheme { get; private set; } = LightTheme;

        
        public static void ApplyTheme(string themeFileName)
        {
            var uri = new Uri($"Styles/{themeFileName}", UriKind.Relative);
            ResourceDictionary resourceDict = Application.LoadComponent(uri) as ResourceDictionary;

            Application.Current.Resources.Clear();
            Application.Current.Resources.MergedDictionaries.Add(resourceDict);

            CurrentTheme = themeFileName;
        }

        public static void ToggleTheme()
        {
            ApplyTheme(CurrentTheme == LightTheme ? DarkTheme : LightTheme);
        }

        public static void SaveCurrentTheme()
        {
            try
            {
                Directory.CreateDirectory(SettingsFolder);
                File.WriteAllText(SettingsFile, CurrentTheme);
            }
            catch
            {
                
            }
        }

        public static string LoadSavedTheme()
        {
            try
            {
                if (File.Exists(SettingsFile))
                {
                    string saved = File.ReadAllText(SettingsFile).Trim();
                    if (saved == LightTheme || saved == DarkTheme)
                        return saved;
                }
            }
            catch
            {
            }

            return LightTheme;
        }
    }
}

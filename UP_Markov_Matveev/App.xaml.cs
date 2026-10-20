using System.Windows;

namespace UP_Markov_Matveev
{
    
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            string savedTheme = ThemeManager.LoadSavedTheme();
            ThemeManager.ApplyTheme(savedTheme);
        }
    }
}

using System;
using System.Windows;
using UP_Markov_Matveev.Pages;

namespace UP_Markov_Matveev
{
    
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new AuthPage());
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.IsEnabled = true;
            timer.Tick += (o, t) => { DateTimeNow.Text = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"); };
            timer.Start();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что хотите закрыть приложение?", "Подтверждение",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
            {
                e.Cancel = true;
                return;
            }

            ThemeManager.SaveCurrentTheme();
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            if (MainFrame.CanGoBack)
                MainFrame.GoBack();
        }

        private void ButtonTheme_Click(object sender, RoutedEventArgs e)
        {
            ThemeManager.ToggleTheme();

            
            if (MainFrame.Content != null)
            {
                var currentPageType = MainFrame.Content.GetType();
                if (currentPageType.GetConstructor(Type.EmptyTypes) != null)
                    MainFrame.Navigate(Activator.CreateInstance(currentPageType));
            }
        }
    }
}

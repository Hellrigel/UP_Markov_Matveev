using System.Windows;
using System.Windows.Controls;

namespace UP_Markov_Matveev.Pages
{
    public partial class AdminPage : Page
    {
        private readonly Users _currentUser;

        public AdminPage() : this(null)
        {
        }

        public AdminPage(Users currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;

            if (_currentUser != null)
                WelcomeText.Text = $"Панель администратора — {_currentUser.FIO}";
        }

        private void BtnTab1_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new ClientsTabPage());
        private void BtnTab2_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new WarehousesTabPage());
        private void BtnTab3_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new OrdersTabPage());
        private void BtnTab4_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new CargoTabPage());
        private void BtnTab5_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new DiagrammPage());

        private void ButtonChangePassword_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new ChangePassPage());
        }

        private void ButtonLogout_Click(object sender, RoutedEventArgs e)
        {
            Entities.ResetContext();
            NavigationService?.Navigate(new AuthPage());
        }
    }
}

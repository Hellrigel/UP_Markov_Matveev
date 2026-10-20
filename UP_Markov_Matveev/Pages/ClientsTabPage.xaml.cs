using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace UP_Markov_Matveev.Pages
{
    public partial class ClientsTabPage : Page
    {
        public ClientsTabPage()
        {
            InitializeComponent();
            LoadData();
            IsVisibleChanged += Page_IsVisibleChanged;
        }

        private void LoadData()
        {
            using (var db = new Entities())
            {
                DataGridClients.ItemsSource = db.Clients
                    .Include(nameof(Clients.ClientType))
                    .Include(nameof(Clients.City))
                    .ToList();
            }
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
                LoadData();
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AddClientPage(null));
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            var selected = (sender as Button)?.DataContext as Clients;
            NavigationService?.Navigate(new AddClientPage(selected));
        }

        private void ButtonDel_Click(object sender, RoutedEventArgs e)
        {
            var itemsForRemoving = DataGridClients.SelectedItems.Cast<Clients>().ToList();
            if (itemsForRemoving.Count == 0)
            {
                MessageBox.Show("Выберите хотя бы одну запись для удаления.");
                return;
            }

            if (MessageBox.Show($"Вы точно хотите удалить записи в количестве {itemsForRemoving.Count} элементов? " +
                    "Все заказы этого клиента будут удалены каскадно вместе с его грузами.",
                    "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;

            try
            {
                using (var db = new Entities())
                {
                    var ids = itemsForRemoving.Select(c => c.ClientId).ToList();
                    var toRemove = db.Clients.Where(c => ids.Contains(c.ClientId)).ToList();
                    db.Clients.RemoveRange(toRemove);
                    db.SaveChanges();
                }

                MessageBox.Show("Данные успешно удалены!");
                LoadData();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ButtonBackToAdmin_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AdminPage());
        }
    }
}

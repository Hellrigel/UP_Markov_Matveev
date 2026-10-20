using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace UP_Markov_Matveev.Pages
{
    
    public partial class OrdersTabPage : Page
    {
        public OrdersTabPage()
        {
            InitializeComponent();
            LoadData();
            IsVisibleChanged += Page_IsVisibleChanged;
        }

        private void LoadData()
        {
            using (var db = new Entities())
            {
                DataGridOrders.ItemsSource = db.Orders
                    .Include(nameof(Orders.Client))
                    .Include(nameof(Orders.Warehouse))
                    .Include(nameof(Orders.OrderStatus))
                    .OrderByDescending(o => o.OrderDate)
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
            NavigationService?.Navigate(new AddOrderPage(null));
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            var selected = (sender as Button)?.DataContext as Orders;
            NavigationService?.Navigate(new AddOrderPage(selected));
        }

        private void ButtonDel_Click(object sender, RoutedEventArgs e)
        {
            var itemsForRemoving = DataGridOrders.SelectedItems.Cast<Orders>().ToList();
            if (itemsForRemoving.Count == 0)
            {
                MessageBox.Show("Выберите хотя бы одну запись для удаления.");
                return;
            }

            if (MessageBox.Show($"Вы точно хотите удалить записи в количестве {itemsForRemoving.Count} элементов? " +
                    "Все грузы этих заказов будут удалены каскадно.",
                    "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;

            try
            {
                using (var db = new Entities())
                {
                    var ids = itemsForRemoving.Select(o => o.OrderId).ToList();
                    var toRemove = db.Orders.Where(o => ids.Contains(o.OrderId)).ToList();
                    db.Orders.RemoveRange(toRemove);
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

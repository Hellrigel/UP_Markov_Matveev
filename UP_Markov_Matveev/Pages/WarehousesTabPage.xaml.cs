using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace UP_Markov_Matveev.Pages
{
    
    public partial class WarehousesTabPage : Page
    {
        public WarehousesTabPage()
        {
            InitializeComponent();
            LoadData();
            IsVisibleChanged += Page_IsVisibleChanged;
        }

        private void LoadData()
        {
            using (var db = new Entities())
            {
                DataGridWarehouses.ItemsSource = db.Warehouses.Include(nameof(Warehouses.City)).ToList();
            }
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
                LoadData();
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AddWarehousePage(null));
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            var selected = (sender as Button)?.DataContext as Warehouses;
            NavigationService?.Navigate(new AddWarehousePage(selected));
        }

        private void ButtonDel_Click(object sender, RoutedEventArgs e)
        {
            var itemsForRemoving = DataGridWarehouses.SelectedItems.Cast<Warehouses>().ToList();
            if (itemsForRemoving.Count == 0)
            {
                MessageBox.Show("Выберите хотя бы одну запись для удаления.");
                return;
            }

            if (MessageBox.Show($"Вы точно хотите удалить записи в количестве {itemsForRemoving.Count} элементов?",
                    "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;

            try
            {
                using (var db = new Entities())
                {
                    var ids = itemsForRemoving.Select(w => w.WarehouseId).ToList();
                    var toRemove = db.Warehouses.Where(w => ids.Contains(w.WarehouseId)).ToList();
                    db.Warehouses.RemoveRange(toRemove);
                    db.SaveChanges();
                }

                MessageBox.Show("Данные успешно удалены!");
                LoadData();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Не удалось удалить запись. Возможно, на этот склад ссылаются существующие заказы.\n" + ex.Message);
            }
        }

        private void ButtonBackToAdmin_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AdminPage());
        }
    }
}

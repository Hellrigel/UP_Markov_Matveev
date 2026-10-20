using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace UP_Markov_Matveev.Pages
{
    
    public partial class CargoTabPage : Page
    {
        public CargoTabPage()
        {
            InitializeComponent();
            LoadData();
            IsVisibleChanged += Page_IsVisibleChanged;
        }

        private void LoadData()
        {
            using (var db = new Entities())
            {
                DataGridCargo.ItemsSource = db.Cargo.Include(nameof(Cargo.CargoType)).ToList();
            }
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
                LoadData();
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AddCargoPage(null));
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            var selected = (sender as Button)?.DataContext as Cargo;
            NavigationService?.Navigate(new AddCargoPage(selected));
        }

        private void ButtonDel_Click(object sender, RoutedEventArgs e)
        {
            var itemsForRemoving = DataGridCargo.SelectedItems.Cast<Cargo>().ToList();
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
                    var ids = itemsForRemoving.Select(c => c.CargoId).ToList();
                    var toRemove = db.Cargo.Where(c => ids.Contains(c.CargoId)).ToList();
                    db.Cargo.RemoveRange(toRemove);
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

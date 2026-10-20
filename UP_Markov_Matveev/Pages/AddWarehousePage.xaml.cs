using System.Linq;
using System.Text;
using System.Windows;

namespace UP_Markov_Matveev.Pages
{
    public partial class AddWarehousePage : System.Windows.Controls.Page
    {
        private Warehouses _currentWarehouse;
        private readonly bool _isNew;

        public AddWarehousePage(Warehouses selectedWarehouse)
        {
            InitializeComponent();

            using (var db = new Entities())
            {
                CBCity.ItemsSource = db.Cities.ToList();
            }

            _isNew = selectedWarehouse == null;
            _currentWarehouse = selectedWarehouse ?? new Warehouses();

            DataContext = _currentWarehouse;
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            var errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(_currentWarehouse.Name))
                errors.AppendLine("Укажите название склада!");
            if (CBCity.SelectedValue == null)
                errors.AppendLine("Выберите город!");
            if (string.IsNullOrWhiteSpace(_currentWarehouse.Address))
                errors.AppendLine("Укажите адрес!");
            if (_currentWarehouse.Capacity <= 0)
                errors.AppendLine("Вместимость должна быть больше нуля!");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString(), "Проверьте введённые данные");
                return;
            }

            try
            {
                using (var db = new Entities())
                {
                    if (_isNew)
                    {
                        db.Warehouses.Add(_currentWarehouse);
                    }
                    else
                    {
                        db.Warehouses.Attach(_currentWarehouse);
                        db.Entry(_currentWarehouse).State = System.Data.Entity.EntityState.Modified;
                    }

                    db.SaveChanges();
                }

                MessageBox.Show("Данные успешно сохранены!");
                NavigationService?.Navigate(new WarehousesTabPage());
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new WarehousesTabPage());
        }
    }
}

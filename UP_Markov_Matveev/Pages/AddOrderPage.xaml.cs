using System.Linq;
using System.Text;
using System.Windows;

namespace UP_Markov_Matveev.Pages
{
    public partial class AddOrderPage : System.Windows.Controls.Page
    {
        private Orders _currentOrder;
        private readonly bool _isNew;

        public AddOrderPage(Orders selectedOrder)
        {
            InitializeComponent();

            using (var db = new Entities())
            {
                CBClient.ItemsSource = db.Clients.ToList();
                CBWarehouse.ItemsSource = db.Warehouses.ToList();
                CBStatus.ItemsSource = db.OrderStatuses.ToList();
            }

            _isNew = selectedOrder == null;
            _currentOrder = selectedOrder ?? new Orders();

            DataContext = _currentOrder;

            if (_isNew && CBStatus.Items.Count > 0)
                CBStatus.SelectedIndex = 0;
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            var errors = new StringBuilder();

            if (CBClient.SelectedValue == null)
                errors.AppendLine("Выберите клиента!");
            if (CBStatus.SelectedValue == null)
                errors.AppendLine("Выберите статус заказа!");
            if (string.IsNullOrWhiteSpace(_currentOrder.PickupAddress))
                errors.AppendLine("Укажите адрес отправления!");
            if (string.IsNullOrWhiteSpace(_currentOrder.DeliveryAddress))
                errors.AppendLine("Укажите адрес доставки!");
            if (DPOrderDate.SelectedDate == null)
                errors.AppendLine("Укажите дату заказа!");
            if (_currentOrder.Cost < 0)
                errors.AppendLine("Стоимость не может быть отрицательной!");

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
                        db.Orders.Add(_currentOrder);
                    }
                    else
                    {
                        db.Orders.Attach(_currentOrder);
                        db.Entry(_currentOrder).State = System.Data.Entity.EntityState.Modified;
                    }

                    db.SaveChanges();
                }

                MessageBox.Show("Данные успешно сохранены!");
                NavigationService?.Navigate(new OrdersTabPage());
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new OrdersTabPage());
        }
    }
}

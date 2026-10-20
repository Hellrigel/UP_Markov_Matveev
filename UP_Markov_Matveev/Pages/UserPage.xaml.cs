using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace UP_Markov_Matveev.Pages
{
   
    public partial class UserPage : Page
    {
        private readonly Users _currentUser;
        private List<Orders> _allOrders;

        public UserPage() : this(null)
        {
        }

        public UserPage(Users currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;

            if (_currentUser != null)
                WelcomeText.Text = $"Заказы логистической компании — {_currentUser.FIO}";

            LoadStatuses();
            LoadOrders();
        }

        private void LoadStatuses()
        {
            using (var db = new Entities())
            {
                var statuses = db.OrderStatuses.ToList();
                statuses.Insert(0, new OrderStatuses { OrderStatusId = 0, Name = "Все статусы" });
                statusFilterComboBox.ItemsSource = statuses;
                statusFilterComboBox.DisplayMemberPath = "Name";
                statusFilterComboBox.SelectedIndex = 0;
            }
        }

        private void LoadOrders()
        {
            using (var db = new Entities())
            {
                _allOrders = db.Orders
                    .Select(o => o)
                    .ToList();

                foreach (var order in _allOrders)
                {
                    db.Entry(order).Reference(o => o.Client).Load();
                    db.Entry(order).Reference(o => o.OrderStatus).Load();
                }
            }

            UpdateOrders();
        }

        private void UpdateOrders()
        {
            if (!IsInitialized || _allOrders == null)
                return;

            IEnumerable<Orders> result = _allOrders;

            if (!string.IsNullOrWhiteSpace(searchTextBox.Text))
            {
                string q = searchTextBox.Text.ToLower();
                result = result.Where(o =>
                    (o.Client != null && o.Client.Name.ToLower().Contains(q)) ||
                    o.PickupAddress.ToLower().Contains(q) ||
                    o.DeliveryAddress.ToLower().Contains(q));
            }

            if (statusFilterComboBox.SelectedItem is OrderStatuses status && status.OrderStatusId != 0)
            {
                result = result.Where(o => o.OrderStatusId == status.OrderStatusId);
            }

            result = sortComboBox.SelectedIndex == 0
                ? result.OrderBy(o => o.OrderDate)
                : result.OrderByDescending(o => o.OrderDate);

            OrdersDataGrid.ItemsSource = result.ToList();
        }

        private void searchTextBox_TextChanged(object sender, TextChangedEventArgs e) => UpdateOrders();

        private void Filter_SelectionChanged(object sender, SelectionChangedEventArgs e) => UpdateOrders();

        private void clearFiltersButton_Click(object sender, RoutedEventArgs e)
        {
            searchTextBox.Text = "";
            statusFilterComboBox.SelectedIndex = 0;
            sortComboBox.SelectedIndex = 1;
        }

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

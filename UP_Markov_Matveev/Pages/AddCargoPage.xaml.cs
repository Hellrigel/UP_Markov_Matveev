using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace UP_Markov_Matveev.Pages
{
    public partial class AddCargoPage : System.Windows.Controls.Page
    {
        private Cargo _currentCargo;
        private readonly bool _isNew;

        private class OrderPickerItem
        {
            public int OrderId { get; set; }
            public string DisplayLabel { get; set; }
        }

        public AddCargoPage(Cargo selectedCargo)
        {
            InitializeComponent();

            using (var db = new Entities())
            {
                var orderItems = db.Orders
                    .Select(o => new { o.OrderId, ClientName = o.Client.Name, o.PickupAddress, o.DeliveryAddress })
                    .ToList()
                    .Select(o => new OrderPickerItem
                    {
                        OrderId = o.OrderId,
                        DisplayLabel = $"№{o.OrderId} — {o.ClientName} ({o.PickupAddress} → {o.DeliveryAddress})"
                    })
                    .ToList();

                CBOrder.ItemsSource = orderItems;
                CBCargoType.ItemsSource = db.CargoTypes.ToList();
            }

            _isNew = selectedCargo == null;
            _currentCargo = selectedCargo ?? new Cargo();

            DataContext = _currentCargo;
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            var errors = new StringBuilder();

            if (CBOrder.SelectedValue == null)
                errors.AppendLine("Выберите заказ!");
            if (CBCargoType.SelectedValue == null)
                errors.AppendLine("Выберите тип груза!");
            if (string.IsNullOrWhiteSpace(_currentCargo.Name))
                errors.AppendLine("Укажите наименование груза!");
            if (_currentCargo.WeightKg <= 0)
                errors.AppendLine("Вес должен быть больше нуля!");
            if (_currentCargo.VolumeM3 <= 0)
                errors.AppendLine("Объём должен быть больше нуля!");

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
                        db.Cargo.Add(_currentCargo);
                    }
                    else
                    {
                        db.Cargo.Attach(_currentCargo);
                        db.Entry(_currentCargo).State = System.Data.Entity.EntityState.Modified;
                    }

                    db.SaveChanges();
                }

                MessageBox.Show("Данные успешно сохранены!");
                NavigationService?.Navigate(new CargoTabPage());
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new CargoTabPage());
        }
    }
}

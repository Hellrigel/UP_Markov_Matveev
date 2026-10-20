using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace UP_Markov_Matveev.Pages
{
    public partial class AddClientPage : Page
    {
        private Clients _currentClient;
        private readonly bool _isNew;

        public AddClientPage(Clients selectedClient)
        {
            InitializeComponent();

            using (var db = new Entities())
            {
                CBClientType.ItemsSource = db.ClientTypes.ToList();
                CBCity.ItemsSource = db.Cities.ToList();
            }

            _isNew = selectedClient == null;
            _currentClient = selectedClient ?? new Clients();

            DataContext = _currentClient;
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            var errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(_currentClient.Name))
                errors.AppendLine("Укажите имя или название организации!");
            if (CBClientType.SelectedValue == null)
                errors.AppendLine("Выберите тип клиента!");
            if (CBCity.SelectedValue == null)
                errors.AppendLine("Выберите город!");
            if (string.IsNullOrWhiteSpace(_currentClient.Phone))
                errors.AppendLine("Укажите телефон!");
            if (string.IsNullOrWhiteSpace(_currentClient.Address))
                errors.AppendLine("Укажите адрес!");

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
                        db.Clients.Add(_currentClient);
                    }
                    else
                    {
                        db.Clients.Attach(_currentClient);
                        db.Entry(_currentClient).State = System.Data.Entity.EntityState.Modified;
                    }

                    db.SaveChanges();
                }

                MessageBox.Show("Данные успешно сохранены!");
                NavigationService?.Navigate(new ClientsTabPage());
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new ClientsTabPage());
        }
    }
}

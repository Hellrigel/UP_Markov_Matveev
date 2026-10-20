using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace UP_Markov_Matveev.Pages
{
    
    public partial class RegPage : Page
    {
        public RegPage()
        {
            InitializeComponent();
        }

        private void ButtonRegister_Click(object sender, RoutedEventArgs e)
        {
            var errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(txtbxLog.Text))
                errors.AppendLine("Укажите логин!");
            if (string.IsNullOrWhiteSpace(txtbxFIO.Text))
                errors.AppendLine("Укажите ФИО!");
            if (string.IsNullOrEmpty(passBxFrst.Password))
                errors.AppendLine("Укажите пароль!");
            if (passBxFrst.Password != passBxScnd.Password)
                errors.AppendLine("Пароли не совпадают!");
            if (!string.IsNullOrEmpty(passBxFrst.Password) && passBxFrst.Password.Length < 6)
                errors.AppendLine("Пароль должен содержать не менее 6 символов!");
            if (comboBxRole.SelectedItem == null)
                errors.AppendLine("Выберите роль!");

            using (var db = new Entities())
            {
                if (!string.IsNullOrWhiteSpace(txtbxLog.Text) &&
                    db.Users.Any(u => u.Login == txtbxLog.Text))
                {
                    errors.AppendLine("Пользователь с таким логином уже существует!");
                }

                if (errors.Length > 0)
                {
                    MessageBox.Show(errors.ToString(), "Проверьте введённые данные");
                    return;
                }

                var newUser = new Users
                {
                    Login = txtbxLog.Text,
                    FIO = txtbxFIO.Text,
                    Password = PasswordHelper.GetHash(passBxFrst.Password),
                    Role = (comboBxRole.SelectedItem as ComboBoxItem)?.Content.ToString()
                };

                db.Users.Add(newUser);

                try
                {
                    db.SaveChanges();
                    MessageBox.Show("Регистрация прошла успешно! Теперь вы можете войти.");
                    NavigationService?.Navigate(new AuthPage());
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AuthPage());
        }

        private void txtbxLog_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void txtbxFIO_TextChanged(object sender, TextChangedEventArgs e)
        {
        }
    }
}

using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace UP_Markov_Matveev.Pages
{
    public partial class ChangePassPage : Page
    {
        public ChangePassPage()
        {
            InitializeComponent();
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(CurrentPasswordBox.Password) ||
                string.IsNullOrEmpty(NewPasswordBox.Password) ||
                string.IsNullOrEmpty(ConfirmPasswordBox.Password) ||
                string.IsNullOrEmpty(TbLogin.Text))
            {
                MessageBox.Show("Все поля обязательны к заполнению!");
                return;
            }

            if (NewPasswordBox.Password != ConfirmPasswordBox.Password)
            {
                MessageBox.Show("Новый пароль и его подтверждение не совпадают!");
                return;
            }

            if (NewPasswordBox.Password.Length < 6)
            {
                MessageBox.Show("Новый пароль должен содержать не менее 6 символов!");
                return;
            }

            using (var db = new Entities())
            {
                string hashedPass = PasswordHelper.GetHash(CurrentPasswordBox.Password);
                var user = db.Users.FirstOrDefault(u => u.Login == TbLogin.Text && u.Password == hashedPass);

                if (user == null)
                {
                    MessageBox.Show("Текущий пароль/Логин неверный!");
                    return;
                }

                user.Password = PasswordHelper.GetHash(NewPasswordBox.Password);
                db.SaveChanges();

                MessageBox.Show("Пароль успешно изменён!");
                NavigationService?.Navigate(new AuthPage());
            }
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AuthPage());
        }
    }
}

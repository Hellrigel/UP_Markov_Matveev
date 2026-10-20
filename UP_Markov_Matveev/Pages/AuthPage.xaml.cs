using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace UP_Markov_Matveev.Pages
{
    
    public partial class AuthPage : Page
    {
        private int failedAttempts = 0;

        public AuthPage()
        {
            InitializeComponent();
        }

        private void ButtonEnter_OnClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TextBoxLogin.Text) || string.IsNullOrEmpty(PasswordBox.Password))
            {
                MessageBox.Show("Введите логин или пароль");
                return;
            }

            string hashedPassword = PasswordHelper.GetHash(PasswordBox.Password);

            using (var db = new Entities())
            {
                var user = db.Users
                    .FirstOrDefault(u => u.Login == TextBoxLogin.Text && u.Password == hashedPassword);

                if (user == null)
                {
                    MessageBox.Show("Пользователь с такими данными не найден!");
                    failedAttempts++;
                    if (failedAttempts >= 3)
                    {
                        CaptchaPanel.Visibility = Visibility.Visible;
                        CaptchaChange();
                    }
                    return;
                }

                MessageBox.Show($"Добро пожаловать, {user.FIO}!");
                Entities.ResetContext();

                switch (user.Role)
                {
                    case "User":
                        NavigationService?.Navigate(new UserPage(user));
                        break;
                    case "Admin":
                        NavigationService?.Navigate(new AdminPage(user));
                        break;
                }
            }
        }

        private void ButtonReg_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new RegPage());
        }

        private void ButtonChangePassword_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new ChangePassPage());
        }

        

        private void CaptchaChange()
        {
            string allowchar = "A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z,";
            allowchar += "a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,y,z,";
            allowchar += "1,2,3,4,5,6,7,8,9,0";
            string[] ar = allowchar.Split(',');
            string pwd = "";
            var r = new Random();

            for (int i = 0; i < 6; i++)
                pwd += ar[r.Next(0, ar.Length)];

            captcha.Text = pwd;
            captchaInput.Text = "";
        }

        private void submitCaptcha_Click(object sender, RoutedEventArgs e)
        {
            if (captchaInput.Text != captcha.Text)
            {
                MessageBox.Show("Неверно введена капча", "Ошибка");
                CaptchaChange();
            }
            else
            {
                MessageBox.Show("Капча введена успешно, можете продолжить авторизацию", "Успех");
                CaptchaPanel.Visibility = Visibility.Collapsed;
                failedAttempts = 0;
                TextBoxLogin.Clear();
                PasswordBox.Clear();
            }
        }

        private void textBox_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            
            if (e.Command == ApplicationCommands.Copy ||
                e.Command == ApplicationCommands.Cut ||
                e.Command == ApplicationCommands.Paste)
            {
                e.Handled = true;
            }
        }

        private void TextBoxLogin_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
        }
    }
}

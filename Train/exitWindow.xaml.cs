using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Train
{
    /// <summary>
    /// Логика взаимодействия для exitWindow.xaml
    /// </summary>
    public partial class exitWindow : Window
    {
        string password;
        public exitWindow(string password)
        {
            InitializeComponent();
            this.password = password;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (password == txt_password.Text)
            {
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                label_password.Text="Неверный пароль!";
            }
        }

        private void key_Click(object sender, RoutedEventArgs e)
        {
            if (sender == key_0) txt_password.Text += "0";
            if (sender == key_1) txt_password.Text += "1";
            if (sender == key_2) txt_password.Text += "2";
            if (sender == key_3) txt_password.Text += "3";
            if (sender == key_4) txt_password.Text += "4";
            if (sender == key_5) txt_password.Text += "5";
            if (sender == key_6) txt_password.Text += "6";
            if (sender == key_7) txt_password.Text += "7";
            if (sender == key_8) txt_password.Text += "8";
            if (sender == key_9) txt_password.Text += "9";
            if (sender == key_delete && txt_password.Text.Length > 0) txt_password.Text = txt_password.Text.Remove(txt_password.Text.Length - 1);
            if (sender == key_clear) txt_password.Text = "";

        }

    }
}

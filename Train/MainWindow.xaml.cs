
using System;
using System.Windows;
using System.Windows.Media.Animation;
using ExcelDataReader;
using System.IO;
using System.Data;
using System.Collections.Generic;
using System.Reflection;
using CefSharp;
using CefSharp.Wpf;
using System.Windows.Controls;
using Train.Model;

namespace Train
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        TrainWorker worker;
        String password = null, dollar = null, yevro = null, rubl = null;
        public MainWindow()
        {
            InitializeComponent();
            setValues();
            web_browser.Address = Environment.CurrentDirectory + "\\Pages\\о станции.html";
            web_browser.FrameLoadEnd += MyBrowserOnFrameLoadEnd;

            frame.Width = frame.Width - 20;
            frame_raschot.Width = frame_raschot.Width - 20;

        }

        private void MyBrowserOnFrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            ChromiumWebBrowser browser = (ChromiumWebBrowser)sender;
            Dispatcher.Invoke(() =>
            {
                browser.SetZoomLevel((Convert.ToDouble(browser.Tag) - 30) / 25.0);
            });
        }

        private void setValues()
        {
            worker = new TrainWorker();

            cmb_export.Items.Add("Внутригосударственный");
            cmb_export.Items.Add("На экспорт");
            cmb_send_staions.Items.Add("Назарбек");
            cmb_export.SelectedIndex = 0;
            cmb_send_staions.SelectedIndex = 0;

            password = worker.mainTable.Rows[5].ItemArray[5].ToString();
            dollar = worker.mainTable.Rows[6].ItemArray[5].ToString();
            yevro = worker.mainTable.Rows[7].ItemArray[5].ToString();
            rubl = worker.mainTable.Rows[8].ItemArray[5].ToString();

            txt_dollar.Text = "1 USD = " + dollar + " UZS";
            txt_yevro.Text = "1 EUR = " + yevro + " UZS";
            txt_rubl.Text = "1 RUB = " + rubl + " UZS";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Storyboard storyboard = new Storyboard();
            DoubleAnimation frameUp = new DoubleAnimation(1200, 15, new Duration(new TimeSpan(0, 0, 1)));
            Storyboard.SetTargetProperty(frameUp, new PropertyPath("(Canvas.Top)"));
            storyboard.Children.Add(frameUp);

            web_browser.Visibility = Visibility.Hidden;
            if (!sender.Equals(btn_main3))
            {

                web_browser.Visibility = Visibility.Visible;
                if (sender.Equals(btn_main1))
                {
                    web_browser.Load(Environment.CurrentDirectory + "\\Pages\\о станции.html");
                    Dispatcher.Invoke(() =>
                    {
                        web_browser.SetZoomLevel((Convert.ToDouble(web_browser.Tag) - 30) / 25.0);
                    });
                }
                if (sender.Equals(btn_main2))
                {
                    web_browser.Load(Environment.CurrentDirectory + "\\Pages\\П-пути.html");
                }
                if (sender.Equals(btn_main4))
                {
                    web_browser.Load(Environment.CurrentDirectory + "\\Pages\\Документы.html");
                }
                if (sender.Equals(btn_main5))
                {
                    web_browser.Load(Environment.CurrentDirectory + "\\Pages\\sxema.html");
                }
                if (sender.Equals(btn_main6))
                {
                    web_browser.Load(Environment.CurrentDirectory + "\\Pages\\Типы жд вагонов - Cargo Star LLC.html");
                }
                if (sender.Equals(btn_main7))
                {
                    web_browser.Load(Environment.CurrentDirectory + "\\Pages\\Перечень транспортно-экспедиторских организаций.html");
                }
                if (sender.Equals(btn_main8))
                {
                    web_browser.Load(Environment.CurrentDirectory + "\\Pages\\АО «Узбекистон темир йуллари» — Льготы грузоотправителям.html");
                }
                frame.BeginStoryboard(storyboard);
            }
            else
            {
                cmb_send_staions.SelectedIndex = 0;
                cmb_get_staions.SelectedIndex = -1;
                cmb_type_cargo.SelectedIndex = -1;
                txt_weight.Text = "";

                txtblock_last_price.Text = "Стоимость тарифа:  0 сум";

                frame_raschot.BeginStoryboard(storyboard);
            }

        }

        private void btn_colculation_Click(object sender, RoutedEventArgs e)
        {
            if (cmb_send_staions.SelectedItem == null ||
                cmb_get_staions.SelectedItem == null ||
                cmb_type_cargo.SelectedItem == null ||
                cmb_owner.SelectedItem == null ||
                txt_weight.Text == "" ||
                !Int32.TryParse(txt_weight.Text, out int t))
            {
                MessageBox.Show("Пожалуйста, заполните необходимые разделы и проверьте правильность!");
            }
            else
            {
                int weight = Int32.Parse(txt_weight.Text);
                if (weight < 10)
                {
                    MessageBox.Show("Минимальная масса груза 10 тонн!");
                    return;
                }
                else if (weight > 80)
                {
                    MessageBox.Show("Максимальная масса груза 80 тонн!");
                    return;
                }

                string selected_station = cmb_get_staions.SelectedItem.ToString().Remove(cmb_get_staions.SelectedItem.ToString().IndexOf('.'));
                if(cmb_get_staions.SelectedItem.ToString().Contains("через"))
                    selected_station = selected_station.Remove(0,cmb_get_staions.SelectedItem.ToString().IndexOf("через ")+6);

                string selected_type_cargo = cmb_type_cargo.SelectedItem.ToString();
                bool inventory = cmb_owner.SelectedIndex == 0;

                double last_price = worker.Calculate(selected_station, selected_type_cargo, inventory, weight);

                txtblock_last_price.Text = "Стоимость тарифа:  " + String.Format("{0:#,###}", Math.Round(last_price)) + " сум";
            }
        }


        private void web_browser_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (e.Delta > 0 && web_browser.ZoomLevel <= 1000)
            {
                web_browser.ZoomInCommand.Execute(null);
            }
            else if (e.Delta < 0 && web_browser.ZoomLevel >= 10)
            {
                web_browser.ZoomOutCommand.Execute(null);
            }
        }

        private void cmb_export_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmb_export.SelectedIndex == 0)
            {
                List<string> list_stations = new List<string>();

                for (int i = 0; i < worker.whiteStations.Rows.Count; i++)
                {
                    list_stations.Add(worker.whiteStations.Rows[i].ItemArray[0].ToString() + ". (" + worker.whiteStations.Rows[i].ItemArray[1].ToString() + " км)");
                }
                for (int i = 0; i < worker.yellowStations.Rows.Count; i++)
                {
                    list_stations.Add(worker.yellowStations.Rows[i].ItemArray[0].ToString() + ". (" + worker.yellowStations.Rows[i].ItemArray[1].ToString() + " км)");
                }
                for (int i = 0; i < worker.greenStations.Rows.Count; i++)
                {
                    list_stations.Add(worker.greenStations.Rows[i].ItemArray[0].ToString() + ". (" + worker.greenStations.Rows[i].ItemArray[1].ToString() + " км)");
                }

                list_stations.Sort();
                cmb_get_staions.ItemsSource = list_stations;
                cmb_type_cargo.Items.Clear();
                foreach (var item in worker.carriageTypes)
                {
                    cmb_type_cargo.Items.Add(item);
                }
            }
            else
            {
                List<string> list_stations = new List<string>();

                for (int i = 0; i < worker.exportStations.Rows.Count; i++)
                {
                    list_stations.Add(worker.exportStations.Rows[i].ItemArray[2].ToString() + " через " + worker.exportStations.Rows[i].ItemArray[0].ToString() + ". (" + worker.exportStations.Rows[i].ItemArray[1].ToString() + " км)");
                }

                cmb_get_staions.ItemsSource = list_stations;
                cmb_type_cargo.Items.Clear();
                for (int i = 0; i<3; i++)
                {
                    cmb_type_cargo.Items.Add(worker.carriageTypes[i]);
                }
            }
        }

        private void cmb_get_staions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cmb_type_cargo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmb_owner.Items.Clear();
                cmb_owner.Items.Add("Инвентарный");
            if (!cmb_type_cargo.SelectedItem?.ToString().Equals("Вагон-термос")??true)
            {
            cmb_owner.Items.Add("Собственный/арендованный");
            }
        }

        private void txt_weight_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        private void txt_weight_LostFocus(object sender, RoutedEventArgs e)
        {

        }

        private void key_Click(object sender, RoutedEventArgs e)
        {
            if (sender == key_0) txt_weight.Text += "0";
            if (sender == key_1) txt_weight.Text += "1";
            if (sender == key_2) txt_weight.Text += "2";
            if (sender == key_3) txt_weight.Text += "3";
            if (sender == key_4) txt_weight.Text += "4";
            if (sender == key_5) txt_weight.Text += "5";
            if (sender == key_6) txt_weight.Text += "6";
            if (sender == key_7) txt_weight.Text += "7";
            if (sender == key_8) txt_weight.Text += "8";
            if (sender == key_9) txt_weight.Text += "9";
            if (sender == key_delete && txt_weight.Text.Length > 0) txt_weight.Text = txt_weight.Text.Remove(txt_weight.Text.Length - 1);
            if (sender == key_clear) txt_weight.Text = "";

        }
        private void Image_TouchDown(object sender, System.Windows.Input.TouchEventArgs e)
        {
            exitWindow exitWindow = new exitWindow(password);
            exitWindow.ShowDialog();
            if (exitWindow.DialogResult.Value)
            {
                this.Close();
            }
        }
        private void btn_exit_frame1_Click(object sender, RoutedEventArgs e)
        {
            Storyboard storyboard = new Storyboard();
            DoubleAnimation frameUp = new DoubleAnimation(15, 1200, new Duration(new TimeSpan(0, 0, 1)));
            Storyboard.SetTargetProperty(frameUp, new PropertyPath("(Canvas.Top)"));
            storyboard.Children.Add(frameUp);
            if (Canvas.GetTop(frame_raschot) < 1000)
                frame_raschot.BeginStoryboard(storyboard);
            else
                frame.BeginStoryboard(storyboard);
        }
    }
}

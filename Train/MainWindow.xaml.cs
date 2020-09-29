
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

namespace Train
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
       
        DataTable white_stations = new DataTable(), yellow_stations=new DataTable(), green_stations=new DataTable();
        DataTable prices_table = new DataTable();

        String password = null, dollar = null, yevro = null, rubl = null,
            nds = null,
            coefficient1=null, coefficient2 = null, coefficient3 = null;
        public MainWindow()
        {
            InitializeComponent();
            setValues();
            web_browser.Address = Environment.CurrentDirectory + "\\Pages\\о станции.html";
            web_browser.FrameLoadEnd += MyBrowserOnFrameLoadEnd;

            frame.Width = frame.MaxWidth - 20;
            frame_raschot.Width = frame_raschot.MaxWidth - 20;

            DoubleAnimation frameHeight = new DoubleAnimation();
            frameHeight.From = frame.ActualHeight;
            frameHeight.To = frame.MaxHeight;
            frameHeight.Duration = TimeSpan.FromSeconds(0.75);
            frame.BeginAnimation(DockPanel.HeightProperty, frameHeight);

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
            using (var stream = File.Open(Environment.CurrentDirectory + "\\Files\\Список станций.xlsx", FileMode.Open, FileAccess.Read))
            {
                IExcelDataReader reader;
                reader = ExcelReaderFactory.CreateOpenXmlReader(stream);

                var dataSet = reader.AsDataSet();

                white_stations = dataSet.Tables[0];
                yellow_stations = dataSet.Tables[1];
                green_stations = dataSet.Tables[2];
            }
            using (var stream = File.Open(Environment.CurrentDirectory + "\\Files\\Стоимость.xlsx", FileMode.Open, FileAccess.Read))
            {
                IExcelDataReader reader;
                reader = ExcelReaderFactory.CreateOpenXmlReader(stream);

                var dataSet = reader.AsDataSet();

                prices_table = dataSet.Tables[0];
            }

            cmb_send_staions.Items.Add("Назарбек");
            cmb_type_cargo.Items.Add("Универсальные вагоны МПС");

            List<string> list_stations = new List<string>();
            
            for (int i = 0; i < white_stations.Rows.Count; i++)
            {
                list_stations.Add(white_stations.Rows[i].ItemArray[0].ToString()+". ("+ white_stations.Rows[i].ItemArray[1].ToString()+" км)");
            }
            for (int i = 0; i < yellow_stations.Rows.Count; i++)
            {
                list_stations.Add(yellow_stations.Rows[i].ItemArray[0].ToString() + ". (" + yellow_stations.Rows[i].ItemArray[1].ToString() + " км)");
            }
            for (int i = 0; i < green_stations.Rows.Count; i++)
            {
                list_stations.Add(green_stations.Rows[i].ItemArray[0].ToString() + ". (" + green_stations.Rows[i].ItemArray[1].ToString() + " км)");
            }

            list_stations.Sort();

            foreach(var station in list_stations)
            {
                cmb_get_staions.Items.Add(station);
            }

            cmb_send_staions.SelectedIndex = 0;

            nds= prices_table.Rows[74].ItemArray[1].ToString();
            coefficient1 = prices_table.Rows[75].ItemArray[1].ToString();
            coefficient2 = prices_table.Rows[76].ItemArray[1].ToString();
            coefficient3 = prices_table.Rows[77].ItemArray[1].ToString();
            password = prices_table.Rows[78].ItemArray[1].ToString();
            dollar = prices_table.Rows[79].ItemArray[1].ToString();
            yevro = prices_table.Rows[80].ItemArray[1].ToString();
            rubl = prices_table.Rows[81].ItemArray[1].ToString();

            txt_dollar.Text = "1 USD = " + dollar + " UZS";
            txt_yevro.Text = "1 EUR = " + yevro + " UZS";
            txt_rubl.Text = "1 RUB = " + rubl + " UZS";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
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

            if (!sender.Equals(btn_main3))
            {            

                DoubleAnimation frameHeight = new DoubleAnimation();
                frameHeight.From = frame.ActualHeight;
                frameHeight.To = frame.MaxHeight;
                frameHeight.Duration = TimeSpan.FromSeconds(0.75);
                frame.BeginAnimation(DockPanel.HeightProperty, frameHeight);


            }
            else
            {
                cmb_send_staions.SelectedIndex = 0;
                cmb_get_staions.SelectedIndex = -1;
                cmb_type_cargo.SelectedIndex = -1;
                txt_weight.Text = "";

                txtblock_last_price.Text = "Итоговая стоимость:  0 сум";
                
                DoubleAnimation frameHeight = new DoubleAnimation();
                frameHeight.From = frame_raschot.ActualHeight;
                frameHeight.To = frame_raschot.MaxHeight;
                frameHeight.Duration = TimeSpan.FromSeconds(0.75);
                frame_raschot.BeginAnimation(DockPanel.HeightProperty, frameHeight);


            }
        }

        private void btn_colculation_Click(object sender, RoutedEventArgs e)
        {
            if(cmb_send_staions.SelectedItem==null ||
                cmb_get_staions.SelectedItem==null ||
                cmb_type_cargo.SelectedItem == null ||
                txt_weight.Text=="" ||
                !Int32.TryParse(txt_weight.Text, out int t))
            {
                MessageBox.Show("Пожалуйста, заполните необходимые разделы и проверьте правильность!");
            }
            else
            {
                int weight = Int32.Parse(txt_weight.Text);
                if (weight < 10)
                {
                    MessageBox.Show("Минимальное количество груза 10 тонн!");
                    return;
                }

                string selected_station = cmb_get_staions.SelectedItem.ToString().Remove(cmb_get_staions.SelectedItem.ToString().IndexOf('.'));
                int way_length = 0;
                double coefficient = 1;

                for(int i = 0; i < white_stations.Rows.Count; i++)
                {
                    if (white_stations.Rows[i].ItemArray[0].ToString().Equals(selected_station))
                    {
                        coefficient = Convert.ToDouble(coefficient1);
                        way_length = Int32.Parse(white_stations.Rows[i].ItemArray[1].ToString());
                        break;
                    }
                }
                for (int i = 0; i < yellow_stations.Rows.Count; i++)
                {
                    if (yellow_stations.Rows[i].ItemArray[0].ToString().Equals(selected_station))
                    {
                        coefficient = Convert.ToDouble(coefficient2);
                        way_length = Int32.Parse(yellow_stations.Rows[i].ItemArray[1].ToString());
                        break;
                    }
                }
                for (int i = 0; i < green_stations.Rows.Count; i++)
                {
                    if (green_stations.Rows[i].ItemArray[0].ToString().Equals(selected_station))
                    {
                        coefficient = Convert.ToDouble(coefficient3);
                        way_length = Int32.Parse(green_stations.Rows[i].ItemArray[1].ToString());
                        break;
                    }
                }

                int index_i = find_i(weight);
                int index_j = find_j(way_length);

                long price = 0;

                string s = prices_table.Rows[index_i].ItemArray[index_j].ToString().Replace(" ", "");

                if (s.Contains(" "))
                {
                    s.Replace(" ", "");
                }

                if (index_i == 73)
                {
                    price = weight * Int32.Parse(s);
                }
                else
                {
                    price = Int64.Parse(s);
                }

                double last_price = price * coefficient * (Convert.ToDouble(nds)/100+1);

                txtblock_last_price.Text = "Итоговая стоимость:  " + String.Format("{0:#,###}",Math.Round(last_price)) + " сум";
            }
        }

        private int find_j(int way_length)
        {
            if (way_length <= 50)
            {
                return way_length / 5 + 1+1;
            }
            else
            {
                if (way_length <= 100)
                {
                    return (way_length - 50) / 10 + 10+1;
                }
                else
                {
                    if (way_length <= 300)
                    {
                        return (way_length - 100) / 20 + 15+1;
                    }
                    else
                    {
                        if (way_length <= 600)
                        {
                            return (way_length - 300) / 30 + 25+1;
                        }
                        else
                        {
                            if (way_length <= 1000)
                            {
                                return (way_length - 600) / 40 + 35+1;
                            }
                            else
                            {
                                if (way_length <= 1500)
                                {
                                    return (way_length - 1000) / 50 + 45+1;
                                }
                                else
                                {
                                    return (way_length - 1500) / 100 + 55+1;
                                }
                            }
                        }
                    }
                }
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


        private int find_i(int weight)
        {
             if (weight <= 80)
            {
                return (weight - 9);
            }
            else
            {
                return 73; // При весе свыше 80 т	
            }
        }

        private void btn_keyboard_Click(object sender, RoutedEventArgs e)
        {
            if (row_keyboard.Height.Value != 0)
            {
                row_keyboard.Height = new GridLength(0);
            }
            else
            {
                row_keyboard.Height = new GridLength(200);
            }
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
            if (sender == key_delete && txt_weight.Text.Length>0) txt_weight.Text = txt_weight.Text.Remove(txt_weight.Text.Length - 1);
            if (sender == key_clear) txt_weight.Text=""; 
            
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
            DoubleAnimation frameHeight = new DoubleAnimation();
            frameHeight.From = frame.ActualHeight;
            frameHeight.To = 0;
            frameHeight.Duration = TimeSpan.FromSeconds(0.75);
            frame.BeginAnimation(DockPanel.HeightProperty, frameHeight);

            DoubleAnimation frameHeight_raschot = new DoubleAnimation();
            frameHeight_raschot.From = frame_raschot.ActualHeight;
            frameHeight_raschot.To = 0;
            frameHeight_raschot.Duration = TimeSpan.FromSeconds(0.75);
            frame_raschot.BeginAnimation(DockPanel.HeightProperty, frameHeight_raschot);
        }


    }
    
}

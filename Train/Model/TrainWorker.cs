using ExcelDataReader;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Train.Model
{
    public class TrainWorker
    {
        public DataTable whiteStations,
            yellowStations,
            greenStations,
            mainTable;
        DataSet mainDataSet;
        DataTable[] prices = new DataTable[28];
        public List<string> carriageTypes;
        string nds = null,  coefficient1 = null, coefficient2 = null, coefficient3 = null;
        public TrainWorker()
        {
            using (var stream = File.Open(Environment.CurrentDirectory + "\\Files\\Список станций.xlsx", FileMode.Open, FileAccess.Read))
            {
                IExcelDataReader reader;
                reader = ExcelReaderFactory.CreateOpenXmlReader(stream);

                var dataSet = reader.AsDataSet();

                whiteStations = new DataTable();
                whiteStations = dataSet.Tables[0];
                yellowStations = new DataTable();
                yellowStations = dataSet.Tables[1];
                greenStations = new DataTable();
                greenStations = dataSet.Tables[2];
            }
            using (var stream = File.Open(Environment.CurrentDirectory + "\\Files\\Стоимость.xlsx", FileMode.Open, FileAccess.Read))
            {
                IExcelDataReader reader;
                reader = ExcelReaderFactory.CreateOpenXmlReader(stream);

                var dataSet = reader.AsDataSet();

                mainTable = new DataTable();
                mainTable = dataSet.Tables[0];
                mainDataSet = dataSet;
            }
            carriageTypes = new List<string>();
            foreach (DataRow item in mainTable.Rows)
            {
                carriageTypes.Add(item.ItemArray[0].ToString());
            }
            carriageTypes.RemoveAt(0);
            nds = mainTable.Rows[0].ItemArray[5].ToString();
            coefficient1 = mainTable.Rows[1].ItemArray[5].ToString();
            coefficient2 = mainTable.Rows[2].ItemArray[5].ToString();
            coefficient3 = mainTable.Rows[3].ItemArray[5].ToString();
        }


        internal double Calculate(string selected_station, string selected_type_cargo, bool inventory, int weight)
        {
            int way_length = 0;
            double coefficient = 1;

            for (int i = 0; i < whiteStations.Rows.Count; i++)
            {
                if (whiteStations.Rows[i].ItemArray[0].ToString().Equals(selected_station))
                {
                    coefficient = Convert.ToDouble(coefficient1);
                    way_length = Int32.Parse(whiteStations.Rows[i].ItemArray[1].ToString());
                    break;
                }
            }
            for (int i = 0; i < yellowStations.Rows.Count; i++)
            {
                if (yellowStations.Rows[i].ItemArray[0].ToString().Equals(selected_station))
                {
                    coefficient = Convert.ToDouble(coefficient2);
                    way_length = Int32.Parse(yellowStations.Rows[i].ItemArray[1].ToString());
                    break;
                }
            }
            for (int i = 0; i < greenStations.Rows.Count; i++)
            {
                if (greenStations.Rows[i].ItemArray[0].ToString().Equals(selected_station))
                {
                    coefficient = Convert.ToDouble(coefficient3);
                    way_length = Int32.Parse(greenStations.Rows[i].ItemArray[1].ToString());
                    break;
                }
            }

            switch (selected_type_cargo)
            {
                case "Крытый вагон":
                case "Полувагон":
                case "Платформа":
                    
                    break;
                case "Зерновоз":
                    break;
                case "Хоппер-цементовоз":
                    break;
                case "Цистерна-цементовоз":
                case "Хоппер-дозатор":
                case "Минераловоз":
                case "Думпкары":
                case "Бункерные полувагоны для битума":
                    break;
                case "Вагон-термос":
                    break;
                case "Цистерна (Нефтепродукты)":
                    break;
                case "Цистерна (Газы сжиженные)":
                    break;
                case "Цистерна (Спирты и фенолы)":
                    break;
                case "Цистерна (Скоропортящиеся грузы)":
                    break;
                case "Цистерна (Остальные наливные грузы)":
                    break;
            }
            int index_i = find_i(weight);
            int index_j = find_j(way_length);

            long price = 0;

            string s = mainTable.Rows[index_i].ItemArray[index_j].ToString().Replace(" ", "");

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

           return price * coefficient * (Convert.ToDouble(nds) / 100 + 1);
        }

        private int find_j(int way_length, int type = 1)
        {
            int distance = type == 1 ? 11 : 2;
            if (way_length <= 50)
            {
                return type == 1 ? way_length / 5 + 1 : distance;
            }
            else
            {
                if (way_length <= 100)
                {
                    return (way_length - 50) / 10 + distance;
                }
                else
                {
                    if (way_length <= 300)
                    {
                        return (way_length - 100) / 20 + 5 + distance;
                    }
                    else
                    {
                        if (way_length <= 600)
                        {
                            return (way_length - 300) / 30 + 15 + distance;
                        }
                        else
                        {
                            if (way_length <= 1000)
                            {
                                return (way_length - 600) / 40 + 25 + distance;
                            }
                            else
                            {
                                if (way_length <= 1500)
                                {
                                    return (way_length - 1000) / 50 + 35 + distance;
                                }
                                else
                                {
                                    return (way_length - 1500) / 100 + 45 + distance;
                                }
                            }
                        }
                    }
                }
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
    }
}

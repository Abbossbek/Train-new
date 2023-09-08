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
            exportStations,
            mainTable;
        DataSet mainDataSet;
        DataTable[] prices = new DataTable[28];
        public List<string> carriageTypes;
        string nds = null, coefficient1 = null, coefficient2 = null, coefficient3 = null, coefficient4 = null;
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
                exportStations = new DataTable();
                exportStations = dataSet.Tables[3];
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
            coefficient4 = mainTable.Rows[4].ItemArray[5].ToString();
        }


        internal double Calculate(string selected_station, string selected_type_cargo, bool inventory, int weight)
        {
            int way_length = 0;
            double coefficient = 1;
            int indexX = 0, indexY = 0;
            bool isExport = false;

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
            for (int i = 0; i < exportStations.Rows.Count; i++)
            {
                if (exportStations.Rows[i].ItemArray[0].ToString().Equals(selected_station))
                {
                    coefficient = Convert.ToDouble(coefficient4);
                    way_length = Int32.Parse(exportStations.Rows[i].ItemArray[1].ToString());
                    isExport = true;
                    break;
                }
            }

            int tableIndex = 0;
            long sum = 0;

            switch (selected_type_cargo)
            {
                case "Крытый вагон":
                case "Полувагон":
                case "Платформа":
                    tableIndex = inventory ? 1 : 2;
                    indexX = FindX(way_length);
                    indexY = weight - 9;
                    sum = Convert.ToInt64(mainDataSet.Tables[tableIndex].Rows[indexY].ItemArray[indexX].ToString().Replace(" ", ""));
                    break;
                case "Зерновоз":
                    tableIndex = 3;
                    indexX = FindX(way_length, 2);
                    if (weight < 70)
                    {
                        indexY = inventory ? 8 : 24;
                        sum = Convert.ToInt64(mainDataSet.Tables[tableIndex].Rows[indexY].ItemArray[indexX].ToString().Replace(" ", ""));
                    }
                    else
                    {
                        indexY = inventory ? 9 : 25;
                        sum = Convert.ToInt64(mainDataSet.Tables[tableIndex].Rows[indexY - 1].ItemArray[indexX].ToString().Replace(" ", "")) +
                            (weight - 70) * Convert.ToInt64(mainDataSet.Tables[tableIndex].Rows[indexY].ItemArray[indexX].ToString().Replace(" ", ""));
                    }
                    break;
                case "Хоппер-цементовоз":
                    tableIndex = 3;
                    indexX = FindX(way_length, 2);
                    if (weight < 70)
                    {
                        indexY = inventory ? 8 : 19;
                        sum = Convert.ToInt64(mainDataSet.Tables[tableIndex].Rows[indexY].ItemArray[indexX].ToString().Replace(" ", ""));
                    }
                    else
                    {
                        indexY = inventory ? 9 : 20;
                        sum = Convert.ToInt64(mainDataSet.Tables[tableIndex].Rows[indexY - 1].ItemArray[indexX].ToString().Replace(" ", "")) +
                            (weight - 70) * Convert.ToInt64(mainDataSet.Tables[tableIndex].Rows[indexY].ItemArray[indexX].ToString().Replace(" ", ""));
                    }
                    break;
                case "Цистерна-цементовоз":
                case "Хоппер-дозатор":
                case "Минераловоз":
                case "Думпкар":
                case "Бункерный полувагон для битума":
                    tableIndex = 3;
                    indexX = FindX(way_length, 2);
                    if (weight < 70)
                    {
                        indexY = inventory ? 11 : 24;
                        sum = Convert.ToInt64(mainDataSet.Tables[tableIndex].Rows[indexY].ItemArray[indexX].ToString().Replace(" ", ""));
                    }
                    else
                    {
                        indexY = inventory ? 12 : 25;
                        sum = Convert.ToInt64(mainDataSet.Tables[tableIndex].Rows[indexY - 1].ItemArray[indexX].ToString().Replace(" ", "")) +
                            (weight - 70) * Convert.ToInt64(mainDataSet.Tables[tableIndex].Rows[indexY].ItemArray[indexX].ToString().Replace(" ", ""));
                    }
                    break;
                case "Вагон-термос":
                    tableIndex = 3;
                    indexX = FindX(way_length, 2);
                    indexY = 38;
                    sum = weight < 70
                        ? Convert.ToInt64(mainDataSet.Tables[tableIndex].Rows[indexY].ItemArray[indexX].ToString().Replace(" ", ""))
                        : Convert.ToInt64(mainDataSet.Tables[tableIndex].Rows[indexY].ItemArray[indexX].ToString().Replace(" ", "")) +
                            (weight - 70) * Convert.ToInt64(mainDataSet.Tables[tableIndex].Rows[indexY + 1].ItemArray[indexX].ToString().Replace(" ", ""));
                    break;
                case "Цистерна (Нефтепродукты)":
                    tableIndex = 6;
                    indexX = FindX(way_length);
                    indexY = inventory ? 4 : 5;
                    sum = weight * Convert.ToInt64(mainDataSet.Tables[tableIndex].Rows[indexY].ItemArray[indexX].ToString().Replace(" ", ""));
                    break;
                case "Цистерна (Газы сжиженные)":
                    tableIndex = 6;
                    indexX = FindX(way_length);
                    indexY = inventory ? 6 : 7;
                    sum = weight * Convert.ToInt64(mainDataSet.Tables[tableIndex].Rows[indexY].ItemArray[indexX].ToString().Replace(" ", ""));
                    break;
                case "Цистерна (Спирты и фенолы)":
                    tableIndex = 6;
                    indexX = FindX(way_length);
                    indexY = inventory ? 8 : 9;
                    sum = weight * Convert.ToInt64(mainDataSet.Tables[tableIndex].Rows[indexY].ItemArray[indexX].ToString().Replace(" ", ""));
                    break;
                case "Цистерна (Скоропортящиеся грузы)":
                    tableIndex = 6;
                    indexX = FindX(way_length);
                    indexY = inventory ? 10 : 11;
                    sum = weight * Convert.ToInt64(mainDataSet.Tables[tableIndex].Rows[indexY].ItemArray[indexX].ToString().Replace(" ", ""));
                    break;
                case "Цистерна (Остальные наливные грузы)":
                    tableIndex = 6;
                    indexX = FindX(way_length);
                    indexY = inventory ? 12 : 13;
                    sum = weight * Convert.ToInt64(mainDataSet.Tables[tableIndex].Rows[indexY].ItemArray[indexX].ToString().Replace(" ", ""));
                    break;
            }

            return sum * coefficient * (isExport ? 1 : (Convert.ToDouble(nds) / 100 + 1));
        }

        private int FindX(int way_length, int type = 1)
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
    }
}

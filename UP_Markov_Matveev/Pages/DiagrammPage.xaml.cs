using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.DataVisualization.Charting;
using Word = Microsoft.Office.Interop.Word;
using Excel = Microsoft.Office.Interop.Excel;

namespace UP_Markov_Matveev.Pages
{
    
    public partial class DiagrammPage : Page
    {
        private readonly string _reportsFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "UP_Markov_Matveev");

        public DiagrammPage()
        {
            InitializeComponent();

            ChartOrders.ChartAreas.Clear();
            ChartOrders.ChartAreas.Add(new ChartArea("Main"));

            var currentSeries = new Series("Стоимость заказов") { IsValueShownAsLabel = true };
            ChartOrders.Series.Clear();
            ChartOrders.Series.Add(currentSeries);

            using (var db = new Entities())
            {
                var clients = db.Clients.ToList();
                clients.Insert(0, new Clients { ClientId = 0, Name = "Все клиенты" });
                CmbClient.ItemsSource = clients;
                CmbClient.SelectedIndex = 0;
            }

            CmbDiagram.ItemsSource = Enum.GetValues(typeof(SeriesChartType));
            CmbDiagram.SelectedItem = SeriesChartType.Column;
        }

        private void UpdateChart(object sender, SelectionChangedEventArgs e)
        {
            if (!(CmbClient.SelectedItem is Clients selectedClient) || !(CmbDiagram.SelectedItem is SeriesChartType chartType))
                return;

            Series currentSeries = ChartOrders.Series.FirstOrDefault();
            if (currentSeries == null)
                return;

            currentSeries.ChartType = chartType;
            currentSeries.Points.Clear();

            using (var db = new Entities())
            {
                var statuses = db.OrderStatuses.ToList();
                var orders = db.Orders.ToList();

                foreach (var status in statuses)
                {
                    var filtered = orders.Where(o => o.OrderStatusId == status.OrderStatusId);
                    if (selectedClient.ClientId != 0)
                        filtered = filtered.Where(o => o.ClientId == selectedClient.ClientId);

                    currentSeries.Points.AddXY(status.Name, filtered.Sum(o => (double)o.Cost));
                }
            }
        }

        

        private void ButtonExportWord_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Directory.CreateDirectory(_reportsFolder);

                List<Clients> allClients;
                List<Orders> allOrders;
                List<OrderStatuses> allStatuses;

                using (var db = new Entities())
                {
                    allClients = db.Clients.OrderBy(c => c.Name).ToList();
                    allOrders = db.Orders.ToList();
                    allStatuses = db.OrderStatuses.ToList();
                }

                var application = new Word.Application();
                Word.Document document = application.Documents.Add();

                foreach (var client in allClients)
                {
                    
                    Word.Paragraph clientParagraph = document.Paragraphs.Add();
                    Word.Range clientRange = clientParagraph.Range;
                    clientRange.Text = client.Name;
                    clientParagraph.set_Style("Заголовок 1");
                    clientRange.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                    clientRange.InsertParagraphAfter();
                    document.Paragraphs.Add();

                    var clientOrders = allOrders.Where(o => o.ClientId == client.ClientId).ToList();

                    
                    Word.Paragraph tableParagraph = document.Paragraphs.Add();
                    Word.Range tableRange = tableParagraph.Range;
                    Word.Table ordersTable = document.Tables.Add(tableRange, allStatuses.Count + 1, 2);
                    ordersTable.Borders.InsideLineStyle = ordersTable.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
                    ordersTable.Range.Cells.VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                    Word.Range cellRange;
                    cellRange = ordersTable.Cell(1, 1).Range;
                    cellRange.Text = "Статус заказа";
                    cellRange = ordersTable.Cell(1, 2).Range;
                    cellRange.Text = "Сумма, руб.";
                    ordersTable.Rows[1].Range.Font.Name = "Times New Roman";
                    ordersTable.Rows[1].Range.Font.Size = 14;
                    ordersTable.Rows[1].Range.Bold = 1;
                    ordersTable.Rows[1].Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;

                    for (int i = 0; i < allStatuses.Count; i++)
                    {
                        var status = allStatuses[i];
                        cellRange = ordersTable.Cell(i + 2, 1).Range;
                        cellRange.Text = status.Name;
                        cellRange.Font.Name = "Times New Roman";
                        cellRange.Font.Size = 12;

                        cellRange = ordersTable.Cell(i + 2, 2).Range;
                        decimal sum = clientOrders.Where(o => o.OrderStatusId == status.OrderStatusId).Sum(o => o.Cost);
                        cellRange.Text = sum.ToString("N2") + " руб.";
                        cellRange.Font.Name = "Times New Roman";
                        cellRange.Font.Size = 12;
                    }

                    document.Paragraphs.Add();

                    var maxOrder = clientOrders.OrderByDescending(o => o.Cost).FirstOrDefault();
                    if (maxOrder != null)
                    {
                        Word.Paragraph maxParagraph = document.Paragraphs.Add();
                        Word.Range maxRange = maxParagraph.Range;
                        maxRange.Text = $"Самый дорогостоящий заказ — {maxOrder.PickupAddress} → {maxOrder.DeliveryAddress} " +
                                        $"на сумму {maxOrder.Cost:N2} руб. от {maxOrder.OrderDate:dd.MM.yyyy}";
                        maxParagraph.set_Style("Подзаголовок");
                        maxRange.Font.Color = Word.WdColor.wdColorDarkRed;
                        maxRange.InsertParagraphAfter();
                    }
                    document.Paragraphs.Add();

                    var minOrder = clientOrders.OrderBy(o => o.Cost).FirstOrDefault();
                    if (minOrder != null)
                    {
                        Word.Paragraph minParagraph = document.Paragraphs.Add();
                        Word.Range minRange = minParagraph.Range;
                        minRange.Text = $"Самый недорогой заказ — {minOrder.PickupAddress} → {minOrder.DeliveryAddress} " +
                                        $"на сумму {minOrder.Cost:N2} руб. от {minOrder.OrderDate:dd.MM.yyyy}";
                        minParagraph.set_Style("Подзаголовок");
                        minRange.Font.Color = Word.WdColor.wdColorDarkGreen;
                        minRange.InsertParagraphAfter();
                    }

                    if (client != allClients.LastOrDefault())
                        document.Words.Last.InsertBreak(Word.WdBreakType.wdPageBreak);
                }

                foreach (Word.Section section in document.Sections)
                {
                    Word.HeaderFooter footer = section.Footers[Word.WdHeaderFooterIndex.wdHeaderFooterPrimary];
                    footer.PageNumbers.Add(Word.WdPageNumberAlignment.wdAlignPageNumberCenter);
                }

                application.Visible = true;

                string docxPath = Path.Combine(_reportsFolder, "Отчёт_по_заказам.docx");
                string pdfPath = Path.Combine(_reportsFolder, "Отчёт_по_заказам.pdf");
                document.SaveAs2(docxPath);
                document.SaveAs2(pdfPath, Word.WdExportFormat.wdExportFormatPDF);

                MessageBox.Show($"Отчёт сохранён:\n{docxPath}\n{pdfPath}");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось выгрузить отчёт в Word. Убедитесь, что на компьютере установлен Microsoft Word.\n\n" + ex.Message);
            }
        }


        private void ButtonExportExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Directory.CreateDirectory(_reportsFolder);

                List<Clients> allClients;
                using (var db = new Entities())
                {
                    allClients = db.Clients.OrderBy(c => c.Name).ToList();
                    foreach (var client in allClients)
                    {
                        db.Entry(client).Collection(c => c.Orders).Load();
                        foreach (var order in client.Orders)
                            db.Entry(order).Reference(o => o.OrderStatus).Load();
                    }
                }

                var application = new Excel.Application();
                application.SheetsInNewWorkbook = Math.Max(allClients.Count, 1);
                Excel.Workbook workbook = application.Workbooks.Add(Type.Missing);

                decimal grandTotal = 0;

                for (int i = 0; i < allClients.Count; i++)
                {
                    var client = allClients[i];
                    int startRowIndex = 1;

                    Excel.Worksheet worksheet = (Excel.Worksheet)application.Worksheets[i + 1];
                    worksheet.Name = SafeSheetName(client.Name, i);

                    worksheet.Cells[startRowIndex, 1] = "Дата заказа";
                    worksheet.Cells[startRowIndex, 2] = "Откуда";
                    worksheet.Cells[startRowIndex, 3] = "Куда";
                    worksheet.Cells[startRowIndex, 4] = "Стоимость";
                    Excel.Range headerRange = worksheet.Range[worksheet.Cells[1, 1], worksheet.Cells[1, 4]];
                    headerRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    headerRange.Font.Bold = true;
                    startRowIndex++;

                    var ordersByStatus = client.Orders
                        .OrderBy(o => o.OrderDate)
                        .GroupBy(o => o.OrderStatus)
                        .OrderBy(g => g.Key.Name);

                    foreach (var group in ordersByStatus)
                    {
                        Excel.Range groupHeader = worksheet.Range[worksheet.Cells[startRowIndex, 1], worksheet.Cells[startRowIndex, 4]];
                        groupHeader.Merge();
                        groupHeader.Value = group.Key.Name;
                        groupHeader.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                        groupHeader.Font.Italic = true;
                        startRowIndex++;

                        foreach (var order in group)
                        {
                            worksheet.Cells[startRowIndex, 1] = order.OrderDate.ToString("dd.MM.yyyy");
                            worksheet.Cells[startRowIndex, 2] = order.PickupAddress;
                            worksheet.Cells[startRowIndex, 3] = order.DeliveryAddress;
                            worksheet.Cells[startRowIndex, 4] = order.Cost;
                            ((Excel.Range)worksheet.Cells[startRowIndex, 4]).NumberFormat = "0.00";
                            startRowIndex++;
                        }

                        Excel.Range sumRange = worksheet.Range[worksheet.Cells[startRowIndex, 1], worksheet.Cells[startRowIndex, 3]];
                        sumRange.Merge();
                        sumRange.Value = "ИТОГО:";
                        sumRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;

                        worksheet.Cells[startRowIndex, 4].Formula = $"=SUM(D{startRowIndex - group.Count()}:D{startRowIndex - 1})";
                        sumRange.Font.Bold = ((Excel.Range)worksheet.Cells[startRowIndex, 4]).Font.Bold = true;
                        startRowIndex++;
                    }

                    grandTotal += client.Orders.Sum(o => o.Cost);

                    if (startRowIndex > 2)
                    {
                        Excel.Range borders = worksheet.Range[worksheet.Cells[1, 1], worksheet.Cells[startRowIndex - 1, 4]];
                        borders.Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle =
                            borders.Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle =
                            borders.Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle =
                            borders.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle =
                            borders.Borders[Excel.XlBordersIndex.xlInsideHorizontal].LineStyle =
                            borders.Borders[Excel.XlBordersIndex.xlInsideVertical].LineStyle = Excel.XlLineStyle.xlContinuous;
                    }

                    worksheet.Columns.AutoFit();
                }

                Excel.Worksheet summarySheet = workbook.Worksheets.Add(Type.Missing, workbook.Worksheets[workbook.Worksheets.Count]);
                summarySheet.Name = "Общий итог";
                summarySheet.Cells[1, 1] = "Общий итог по всем клиентам:";
                summarySheet.Cells[1, 2] = grandTotal;
                Excel.Range summaryRange = summarySheet.Range[summarySheet.Cells[1, 1], summarySheet.Cells[1, 2]];
                summaryRange.Font.Color = Excel.XlRgbColor.rgbRed;
                summaryRange.Font.Bold = true;
                summarySheet.Columns.AutoFit();

                application.Visible = true;

                string xlsxPath = Path.Combine(_reportsFolder, "Отчёт_по_заказам.xlsx");
                workbook.SaveAs(xlsxPath);

                MessageBox.Show($"Отчёт сохранён:\n{xlsxPath}");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось выгрузить отчёт в Excel. Убедитесь, что на компьютере установлен Microsoft Excel.\n\n" + ex.Message);
            }
        }

        private static string SafeSheetName(string name, int index)
        {
            foreach (char c in new[] { ':', '\\', '/', '?', '*', '[', ']' })
                name = name.Replace(c, '_');

            if (string.IsNullOrWhiteSpace(name))
                name = $"Клиент {index + 1}";

            return name.Length > 31 ? name.Substring(0, 31) : name;
        }

        private void ButtonBackToAdmin_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AdminPage());
        }
    }
}

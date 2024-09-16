using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.Util;
using NPOI.SS.UserModel.Charts;
using System;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        // Create a new workbook
        IWorkbook workbook = new XSSFWorkbook();

        // Create a sheet for the data
        ISheet sheet1 = workbook.CreateSheet("Sales Data");

        // Create the header row
        IRow headerRow = sheet1.CreateRow(0);
        headerRow.CreateCell(0).SetCellValue("Product");
        headerRow.CreateCell(1).SetCellValue("Quantity");
        headerRow.CreateCell(2).SetCellValue("Price");
        headerRow.CreateCell(3).SetCellValue("Total");

        // Sample data
        string[] products = { "Apples", "Bananas", "Oranges", "Grapes", "Strawberries" };
        int[] quantities = { 100, 150, 200, 120, 180 };
        double[] prices = { 1.2, 0.8, 1.5, 2.0, 2.5 };

        for (int i = 0; i < products.Length; i++)
        {
            IRow row = sheet1.CreateRow(i + 1);
            row.CreateCell(0).SetCellValue(products[i]);
            row.CreateCell(1).SetCellValue(quantities[i]);
            row.CreateCell(2).SetCellValue(prices[i]);
            row.CreateCell(3).SetCellValue(quantities[i] * prices[i]); // Total = Quantity * Price
        }

        // Auto-size columns
        for (int i = 0; i < 4; i++)
        {
            sheet1.AutoSizeColumn(i);
        }

        // Create a second sheet for the chart
        ISheet sheet2 = workbook.CreateSheet("Sales Chart");

        // Create a drawing canvas on the second sheet for the chart
        IDrawing drawing = sheet2.CreateDrawingPatriarch();
        IClientAnchor anchor = drawing.CreateAnchor(0, 0, 0, 0, 0, 0, 15, 25);

        // Create a chart
        IChart chart = drawing.CreateChart(anchor);
        IChartLegend legend = chart.GetOrCreateLegend();
        legend.Position = LegendPosition.TopRight;

        // Create chart data
        IChartDataFactory dataFactory = chart.ChartDataFactory;
        IChartAxisFactory axisFactory = chart.ChartAxisFactory;

        // Define chart data sources
        IChartDataSource<string> xs = DataSources.FromStringCellRange(sheet1, new CellRangeAddress(1, products.Length, 0, 0)); // Product Names
        IChartDataSource<double> ys = DataSources.FromNumericCellRange(sheet1, new CellRangeAddress(1, products.Length, 1, 1)); // Quantities

        // Create line chart data (simpler chart type)
        ILineChartData<string, double> lineChartData = dataFactory.CreateLineChartData<string, double>();
        ILineChartSeries<string, double> series = lineChartData.AddSeries(xs, ys);
        series.SetTitle("Product Quantities");

        // Setup chart axes
        IChartAxis bottomAxis = axisFactory.CreateCategoryAxis(AxisPosition.Bottom); // X-axis: Product names (categories)
        IValueAxis leftAxis = axisFactory.CreateValueAxis(AxisPosition.Left); // Y-axis: Quantities
        leftAxis.Crosses = AxisCrosses.AutoZero;

        // Plot the chart
        chart.Plot(lineChartData);

        // Save the Excel file
        using (var fileData = new FileStream("SalesReportWithLineChart_NPOI_fixed2.xlsx", FileMode.Create))
        {
            workbook.Write(fileData);
        }

        Console.WriteLine("Excel file with line chart created successfully!");
    }
}

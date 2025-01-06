using System.Windows;
using System.Windows.Controls;
using ScottPlot;
using ScottPlot.Plottables;
using ScottPlot.TickGenerators;

namespace iLabTicketMgr;

public partial class ChartView : TabContentControl
{
    private ChartDataFetcher _dataFetcher = new();
    public ChartView()
    {
        InitializeComponent();
        Loaded += (s, e) =>
        {
            RefreshCharts();
        };
    }

    public void RefreshCharts()
    {
        FillStatusChart();
        FillTypeChart();
        FillSubtypeChart();
        FillCreationChart();
        FillClosureChart();
        FillSubtypeClosureChart();
    }

    private void FillStatusChart()
    {
        var data = _dataFetcher.GetTicketDistribution("status");
        
        float total = data.Values.Sum();
        var slices = data.Select(status => new PieSlice(status.Value, Colors.Azure, $"{status.Key} ({(status.Value/total):P})")).ToList();

        var pie = StatusChart.Plot.Add.Pie(slices);
        pie.DonutFraction = 0.35f;
        pie.ExplodeFraction = 0.1f;
        pie.SliceLabelDistance = 0.8;

        StatusChart.Plot.Title("Tickets by Current Status");
        StatusChart.Plot.Axes.Title.FullFigureCenter = true;

        // hide unnecessary plot components
        StatusChart.Plot.HideAxesAndGrid();
        StatusChart.Plot.ShowLegend();

        StatusChart.Refresh();
    }
    
    private void FillTypeChart()
    {
        var data = _dataFetcher.GetTicketDistribution("type");
        
        float total = data.Values.Sum();
        var slices = data.Select(status => new PieSlice(status.Value, Colors.Azure, $"{status.Key} ({(status.Value/total):P})")).ToList();

        var pie = TypeChart.Plot.Add.Pie(slices);
        pie.DonutFraction = 0.35f;
        pie.ExplodeFraction = 0.1f;
        pie.SliceLabelDistance = 0.8;

        TypeChart.Plot.Title("Tickets by Type");
        TypeChart.Plot.Axes.Title.FullFigureCenter = true;

        // hide unnecessary plot components
        TypeChart.Plot.HideAxesAndGrid();
        TypeChart.Plot.ShowLegend();

        TypeChart.Refresh();
    }
    
    private void FillSubtypeChart()
    {
        var data = _dataFetcher.GetTicketDistribution("subtype");
        
        float total = data.Values.Sum();
        var slices = data.Select(status => new PieSlice(status.Value, Colors.Azure, $"{status.Key} ({(status.Value/total):P})")).ToList();

        var pie = SubtypeChart.Plot.Add.Pie(slices);
        pie.DonutFraction = 0.35f;
        pie.ExplodeFraction = 0.1f;
        pie.SliceLabelDistance = 0.8;

        SubtypeChart.Plot.Title("Tickets by Subtype");
        SubtypeChart.Plot.Axes.Title.FullFigureCenter = true;

        // hide unnecessary plot components
        SubtypeChart.Plot.HideAxesAndGrid();
        SubtypeChart.Plot.ShowLegend();

        SubtypeChart.Refresh();
    }

    private void FillCreationChart()
    {
        var data = _dataFetcher.GetTicketCreationData();

        var dates = data.Keys.ToArray();
        var values = data.Values.ToArray();
        
        CreationChart.Plot.Add.Scatter(dates, values, color: Colors.Black);
        CreationChart.Plot.Axes.DateTimeTicksBottom();
        CreationChart.Plot.Axes.Margins(bottom: 0);
        
        CreationChart.Plot.Title("Tickets Created");
        CreationChart.Refresh();
    }
    
    private void FillClosureChart()
    {
        var data = _dataFetcher.GetTicketClosureData();

        var dates = data.Keys.ToArray();
        var values = data.Values.ToArray();
        
        ClosureChart.Plot.Add.Scatter(dates, values, color: Colors.Black);
        ClosureChart.Plot.Axes.DateTimeTicksBottom();
        ClosureChart.Plot.Axes.Margins(bottom: 0);
        
        ClosureChart.Plot.Title("Mean Time to Closure (hrs)");
        ClosureChart.Refresh();
    }

    private void FillSubtypeClosureChart()
    {
        var data = _dataFetcher.GetTicketClosureBySubtype();

        var ticks = data.Select((item, i) => new Tick(i, item.subtype)).ToArray();
        var bars = data.Select((item, i) => new Bar { Position = i, Value = item.hours, FillColor = Colors.Lavender}).ToArray();
        ClosureBySubtypeChart.Plot.Add.Bars(bars);
        ClosureBySubtypeChart.Plot.Axes.Bottom.TickGenerator = new NumericManual(ticks);

        ClosureBySubtypeChart.Plot.Title("Mean Time to Closure by Subtype (hrs)");
        ClosureBySubtypeChart.Plot.HideGrid();
        ClosureBySubtypeChart.Plot.Axes.Margins(bottom: 0);
        
        ClosureBySubtypeChart.Refresh();
    }

    private void DataRangeBtn_OnClick(object sender, RoutedEventArgs e)
    {
        
    }
}
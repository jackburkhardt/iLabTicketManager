using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;

namespace iLabTicketMgr;

public partial class TicketListView : TabContentControl
{
    public ObservableCollection<Ticket> VisibleTickets { get; set; } = [];

    public TicketListView()
    {
        InitializeComponent();
        Loaded += (s, e) =>
        {
            var foundTix = MainWindow.DB.SelectNLatestTickets(50);
            VisibleTickets.Clear();
            foreach (var ticket in foundTix)
            {
                VisibleTickets.Add(ticket);
            }
        };
    }
    
    private void ImportCSV_OnClick(object sender, RoutedEventArgs e)
    {
        var dialog = new Microsoft.Win32.OpenFileDialog
        {
            Filter = "CSV files (*.csv)|*.csv",
        };

        bool? result = dialog.ShowDialog();

        if (result.HasValue && result.Value)
        {
            var tickets = TicketReader.ParseTickets(dialog.FileName);
            foreach (var ticket in tickets)
            {
                MainWindow.DB.UpsertTicket(ticket);
            }
            
            var foundTix = MainWindow.DB.SelectNLatestTickets(50);
            VisibleTickets.Clear();
            foreach (var ticket in foundTix)
            {
                VisibleTickets.Add(ticket);
            }
        }
    }

    private void UpdateNumRows(object sender, RoutedEventArgs e)
    {
        var text = ((RibbonTextBox)sender).Text;
        int rows = int.TryParse(text, out int count) ? count : 0;
        var newRows = MainWindow.DB.SelectNLatestTickets(rows);
        VisibleTickets.Clear();
        foreach (var ticket in newRows)
        {
            VisibleTickets.Add(ticket);
        }
    }

    private void RunSQL_OnClick(object sender, RoutedEventArgs e)
    {
        var popup = new UserSQLPopup();
        popup.ShowDialog();
    }
}

public class TicketRowConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var ticket = (Ticket)value;
        if (ticket.Status.Contains("Closed"))
        {
            return Brushes.LightSlateGray;
        }
        else if (ticket.DateModified < DateTime.Now.AddDays(-3))
        {
            return Brushes.LightCoral;
        }
        else if (ticket.DateModified < DateTime.Now.AddDays(-1))
        {
            return Brushes.PaleGoldenrod;
        }
        else
        {
            return DependencyProperty.UnsetValue;
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
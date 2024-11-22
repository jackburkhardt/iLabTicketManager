using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace iLabTicketMgr;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public static Database DB { get; set; } = new Database();
    public ObservableCollection<Ticket> VisibleTickets { get; set; } = [];
    
    public MainWindow()
    {
        DataContext = this;
        InitializeComponent();
        Loaded += (s, e) =>
        {
            var foundTix = DB.SelectNLatestTickets(50);
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
                DB.UpsertTicket(ticket);
            }
            
            var foundTix = DB.SelectNLatestTickets(50);
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
        var newRows = DB.SelectNLatestTickets(rows);
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
        throw new NotImplementedException();
    }
}
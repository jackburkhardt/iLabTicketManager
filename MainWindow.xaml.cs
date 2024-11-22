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
    
    public MainWindow()
    {
        DataContext = this;
        InitializeComponent();
    }
    

    private void TicketListView_OnClick(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void ChartView_OnClick(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void DashboardView_OnClick(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }
}

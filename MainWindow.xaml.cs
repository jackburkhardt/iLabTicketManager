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
    public ObservableCollection<TabContentControl> Tabs { get; set; } = [];
    
    public MainWindow()
    {
        DataContext = this;
        InitializeComponent();
        Loaded += (s, e) =>
        {
            Tabs.Add(new TicketListView());
        };
    }
    

    private void TicketListView_OnClick(object sender, RoutedEventArgs e)
    {
        Tabs.Add(new TicketListView());
    }

    private void ChartView_OnClick(object sender, RoutedEventArgs e)
    {
        Tabs.Add(new ChartView());
    }

    private void DashboardView_OnClick(object sender, RoutedEventArgs e)
    {
        Tabs.Add(new DashboardView());
    }

    private void CloseTabClicked(object sender, RoutedEventArgs e)
    {
        Tabs.Remove((TabContentControl)((RibbonButton)sender).Tag);
    }
}

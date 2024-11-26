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
    public ObservableCollection<TabViewModel> Tabs { get; set; } = [];
    
    public MainWindow()
    {
        InitializeComponent();
        Loaded += (s, e) =>
        {
            var content = new TicketListView();
            var vm = new TabViewModel()
            {
                Content = content,
                Header = content.TabHeader,
                MenuItems = content.MenuGroups
            };
        
            AddTab(vm);
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
            
            ((TabViewModel)TabView.SelectedContent).Content.RefreshView();
            
        }
    }
    

    private void TicketListView_OnClick(object sender, RoutedEventArgs e)
    {
        var view = new TicketListView();
        var vm = new TabViewModel()
        {
            Content = view,
            Header = view.TabHeader,
            MenuItems = view.MenuGroups
        };
        
        AddTab(vm);
    }

    private void ChartView_OnClick(object sender, RoutedEventArgs e)
    {
        //Tabs.Add(new ChartView());
    }

    private void DashboardView_OnClick(object sender, RoutedEventArgs e)
    {
       // Tabs.Add(new DashboardView());
    }

    private void CloseTabClicked(object sender, RoutedEventArgs e)
    {
        RemoveTab(Tabs[TabView.SelectedIndex]);
    }

    private void AddTab(TabViewModel tabInfo)
    {
        Tabs.Add(tabInfo);
        foreach (var group in tabInfo.MenuItems)
        {
            group.Visibility = Visibility.Collapsed;
            menuRibbon.Items.Add(group);
        }
        TabView.SelectedItem = tabInfo;
    }

    private void OnTabSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.RemovedItems.Count == 0) return;
        
        var newTab = e.AddedItems.Cast<TabViewModel>().FirstOrDefault();
        var lastTab = e.RemovedItems.Cast<TabViewModel>().FirstOrDefault();

        foreach (var oldGroup in lastTab.MenuItems)
        {
            oldGroup.Visibility = Visibility.Collapsed;
        }

        foreach (var newGroup in newTab.MenuItems)
        {
            newGroup.Visibility = Visibility.Visible;
        }
    }

    private void RemoveTab(TabViewModel tabInfo)
    {
        Tabs.Remove(tabInfo);
        foreach (var group in tabInfo.MenuItems)
        {
            menuRibbon.Items.Remove(group);
        }
    }
}

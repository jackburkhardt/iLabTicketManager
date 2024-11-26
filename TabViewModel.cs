using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Controls.Ribbon;

namespace iLabTicketMgr;

public class TabViewModel : INotifyPropertyChanged
{
    public string Header
    {
        get => header;
        set
        {
            header = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Header)));
        }
    }
    private string header;

    public TabContentControl Content
    {
        get => content;
        set
        {
            content = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Content)));
        }
    }
    private TabContentControl content;
    
    public ObservableCollection<RibbonGroup> MenuItems { get; set; } = new();
   
    
    public event PropertyChangedEventHandler? PropertyChanged;
    
}
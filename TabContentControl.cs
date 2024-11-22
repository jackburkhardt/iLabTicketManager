using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Controls.Ribbon;

namespace iLabTicketMgr;

public class TabContentControl : UserControl
{
    public ObservableCollection<RibbonGroup> MenuGroups { get; set; } = new();
    public virtual string TabHeader { get; set; } = "Tab";
}
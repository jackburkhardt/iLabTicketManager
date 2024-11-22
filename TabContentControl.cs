using System.Windows.Controls;
using System.Windows.Controls.Ribbon;

namespace iLabTicketMgr;

public class TabContentControl : UserControl
{
    public List<RibbonGroup> MenuGroups { get; set; } = new();
}
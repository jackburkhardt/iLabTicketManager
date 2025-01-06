using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Ribbon;
using System.Windows.Input;

namespace iLabTicketMgr;

public abstract class TabContentControl : UserControl
{
    
    public static readonly DependencyProperty TabHeaderProperty = DependencyProperty.Register(nameof(TabHeader), typeof(string), typeof(TabContentControl));

    public string TabHeader
    {
        get => (string)GetValue(TabHeaderProperty);
        set => SetValue(TabHeaderProperty, value);
    }

    public ObservableCollection<RibbonGroup> MenuGroups { get; set; } = new();
    
    public virtual void RefreshView(){}

}
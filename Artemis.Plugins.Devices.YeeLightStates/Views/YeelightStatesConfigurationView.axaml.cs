using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Artemis.Plugins.Devices.YeeLightStates.Views;

public partial class YeelightStatesConfigurationView : UserControl
{
    public YeelightStatesConfigurationView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
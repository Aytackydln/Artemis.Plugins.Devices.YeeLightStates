using Artemis.Core;
using Artemis.UI.Shared;

namespace Artemis.Plugins.Devices.YeeLightStates.ViewModels;

public class YeelightStatesConfigurationViewModel : PluginConfigurationViewModel
{
    public YeelightStatesConfigurationViewModel(Plugin plugin, PluginSettings pluginSettings) : base(plugin)
    {
        DeviceIds = pluginSettings.GetSetting(YeelightDeviceProvider.DeviceIpAddress, "192.168.1.248");
        DeviceIds.AutoSave = true;
    }

    public PluginSetting<string> DeviceIds { get; }

    public override void OnCloseRequested()
    {
        DeviceIds.AutoSave = false;
    }
}
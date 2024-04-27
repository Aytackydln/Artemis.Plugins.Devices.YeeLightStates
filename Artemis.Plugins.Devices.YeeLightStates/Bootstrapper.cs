using Artemis.Core;
using Artemis.Plugins.Devices.YeeLightStates.ViewModels;
using Artemis.UI.Shared;

namespace Artemis.Plugins.Devices.YeeLightStates
{
    public class Bootstrapper : PluginBootstrapper
    {
        public override void OnPluginLoaded(Plugin plugin)
        {
            plugin.ConfigurationDialog = new PluginConfigurationDialog<YeelightStatesConfigurationViewModel>();
        }

        public override void OnPluginEnabled(Plugin plugin)
        {
        }

        public override void OnPluginDisabled(Plugin plugin)
        {
        }
    }
}
using System;
using System.Net;
using Artemis.Core;
using Artemis.Core.DeviceProviders;
using Artemis.Core.Services;

namespace Artemis.Plugins.Devices.YeeLightStates;

public class YeelightDeviceProvider : DeviceProvider
{
    public static readonly String DeviceIpAddress = "DeviceIpAddress";
        
    private readonly IRgbService _rgbService;
    private readonly PluginSetting<String> _ipSetting;

    public YeelightDeviceProvider(IRgbService rgbService, PluginSettings settings) : base(RGB.NET.YeeLightStates.YeelightProvider.Instance)
    {
        _ipSetting = settings.GetSetting(DeviceIpAddress, "192.168.1.248");
        RGB.NET.YeeLightStates.YeelightProvider.IpAddresses = _ipSetting.Value;
        _rgbService = rgbService;
        CreateMissingLedsSupported = false;
        RemoveExcessiveLedsSupported = true;

        CanDetectLogicalLayout = false;
        CanDetectPhysicalLayout = false;
    }

    public override void Enable()
    {
        RGB.NET.YeeLightStates.YeelightProvider.IpAddresses = "192.168.1.248";
        _rgbService.AddDeviceProvider(RgbDeviceProvider);
    }

    public override void Disable()
    {
        _rgbService.RemoveDeviceProvider(RgbDeviceProvider);
        RgbDeviceProvider.Dispose();
    }
}
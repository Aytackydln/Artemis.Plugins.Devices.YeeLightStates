using RGB.NET.Core;
using YeeLightAPI;

namespace RGB.NET.YeeLightStates;

public class YeelightDeviceInfo : IRGBDeviceInfo
{
    public RGBDeviceType DeviceType => RGBDeviceType.LedStripe;
    public string Manufacturer => "Yeelight";
    public string DeviceName { get; }
    public string Model { get; }
    public object? LayoutMetadata { get; set; }

    public YeelightDeviceInfo(YeeLightDevice device)
    {
        var (ip, _) = device.GetLightIPAddressAndPort();
        DeviceName = "Yeelight: " + ip;
        Model = "(" + ip + ")";
    }
}
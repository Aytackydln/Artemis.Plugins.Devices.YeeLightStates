using System.Collections.Generic;
using RGB.NET.Core;

namespace RGB.NET.YeeLightStates;

public class RgbYeelightDevice : AbstractRGBDevice<YeelightDeviceInfo>
{
    private readonly Dictionary<object, LedId> _keyMapping = new()
    {
        [0] = LedId.Custom1
    };

    internal RgbYeelightDevice(YeelightDeviceInfo deviceInfo, IUpdateQueue updateQueue)
        : base(deviceInfo, updateQueue)
    {
        InitializeLayout();
    }

    private void InitializeLayout()
    {
        var x = 0;
        foreach (var key in _keyMapping.Keys)
        {
            if (!_keyMapping.TryGetValue(key, out LedId ledId))
            {
                continue;
            }

            AddLed(ledId, new Point(x, 0), new Size(19), key);
            x += 20;
        }
    }

    private bool Equals(RgbYeelightDevice other)
    {
        return DeviceInfo.DeviceName.Equals(other.DeviceInfo.DeviceName);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((RgbYeelightDevice)obj);
    }

    public override int GetHashCode()
    {
        return DeviceInfo.DeviceName.GetHashCode();
    }
}
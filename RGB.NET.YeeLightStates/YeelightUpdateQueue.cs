using System;
using System.Timers;
using RGB.NET.Core;

namespace RGB.NET.YeeLightStates;

public sealed class YeelightUpdateQueue : UpdateQueue
{
    private readonly YeeLightAPI.YeeLightDevice _yeeLightDevice;

    private readonly Timer _refreshTimer; // To avoid power off by inactivity
    private readonly Timer _connectTimer;

    private IYeeLightState _state;
    private Color _color = Color.Transparent;

    public YeelightUpdateQueue(IDeviceUpdateTrigger updateTrigger, YeeLightAPI.YeeLightDevice yeeLightDevice) :
        base(updateTrigger)
    {
        _yeeLightDevice = yeeLightDevice;

        _refreshTimer = new Timer(5000);
        _refreshTimer.Elapsed += _refreshTimer_Elapsed;
        _refreshTimer.AutoReset = true;

        _connectTimer = new Timer(1000);
        _connectTimer.Elapsed += _connectTimer_Elapsed;
        _connectTimer.Start();

        _state = YeeLightStateBuilder.Build(_yeeLightDevice, 5);
    }

    private void _refreshTimer_Elapsed(object sender, ElapsedEventArgs e)
    {
        _refreshTimer.Stop();
        _refreshTimer.Start();
        if (_yeeLightDevice.IsConnected())
        {
            _state = _state.Update(_color);
        }
    }

    private void _connectTimer_Elapsed(object sender, ElapsedEventArgs e)
    {
        if (!_yeeLightDevice.IsConnected())
        {
            _yeeLightDevice.Connect();
        }
    }

    protected override bool Update(in ReadOnlySpan<(object key, Color color)> dataSet)
    {
        _refreshTimer.Stop();
        _refreshTimer.Start();

        if (!_yeeLightDevice.IsConnected())
        {
            return false;
        }

        foreach (var item in dataSet)
        {
            _state = _state.Update(item.color);
            _color = item.color;
        }

        return true;
    }

    public override void Dispose()
    {
        _connectTimer.Stop();
        _refreshTimer.Stop();
        base.Dispose();
    }
}
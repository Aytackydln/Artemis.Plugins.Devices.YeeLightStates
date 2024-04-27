using System;
using RGB.NET.Core;
using YeeLightAPI;
using YeeLightAPI.YeeLightConstants;

namespace RGB.NET.YeeLightStates;

public interface IYeeLightState
{
    void InitState();

    IYeeLightState Update(Color color);
}

internal static class YeeLightStateBuilder
{
    public static IYeeLightState Build(YeeLightDevice device, int whiteCounter)
    {
        var colorState = new YeeLightStateColor(device, whiteCounter);
        var whiteState = new YeeLightStateWhite(device);
        var offState = new YeeLightStateOff(device);
            
        colorState.WhiteState = whiteState;
        colorState.OffState = offState;
        whiteState.ColorState = colorState;
        whiteState.OffState = offState;
        offState.ColorState = colorState;

        return offState;
    }
}

internal class YeeLightStateOff : IYeeLightState
{
    public IYeeLightState ColorState;

    private readonly YeeLightDevice _light;

    public YeeLightStateOff(YeeLightDevice light)
    {
        _light = light;
    }

    public void InitState()
    {
        TurnOff();
    }

    public IYeeLightState Update(Color color)
    {
        if (Utils.IsBlack(color))
        {
            return this;
        }

        TurnOn();
        ColorState.InitState();
        var newState = ColorState.Update(color);
        return newState;
    }
        
    private void TurnOff()
    {
        _light.SetPower(Constants.PowerStateParamValues.OFF);
    }
        
    private void TurnOn()
    {
        _light.SetPower(Constants.PowerStateParamValues.ON);
    }
}
    
internal class YeeLightStateColor : IYeeLightState
{
    public IYeeLightState WhiteState;
    public IYeeLightState OffState;

    private readonly YeeLightDevice _light;
    private readonly int _whiteCounterStart;

    private int _previousBrightness;
    private int _whiteCounter;

    public YeeLightStateColor(YeeLightDevice light, int whiteCounterStart)
    {
        _light = light;
        _whiteCounterStart = whiteCounterStart;
    }

    public void InitState()
    {
        _previousBrightness = -1;
        _whiteCounter = _whiteCounterStart;
    }

    public IYeeLightState Update(Color color)
    {
        if (Utils.IsWhiteTone(color))
        {
            if (Utils.IsBlack(color))
            {
                OffState.InitState();
                return OffState.Update(color);
            }
            if (_whiteCounter-- <= 0)
            {
                WhiteState.InitState();
                return WhiteState.Update(color);
            }
        }
        else
        {
            _whiteCounter = _whiteCounterStart;
        }

        //color changed
        UpdateLights(color);
        return this;
    }

    private void UpdateLights(Color color)
    {
        _light.SetColor(color.GetR(), color.GetG(), color.GetB(),
            100,
            Constants.EffectParamValues.SMOOTH
        );
        //var brightness = (int)(Math.Max(color.R, Math.Max(color.G, color.B)) * 100);
        //SetBrightness(color);
        //_light.SetColorAndBrightness(color.GetR(), color.GetG(), color.GetB(),
        //    brightness,
        //    50,
        //    Constants.EffectParamValues.SMOOTH
        //);
    }

    private void SetBrightness(Color color)
    {
        var brightness = (int)(Math.Max(color.R, Math.Max(color.G, color.B)) * 100);
        if (_previousBrightness == brightness) return;
        _previousBrightness = brightness;
        _light.SetBrightness(brightness, 50);
    }
}

internal class YeeLightStateWhite : IYeeLightState
{
    public IYeeLightState ColorState;
    public IYeeLightState OffState;

    private readonly YeeLightDevice _light;

    public YeeLightStateWhite(YeeLightDevice light)
    {
        _light = light;
    }

    public void InitState()
    {
        _light.SetColorTemperature(6500);
    }

    public IYeeLightState Update(Color color)
    {
        if (!Utils.IsWhiteTone(color))
        {
            ColorState.InitState();
            return ColorState.Update(color);
        }
            
        if (Utils.IsBlack(color))
        {
            OffState.InitState();
            return OffState.Update(color);
        }
            
        //color changed
        UpdateLights(color);
        return this;
    }

    private void UpdateLights(Color color)
    {
        _light.SetBrightness(color.GetR() * 100 / 255);
    }
}

internal static class Utils
{
    internal static bool IsWhiteTone(Color color)
    {
        return color.GetR() == color.GetG() && color.GetG() == color.GetB();
    }
    internal static bool IsBlack(Color color)
    {
        return color is { R: 0, G: 0, B: 0 };
    }
}
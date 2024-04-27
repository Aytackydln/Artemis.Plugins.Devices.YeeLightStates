using System;
using RGB.NET.Core;

namespace RGB.NET.YeeLightStates
{
    internal interface IYeelightDeviceState
    {
        public IYeelightDeviceState SetColor(Color targetColor);
    }
    
    internal class WhiteDisabledState : IYeelightDeviceState
    {
        private readonly YeeLightAPI.YeeLightDevice _device;
        
        internal WhiteDisabledState(YeeLightAPI.YeeLightDevice device)
        {
            _device = device;
        }
        
        public IYeelightDeviceState SetColor(Color targetColor)
        {
            var brightness = Math.Max(targetColor.GetR(), Math.Max(targetColor.GetG(), Math.Max(targetColor.GetB(), (short)1))) * 100 / 255;
            _device.SetColor(targetColor.GetR(), targetColor.GetG(), targetColor.GetB());
            _device.SetBrightness(brightness);
            return this;
        }
    }
    
    internal class WhiteEnabledState : IYeelightDeviceState
    {
        private readonly YeeLightAPI.YeeLightDevice _device;
        
        public WhiteState WhiteState { get; set; }
        
        internal WhiteEnabledState(YeeLightAPI.YeeLightDevice device)
        {
            _device = device;
        }
        
        public IYeelightDeviceState SetColor(Color targetColor)
        {
            var brightness = Math.Max(targetColor.GetR(), Math.Max(targetColor.GetG(), Math.Max(targetColor.GetB(), (short)1))) * 100 / 255;
            _device.SetColor(targetColor.GetR(), targetColor.GetG(), targetColor.GetB());
            _device.SetBrightness(brightness);
            return this;
        }
        
    }

    internal class WhiteState : IYeelightDeviceState
    {
        private readonly YeeLightAPI.YeeLightDevice _device;
        
        public WhiteState(YeeLightAPI.YeeLightDevice device)
        {
            _device = device;
        }

        IYeelightDeviceState IYeelightDeviceState.SetColor(Color targetColor)
        {
            _device.SetColorTemperature(6500);
            _device.SetBrightness(targetColor.GetR() * 100 / 255);
            return this;
        }
    }
}
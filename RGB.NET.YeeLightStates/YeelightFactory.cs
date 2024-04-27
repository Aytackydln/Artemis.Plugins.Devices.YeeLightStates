using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using RGB.NET.Core;
using YeeLightAPI;
using YeeLightAPI.YeeLightDeviceLocator;
using YeeLightAPI.YeeLightExceptions;

namespace RGB.NET.YeeLightStates;

[PublicAPI]
public sealed class YeelightProvider : AbstractRGBDeviceProvider
{
    private static Lazy<YeelightProvider> _lazyInstance = new(LazyThreadSafetyMode.ExecutionAndPublication);
    public static YeelightProvider Instance => _lazyInstance.Value;

    public static string IpAddresses { get; set; } = string.Empty;
    public bool MusicModeOnly { get; set; }

    private const int LightListenPort = 55443;
    private readonly YeelightConnectionListener _connectionListener = new();

    protected override void InitializeSDK()
    {
        _connectionListener.DeviceDetected += ConnectionListenerOnDeviceDetected;
        _connectionListener.StartListeningConnections();
    }

    private void ConnectionListenerOnDeviceDetected(object? sender, DeviceDetectedEventArgs e)
    {
        var yeeLightDevice = new YeeLightDevice(e.IpAddress);
        var rgbYeelightDevice = LoadDevice(yeeLightDevice);
        AddDevice(rgbYeelightDevice);
    }

    protected override IEnumerable<IRGBDevice> LoadDevices()
    {
        if (!string.IsNullOrWhiteSpace(IpAddresses))
            return IpAddresses.Split(',')
                .Select(s => s.Trim())
                .Select(IPAddress.Parse)
                .Select(ip => new YeeLightDevice(ip))
                .Select(LoadDevice);
        Task.Run(() =>
        {
            foreach (var rgbYeelightDevice in DeviceLocator.DiscoverDevices(5000, 2))
            {
                var discoveredDevice = LoadDevice(rgbYeelightDevice);
                AddDevice(discoveredDevice);
            }
        });
        return Enumerable.Empty<IRGBDevice>();
    }

    private RgbYeelightDevice LoadDevice(YeeLightDevice light)
    {
        var (ipAddress, port) = light.GetLightIPAddressAndPort();
        light.SetLightIPAddressAndPort(ipAddress, port);
        LightConnectAndEnableMusicMode(light);

        var updateTrigger = GetUpdateTrigger(-1, 0.4);
        var yeelightUpdateQueue = new YeelightUpdateQueue(
            updateTrigger,
            light
        );
        var rgbYeelightDevice = new RgbYeelightDevice(new YeelightDeviceInfo(light),
            yeelightUpdateQueue);

        light.Disconnected += (_, _) => RemoveDevice(rgbYeelightDevice);
        
        return rgbYeelightDevice;
    }

    private void LightConnectAndEnableMusicMode(YeeLightDevice light)
    {
        var localMusicModeListenPort = GetFreeTcpPort(); // This can be any free port

        IPAddress localIp;
        using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
        {
            var lightIp = light.GetLightIPAddressAndPort().ipAddress;
            socket.Connect(lightIp, LightListenPort);
            localIp = ((IPEndPoint)socket.LocalEndPoint).Address;
        }
 
        light.Connect();
        var connectionTries = 100;
        do
        {
            Thread.Sleep(500);
        } while (!light.IsConnected() && --connectionTries > 0);

        try
        {
            light.SetMusicMode(localIp, (ushort)localMusicModeListenPort, false);
            light.SetMusicMode(localIp, (ushort)localMusicModeListenPort, true);
        }
        catch(Exception e)
        {
            if (e is Exceptions.DeviceIsAlreadyInMusicMode)
            {
                return;
            }
            if (MusicModeOnly)
            {
                light.CloseConnection();
            }
        }
    }

    private int GetFreeTcpPort()
    {
        // When a TCPListener is created with 0 as port, the TCP/IP stack will assign it a free port
        var listener = new TcpListener(IPAddress.Loopback, 0); // Create a TcpListener on loopback with 0 as the port
        listener.Start();
        var freePort = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return freePort;
    }

    protected override void Dispose(bool disposing)
    {
        _connectionListener.StopListeningConnections();
        _lazyInstance = new(LazyThreadSafetyMode.ExecutionAndPublication);
        base.Dispose(disposing);
    }
}
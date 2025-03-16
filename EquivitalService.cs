using System;
using System.Collections.Generic;
using System.ServiceProcess;
using Equivital.DongleExtension;
using static SemParserLibrary.SemDevice;

using System.Linq;
using System.Text;
using Newtonsoft.Json; // Use Newtonsoft.Json for .NET 4.5

using SemParserLibrary;
using Equivital.DongleExtension;
using System.IO;
using System.IO.Ports;

using System.Reflection;

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Timers;
using System.Runtime.InteropServices;
using System.Xml.Linq;


public class EquivitalService
{
    private EquivitalDongleManager _manager;
    private readonly LSLService _lslService;
    private readonly DatabaseManager _dbManager;
    private string _roundId;

    public EquivitalService(LSLService lslService)
    {
        _lslService = lslService;
        _dbManager = new DatabaseManager();
        _manager = EquivitalDongleManager.Instance;
    }

    public void Connect(string devName, string licenseKey, string pinCode)
    {
        try
        {
            SemDevice.License.DeveloperName = devName;
            SemDevice.License.LicenseCode = licenseKey;

            Console.WriteLine("Starting Equivital Dongle Manager...");
            _manager.ConnectionAdded += DeviceConnectionAdded;
            _manager.ConnectionRemoved += DeviceConnectionRemoved;
            _manager.Start();

            Console.WriteLine("Waiting for Equivital Dongle to connect...");
            while (_manager.ConnectedIWrapDeviceCount == 0)
            {
                Console.WriteLine("Please connect an Equivital Dongle and press enter.");
                Console.ReadLine();
            }

            Console.WriteLine("Dongle connected. Starting Bluetooth discovery...");
            _manager.EquivitalSensorFound += DiscoveryEquivitalSensorFound;
            _manager.SensorDiscoveryComplete += DiscoverySensorDiscoveryComplete;
            _manager.DiscoverDevicesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error during Equivital setup: {e.Message}");
        }
    }

    private void DeviceConnectionAdded(object sender, SemConnectionEventArgs e)
    {
        Console.WriteLine("SEM device connected.");

        SemDevice device = new SemDevice();
        device.HeartRateDataReceived += DeviceHeartRateDataReceived;
        device.ECGDataReceived += DeviceECGDataReceived;
        device.Start(e.Connection);
    }

    private void DeviceConnectionRemoved(object sender, SemConnectionEventArgs e)
    {
        Console.WriteLine("SEM device disconnected.");
    }

    private static void DiscoveryEquivitalSensorFound(object sender, EquivitalBluetoothInfoEventArgs e)
    {
        Console.WriteLine($"Found SEM {e.Info.Name}");
    }

    private static void DiscoverySensorDiscoveryComplete(object sender, EventArgs e)
    {
        Console.WriteLine("Bluetooth discovery complete.");
    }

    private void DeviceHeartRateDataReceived(object sender, HeartRateEventArgs e)
    {
        var data = new
        {
            round_id = _roundId,
            hr_bpm = e.BeatsPerMinute,
            event_time = LSLService.GetUnixTimestampNow()
        };
        Console.WriteLine(data);
        _lslService.PushHeartRateData(data);
    }

    private void DeviceECGDataReceived(object sender, ECGSemMessageEventArgs e)
    {
        var data = new
        {
            round_id = _roundId,
            lead_one_raw = e.LeadOneRaw,
            lead_two_raw = e.LeadTwoRaw,
            event_time = LSLService.GetUnixTimestampNow()
        };
        Console.WriteLine(data);
        _lslService.PushECGData(data);
    }
}

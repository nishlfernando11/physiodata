using System;
using ECGDataStream;
using System.Data;
using Quobject.SocketIoClientDotNet.Client;
using Quobject.Collections.Immutable;
using System.Runtime.InteropServices;

public class DataService
{
    private Socket socket;
    private bool _isStreaming = false;
    private EquivitalService _equivitalService;
    
    public DataService(EquivitalService service)
    {
        _equivitalService = service;

    }
    public void StartListening()
    {
        string overcookedUrl = null;

        try
        {
            Config.LoadEnvVariables("..\\..\\.env"); // Change to your .env file

            overcookedUrl = Environment.GetEnvironmentVariable("OVERCOOKED_URL");
            Console.WriteLine(overcookedUrl);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading .env file: {ex.Message}");
        }

        try
        {

            Console.WriteLine("Connecting to Socket.IO Server...");
            socket = IO.Socket("http://localhost:80");  // Use HTTP for Socket.IO connection
            //socket = IO.Socket("http://host.docker.internal");

            //socket = IO.Socket("http://host.docker.internal", new IO.Options
            //{
            //    Reconnection = true,
            //    AutoConnect = true,
            //    Transports = ImmutableList.Create("websocket")
            //});

            //socket.On(Socket.EVENT_CONNECT, () =>
            //{
            //    Console.WriteLine("✅ Connected to Overcooked Socket.IO server.");
            //    socket.Emit("join", "{}"); // Ensure it joins the Overcooked room
            //});

            //string[] eventNames = { "start_game", "end_game", "state_pong", "error", "disconnect", "join" };
            //foreach (string eventName in eventNames)
            //{
            //    socket.On(eventName, (data) =>
            //    {
            //        Console.WriteLine($"🔍 Received event: {eventName} | Data: {data}");
            //    });
            //}

            socket.On(Socket.EVENT_CONNECT, (data) =>
            {
                Console.WriteLine("Connected to Overcooked Socket.IO server." + data);
            });

            socket.On("start_game", (data) =>
            {
                Console.WriteLine("Received start_ecg event. Starting ECG collection...");
                StartECGCollection();
            });

            socket.On("end_game", (data) =>
            {
                Console.WriteLine("Received stop_ecg event. Stopping ECG collection...");
                StopECGCollection();
            });

            socket.On("start_ecg", (data) =>
            {
                Console.WriteLine("Received start_ecg event. Starting ECG collection...");
                StartECGCollection();
            });

            socket.On("stop_ecg", (data) =>
            {
                Console.WriteLine("Received stop_ecg event. Stopping ECG collection...");
                StopECGCollection();
            });

            socket.On(Socket.EVENT_DISCONNECT, () =>
            {
                Console.WriteLine("Disconnected from Socket.IO server.");
            });

            socket.On(Socket.EVENT_MESSAGE, (data) =>
            {
                Console.WriteLine("Received event: ." + data);
            });


        }
        catch (Exception ex)
        {
            Console.WriteLine($"Socket.IO Connection Failed: {ex.Message}");
        }

    }

    private void HandleTrigger(string eventData)
    {
        if (eventData.Contains("start_ecg") && !_isStreaming)
        {
            Console.WriteLine("Received start_ecg. Beginning ECG data collection...");
            StartECGCollection();
        }
        else if (eventData.Contains("stop_ecg") && _isStreaming)
        {
            Console.WriteLine("Received stop_ecg. Stopping ECG data collection...");
            StopECGCollection();
        }
    }

    private void StartECGCollection()
    {
        _isStreaming = true;
        // Call your Equivital data streaming function here
        Console.WriteLine("ECG Data Collection Started...");
        _equivitalService.StartDataCollection();
    }

    private void StopECGCollection()
    {
        _isStreaming = false;
        // Call function to stop ECG streaming
        Console.WriteLine("ECG Data Collection Stopped...");
        _equivitalService.StopDataCollection();
    }
}

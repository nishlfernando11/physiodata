using System;
using ECGDataStream;
using System.Data;
using Quobject.SocketIoClientDotNet.Client;
using Quobject.Collections.Immutable;
using System.Runtime.InteropServices;
using Newtonsoft.Json.Linq;

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
            socket = IO.Socket(overcookedUrl);  // Use HTTP for Socket.IO connection
            //socket = IO.Socket("http://host.docker.internal");

        
            socket.On(Socket.EVENT_CONNECT, (data) =>
            {
                Console.WriteLine("Connected to Overcooked Socket.IO server." + data);
            });

            socket.On("start_ecg", (data) =>
            {
                Console.WriteLine("======Starting a new round ===============================");
                // Parse incoming data as JObject
                JObject json_data = JObject.Parse(data.ToString());

                // Generate default values based on UTC time
                long utctime = DateTime.UtcNow.Ticks;
                string defaultRoundId = utctime.ToString(); // Unique timestamp-based ID
                string defaultPlayerId = (utctime % int.MaxValue).ToString(); // Ensure it's within int range

                // Extract values from the incoming data, using UTC-based defaults if missing
                string roundId  = json_data["start_info"]?["round_id"]?.Value<string>() ?? defaultRoundId;
                string playerId = json_data["start_info"]?["player_id"]?.Value<string>() ?? defaultPlayerId;

                // Construct the JObject with extracted values
                JObject jdata = new JObject(
                    new JProperty("spectating", false),
                    new JProperty("start_info", new JObject(
                        new JProperty("round_id", roundId),
                        new JProperty("player_id", playerId)
                    ))
                );


                // Accessing nested properties using the JObject indexer

                // Print the values to console
                Console.WriteLine($"Round ID: {roundId}");
                Console.WriteLine($"Player ID: {playerId}");
                StartECGCollection(roundId, playerId);
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

            //socket.On(Socket.EVENT_MESSAGE, (data) =>
            //{
            //    Console.WriteLine("Received event: ." + data);
            //});


        }
        catch (Exception ex)
        {
            Console.WriteLine($"Socket.IO Connection Failed: {ex.Message}");
        }

    }

    //private void HandleTrigger(string eventData)
    //{
    //    if (eventData.Contains("start_ecg") && !_isStreaming)
    //    {
    //        Console.WriteLine("Received start_ecg. Beginning ECG data collection...");
    //        StartECGCollection(eventData);
    //    }
    //    else if (eventData.Contains("stop_ecg") && _isStreaming)
    //    {
    //        Console.WriteLine("Received stop_ecg. Stopping ECG data collection...");
    //        StopECGCollection();
    //    }
    //}

    private void StartECGCollection(string roundID, string playerID)
    {
        _isStreaming = true;
        // Call your Equivital data streaming function here
        Console.WriteLine("ECG Data Collection Started...");
        _equivitalService.StartDataCollection(roundID, playerID);
    }

    private void StopECGCollection()
    {
        _isStreaming = false;
        // Call function to stop ECG streaming
        Console.WriteLine("ECG Data Collection Stopped...");
        _equivitalService.StopDataCollection();
    }
}

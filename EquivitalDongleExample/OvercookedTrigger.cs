using System;
using ECGDataStream;


public class OvercookedTrigger
{
    private EquivitalService _equivitalService;

    public OvercookedTrigger(EquivitalService service)
    {
        _equivitalService = service;
    }

    public void ListenForKeyPress()
    {
        Console.WriteLine("Listening for Overcooked UI trigger... (Press 'S' to start, 'X' to stop)");
        while (true)
        {
            var key = Console.ReadKey(true).Key;
            if (key == ConsoleKey.Q)
            {
                _equivitalService.QuitExperiment();
            }
            else if (key == ConsoleKey.S)
            {
                //_equivitalService.StartDataCollection();
            }
            else if (key == ConsoleKey.X)
            {
                _equivitalService.StopDataCollection();
            }

        }
    }
}



//using System;
//using System.IO;
//using System.Threading;

//public class OvercookedTrigger
//{
//    private EquivitalService _equivitalService;
//    private string _filePath = "trigger.txt";

//    public OvercookedTrigger(EquivitalService service)
//    {
//        _equivitalService = service;
//    }

//    public void MonitorTriggerFile()
//    {
//        Console.WriteLine("Monitoring Overcooked UI trigger file...");
//        while (true)
//        {
//            if (File.Exists(_filePath))
//            {
//                string action = File.ReadAllText(_filePath);
//                if (action.Trim() == "start")
//                {
//                    _equivitalService.StartDataCollection();
//                }
//                else if (action.Trim() == "stop")
//                {
//                    _equivitalService.StopDataCollection();
//                }

//                // Clear the file to avoid repeated triggers
//                File.WriteAllText(_filePath, "");
//            }

//            Thread.Sleep(500); // Check every 500ms
//        }
//    }
//}

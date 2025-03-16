
using System;
using ECGDataStream;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using Newtonsoft.Json; // Use Newtonsoft.Json for .NET 4.5

using SemParserLibrary;
using Equivital.DongleExtension;
using static SemParserLibrary.SemDevice;
using System.IO;
using System.IO.Ports;

using System.Reflection;

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Timers;
using System.Runtime.InteropServices;
using System.Xml.Linq;

namespace ECGDataStream
{
    class Program
    {

        private readonly DatabaseManager _dbManager;
        private readonly LSLWrapper _lsl;
        //public double roundId;
        public string roundId;

        public Program()
        {
            _dbManager = new DatabaseManager();
            _lsl = new LSLWrapper();

            //Environment.SetEnvironmentVariable("LSL_ALLOW_REMOTE", "1", EnvironmentVariableTarget.Process);
            //Environment.SetEnvironmentVariable("LSL_LISTEN_ADDRESS", "192.168.1.103", EnvironmentVariableTarget.Process);



        }

        static void Main(string[] args)
        {
            Config.LoadEnvVariables("..\\..\\.env");

            string devName = Environment.GetEnvironmentVariable("EQ_DEV_NAME");
            string licenseKey = Environment.GetEnvironmentVariable("EQ_LICENSE_KEY");
            string pinCode = Environment.GetEnvironmentVariable("EQ_DONGLE_PIN");

            if (string.IsNullOrWhiteSpace(devName) || string.IsNullOrWhiteSpace(licenseKey))
            {
                Console.WriteLine("Missing Equivital credentials.");
                return;
            }

            Program program = new Program();
            EquivitalService equivitalService = new EquivitalService(devName, licenseKey, pinCode);

            OvercookedTrigger overcookedTrigger = new OvercookedTrigger(equivitalService);

            // Start monitoring in a separate thread
            Thread triggerThread = new Thread(overcookedTrigger.ListenForKeyPress);
            triggerThread.Start();


            // Start the Equivital Dongle Manager
            equivitalService.StartDongleManager();

            Console.WriteLine("Listening for Overcooked UI trigger... (Press 'S' to start round, 'X' to stop round)");
            Console.WriteLine("Press 'q' to exit...");

            Console.ReadKey();


            try
            {

            }
            catch (Exception e)
            {
                Console.WriteLine($"Error during execution: {e.Message}");
            }
            finally
            {
                // Stop the Equivital Dongle Manager at the end of the experiment
                //equivitalService.StopDongleManager();
                Console.WriteLine("Data collection done for this round.");
            }

            //Console.ReadLine();
        }
    }


}
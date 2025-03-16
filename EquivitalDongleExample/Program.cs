//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Newtonsoft.Json; // Use Newtonsoft.Json for .NET 4.5

//using SemParserLibrary;
//using Equivital.DongleExtension;
//using static SemParserLibrary.SemDevice;
//using System.IO;
//using System.IO.Ports;

//using System.Reflection;

//using System.Diagnostics;
//using System.Runtime.CompilerServices;
//using System.Threading;
//using System.Timers;
//using System.Runtime.InteropServices;
//using System.Xml.Linq;


//namespace ECGDataStream
//{
//    class Program
//    {

//        private readonly DatabaseManager _dbManager;
//        private readonly LSLWrapper _lsl;
//        //public double roundId;
//        public string roundId;
//        //public static bool isMultDongles = false;
//        private IntPtr ecgStreamInfo;
//        private IntPtr hrStreamInfo;
//        private IntPtr accelerometerStreamInfo;
//        private IntPtr respirationRateStreamInfo;
//        private IntPtr impedanceRespirationStreamInfo;
//        private IntPtr skinTemperatureStreamInfo;
//        private IntPtr gsrStreamInfo;

//        private IntPtr ecgOutlet;
//        private IntPtr hrOutlet;
//        private IntPtr accelerometerOutlet;
//        private IntPtr respirationRateOutlet;
//        private IntPtr impedanceRespirationOutlet;
//        private IntPtr skinTemperatureOutlet;
//        private IntPtr gsrOutlet;

//        public Program()
//        {
//            _dbManager = new DatabaseManager();
//            _lsl = new LSLWrapper();

//            //Environment.SetEnvironmentVariable("LSL_ALLOW_REMOTE", "1", EnvironmentVariableTarget.Process);
//            //Environment.SetEnvironmentVariable("LSL_LISTEN_ADDRESS", "192.168.1.103", EnvironmentVariableTarget.Process);

//            //_config = new Config();
//            //LSL outlets

//            //Environment.SetEnvironmentVariable("LSL_LISTEN_ADDRESS", "127.0.0.1", EnvironmentVariableTarget.Process);
//            // string samples
//            ecgStreamInfo = this._lsl.CreateECGStream();


//            //ecgStreamInfo = this._lsl.CreateECGStream();
//            hrStreamInfo = this._lsl.CreateHRStream();
//            accelerometerStreamInfo = this._lsl.CreateAccelerometerStream();
//            respirationRateStreamInfo = this._lsl.CreateRRStream();
//            impedanceRespirationStreamInfo = this._lsl.CreateIRStream();
//            skinTemperatureStreamInfo = this._lsl.CreateSkinTempStream();
//            gsrStreamInfo = this._lsl.CreateGSRStream();


//            ecgOutlet = this._lsl.CreateOutlet(ecgStreamInfo);
//            hrOutlet = this._lsl.CreateOutlet(hrStreamInfo);
//            accelerometerOutlet = this._lsl.CreateOutlet(accelerometerStreamInfo);
//            respirationRateOutlet = this._lsl.CreateOutlet(respirationRateStreamInfo);
//            impedanceRespirationOutlet = this._lsl.CreateOutlet(impedanceRespirationStreamInfo);
//            skinTemperatureOutlet = this._lsl.CreateOutlet(skinTemperatureStreamInfo);
//            gsrOutlet = this._lsl.CreateOutlet(gsrStreamInfo);

//        }

//        //static void LoadEnvVariables(string filePath)
//        //{
//        //    foreach (var line in File.ReadAllLines(filePath))
//        //    {
//        //        if (line.Contains("="))
//        //        {
//        //            var parts = line.Split(new char[] { '=' }, 2);
//        //            Environment.SetEnvironmentVariable(parts[0].Trim(), parts[1].Trim());
//        //        }
//        //    }
//        //}


//        static void Main(string[] args)
//        {
//            EquivitalDongleManager manager = null;
//            bool ednWasRunning = false;
//            string licenseKey = null;
//            string devName = null;
//            string pinCode = null;
//            try
//            {
//                Config.LoadEnvVariables("..\\..\\.env"); // Change to your .env file

//                licenseKey = Environment.GetEnvironmentVariable("EQ_LICENSE_KEY");
//                devName = Environment.GetEnvironmentVariable("EQ_DEV_NAME");
//                pinCode = Environment.GetEnvironmentVariable("EQ_DONGLE_PIN");
//                //isMultDongles = bool.Parse(Environment.GetEnvironmentVariable("MULT_DONGLES"));


//                if (string.IsNullOrWhiteSpace(licenseKey) || string.IsNullOrWhiteSpace(devName))
//                {
//                    Console.WriteLine("Environment variables are not set or empty.");
//                }
//                else
//                {
//                    Console.WriteLine($"Developer Name: {devName}");
//                }
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Error loading .env file: {ex.Message}");
//            }


//            Program program = new Program();

//            //---Canned connection --
//            //Console.Write("Enter Round ID: ");
//            //string roundID = Console.ReadLine();
//            //Console.Write("Enter Player ID: ");
//            //string playerID = Console.ReadLine();


//            //string NewRoundID = GenerateCustomRoundId(roundID);
//            //program.roundId = NewRoundID; //assign to static 

//            //Console.WriteLine($"customRoundId: {NewRoundID}");
//            //// create new round
//            //var roundObj = new Dictionary<string, object>{
//            //       { "round_id", NewRoundID },
//            //       { "player_id", playerID },
//            //       { "start_time", program._lsl.GetUnixTimestampNow() },
//            //    };

//            //program.createRound(roundObj);
//            //try
//            //{
//            //    // Register SDK
//            //    SemDevice.License.DeveloperName = devName;
//            //    SemDevice.License.LicenseCode = licenseKey;

//            //    // Create a real-time "canned data" connection. 
//            //    ISemConnection cannedConnection = SemRealTimeFileConnection.CreateConnection();

//            //    // Create a decoder instance.
//            //    SemDevice device = new SemDevice();


//            //    // We're interested in heart rate and timer messages.
//            //    device.HeartRateDataReceived += program.DeviceHeartRateDataReceived;
//            //    device.ECGDataReceived += program.DeviceECGDataReceived;
//            //    device.AccelerometerDataReceived += program.DeviceAccelerometerDataReceived;
//            //    device.BeltRespirationRateDataReceived += program.DeviceRespirationRateDataReceived;
//            //    device.SkinTemperatureDataReceived += program.DeviceSkinTemperatureDataReceived;
//            //    device.ImpedanceRespirationDataReceived += program.DeviceImpedanceRespirationDataReceived; //TODO: not recieved in data
//            //    device.GalvanicSkinResistanceDataReceived += program.DeviceGSRDataReceived;
//            //    device.RawDataReceived += program.DeviceRawDataReceived;
//            //    device.SynchronisationTimerDataReceived += program.DeviceSynchronisationTimerDataReceived;
//            //    //device.SynchronisationTimerDataReceived += programprogram.DeviceSynchronisationTimerDataReceived;

//            //    // Start the decoder.
//            //    Console.WriteLine("Press any key to terminate the application.\r\n");
//            //    device.Start(cannedConnection);
//            //}
//            //catch (BadLicenseException)
//            //{
//            //    Console.WriteLine("Please enter a valid developer name and license code!");
//            //}
//            //catch (Exception e)
//            //{
//            //    Console.WriteLine("An error occurred while executing this example.\r\n\r\n" + e.Message);
//            //}

//            //Console.ReadLine();

//            //--- End Canned connection --

//            Console.Write("Enter Round ID: ");
//            string roundID = Console.ReadLine();
//            Console.Write("Enter Player ID: ");
//            string playerID = Console.ReadLine();


//            string NewRoundID = GenerateCustomRoundId(roundID);
//            program.roundId = NewRoundID; //assign to static 

//            Console.WriteLine($"customRoundId: {NewRoundID}");
//            // create new round
//            var roundObj = new Dictionary<string, object>{
//                   { "round_id", NewRoundID },
//                   { "player_id", playerID },
//                   { "start_time",  program._lsl.GetUnixTimestampNow() },
//                };

//            program.createRound(roundObj);

//            try
//            {
//                // Register SDK
//                SemDevice.License.DeveloperName = devName;
//                SemDevice.License.LicenseCode = licenseKey;


//                // If you want to use the Equivital Dongle with the SDK then we recommend that you disable
//                // the EDN Service so that the Equivital Dongle is not being used by eqView Pro or Equivital Manager.
//                // This can be done programmatically if you have the required permissions:
//                try
//                {
//                    System.ServiceProcess.ServiceController ednService =
//                        new System.ServiceProcess.ServiceController("EDN Node Service");

//                    ednWasRunning = ednService.Status == System.ServiceProcess.ServiceControllerStatus.Running;

//                    if (ednWasRunning)
//                    {
//                        Console.WriteLine("EDN Service running. Requesting it to stop");
//                        ednService.Stop();

//                        ednService.WaitForStatus(System.ServiceProcess.ServiceControllerStatus.Stopped);
//                    }
//                }
//                catch (Exception e)
//                {
//                    Console.WriteLine("An error occurred while executing stopping the EDN service.\r\n" + e.Message);
//                }




//                // Create the singleton dongle manager.
//                // The manager will then start looking for Equivital Bluetooth Dongles that are connected
//                // to the computer.
//                // Subscribe to the connection added and removed events to get notified on when a
//                // Equivital SEM device is connected or disconnected over Bluetooth.
//                Console.WriteLine("Starting Dongle Manager");

//                manager = EquivitalDongleManager.Instance;

//                manager.ConnectionAdded += program.DeviceConnectionAdded;
//                manager.ConnectionRemoved += program.DeviceConnectionRemoved;
//                manager.Start();


//                program._lsl.SynchronizeLSLClock();


//                Console.WriteLine("Enter q to terminate the application.\r\n");



//                // Here we wait for a dongle to be connected.
//                // Usually you would not need to do this as you can call manager.AddBluetoothConnectableDevice( EquivitalBluetoothSensorInfo )
//                // at any time and the connection will be made once a dongle is found.
//                while (manager.ConnectedIWrapDeviceCount == 0)
//                {

//                    Console.WriteLine("Please connect an Equivital Dongle if it is not already connected and press enter.");

//                    if (Console.ReadLine().ToLowerInvariant() == "q")
//                    {
//                        return;
//                    }

//                }


//                Console.WriteLine("There are {0} Equivital Dongles connected", manager.ConnectedIWrapDeviceCount);



//                Console.WriteLine("Turn on your SEM device and press enter to begin Bluetooth discovery");
//                //Console.ReadLine();

//                // Perform Bluetooth discovery to find Equivital SEM devices. 
//                // This happens asynchronously, with events firing when a device is discovered and when discovery completes.
//                manager.EquivitalSensorFound += DiscoveryEquivitalSensorFound;
//                manager.SensorDiscoveryComplete += new EventHandler(DiscoverySensorDiscoveryComplete);
//                manager.DiscoverDevicesAsync();

//                Console.WriteLine("Bluetooth discovery started");

//                // In this simple example we simply wait until the user the user selects a SEM to connect to
//                // or they enter "q" 
//                //int indexTonnectTo = 0;

//                //if (_foundSensors.Count > indexTonnectTo && indexTonnectTo >= 0)
//                //{
//                //    EquivitalBluetoothSensorInfo sensorInfo = _foundSensors[indexTonnectTo];
//                //    sensorInfo.PinCode = pinCode;
//                //    manager.AddBluetoothConnectableDevice(sensorInfo);
//                //}
//                //----
//                string userInput = null;
//                do
//                {
//                    int indexTonnectTo = 0;

//                    userInput = Console.ReadLine().ToLowerInvariant();

//                    if (_foundSensors.Count > indexTonnectTo && indexTonnectTo >= 0)
//                    {
//                        EquivitalBluetoothSensorInfo sensorInfo = _foundSensors[indexTonnectTo];
//                        sensorInfo.PinCode = pinCode;
//                        manager.AddBluetoothConnectableDevice(sensorInfo);
//                    }
//                } while (userInput != "q");
//                //--

//            }
//            catch (Exception e)
//            {
//                Console.WriteLine("An error occurred while executing this example.\r\n" + e.Message);
//            }
//            finally
//            {

//                // Execute the update
//                program.updateRound(program.roundId);

//                Console.WriteLine("Round ended, end time updated.");

//                if (manager != null)
//                {
//                    // Always terminate the manager when closing the application
//                    manager.Terminate();
//                    Console.WriteLine("Manager terminated!");
//                }

//                // If you have stopped the EDN service then you may want to restart it so that eqView Pro
//                // and Equivital Manager can use the Equivital Dongle again.
//                if (ednWasRunning)
//                {
//                    try
//                    {
//                        Console.WriteLine("Restarting EDN service");
//                        System.ServiceProcess.ServiceController ednService =
//                            new System.ServiceProcess.ServiceController("EDN Node Service");

//                        ednService.Start();
//                    }
//                    catch (Exception e)
//                    {
//                        Console.WriteLine("An error occurred while restarting the EDN service.\r\n" + e.Message);
//                    }
//                }
//            }

//            Console.WriteLine("Data collection done!");
//            Console.ReadLine();
//        }

//        private static void ConnectionStateChangedHandler(object sender, SemDeviceConnectionStateChangedEventArgs e)
//        {
//            Console.WriteLine($"Connection state changed: {e.PreviousState} -> {e.State}");
//        }

//        public void createRound(object roundObj)
//        {
//            _dbManager.InsertData("rounds", roundObj);
//        }

//        public void updateRound(string roundId)
//        {

//            // Data to update
//            var updateValues = new
//            {
//                end_time = this._lsl.GetUnixTimestampNow() // round end timestamp
//            };

//            var whereConditions = new Dictionary<string, object>
//            {
//                { "round_id", roundId }
//            };

//            // Execute update
//            _dbManager.UpdateData("rounds", updateValues, whereConditions);
//        }


//        static List<EquivitalBluetoothSensorInfo> _foundSensors = new List<EquivitalBluetoothSensorInfo>();
//        static void DiscoverySensorDiscoveryComplete(object sender, EventArgs e)
//        {
//            // When discovery is complete tell the user
//            int foundSensorCount = _foundSensors.Count;
//            Console.WriteLine("Discovery complete. {0} SEMs were found", foundSensorCount);

//            for (int i = 0; i < foundSensorCount; i++)
//            {
//                Console.WriteLine("[{0}] {1}", i, _foundSensors[i].Name);
//            }

//            Console.WriteLine("Please press enter to connect to the SEM");

//            //if (isMultDongles)
//            //{
//            //    Console.WriteLine("Please enter the index of the SEM that you would like to connect to");
//            //}
//        }

//        static void DiscoveryEquivitalSensorFound(object sender, EquivitalBluetoothInfoEventArgs e)
//        {
//            // The discovery will return a EquivitalBluetoothSensorInfo which includes information about the discovered SEM.
//            Console.WriteLine("Found SEM {0}", e.Info.Name);

//            if (!_foundSensors.Any(sem => sem.Address == e.Info.Address))
//            {
//                _foundSensors.Add(e.Info);
//            }
//        }

//        public void DeviceConnectionRemoved(object sender, SemConnectionEventArgs e)
//        {
//            Console.WriteLine("A SEM has disconnected");
//        }

//        public void DeviceConnectionAdded(object sender, SemConnectionEventArgs e)
//        {
//            try
//            {
//                Console.WriteLine("A new SEM is connected");

//                // Create a decoder instance.
//                SemDevice device = new SemDevice();
//                device.SetOperatingMode((SemOperatingModeType)DisclosureMode.Full);

//                // We're interested in heart rate and timer messages.
//                device.HeartRateDataReceived += this.DeviceHeartRateDataReceived;
//                device.ECGDataReceived += this.DeviceECGDataReceived;
//                device.AccelerometerDataReceived += this.DeviceAccelerometerDataReceived;
//                device.BeltRespirationRateDataReceived += this.DeviceRespirationRateDataReceived;
//                device.SkinTemperatureDataReceived += this.DeviceSkinTemperatureDataReceived;
//                device.ImpedanceRespirationDataReceived += this.DeviceImpedanceRespirationDataReceived; //TODO: not recieved in data
//                device.GalvanicSkinResistanceDataReceived += this.DeviceGSRDataReceived;
//                device.RawDataReceived += this.DeviceRawDataReceived;
//                device.SynchronisationTimerDataReceived += this.DeviceSynchronisationTimerDataReceived;

//                // Start the decoder.
//                Console.WriteLine("Press any key to terminate the application.\r\n");
//                // Start the decoder.
//                device.Start(e.Connection);
//            }
//            catch (BadLicenseException)
//            {
//                Console.WriteLine("Please enter a valid developer name and license code!");
//            }
//        }


//        public void DeviceSynchronisationTimerDataReceived(object sender, SyncrhonisationTimerEventArgs e)
//        {
//            double roundTime = correctedEventTime(e.SessionTime); // Assuming e.SessionTime is a DateTime with 1970 default
//            //DateTime currentDateWithRoundTime = DateTime.Today.Add(roundTime.TimeOfDay);

//            DateTime roundStartTime = DateTime.UtcNow; // Actual round start time
//            Console.WriteLine(e.SessionTime);
//            DateTime currentDateWithRoundTime = DateTime.Today.Add(roundStartTime.TimeOfDay);
//            Console.WriteLine(currentDateWithRoundTime.ToString("yyyy-MM-dd HH:mm:ss"));

//            Console.WriteLine(e + " (" + currentDateWithRoundTime.ToString("yyyy-MM-dd HH:mm:ss") + ")");
//            SaveData("SynchronisationTimerData", e + " (" + currentDateWithRoundTime.ToString("yyyy-MM-dd HH:mm:ss") + ")");

//        }

//        public void DeviceHeartRateDataReceived(object sender, HeartRateEventArgs e)
//        {
//            double lsl_timestamp = _lsl.GetLSLTimestampNow();
//            double unixTimestamp = _lsl.GetUnixTimestampNow();
//            double lslCorrectedTimestamp = _lsl.ConvertToLSLTime(unixTimestamp);

//            object heartRateData = new
//            {
//                round_id = this.roundId,
//                hr_bpm = e.BeatsPerMinute,
//                event_time = correctedEventTime(e.SessionTime),
//                lsl_timestamp = lsl_timestamp,
//                unix_timestamp = unixTimestamp
//            };
//            Console.WriteLine(e);
//            Console.WriteLine(heartRateData);

//            //double[] lslData = ConvertObjectToDoubleArray(heartRateData);
//            // Now push to LSL outlet (No need to assign the return value, as PushSample() is void)
//            Console.WriteLine("lsls data hr: ", heartRateData);

//            //this._lsl.PushSample(hrOutlet, lslData);
//            //this._lsl.PushSampleWithTimestamp(hrOutlet, lslData, lslCorrectedTimestamp);

//            string jsonData = JsonConvert.SerializeObject(heartRateData);
//            Console.WriteLine(jsonData);
//            this._lsl.PushSerializedSampleWithTimestamp(hrOutlet, jsonData, lslCorrectedTimestamp);

//            SaveData("HeartRateData", heartRateData);

//        }

//        public void DeviceECGDataReceived(object sender, ECGSemMessageEventArgs e)
//        {

//            double lsl_timestamp = _lsl.GetLSLTimestampNow();
//            double unixTimestamp = _lsl.GetUnixTimestampNow();
//            double lslCorrectedTimestamp = _lsl.ConvertToLSLTime(unixTimestamp);

//            object ECGData = new
//            {
//                round_id = this.roundId,
//                lead_one_raw = e.LeadOneRaw,
//                lead_two_raw = e.LeadTwoRaw,
//                sequence_number = e.SequenceNumber,
//                lead_one_mv = e.LeadOne_mV,
//                lead_two_mv = e.LeadTwo_mV,
//                event_time = correctedEventTime(e.SessionTime), //round_time
//                lsl_timestamp = lsl_timestamp,
//                unix_timestamp = unixTimestamp
//            };
//            Console.WriteLine(e);
//            Console.WriteLine(ECGData);

//            SaveData("ECGData", ECGData);
//            //double[] lslData = ConvertObjectToDoubleArray(ECGData);

//            //this._lsl.PushSample(ecgOutlet, lslData);
//            //this._lsl.PushSampleWithTimestamp(ecgOutlet, lslData, lslCorrectedTimestamp);

//            string jsonData = JsonConvert.SerializeObject(ECGData);
//            Console.WriteLine(jsonData);
//            this._lsl.PushSerializedSampleWithTimestamp(ecgOutlet, jsonData, lslCorrectedTimestamp);

//        }

//        public void DeviceAccelerometerDataReceived(object sender, AccelerometerSemMessageEventArgs e)
//        {

//            double lsl_timestamp = _lsl.GetLSLTimestampNow();
//            double unixTimestamp = _lsl.GetUnixTimestampNow();
//            double lslCorrectedTimestamp = _lsl.ConvertToLSLTime(unixTimestamp);

//            object accelerometerData = new
//            {
//                round_id = this.roundId,
//                vertical_mg = e.Vertical_mG,
//                lateral_mg = e.Lateral_mG,
//                longitudinal_mg = e.Longitudinal_mG,
//                resultant_mg = e.Resultant_mG,
//                vertical_raw = e.VerticalRaw,
//                lateral_raw = e.LateralRaw,
//                longitudinal_raw = e.LongitudinalRaw,
//                event_time = correctedEventTime(e.SessionTime),
//                lsl_timestamp = lsl_timestamp,
//                unix_timestamp = unixTimestamp
//            };
//            Console.WriteLine(e);
//            Console.WriteLine(accelerometerData);

//            SaveData("AccelerometerData", accelerometerData);
//            //double[] lslData = ConvertObjectToDoubleArray(accelerometerData);


//            //this._lsl.PushSample(accelerometerOutlet, lslData);
//            //this._lsl.PushSampleWithTimestamp(accelerometerOutlet, lslData, lslCorrectedTimestamp);

//            string jsonData = JsonConvert.SerializeObject(accelerometerData);
//            Console.WriteLine(jsonData);
//            this._lsl.PushSerializedSampleWithTimestamp(accelerometerOutlet, jsonData, lslCorrectedTimestamp);
//        }

//        public void DeviceRespirationRateDataReceived(object sender, RespirationRateEventArgs e)
//        {
//            double lsl_timestamp = _lsl.GetLSLTimestampNow();
//            double unixTimestamp = _lsl.GetUnixTimestampNow();
//            double lslCorrectedTimestamp = _lsl.ConvertToLSLTime(unixTimestamp);

//            object respirationRateData = new
//            {
//                round_id = this.roundId,
//                breaths_per_minute = e.BreathsPerMinute,
//                event_time = correctedEventTime(e.SessionTime),
//                lsl_timestamp = lsl_timestamp,
//                unix_timestamp = unixTimestamp
//            };
//            Console.WriteLine(e);
//            Console.WriteLine(respirationRateData);

//            SaveData("RespirationRateData", respirationRateData);
//            //double[] lslData = ConvertObjectToDoubleArray(respirationRateData);

//            //this._lsl.PushSample(respirationRateOutlet, lslData);
//            //this._lsl.PushSampleWithTimestamp(respirationRateOutlet, lslData, lslCorrectedTimestamp);

//            string jsonData = JsonConvert.SerializeObject(respirationRateData);
//            Console.WriteLine(jsonData);
//            this._lsl.PushSerializedSampleWithTimestamp(respirationRateOutlet, jsonData, lslCorrectedTimestamp);

//        }

//        public void DeviceImpedanceRespirationDataReceived(object sender, ImpedanceRespirationEventArgs e)
//        {

//            double lsl_timestamp = _lsl.GetLSLTimestampNow();
//            double unixTimestamp = _lsl.GetUnixTimestampNow();
//            double lslCorrectedTimestamp = _lsl.ConvertToLSLTime(unixTimestamp);

//            object impedanceRespirationData = new
//            {
//                round_id = this.roundId,
//                impedance = e.Impedance,
//                event_time = correctedEventTime(e.SessionTime),
//                lsl_timestamp = lsl_timestamp,
//                unix_timestamp = unixTimestamp
//            };
//            Console.WriteLine(e);
//            Console.WriteLine(impedanceRespirationData);

//            SaveData("ImpedanceRespirationData", impedanceRespirationData);
//            //double[] lslData = ConvertObjectToDoubleArray(impedanceRespirationData);

//            //this._lsl.PushSample(impedanceRespirationOutlet, lslData);
//            //this._lsl.PushSampleWithTimestamp(impedanceRespirationOutlet, lslData, lslCorrectedTimestamp);

//            string jsonData = JsonConvert.SerializeObject(impedanceRespirationData);
//            Console.WriteLine(jsonData);
//            this._lsl.PushSerializedSampleWithTimestamp(impedanceRespirationOutlet, jsonData, lslCorrectedTimestamp);

//        }

//        public void DeviceSkinTemperatureDataReceived(object sender, SkinTemperatureEventArgs e)
//        {
//            double lsl_timestamp = _lsl.GetLSLTimestampNow();
//            double unixTimestamp = _lsl.GetUnixTimestampNow();
//            double lslCorrectedTimestamp = _lsl.ConvertToLSLTime(unixTimestamp);

//            object skinTemperatureData = new
//            {
//                round_id = this.roundId,
//                temperature_deg = e.TemperatureDeg,
//                event_time = correctedEventTime(e.SessionTime),
//                lsl_timestamp = lsl_timestamp,
//                unix_timestamp = unixTimestamp
//            };
//            Console.WriteLine(e);
//            Console.WriteLine(skinTemperatureData);

//            SaveData("SkinTemperatureData", skinTemperatureData);
//            //double[] lslData = ConvertObjectToDoubleArray(skinTemperatureData);

//            //this._lsl.PushSample(skinTemperatureOutlet, lslData);
//            //this._lsl.PushSampleWithTimestamp(skinTemperatureOutlet, lslData, lslCorrectedTimestamp);

//            string jsonData = JsonConvert.SerializeObject(skinTemperatureData);
//            Console.WriteLine(jsonData);
//            this._lsl.PushSerializedSampleWithTimestamp(skinTemperatureOutlet, jsonData, lslCorrectedTimestamp);

//        }
//        public void DeviceGSRDataReceived(object sender, GalvanicSkinResistanceEventArgs e)
//        {
//            double lsl_timestamp = _lsl.GetLSLTimestampNow();
//            double unixTimestamp = _lsl.GetUnixTimestampNow();
//            double lslCorrectedTimestamp = _lsl.ConvertToLSLTime(unixTimestamp);

//            object gsrData = new
//            {
//                round_id = this.roundId,
//                raw_adc_reading = e.Reading,
//                micro_siemens_reading = e.Reading100MicroSiemens,
//                event_time = correctedEventTime(e.SessionTime),
//                lsl_timestamp = lsl_timestamp,
//                unix_timestamp = unixTimestamp
//            };
//            Console.WriteLine(e);
//            Console.WriteLine(gsrData);

//            SaveData("GSRData", gsrData);
//            //double[] lslData = ConvertObjectToDoubleArray(gsrData);

//            //this._lsl.PushSample(gsrOutlet, lslData);
//            //this._lsl.PushSampleWithTimestamp(gsrOutlet, lslData, lslCorrectedTimestamp);

//            string jsonData = JsonConvert.SerializeObject(gsrData);
//            Console.WriteLine(jsonData);
//            this._lsl.PushSerializedSampleWithTimestamp(skinTemperatureOutlet, jsonData, lslCorrectedTimestamp);

//        }

//        public void DeviceRawDataReceived(object sender, SemMessageEventArgs e)
//        {
//            Console.WriteLine(e);
//            SaveData("RawData", e);


//        }

//        public void SaveData(string dataType, object data)
//        {
//            DateTime date = DateTime.UtcNow;
//            // Define the log file path
//            string logFilePath = $@"C:\EquivitalData\EquivitalLog-{date:yyyy-MM-dd-HH}.txt";
//            Directory.CreateDirectory(Path.GetDirectoryName(logFilePath)); // Ensure directory exists

//            // Append data to the file
//            using (StreamWriter sw = new StreamWriter(logFilePath, true))
//            {
//                sw.WriteLine($"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} | {dataType}: {data}");
//            }

//            Console.WriteLine($"Data appended to {logFilePath}");

//            string tableName = null;

//            switch (dataType)
//            {
//                case "ECGData":
//                    tableName = "ecg_data";
//                    break;
//                case "HeartRateData":
//                    tableName = "heart_rate_data";
//                    break;
//                case "AccelerometerData":
//                    tableName = "accelerometer_data";
//                    break;
//                case "RespirationRateData":
//                    tableName = "respiration_rate_data";
//                    break;
//                case "ImpedanceRespirationData":
//                    tableName = "impedance_respiration_data";
//                    break;
//                case "SkinTemperatureData":
//                    tableName = "skin_temperature_data";
//                    break;
//                case "GSRData":
//                    tableName = "gsr_data";
//                    break;
//                default:
//                    break;
//            }
//            // Save to PostgreSQL database
//            try
//            {
//                if (tableName != null)
//                {
//                    _dbManager.InsertData(tableName, data); // Access _dbManager directly //TODO:update schema
//                    Console.WriteLine("Data saved to database successfully.");
//                }

//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Error saving to database: {ex.Message}");
//            }
//        }

//        static double correctedEventTime(DateTime roundTime)
//        {
//            Console.WriteLine($"roundTime: {roundTime}");
//            //DateTime roundStartTime = DateTime.UtcNow; // Approximate round start time
//            double timestamp = (roundTime - new DateTime(1970, 1, 1)).TotalSeconds; //unix
//            //Console.WriteLine($"roundStartTime {roundStartTime}, roundTime: {roundTime}, timestamp: {timestamp}");
//            Console.WriteLine($"roundTime: {roundTime}, timestamp: {timestamp}");

//            return timestamp;
//        }

//        public static string GenerateCustomRoundId(string userRoundId)
//        {
//            // Validate the input round ID (e.g., ensure it's not null/empty)
//            if (string.IsNullOrWhiteSpace(userRoundId))
//            {
//                throw new ArgumentException("Round ID cannot be null or empty.");
//            }

//            // Get the current timestamp in a compact format (e.g., YYYYMMDDHHMMSS)
//            string timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");


//            // Combine the user-provided round ID and timestamp
//            string customRoundId = $"{userRoundId}{timestamp}";

//            return customRoundId;
//        }


//        //public static double ConvertTimeToDouble(DateTime time)
//        //{
//        //    DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

//        //    TimeSpan timeSpan = time - unixEpoch;
//        //    double timestamp = timeSpan.TotalSeconds;
//        //    return timestamp;
//        //}

//        // Example of converting DateTime to a double (could be epoch time or any other conversion)
//        public static double ConvertTimeToDouble(DateTime time)
//        {
//            // Example: Convert DateTime to Unix timestamp (seconds since 1970-01-01)
//            return (time - new DateTime(1970, 1, 1)).TotalSeconds;
//        }


//        // Function to convert an object to a double array
//        public static double[] ConvertObjectToDoubleArray(object obj)
//        {
//            // Get the properties of the object
//            PropertyInfo[] properties = obj.GetType().GetProperties();

//            // Create a list to store the converted values
//            var doubleValues = new System.Collections.Generic.List<double>();

//            // Loop through each property
//            foreach (var property in properties)
//            {
//                // Check if the property can be converted to a double
//                if (property.GetValue(obj) is IConvertible)
//                {
//                    var value = property.GetValue(obj);
//                    if (value is DateTime)
//                    {
//                        // Custom logic to handle DateTime (or any other special cases)
//                        doubleValues.Add(ConvertTimeToDouble((DateTime)value));
//                    }
//                    else
//                    {
//                        // Convert the value to double
//                        doubleValues.Add(Convert.ToDouble(value));
//                    }
//                }
//            }

//            // Return the double array
//            return doubleValues.ToArray();
//        }


//    }
//}


//======================================= new program start=====

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

            //_config = new Config();
            //LSL outlets

            //Environment.SetEnvironmentVariable("LSL_LISTEN_ADDRESS", "127.0.0.1", EnvironmentVariableTarget.Process);

            //string samples
            //ecgStreamInfo = this._lsl.CreateECGStream();


            ////ecgStreamInfo = this._lsl.CreateECGStream();
            //hrStreamInfo = this._lsl.CreateHRStream();
            //accelerometerStreamInfo = this._lsl.CreateAccelerometerStream();
            //respirationRateStreamInfo = this._lsl.CreateRRStream();
            //impedanceRespirationStreamInfo = this._lsl.CreateIRStream();
            //skinTemperatureStreamInfo = this._lsl.CreateSkinTempStream();
            //gsrStreamInfo = this._lsl.CreateGSRStream();


            //ecgOutlet = this._lsl.CreateOutlet(ecgStreamInfo);
            //hrOutlet = this._lsl.CreateOutlet(hrStreamInfo);
            //accelerometerOutlet = this._lsl.CreateOutlet(accelerometerStreamInfo);
            //respirationRateOutlet = this._lsl.CreateOutlet(respirationRateStreamInfo);
            //impedanceRespirationOutlet = this._lsl.CreateOutlet(impedanceRespirationStreamInfo);
            //skinTemperatureOutlet = this._lsl.CreateOutlet(skinTemperatureStreamInfo);
            //gsrOutlet = this._lsl.CreateOutlet(gsrStreamInfo);

        }

        //static void Main(string[] args)
        //{
        //    Config.LoadEnvVariables("..\\..\\.env");

        //    string devName = Environment.GetEnvironmentVariable("EQ_DEV_NAME");
        //    string licenseKey = Environment.GetEnvironmentVariable("EQ_LICENSE_KEY");
        //    string pinCode = Environment.GetEnvironmentVariable("EQ_DONGLE_PIN");

        //    if (string.IsNullOrWhiteSpace(devName) || string.IsNullOrWhiteSpace(licenseKey))
        //    {
        //        Console.WriteLine("Missing Equivital credentials.");
        //        return;
        //    }

        //    Program program = new Program();
        //    EquivitalService equivitalService = new EquivitalService(devName, licenseKey, pinCode);

        //    Console.Write("Enter Round ID: ");
        //    string roundID = Console.ReadLine();
        //    Console.Write("Enter Player ID: ");
        //    string playerID = Console.ReadLine();

        //    string NewRoundID = GenerateCustomRoundId(roundID);
        //    program.roundId = NewRoundID; // Assign new Round ID

        //    Console.WriteLine($"Custom Round ID: {NewRoundID}");

        //    // Start the Equivital Dongle Manager
        //    equivitalService.StartDongleManager();

        //    try
        //    {
        //        var roundObj = new Dictionary<string, object>
        //    {
        //        { "round_id", NewRoundID },
        //        { "player_id", playerID },
        //        { "start_time", program._lsl.GetUnixTimestampNow() }
        //    };

        //        program.createRound(roundObj);

        //        Console.WriteLine("Press 'q' to stop the experiment...");
        //        while (Console.ReadLine()?.ToLowerInvariant() != "q") { }

        //        // End round
        //        program.updateRound(program.roundId);
        //        Console.WriteLine("Round ended, end time updated.");
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine($"Error during execution: {e.Message}");
        //    }
        //    finally
        //    {
        //        // Stop the Equivital Dongle Manager at the end of the experiment
        //        equivitalService.StopDongleManager();
        //        Console.WriteLine("Data collection done.");
        //    }

        //    Console.ReadLine();
        //}

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
                //    Console.Write("Enter Round ID: ");
                //    string roundID = Console.ReadLine();
                //    Console.Write("Enter Player ID: ");
                //    string playerID = Console.ReadLine();

                //    string NewRoundID = GenerateCustomRoundId(roundID);
                //    program.roundId = NewRoundID; // Assign new Round ID

                //    Console.WriteLine($"Custom Round ID: {NewRoundID}");

                //    var roundObj = new Dictionary<string, object>
                //{
                //    { "round_id", NewRoundID },
                //    { "player_id", playerID },
                //    { "start_time", program._lsl.GetUnixTimestampNow() }
                //};

                //    program.createRound(roundObj);

                //Console.WriteLine("Press 'q' to stop the experiment...");
                //while (Console.ReadLine()?.ToLowerInvariant() != "q") { }

                //// End round
                //program.updateRound(program.roundId);
                //Console.WriteLine("Round ended, end time updated.");
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
        private static void ConnectionStateChangedHandler(object sender, SemDeviceConnectionStateChangedEventArgs e)
        {
            Console.WriteLine($"Connection state changed: {e.PreviousState} -> {e.State}");
        }

        public void createRound(object roundObj)
        {
            _dbManager.InsertData("rounds", roundObj);
        }

        public void updateRound(string roundId)
        {

            // Data to update
            var updateValues = new
            {
                end_time = this._lsl.GetUnixTimestampNow() // round end timestamp
            };

            var whereConditions = new Dictionary<string, object>
                    {
                        { "round_id", roundId }
                    };

            // Execute update
            _dbManager.UpdateData("rounds", updateValues, whereConditions);
        }


        static List<EquivitalBluetoothSensorInfo> _foundSensors = new List<EquivitalBluetoothSensorInfo>();
        static void DiscoverySensorDiscoveryComplete(object sender, EventArgs e)
        {
            // When discovery is complete tell the user
            int foundSensorCount = _foundSensors.Count;
            Console.WriteLine("Discovery complete. {0} SEMs were found", foundSensorCount);

            for (int i = 0; i < foundSensorCount; i++)
            {
                Console.WriteLine("[{0}] {1}", i, _foundSensors[i].Name);
            }

            Console.WriteLine("Please press enter to connect to the SEM");

            //if (isMultDongles)
            //{
            //    Console.WriteLine("Please enter the index of the SEM that you would like to connect to");
            //}
        }

        static void DiscoveryEquivitalSensorFound(object sender, EquivitalBluetoothInfoEventArgs e)
        {
            // The discovery will return a EquivitalBluetoothSensorInfo which includes information about the discovered SEM.
            Console.WriteLine("Found SEM {0}", e.Info.Name);

            if (!_foundSensors.Any(sem => sem.Address == e.Info.Address))
            {
                _foundSensors.Add(e.Info);
            }
        }

        public void DeviceConnectionRemoved(object sender, SemConnectionEventArgs e)
        {
            Console.WriteLine("A SEM has disconnected");
        }

        public void DeviceConnectionAdded(object sender, SemConnectionEventArgs e)
        {
            try
            {
                Console.WriteLine("A new SEM is connected");

                // Create a decoder instance.
                SemDevice device = new SemDevice();
                device.SetOperatingMode((SemOperatingModeType)DisclosureMode.Full);

                // We're interested in heart rate and timer messages.
                device.HeartRateDataReceived += this.DeviceHeartRateDataReceived;
                device.ECGDataReceived += this.DeviceECGDataReceived;
                device.AccelerometerDataReceived += this.DeviceAccelerometerDataReceived;
                device.BeltRespirationRateDataReceived += this.DeviceRespirationRateDataReceived;
                device.SkinTemperatureDataReceived += this.DeviceSkinTemperatureDataReceived;
                device.ImpedanceRespirationDataReceived += this.DeviceImpedanceRespirationDataReceived; //TODO: not recieved in data
                device.GalvanicSkinResistanceDataReceived += this.DeviceGSRDataReceived;
                device.RawDataReceived += this.DeviceRawDataReceived;
                device.SynchronisationTimerDataReceived += this.DeviceSynchronisationTimerDataReceived;

                // Start the decoder.
                Console.WriteLine("Press any key to terminate the application.\r\n");
                // Start the decoder.
                device.Start(e.Connection);
            }
            catch (BadLicenseException)
            {
                Console.WriteLine("Please enter a valid developer name and license code!");
            }
        }


        public void DeviceSynchronisationTimerDataReceived(object sender, SyncrhonisationTimerEventArgs e)
        {
            double roundTime = correctedEventTime(e.SessionTime); // Assuming e.SessionTime is a DateTime with 1970 default
                                                                  //DateTime currentDateWithRoundTime = DateTime.Today.Add(roundTime.TimeOfDay);

            DateTime roundStartTime = DateTime.UtcNow; // Actual round start time
            Console.WriteLine(e.SessionTime);
            DateTime currentDateWithRoundTime = DateTime.Today.Add(roundStartTime.TimeOfDay);
            Console.WriteLine(currentDateWithRoundTime.ToString("yyyy-MM-dd HH:mm:ss"));

            Console.WriteLine(e + " (" + currentDateWithRoundTime.ToString("yyyy-MM-dd HH:mm:ss") + ")");
            SaveData("SynchronisationTimerData", e + " (" + currentDateWithRoundTime.ToString("yyyy-MM-dd HH:mm:ss") + ")");

        }

        public void DeviceHeartRateDataReceived(object sender, HeartRateEventArgs e)
        {
            double lsl_timestamp = _lsl.GetLSLTimestampNow();
            double unixTimestamp = _lsl.GetUnixTimestampNow();
            double lslCorrectedTimestamp = _lsl.ConvertToLSLTime(unixTimestamp);

            object heartRateData = new
            {
                round_id = this.roundId,
                hr_bpm = e.BeatsPerMinute,
                event_time = correctedEventTime(e.SessionTime),
                lsl_timestamp = lsl_timestamp,
                unix_timestamp = unixTimestamp
            };
            Console.WriteLine(e);
            Console.WriteLine(heartRateData);

            //double[] lslData = ConvertObjectToDoubleArray(heartRateData);
            // Now push to LSL outlet (No need to assign the return value, as PushSample() is void)
            Console.WriteLine("lsls data hr: ", heartRateData);

            //this._lsl.PushSample(hrOutlet, lslData);
            //this._lsl.PushSampleWithTimestamp(hrOutlet, lslData, lslCorrectedTimestamp);

            string jsonData = JsonConvert.SerializeObject(heartRateData);
            Console.WriteLine(jsonData);
            //this._lsl.PushSerializedSampleWithTimestamp(hrOutlet, jsonData, lslCorrectedTimestamp);

            SaveData("HeartRateData", heartRateData);

        }

        public void DeviceECGDataReceived(object sender, ECGSemMessageEventArgs e)
        {

            double lsl_timestamp = _lsl.GetLSLTimestampNow();
            double unixTimestamp = _lsl.GetUnixTimestampNow();
            double lslCorrectedTimestamp = _lsl.ConvertToLSLTime(unixTimestamp);

            object ECGData = new
            {
                round_id = this.roundId,
                lead_one_raw = e.LeadOneRaw,
                lead_two_raw = e.LeadTwoRaw,
                sequence_number = e.SequenceNumber,
                lead_one_mv = e.LeadOne_mV,
                lead_two_mv = e.LeadTwo_mV,
                event_time = correctedEventTime(e.SessionTime), //round_time
                lsl_timestamp = lsl_timestamp,
                unix_timestamp = unixTimestamp
            };
            Console.WriteLine(e);
            Console.WriteLine(ECGData);

            SaveData("ECGData", ECGData);
            //double[] lslData = ConvertObjectToDoubleArray(ECGData);

            //this._lsl.PushSample(ecgOutlet, lslData);
            //this._lsl.PushSampleWithTimestamp(ecgOutlet, lslData, lslCorrectedTimestamp);

            string jsonData = JsonConvert.SerializeObject(ECGData);
            Console.WriteLine(jsonData);
            //this._lsl.PushSerializedSampleWithTimestamp(ecgOutlet, jsonData, lslCorrectedTimestamp);

        }

        public void DeviceAccelerometerDataReceived(object sender, AccelerometerSemMessageEventArgs e)
        {

            double lsl_timestamp = _lsl.GetLSLTimestampNow();
            double unixTimestamp = _lsl.GetUnixTimestampNow();
            double lslCorrectedTimestamp = _lsl.ConvertToLSLTime(unixTimestamp);

            object accelerometerData = new
            {
                round_id = this.roundId,
                vertical_mg = e.Vertical_mG,
                lateral_mg = e.Lateral_mG,
                longitudinal_mg = e.Longitudinal_mG,
                resultant_mg = e.Resultant_mG,
                vertical_raw = e.VerticalRaw,
                lateral_raw = e.LateralRaw,
                longitudinal_raw = e.LongitudinalRaw,
                event_time = correctedEventTime(e.SessionTime),
                lsl_timestamp = lsl_timestamp,
                unix_timestamp = unixTimestamp
            };
            Console.WriteLine(e);
            Console.WriteLine(accelerometerData);

            SaveData("AccelerometerData", accelerometerData);
            //double[] lslData = ConvertObjectToDoubleArray(accelerometerData);


            //this._lsl.PushSample(accelerometerOutlet, lslData);
            //this._lsl.PushSampleWithTimestamp(accelerometerOutlet, lslData, lslCorrectedTimestamp);

            string jsonData = JsonConvert.SerializeObject(accelerometerData);
            Console.WriteLine(jsonData);
            //this._lsl.PushSerializedSampleWithTimestamp(accelerometerOutlet, jsonData, lslCorrectedTimestamp);
        }

        public void DeviceRespirationRateDataReceived(object sender, RespirationRateEventArgs e)
        {
            double lsl_timestamp = _lsl.GetLSLTimestampNow();
            double unixTimestamp = _lsl.GetUnixTimestampNow();
            double lslCorrectedTimestamp = _lsl.ConvertToLSLTime(unixTimestamp);

            object respirationRateData = new
            {
                round_id = this.roundId,
                breaths_per_minute = e.BreathsPerMinute,
                event_time = correctedEventTime(e.SessionTime),
                lsl_timestamp = lsl_timestamp,
                unix_timestamp = unixTimestamp
            };
            Console.WriteLine(e);
            Console.WriteLine(respirationRateData);

            SaveData("RespirationRateData", respirationRateData);
            //double[] lslData = ConvertObjectToDoubleArray(respirationRateData);

            //this._lsl.PushSample(respirationRateOutlet, lslData);
            //this._lsl.PushSampleWithTimestamp(respirationRateOutlet, lslData, lslCorrectedTimestamp);

            string jsonData = JsonConvert.SerializeObject(respirationRateData);
            Console.WriteLine(jsonData);
            //this._lsl.PushSerializedSampleWithTimestamp(respirationRateOutlet, jsonData, lslCorrectedTimestamp);

        }

        public void DeviceImpedanceRespirationDataReceived(object sender, ImpedanceRespirationEventArgs e)
        {

            double lsl_timestamp = _lsl.GetLSLTimestampNow();
            double unixTimestamp = _lsl.GetUnixTimestampNow();
            double lslCorrectedTimestamp = _lsl.ConvertToLSLTime(unixTimestamp);

            object impedanceRespirationData = new
            {
                round_id = this.roundId,
                impedance = e.Impedance,
                event_time = correctedEventTime(e.SessionTime),
                lsl_timestamp = lsl_timestamp,
                unix_timestamp = unixTimestamp
            };
            Console.WriteLine(e);
            Console.WriteLine(impedanceRespirationData);

            SaveData("ImpedanceRespirationData", impedanceRespirationData);
            //double[] lslData = ConvertObjectToDoubleArray(impedanceRespirationData);

            //this._lsl.PushSample(impedanceRespirationOutlet, lslData);
            //this._lsl.PushSampleWithTimestamp(impedanceRespirationOutlet, lslData, lslCorrectedTimestamp);

            string jsonData = JsonConvert.SerializeObject(impedanceRespirationData);
            Console.WriteLine(jsonData);
            //this._lsl.PushSerializedSampleWithTimestamp(impedanceRespirationOutlet, jsonData, lslCorrectedTimestamp);

        }

        public void DeviceSkinTemperatureDataReceived(object sender, SkinTemperatureEventArgs e)
        {
            double lsl_timestamp = _lsl.GetLSLTimestampNow();
            double unixTimestamp = _lsl.GetUnixTimestampNow();
            double lslCorrectedTimestamp = _lsl.ConvertToLSLTime(unixTimestamp);

            object skinTemperatureData = new
            {
                round_id = this.roundId,
                temperature_deg = e.TemperatureDeg,
                event_time = correctedEventTime(e.SessionTime),
                lsl_timestamp = lsl_timestamp,
                unix_timestamp = unixTimestamp
            };
            Console.WriteLine(e);
            Console.WriteLine(skinTemperatureData);

            SaveData("SkinTemperatureData", skinTemperatureData);
            //double[] lslData = ConvertObjectToDoubleArray(skinTemperatureData);

            //this._lsl.PushSample(skinTemperatureOutlet, lslData);
            //this._lsl.PushSampleWithTimestamp(skinTemperatureOutlet, lslData, lslCorrectedTimestamp);

            string jsonData = JsonConvert.SerializeObject(skinTemperatureData);
            Console.WriteLine(jsonData);
            //this._lsl.PushSerializedSampleWithTimestamp(skinTemperatureOutlet, jsonData, lslCorrectedTimestamp);

        }
        public void DeviceGSRDataReceived(object sender, GalvanicSkinResistanceEventArgs e)
        {
            double lsl_timestamp = _lsl.GetLSLTimestampNow();
            double unixTimestamp = _lsl.GetUnixTimestampNow();
            double lslCorrectedTimestamp = _lsl.ConvertToLSLTime(unixTimestamp);

            object gsrData = new
            {
                round_id = this.roundId,
                raw_adc_reading = e.Reading,
                micro_siemens_reading = e.Reading100MicroSiemens,
                event_time = correctedEventTime(e.SessionTime),
                lsl_timestamp = lsl_timestamp,
                unix_timestamp = unixTimestamp
            };
            Console.WriteLine(e);
            Console.WriteLine(gsrData);

            SaveData("GSRData", gsrData);
            //double[] lslData = ConvertObjectToDoubleArray(gsrData);

            //this._lsl.PushSample(gsrOutlet, lslData);
            //this._lsl.PushSampleWithTimestamp(gsrOutlet, lslData, lslCorrectedTimestamp);

            string jsonData = JsonConvert.SerializeObject(gsrData);
            Console.WriteLine(jsonData);
            //this._lsl.PushSerializedSampleWithTimestamp(skinTemperatureOutlet, jsonData, lslCorrectedTimestamp);

        }

        public void DeviceRawDataReceived(object sender, SemMessageEventArgs e)
        {
            Console.WriteLine(e);
            SaveData("RawData", e);


        }

        public void SaveData(string dataType, object data)
        {
            DateTime date = DateTime.UtcNow;
            // Define the log file path
            string logFilePath = $@"C:\EquivitalData\EquivitalLog-{date:yyyy-MM-dd-HH}.txt";
            Directory.CreateDirectory(Path.GetDirectoryName(logFilePath)); // Ensure directory exists

            // Append data to the file
            using (StreamWriter sw = new StreamWriter(logFilePath, true))
            {
                sw.WriteLine($"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} | {dataType}: {data}");
            }

            Console.WriteLine($"Data appended to {logFilePath}");

            string tableName = null;

            switch (dataType)
            {
                case "ECGData":
                    tableName = "ecg_data";
                    break;
                case "HeartRateData":
                    tableName = "heart_rate_data";
                    break;
                case "AccelerometerData":
                    tableName = "accelerometer_data";
                    break;
                case "RespirationRateData":
                    tableName = "respiration_rate_data";
                    break;
                case "ImpedanceRespirationData":
                    tableName = "impedance_respiration_data";
                    break;
                case "SkinTemperatureData":
                    tableName = "skin_temperature_data";
                    break;
                case "GSRData":
                    tableName = "gsr_data";
                    break;
                default:
                    break;
            }
            // Save to PostgreSQL database
            try
            {
                if (tableName != null)
                {
                    _dbManager.InsertData(tableName, data); // Access _dbManager directly //TODO:update schema
                    Console.WriteLine("Data saved to database successfully.");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving to database: {ex.Message}");
            }
        }

        static double correctedEventTime(DateTime roundTime)
        {
            Console.WriteLine($"roundTime: {roundTime}");
            //DateTime roundStartTime = DateTime.UtcNow; // Approximate round start time
            double timestamp = (roundTime - new DateTime(1970, 1, 1)).TotalSeconds; //unix
                                                                                    //Console.WriteLine($"roundStartTime {roundStartTime}, roundTime: {roundTime}, timestamp: {timestamp}");
            Console.WriteLine($"roundTime: {roundTime}, timestamp: {timestamp}");

            return timestamp;
        }

        public static string GenerateCustomRoundId(string userRoundId)
        {
            // Validate the input round ID (e.g., ensure it's not null/empty)
            if (string.IsNullOrWhiteSpace(userRoundId))
            {
                throw new ArgumentException("Round ID cannot be null or empty.");
            }

            // Get the current timestamp in a compact format (e.g., YYYYMMDDHHMMSS)
            string timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");


            // Combine the user-provided round ID and timestamp
            string customRoundId = $"{userRoundId}{timestamp}";

            return customRoundId;
        }


        //public static double ConvertTimeToDouble(DateTime time)
        //{
        //    DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        //    TimeSpan timeSpan = time - unixEpoch;
        //    double timestamp = timeSpan.TotalSeconds;
        //    return timestamp;
        //}

        // Example of converting DateTime to a double (could be epoch time or any other conversion)
        public static double ConvertTimeToDouble(DateTime time)
        {
            // Example: Convert DateTime to Unix timestamp (seconds since 1970-01-01)
            return (time - new DateTime(1970, 1, 1)).TotalSeconds;
        }


        // Function to convert an object to a double array
        public static double[] ConvertObjectToDoubleArray(object obj)
        {
            // Get the properties of the object
            PropertyInfo[] properties = obj.GetType().GetProperties();

            // Create a list to store the converted values
            var doubleValues = new System.Collections.Generic.List<double>();

            // Loop through each property
            foreach (var property in properties)
            {
                // Check if the property can be converted to a double
                if (property.GetValue(obj) is IConvertible)
                {
                    var value = property.GetValue(obj);
                    if (value is DateTime)
                    {
                        // Custom logic to handle DateTime (or any other special cases)
                        doubleValues.Add(ConvertTimeToDouble((DateTime)value));
                    }
                    else
                    {
                        // Convert the value to double
                        doubleValues.Add(Convert.ToDouble(value));
                    }
                }
            }

            // Return the double array
            return doubleValues.ToArray();
        }
    }


}
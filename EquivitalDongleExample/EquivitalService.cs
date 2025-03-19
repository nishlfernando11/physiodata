using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.ServiceProcess;
using System.Threading;
using ECGDataStream;
using Equivital.DongleExtension;
using Newtonsoft.Json;
using SemParserLibrary;
using static SemParserLibrary.SemDevice;
using System.Linq;

namespace ECGDataStream
{
    public class EquivitalService
    {
        private EquivitalDongleManager _manager;
        private bool _ednWasRunning;
        private string _devName;
        private string _licenseKey;
        private string _pinCode;

        private readonly DatabaseManager _dbManager;
        private readonly LSLWrapper _lsl;
        //public double roundId;
        public string roundId;
        private bool _isCollecting;
        private SemDevice device;
        private ISemConnection semConnection;

        private IntPtr ecgStreamInfo;
        private IntPtr hrStreamInfo;
        private IntPtr accelerometerStreamInfo;
        private IntPtr respirationRateStreamInfo;
        private IntPtr impedanceRespirationStreamInfo;
        private IntPtr skinTemperatureStreamInfo;
        private IntPtr gsrStreamInfo;

        private IntPtr ecgOutlet;
        private IntPtr hrOutlet;
        private IntPtr accelerometerOutlet;
        private IntPtr respirationRateOutlet;
        private IntPtr impedanceRespirationOutlet;
        private IntPtr skinTemperatureOutlet;
        private IntPtr gsrOutlet;


        public EquivitalService(string devName, string licenseKey, string pinCode)
        {
            _devName = devName;
            _licenseKey = licenseKey;
            _pinCode = pinCode;
            _dbManager = new DatabaseManager();
            _lsl = new LSLWrapper();


            ecgStreamInfo = this._lsl.CreateECGStream();
            hrStreamInfo = this._lsl.CreateHRStream();
            accelerometerStreamInfo = this._lsl.CreateAccelerometerStream();
            respirationRateStreamInfo = this._lsl.CreateRRStream();
            impedanceRespirationStreamInfo = this._lsl.CreateIRStream();
            skinTemperatureStreamInfo = this._lsl.CreateSkinTempStream();
            gsrStreamInfo = this._lsl.CreateGSRStream();


            ecgOutlet = this._lsl.CreateOutlet(ecgStreamInfo);
            hrOutlet = this._lsl.CreateOutlet(hrStreamInfo);
            accelerometerOutlet = this._lsl.CreateOutlet(accelerometerStreamInfo);
            respirationRateOutlet = this._lsl.CreateOutlet(respirationRateStreamInfo);
            impedanceRespirationOutlet = this._lsl.CreateOutlet(impedanceRespirationStreamInfo);
            skinTemperatureOutlet = this._lsl.CreateOutlet(skinTemperatureStreamInfo);
            gsrOutlet = this._lsl.CreateOutlet(gsrStreamInfo);

        }

        public void StartDongleManager()
        {
            try
            {
                // Register SDK
                SemDevice.License.DeveloperName = _devName;
                SemDevice.License.LicenseCode = _licenseKey;
                device = new SemDevice();

                StopEdnService();

                Console.WriteLine("Starting Equivital Dongle Manager...");
                _manager = EquivitalDongleManager.Instance;
                _manager.ConnectionAdded += DeviceConnectionAdded;
                _manager.ConnectionRemoved += DeviceConnectionRemoved;
                _manager.Start();

                Console.WriteLine("Waiting for a dongle to connect...");
                while (_manager.ConnectedIWrapDeviceCount == 0)
                {
                    Console.WriteLine("Connect an Equivital Dongle and press enter. Enter again if unsuccessful.");

                    if (Console.ReadLine().ToLowerInvariant() == "q")
                    {
                        return;
                    }
                }

                Console.WriteLine($"Dongle connected. Total connected devices: {_manager.ConnectedIWrapDeviceCount}");
                StartBluetoothDiscovery();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error starting Equivital service: {e.Message}");
            }
        }

        public void StopDongleManager()
        {
            try
            {
                if (_manager != null)
                {
                    _manager.Terminate();
                    Console.WriteLine("Equivital Dongle Manager terminated.");
                }

                RestartEdnService();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error stopping Equivital service: {e.Message}");
            }
        }

        private void StartBluetoothDiscovery()
        {
            Console.WriteLine("Starting Bluetooth discovery...");
            _manager.EquivitalSensorFound += DiscoveryEquivitalSensorFound;
            _manager.SensorDiscoveryComplete += new EventHandler(DiscoverySensorDiscoveryComplete);// DiscoverySensorDiscoveryComplete;
            _manager.DiscoverDevicesAsync();

            Console.WriteLine("Bluetooth discovery started");

            string userInput = null;
            int indexTonnectTo = 0;

            userInput = Console.ReadLine().ToLowerInvariant();

            if (_foundSensors.Count > indexTonnectTo && indexTonnectTo >= 0)
            {
                EquivitalBluetoothSensorInfo sensorInfo = _foundSensors[indexTonnectTo];
                sensorInfo.PinCode = _pinCode;
                _manager.AddBluetoothConnectableDevice(sensorInfo);
            }
            
        }

        private void StopEdnService()
        {
            try
            {
                ServiceController ednService = new ServiceController("EDN Node Service");
                _ednWasRunning = ednService.Status == ServiceControllerStatus.Running;

                if (_ednWasRunning)
                {
                    Console.WriteLine("Stopping EDN service...");
                    ednService.Stop();
                    ednService.WaitForStatus(ServiceControllerStatus.Stopped);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error stopping EDN service: {e.Message}");
            }
        }

        private void RestartEdnService()
        {
            if (_ednWasRunning)
            {
                try
                {
                    Console.WriteLine("Restarting EDN service...");
                    ServiceController ednService = new ServiceController("EDN Node Service");
                    ednService.Start();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error restarting EDN service: {e.Message}");
                }
            }
        }

        public void StartDataCollection()
        {
            if (!_isCollecting)
            {
                Console.WriteLine("Adding player round data");
                AddPlayer();
                Console.WriteLine("Starting ECG Data Collection...");
                _isCollecting = true;
                device.Start(semConnection);

                //// Example of background ECG data collection
                //Thread ecgThread = new Thread(device.Start(semConnection));
                //ecgThread.Start();

            }
        }

        public void AddPlayer()
        {
            Console.Write("Enter Round ID: ");
            string roundID = Console.ReadLine();
            Console.Write("Enter Player ID: ");
            string playerID = Console.ReadLine();

            string NewRoundID = GenerateCustomRoundId(roundID, playerID);
            this.roundId = NewRoundID; // Assign new Round ID

            Console.WriteLine($"Custom Round ID: {NewRoundID}");

            var roundObj = new Dictionary<string, object>
            {
                { "round_id", NewRoundID },
                { "player_id", playerID },
                { "start_time", this._lsl.GetUnixTimestampNow() }
            };
            this.createRound(roundObj);

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

        public void StopDataCollection()
        {
            Console.WriteLine("Stopping ECG Data Collection...");
            device.Stop(true);

            //StopDongleManager();
            _isCollecting = false;
            // End round
            this.updateRound(this.roundId);
            Console.WriteLine("\n\nRound ended, end time updated.\n\n");

        }

        public void QuitExperiment()
        {
            Console.WriteLine("Enter 'q' to confirm quit experiment.");

            string userInput = Console.ReadLine()?.ToLowerInvariant();
            while (userInput == "q")
            {
                // Stop the Equivital Dongle Manager at the end of the experiment
                StopDongleManager();
                Console.WriteLine("Data collection done.");
                Console.WriteLine("Exiting application...");
                Environment.Exit(0); // Forcefully exits the console application
            }
        }

        private void DeviceConnectionAdded(object sender, SemConnectionEventArgs e)
        {
            //Console.WriteLine("A new SEM device is connected.");
            try
            {

                // Create a decoder instance.
                device.SetOperatingMode((SemOperatingModeType)DisclosureMode.Full);

                // We're interested in heart rate and timer messages.
                device.HeartRateDataReceived += DeviceHeartRateDataReceived;
                device.ECGDataReceived += DeviceECGDataReceived;
                device.AccelerometerDataReceived += DeviceAccelerometerDataReceived;
                device.BeltRespirationRateDataReceived += DeviceRespirationRateDataReceived;
                device.SkinTemperatureDataReceived += DeviceSkinTemperatureDataReceived;
                device.ImpedanceRespirationDataReceived += DeviceImpedanceRespirationDataReceived; //TODO: not recieved in data
                device.GalvanicSkinResistanceDataReceived += DeviceGSRDataReceived;
                device.RawDataReceived += DeviceRawDataReceived;
                device.SynchronisationTimerDataReceived += DeviceSynchronisationTimerDataReceived;

                semConnection = e.Connection;
            }
            catch (BadLicenseException)
            {
                Console.WriteLine("Please enter a valid developer name and license code!");
            }
            //DeviceStartDataCollection(sender, e);
        }

        private void DeviceConnectionRemoved(object sender, SemConnectionEventArgs e)
        {
            Console.WriteLine("A SEM device has disconnected.");
        }

        static List<EquivitalBluetoothSensorInfo> _foundSensors = new List<EquivitalBluetoothSensorInfo>();

        static void DiscoverySensorDiscoveryComplete(object sender, EventArgs e)
        {
            // When discovery is complete tell the user
            int foundSensorCount = _foundSensors.Count;
            Console.WriteLine("Bluetooth Discovery complete. {0} SEMs were found", foundSensorCount);

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

        public void DeviceStartDataCollection(object sender, SemConnectionEventArgs e)
        {

            // Start the decoder.
            Console.WriteLine("Data collection started.");
            Console.WriteLine("Press 'X' to stop round {0} data collection.", roundId);

            // Start the decoder.
            device.Start(e.Connection);
           
        }


        public void DeviceSynchronisationTimerDataReceived(object sender, SyncrhonisationTimerEventArgs e)
        {
            double roundTime = correctedEventTime(e.SessionTime); // Assuming e.SessionTime is a DateTime with 1970 default

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

            Console.WriteLine("lsls data hr: ", heartRateData);


            string jsonData = JsonConvert.SerializeObject(heartRateData);
            Console.WriteLine(jsonData);
            this._lsl.PushSerializedSampleWithTimestamp(hrOutlet, jsonData, lslCorrectedTimestamp);

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

            string jsonData = JsonConvert.SerializeObject(ECGData);
            Console.WriteLine(jsonData);
            this._lsl.PushSerializedSampleWithTimestamp(ecgOutlet, jsonData, lslCorrectedTimestamp);

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

            string jsonData = JsonConvert.SerializeObject(accelerometerData);
            Console.WriteLine(jsonData);
            this._lsl.PushSerializedSampleWithTimestamp(accelerometerOutlet, jsonData, lslCorrectedTimestamp);
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

            string jsonData = JsonConvert.SerializeObject(respirationRateData);
            Console.WriteLine(jsonData);
            this._lsl.PushSerializedSampleWithTimestamp(respirationRateOutlet, jsonData, lslCorrectedTimestamp);

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

            string jsonData = JsonConvert.SerializeObject(impedanceRespirationData);
            Console.WriteLine(jsonData);
            this._lsl.PushSerializedSampleWithTimestamp(impedanceRespirationOutlet, jsonData, lslCorrectedTimestamp);

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

            string jsonData = JsonConvert.SerializeObject(skinTemperatureData);
            Console.WriteLine(jsonData);
            this._lsl.PushSerializedSampleWithTimestamp(skinTemperatureOutlet, jsonData, lslCorrectedTimestamp);

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

            string jsonData = JsonConvert.SerializeObject(gsrData);
            Console.WriteLine(jsonData);
            this._lsl.PushSerializedSampleWithTimestamp(skinTemperatureOutlet, jsonData, lslCorrectedTimestamp);

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

        public static string GenerateCustomRoundId(string userRoundId, string playerID)
        {
            // Validate the input round ID (e.g., ensure it's not null/empty)
            if (string.IsNullOrWhiteSpace(userRoundId))
            {
                throw new ArgumentException("Round ID cannot be null or empty.");
            }

            // Get the current timestamp in a compact format (e.g., YYYYMMDDHHMMSS)
            string timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");


            // Combine the user-provided round ID and timestamp
            string customRoundId = $"{playerID}-{userRoundId}-{timestamp}";

            return customRoundId;
        }
               

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
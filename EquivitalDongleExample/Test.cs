//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using SemParserLibrary;
//using System.IO.Ports;

//using LSL;
//using System.Runtime.InteropServices;

//namespace SimpleSerialPortExample
//{
//    class Program
//    {
//        // Declare the P/Invoke function for LSL
//        [DllImport("lsl.dll", CallingConvention = CallingConvention.Cdecl)]
//        public static extern IntPtr lsl_create_streaminfo(string name, string type, int channel_count, double nominal_srate, int channel_format, string source_id);
//        static void Main(string[] args)
//        {
//            try
//            {
//                // Register SDK
//                SemDevice.License.DeveloperName = "Ahmad Abu Alqumsan";
//                SemDevice.License.LicenseCode = "OYftPaoj4od3KGo5h+scee7QkoZZMvXNTkGXFJRWcJWYOzJZiES91qJ3WcsbTkN8aEtKOlGEgQ+V9R63/sK7hVRA6BOgMOrlRagMabpXMRiRyM2/T6GqPTAOx3nj8oPdgU7V/8AS/O+bnlgswOxzcC9ZU/ayQ3bLRvFuM3XAa8s=";


//                // Set up the LSL outlet for streaming data
//                StreamInfo streamInfo = new StreamInfo("Equivital_Sensors", "Physiological", 3, 100, "float32", "EquivitalStream");
//                StreamOutlet outlet = new StreamOutlet(streamInfo);


//                // Output a list of all the serial ports that were detected on the system.
//                Console.WriteLine("DETECTED SERIAL PORTS:");
//                foreach (String thisPort in SerialPort.GetPortNames())
//                {
//                    Console.Write(thisPort + " ");
//                }

//                // Ask the user to enter a port number from the list..
//                Console.Write("\r\n\r\nENTER A PORT NUMBER: ");
//                String port = Console.ReadLine().ToUpper();
//                if (!port.StartsWith("COM")) port = "COM" + port;

//                // Create a real-time "canned data" connection. 
//                ISemConnection serialConnection = SemSerialConnection.CreateConnection(port);

//                if (serialConnection == null)
//                {
//                    throw new ApplicationException("The specified port name was not valid, or the port could not be opened.");
//                }

//                // Create a decoder instance.
//                SemDevice device = new SemDevice();

//                // We're interested in heart rate and timer messages.
//                device.HeartRateDataReceived += DeviceHeartRateDataReceived;
//                device.SynchronisationTimerDataReceived += DeviceSynchronisationTimerDataReceived;

//                // Start the decoder.
//                Console.Clear();
//                Console.WriteLine("Press any key to terminate the application.\r\n");
//                device.Start(serialConnection);


//                // Combine data into a sample (could be a float array or another format)
//                float[] sample = { DeviceHeartRateDataReceived };

//                // Push the data to LSL
//                outlet.PushSample(sample);

//                // Wait briefly to avoid overloading the outlet
//                System.Threading.Thread.Sleep(10);
//            }
//            catch (BadLicenseException)
//            {
//                Console.WriteLine("Please enter a valid developer name and license code!");
//            }
//            catch (Exception e)
//            {
//                Console.WriteLine("An error occurred while executing this example.\r\n" + e.Message);
//            }

//            Console.ReadLine();
//        }

//        static void DeviceSynchronisationTimerDataReceived(object sender, SyncrhonisationTimerEventArgs e)
//        {
//            SemDevice device = sender as SemDevice;
//            Console.WriteLine("[" + device.SensorInfo + "] " + e + " (" + e.SessionTime.ToString("HH:mm:ss") + ")");
//        }

//        static void DeviceHeartRateDataReceived(object sender, HeartRateEventArgs e)
//        {
//            SemDevice device = sender as SemDevice;
//            Console.WriteLine("[" + device.SensorInfo + "] " + e);
//        }
//    }
//}

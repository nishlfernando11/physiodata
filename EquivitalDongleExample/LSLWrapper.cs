using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ECGDataStream
{
    public class LSLWrapper
    {

        // Declare the P/Invoke function for LSL
        [DllImport("lsl.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr lsl_create_streaminfo(string name, string type, int channel_count, double nominal_srate, int channel_format, string source_id);

        [DllImport("lsl.dll", EntryPoint = "lsl_push_sample_d", CallingConvention = CallingConvention.Cdecl)]
        public static extern void lsl_push_sample_d(IntPtr outlet, double[] sample, double timestamp);

        [DllImport("lsl.dll", EntryPoint = "lsl_push_sample_str", CallingConvention = CallingConvention.Cdecl)]
        public static extern void lsl_push_sample_str(IntPtr outlet, string[] sample, double timestamp);

        [DllImport("lsl.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr lsl_create_outlet(IntPtr stream_info, int buffer_size);


        [DllImport("lsl.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr lsl_get_desc(IntPtr stream_info);

        [DllImport("lsl.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern double lsl_local_clock();

        private double lsl_to_unix_offset = 0;

        public LSLWrapper()
        {
            SynchronizeLSLClock();
        }

        private IntPtr CreateStream(string name, string type, int channel_count, double nominal_srate, int channel_format, string source_id)
        {
            // Create a StreamInfo for LSL
            IntPtr streamInfo = lsl_create_streaminfo(name, type, channel_count, nominal_srate, channel_format, source_id);

            // Additional logic for working with LSL (e.g., sending data, streaming)
            Console.WriteLine("StreamInfo created successfully.");
            return streamInfo;
        }
        //

        public IntPtr CreateECGStream()
        {

            string name = "EQ_ECG_Stream", type = "ECG";
            int channel_count = 1;//7;
            double nominal_srate = 256;
            int channel_format = 3;//2;

            //0: float32(32 - bit floating - point numbers) — Most commonly used for sensor data like ECG, HR, etc.
            //1: int32(32 - bit integers)
            //2: double(64 - bit floating - point numbers)
            //3: string


            string source_id = "Equivital";

            return CreateStream(name, type, channel_count, nominal_srate, channel_format, source_id);
        }

        public IntPtr CreateHRStream()
        {
            string name = "EQ_HR_Stream", type = "HR";
            int channel_count = 1;//3;
            double nominal_srate = 0.2;
            int channel_format = 3;// 2;

            //0: float32(32 - bit floating - point numbers) — Most commonly used for sensor data like ECG, HR, etc.
            //1: int32(32 - bit integers)
            //2: double(64 - bit floating - point numbers)
            //3: string


            string source_id = "Equivital";

            return CreateStream(name, type, channel_count, nominal_srate, channel_format, source_id);
        }


        public IntPtr CreateAccelerometerStream()
        {
            string name = "EQ_Accel_Stream", type = "Accel";
            int channel_count = 1;// 9;
            double nominal_srate = 256; // High-res
            int channel_format = 3;// 2;

            //0: float32(32 - bit floating - point numbers) — Most commonly used for sensor data like ECG, HR, etc.
            //1: int32(32 - bit integers)
            //2: double(64 - bit floating - point numbers)
            //3: string

            string source_id = "Equivital";

            return CreateStream(name, type, channel_count, nominal_srate, channel_format, source_id);
        }



        public IntPtr CreateRRStream()
        {
            string name = "EQ_RR_Stream", type = "RR";
            int channel_count = 1;// 3;
            double nominal_srate = 25.6;
            int channel_format = 3;// 2;

            //0: float32(32 - bit floating - point numbers) — Most commonly used for sensor data like ECG, HR, etc.
            //1: int32(32 - bit integers)
            //2: double(64 - bit floating - point numbers)
            //3: string


            string source_id = "Equivital";

            return CreateStream(name, type, channel_count, nominal_srate, channel_format, source_id);
        }


        public IntPtr CreateIRStream()
        {
            string name = "EQ_IR_Stream", type = "IR";
            int channel_count = 1; // 3;
            double nominal_srate = 25.6; //Todo: check
            int channel_format = 3; // 2;

            //0: float32(32 - bit floating - point numbers) — Most commonly used for sensor data like ECG, HR, etc.
            //1: int32(32 - bit integers)
            //2: double(64 - bit floating - point numbers)
            //3: string


            string source_id = "Equivital";

            return CreateStream(name, type, channel_count, nominal_srate, channel_format, source_id);
        }


        public IntPtr CreateSkinTempStream()
        {
            string name = "EQ_SkinTemp_Stream", type = "SkinTemp";
            int channel_count = 1; // 3;
            double nominal_srate = 1.0/15; //Todo: check
            int channel_format = 3; // 2;

            //0: float32(32 - bit floating - point numbers) — Most commonly used for sensor data like ECG, HR, etc.
            //1: int32(32 - bit integers)
            //2: double(64 - bit floating - point numbers)
            //3: string


            string source_id = "Equivital";

            return CreateStream(name, type, channel_count, nominal_srate, channel_format, source_id);
        }


        public IntPtr CreateGSRStream()
        {
            string name = "EQ_GSR_Stream", type = "GSR";
            int channel_count = 1; // 4;
            double nominal_srate = 16; // GSR Settings, set a new sample frequency of either 2, 4, 8 or 16Hz.
            int channel_format = 3; // 2;

            //0: float32(32 - bit floating - point numbers) — Most commonly used for sensor data like ECG, HR, etc.
            //1: int32(32 - bit integers)
            //2: double(64 - bit floating - point numbers)
            //3: string


            string source_id = "Equivital";

            return CreateStream(name, type, channel_count, nominal_srate, channel_format, source_id);
        }

        public IntPtr CreateOutlet(IntPtr streamInfo)
        {
            IntPtr outlet = lsl_create_outlet(streamInfo, 0);
            return outlet;
        }

        public void PushSample(IntPtr outlet, double[] data)
        {
            double timestamp = GetUnixTimestampNow();

            // Optionally, you can include logic to push sample at specific intervals
            Console.WriteLine($"Pushing sample at timestamp: {timestamp}");

            // Push the sample
            //Thread.Sleep(4);
        }

        // Function to push samples with correct timestamp
        public void PushSampleWithTimestamp(IntPtr outlet, double[] sample, double timestamp)
        {

            // Optionally, you can include logic to push sample at specific intervals
            Console.WriteLine($"Pushing sample at timestamp: {timestamp}");

            // Push the sample
            lsl_push_sample_d(outlet, sample, timestamp);
        }

        public void PushSerializedSampleWithTimestamp(IntPtr outlet, string sample, double timestamp)
        {

            // Optionally, you can include logic to push sample at specific intervals
            //Console.WriteLine($"Pushing Json string sample {sample} at timestamp: {timestamp}");
            //Console.WriteLine($"Pushing Json string sample at timestamp: {timestamp}");

            // Push the sample
            lsl_push_sample_str(outlet, new string[] { sample }, timestamp);
        }

        // ✅ Get Unix timestamp in .NET 4.5 (seconds since 1970)
        public double GetUnixTimestampNow()
        {
            return (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
        }

        public double GetLSLTimestampNow()
        {
            return lsl_local_clock();
        }

        // 🔥 Force LSL’s internal clock to align with Unix time
        public void SynchronizeLSLClock()
        {
            double lsl_time = lsl_local_clock();
            double unix_time = GetUnixTimestampNow();
            double offset = unix_time - lsl_time;  // Difference between Unix and LSL clock

            Console.WriteLine($"Synchronizing LSL clock... LSL Time: {lsl_time}, Unix Time: {unix_time}, Offset: {offset}");
        }

        // ✅ Convert Unix timestamp to match LSL’s expected format
        public double ConvertToLSLTime(double unixTimestamp)
        {
            //double lsl_time = lsl_local_clock();
            //double unix_time = GetUnixTimestampNow();
            //double offset = unix_time - lsl_time;  // Get the LSL-to-Unix time offset

            //return unixTimestamp - offset;  // Shift Unix time to match LSL time

            return unixTimestamp - lsl_to_unix_offset;
        }
    }
}

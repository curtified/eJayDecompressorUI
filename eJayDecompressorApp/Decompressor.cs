using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace eJayDecompressorApp
{
    public class Decompressor
    {
        [DllImport("pxd32d5_d4.dll", EntryPoint = "PInit", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        private static extern int Initialize();

        [DllImport("pxd32d5_d4.dll", EntryPoint = "RWavToTemp", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        private static extern int WavToTemp([MarshalAs(UnmanagedType.LPStr)] string pxdPath, [MarshalAs(UnmanagedType.LPStr)] string tmpPath, int a, int b, int c, int d, int f);

        [DllImport("pxd32d5_d4.dll", EntryPoint = "PClose", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        private static extern int Close();

        [DllImport("eJ_Tool.dll", EntryPoint = "ADecompress", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        private static extern int ADecompress([MarshalAs(UnmanagedType.LPStr)] string pxdPath, int leftOffset, int leftSize, int rightOffset, int rightLenght, int sampleRate, [MarshalAs(UnmanagedType.LPStr)] string tmpPath);

        public static void InitializeDll()
        {
            try { Initialize(); } catch { throw new Exception("Couldn't find or initialize pxd32d5_d4.dll. Be sure to locate it in the same location as the application."); }
        }
        public static void CloseDll()
        {
            try { Close(); } catch { throw new Exception("Couldn't close the pxd32d5_d4.dll!"); }
        }

        public void DecompressSinglePxd(string pxdPath, string outputWavPath)
        {
            InitializeDll();
            try
            {
                // Check if already a WAV file
                using (var fs = File.OpenRead(pxdPath))
                {
                    byte[] riff = new byte[4];
                    fs.Read(riff, 0, 4);
                    if (Encoding.ASCII.GetString(riff) == "RIFF")
                    {
                        File.Copy(pxdPath, outputWavPath, true);
                        return;
                    }
                }
                // Not a WAV, decompress
                string tmpPath = Path.GetTempFileName();
                WavToTemp(pxdPath, tmpPath, 0, 0, 0, 0, 0);
                RawToWav(pxdPath, tmpPath, outputWavPath);
                File.Delete(tmpPath);
            }
            finally { CloseDll(); }
        }

        private void RawToWav(string pxdPath, string tmpPath, string outputWavPath)
        {
            // Read the raw audio data from the temporary file
            byte[] rawData = File.ReadAllBytes(tmpPath);
            int sampleRate = 44100;
            int bitsPerSample = 16;
            int channels = 1; // Mono
            int dataSize = rawData.Length;
            int headerSize = 44;
            int fileSize = headerSize + dataSize - 8; // RIFF chunk size is file size - 8

            using (FileStream fs = new FileStream(outputWavPath, FileMode.Create))
            {
                using (BinaryWriter writer = new BinaryWriter(fs))
                {
                    // Write WAV header
                    writer.Write(Encoding.ASCII.GetBytes("RIFF"));
                    writer.Write(fileSize);
                    writer.Write(Encoding.ASCII.GetBytes("WAVE"));
                    writer.Write(Encoding.ASCII.GetBytes("fmt "));
                    writer.Write(16); // fmt chunk size
                    writer.Write((short)1); // Audio format (1 for PCM)
                    writer.Write((short)channels);
                    writer.Write(sampleRate);
                    writer.Write(sampleRate * channels * bitsPerSample / 8); // Byte rate
                    writer.Write((short)(channels * bitsPerSample / 8)); // Block align
                    writer.Write((short)bitsPerSample);
                    writer.Write(Encoding.ASCII.GetBytes("data"));
                    writer.Write(dataSize);
                    // Write the raw audio data
                    writer.Write(rawData);
                }
            }
        }

        // Add more methods for MultiPXD and .inf/.bin support as needed
    }
} 
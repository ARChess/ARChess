using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace ARChess
{
    public class NuanceAPIKey
    {
        /* NOTE: SSL may not function correctly while the debugger is connected. */
        public static bool SpeechKitSsl = false;

        /* Please contact Nuance to receive the necessary connection and login parameters */

        public static string SpeechKitServer = "sandbox.nmdp.nuancemobility.net";

        public static int SpeechKitPort = 443;

        public static readonly string SpeechKitAppId = "NMDPTRIAL_bulletshot6020121007134420";

        public static readonly byte[] SpeechKitApplicationKey = { (byte)0x56, (byte)0xd7, (byte)0xaa, (byte)0xca, (byte)0x59, (byte)0x9b, (byte)0x95, (byte)0x47, (byte)0x17, (byte)0xca, (byte)0x84, (byte)0x44, (byte)0xc7, (byte)0x29, (byte)0xb8, (byte)0xd9, (byte)0x52, (byte)0xb2, (byte)0xa6, (byte)0x64, (byte)0xfe, (byte)0xc0, (byte)0x43, (byte)0x46, (byte)0x3e, (byte)0xb3, (byte)0x05, (byte)0x8f, (byte)0x03, (byte)0x87, (byte)0xdc, (byte)0x59, (byte)0x5e, (byte)0x86, (byte)0x23, (byte)0xb7, (byte)0x21, (byte)0xb9, (byte)0xff, (byte)0x39, (byte)0x83, (byte)0x83, (byte)0x7d, (byte)0x4a, (byte)0xad, (byte)0x35, (byte)0x23, (byte)0xff, (byte)0xe7, (byte)0xca, (byte)0x63, (byte)0xe5, (byte)0x8a, (byte)0xa7, (byte)0x2b, (byte)0xc8, (byte)0x5a, (byte)0x30, (byte)0x57, (byte)0x5e, (byte)0xb8, (byte)0x33, (byte)0xb7, (byte)0x16 };

        /**
         * The above parameters should be specified in the following manner:
         * 
         * public static string SpeechKitServer = "ndev.host.name.net";
         * 
         * public static int SpeechKitPort = 1000;
         * 
         * public static readonly string SpeechKitAppId = "ExampleSpeechKitSampleID";
         * 
         * public static readonly byte[] SpeechKitApplicationKey =
         * {
         *   0x38, 0x32, 0x0e, 0x46, 0x4e, 0x46, 0x12, 0x5c, 0x50, 0x1d,
         *   0x4a, 0x39, 0x4f, 0x12, 0x48, 0x53, 0x3e, 0x5b, 0x31, 0x22,
         *   0x5d, 0x4b, 0x22, 0x09, 0x13, 0x46, 0x61, 0x19, 0x1f, 0x2d,
         *   0x13, 0x47, 0x3d, 0x58, 0x30, 0x29, 0x56, 0x04, 0x20, 0x33,
         *   0x27, 0x0f, 0x57, 0x45, 0x61, 0x5f, 0x25, 0x0d, 0x48, 0x21,
         *   0x2a, 0x62, 0x46, 0x64, 0x54, 0x4a, 0x10, 0x36, 0x4f, 0x64
         * }; 
         * 
         * Please note that all the specified values are non-functional
         * and are provided solely as an illustrative example.
         * 
         */
    }
}

﻿using SharedProgram.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SharedProgram.DataTypes.CommonDataType;

namespace SharedProgram.DeviceTransfer
{
    public class DeviceSharedValues
    {
        // Connection parameters
        public static int Index = 0;

        public static bool EnController;

        public static string CameraIP = "";
        public static string PrinterIP = "";
        public static string PrinterPort = "";
        public static string ControllerIP = "";
        public static string ControllerPort = "";

        public static string DelaySensor = "";
        public static string DisableSensor = "";
        public static string PulseEncoder = "";
        public static string EncoderDiameter = "";
        public static ActionButtonType ActionButtonType;

        public static VerifyAndPrintModel VPObject = new();
    }
}

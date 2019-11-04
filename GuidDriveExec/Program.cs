using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Threading;

namespace GuidDriveExec
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                if(Thread.CurrentThread.CurrentUICulture.ToString() == "ja-JP")
                {
                    Console.WriteLine("コマンドに含まれるドライブGUIDをドライブ文字に置き換えてから実行します。");
                    Console.WriteLine();
                    Console.WriteLine(Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location) + " <Command>");
                    Console.WriteLine();
                    Console.WriteLine("例: " + Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location) + " robocopy /E \\\\?\\Volume{xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx}\\ D:\\backup\\");
                }
                else
                {
                    Console.WriteLine("Replace the drive GUID in the command with a drive letter and then run the command.");
                    Console.WriteLine();
                    Console.WriteLine(Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location) + " <Command>");
                    Console.WriteLine();
                    Console.WriteLine("Example: " + Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location) + " robocopy /E \\\\?\\Volume{xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx}\\ D:\\backup\\");
                }
                
                
                return;
            }
            String argument = String.Join(" ", args.Skip(1));
            ManagementObjectSearcher ms = new ManagementObjectSearcher("Select * from Win32_Volume");
            foreach (ManagementObject mo in ms.Get())
            {
                if (mo["FreeSpace"] == null) continue;
                string guid = mo["DeviceID"].ToString();

                if (!(mo["DriveLetter"] == null || !(mo["DriveLetter"] is string) || mo["DriveLetter"].ToString().Length == 0))
                {
                    argument = argument.Replace(mo["DeviceID"].ToString(), mo["DriveLetter"].ToString().Substring(0, 2));
                }
            }
            ms.Dispose();
            Console.WriteLine(args[0] + argument);
            Process p = new Process();
            p.StartInfo.FileName = args[0];
            p.StartInfo.Arguments = argument;
            p.StartInfo.UseShellExecute = false;
            p.Start();
            p.WaitForExit();
            p.Dispose();
        }
    }
}

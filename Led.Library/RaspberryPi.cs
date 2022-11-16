using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Led.Library;
public static class RaspberryPi
{
    public static bool IsRecording { get; private set; } = false;
    private static System.Diagnostics.Process? recordingProcess;

    public static void ToggleRecording()
    {
        if (IsRecording)
        {
            if (recordingProcess != null && !recordingProcess.HasExited)
            {
                IsRecording = false;
                recordingProcess.Kill();
                recordingProcess.Dispose();
            }
        }
        else
        {
            IsRecording = true;
            Task.Run(
                () =>
                {
                    while (IsRecording)
                    {
                        string path = "/home/pi/Videos/Recordings/";
                        string currentDateTime = DateTime.Now.ToString("yyyy-MM-dd HHmmss");
                        int rotation = 90;
                        int timeoutMinutes = 30;

                        string command = $"raspivid -o {path}{currentDateTime}.h264 -t {timeoutMinutes * 60 * 1000} -rot {rotation}  -vs --nopreview";

                        recordingProcess = new();
                        recordingProcess.StartInfo.FileName = "/bin/bash";
                        recordingProcess.StartInfo.Arguments = $"-c \"{command}\"";
                        recordingProcess.StartInfo.UseShellExecute = false;
                        recordingProcess.StartInfo.RedirectStandardOutput = true;
                        recordingProcess.StartInfo.RedirectStandardError = true;
                        recordingProcess.Start();
                        recordingProcess.WaitForExit();
                    }
                }
            );
        }
    }

    public static void TurnOff()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) return;

        string command = "sudo poweroff";
        using (System.Diagnostics.Process proc = new System.Diagnostics.Process())
        {
            proc.StartInfo.FileName = "/bin/bash";
            proc.StartInfo.Arguments = $"-c \"{command}\"";
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.RedirectStandardError = true;
            proc.Start();
        }
    }
}

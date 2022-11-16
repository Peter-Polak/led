using System.Runtime.InteropServices;
using Led.Library.Matrices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Led.BlazorServerWebApp.API_Controllers;
[Route("api/[controller]")]
[ApiController]
public class RaspberryPiController : ControllerBase
{
    // PUT api/[controller]/power
    [HttpDelete("power")]
    public void TurnOffRaspberryPi()
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

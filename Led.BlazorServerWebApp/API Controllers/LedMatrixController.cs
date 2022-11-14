using Led.BlazorServerWebApp.Constants;
using Led.Library.Matrices;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Led.BlazorServerWebApp.API_Controllers
{
    [Route("api/matrix")]
    [ApiController]
    public class LedMatrixController : ControllerBase
    {
        private Hub75Matrix matrix;
        public LedMatrixController(Hub75Matrix matrix)
        {
            this.matrix = matrix;
        }

        // PUT api/matrix/power
        [HttpPut("power")]
        public void SetMatrixPower(bool isOn)
        {
            matrix.IsOn = isOn;
        }

        // PUT api/matrix/image/bike
        [HttpPut("image/bike")]
        public void RenderBike()
        {
            matrix.CancelCurrentTask(false);
            matrix.DrawImage(Media.Image.Bike);
        }
    }
}

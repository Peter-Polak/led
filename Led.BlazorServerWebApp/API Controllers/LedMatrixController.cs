﻿using Led.BlazorServerWebApp.Constants;
using Led.Library.Matrices;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Led.BlazorServerWebApp.API_Controllers
{
    [Route("api")]
    [ApiController]
    public class LedMatrixController : ControllerBase
    {
        private Hub75Matrix matrix;
        public LedMatrixController(Hub75Matrix matrix)
        {
            this.matrix = matrix;
        }

        // GET: api/<LedMatrixController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<LedMatrixController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/bike
        [HttpPost("bike")]
        public void Post()
        {
            matrix.IsOn = true;
            matrix.CancelCurrentTask(false);
            matrix.Clear();

            var imageFile = Image.Load<Rgb24>(Media.Image.Bike.GetRelativePathWeb());
            matrix.DrawImage(imageFile, Media.Image.Bike.GetRelativePathWeb());
        }

        // PUT api/<LedMatrixController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<LedMatrixController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}

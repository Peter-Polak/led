using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Iot.Device.LEDMatrix;
using Led.Library.Iot;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Led.Library.Matrices
{

    public class Hub75Matrix : LedMatrix
    {
        public int RenderDelay 
        {
            get
            {
                if (ledMatrix == null) return 0;
                return ledMatrix.RenderDelay;
            }
            set
            {
                if (ledMatrix == null) return;
                ledMatrix.RenderDelay = value;
            }
        }

        private readonly RgbLedMatrix? ledMatrix;

        public long PwmDuration
        {
            get
            {
                if (ledMatrix == null) return 0;
                return ledMatrix.PWMDuration;
            }
            set
            {
                if (ledMatrix == null) return;
                ledMatrix.PWMDuration = value;
            }
        }

        public override bool IsOn 
        {
            get => _isOn;
            set
            {
                if (_isOn == value) return;
                _isOn = value;
                if(_isOn == true) StartRendering(); else StopRendering();
            }
        }

        public Hub75Matrix(int width = 64, int height = 64, int renderDelay = 1, int pwmDuration = 100) : base(width, height)
        {
            RenderDelay = renderDelay;

            //PinMapping pinMapping = new PinMapping(2, 3, 4, 17, 27, 22, 10, 9, 11, 5, 6, 13, 19, 26); // RGB pin mapping
            PinMapping pinMapping = new PinMapping(
                r1: 2, g1: 4, b1: 3, 
                r2: 17, g2: 22, b2: 27, 
                oe: 10, clock: 9, latch: 11, 
                a: 5, b: 6, c: 13, d: 19, e: 26
            ); // GRB pin mapping

            try
            {
                ledMatrix = new RgbLedMatrix(pinMapping, width, height, 1, 1, renderDelay);
                PwmDuration = pwmDuration;
            }
            catch (Exception)
            {
                
            }
        }

        public override Task PlayGif(Image<Rgb24> gif, int delayBetweenFrames)
        {
            return Task.Run(
                async () =>
                {
                    for (int i = 0; i < gif.Frames.Count; i++)
                    {
                        var frame = gif.Frames.CloneFrame(i);
                        var frameDelay = gif.Frames[i].Metadata.GetGifMetadata().FrameDelay;

                        //DrawImage(frame);
                        //Thread.Sleep(frameDelay * 10);
                        await Task.Delay(delayBetweenFrames);
                    }
                }
            );
        }

        public override void DrawImage(Image<Rgb24> image, string imagePath, int x = 0, int y = 0)
        {
            base.DrawImage(image, imagePath);
            if (ledMatrix == null) return;

            var bitmap = ImageSharpExtensions.ToBitmap(image);

            ledMatrix.DrawBitmap(x, y, bitmap);
        }

        public override void SetPixel(int x, int y, Color color)
        {
            base.SetPixel(x, y, color);

            if (ledMatrix == null) return;
            if (x < 0 || x >= Width) return;
            if (y < 0 || y >= Height) return;

            byte red = color.ToPixel<Rgb24>().R;
            byte green = color.ToPixel<Rgb24>().G;
            byte blue = color.ToPixel<Rgb24>().B;

            ledMatrix.SetPixel(x, y, red, green, blue);
        }

        public override void SetPixel(int x, int y, int red, int green, int blue)
        {
            base.SetPixel(x, y, red, green, blue);

            if (ledMatrix == null) return;
            if (x < 0 || x >= Width) return;
            if (y < 0 || y >= Height) return;

            ledMatrix.SetPixel(x, y, (byte)red, (byte)green, (byte)blue);
        }

        public override void Fill(Color color)
        {
            base.Fill(color);

            if (ledMatrix == null) return;

            byte red = color.ToPixel<Rgb24>().R;
            byte green = color.ToPixel<Rgb24>().G;
            byte blue = color.ToPixel<Rgb24>().B;

            ledMatrix.Fill(red, green, blue);
        }

        public override void Clear()
        {
            base.Clear();

            Fill(Color.Black);
        }

        public override void Update()
        {
            
        }

        public void StartRendering()
        {
            if (ledMatrix == null) return;
            ledMatrix.StartRendering();
        }

        public void StopRendering()
        {
            if (ledMatrix == null) return;
            ledMatrix.StopRendering();
        }

        public void Dispose()
        {
            if (ledMatrix == null) return;
            ledMatrix.Dispose();
        }
    }
}

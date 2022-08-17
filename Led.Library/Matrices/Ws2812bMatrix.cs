//using Iot.Device.Ws28xx;
//using SixLabors.ImageSharp;
//using SixLabors.ImageSharp.PixelFormats;
//using System;
//using System.Collections.Generic;
//using System.Device.Gpio;
//using System.Device.Spi;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Led.Library.Matrices
//{
//    public class Ws2812bMatrix : LedMatrix
//    {
//        private Ws2812b ledMatrix;

//        //TEMPORARY
//        public const int psuPin = 17;
//        private GpioController gpioController;
//        public override bool IsOn
//        {
//            get
//            {
//                return _isOn;
//            }

//            set
//            {
//                if (_isOn == value)
//                {
//                    return;
//                }
//                else
//                {
//                    _isOn = value;
//                }

//                if (value)
//                {
//                    if(gpioController != null)
//                    {
//                        gpioController.OpenPin(psuPin, PinMode.Output);
//                        gpioController.Write(psuPin, PinValue.High);
//                    }

//                    Thread.Sleep(100);

//                    Update();
//                }
//                else
//                {
//                    if(gpioController != null)
//                    {
//                        gpioController.Write(psuPin, PinValue.Low);
//                        gpioController.ClosePin(psuPin);
//                    }
//                }
//            }
//        }

//        public Ws2812bMatrix(int width = 16, int height = 16) : base(width, height)
//        {
//            var spiSettings = new SpiConnectionSettings(0, 0)
//            {
//                ClockFrequency = 2_400_000,
//                Mode = SpiMode.Mode0,
//                DataBitLength = 8
//            };

//            try
//            {
//                var spiDevice = SpiDevice.Create(spiSettings);
//                ledMatrix = new Ws2812b(spiDevice, width, height);
//                gpioController = new GpioController();
//            }
//            catch (Exception)
//            {

//            }

//            IsOn = true;

//            Clear();
//            Update();
//        }

//        public override Task PlayGif(Image<Rgb24> gif, int delayBetweenFrames)
//        {
//            throw new NotImplementedException();
//        }

//        public override void DrawImage(Image<Rgb24> image, int x = 0, int y = 0)
//        {
//            base.DrawImage(image);
//            for (int yCoordinate = 0; yCoordinate < Height; yCoordinate++)
//            {
//                for (int xCoordinate = 0; xCoordinate < Width; xCoordinate++)
//                {
//                    SetPixel(xCoordinate, yCoordinate, image[xCoordinate, yCoordinate]);
//                }
//            }
//        }

//        public override void SetPixel(int x, int y, Color color)
//        {
//            base.SetPixel(x, y, color);
//            if (ledMatrix == null) return;

//            int red = color.ToPixel<Rgb24>().R;
//            int green = color.ToPixel<Rgb24>().G;
//            int blue = color.ToPixel<Rgb24>().B;

//            int normalizedX = x;
//            if(y % 2 == 0)
//            {
//                normalizedX = (Width - 1) - x;
//            }

//            ledMatrix.Image.SetPixel(normalizedX, y, System.Drawing.Color.FromArgb(red, green, blue));
//        }

//        public override void SetPixel(int x, int y, int red, int green, int blue)
//        {
//            base.SetPixel(x, y, red, green, blue);
//            if (ledMatrix == null) return;

//            int normalizedX = x;
//            if (y % 2 == 0)
//            {
//                normalizedX = (Width - 1) - x;
//            }

//            ledMatrix.Image.SetPixel(normalizedX, y, System.Drawing.Color.FromArgb(red, green, blue));
//        }

//        public override void Update()
//        {
//            if (ledMatrix == null) return;
//            ledMatrix.Update();
//        }

//        public override void Clear()
//        {
//            base.Clear();

//            for (int y = 0; y < Height; y++)
//            {
//                for (int x = 0; x < Width; x++)
//                {
//                    SetPixel(x, y, Color.Black);
//                }
//            }
//        }
//    }
//}

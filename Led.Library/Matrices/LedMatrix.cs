using System;
using System.Collections.Generic;
using System.Device.Spi;
using SixLabors.ImageSharp;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Led.Library.Matrices
{
    public abstract class LedMatrix
    {
        #region Events
        public event EventHandler<string> OnImageDraw;
        public event EventHandler<string> OnCurrentTaskComplete;
        public event EventHandler<string> OnCurrentTaskStart;
        #endregion

        public int Width { get; private set; }
        public int Height { get; private set; }
        public string CurrentImagePath { get; private set; }
        public Image<Rgb24> Matrix { get; private set; }
        public string CurrentTaskName { get; private set; }
        public Task? CurrentTask { get; private set; }
        
        public CancellationToken CancellationToken => cancellationTokenSource.Token;

        public abstract bool IsOn { get; set; }

        protected  bool _isOn;
        private CancellationTokenSource cancellationTokenSource;
        private Image<Rgb24>? backupImage;

        public LedMatrix(int width, int height)
        {
            Matrix = new Image<Rgb24>(width, height);
            Width = width;
            Height = height;

            cancellationTokenSource = new CancellationTokenSource();
            backupImage = null;
        }

        public abstract void Update();

        public virtual void SetPixel(int x, int y, Color color)
        {
            if (Matrix == null) return;
            if (x < 0 || x >= Width) return;
            if (y < 0 || y >= Height) return;
            Matrix[x, y] = color;
        }

        public virtual void SetPixel(int x, int y, int red, int green, int blue)
        {
            var color = Color.FromRgb((byte)red, (byte)green, (byte)blue);
            SetPixel(x, y, color);
        }

        public Color GetPixel(int x, int y)
        {
            if (x < 0 || x >= Width) return Color.Black;
            if (y < 0 || y >= Height) return Color.Black;

            return Matrix[x, y];
        }

        public virtual void DrawImage(Image<Rgb24> image, string imagePath, int x = 0, int y = 0)
        {
            if (image.Width > Width || image.Height > Height)
                image.Mutate(image => image.Resize(Width, Height));

            Matrix.Mutate(matrix => matrix.DrawImage(image, new Point(x, y), 1f));
            CurrentImagePath = imagePath;
            OnImageDraw?.Invoke(this, imagePath);
        }

        public virtual void DrawImage(string imagePath, int x = 0, int y = 0)
        {
            using var image = Image.Load<Rgb24>(imagePath);
            DrawImage(image, imagePath, x, y);
        }

        public virtual void Fill(Color color)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    SetPixel(x, y, color);
                }
            }
        }

        public virtual void Clear()
        {
            OnImageDraw?.Invoke(this, string.Empty);
            Fill(Color.Black);
        }

        public async void RunTask(Action action, string taskName)
        {
            if (CurrentTask != null && !CurrentTask.IsCompleted)
            {
                CancelCurrentTask(false);
            }

            if(backupImage == null) backupImage = Matrix.Clone();
            CurrentTaskName = taskName;
            OnCurrentTaskStart?.Invoke(this, CurrentTaskName);
            CurrentTask = Task.Run(action, CancellationToken);
            CancellationToken.Register(() => { OnCurrentTaskComplete?.Invoke(this, CurrentTaskName); CurrentTaskName = string.Empty; });
            await CurrentTask;
        }

        public void CancelCurrentTask(bool restoreMatrix = true)
        {
            if (CurrentTask == null || CurrentTask.IsCompleted) return;
            
            cancellationTokenSource.Cancel();
            CurrentTask.Wait();
            cancellationTokenSource.Dispose();
            cancellationTokenSource = new CancellationTokenSource();
            
            if(restoreMatrix && backupImage != null)
            {
                DrawImage(backupImage, "backup");
                backupImage.Dispose();
                backupImage = null;
            }
        }

        public abstract Task PlayGif(Image<Rgb24> gif, int delayBetweenFrames);
    }
}

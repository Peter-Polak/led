using System;
using System.Collections.Generic;
using System.Device.Spi;
using SixLabors.ImageSharp;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Led.BlazorServerWebApp.Constants;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.Fonts;

namespace Led.Library.Matrices;

public abstract class LedMatrix
{
    #region Events
    public event EventHandler<bool> PowerSwitch;
    public event EventHandler<Media?>? ImageDrawn;
    public event EventHandler<string>? CurrentTaskStarted;
    public event EventHandler<string>? CurrentTaskCompleted;
    #endregion

    protected bool _isOn;
    private CancellationTokenSource cancellationTokenSource;
    private Media? backupImage;

    public int Width { get; private set; }
    public int Height { get; private set; }
    public float Brightness { get; set; }
    public float Contrast { get; set; }
    public float Hue { get; set; }
    public float Saturation { get; set; }
    public float Lightness { get; set; }
    public Media? CurrentMedia { get; private set; }
    public Image<Rgb24> Matrix { get; private set; }
    public string CurrentTaskName { get; private set; }
    public Task? CurrentTask { get; private set; }
    public CancellationToken CancellationToken => cancellationTokenSource.Token;

    public virtual bool IsOn
    {
        get => _isOn;

        set
        {
            _isOn = value;
            OnPowerSwitch(value);
        }
    }

    protected virtual void OnPowerSwitch(bool isOn) => PowerSwitch?.Invoke(this, IsOn);

    public LedMatrix(int width, int height)
    {
        Matrix = new Image<Rgb24>(width, height);
        Width = width;
        Height = height;
        Brightness = 1.0f;
        Contrast = 1.0f;
        Hue = 0f;
        Saturation = 1.0f;
        Lightness = 1.0f;
        CurrentTaskName = string.Empty;

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

    public virtual void DrawImage(Image<Rgb24> image, Media? media, int x = 0, int y = 0)
    {
        if (image.Width > Width || image.Height > Height)
            image.Mutate(image => image.Resize(Width, Height));

        image.Mutate(image => image.Resize(Width, Height).Brightness(Brightness).Contrast(Contrast).Hue(Hue).Saturate(Saturation).Lightness(Lightness));

        Matrix.Mutate(matrix => matrix.DrawImage(image, new Point(x, y), 1f));
        CurrentMedia = media;
        OnImageDrawn(media);
    }

    public virtual void DrawImage(Media currentImage, int x = 0, int y = 0)
    {
        using var image = Image.Load<Rgb24>(currentImage.GetPathRelative(true));
        DrawImage(image, currentImage, x, y);
    }

    protected virtual void OnImageDrawn(Media? image) => ImageDrawn?.Invoke(this, image);

    public virtual void DrawText(string text, Color color, TextOptions textOptions, Image<Rgb24> background)
    {
        background.Mutate(x => x.DrawText(textOptions, text, color));
        DrawImage(background, null);
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
        CurrentMedia = null;
        ImageDrawn?.Invoke(this, CurrentMedia);
        Fill(Color.Black);
    }

    public void RunTask(Action action, string taskName)
    {
        if (CurrentTask != null && !CurrentTask.IsCompleted)
        {
            CancelCurrentTask(false);
        }

        if(backupImage == null) backupImage = CurrentMedia;
        CurrentTaskName = taskName;
        OnCurrentTaskStarted(CurrentTaskName);
        CurrentTask = Task.Run(action, CancellationToken);
        CancellationToken.Register(() => OnCurrentTaskCompleted(CurrentTaskName));
    }

    protected virtual void OnCurrentTaskStarted(string currentTaskName) => CurrentTaskStarted?.Invoke(this, currentTaskName);
    protected virtual void OnCurrentTaskCompleted(string currentTaskName)
    {
        CurrentTaskCompleted?.Invoke(this, CurrentTaskName); 
        CurrentTaskName = string.Empty;
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
            DrawImage(backupImage);
            backupImage = null;
        }
    }

    public virtual void PlayGif(Image<Rgb24> gif, Media currentGif, int? delayBetweenFrames = null, int x = 0, int y = 0)
    {
        RunTask(
            async () =>
            {
                for (int i = 0; i < gif.Frames.Count; i++)
                {
                    var frame = gif.Frames.CloneFrame(i);
                    var frameDelay = gif.Frames[i].Metadata.GetGifMetadata().FrameDelay;

                    int delay = (int)(delayBetweenFrames == null ? frameDelay : delayBetweenFrames);
                    DrawImage(frame, currentGif, x, y);
                    await Task.Delay(delay);
                }
            }
            , currentGif.GetPathRelative(true)
        );
    }

    public virtual void PlayGif(Media currentGif, int? delayBetweenFrames = null, int x = 0, int y = 0)
    {
        using var gif = Image.Load<Rgb24>(currentGif.GetPathRelative(true));
        PlayGif(gif, currentGif, x, y);
    }
}

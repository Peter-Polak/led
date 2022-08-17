using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Led.Library
{
    public class Gif
    {
        public string Path { get; private set; }
        public int FrameCount { get; private set; }
        public Bitmap[] Frames { get; private set; }

        // Thanks to https://stackoverflow.com/a/26178389 for some help.
        public Gif(string path)
        {
            Image gif = Image.FromFile(path);
            FrameDimension dimension = new FrameDimension(gif.FrameDimensionsList[0]);

            Path = path;
            FrameCount = gif.GetFrameCount(dimension);
            Frames = new Bitmap[FrameCount];

            for (int index = 0; index < FrameCount; index++)
            {
                gif.SelectActiveFrame(dimension, index);

                Frames[index] = new Bitmap(gif.Size.Width, gif.Size.Height);
                Graphics.FromImage(Frames[index]).DrawImage(gif, new Point(0, 0));
            }
        }

        public void ExportFrames(string path)
        {
            for (int index = 0; index < FrameCount; index++)
            {
                Frames[index].Save($"{path}/{index}.jpeg", ImageFormat.Jpeg);
            }
        }
    }
}

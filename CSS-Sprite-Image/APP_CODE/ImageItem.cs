using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace CSS_Sprite_Image
{
    class ImageItem
    {
        static double MaxCanvasWidth = 1024;
        static double MaxCanvasHeight = 1024;
        static List<double> LineHeights { get; set; } = new List<double>();
        static List<ImageItem> ImagesList { get; set; } = new List<ImageItem>();
        int Id { get; set; }
        string Path { get; set; }
        internal double Width { get; set; }
        internal double Height { get; set; }
        internal Point Position { get; set; }
        int Row { get; set; }
        int Column { get; set; }

        internal bool Added { get; set; } = false;

        internal ImageItem(string path)
        {
            Id = ImagesList.Count > 0 ? ImagesList.Max(it => it.Id) + 1 : 1;
            BitmapFrame bitmapFrame;
            Path = path;
            try
            {
                bitmapFrame = BitmapFrame.Create(new Uri(path), BitmapCreateOptions.DelayCreation, BitmapCacheOption.None);
            }
            catch (Exception)
            {
                return;
            }
            Width = bitmapFrame.PixelWidth;
            Height = bitmapFrame.PixelHeight;
            List<int> lines = new List<int>();
            for (int i = 0; i < LineHeights.Count; i++)
            {
                if (LineHeights[i] == Height)
                {
                    lines.Add(i);
                }
            }

            bool found = false;
            foreach (int i in lines)
            {
                double X = 0, Y = 0;
                var lineList = ImagesList.Where(it => it.Row == i);
                var maxWidth = lineList.Max(it => (it.Position.X + it.Width));
                if (maxWidth + Width <= MaxCanvasWidth)
                {
                    Row = i;
                    Column = lineList.Count();
                    if (Column > 0)
                    {
                        Y = lineList.First().Position.Y;
                    }
                    else
                    {
                        Y = LineHeights.Take(i).Sum();
                    }

                    X = maxWidth;
                    Position = new Point(X, Y);
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                double maxHieght = LineHeights.Sum();
                if (maxHieght + Height <= MaxCanvasHeight)
                {
                    Row = LineHeights.Count;
                    Column = 0;
                    double X = 0, Y = 0;
                    Y = maxHieght;
                    Position = new Point(X, Y);
                    LineHeights.Add(Height);
                    found = true;
                }
            }
            if (found)
            {
                Added = true;
                ImagesList.Add(this);
            }
        }

    }
}

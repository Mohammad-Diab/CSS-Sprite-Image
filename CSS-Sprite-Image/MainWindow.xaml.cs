﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.IO;
using System.Linq;

namespace CSS_Sprite_Image
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        #region Events
        private void CreateNewProject_Click(object sender, RoutedEventArgs e)
        {
            new NewProject().ShowDialog();
        }

        private void AddImages_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                foreach (string it in ofd.FileNames)
                {
                    UIElement result = AddImageToCanvas(it);
                    if (result != null)
                    {
                        canvas.Children.Add(result);
                    }
                }
                Point dimensions = GetCanvasDimensions();
                canvas.Tag = new Point(dimensions.X, dimensions.Y);
                canvas.Width = (dimensions.X) * canvasScaleFactor.ScaleX;
                canvas.Height = (dimensions.Y) * canvasScaleFactor.ScaleY;
            }
        }

        private void CanvasZoomIn_Click(object sender, RoutedEventArgs e)
        {
            CanvasZoom(Zoom_Enum.ZoomIn);
        }

        private void CanvasZoomOut_Click(object sender, RoutedEventArgs e)
        {
            CanvasZoom(Zoom_Enum.ZoomOut);
        }

        #endregion Events

        #region Functions
        private Border AddImageToCanvas(string path)
        {
            if (File.Exists(path))
            {
                ImageItem newImage = new ImageItem(path);
                if (newImage.Added)
                {
                    Image im = new Image()
                    {
                        Width = newImage.Width - 4,
                        Height = newImage.Height - 4,
                        Source = new BitmapImage(new Uri(path))
                    };
                    Border br = new Border()
                    {
                        Width = newImage.Width,
                        Height = newImage.Height,
                        BorderBrush = Brushes.Gray,
                        BorderThickness = new Thickness(2),
                        Child = im
                    };
                    Canvas.SetLeft(br, newImage.Position.X);
                    Canvas.SetTop(br, newImage.Position.Y);
                    ImageItem.GetCanvasDimensions();
                    return br;
                }
            }
            return null;
        }

        private void CanvasZoom(Zoom_Enum zoom)
        {
            List<double> zoomValues = new List<double>() { 0.1, 0.25, 0.33, 0.5, 0.67, 0.8, 0.9, 1, 1.1, 1.25, 1.5, 1.75, 2 };
            var currentZoomIndex = zoomValues.FindIndex(it => canvasScaleFactor.ScaleX == it);
            if (currentZoomIndex < 0)
            {
                currentZoomIndex = zoomValues.IndexOf(1);
            }
            else
            {
                currentZoomIndex += (zoom == Zoom_Enum.ZoomIn ? +1 : -1);
                if (currentZoomIndex < 0)
                    currentZoomIndex = 0;
                else if (currentZoomIndex >= zoomValues.Count)
                    currentZoomIndex = zoomValues.Count - 1;
            }
            
            canvasScaleFactor.ScaleX = zoomValues[currentZoomIndex];
            canvasScaleFactor.ScaleY = zoomValues[currentZoomIndex];
            if (canvas.Tag != null)
            {
                canvas.Width = ((Point)canvas.Tag).X * zoomValues[currentZoomIndex];
                canvas.Height = ((Point)canvas.Tag).Y * zoomValues[currentZoomIndex];
            }

        }

        private Point GetCanvasDimensions()
        {
            return ImageItem.GetCanvasDimensions();
        }


        #endregion Functions
    }
}

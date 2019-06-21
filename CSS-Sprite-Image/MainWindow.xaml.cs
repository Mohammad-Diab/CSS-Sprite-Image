using System;
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
using Ookii.Dialogs.Wpf;

namespace CSS_Sprite_Image
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        ProjectFile currentProject;
        public MainWindow()
        {
            InitializeComponent();
            currentProject = new ProjectFile("project1", 1024, 1024, OrganizeMethod.EachImageHeightInRow, Environment.UserName);
        }

        #region Events
        private void CreateNewProject_Click(object sender, RoutedEventArgs e)
        {
            new NewProject().ShowDialog();
        }

        private void AddImages_Click(object sender, RoutedEventArgs e)
        {
            VistaOpenFileDialog vofd = new VistaOpenFileDialog()
            {
                Multiselect = true,
                CheckFileExists = true,
                Filter = "All image Files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png, *.bmp) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png; *.bmp",
                Title = "Select Images to add..."
            };
            if (vofd.ShowDialog() ==  true)
            {
                foreach (string it in vofd.FileNames)
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

        private void SaveProject_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog vfbd = new VistaFolderBrowserDialog()
            {
                Description = "Select a path to save your project in...",
                ShowNewFolderButton = true,
                UseDescriptionForTitle = true
            };
            if (vfbd.ShowDialog() == true)
            {
                SaveProject(vfbd.SelectedPath);
            }
        }

        private void LoadProject_Click(object sender, RoutedEventArgs e)
        {
            VistaOpenFileDialog vofd = new VistaOpenFileDialog()
            {
                Filter = "Sprite Image Project File (*.sip)|*.sip",
                CheckFileExists = true,
                CheckPathExists = true,
                Multiselect = false,
                Title = "Select a project file to load...",
            };
            if (vofd.ShowDialog() == true)
            {
                LoadProject(vofd.FileName);
                foreach (ImageItem it in currentProject?.ImagesList)
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

        #endregion Events

        #region Functions
        private Border AddImageToCanvas(string path)
        {
            if (File.Exists(path))
            {
                ImageItem newImage = new ImageItem(currentProject, path);
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
                    Canvas.SetLeft(br, newImage.PositionX);
                    Canvas.SetTop(br, newImage.PositionY);
                    return br;
                }
            }
            return null;
        }

        private Border AddImageToCanvas(ImageItem image)
        {
            if (File.Exists(image.ImagePath))
            {
                if (image.Added)
                {
                    Image im = new Image()
                    {
                        Width = image.Width - 4,
                        Height = image.Height - 4,
                        Source = new BitmapImage(new Uri(image.ImagePath))
                    };
                    Border br = new Border()
                    {
                        Width = image.Width,
                        Height = image.Height,
                        BorderBrush = Brushes.Gray,
                        BorderThickness = new Thickness(2),
                        Child = im
                    };
                    Canvas.SetLeft(br, image.PositionX);
                    Canvas.SetTop(br, image.PositionY);
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
            return currentProject?.GetCanvasDimensions() ?? new Point(0, 0);
        }

        private void SaveProject(string selectedPath)
        {
            if (!string.IsNullOrEmpty(selectedPath))
            {
                DirectoryInfo di = new DirectoryInfo(selectedPath);
                if (!di.Exists)
                {
                    try
                    {
                        di.Create();
                    }
                    catch (Exception) { }
                }
                if (di.Exists)
                {
                    currentProject.SaveProject(di);
                }
            }
        }

        private void LoadProject(string selectedPath)
        {
            if (!string.IsNullOrEmpty(selectedPath))
            {
                FileInfo fi = new FileInfo(selectedPath);
                if (!fi.Exists|| !fi.Directory.Exists)
                {
                    return;
                }
                currentProject = ProjectFile.LoadProject(fi.Directory, selectedPath);
                if(currentProject == null)
                {
                    return;
                }
            }
        }


        #endregion Functions


    }
}

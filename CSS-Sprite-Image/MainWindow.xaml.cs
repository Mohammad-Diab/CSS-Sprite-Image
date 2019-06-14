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
                    AddImageToCanvas(it);
                }
            }
        }

        private void AddImageToCanvas(string path)
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
                    canvas.Children.Add(br);
                    canvas.Width = newImage.Width + newImage.Position.X;
                    canvas.Height = newImage.Height + newImage.Position.Y;
                }
            }
        }

    }
}

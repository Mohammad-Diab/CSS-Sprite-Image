using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Shapes;

namespace CSS_Sprite_Image
{
    /// <summary>
    /// Interaction logic for ExportProject.xaml
    /// </summary>
    public partial class ExportProject : Window
    {
        public ExportProject()
        {
            InitializeComponent();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        internal ProjectFile project;

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            if (project == null)
            {
                return;
            }
            if (string.IsNullOrEmpty(cssClassPattern.Text) || string.IsNullOrEmpty(location.Text) || string.IsNullOrEmpty(exportPath.Text) || type.SelectedIndex < 0)
            {
                return;
            }
            string destenationPath = Functions.ProcessLocation(exportPath.Text);
            DirectoryInfo destenation = new DirectoryInfo(destenationPath);

            if (project.Export(destenation, cssClassPattern.Text, location.Text, (ExportImageFormat_Enum)type.SelectedIndex))
            {
                MessageBox.Show("Done");
                Close();
            }
            else
            {
                MessageBox.Show("Error");
            }
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog vfbd = new VistaFolderBrowserDialog()
            {
                Description = "Select a path to export your project to...",
                ShowNewFolderButton = true,
                UseDescriptionForTitle = true
            };
            if (vfbd.ShowDialog() == true)
            {
                exportPath.Text = vfbd.SelectedPath;
            }
        }
    }
}

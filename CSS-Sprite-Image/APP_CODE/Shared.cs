using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace CSS_Sprite_Image
{

    enum Zoom_Enum
    {
        ZoomIn = 1,
        ZoomOut = 2
    }

    public enum OrganizeMethod
    {
        EachImageHeightInRow = 1,
        Sequence = 2,
        BinaryTree = 3
    }

    enum ExportImageFormat_Enum
    {
        PNG = 0,
        JPG = 1,
        GIF = 2,
        WEBP = 3,
        BMP = 4
    }

    internal static class Functions
    {
        internal static string BeautifyXml(string xml)
        {
            var stringBuilder = new StringBuilder();
            var element = System.Xml.Linq.XElement.Parse(xml);

            var settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true;
            settings.Indent = true;
            settings.NewLineOnAttributes = true;
            using (var xmlWriter = XmlWriter.Create(stringBuilder, settings))
            {
                element.Save(xmlWriter);
            }

            return stringBuilder.ToString();
        }

        internal static string ProcessImagePath(string imagePath, string projectName, string extention)
        {
            extention = Regex.Replace(extention, "[\\\\~#\\.%&*<>?:\\/|\"]", "_");
            projectName = Regex.Replace(projectName, "[\\\\~#\\.%&*<>?:\\/|\"]", "_");
            string path = Regex.Replace(imagePath, "[~#%&*<>?|\"]", "_");
            path = path.Replace("{{", "<<").Replace("}}", ">>");
            path = path.Replace("{currentImageName}", $"{projectName}.{extention}");
            path = path.Replace("<<", "{").Replace(">>", "}");
            return path;
        }

        internal static string ProcessClassName(ImageItem item, string classNamePattern)
        {
            string pattern = classNamePattern;
            string imageName = Regex.Replace(item.ImageName, @"[^a-zA-Z0-9_\-]", "-");
            pattern = pattern.Replace("{imageName}", imageName);
            pattern = pattern.Replace("{imageWidth}", item.Width.ToString());
            pattern = pattern.Replace("{imageHeight}", item.Height.ToString());
            pattern = pattern.Replace("{PositionX}", item.PositionX.ToString());
            pattern = pattern.Replace("{PositionY}", item.PositionY.ToString());
            pattern = Regex.Replace(pattern, @"[^a-zA-Z0-9_\-]", "-");
            return pattern;
        }

        internal static string ProcessLocation(string location)
        {
            switch (location.ToLower())
            {
                case "{desktop}":
                    return Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                case "{documents}":
                    return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                case "{home}":
                    return Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            }
            return location;
        }

        internal static ImageFormat GetImageFormat(string imageExtention)
        {
            switch (imageExtention.ToLower())
            {
                case "png":
                    return ImageFormat.Png;
                case "jpg":
                    return ImageFormat.Jpeg;
                case "gif":
                    return ImageFormat.Gif;
                case "bmp":
                    return ImageFormat.Bmp;
                default:
                    return ImageFormat.Png;
            }
        }
    }
}

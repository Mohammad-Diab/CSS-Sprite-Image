using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Xml;

namespace CSS_Sprite_Image
{
    class ImageItem
    {
        internal string Id { get; set; }
        internal string ImagePath { get; set; }
        internal string ImageName { get; set; }
        internal double Width { get; set; }
        internal double Height { get; set; }
        internal Point Position { get; set; }
        internal int Row { get; set; }
        internal int Column { get; set; }
        internal bool Added { get; set; } = false;

        internal ProjectFile project;

        internal ImageItem(ProjectFile project, string path)
        {
            if (string.IsNullOrEmpty(path))
                return;
            //Id = ImagesList.Count > 0 ? ImagesList.Max(it => it.Id) + 1 : 1;
            Id = Guid.NewGuid().ToString();
            ImageName = Path.GetFileNameWithoutExtension(path);
            ImagePath = path;
            BitmapFrame bitmapFrame;
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
            for (int i = 0; i < project.LineHeights.Count; i++)
            {
                if (project.LineHeights[i] == Height)
                {
                    lines.Add(i);
                }
            }

            bool found = false;
            foreach (int i in lines)
            {
                double X = 0, Y = 0;
                var lineList = project.ImagesList.Where(it => it.Row == i);
                var maxWidth = lineList.Max(it => (it.Position.X + it.Width));
                if (maxWidth + Width <= project.MaxCanvasWidth)
                {
                    Row = i;
                    Column = lineList.Count();
                    if (Column > 0)
                    {
                        Y = lineList.First().Position.Y;
                    }
                    else
                    {
                        Y = project.LineHeights.Take(i).Sum();
                    }

                    X = maxWidth;
                    Position = new Point(X, Y);
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                double maxHieght = project.LineHeights.Sum();
                if (maxHieght + Height <= project.MaxCanvasHeight)
                {
                    Row = project.LineHeights.Count;
                    Column = 0;
                    double X = 0, Y = 0;
                    Y = maxHieght;
                    Position = new Point(X, Y);
                    project.LineHeights.Add(Height);
                    found = true;
                }
            }
            if (found)
            {
                Added = true;
                this.project = project;
                project.ImagesList.Add(this);
            }
        }

        internal XmlNode GetXmlImage(XmlDocument document)
        {
            XmlNode parent = document.CreateNode(XmlNodeType.Element, "image", "");

            XmlNode id = document.CreateNode(XmlNodeType.Element, "id", "");
            id.InnerText = Id;

            XmlNode name = document.CreateNode(XmlNodeType.Element, "name", "");
            name.InnerText = ImageName;

            XmlNode path = document.CreateNode(XmlNodeType.Element, "path", "");
            path.InnerText = ImagePath.ToString();

            XmlNode width = document.CreateNode(XmlNodeType.Element, "width", "");
            width.InnerText = Width.ToString();

            XmlNode height = document.CreateNode(XmlNodeType.Element, "height", "");
            height.InnerText = Height.ToString();

            XmlNode positionX = document.CreateNode(XmlNodeType.Element, "positionX", "");
            positionX.InnerText = Position.X.ToString();

            XmlNode positionY = document.CreateNode(XmlNodeType.Element, "positionY", "");
            positionY.InnerText = Position.Y.ToString();

            XmlNode row = document.CreateNode(XmlNodeType.Element, "row", "");
            row.InnerText = Row.ToString();

            XmlNode column = document.CreateNode(XmlNodeType.Element, "column", "");
            column.InnerText = Column.ToString();

            parent.AppendChild(id);
            parent.AppendChild(name);
            parent.AppendChild(path);
            parent.AppendChild(width);
            parent.AppendChild(height);
            parent.AppendChild(positionX);
            parent.AppendChild(positionY);
            parent.AppendChild(row);
            parent.AppendChild(column);

            return parent;
        }
    }

    class ProjectFile
    {
        string ProjectName { get; set; }

        internal double MaxCanvasWidth { get; set; }

        internal double MaxCanvasHeight { get; set; }

        internal List<ImageItem> ImagesList { get; set; }

        internal List<double> LineHeights { get; set; }

        internal OrganizeMethod OrganizeMethod { get; set; }

        string CreatorUser { get; set; }

        string CreatedDate { get; set; }

        string LastSaveDate { get; set; }

        internal ProjectFile(string name, double maxWidth, double maxHeight, OrganizeMethod organizeMethod, string creator)
        {
            ProjectName = name;
            MaxCanvasWidth = maxWidth;
            MaxCanvasHeight = maxHeight;
            OrganizeMethod = organizeMethod;
            ImagesList = new List<ImageItem>();
            LineHeights = new List<double>();
            CreatorUser = creator;
            CreatedDate = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
        }

        internal void SaveProject(DirectoryInfo destination)
        {
            if (ImagesList?.Count == 0)
            {
                return;
            }
            LastSaveDate = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            if (!destination.Exists)
            {
                try
                {
                    destination.Create();
                }
                catch (Exception) { }
            }
            if (destination.Exists)
            {
                string path = Path.Combine(destination.FullName, ProjectName);
                int number = 1;
                while (Directory.Exists(path))
                {
                    path = Path.Combine(destination.FullName, ProjectName + "_" + number);
                    number++;
                }
                try
                {
                    destination = Directory.CreateDirectory(path);
                    DirectoryInfo imageDir = Directory.CreateDirectory(Path.Combine(path, "Assets"));   
                    for (int i = 0; i < ImagesList.Count; i++)
                    {
                        try
                        {
                            string newpath = Path.Combine(imageDir.FullName, ImagesList[i].Id + Path.GetExtension(ImagesList[i].ImagePath));
                            File.Copy(ImagesList[i].ImagePath, newpath);
                            ImagesList[i].ImagePath = newpath;
                        }
                        catch (Exception)
                        {
                            ImagesList[i].ImagePath = "";
                        }
                    }


                    using (StreamWriter imageListFile = new StreamWriter(File.Open(Path.Combine(path, "Images.xml"), FileMode.OpenOrCreate)))
                    {
                        string xmlImagesFile = GetXmlImagesFile();
                        imageListFile.Write(xmlImagesFile);
                        imageListFile.Close();
                    }

                    using (StreamWriter projectFile = new StreamWriter(File.Open(Path.Combine(path, ProjectName + ".sip"), FileMode.OpenOrCreate)))
                    {
                        string xmlProjectFile = GetXmlProjectFile();
                        projectFile.Write(xmlProjectFile);
                        projectFile.Close();
                    }
                }
                catch (Exception) { }
            }
        }

        internal string GetXmlProjectFile()
        {
            XmlDocument document = new XmlDocument();

            document.LoadXml("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>\n<project>\n</project>");

            //XmlNode mainNode = document.CreateNode(XmlNodeType.Element, "document", "");

            XmlNode mainNode = document.LastChild;

            XmlNode comment = document.CreateNode(XmlNodeType.Comment, "", "");
            comment.InnerText = " kod by Mohammad (https://www.linkedin.com/in/mohammaddiab0) ";
            mainNode.AppendChild(comment);
            XmlNode node;

            node = document.CreateNode(XmlNodeType.Element, "fileType", "");
            node.InnerText = "Sprite Image Project File";
            mainNode.AppendChild(node);

            node = document.CreateNode(XmlNodeType.Element, "version", "");
            node.InnerText = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            mainNode.AppendChild(node);

            node = document.CreateNode(XmlNodeType.Element, "projectName", "");
            node.InnerText = ProjectName;
            mainNode.AppendChild(node);

            node = document.CreateNode(XmlNodeType.Element, "by", "");
            node.InnerText = "Mohammad Diab";
            mainNode.AppendChild(node);

            node = document.CreateNode(XmlNodeType.Element, "maxCanvasWidth", "");
            node.InnerText = MaxCanvasWidth.ToString();
            mainNode.AppendChild(node);

            node = document.CreateNode(XmlNodeType.Element, "maxCanvasHeight", "");
            node.InnerText = MaxCanvasHeight.ToString();
            mainNode.AppendChild(node);

            node = document.CreateNode(XmlNodeType.Element, "relatedFilesCount", "");
            node.InnerText = ImagesList.Count.ToString();
            mainNode.AppendChild(node);

            node = document.CreateNode(XmlNodeType.Element, "organizeMethodId", "");
            node.InnerText = OrganizeMethod.ToString("d");
            mainNode.AppendChild(node);

            node = document.CreateNode(XmlNodeType.Element, "organizeMethodName", "");
            node.InnerText = OrganizeMethod.ToString();
            mainNode.AppendChild(node);

            node = document.CreateNode(XmlNodeType.Element, "creatorUser", "");
            node.InnerText = CreatorUser.ToString();
            mainNode.AppendChild(node);

            node = document.CreateNode(XmlNodeType.Element, "creatorDate", "");
            node.InnerText = CreatedDate.ToString();
            mainNode.AppendChild(node);

            node = document.CreateNode(XmlNodeType.Element, "lastSaveDate", "");
            node.InnerText = LastSaveDate.ToString();
            mainNode.AppendChild(node);

            document.AppendChild(mainNode);

            return Functions.BeautifyXml(document.OuterXml);
        }

        internal string GetXmlImagesFile()
        {
            XmlDocument document = new XmlDocument();

            document.LoadXml("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>\n<document>\n</document>");

            //XmlNode mainNode = document.CreateNode(XmlNodeType.Element, "document", "");

            XmlNode mainNode = document.LastChild;

            XmlNode comment = document.CreateNode(XmlNodeType.Comment, "", "");
            comment.InnerText = " kod by Mohammad (https://www.linkedin.com/in/mohammaddiab0) ";
            mainNode.AppendChild(comment);

            XmlNode linesNode = document.CreateNode(XmlNodeType.Element, "lines", "");
            for (int i = 0; i < LineHeights.Count; i++)
            {
                XmlNode parent = document.CreateNode(XmlNodeType.Element, "line", "");
                XmlNode key = document.CreateNode(XmlNodeType.Element, "id", "");
                key.InnerText = i.ToString();
                XmlNode value = document.CreateNode(XmlNodeType.Element, "height", "");
                value.InnerText = LineHeights[i].ToString();
                parent.AppendChild(key);
                parent.AppendChild(value);
                linesNode.AppendChild(parent);
            }

            XmlNode imagesNode = document.CreateNode(XmlNodeType.Element, "images", "");
            foreach (var item in ImagesList)
            {
                XmlNode parent = item.GetXmlImage(document);
                imagesNode.AppendChild(parent);
            }

            mainNode.AppendChild(linesNode);
            mainNode.AppendChild(imagesNode);
            document.AppendChild(mainNode);

            return Functions.BeautifyXml(document.OuterXml);
        }

        internal Point GetCanvasDimensions()
        {
            Point result = new Point(0, 0);
            if (ImagesList.Count > 0)
            {
                var x = ImagesList.Max(it => it.Position.X + it.Width);
                var y = ImagesList.Max(it => it.Position.Y + it.Height);
                result = new Point(x, y);
            }
            return result;
        }

    }
}

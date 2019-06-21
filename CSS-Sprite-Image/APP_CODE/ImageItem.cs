using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Xml;
using System.Xml.Serialization;

namespace CSS_Sprite_Image
{
    public class ImageItem
    {
        [XmlElement("id")]
        public string Id { get; set; }

        [XmlElement("path")]
        public string ImagePath { get; set; }

        [XmlElement("name")]
        public string ImageName { get; set; }

        [XmlElement("width")]
        public double Width { get; set; }

        [XmlElement("height")]
        public double Height { get; set; }

        [XmlElement("positionX")]
        public double PositionX { get; set; }

        [XmlElement("positionY")]
        public double PositionY { get; set; }

        [XmlElement("row")]
        public int Row { get; set; }

        [XmlElement("column")]
        public int Column { get; set; }

        internal bool Added { get; set; } = false;

        internal ProjectFile project;

        public ImageItem()
        {
            Added = true;
        }

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
                if (project.LineHeights[i].Height == Height)
                {
                    lines.Add(i);
                }
            }

            bool found = false;
            foreach (int i in lines)
            {
                var lineList = project.ImagesList.Where(it => it.Row == i);
                var maxWidth = lineList.Max(it => (it.PositionX + it.Width));
                if (maxWidth + Width <= project.MaxCanvasWidth)
                {
                    Row = i;
                    Column = lineList.Count();
                    if (Column > 0)
                    {
                        PositionY = lineList.First().PositionY;
                    }
                    else
                    {
                        PositionY = project.LineHeights.Take(i).Select(it => it.Height).Sum();
                    }

                    PositionX = maxWidth;
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                double maxHieght = project.LineHeights.Select(it => it.Height).Sum();
                if (maxHieght + Height <= project.MaxCanvasHeight)
                {
                    Row = project.LineHeights.Count;
                    Column = 0;
                    PositionX = 0;
                    PositionY = maxHieght;
                    project.LineHeights.Add(new LineHeight(project.LineHeights.Count, Height));
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
            positionX.InnerText = PositionX.ToString();

            XmlNode positionY = document.CreateNode(XmlNodeType.Element, "positionY", "");
            positionY.InnerText = PositionY.ToString();

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

    public class LineHeight
    {
        [XmlElement("id")]
        public int Id { get; set; }

        [XmlElement("height")]
        public double Height { get; set; }

        public LineHeight()
        {
        }

        internal LineHeight(int id, double height)
        {
            Id = id;
            Height = height;
        }
    }

    [XmlRoot("project")]
    public class ProjectFile
    {
        [XmlElement("!--")]
        public const string By = "kod by Mohammad (https://www.linkedin.com/in/mohammaddiab0)";

        [XmlElement("fileType")]
        public const string FileType = "Sprite Image Project File";

        [XmlElement("version")]
        public string Version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

        [XmlElement("projectName")]
        public string ProjectName { get; set; }

        [XmlElement("maxCanvasWidth")]
        public double MaxCanvasWidth { get; set; }

        [XmlElement("maxCanvasHeight")]
        public double MaxCanvasHeight { get; set; }

        [XmlArray("images")]
        [XmlArrayItem("image")]
        public List<ImageItem> ImagesList { get; set; }

        [XmlArray("lines")]
        [XmlArrayItem("line")]
        public List<LineHeight> LineHeights { get; set; }

        private int organizeMethodId;

        [XmlElement("organizeMethodId")]
        public int OrganizeMethodId
        {
            get => organizeMethodId;
            set
            {
                organizeMethodId = value;
                OrganizeMethod = (OrganizeMethod)organizeMethodId;
                OrganizeMethodName = OrganizeMethod.ToString();
            }
        }

        //[XmlElement("organizeMethodName")]
        public string OrganizeMethodName { get; set; }

        public OrganizeMethod OrganizeMethod { get; set; }

        [XmlElement("creatorUser")]
        public string CreatorUser { get; set; }

        [XmlElement("creatorDate")]
        public string CreatedDate { get; set; }

        [XmlElement("lastSaveDate")]
        public string LastSaveDate { get; set; }

        public ProjectFile()
        {
            ImagesList = new List<ImageItem>();
            LineHeights = new List<LineHeight>();
        }

        public ProjectFile(string name, double maxWidth, double maxHeight, OrganizeMethod organizeMethod, string creator)
        {
            ProjectName = name;
            MaxCanvasWidth = maxWidth;
            MaxCanvasHeight = maxHeight;
            OrganizeMethodId = (int)organizeMethod;
            ImagesList = new List<ImageItem>();
            LineHeights = new List<LineHeight>();
            CreatorUser = creator;
            CreatedDate = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
        }

        public void SaveProject(DirectoryInfo destination)
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
                        string xmlImagesFile = CreateXmlImagesFile();
                        imageListFile.Write(xmlImagesFile);
                        imageListFile.Close();
                    }

                    using (StreamWriter projectFile = new StreamWriter(File.Open(Path.Combine(path, ProjectName + ".sip"), FileMode.OpenOrCreate)))
                    {
                        string xmlProjectFile = CreateXmlProjectFile();
                        projectFile.Write(xmlProjectFile);
                        projectFile.Close();
                    }
                }
                catch (Exception) { }
            }
        }

        public string CreateXmlProjectFile()
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
            node.InnerText = OrganizeMethodId.ToString("d");
            mainNode.AppendChild(node);

            node = document.CreateNode(XmlNodeType.Element, "organizeMethodName", "");
            node.InnerText = OrganizeMethodId.ToString();
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

        public string CreateXmlImagesFile()
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

        public static ProjectFile LoadProject(DirectoryInfo source, string projectFilePath)
        {
            ProjectFile result = null;
            if (source is null || !source.Exists)
            {
                return result;
            }
            DirectoryInfo assetsFolder = new DirectoryInfo(Path.Combine(source.FullName, "Assets"));
            if (!assetsFolder.Exists)
            {
                return result;
            }

            FileInfo[] imageFiles = assetsFolder.GetFiles();
            FileInfo projectFile = new FileInfo(projectFilePath);

            if (imageFiles.Length == 0 || !projectFile.Exists)
            {
                return result;
            }

            var serializer = new XmlSerializer(typeof(ProjectFile));
            using (Stream stream = projectFile.Open(FileMode.Open, FileAccess.Read))
            {
                //XmlDocument doc = new XmlDocument();
                //doc.Load(stream);
                //XmlReader xmlreader = XmlReader.Create(stream);
                result = (ProjectFile)serializer.Deserialize(stream);
                //xmlreader.Close();
                stream.Close();
                foreach (ImageItem it in result.ImagesList)
                {
                    it.ImagePath = Path.Combine(source.FullName, it.ImagePath);
                }
            }
            return result;
        }

        public Point GetCanvasDimensions()
        {
            Point result = new Point(0, 0);
            if (ImagesList.Count > 0)
            {
                var x = ImagesList.Max(it => it.PositionX + it.Width);
                var y = ImagesList.Max(it => it.PositionY + it.Height);
                result = new Point(x, y);
            }
            return result;
        }

    }
}

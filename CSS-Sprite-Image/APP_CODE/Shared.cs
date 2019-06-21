using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    }
}

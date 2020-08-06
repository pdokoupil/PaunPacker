using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using PaunPacker.Core.Atlas;
using PaunPacker.Core.Types;

namespace PaunPacker
{
    /// <summary>
    /// Represents a PaunPacker project
    /// </summary>
    public class Project : IXmlSerializable
    {
        /// <summary>
        /// Path of the project (project is contained within a directory)
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Name of the project
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Images that are opened in the project
        /// </summary>
        public IEnumerable<PPImage> OpenedImages { get; set; }

        /// <summary>
        /// Texture atlas yielded by the project
        /// </summary>
        public TextureAtlas TextureAtlas { get; set; }
        
        /// <summary>
        /// Implements the GetSchema method
        /// </summary>
        /// <returns>Null</returns>
        public XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Deserializes the project by reading it from XML (via <paramref name="reader"/>)
        /// </summary>
        /// <param name="reader">XmlReader from which the project should be read</param>
        public void ReadXml(XmlReader reader)
        {
            //Prevent Code analysis warnings about reader
            Contract.Requires(reader != null);

            reader.MoveToContent();
            Path = reader.GetAttribute(nameof(Path));
            Name = reader.GetAttribute(nameof(Name));
            var openedImgs = new List<PPImage>();
            reader.Read();

            if (reader.IsStartElement() && reader.Name == nameof(OpenedImages))
            {
                while (!reader.EOF)
                {
                    if (reader.IsStartElement("Image"))
                    {
                        var imagePath = reader.GetAttribute("ImagePath");
                        if (imagePath != null)
                        {
#pragma warning disable CA2000 // The loaded image is owned by the BitmapManager
                            var loadedImage = BitmapManager.LoadBitmap(imagePath);
#pragma warning restore CA2000
                            if (loadedImage != null)
                            {
                                //Load of image succeeded
                                openedImgs.Add(loadedImage);
                            }
                        }
                        reader.Read();
                    }
                    else
                    {
                        reader.Read();
                    }
                }
            }
            OpenedImages = openedImgs;
        }

        /// <summary>
        /// Serializes the project into XML
        /// </summary>
        /// <param name="writer">XmlWriter that should be used for serialization</param>
        public void WriteXml(XmlWriter writer)
        {
            //Prevent Code analysis warnings about reader
            Contract.Requires(writer != null);

            writer.WriteAttributeString(nameof(Path), Path);
            writer.WriteAttributeString(nameof(Name), Name);

            if (OpenedImages != null && OpenedImages.Any())
            {
                writer.WriteStartElement(nameof(OpenedImages));

                foreach (var Image in OpenedImages)
                {
                    writer.WriteStartElement(nameof(Image));
                    writer.WriteAttributeString(nameof(Image.ImagePath), Image.ImagePath);
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }
        }
    }
}

using System.IO;
using System.Xml.Serialization;

namespace PersonnelInformationMerger.Core.Helpers
{
    public static class XmlHelper
    {
        public static T Deserialize<T>(string filePath)
        {
            FileHelper.ValidateFilePath(filePath);

            var serializer = new XmlSerializer(typeof(T));

            using var streamReader = new StreamReader(filePath);

            return (T)serializer.Deserialize(streamReader);
        }

        public static void Serialize<T>(T source, string filePath)
        {
            FileHelper.ValidateFilePath(filePath);

            var serializer = new XmlSerializer(typeof(T));

            using var streamWriter = new StreamWriter(filePath);

            serializer.Serialize(streamWriter, source);
        }


    }
}

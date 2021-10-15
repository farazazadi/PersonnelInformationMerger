using System.IO;
using System.Threading.Tasks;

namespace PersonnelInformationMerger.Core.Helpers
{
    public static class FileHelper
    {
        public static async Task<string> ReadAllTextAsync(string filePath)
        {
            ValidateFilePath(filePath);

            var text = await File.ReadAllTextAsync(filePath);

            return text;
        }

        public static string ReadAllText(string filePath)
        {
            ValidateFilePath(filePath);

            var text = File.ReadAllText(filePath);

            return text;
        }
        
        public static void WriteAllText(string filePath, string content)
        {
            File.WriteAllText(filePath, content);
        }
        
        public static async Task WriteAllTextAsync(string filePath, string content)
        {
            await File.WriteAllTextAsync(filePath, content);
        }
        
        internal static void ValidateFilePath(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"File ({filePath}) Not Found!");
        }
    }
}

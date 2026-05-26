using System;
using System.IO;

namespace Project147.PlatformServices.Save
{
    public sealed class FileTextSaveStorage : ITextSaveStorage
    {
        private readonly string path;

        public FileTextSaveStorage(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("Save path is required.", nameof(path));
            }

            this.path = path;
        }

        public bool Exists
        {
            get { return File.Exists(path); }
        }

        public string ReadAllText()
        {
            return File.ReadAllText(path);
        }

        public void WriteAllText(string text)
        {
            var directory = Path.GetDirectoryName(path);

            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(path, text ?? string.Empty);
        }

        public void Delete()
        {
            if (Exists)
            {
                File.Delete(path);
            }
        }
    }
}

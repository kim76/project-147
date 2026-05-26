using Project147.PlatformServices.Save;

namespace Project147.Tests.EditMode.PlatformServices.Save
{
    public sealed class InMemoryTextSaveStorage : ITextSaveStorage
    {
        private string text;

        public bool Exists
        {
            get { return text != null; }
        }

        public string ReadAllText()
        {
            return text;
        }

        public void WriteAllText(string value)
        {
            text = value;
        }

        public void Delete()
        {
            text = null;
        }
    }
}

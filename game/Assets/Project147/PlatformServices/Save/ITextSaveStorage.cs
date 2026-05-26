namespace Project147.PlatformServices.Save
{
    public interface ITextSaveStorage
    {
        bool Exists { get; }

        string ReadAllText();

        void WriteAllText(string text);

        void Delete();
    }
}

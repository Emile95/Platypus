namespace PlatypusRepository.Folder.Configuration
{
    public abstract class FileAttribute : Attribute
    {
        public string FileName { get; set; }
        public virtual string Extension { get; protected set; }

    }
}

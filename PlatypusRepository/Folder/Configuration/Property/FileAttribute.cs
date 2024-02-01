namespace PlatypusRepository.Folder.Configuration.Property
{
    public abstract class FileAttribute : FolderEntityPropertyAttribute
    {
        public string FileName { get; set; }
        public virtual string Extension { get; protected set; }
    }
}

namespace PlatypusRepository.Folder.Configuration
{
    public abstract class FolderEntityCreatorAttribute : Attribute
    {
        public abstract void OnCreate(string entityDirectoryPath);
    }
}

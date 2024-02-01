namespace PlatypusRepository.Folder.Configuration.Class
{
    public class CreateFolderAttribute : FolderEntityCreatorAttribute
    {
        public string Name { get; set; }

        public override void OnCreate(string entityDirectoryPath)
        {
            string folderPath = Path.Combine(entityDirectoryPath, Name);
            Directory.CreateDirectory(folderPath);
        }
    }
}

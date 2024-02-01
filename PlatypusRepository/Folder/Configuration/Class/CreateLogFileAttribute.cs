namespace PlatypusRepository.Folder.Configuration.Class
{
    public class CreateEmptyFileAttribute : FolderEntityCreatorAttribute
    {
        public string Name { get; set; }
        public string Extension { get; set; }

        public override void OnCreate(string entityDirectoryPath)
        {
            string fileName = string.IsNullOrEmpty(Extension) ? Name : Name + "." + Extension;
            string filePath = Path.Combine(entityDirectoryPath, fileName);
            File.Create(filePath);
        }
    }
}

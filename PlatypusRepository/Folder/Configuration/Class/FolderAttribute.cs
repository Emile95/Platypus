using PlatypusRepository.Folder.Abstract;

namespace PlatypusRepository.Folder.Configuration.Class
{
    public class FolderAttribute : FolderEntityClassAttribute, IFolderEntityClassResolver
    {
        public string Name { get; set; }

        public void Resolve(string directoryPath)
        {
            string folderPath = Path.Combine(directoryPath, Name);
            Directory.CreateDirectory(folderPath);
        }
    }
}

using PlatypusRepository.Folder.Abstract;

namespace PlatypusRepository.Folder.Configuration.Class
{
    public class FileAttribute : FolderEntityClassAttribute, IFolderEntityClassResolver
    {
        public string Name { get; set; }
        public string Extension { get; set; }

        public void Resolve(string directoryPath)
        {
            string fileName = string.IsNullOrEmpty(Extension) ? Name : Name + "." + Extension;
            string filePath = Path.Combine(directoryPath, fileName);
            FileStream stream = File.Create(filePath);
            stream.Close();
        }
    }
}

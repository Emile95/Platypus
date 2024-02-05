using PlatypusRepository.FolderPath.Abstract;
using PlatypusRepository.FolderPath.Folder.Operator;

namespace PlatypusRepository.FolderPath.Folder
{
    public class FolderRepository<EntityType> : FolderPathRepository<EntityType>
        where EntityType : class
    {
        public FolderRepository(string directoryPath)
        {
            _addOperator = new FolderRepositoryAddOperator<EntityType>(_entityType, directoryPath, _folderEntityHandler);
            _updateOperator = new FolderRepositoryUpdateOperator<EntityType>(_entityType, directoryPath, _folderEntityHandler);
            _removeOperator = new FolderRepositoryRemoveOperator<EntityType>(_entityType, directoryPath, _folderEntityHandler);
            _consumeOperator = new FolderRepositoryConsumeOperator<EntityType>(_entityType, directoryPath, _folderEntityHandler);
        }
    }
}

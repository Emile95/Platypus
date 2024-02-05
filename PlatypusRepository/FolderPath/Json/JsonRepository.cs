using PlatypusRepository.FolderPath.Abstract;
using PlatypusRepository.FolderPath.Json.Operator;

namespace PlatypusRepository.FolderPath.Json
{
    public class JsonRepository<EntityType> : FolderPathRepository<EntityType>
        where EntityType : class
    {
        public JsonRepository(string directoryPath)
        {
            _addOperator = new JsonRepositoryAddOperator<EntityType>(_entityType, directoryPath, _folderEntityHandler);
            _updateOperator = new JsonRepositoryUpdateOperator<EntityType>(_entityType, directoryPath, _folderEntityHandler);
            _removeOperator = new JsonRepositoryRemoveOperator<EntityType>(_entityType, directoryPath, _folderEntityHandler);
            _consumeOperator = new JsonRepositoryConsumeOperator<EntityType>(_entityType, directoryPath, _folderEntityHandler);
        }
    }
}

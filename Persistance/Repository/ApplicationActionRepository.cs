using Newtonsoft.Json;
using Persistance.Entity;

namespace Persistance.Repository
{
    public class ApplicationActionRepository : MemberOfApplicationRepository
    {
        public void SaveActionRunResult(string actionGuid, int runNumber, ApplicationActionResultEntity result)
        {
            string actionRunDirectoryPath = Path.Combine(ApplicationPaths.GetActionRunsDirectoryPath(actionGuid), runNumber.ToString());
            string resultFilePath = Path.Combine(actionRunDirectoryPath, "result.json");
            File.WriteAllText(resultFilePath, JsonConvert.SerializeObject(result, Formatting.Indented));
        }

        public List<string> RemoveActionsOfApplication(string applicationGuid)
        {
            return RemoveByApplicationGuid(ApplicationPaths.ACTIONSDIRECTORYPATH, applicationGuid);
        }

        public void SaveAction(string actionGuid)
        {
            string actionDirectoryPath = ApplicationPaths.GetActionDirectoryPath(actionGuid);
            if (Directory.Exists(actionDirectoryPath)) return;
            Directory.CreateDirectory(actionDirectoryPath);

            string actionRunsDirectoryPath = ApplicationPaths.GetActionRunsDirectoryPathByBasePath(actionDirectoryPath);
            Directory.CreateDirectory(actionRunsDirectoryPath);
            File.WriteAllText(
                ApplicationPaths.GetActionLastRunNumberFilePathByBasePath(actionRunsDirectoryPath),
                "0"
            );
        }

        public void SaveActionRun(string actionGuid, int runNumber)
        {
            string actionRunsDirectoryPath = ApplicationPaths.GetActionRunsDirectoryPath(actionGuid);
            SaveActionRunByBasePath(actionRunsDirectoryPath, runNumber);
        }

        public void SaveRunningAction(RunningApplicationActionEntity entity)
        {
            string runningActionFilePath = ApplicationPaths.GetRunningActionFilePath(entity.Guid);
            File.WriteAllText(runningActionFilePath, JsonConvert.SerializeObject(entity, Formatting.Indented));
        }

        public void RemoveRunningAction(string guid)
        {
            string runningActionFilePath = ApplicationPaths.GetRunningActionFilePath(guid);
            File.Delete(runningActionFilePath);
        }

        public List<RunningApplicationActionEntity> LoadRunningActions()
        {
            List<RunningApplicationActionEntity> list = new List<RunningApplicationActionEntity>();

            string[] files = Directory.GetFiles(ApplicationPaths.RUNNINGACTIONSDIRECTORYPATH);

            foreach(string file in files)
            {
                string json = File.ReadAllText(file);
                RunningApplicationActionEntity entity = JsonConvert.DeserializeObject<RunningApplicationActionEntity>(json);
                list.Add(entity);
            }

            return list;
        }

        public void SaveActionRunByBasePath(string basePath, int runNumber)
        {
            string actionRunDirectoryPath = Path.Combine(basePath, runNumber.ToString());
            Directory.CreateDirectory(actionRunDirectoryPath);
            FileStream stream = File.Create(Path.Combine(actionRunDirectoryPath, ApplicationPaths.ACTIONLOGFILENAME));
            stream.Close();
        }

        public int GetAndIncrementActionRunNumber(string actionGuid)
        {
            string actionLastRunNumberFilePath = ApplicationPaths.GetActionLastRunNumberFilePath(actionGuid);
            return GetAndIncrementActionRunNumberByBasePath(actionLastRunNumberFilePath);
        }

        public int GetAndIncrementActionRunNumberByBasePath(string basePath)
        {
            string actionLastRunNumberFilePath = ApplicationPaths.GetActionLastRunNumberFilePathByBasePath(basePath);
            string lastRunNumberStr = File.ReadAllText(actionLastRunNumberFilePath);
            int lastRunNumber = Convert.ToInt32(lastRunNumberStr);
            File.WriteAllText(actionLastRunNumberFilePath, (++lastRunNumber).ToString());
            return lastRunNumber;
        }

        public string GetRunActionLogFilePath(string actionGuid, int runNumber)
        {
            return ApplicationPaths.GetActionRunLogFilePath(actionGuid, runNumber);
        }
    }
}

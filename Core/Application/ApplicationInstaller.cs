﻿using Core.Persistance.Repository;
using PlatypusFramework.Core.Application;
using PlatypusFramework.Configuration.Application;
using PlatypusFramework.Configuration.ApplicationAction;
using System.Reflection;
using PlatypusUtils;
using PlatypusFramework.Configuration.User;
using Core.Exceptions;
using PlatypusRepository;
using System.IO.Compression;
using Core.Persistance.Entity;

namespace Core.Application
{
    internal class ApplicationInstaller
    {
        private readonly IRepositoryAddOperator<ApplicationEntity> _applicationRepositoryAddOperator;
        private readonly IRepositoryRemoveOperator<ApplicationEntity> _applicationRepositoryRemoveOperator;

        private readonly IRepositoryAddOperator<ApplicationActionEntity> _applicationActionRepositoryAddOperator;
        private readonly IRepositoryRemoveOperator<ApplicationActionEntity> _applicationActionRepositoryRemoveOperator;

        private readonly UserRepository _userRepository;

        internal ApplicationInstaller(
            ApplicationRepository applicationRepository,
            ApplicationActionRepository applicationActionRepository,
            UserRepository userRepository
        )
        {
            _applicationRepositoryAddOperator = applicationRepository;
            _applicationRepositoryRemoveOperator = applicationRepository;

            _applicationActionRepositoryAddOperator = applicationActionRepository;
            _applicationActionRepositoryRemoveOperator = applicationActionRepository;

            _userRepository = userRepository;
        }

        internal PlatypusApplicationBase InstallApplication(string newGuid, string applicationPath)
        {
            FileInfo applicationFileInfo = new FileInfo(applicationPath);
            if (applicationFileInfo.Extension != ".platypus")
                throw new InvalidPlatypusApplicationPackageException(applicationFileInfo.Name, "wrong file extension");

            ApplicationEntity entity = new ApplicationEntity()
            {
                Guid = newGuid
            };

            //Directory.CreateDirectory(entity.DirectoryPath);

            ExtractPackage(entity, applicationPath);

            _applicationRepositoryAddOperator.Add(entity);

            try
            {
                PlatypusApplicationBase platypusApplication = PluginResolver.InstanciateImplementationFromRawBytes<PlatypusApplicationBase>(entity.AssemblyRaw);

                Type type = platypusApplication.GetType();
                MethodInfo[] methods = type.GetMethods();

                foreach (MethodInfo method in methods)
                {
                    if (InstallAction(newGuid, method)) continue;
                    if (InstallUserConnectionMethod(platypusApplication, newGuid, method)) continue;
                }

                ApplicationInstallEnvironment env = new ApplicationInstallEnvironment();
                env.ApplicationGuid = newGuid;

                platypusApplication.Install(env);

                return platypusApplication;
            } catch(Exception)
            {
                _applicationRepositoryRemoveOperator.Remove(entity);
                return null;
            }
        }

        internal void UninstallApplication(PlatypusApplicationBase application, string applicationGuid)
        {
            ApplicationInstallEnvironment env = new ApplicationInstallEnvironment();
            env.ApplicationGuid = applicationGuid;

            application.Uninstall(env);

            _applicationRepositoryRemoveOperator.Remove(new ApplicationEntity() { Guid = applicationGuid });

            string[] applicationActionsNames = application.GetAllApplicationActionNames();
            foreach(string applicationActionsName in applicationActionsNames)
                _applicationActionRepositoryRemoveOperator.Remove(new ApplicationActionEntity()
                {
                    Guid = applicationActionsName + applicationGuid
                });
        }

        private bool InstallAction(string applicationGuid, MethodInfo methodInfo)
        {
            ActionDefinitionAttribute actionDefinition = methodInfo.GetCustomAttribute<ActionDefinitionAttribute>();
            if (actionDefinition == null) return false;

            _applicationActionRepositoryAddOperator.Add(new ApplicationActionEntity() { 
                Guid = actionDefinition.Name + applicationGuid
            });
            return true;
        }

        internal bool InstallUserConnectionMethod(PlatypusApplicationBase application, string applicationGuid, MethodInfo methodInfo)
        {
            UserConnectionMethodCreatorAttribute userConnectionMethodCreatorAttribute = methodInfo.GetCustomAttribute<UserConnectionMethodCreatorAttribute>();
            if (userConnectionMethodCreatorAttribute == null) return false;

            IUserConnectionMethod connectionMethod = methodInfo.Invoke(application, new object[] { }) as IUserConnectionMethod;
            string connectionMethodName = connectionMethod.GetName();
            _userRepository.SaveUserConnectionMethod(new UserConnectionMethodEntity()
            {
                Description = connectionMethod.GetDescription(),
                Name = connectionMethodName,
                Guid = connectionMethodName+applicationGuid
            });

            return true;
        }

        private void ExtractPackage(ApplicationEntity entity, string sourceDirectoryPath)
        {
            string temporaryDirectoryPath = Path.Combine(Path.GetTempPath(), entity.Guid);
            Directory.CreateDirectory(temporaryDirectoryPath);

            ZipFile.ExtractToDirectory(sourceDirectoryPath, temporaryDirectoryPath);
            string[] dllFiles = Directory.GetFiles(temporaryDirectoryPath, "*.dll");
            if (dllFiles.Length == 0) return;

            entity.AssemblyRaw = File.ReadAllBytes(dllFiles[0]);

            /*string[] directoriesPath = Directory.GetDirectories(temporaryDirectoryPath);
            if (directoriesPath.Length > 0)
                foreach (string directoryPath in directoriesPath)
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);
                    Directory.Move(directoryPath, Path.Combine(entity.DirectoryPath, directoryInfo.Name));
                }*/
        }
    }
}

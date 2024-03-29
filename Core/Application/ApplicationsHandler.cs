﻿using PlatypusFramework.Configuration.Application;
using PlatypusUtils;
using PlatypusRepository;
using Core.Persistance.Entity;
using Core.Abstract;
using Core.Application.Abstract;
using PlatypusAPI.Application;

namespace Core.Application
{
    public class ApplicationsHandler : 
        IRepositoryRemoveOperator<PlatypusApplicationBase, string>,
        IRepositoryAddOperator<PlatypusApplicationBase>,
        IRepositoryConsumeOperator<ApplicationInfo>,
        IGuidValidator<PlatypusApplicationBase>,
        IGetterByGuid<PlatypusApplicationBase>,
        IServerStarter<ApplicationsHandler>
    {
        private readonly Dictionary<string, PlatypusApplicationBase> _applications;
        private readonly IRepositoryConsumeOperator<ApplicationEntity> _applicationConsumeOperator;
        private readonly IApplicationResolver<PlatypusApplicationBase> _applicationResolver;

        public ApplicationsHandler(
            IRepositoryConsumeOperator<ApplicationEntity> applicationConsumeOperator,
            IApplicationResolver<PlatypusApplicationBase> applicationResolver
        )
        {
            _applications = new Dictionary<string, PlatypusApplicationBase>();
            _applicationConsumeOperator = applicationConsumeOperator;
            _applicationResolver = applicationResolver;
        }

        public PlatypusApplicationBase Add(PlatypusApplicationBase application)
        {
            _applications.Add(application.ApplicationGuid, application);
            _applicationResolver.Resolve(application);
            return application;
        }

        public void Remove(string id)
        {
            _applications.Remove(id);
        }

        public bool Validate(string guid)
        {
            return _applications.ContainsKey(guid);
        }

        public PlatypusApplicationBase Get(string guid)
        {
            return _applications[guid];
        }

        public void Start()
        {
            _applicationConsumeOperator.Consume((entity) => {

                PlatypusApplicationBase applicationBase = PluginResolver.InstanciateImplementationFromRawBytes<PlatypusApplicationBase>(entity.AssemblyRaw);
                applicationBase.ApplicationGuid = entity.Guid;
                _applications.Add(applicationBase.ApplicationGuid, applicationBase);
                _applicationResolver.Resolve(applicationBase);
            });
        }

        public void Consume(Action<ApplicationInfo> consumer, Predicate<ApplicationInfo> condition = null)
        {
            foreach(PlatypusApplicationBase application in _applications.Values)
            {
                ApplicationInfo info = new ApplicationInfo()
                {
                    Guid = application.ApplicationGuid,
                    Name = application.GetApplicationName()
                };

                if (condition != null && condition(info) == false) continue;
                consumer(info);
            }
        }
    }
}

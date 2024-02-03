using Core.Exceptions;
using PlatypusFramework.Core.Event;

namespace Core.Event.Abstract
{
    public interface IEventHandlerRunner
    {
        T Run<T>(EventHandlerType eventHandlerType, EventHandlerEnvironment eventEnv, Func<EventHandlerException, T> exceptionObjectCreator)
            where T : class;
    }
}

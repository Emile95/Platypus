using PlatypusApplicationFramework.Configuration;
using PlatypusApplicationFramework.Configuration.Exceptions;
using System.Reflection;

namespace PlatypusApplicationFramework.Confugration
{
    public static class ParameterEditorObjectResolver
    {
        public static object ResolveByDictionnary(Type type, Dictionary<string, object> dict)
        {
            object newObject = Activator.CreateInstance(type);

            PropertyInfo[] propertyInfos = type.GetProperties();
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                ParameterEditorAttribute parameterEditor = propertyInfo.GetCustomAttribute<ParameterEditorAttribute>();
                if (parameterEditor == null) continue;

                if (dict.ContainsKey(parameterEditor.Name))
                {
                    propertyInfo.SetValue(newObject, dict[parameterEditor.Name]);
                    continue;
                }

                if (parameterEditor.IsRequired)
                    throw new ParameterEditorFieldRequiredException(parameterEditor.Name);

                if (parameterEditor.DefaultValue != null)
                    propertyInfo.SetValue(newObject, parameterEditor.DefaultValue);
            }

            return newObject;
        }

        public static ObjectType ResolveByDictionnary<ObjectType>(Dictionary<string,object> dict)
            where ObjectType : class, new()
        {
            Type typeOfObject = typeof(ObjectType);

            return ResolveByDictionnary(typeOfObject, dict) as ObjectType;
        }
    }
}

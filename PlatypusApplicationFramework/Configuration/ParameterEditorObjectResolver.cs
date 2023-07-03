using PlatypusAPI.Exceptions;
using PlatypusApplicationFramework.Configuration;
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
                    object convertedValue = Convert.ChangeType(dict[parameterEditor.Name], propertyInfo.PropertyType);
                    propertyInfo.SetValue(newObject, convertedValue);
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

        public static ObjectType CreateDefaultObject<ObjectType>()
            where ObjectType : class, new()
        {
            return CreateDefaultObject(typeof(ObjectType)) as ObjectType;
        }

        public static object CreateDefaultObject(Type type)
        {
            PropertyInfo[] propertyInfos = type.GetProperties();
            object newObject = Activator.CreateInstance(type);
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                ParameterEditorAttribute parameterEditor = propertyInfo.GetCustomAttribute<ParameterEditorAttribute>();
                if (parameterEditor == null) continue;

                if (parameterEditor.DefaultValue != null)
                    propertyInfo.SetValue(newObject, parameterEditor.DefaultValue);
            }

            return newObject;
        }

        public static void ValidateDictionnary(Type type, Dictionary<string, object> dict)
        {
            PropertyInfo[] propertyInfos = type.GetProperties();
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                ParameterEditorAttribute parameterEditor = propertyInfo.GetCustomAttribute<ParameterEditorAttribute>();
                if (parameterEditor == null) continue;

                if (dict.ContainsKey(parameterEditor.Name) == false && parameterEditor.IsRequired)
                    throw new ParameterEditorFieldRequiredException(parameterEditor.Name);
            }
        }
    }
}

using Newtonsoft.Json;
using PlatypusAPI.Exceptions;
using PlatypusFramework.Configuration;
using System.Collections;
using System.Reflection;

namespace PlatypusFramework.Confugration
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
                    if(propertyInfo.PropertyType.IsValueType == false &&
                       propertyInfo.PropertyType.IsEquivalentTo(typeof(string)) == false &&
                       propertyInfo.PropertyType.IsAssignableTo(typeof(object)) &&
                       propertyInfo.PropertyType.IsAssignableTo(typeof(IList)) == false)
                    {
                        string jsonObject = JsonConvert.SerializeObject(dict[parameterEditor.Name]);
                        object newValue = ResolveByDictionnary(propertyInfo.PropertyType, JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonObject));
                        propertyInfo.SetValue(newObject, newValue);
                        continue;
                    }

                    if (propertyInfo.PropertyType.IsAssignableTo(typeof(IList)))
                    {
                        object newList = Activator.CreateInstance(propertyInfo.PropertyType);
                        Type genericArg = propertyInfo.PropertyType.GetGenericArguments()[0];

                        IList list = (IList)dict[parameterEditor.Name];
                        foreach (object member in list)
                        {
                            string jsonObject = JsonConvert.SerializeObject(member);
                            object newValue = ResolveByDictionnary(genericArg, JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonObject));
                            MethodInfo addMethodInfo = newList.GetType().GetMethod("Add");
                            addMethodInfo.Invoke(newList, new object[] { newValue });
                        }

                        propertyInfo.SetValue(newObject, newList);

                        continue;
                    }

                    if(propertyInfo.PropertyType.IsEnum)
                    {
                        dict[parameterEditor.Name] = Enum.Parse(propertyInfo.PropertyType, dict[parameterEditor.Name].ToString());
                        propertyInfo.SetValue(newObject, dict[parameterEditor.Name]);
                        continue;
                    }

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

                if (propertyInfo.PropertyType.IsValueType == false &&
                    propertyInfo.PropertyType.IsEquivalentTo(typeof(string)) == false &&
                    propertyInfo.PropertyType.IsAssignableTo(typeof(object)) &&
                    propertyInfo.PropertyType.IsAssignableTo(typeof(IList)) == false)
                {
                    if (dict[parameterEditor.Name] == null) continue;
                    string jsonObject = JsonConvert.SerializeObject(dict[parameterEditor.Name]);
                    ValidateDictionnary(propertyInfo.PropertyType, JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonObject));
                    continue;
                }

                if (propertyInfo.PropertyType.IsAssignableTo(typeof(IList)))
                {
                    if (dict[parameterEditor.Name] == null) continue;
                    IList list = (IList)dict[parameterEditor.Name];
                    if (list.Count == 0) continue;

                    Type genericArg = propertyInfo.PropertyType.GetGenericArguments()[0];

                    foreach (object member in list)
                    {
                        string jsonObject = JsonConvert.SerializeObject(member);
                        ResolveByDictionnary(genericArg, JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonObject));
                    }

                    continue;
                }
            }
        }
    }
}

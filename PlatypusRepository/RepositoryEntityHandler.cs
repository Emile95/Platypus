using PlatypusRepository.Configuration;
using System.Reflection;

namespace PlatypusRepository
{
    public class RepositoryEntityHandler<EntityType, IDType>
    {
        protected Type _entityType;
        protected PropertyInfo _idPropertyInfo;

        internal RepositoryEntityHandler() 
            : this(typeof(EntityType)) { }

        internal RepositoryEntityHandler(Type entityType)
        {
            _entityType = entityType;
            PropertyInfo[] propertyInfos = _entityType.GetProperties();

            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                if (propertyInfo.GetCustomAttribute<RepositoryEntityIDAttribute>() != null)
                {
                    _idPropertyInfo = propertyInfo;
                    break;
                }
            }
        }

        internal IDType GetID(EntityType entity)
        {
            return (IDType)_idPropertyInfo.GetValue(entity);
        }

        internal void SetID(EntityType entity, object value)
        {
            _idPropertyInfo.SetValue(entity, value);
        }

        internal void IterateAttributesOfProperties<AttributeType>(Action<AttributeType, PropertyInfo> consumer)
            where AttributeType : Attribute
        {
            PropertyInfo[] propertyInfos = _entityType.GetProperties();

            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                IEnumerable<AttributeType> attributes = propertyInfo.GetCustomAttributes<AttributeType>();
                if (attributes == null) continue;

                foreach(AttributeType attribute in attributes)
                    consumer(attribute, propertyInfo);
            }
        }

        internal void IterateAttributesOfClass<AttributeType>(Action<AttributeType> consumer)
            where AttributeType : Attribute
        {
            IEnumerable<AttributeType> attributes = _entityType.GetCustomAttributes<AttributeType>();
            if (attributes == null) return;

            foreach (AttributeType attribute in attributes)
                consumer(attribute);
        }
    }
}

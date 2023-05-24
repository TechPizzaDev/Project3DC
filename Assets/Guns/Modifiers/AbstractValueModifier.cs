using System;
using System.Reflection;

namespace Guns.Modifiers
{
    public abstract class AbstractValueModifier<T> : IModifier
    {
        public string description;
        public string attributeName;
        public T amount;

        public abstract void Apply(Gun gun);

        protected FieldType GetAttribute<FieldType>(
            Gun gun,
            out object targetObject,
            out FieldInfo Field)
        {
            string[] paths = attributeName.Split('/');
            string attribute = paths[paths.Length - 1];

            Type type = gun.GetType();
            object target = gun;

            for (int i = 0; i < paths.Length - 1; i++)
            {
                FieldInfo field = type.GetField(paths[i]);
                if (field == null)
                {
                    UnityEngine.Debug.LogError($"Unable to apply modifier" +
                        $" to attribute {attributeName} because it does not exist on {gun}");
                    throw new InvalidPathSpecifiedException(attributeName);
                }
                else
                {
                    target = field.GetValue(target);
                    type = target.GetType();
                }
            }

            FieldInfo attributeField = type.GetField(attribute);
            if (attributeField == null)
            {
                UnityEngine.Debug.LogError($"Unable to apply modifier" +
                        $" to attribute {attributeName} because it does not exist on {gun}");
                throw new InvalidPathSpecifiedException(attributeName);
            }

            Field = attributeField;
            targetObject = target;
            return (FieldType)attributeField.GetValue(target);
        }
    }
}


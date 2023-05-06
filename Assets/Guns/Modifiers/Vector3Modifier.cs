using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Guns.Modifiers
{
    public class Vector3Modifier : AbstractValueModifier<Vector3>
    {
        public override void Apply(Gun gun)
        {
            try
            {
                Vector3 value = GetAttribute<Vector3>(gun, out object targetObject, out FieldInfo field);
                value = new(value.x * amount.x, value.y * amount.y, value.z * amount.z);
                field.SetValue(targetObject, value);
            }
            catch (InvalidPathSpecifiedException) { }
        }
    }
}


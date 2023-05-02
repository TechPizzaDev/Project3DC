using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Guns.Modifiers
{
    public class FloatModifier : AbstractValueModifier<float>
    {
        public override void Apply(Gun gun)
        {
            try
            {
                float value = GetAttribute<float>(gun, out object targetObject, out FieldInfo field);
                value = value * amount;
                field.SetValue(targetObject, value);
            }
            catch (InvalidPathSpecifiedException) { }
        }
    }
}


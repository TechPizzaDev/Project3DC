using System.Reflection;
using static UnityEngine.ParticleSystem;

namespace Guns.Modifiers
{
    public class DamageModifier : AbstractValueModifier<float>
    {
        public override void Apply(Gun gun)
        {
            try
            {
                MinMaxCurve damageCurve = GetAttribute<MinMaxCurve>(
                    gun,
                    out object targetObject,
                    out FieldInfo field
                );

                switch (damageCurve.mode)
                {
                    case UnityEngine.ParticleSystemCurveMode.TwoConstants:
                        damageCurve.constantMin *= amount;
                        damageCurve.constantMax *= amount;
                        break;
                    case UnityEngine.ParticleSystemCurveMode.TwoCurves:
                        damageCurve.curveMultiplier *= amount;
                        break;
                    case UnityEngine.ParticleSystemCurveMode.Curve:
                        damageCurve.curveMultiplier *= amount;
                        break;
                    case UnityEngine.ParticleSystemCurveMode.Constant:
                        damageCurve.constant *= amount;
                        break;
                }

                field.SetValue(targetObject, damageCurve);
            }
            catch (InvalidPathSpecifiedException) { } 
        }
    }
}


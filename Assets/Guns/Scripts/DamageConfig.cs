using UnityEngine;
using static UnityEngine.ParticleSystem;

[CreateAssetMenu(fileName = "Damage Config", menuName = "Guns/Damage Config", order = 1)]
public class DamageConfig : ScriptableObject, System.ICloneable
{
    public MinMaxCurve DamageCurve;

    private void Reset()
    {
        DamageCurve.mode = ParticleSystemCurveMode.Curve;
    }

    public int GetDamage(float distance = 0)
    {
        return Mathf.CeilToInt(DamageCurve.Evaluate(distance, Random.value));
    }

    public object Clone()
    {
        DamageConfig config = CreateInstance<DamageConfig>();

        config.DamageCurve = DamageCurve;
        return config;
    }
}

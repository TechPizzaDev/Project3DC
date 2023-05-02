using UnityEngine;

[CreateAssetMenu(fileName = "Trail Config", menuName = "Guns/Trail Configuration", order = 4)]
public class TrailConfig : ScriptableObject, System.ICloneable
{
    public Material material;
    public AnimationCurve widthCurve;
    public float duration = 0.5f;
    public float minVertexDistance = 0.1f;
    public Gradient color;

    public float missDistance = 100f;
    public float simulationSpeed = 100f;

    public object Clone()
    {
        TrailConfig config = CreateInstance<TrailConfig>();
        Utilities.CopyValues(this, config);
        return config;
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.VFX;

[CreateAssetMenu(fileName = "Gun", menuName = "Guns/Gun", order = 0)]
public class Gun : ScriptableObject
{
    public GunType type;
    public string gunName;
    public GameObject modelPrefab;
    public Vector3 spawnPoint;
    public Vector3 spawnRotation;

    public ShootConfig shootConfig;
    public TrailConfig trailConfig;

    private MonoBehaviour activeMonoBehaviour;
    private GameObject model;
    private float lastShootTime;
    private ParticleSystem shootSystem;
    private VisualEffect muzzleFlash;
    private ObjectPool<TrailRenderer> trailPool;

    public void Spawn(Transform parent, MonoBehaviour activeMonoBehaviour)
    {
        this.activeMonoBehaviour = activeMonoBehaviour;
        lastShootTime = 0;
        trailPool = new ObjectPool<TrailRenderer>(CreateTrail);
        model = Instantiate(modelPrefab);
        model.transform.SetParent(parent, false);
        model.transform.localPosition = spawnPoint;
        model.transform.localRotation = Quaternion.Euler(spawnRotation);

        shootSystem = model.GetComponentInChildren<ParticleSystem>();
        muzzleFlash = model.GetComponentInChildren<VisualEffect>();
    }

    public void Shoot()
    {
        if (Time.time > shootConfig.fireRate + lastShootTime)
        {
            lastShootTime = Time.time;
            shootSystem.Play();
            muzzleFlash.Play();
            Vector3 shootDirection = shootSystem.transform.forward
                + new Vector3(
                    Random.Range(
                        -shootConfig.spread.x,
                        shootConfig.spread.x
                    ),
                    Random.Range(
                        -shootConfig.spread.y,
                        shootConfig.spread.y
                    ),
                    Random.Range(
                        -shootConfig.spread.z,
                        shootConfig.spread.z
                    )
                );
            shootDirection.Normalize();

            if (Physics.Raycast(
                shootSystem.transform.position,
                shootDirection,
                out RaycastHit hit,
                float.MaxValue,
                shootConfig.hitMask
                ))
            {
                activeMonoBehaviour.StartCoroutine(
                    PlayTrail(
                        shootSystem.transform.position,
                        hit.point,
                        hit
                    )
                );
            }
            else
            {
                activeMonoBehaviour.StartCoroutine(
                    PlayTrail(
                        shootSystem.transform.position,
                        shootSystem.transform.position + (shootDirection * trailConfig.missDistance),
                        new RaycastHit()
                    )
                );
            }
        }
    }

    private IEnumerator PlayTrail(Vector3 startPoint, Vector3 endPoint, RaycastHit hit)
    {
        TrailRenderer instance = trailPool.Get();
        instance.gameObject.SetActive(true);
        instance.transform.position = startPoint;
        yield return null; //avoid position carry-over from last frame

        instance.emitting = true;

        float distance = Vector3.Distance(startPoint, endPoint);
        float remainingDistance = distance;
        while (remainingDistance > 0)
        {
            instance.transform.position = Vector3.Lerp(
                    startPoint,
                    endPoint,
                    Mathf.Clamp01(1 - (remainingDistance / distance))
            );
            remainingDistance -= trailConfig.simulationSpeed * Time.deltaTime;

            yield return null;
        }

        instance.transform.position = endPoint;

        yield return new WaitForSeconds(trailConfig.duration);
        yield return null;
        instance.emitting = false;
        instance.gameObject.SetActive(false);
        trailPool.Release(instance);
    }

    private TrailRenderer CreateTrail()
    {
        GameObject instance = new GameObject("Bullet Trail");
        TrailRenderer trail = instance.AddComponent<TrailRenderer>();
        trail.colorGradient = trailConfig.color;
        trail.material = trailConfig.material;
        trail.widthCurve = trailConfig.widthCurve;
        trail.time = trailConfig.duration;
        trail.minVertexDistance = trailConfig.minVertexDistance;

        trail.emitting = false;
        trail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        return trail;
    }
}

using System.Collections;
using UnityEngine;
using Rng = System.Random;

public class MobSpawner : MonoBehaviour
{
    private Rng _rng;

    public int RngSeed = 1234;
    public int SpawnCount = 1;
    public float SpawnDelay = 0.5f;
    public bool SpawnOnStart = false;

    public GameObject[] Prefabs;

    public int CurrentMobCount { get; private set; }

    private void Awake()
    {
        _rng = new Rng(1234);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (SpawnOnStart)
        {
            StartSpawning();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemies());
    }

    IEnumerator SpawnEnemies()
    {
        CurrentMobCount = SpawnCount;

        for (int i = 0; i < SpawnCount; i++)
        {
            yield return new WaitForSeconds(SpawnDelay);

            GameObject obj = GetRandomPrefab();
            GameObject instance = Instantiate(obj, transform.position, transform.rotation, transform);

            instance.GetComponent<UnitHealth>().OnDeath += MobSpawner_OnDeath;
        }
    }

    private void MobSpawner_OnDeath(GameObject sender, Vector3 position)
    {
        CurrentMobCount--;
    }

    GameObject GetRandomPrefab()
    {
        return Prefabs[_rng.Next(Prefabs.Length)];
    }
}

using System.Collections;
using UnityEngine;
using Rng = System.Random;

public class MobSpawner : MonoBehaviour
{
    private Rng _rng;

    public int RngSeed = 1234;
    public int SpawnCount = 1;
    public float SpawnDelay = 0.5f;

    public GameObject[] Prefabs;

    private void Awake()
    {
        _rng = new Rng(1234);
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnEnemies());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator SpawnEnemies()
    {
        for (int i = 0; i < SpawnCount; i++)
        {
            yield return new WaitForSeconds(SpawnDelay);

            GameObject obj = GetRandomPrefab();
            GameObject instance = Instantiate(obj, transform.position, transform.rotation, transform);
        }
    }

    GameObject GetRandomPrefab()
    {
        return Prefabs[_rng.Next(Prefabs.Length)];
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ObjectPooling;

public class ZoombieManager : MonoBehaviour
{
    public PlayerController playerController;
    
    public bool canSpawn;
    public bool isOnNavMesh;
    public Transform playerPosition;

    [SerializeField] private List<ZoombieController> zoombieControllers = new List<ZoombieController>();
    [Range(1, 30)]
    [SerializeField] private int maxAgentsCount;
    [SerializeField] private GameObject agentPrefab;
    [SerializeField] private Vector2 spawningRate;
    [SerializeField] private Vector2 xSpawnPosition;
    [SerializeField] private Vector2 zSpawnPosition;
    [SerializeField] private float ySpawnPosition;

    private float spawnTimer;
    private PrefabPool _pool;

    private void Awake()
    {
        _pool = new PrefabPool(agentPrefab, maxAgentsCount);
    }

    private void Start()
    {
        StartCoroutine(SpawningCoroutine());
    }

    private void Update()
    {
        SpawnTimerUpdate();
    }

    private void SpawnTimerUpdate()
    {
        spawnTimer += Time.deltaTime;
    }

    IEnumerator SpawningCoroutine()
    {
        yield return new WaitUntil(() => CheckCanSpawnAgent());
        spawnTimer = 0;
        yield return new WaitForSeconds(RandomSpawnTime());
        var agent = _pool.GetInstance() as ZoombieController;
        agent.name += zoombieControllers.Count;
        zoombieControllers.Add(agent);
        agent.zoombieManager = this;
        agent.transform.position = RandomPosition();
        agent.enemyScript.player = playerPosition;
        StartCoroutine(SpawningCoroutine());
    }

    private bool CheckCanSpawnAgent()
    {
        if (!canSpawn)
        {
            return false;
        }
        if (spawnTimer <= RandomSpawnTime())
        {
            return false;
        }
        if (zoombieControllers.Count >= maxAgentsCount)
        {
            return false;
        }
        return true;
    }

    private Vector3 RandomPosition()
    {
        float xRandomSpawnPositon = UnityEngine.Random.Range(xSpawnPosition.x, xSpawnPosition.y);
        float zRandomSpawnPositon = UnityEngine.Random.Range(zSpawnPosition.x, zSpawnPosition.y);
        return new Vector3(xRandomSpawnPositon, ySpawnPosition, zRandomSpawnPositon);
    }

    private float RandomSpawnTime()
    {
        return UnityEngine.Random.Range(spawningRate.x, spawningRate.y);
    }
}

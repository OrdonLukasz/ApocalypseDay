using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ObjectPooling;

public class ZoombieManager : MonoBehaviour
{
    public PlayerController playerController;

    public bool canSpawn;
    public bool isOnNavMesh;

    [SerializeField] private List<ZoombieController> zoombieControllers = new List<ZoombieController>();
    [Range(1, 30)]
    [SerializeField] private int maxAgentsCount;
    [SerializeField] private GameObject agentPrefab;
    [SerializeField] private Vector2 spawningRate;
    [SerializeField] private Vector2 xSpawnPosition;
    [SerializeField] private Vector2 zSpawnPosition;
    [SerializeField] private float ySpawnPosition;
    private Vector3 currentRandomPosition;
    public Transform playerPosition;

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
        //CheckSpawnPointOnNavMesh();
        //agent.transform.position = ReturnSpawnPosition();//
        agent.enemyScript.player = playerPosition;
        StartCoroutine(SpawningCoroutine());
    }

    private void CheckSpawnPointOnNavMesh()
    {
        isOnNavMesh = false;
        currentRandomPosition = RandomPosition();
        Debug.DrawRay(currentRandomPosition, Vector3.down * 10, Color.blue);
        RaycastHit hit;
        if (Physics.Raycast(currentRandomPosition, Vector3.down, out hit))
        {
            // the raycast hit
            var NavMesh = LayerMask.NameToLayer("NavMesh");
            if (hit.transform.gameObject.layer == NavMesh)
            {
                Debug.Log("trafiony!!!!");
                isOnNavMesh = true;
            }
        }
        else
        {
            isOnNavMesh = false;
            CheckSpawnPointOnNavMesh();
            //RandomPosition();
            Debug.Log("pufło!!!!");

            // the raycast didn't hit, maybe there's a hole in the terrain?
        }
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

    public Vector3 ReturnSpawnPosition()
    {
        
        do
        {
            Debug.Log("returnspawnpos");
            Vector3 spwnPos = currentRandomPosition;

            return spwnPos;

        } while (isOnNavMesh);
    }

    private Vector3 RandomPosition()
    {
        Debug.Log("dsadsa");
        float xRandomSpawnPositon = UnityEngine.Random.Range(xSpawnPosition.x, xSpawnPosition.y);
        float zRandomSpawnPositon = UnityEngine.Random.Range(zSpawnPosition.x, zSpawnPosition.y);
        //Debug.Log(xRandomSpawnPositon +  ySpawnPosition + zRandomSpawnPositon);
        return new Vector3(xRandomSpawnPositon, ySpawnPosition, zRandomSpawnPositon);
    }



    //private Vector3 RandomPosition()
    //{
    //    Debug.Log("dsadsa");
    //    float xRandomSpawnPositon = UnityEngine.Random.Range(xSpawnPosition.x, xSpawnPosition.y);
    //    float zRandomSpawnPositon = UnityEngine.Random.Range(zSpawnPosition.x, zSpawnPosition.y);

    //    return new Vector3(xRandomSpawnPositon, ySpawnPosition, zRandomSpawnPositon);
    //}

    private float RandomSpawnTime()
    {
        return UnityEngine.Random.Range(spawningRate.x, spawningRate.y);
    }
}

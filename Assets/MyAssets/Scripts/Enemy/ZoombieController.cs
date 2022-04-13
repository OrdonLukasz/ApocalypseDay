using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ObjectPooling;
using UnityEngine.AI;

public class ZoombieController : MonoBehaviour, IPoolable
{

    public ZoombieManager zoombieManager;
    public EnemyScript enemyScript;
    public NavMeshAgent navMeshAgent;

    public bool active;

    private PrefabPool _pool;

    public void SetParentPool(PrefabPool pool)
    {
        _pool = pool;
    }

    public void PrepareForPooling()
    {
        active = false;
        gameObject.SetActive(false);
        return;
    }

    public void HandleSpawn()
    {
        active = true;
        gameObject.SetActive(true);
    }

  
}

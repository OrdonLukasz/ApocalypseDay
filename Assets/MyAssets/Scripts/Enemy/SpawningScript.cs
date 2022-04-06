using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpawningScript : MonoBehaviour
{
    [SerializeField]
    private int enemyAmount;

    public GameObject player;
    public GameObject enemy;

    void Start()
    {
        for (int enemyAmountStart = 0; enemyAmountStart <= enemyAmount - 1; enemyAmountStart++)
        {
            Vector3 pos = new Vector3(Random.Range(-21f, 18f), 1f, Random.Range(-2.5f, 40f));
            GameObject Enemy_ = Instantiate(enemy, pos, Quaternion.identity);
            Enemy_.GetComponent<EnemyScript>().player = player.transform;
        }
    }
}

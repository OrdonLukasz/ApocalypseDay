using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationController : MonoBehaviour
{
    public ZoombieController zoombieController;
    public void OnHitPlayer()
    {
        zoombieController.zoombieManager.playerController.OnGetDamage(30);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.Animations.Rigging;

public class ShootingController : MonoBehaviour
{

    [SerializeField] Camera aimingTarget;
    [SerializeField] private Image viewfinder;
    [SerializeField] private float Range = 1000;
    [SerializeField] private float Force = 1000;
    [SerializeField] private Rig aimingWeaponRig;
    [SerializeField] private float timeToEnd;
    [SerializeField] private AnimationCurve curve;

    private float weaponAimedForce = 1;
    private ParticleSystem shootParticle;

    public void Update()
    {
        RayShoot();
    }

    
    public void RayShoot()
    {

        Vector3 DirectionRay = aimingTarget.transform.TransformDirection(Vector3.forward);
        Debug.DrawRay(aimingTarget.transform.position, DirectionRay * Range, Color.blue);
        RaycastHit Hit;
        if (Physics.Raycast(aimingTarget.transform.position, DirectionRay, out Hit, Range))
        {
            if (Hit.collider.CompareTag("Enemy"))
            {
                viewfinder.DOColor(Color.red, 0.2f);
                //viewfinder.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 2f);
                
            }
            else
            {
                viewfinder.DOColor(Color.white, 0.2f);
               // viewfinder.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0.5f);
            }
            if (Input.GetMouseButton(0))
            {

                if (Hit.transform.gameObject.GetComponent<EnemyScript>())
                {
                    EnemyScript enemyScript = Hit.transform.gameObject.GetComponent<EnemyScript>();
                    enemyScript.OnHit(10);
                }
                //AddDamage
            }
            if (Input.GetMouseButton(1))
            {
                DOTween.To(() => aimingWeaponRig.weight, x => aimingWeaponRig.weight = x, weaponAimedForce, timeToEnd).SetEase(curve);
            }
            else
            DOTween.To(() => aimingWeaponRig.weight, x => aimingWeaponRig.weight = x, 0f, timeToEnd).SetEase(curve);
        }


    }
    private void Fire()
    {
        shootParticle.Emit(1);
    }
}

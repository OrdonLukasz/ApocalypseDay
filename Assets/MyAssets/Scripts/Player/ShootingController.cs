using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.Animations.Rigging;
using DG.Tweening;


public class ShootingController : MonoBehaviour
{

    [SerializeField] Camera aimingTarget;
    [SerializeField] private Image viewfinder;
    [SerializeField] private float Range = 1000;
    [SerializeField] private float Force = 1000;
    [SerializeField] private Rig aimingWeaponRig;
    [SerializeField] private float timeToEnd;
    [SerializeField] private AnimationCurve curve;
    [SerializeField] private int ammo = 20;

    [SerializeField] private float weaponAimedForce = 1;
    [SerializeField] private ParticleSystem shootParticle;
    [SerializeField] public float sightRotationSpeed;

    [SerializeField] public float fireRate = 0.1f;
    [SerializeField] public float nextFire = 0.0f;
    [SerializeField] public float footStepsRate = 0.1f;
    [SerializeField] public float nextFootStep = 0.7f;
    [SerializeField] public Light fireLight;
    [SerializeField] public Transform lookFor;
    //public AudioClip machineGunShot;
    //public AudioClip walkfootStep;
    //public AudioClip runfootStep;
    //public AudioClip noAmmo;
    //public AudioClip reload;

    private Vector3 lookPoint;
    private Vector3 lookDirection;
    private Quaternion lookRotation;

    public void Update()
    {
        RayShoot();
        Reload();
    }


    public void RayShoot()
    {

        Vector3 DirectionRay = aimingTarget.transform.TransformDirection(Vector3.forward);
        Debug.DrawRay(aimingTarget.transform.position, DirectionRay * Range, Color.blue);
        RaycastHit Hit;
        if (Physics.Raycast(aimingTarget.transform.position, DirectionRay, out Hit, Range))
        {
            lookPoint = Hit.point;
            lookDirection = (Hit.point - shootParticle.transform.position);
            lookRotation = Quaternion.LookRotation(lookDirection);

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
                Fire();
                
            }
            lookFor.position = lookPoint;
            if (Input.GetMouseButton(1))
            {
                DOTween.To(() => aimingWeaponRig.weight, x => aimingWeaponRig.weight = x, weaponAimedForce, timeToEnd).SetEase(curve);
            }
            else
                DOTween.To(() => aimingWeaponRig.weight, x => aimingWeaponRig.weight = x, 0f, timeToEnd).SetEase(curve);
        }

    }

    void Fire()
    {
        if (Time.time > nextFire)
        {
            if (ammo > 0)
            {
                nextFire = Time.time + fireRate;
                shootParticle.Emit(1);
                //AudioSource.PlayClipAtPoint(machineGunShot, transform.position, 1);
                shootParticle.transform.rotation = Quaternion.Slerp(shootParticle.transform.rotation, lookRotation, Time.deltaTime * sightRotationSpeed);

                Sequence doorSequence = DOTween.Sequence();
                doorSequence.Append(fireLight.DOIntensity(8, 0.05f));
                doorSequence.Append(fireLight.DOIntensity(0, 0.05f));
                ammo--;
                //playerMovement.ammoText.text = $"{playerMovement.ammo}";
            }
            else
            {
                nextFire = Time.time + fireRate;
                //AudioSource.PlayClipAtPoint(noAmmo, transform.position, 1);
            }
        }
    }

    void Reload()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            //AudioSource.PlayClipAtPoint(reload, transform.position, 1);
            ammo = 30;
            //playerMovement.ammoText.text = $"{playerMovement.ammo}";
        }
    }
}

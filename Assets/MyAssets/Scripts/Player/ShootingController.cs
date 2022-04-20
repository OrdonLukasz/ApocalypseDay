using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.Animations.Rigging;
using TMPro;


public class ShootingController : MonoBehaviour
{
    public AudioClip noAmmo;
    public AudioClip reload;
    public AudioClip machineGunShot;
    public Transform empty;
    public LayerMask hitableLayerMasks;

    [SerializeField] private Camera aimingTarget;
    [SerializeField] private Image viewfinder;
    [SerializeField] private float Range = 1000;
    [SerializeField] private float Force = 1000;
    [SerializeField] private Rig aimingWeaponRig;
    [SerializeField] private float timeToEnd;
    [SerializeField] private AnimationCurve curve;
    [SerializeField] private int ammo;

    [SerializeField] private float weaponAimedForce = 1;
    [SerializeField] private ParticleSystem shootParticle;
    [SerializeField] private float sightRotationSpeed;

    [SerializeField] private float fireRate = 0.1f;
    [SerializeField] private float nextFire = 0.0f;
    [SerializeField] private float footStepsRate = 0.1f;
    [SerializeField] private float nextFootStep = 0.7f;
    [SerializeField] private Light fireLight;
    [SerializeField] private Transform lookFor;
    [SerializeField] private TMP_Text ammoText;

    private Vector3 lookPoint;
    private Vector3 lookDirection;
    private Quaternion lookRotation;


    public void Start()
    {
        ammoText.text = ammo.ToString();
    }

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
        if (Physics.Raycast(aimingTarget.transform.position, DirectionRay, out Hit, Range, hitableLayerMasks))
        {
            lookPoint = Hit.point;
            lookDirection = (Hit.point - shootParticle.transform.position);
            lookRotation = Quaternion.LookRotation(lookDirection);

            if (Hit.collider.CompareTag("Enemy"))
            {
                viewfinder.DOColor(Color.red, 0.2f);
            }
            else
            {
                viewfinder.DOColor(Color.white, 0.2f);
            }
            if (Input.GetMouseButton(0) && Input.GetMouseButton(1))
            {
                Debug.Log(Hit.transform.gameObject.name);
                if (Hit.transform.gameObject.GetComponent<EnemyScript>() && ammo > 0)
                {
                    EnemyScript enemyScript = Hit.transform.gameObject.GetComponent<EnemyScript>();
                    enemyScript.OnHit(10);
                }
                Fire();
            }
            lookFor.position = lookPoint;
            if (Input.GetMouseButtonDown(1))
            {
                DOTween.To(() => aimingWeaponRig.weight, x => aimingWeaponRig.weight = x, weaponAimedForce, timeToEnd).SetEase(curve);
            }
            if (Input.GetMouseButtonUp(1))
            {
                DOTween.To(() => aimingWeaponRig.weight, x => aimingWeaponRig.weight = x, 0f, timeToEnd).SetEase(curve);
            }
        }
        else
        {
            viewfinder.DOColor(Color.white, 0.2f);
            lookFor.position = empty.position;
            if (Input.GetMouseButton(0))
            {
                Fire();
            }
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
                shootParticle.transform.rotation = Quaternion.Slerp(shootParticle.transform.rotation, lookRotation, Time.deltaTime * sightRotationSpeed);
                AudioSource.PlayClipAtPoint(machineGunShot, transform.position, 1);
                Sequence doorSequence = DOTween.Sequence();
                doorSequence.Append(fireLight.DOIntensity(8, 0.05f));
                doorSequence.Append(fireLight.DOIntensity(0, 0.05f));
                ammo--;
                ammoText.text = ammo.ToString();

            }
            if (ammo <= 3 && ammo >= 0)
            {
                ammoText.DOColor(Color.red, 0.1f);
            }
            else
            {
                ammoText.DOColor(Color.white, 0.1f);
                nextFire = Time.time + fireRate;
                AudioSource.PlayClipAtPoint(noAmmo, transform.position, 1);
            }
        }
    }

    void Reload()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            AudioSource.PlayClipAtPoint(reload, transform.position, 1);
            ammo = 30;
            ammoText.text = ammo.ToString();
            ammoText.DOColor(Color.white, 0.1f);
        }
    }
}

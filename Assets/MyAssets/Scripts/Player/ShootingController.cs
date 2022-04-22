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
    [SerializeField] private Rig aimingWeaponRig;
    [SerializeField] private ParticleSystem shootParticle;
    [SerializeField] private AnimationCurve curve;
    [SerializeField] private Light fireLight;
    [SerializeField] private Transform lookFor;
    [SerializeField] private TMP_Text ammoText;
    [SerializeField] private int ammo;
    [SerializeField] private bool isMaxAmmo = true;

    [SerializeField] private float Range = 1000f;
    [SerializeField] private float Force = 1000f;
    [SerializeField] private float weaponAimedForce = 1f;
    [SerializeField] private float timeToEnd;
    [SerializeField] private float sightRotationSpeed;
    [SerializeField] private float fireRate = 0.1f;
    [SerializeField] private float nextFire = 0.0f;
    [SerializeField] private float footStepsRate = 0.1f;
    [SerializeField] private float nextFootStep = 0.7f;
    [SerializeField] private float minTimeToStartShoot = 0.3f;
    [SerializeField] private float currentTimeToStartShoot = 0f;

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
        Vector3 directionRay = aimingTarget.transform.TransformDirection(Vector3.forward);
        Debug.DrawRay(aimingTarget.transform.position, directionRay * Range, Color.blue);
        RaycastHit hit;
        if (Physics.Raycast(aimingTarget.transform.position, directionRay, out hit, Range, hitableLayerMasks))
        {
            lookPoint = hit.point;
            lookDirection = (hit.point - shootParticle.transform.position);
            lookRotation = Quaternion.LookRotation(lookDirection);
            lookFor.position = lookPoint;
            if (hit.collider.CompareTag("Enemy"))
            {
                viewfinder.DOColor(Color.red, 0.2f);
            }
            else
            {
                viewfinder.DOColor(Color.white, 0.2f);
            }
            if (Input.GetMouseButtonUp(1) || Input.GetMouseButtonUp(0))
            {
                OnTargetDeFocus();
                currentTimeToStartShoot = 0f;
            }
            if (Input.GetMouseButton(0))
            {
                OnTargetFocus();
                Fire(hit.transform);
                currentTimeToStartShoot += Time.deltaTime;
                return;
            }
            if (Input.GetMouseButtonDown(1))
            {
                OnTargetFocus();
            }
        }
        else
        {
            viewfinder.DOColor(Color.white, 0.2f);
            lookFor.position = empty.position;
            if (Input.GetMouseButton(0))
            {
                Fire(null);
            }
        }
    }

    private void OnTargetFocus()
    {
        DOTween.To(() => aimingWeaponRig.weight, x => aimingWeaponRig.weight = x, weaponAimedForce, timeToEnd).SetEase(curve);
    }

    private void OnTargetDeFocus()
    {
        DOTween.To(() => aimingWeaponRig.weight, x => aimingWeaponRig.weight = x, 0f, timeToEnd).SetEase(curve);
    }

    void Fire(Transform hit)
    {
        if (currentTimeToStartShoot >= minTimeToStartShoot)
        {
            if (Time.time > nextFire)
            {
                if (ammo > 0)
                {

                    if (hit != null)
                    {
                        if (hit.transform.GetComponent<EnemyScript>())
                        {
                            EnemyScript enemyScript = hit.transform.gameObject.GetComponent<EnemyScript>();
                            enemyScript.OnHit(20);
                        }
                    }
                    nextFire = Time.time + fireRate;
                    shootParticle.Emit(1);
                    shootParticle.transform.rotation = Quaternion.Slerp(shootParticle.transform.rotation, lookRotation, Time.deltaTime * sightRotationSpeed);
                    AudioSource.PlayClipAtPoint(machineGunShot, transform.position, 1);
                    Sequence doorSequence = DOTween.Sequence();
                    doorSequence.Append(fireLight.DOIntensity(8, 0.05f));
                    doorSequence.Append(fireLight.DOIntensity(0, 0.05f));
                    ammo--;
                    ammoText.text = ammo.ToString();
                    isMaxAmmo = false;
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
    }

    void Reload()
    {
        if (Input.GetKeyDown(KeyCode.R) && !isMaxAmmo)
        {
            AudioSource.PlayClipAtPoint(reload, transform.position, 1);
            ammo = 30;
            ammoText.text = ammo.ToString();
            ammoText.DOColor(Color.white, 0.1f);
            isMaxAmmo = true;
        }
    }
}

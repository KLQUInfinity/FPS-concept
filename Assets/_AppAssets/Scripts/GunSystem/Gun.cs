using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Gun : MonoBehaviourPunCallbacks
{
    [SerializeField] protected Transform gunShootPoint;
    [SerializeField] protected Transform bulletShellEjectionPoint;
    public GunType gunType;
    [SerializeField] protected float rpm;
    [SerializeField] protected float aimSwitchSpeed;
    [SerializeField] private Transform gunAnchor;
    [SerializeField] private Transform hipPos, adsPos;
    [SerializeField] protected LayerMask canBeShot;
    [SerializeField] protected float bloom, recoil, kickBack;

    protected float secondsBetweenShot;
    protected float nextPossibleShootTime;
    protected LineRenderer tracer;


    private void Start()
    {
        secondsBetweenShot = 60 / rpm;
        if (GetComponent<LineRenderer>())
        {
            tracer = GetComponent<LineRenderer>();
        }
    }

    private void Update()
    {
        if (!photonView.IsMine) return;
        SwitchAim(Input.GetMouseButton(1));

        transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, Time.deltaTime * 4f);
    }

    public void SwitchAim(bool isAiming)
    {
        if (isAiming)
        {
            gunAnchor.position = Vector3.Lerp(gunAnchor.position, adsPos.position, Time.deltaTime * aimSwitchSpeed);
        }
        else
        {
            gunAnchor.position = Vector3.Lerp(gunAnchor.position, hipPos.position, Time.deltaTime * aimSwitchSpeed);
        }
    }

    public virtual void Shoot() { }

    [PunRPC]
    public void ShootContinuous()
    {
        if (gunType == GunType.Auto)
        {
            Shoot();
        }
    }

    protected bool CanShoot()
    {
        bool canShoot = true;

        if (Time.time < nextPossibleShootTime)
        {
            canShoot = false;
        }

        return canShoot;
    }

    protected IEnumerator RenderTracer(Vector3 hitPoint)
    {
        tracer.enabled = true;
        tracer.SetPosition(0, gunShootPoint.position);
        tracer.SetPosition(1, gunShootPoint.position + hitPoint);
        yield return null;
        tracer.enabled = false;
    }

    protected void ShowBulletHole()
    {
        Vector3 t_Bloom = gunShootPoint.position + gunShootPoint.forward * 1000f;
        t_Bloom += Random.Range(-bloom, bloom) * gunShootPoint.up;
        t_Bloom += Random.Range(-bloom, bloom) * gunShootPoint.right;
        t_Bloom -= gunShootPoint.position;
        t_Bloom.Normalize();

        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(gunShootPoint.position, t_Bloom, out hit, 1000f, canBeShot))
        {
            GameObject bulletHole = ObjectPooling.Instance.SpawnFromPool("BulletHole", hit.point + hit.normal * 0.001f, Quaternion.identity);
            bulletHole.transform.LookAt(hit.point + hit.normal);

            if (photonView.IsMine)
            {
                if (hit.collider.gameObject.layer == 12)
                {
                    // RPC call to damage Player 
                }
            }
        }
    }

    protected void RecoilEffect()
    {
        transform.Rotate(-recoil, 0, 0);
        transform.position -= transform.forward * kickBack;

    }
}

public enum GunType
{
    Semi,
    Burst,
    Auto
}
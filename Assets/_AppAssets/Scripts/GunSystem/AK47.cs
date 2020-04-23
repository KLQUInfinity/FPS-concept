using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AK47 : Gun
{
    public override void Shoot()
    {
        if (CanShoot())
        {
            Ray ray = new Ray(gunShootPoint.position, gunShootPoint.forward);
            RaycastHit hit;

            float shotDistance = 20f;

            if (Physics.Raycast(ray, out hit, shotDistance))
            {
                shotDistance = hit.distance;
            }
            nextPossibleShootTime = Time.time + secondsBetweenShot;

            GetComponent<AudioSource>().Play();

            Rigidbody newShell = ObjectPooling.Instance.SpawnFromPool("BulletShell", bulletShellEjectionPoint.position, Quaternion.Euler(90, 0, 0)).GetComponent<Rigidbody>();
            newShell.AddForce(bulletShellEjectionPoint.forward * Random.Range(150f, 200f) + bulletShellEjectionPoint.forward * Random.Range(-10f, 10f));

            ShowBulletHole();

            if (tracer)
            {
                StartCoroutine(RenderTracer(shotDistance * ray.direction));
            }

            RecoilEffect();
        }
    }
}

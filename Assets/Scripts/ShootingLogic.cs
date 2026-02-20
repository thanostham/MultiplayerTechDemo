//Stratos Salpas

using System;
using UnityEngine;
using PurrNet;


public class ShootingLogic : NetworkBehaviour
{
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject bulletTrail;
    [SerializeField] private float weaponRange = 10f;
    [SerializeField] private LayerMask hitLayer;
    [SerializeField] private int damage = 20;
    [SerializeField] private AudioPlayer soundPlayerPrefab;
    [SerializeField] private AudioClip fireSound;


    protected override void OnSpawned()
    {
        base.OnSpawned();

        enabled = isController;
    }

    private void Update()
    {
        if (!isController)
            return;


        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    [ObserversRpc(runLocally:true)]
    private void Shoot()
    {
        var hit = Physics2D.Raycast(firePoint.position, transform.up, weaponRange, hitLayer);
        var trail = Instantiate(bulletTrail, firePoint.position, transform.rotation);
        var trailScript = trail.GetComponent<BulletTrail>();

        Instantiate(soundPlayerPrefab, firePoint.position, Quaternion.identity).PlaySound(fireSound);

        if (hit.collider != null)
        {
            Debug.Log($"Hit: {hit.transform.name}");
            trailScript.SetTrargetPosition(hit.point);

            if (hit.transform.TryGetComponent(out PlayerHealth playerHealth))
            {
                playerHealth.ChangeHealth(-damage);
            }

        }
        else
        {
            Debug.Log("No hit");
            var targetPosition = firePoint.position + transform.up * weaponRange;
            trailScript.SetTrargetPosition(targetPosition);
        }


        NetworkIdentity netId = trail.GetComponent<NetworkIdentity>();
        netId.Spawn(null);
    }
}



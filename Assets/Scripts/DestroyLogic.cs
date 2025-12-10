using UnityEngine;
using PurrNet;
using System;

public class AutoDestroy : NetworkBehaviour
{
    [SerializeField] private float lifetime = 5f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (isHost)
    //    {
    //        DestroyBullet();
    //    }
    //}

    [ServerRpc]
    void DestroyBulletServerRpc()
    {
        DestroyBullet();
    } 

    private void DestroyBullet()
    {
        NetworkIdentity netId = GetComponent<NetworkIdentity>();
        if (netId != null)
        {
            netId.Despawn(); 
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
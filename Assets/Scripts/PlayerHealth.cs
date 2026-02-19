using UnityEngine;
using PurrNet;
using System;

public class PlayerHealth : NetworkBehaviour
{
    [SerializeField] private SyncVar<int> health = new(100);
    [SerializeField] private int selfLayer, otherLayer;
    [SerializeField] private AudioPlayer soundPlayerPrefab;
    [SerializeField] private AudioClip deathSound;

    public Action<PlayerHealth> OnDeath_Server;

    public int Health => health;

    protected override void OnSpawned()
    {
        base.OnSpawned();   

        var actualLayer = isController ? selfLayer : otherLayer;
        SetLayerRecursive(gameObject, actualLayer);

        if (isController)
            health.onChanged += OnHealthChanged;
    }

    private void OnDestroy()
    {
        base.OnDestroy();

        health.onChanged -= OnHealthChanged;
    }

    private void OnHealthChanged(int newHealth)
    {
        InstanceHandler.GetInstance<MainGameView>().UpdateHealth(newHealth);
    }

    private void SetLayerRecursive(GameObject obj, int layer)
    {
        obj.layer = layer;

        foreach (Transform child in obj.transform)
        {
            SetLayerRecursive(child.gameObject, layer);
        }
    }

    [ServerRpc(requireOwnership:false)]
    public void ChangeHealth(int amount, RPCInfo info = default)
    {
        health.value += amount;

        if (health.value <= 0)
        {
            if (InstanceHandler.TryGetInstance(out ScoreManager scoreManager))
            {
                scoreManager.AddKill(info.sender);

                if (owner.HasValue)
                    scoreManager.AddDeath(owner.Value);    
            }

            PlayDeathEffects();
            OnDeath_Server?.Invoke(this);
            //Destroy(gameObject);

            Respawn();
        }
    }

    //Try respawn when pl dies instead of destroy 
    private void Respawn()
    {
        health.value = 100;

        float spawnRange = 30f;
        Vector3 newPos = new Vector3(
            UnityEngine.Random.Range(-spawnRange / 2f, spawnRange / 2f),
            UnityEngine.Random.Range(-spawnRange / 2f, spawnRange / 2f),
            0f
        );

        RpcTeleport(newPos);
        //transform.position = newPos;
    }

    [ObserversRpc(runLocally: true)]
    private void RpcTeleport(Vector3 position)
    {
        transform.position = position;

        if (TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    [ObserversRpc(runLocally:true)]
    private void PlayDeathEffects()
    {
        var soundPlayer = Instantiate(soundPlayerPrefab, transform.position, Quaternion.identity);
        soundPlayer.PlaySound(deathSound);
    }
}

using UnityEngine;
using PurrNet;
using System;

public class PlayerHealth : NetworkBehaviour
{
    [SerializeField] private SyncVar<int> health = new(100);
    [SerializeField] private int selfLayer, otherLayer;

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
    public void ChangeHealth(int amount)
    {
        health.value += amount;

        if (health.value <= 0)
        {
            Destroy(gameObject);
        }
    }
}

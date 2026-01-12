using System;
using UnityEngine;
using System.Collections.Generic;
using PurrNet;
using Lean.Pool;
using Random = UnityEngine.Random;

public class FoodManager : MonoBehaviour
{
    public static FoodManager Instance { get; private set; }

    private void Awake()
    {
        //Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    public GameObject foodPrefab;
    public int poolSize = 500;
    public Vector2 mapSize = new Vector2(100, 100);

    private List<GameObject> activeFoodList = new List<GameObject>();

    public void StartFood()
    {
        if (Network.NetInstance)
        {
            PreloadPool();
            SpawnAllFood();
        }
    }
    
    private void PreloadPool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject food = LeanPool.Spawn(foodPrefab);
            LeanPool.Despawn(food);
        }
    }

    private void SpawnAllFood()
    {
        for (int i = 0; i < poolSize; i++)
        {
            SpawnFood();
        }
    }
    
    public GameObject SpawnFood()
    {
        Vector3 pos = new Vector3(
            Random.Range(-mapSize.x / 2, mapSize.x / 2), 
            Random.Range(-mapSize.y / 2, mapSize.y / 2), 
            1f
        );

        //Spawn from pool
        GameObject food = LeanPool.Spawn(foodPrefab, pos, Quaternion.identity);

        //Randomize size
        float size = Random.Range(0.3f, 1.2f);
        food.transform.localScale = Vector3.one * size;

        //Randomize color
        Color randomColor = Random.ColorHSV(0f, 1f, 1f, 1f, 1f, 1f);
        food.GetComponent<SpriteRenderer>().color = randomColor;

        activeFoodList.Add(food);
        return food;
    }
    
    public void DespawnFood(GameObject food)
    {
        if (activeFoodList.Contains(food))
        {
            activeFoodList.Remove(food);
        }
        
        //Return to pool
        LeanPool.Despawn(food);
        
        //Maintain pool size
        SpawnFood();
    }

    //Clean up all food
    public void DespawnAllFood()
    {
        foreach (GameObject food in activeFoodList.ToArray())
        {
            LeanPool.Despawn(food);
        }
        activeFoodList.Clear();
    }

    private void OnDestroy()
    {
        DespawnAllFood();
    }
}
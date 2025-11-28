using System;
using UnityEngine;
using System.Collections.Generic;
using PurrNet;
using UnityEditor;
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

    private List<GameObject> foodPool = new List<GameObject>();
    

    public void StartFood()
    {
        CreatePool();
        SpawnAllFood();
    }
    

    private void CreatePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject food = Instantiate(foodPrefab);
            food.SetActive(false);
            foodPool.Add(food);
        }
    }

    private void SpawnAllFood()
    {
        foreach (GameObject food in foodPool)
        {
            RespawnFood(food);
        }
    }

    public void RespawnFood(GameObject food)
    {
        Vector3 pos = new Vector3(Random.Range(-mapSize.x / 2, mapSize.x / 2), Random.Range(-mapSize.y / 2, mapSize.y / 2), 1f);

        food.transform.position = pos;

        float size = Random.Range(0.3f, 1.2f);
        food.transform.localScale = Vector3.one * size;

        Color randomColor = Random.ColorHSV(0f, 1f, 1f, 1f, 1f, 1f);
        food.GetComponent<SpriteRenderer>().color = randomColor;

        food.SetActive(true);
    }

}
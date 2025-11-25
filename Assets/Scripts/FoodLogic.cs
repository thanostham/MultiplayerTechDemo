//Thanos Thamnopoulos
using UnityEngine;

public class Food : MonoBehaviour
{
    [SerializeField]GameObject food;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player Collided with Food");
            Destroy(gameObject);
            FoodManager.Instance.RespawnFood(food);
        }
    }
}
//Thanos Thamnopoulos
using UnityEngine;

public class Food : MonoBehaviour
{
    GameObject food;
    private void OnTriggerEnter2D(Collider other)
    {
        if (other.CompareTag("Food"))
        {
            Destroy(other.gameObject);
            FoodManager.Instance.RespawnFood(food);
        }
    }
}
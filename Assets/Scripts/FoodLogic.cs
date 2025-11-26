//Thanos Thamnopoulos

using PurrNet;
using UnityEngine;

public class Food : NetworkBehaviour
{
    [SerializeField] GameObject food;
    [SerializeField] float lerpSpeed = 5f;
    [SerializeField] float attractionDistance = 1f;
    
    private Transform playerTransform;
    private bool isLerping = false;

    private void Update()
    {
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
            }
            return;
        }

        float distanceX = Mathf.Abs(playerTransform.position.x - transform.position.x);
        float distanceY = Mathf.Abs(playerTransform.position.y - transform.position.y);

        if (distanceX <= attractionDistance && distanceY <= attractionDistance)
        {
            isLerping = true;
        }

        if (isLerping)
        {
            Vector3 targetPosition = new Vector3(
                playerTransform.position.x,
                playerTransform.position.y,
                transform.position.z
            );

            transform.position = Vector3.Lerp(
                transform.position,
                targetPosition,
                lerpSpeed * Time.deltaTime
            );
        }
    }

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
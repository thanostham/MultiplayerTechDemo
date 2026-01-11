//Thanos Thamnopoulos

using PurrNet;
using UnityEngine;
using Lean.Pool;

public class Food : NetworkBehaviour
{
    [SerializeField] float lerpSpeed = 5f;
    [SerializeField] float attractionDistance = 1f;
    
    private Transform playerTransform;
    private bool isLerping = false;

    //Called when spawned from pool
    private void OnEnable()
    {
        // Reset state when food is spawned
        isLerping = false;
        playerTransform = null;
    }

    //Called when returned to pool
    private void OnDisable()
    {
        playerTransform = null;
        isLerping = false;
    }

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
            FoodManager.Instance.DespawnFood(gameObject);
        }
    }
}
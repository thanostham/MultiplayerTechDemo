//Contribution - Both
//Stratos Salpas
using UnityEngine;
using PurrNet;

public class MovementLogic : NetworkBehaviour
{
    [SerializeField] private float movementSpeed = 5f;
    public Rigidbody2D rb;
    public Camera cam;   
    
    Vector2 mousePos;

    private void Update()
    {
        Debug.Log($"isOwner: {isOwner}");
        
        if (!isOwner) return;

        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        cam.transform.position = rb.transform.position.WithNewZ(-10);

        SendMovementToServer(mousePos);
    }

    [ServerRpc]
    private void SendMovementToServer(Vector2 targetMousePos)
    {
        Vector2 target = Vector2.Lerp(rb.position, targetMousePos, 0.1f);
        Vector2 dir = (target - rb.position).normalized;

        rb.position += dir * movementSpeed * Time.deltaTime;

        Vector2 lookDir = targetMousePos - rb.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        rb.rotation = angle;
    }
}

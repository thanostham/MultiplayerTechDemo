//Stratos Salpas
using UnityEngine;

public class MovementLogic : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 5f;
    public Rigidbody2D rb;
    public Camera cam;
    
    Vector2 mousePos;

    // Update is called once per frame
    void Update()
    {
        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        
        Vector2 target = Vector2.Lerp(rb.position, mousePos, 0.1f); // smooth 10% follow
        Vector2 dir = (target - rb.position).normalized;

        rb.position += dir * movementSpeed * Time.deltaTime;
    }

    private void FixedUpdate()
    {
        Vector2 lookDir = mousePos - rb.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg -90f;
        rb.rotation = angle;
    }
}


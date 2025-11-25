//Stratos Salpas
using UnityEngine;

public class MovementLogic : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 5f;
    public Rigidbody2D rb;
    public Camera cam;   
    
    Vector2 mousePos;
    void Update()
    {
        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        cam.transform.position = rb.transform.position.WithNewZ(-10);

        Vector2 target = Vector2.Lerp(rb.position, mousePos, 0.1f);
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


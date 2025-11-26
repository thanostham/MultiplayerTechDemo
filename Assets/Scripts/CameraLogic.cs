//Thanos
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;
    
    [Header("Parallax Settings")]
    [SerializeField] private float parallaxStrength = 2f;
    [SerializeField] private float parallaxSmoothSpeed = 5f;
    [SerializeField] private float edgeThreshold = 0.15f; // 15% from screen edge
    
    [Header("Camera Settings")]
    [SerializeField] private float followSmoothSpeed = 10f;
    [SerializeField] private float zPosition = -10f;
    
    private Vector3 parallaxOffset;
    [SerializeField]private Camera cam;
    private Quaternion frozenRotation;

    private void Start()
    {
        frozenRotation = Quaternion.identity;
        transform.rotation = frozenRotation;
    }

    private void LateUpdate()
    {
        if (target == null) return;
        
        CalculateParallaxOffset();
        
        Vector3 desiredPosition = target.position + parallaxOffset;
        desiredPosition.z = zPosition;

        transform.position = Vector3.Lerp(
            transform.position, 
            desiredPosition, 
            followSmoothSpeed * Time.deltaTime
        );
        
        //Force rotation to stay frozen every frame
        transform.rotation = frozenRotation;
    }

    private void CalculateParallaxOffset()
    {
        Vector2 mouseViewportPos = cam.ScreenToViewportPoint(Input.mousePosition);
        
        //Calculate offset
        Vector2 offsetFromCenter = mouseViewportPos - new Vector2(0.5f, 0.5f);
        
        //Apply parallax when near edges
        Vector2 edgeInfluence = Vector2.zero;
        
        //Horizontal
        if (Mathf.Abs(offsetFromCenter.x) > (0.5f - edgeThreshold))
        {
            float edgeDistance = Mathf.Abs(offsetFromCenter.x) - (0.5f - edgeThreshold);
            edgeInfluence.x = Mathf.Sign(offsetFromCenter.x) * (edgeDistance / edgeThreshold);
        }
        
        //Vertical
        if (Mathf.Abs(offsetFromCenter.y) > (0.5f - edgeThreshold))
        {
            float edgeDistance = Mathf.Abs(offsetFromCenter.y) - (0.5f - edgeThreshold);
            edgeInfluence.y = Mathf.Sign(offsetFromCenter.y) * (edgeDistance / edgeThreshold);
        }
        
        //Calculate target offset
        Vector3 targetParallaxOffset = new Vector3(
            edgeInfluence.x * parallaxStrength,
            edgeInfluence.y * parallaxStrength,
            0f
        );
        
        //Smooth the offset
        parallaxOffset = Vector3.Lerp(
            parallaxOffset,
            targetParallaxOffset,
            parallaxSmoothSpeed * Time.deltaTime
        );
    }

    //===We may need this later to target the player that killed us===
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
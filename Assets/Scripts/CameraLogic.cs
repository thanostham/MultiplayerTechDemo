//Thanos
using PurrNet;
using Unity.VisualScripting;
using UnityEngine;

public class CameraController : NetworkBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;
    public NetworkIdentity netID;
    
    [Header("Camera Settings")]
    [SerializeField] private float followSmoothSpeed = 10f;
    [SerializeField] private float zPosition = -10f;
    
    [SerializeField]private Camera cam;
    private Quaternion frozenRotation;

    
    private void Start()
    {
        frozenRotation = Quaternion.identity;
        transform.rotation = frozenRotation;
        if (target == null)
        {
            //get parent player
            target = transform.parent;
        
            //find the rb
            if (target == null)
            {
                var rb = GetComponentInParent<Rigidbody2D>();
                if (rb != null) target = rb.transform;
            }
        }
    }
    
    protected override void OnSpawned()
    {
        base.OnSpawned();
    
        if (!isOwner)
        {
            cam.enabled = false;
            this.enabled = false;
            return;
        }
    
        cam.enabled = true;
    }

    private void LateUpdate()
    {
        if (!isOwner) return;
        if (target == null) return;
    
        Vector3 desiredPosition = target.position;
        desiredPosition.z = zPosition;

        transform.position = Vector3.Lerp(
            transform.position, 
            desiredPosition, 
            followSmoothSpeed * Time.deltaTime
        );
    
        transform.rotation = frozenRotation;
    }

    //just in case of killcam or sth
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
//Thanos
using PurrNet;
using Unity.VisualScripting;
using UnityEngine;

public class CameraLogic : NetworkBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;
    //public NetworkIdentity netID;
    
    [Header("Camera Settings")]
    [SerializeField] private float followSmoothSpeed = 10f;
    [SerializeField] private float zPosition = -10f;
    
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
        
        Vector3 desiredPosition = target.position;
        desiredPosition.z = zPosition;

        transform.position = Vector3.Lerp(
            transform.position, 
            desiredPosition, 
            followSmoothSpeed * Time.deltaTime
        );
        
        //Force rotation to stay frozen every frame
        transform.rotation = frozenRotation;
    }

    //===We may need this later to target the player that killed us===
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
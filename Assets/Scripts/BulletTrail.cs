using UnityEngine;
using PurrNet;

public class BulletTrail : NetworkBehaviour
{
    private Vector3 startPos;
    private Vector3 targerPos;
    private float progress;

    [SerializeField] private float speed = 40f;

    private void Start()
    {
        startPos = transform.position.WithAxis(Axis.Z, -1);
    }

    private void Update()
    {
        progress += Time.deltaTime * speed;
        transform.position = Vector3.Lerp(startPos, targerPos, progress);
    }

    public void SetTrargetPosition(Vector3 targetPosition)
    {
        targerPos = targetPosition.WithAxis(Axis.Z, -1);
    }
}

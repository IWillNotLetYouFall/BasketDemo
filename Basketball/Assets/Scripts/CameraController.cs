using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform camTarget;
    [SerializeField] float pLerp = 0.15f;
    [SerializeField] float rLerp = 0.2f;

    void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, camTarget.position, pLerp);
        transform.rotation = Quaternion.Lerp(transform.rotation, camTarget.rotation, rLerp);
    }
}

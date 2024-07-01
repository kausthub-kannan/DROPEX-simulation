using UnityEngine;
using UnityEngine.Serialization;

public class CameraManager : MonoBehaviour
{
    public Transform mainCamera;
    public Transform droneCamera;
    public Transform drone;

    private float cameraAngle = 41.5f;

    void LateUpdate()
    {
        mainCamera.position = drone.position + new Vector3(0f, 1f, -10f);
        droneCamera.position = drone.position + new Vector3(0f, -0.5f, 0f);


        droneCamera.transform.rotation = Quaternion.Euler(cameraAngle, 0f, 0f);
    }
}
using UnityEngine;
using Unity.Cinemachine;

public class CameraManager : MonoBehaviour
{
    [Header("Cameras")]
    public CinemachineCamera FPS;
    public CinemachineCamera TopDown;
    public int steeringModeLocal = 1;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            steeringModeLocal += 1;
            if (steeringModeLocal > 2) steeringModeLocal = 1;
            SwitchCamera();
        }
    }

    public void SetTarget(Transform target)
    {
        if (FPS)
        {
            FPS.Follow = target;
            FPS.LookAt = target;
        }

        if (TopDown)
        {
            TopDown.Follow = target;
            TopDown.LookAt = target;
        }

        SwitchCamera();
    }

    public void SwitchCamera()
    {
        if (steeringModeLocal == 1)
        {
            FPS.Priority = 20;
            TopDown.Priority = 10;
        }
        else if (steeringModeLocal == 2)
        {
            FPS.Priority = 10;
            TopDown.Priority = 20;
        }
    }
}

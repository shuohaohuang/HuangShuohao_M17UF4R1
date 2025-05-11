using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class PCCamara : MonoBehaviour
{
    public List<CinemachineCamera> cams = new();
    public int active,
        lastActive;

    public void SwitchCam()
    {
        lastActive = active;
        if (cams[0].enabled)
        {
            cams[0].enabled = false;
            cams[2].enabled = true;
            active = 2;
        }
        else
        {
            active = 0;
            cams[2].enabled = false;
            cams[0].enabled = true;
        }
    }

    public void Dance()
    {
        if (cams[1].enabled)
        {
            cams[1].enabled = false;
            cams[lastActive].enabled = true;
        }
        else
        {
            lastActive = active;
            cams[1].enabled = true;
        }
    }
}

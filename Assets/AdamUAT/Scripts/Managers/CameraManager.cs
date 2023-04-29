using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private Camera currentCamera;

    /// <summary>
    /// The camera currently being used by this manager as what's displaying to the screen.
    /// </summary>
    public Camera CurrentCamera
    {
        get
        {
            return currentCamera;
        }
        set
        {
            currentCamera = value;

            //Disables all cameras except the current camera.
            Camera[] allCameras = new Camera[Camera.allCamerasCount];
            Camera.GetAllCameras(allCameras);
            for(int i = 0; i < Camera.allCamerasCount; i++)
            {
                allCameras[i].enabled = false;
            }
            currentCamera.enabled = true;
        }
    }
}

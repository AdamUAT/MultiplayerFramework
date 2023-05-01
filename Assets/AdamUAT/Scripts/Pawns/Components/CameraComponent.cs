using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CameraComponent : PawnComponent
{
    [SerializeField]
    [Tooltip("The local offset from the parent.")]
    private Vector3 cameraTargetOffset;

    [SerializeField]
    [Tooltip("The local rotation the camera should be.")]
    private Quaternion cameraTargetRotation = new Quaternion();

    [SerializeField]
    private float lerpSpeed = 10.0f;

    private void Awake()
    {
        Parent = GetComponent<Pawn>();
    }

    private void Start()
    {
        //Checks if this component is on the pawn that this client uses.
        if (Parent != null)
        {
            if (Parent.Controller != null)
            {
                if (Parent.Controller.IsLocalPlayer)
                {
                    //If there is not a dedicated camera for the player, it will spawn a new one.
                    if (GameManager.instance.cameraManager.CurrentCamera == null || GameManager.instance.cameraManager.CurrentCamera.name != "PlayerCamera")
                    {
                        GameObject newCamera = new GameObject("PlayerCamera");
                        GameManager.instance.cameraManager.CurrentCamera = newCamera.AddComponent<Camera>();
                    }
                }
                else
                {
                    Destroy(this);
                }
            }
            else
            {
                Destroy(this);
                Debug.LogError("Parent's controller is null.");
            }
        }
        else
        {
            Debug.LogError("Parent is null.");
        }
    }

    private void LateUpdate()
    {
        //Smoothly moves the camera to where it should be.
        GameManager.instance.cameraManager.CurrentCamera.gameObject.transform.position = Vector3.Lerp(GameManager.instance.cameraManager.CurrentCamera.gameObject.transform.position, Parent.transform.TransformPoint(cameraTargetOffset), lerpSpeed * Time.deltaTime);

        GameManager.instance.cameraManager.CurrentCamera.gameObject.transform.rotation = Quaternion.Lerp(GameManager.instance.cameraManager.CurrentCamera.gameObject.transform.rotation, Parent.transform.rotation * cameraTargetRotation, lerpSpeed * Time.deltaTime);
    }
}

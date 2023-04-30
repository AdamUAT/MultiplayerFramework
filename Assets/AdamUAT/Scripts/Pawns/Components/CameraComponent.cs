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
        if(Parent.Controller.gameObject == NetworkManager.Singleton.LocalClient.PlayerObject.gameObject)
        {
            //Spawns a new camera that this pawn will use.
            GameObject newCamera = new GameObject("PlayerCamera");
            GameManager.instance.cameraManager.CurrentCamera = newCamera.AddComponent<Camera>();
        }
        else
        {
            Destroy(this);
        }
    }

    private void LateUpdate()
    {
        //Smoothly moves the camera to where it should be.
        GameManager.instance.cameraManager.CurrentCamera.gameObject.transform.position = Vector3.Lerp(GameManager.instance.cameraManager.CurrentCamera.gameObject.transform.position, Parent.transform.TransformPoint(cameraTargetOffset), lerpSpeed * Time.deltaTime);

        GameManager.instance.cameraManager.CurrentCamera.gameObject.transform.rotation = Quaternion.Lerp(GameManager.instance.cameraManager.CurrentCamera.gameObject.transform.rotation, Parent.transform.rotation * cameraTargetRotation, lerpSpeed * Time.deltaTime);
    }
}

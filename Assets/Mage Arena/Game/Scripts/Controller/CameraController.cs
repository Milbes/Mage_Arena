#pragma warning disable 649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Watermelon;
using EZCameraShake;

public class CameraController : MonoBehaviour
{
    private static CameraController instance;

    [SerializeField] Camera mainCamera;
    public static Camera MainCamera => instance.mainCamera;

    [SerializeField] CameraSettings cameraSettings;
    private static CameraSettings CameraSettings => instance.cameraSettings;


    public static PlayerController Target { get; set; }

    private void Awake()
    {
        instance = this;

        MainCamera.orthographicSize = cameraSettings.cameraWidth /  MainCamera.aspect * (9f / 16f);
    }

    public static void InitCamera()
    {
        instance.transform.position = CameraSettings.cameraIntitalPosition;
    }

    public void Update()
    {
        if(Target != null)
        {
            float targetPosZ = Target.transform.position.z;

            if (targetPosZ < cameraSettings.minPosZ) targetPosZ = cameraSettings.minPosZ;
            if (targetPosZ > GameController.CurrentRoom.Size.y + cameraSettings.maxPosRoomZOffset) targetPosZ = GameController.CurrentRoom.Size.y + cameraSettings.maxPosRoomZOffset;

            transform.position = transform.position.SetZ(targetPosZ + cameraSettings.posTargetZOffset);
        }
    }

    public static void ShakeCamera()
    {
        CameraShaker.Instance.ShakeOnce(CameraSettings.shakeMagnitude, CameraSettings.shakeRoughness, CameraSettings.shakeFadeInTime, CameraSettings.shakeFadeOutTime);
    }
}

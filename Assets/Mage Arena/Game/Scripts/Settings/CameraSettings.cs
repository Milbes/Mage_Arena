using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Camera Settings", menuName = "Settings/Camera Settings")]
public class CameraSettings : ScriptableObject
{
    [Header("Camera settings")]

    [Tooltip("Width of the camera in units. Standart room is 11 units in width.")]
    public float cameraWidth = 12f;
    [Tooltip("Minimal z position of the camera")]
    public float minPosZ = 10f;
    [Tooltip("Max z position of the camera depending on a room length")]
    public float maxPosRoomZOffset = -5;
    [Tooltip("Z offset between a player and the camera if other constrains don't apply")]
    public float posTargetZOffset = -15f;
    [Tooltip("Initial position of the camera. Use to set constant X and Y coordinates of the camera")]
    public Vector3 cameraIntitalPosition = new Vector3(5.5f, 25, -10);


    [Header("Shake Settings")]

    public float shakeMagnitude = 1f;
    public float shakeRoughness = 2f;
    public float shakeFadeInTime = 0.7f;
    public float shakeFadeOutTime = 0.15f;

}

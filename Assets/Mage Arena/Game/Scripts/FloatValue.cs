#pragma warning disable 649

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Watermelon;

[CreateAssetMenu(menuName = "Values/Float Value", fileName = "Float Value")]
public class FloatValue : ScriptableObject
{
    [SerializeField] float value;

    [SerializeField] [ReadOnly] public float runtimeValue;

    private void OnEnable()
    {
        runtimeValue = value;
    }

}

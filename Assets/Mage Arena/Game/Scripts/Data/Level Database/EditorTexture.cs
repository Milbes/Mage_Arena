#pragma warning disable 649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EditorTexture
{
    [SerializeField] private Texture2D texture;
    [SerializeField] private float horizontalOffset;
    [SerializeField] private float verticalOffset;
}

#pragma warning disable 649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Level Database/Obstacle", fileName = "Obstacle")]
public class Obstacle : ScriptableObject 
{
    [SerializeField] GameObject prefab;
    [SerializeField] Vector2Int size;
    [SerializeField] EditorTexture editorTexture;

    public GameObject Prefab => prefab;
    public Vector2Int Size => size;
}

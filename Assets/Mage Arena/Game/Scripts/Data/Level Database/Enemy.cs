#pragma warning disable 649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Level Database/Enemy", fileName = "Enemy")]
public class Enemy : ScriptableObject
{
    [SerializeField] bool isBoss;
    [SerializeField] GameObject prefab;
    [SerializeField] float maxHP;
    [SerializeField] EditorTexture editorTexture;
    [SerializeField] Vector2Int size;

    public bool IsBoss => isBoss;
    public float MaxHP => maxHP;
    public GameObject Prefab => prefab;
    public Vector2Int Size => size;
    
}

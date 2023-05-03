#pragma warning disable 649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleBehavior : MonoBehaviour
{

    [SerializeField] Type type;


    public void Init(Vector2Int position)
    {
        transform.position = new Vector3(position.x + 0.5f, 0, position.y + 0.5f);

        if(type == Type.Random)
        {
            ((RandomObstacleBehaviour)this).Init();
        } else if(type == Type.Whater)
        {
            ((WaterObstacleBehaviour)this).Init(position);
        }
    }


    public enum Type
    {
        Random, Spike, Whater
    }
}

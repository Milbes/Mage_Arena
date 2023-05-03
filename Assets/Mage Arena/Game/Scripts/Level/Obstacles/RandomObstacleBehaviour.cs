#pragma warning disable 649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomObstacleBehaviour : ObstacleBehavior
{
    [SerializeField] MeshFilter meshFilter;

    [Space]
    [SerializeField] List<RandomMeshData> meshes;


    public void Init()
    {

        float chanceSum = 0;

        for(int i = 0; i < meshes.Count; i++)
        {
            float chance = meshes[i].Chance;
            Mesh mesh = meshes[i].Mesh;

            chanceSum += chance;
            if (Random.value <= chanceSum)
            {
                meshFilter.mesh = mesh;
                break;
            }
        }

        transform.eulerAngles = Vector3.up * 90 * Random.Range(0, 4);
    }


    [System.Serializable]
    public class RandomMeshData
    {
        [SerializeField] Mesh mesh;
        [SerializeField] float chance;

        public Mesh Mesh => mesh;
        public float Chance => chance;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    private float startPos;

    public float length;

    public Transform target;
    public float parallaxEffect;

    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = (RectTransform)transform;

        startPos = transform.position.x;
    }

    void Update()
    {
        float temp = target.position.x * (1 - parallaxEffect);
        float dist = target.position.x * parallaxEffect;

        rectTransform.position = new Vector3(startPos + dist, transform.position.y, transform.position.z);

        if (temp > startPos + length)
            startPos += length;
        else if (temp < startPos - length)
            startPos -= length;
    }
}

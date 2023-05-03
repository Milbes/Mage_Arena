using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimCut : MonoBehaviour
{
    public bool updateGI;


    Renderer mRenderer;
    Material material;

    float speed;


    private void Start()
    {
        mRenderer = GetComponent<Renderer>();

        material = mRenderer.material;

        speed = 3f;
    }

    // Update is called once per frame
    void Update()
    {
        bool down = Input.GetKeyDown(KeyCode.Space);
        bool held = Input.GetKey(KeyCode.Space);

            material.SetFloat("_DissolveCutoff", Time.time * speed);


        if (updateGI)
        {
            //Dynamic GI uses META pass, which has no info about mesh position in the world.
            //We have to provive mesh world position data manually 
            material.SetVector("_Dissolve_ObjectWorldPos", transform.position);


            //We need to update Unity GI every time we change material properties effecting GI
            RendererExtensions.UpdateGIMaterials(mRenderer);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;

public class Objective : MonoBehaviour
{
    public Sprite objectiveIcon;

    private ParticleSystem emitter = null;

    // Start is called before the first frame update
    void Start()
    {
        foreach(Transform child in transform){
            ParticleSystem childEmitter = child.GetComponent<ParticleSystem>();
            if(childEmitter != null)
            {
                emitter = childEmitter;
                break;
            }
        }

        if(emitter == null)
        {
            Debug.Log("Objective child emitter not found!");
        }
        else
        {
            emitter.Stop();
        }
    }

    public void EnableEmmiter()
    {
        emitter.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

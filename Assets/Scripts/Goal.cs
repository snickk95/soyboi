using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    public AudioClip goalClip;

     void OnCollisionEnter2D(Collision2D coll)
    {
        if(coll.gameObject.tag=="Player")
        {
            var audioScource = GetComponent<AudioSource>();
            if(audioScource != null && goalClip!= null)
            {
                audioScource.PlayOneShot(goalClip);
            }
            GameManager.instance.RestartLevel(0.5f);
        }
    }

    void Start()
    {
        
    }

    
    void Update()
    {
        
    }
}

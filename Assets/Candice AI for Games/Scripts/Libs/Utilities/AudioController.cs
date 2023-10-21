using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{

    private AudioSource audio;

    // Start is called before the first frame update
    void Start()
    {
        audio = GetComponent<AudioSource>();
        if (audio != null) {
            audio.Play();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

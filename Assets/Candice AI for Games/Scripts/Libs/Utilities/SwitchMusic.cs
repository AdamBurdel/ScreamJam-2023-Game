using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchMusic : MonoBehaviour
{

    private TMPro.TMP_Dropdown musicDrop;
    public AudioClip[] Tracks;
    public AudioSource MainAudio;

    // Start is called before the first frame update
    void Start()
    {
        musicDrop = GetComponent<TMPro.TMP_Dropdown>();
        musicDrop.onValueChanged.AddListener(delegate { ChangeTrack(); });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeTrack()
    {
        MainAudio.clip = Tracks[musicDrop.value];
        MainAudio.Play();
    }

}

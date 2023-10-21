using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    public float TimeTillDeath = 1f;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(transform.gameObject, TimeTillDeath);
    }

    // Update is called once per frame
    void Update()
    {

    }
}

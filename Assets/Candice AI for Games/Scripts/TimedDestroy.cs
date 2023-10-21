using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedDestroy : MonoBehaviour
{

    public float destroyTimer = 3f;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(transform.gameObject, destroyTimer);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

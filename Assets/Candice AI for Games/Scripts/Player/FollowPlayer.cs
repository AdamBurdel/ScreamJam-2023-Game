using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Candice AI
using CandiceAIforGames.AI;

public class FollowPlayer : MonoBehaviour
{

    public Transform player;
    public Vector3 followDistance;
    public bool KillCam = false;
    private float mousePosBuffer = 0f;
    public bool canRotate = true;

    void Start() {
        if (player == null) {
            player = GameObject.FindWithTag("Player").transform;            
        }
        mousePosBuffer = CandicePlayerOverrides.bufferMousePositionz;
    }


    // Update is called once per frame
    void Update()
    {
        if (player != null) {
            if (canRotate && !KillCam)
            {
                transform.position = player.transform.position + new Vector3(followDistance.x, followDistance.y, followDistance.z);
                transform.rotation = player.transform.rotation;
                CandicePlayerOverrides.bufferMousePositionz = mousePosBuffer + followDistance.y;
            }
            else if (!canRotate && !KillCam) {
                transform.position = player.transform.position + new Vector3(followDistance.x, followDistance.y, followDistance.z);                
                CandicePlayerOverrides.bufferMousePositionz = mousePosBuffer + followDistance.y;

            }
            else if (canRotate && KillCam)
            {
                transform.position = player.transform.position + new Vector3(followDistance.x, followDistance.y, followDistance.z);
                transform.rotation = Quaternion.LookRotation(transform.position);
            }
            else if (!canRotate && KillCam) {
                transform.position = player.transform.position + new Vector3(followDistance.x, followDistance.y, followDistance.z); ;
            }
        }
    }

}

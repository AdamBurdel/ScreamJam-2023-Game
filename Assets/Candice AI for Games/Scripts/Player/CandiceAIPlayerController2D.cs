using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]

public class CandiceAIPlayerController2D : MonoBehaviour
{

    //player movement
    private Rigidbody2D rb;
    [SerializeField] float speed = 4.0f;    
    public float animSpeedControl = 1f; //animation speed control
    [SerializeField] float jumpForce = 7.5f;
    [SerializeField] float rollForce = 6.0f;

    //facing
    private int direction = 1;
    private bool grounded = false;
    private bool rolling = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // -- Handle input and movement --
        float inputX = Input.GetAxis("Horizontal");

        // Swap direction of sprite depending on walk direction
        //if (inputX > 0)
        //{
        //    GetComponent<SpriteRenderer>().flipX = false;
        //    direction = 1;
        //}

        //else if (inputX < 0)
        //{
        //    GetComponent<SpriteRenderer>().flipX = true;
        //    direction = -1;
        //}

        // Move
        if (!rolling)
            rb.velocity = new Vector2(inputX * speed, rb.velocity.y);

        //Jump
        if (Input.GetKeyDown("space"))
        {
            //m_animator.SetTrigger("Jump");
            //audioManager.Play("GruntVoice02");
            //grounded = false;
            //m_animator.SetBool("Grounded", grounded);
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            //m_groundSensor.Disable(0.2f);
        }

    }
}

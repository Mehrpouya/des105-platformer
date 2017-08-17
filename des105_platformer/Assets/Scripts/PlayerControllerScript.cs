using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerScript : MonoBehaviour {

    public float maxSpeed = 10f;
    bool facingRight = true;
    Animator anim;
    bool movementAllowed = true;

    bool grounded = false;
    public GameObject groundCheck;
    float groundRadius = 0.2f;
    public LayerMask whatIsGround;
    public float jumpForce = 700f;
    

    // Use this for initialization
    void Start () {
        anim = GetComponent<Animator>();

    }
	
	// Update is called once per frame - Good for input/game mechanic updating
	void Update ()
    {
        if(grounded && Input.GetKeyDown(KeyCode.Space) && movementAllowed)
        {
            anim.SetBool("ground", false);
            GetComponent<Rigidbody2D>().AddForce(new Vector2(0, jumpForce));
        }
	}

    // Called at set interval each time. Good for physics
    void FixedUpdate()
    {
        Rigidbody2D rigidBodyComp = GetComponent<Rigidbody2D>();

        grounded = Physics2D.OverlapCircle(groundCheck.transform.position, groundRadius, whatIsGround);
        anim.SetBool("ground", grounded);

        anim.SetFloat("vSpeed", rigidBodyComp.velocity.y);

        float move = Input.GetAxis("Horizontal");

        if (movementAllowed)
        {
            rigidBodyComp.velocity = new Vector2(move * maxSpeed, rigidBodyComp.velocity.y);
            anim.SetFloat("speed", Mathf.Abs(move));
        }
        else
        {
            rigidBodyComp.velocity = new Vector2(Mathf.SmoothStep(rigidBodyComp.velocity.x, 0, Time.deltaTime), rigidBodyComp.velocity.y);
            anim.SetFloat("speed", Mathf.Abs(rigidBodyComp.velocity.x / maxSpeed));
        }
        

        if (move > 0 && !facingRight)
        {
            Flip();
        }
        else if (move < 0 && facingRight)
        {
            Flip();
        }

        if(rigidBodyComp.position.y <= -10f)
        {
            PlayerEndGame();
        }
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "PickupObject")
        {
            other.gameObject.GetComponent<PickupControllerScript>().onPickupTriggered();
        }
        else if(other.gameObject.tag == "ExitGate")
        {
            PlayerEndGame();
        }
    }

    private void PlayerEndGame()
    {
        movementAllowed = false;
        GameObject gameOverView = GameObject.Find("GameOverView");
        gameOverView.GetComponent<GameOverControllerScript>().BeginEndGame();
    }
}

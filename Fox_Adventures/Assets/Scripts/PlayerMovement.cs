using System;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;
using UnityEngine.Device;
using UnityEngine.UI;
using UnityEngine.XR;
using static UnityEngine.GraphicsBuffer;

public class PlayerMovement : MonoBehaviour
{
    private enum MovementState 
    {
        idel, 
        running, 
        jumping, 
        falling 
    };

    private int leverFlipped = 0;
    private float horizontalInput;
    private bool playerCanMove;
    private float jumpBoost;
    private float jumpBoostTimer;
    private float sprintBoost;
    private float sprintBoostTimer;
    private float screenWitdh = UnityEngine.Screen.width;
    private int platform;
    private Animator animator;
    private SpriteRenderer sprite;
    private GameObject go;
    private SpriteRenderer sr;
    [SerializeField] private ParticleSystem dust;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float jumpHight = 5f;
    [SerializeField] private float movementSpeed = 1f;
    [SerializeField] private Text leverFlippedText;
    [SerializeField] private Text jumpBoostText;
    [SerializeField] private Text sprintBoostText;
    [SerializeField] private LayerMask groundLayer;

    private void Start()
    {
        go = new GameObject();
        sr = GetComponent<SpriteRenderer>();
        
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        platform = UnityEngine.Application.platform.GetHashCode();
    }
    // Update is called once per frame
    void Update()
    {
        Booster();
        if (platform == 8f || platform == 11f)
        {
            TouchInput();
        }
        else
        {
            UpdateMovement();
        }
        PlayerAnimation();
        PlayerFallDown();
    }
    private void FixedUpdate()
    {
        if (playerCanMove)
        {
            rb.velocity = new Vector2(horizontalInput * movementSpeed * sprintBoost, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
        }
    }
    /// <summary>
    /// only render if TouchScreen is input device
    /// </summary>
    private void RenderControls()
    {
        
    }
    //Check for contackt with groundLayer
    private bool IsGrounded()
    {
        return Physics2D.OverlapBox(groundCheck.position, new Vector2(0.7f, 0.5f), 0, groundLayer);
    }
    private void UpdateMovement()
    {
        playerCanMove = FindAnyObjectByType<GameManager>().playerAlive;

        horizontalInput = Input.GetAxisRaw("Horizontal");
         if (Input.GetButtonDown("Jump") && IsGrounded() && playerCanMove)
         {
            rb.velocity = new Vector2(rb.velocity.x, jumpHight  * jumpBoost);
         }
         if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f)
         {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
         }
    }

    private void PlayerAnimation()
    {
        MovementState State;
        if (horizontalInput > 0f)
        {
            State = MovementState.running;
            sprite.flipX = false;
        }
        else if (horizontalInput < 0f)
        {
            State = MovementState.running;
            sprite.flipX = true;
        }
        else
        {
            State = MovementState.idel;
        }
        if (rb.velocity.y > 0.1f)
        {
            State = MovementState.jumping;
        }
        else if (rb.velocity.y < -0.1f)
        {
            State = MovementState.falling;
        }
        if (playerCanMove == false)
        {
            State = MovementState.idel;
        }
        animator.SetInteger("State", (int)State);
        if ((int)State == 1f)
        {
            CreateDust();
        }
    }
    private void CreateDust()
    {
        dust.Play();
    }

    private void PlayerFallDown()
    {
        if (rb.transform.position.y < -6f && FindAnyObjectByType<GameManager>().playerAlive)
        {
            FindAnyObjectByType<GameManager>().GameOver();
        }
    }
    //Check for Collision
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "JumpBlockLever")
        {
            Destroy(collision.gameObject);
            leverFlipped++;
            leverFlippedText.text = leverFlipped.ToString();
        }
        else if (collision.tag == "PilleRot")
        {
            Destroy(collision.gameObject);
            jumpBoostTimer = 5f;
        }
        else if (collision.tag == "PilleBlau")
        {
            Destroy(collision.gameObject);
            sprintBoostTimer = 5f;
        }
        else if (collision.tag == "EndTrigger")
        {
            FindAnyObjectByType<GameManager>().LevelCompleted();
        }
        else if (collision.tag == "Obstacle" && playerCanMove)
        {
            FindAnyObjectByType<GameManager>().GameOver();
        }
        else if (collision.tag == "JumpBlock" && leverFlipped != 0)
        {
            leverFlipped--;
            rb.velocity = new Vector2(rb.velocity.x, 20f);
            leverFlippedText.text = leverFlipped.ToString();
        }
        else
            return;
    }
    private void Booster()
    {
        if (jumpBoostTimer > 0f)
        {
            jumpBoost = 1.3f;
            jumpBoostTimer -= 1f * Time.deltaTime;
            jumpBoostText.text = jumpBoostTimer.ToString("0");
        }
        else
        {
            jumpBoost = 1f;
        }
        if (sprintBoostTimer > 0f)
        {
            sprintBoost = 1.5f;
            sprintBoostTimer -= 1f * Time.deltaTime;
            sprintBoostText.text = sprintBoostTimer.ToString("0");
        }
        else
        {
            sprintBoost = 1f;
        }
    }

    private void TouchInput()
    {
        horizontalInput = 0f;

        foreach (Touch touch in Input.touches)
        {
            
            playerCanMove = FindAnyObjectByType<GameManager>().playerAlive;
            if (touch.position.x < screenWitdh / 3f && playerCanMove)
            {
                //jump
                if (IsGrounded())
                {
                    rb.velocity = new Vector2(rb.velocity.x, jumpHight * jumpBoost);
                }
                if (rb.velocity.y > 0f)
                {
                   //rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
                }
            }
            if (touch.position.x  > screenWitdh / 3f && touch.position.x < screenWitdh *11f/12f)
            {
                horizontalInput = -1f;
                Debug.Log("left");
            }
            else if (touch.position.x > screenWitdh *11/12)
            {
                horizontalInput = +1f;
                Debug.Log("right");
                Debug.Log(playerCanMove);
            }
        }
    }
    
}

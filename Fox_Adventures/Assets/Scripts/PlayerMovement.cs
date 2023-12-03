using System.Threading;
using UnityEngine;
using UnityEngine.Device;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    private enum movementState { idel, running, jumping, falling };
    private int leverFlipped = 0;
    private float horizontalInput;
    private bool playerCanMove;
    private float jumpBoost;
    private float jumpBoostTimer;
    private float sprintBoost;
    private float sprintBoostTimer;
    private float screenWitdh = UnityEngine.Screen.width;
    private Animator animator;
    private SpriteRenderer sprite;
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
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
    }
    // Update is called once per frame
    void Update()
    {
        booster(); 
        Touchinput();
        playerAnimation();
        //playerMovement();
        playerFallDown();
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
    //Check for contackt with groundLayer
    private bool IsGrounded()
    {
        return Physics2D.OverlapBox(groundCheck.position, new Vector2(0.7f, 0.5f), 0, groundLayer);
    }
    private void playerMovement()
    {
        playerCanMove = FindAnyObjectByType<GameManager>().playerAlive;
        if (UnityEngine.Application.platform == RuntimePlatform.WindowsEditor)
        {
            Touchinput();
            return;
        }
        if (playerCanMove)
        {
            horizontalInput = Input.GetAxisRaw("Horizontal");
            if (Input.GetButtonDown("Jump") && IsGrounded())
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpHight  * jumpBoost);
            }
            if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f)
            {
               rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            }
        }
    }

    private void playerAnimation()
    {
        movementState State;
        if (horizontalInput > 0f)
        {
            State = movementState.running;
            sprite.flipX = false;
        }
        else if (horizontalInput < 0f)
        {
            State = movementState.running;
            sprite.flipX = true;
        }
        else
        {
            State = movementState.idel;
        }
        if (rb.velocity.y > 0.1f)
        {
            State = movementState.jumping;
        }
        else if (rb.velocity.y < -0.1f)
        {
            State = movementState.falling;
        }
        if (playerCanMove == false)
        {
            State = movementState.idel;
        }
        animator.SetInteger("State", (int)State);
        if ((int)State == 1f)
        {
            createDust();
        }
    }
    private void createDust()
    {
        dust.Play();
    }

    private void playerFallDown()
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
    private void booster()
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

    private void Touchinput()
    {
        horizontalInput = 0f;

        foreach (Touch touch in Input.touches)
        {
            
            playerCanMove = FindAnyObjectByType<GameManager>().playerAlive;
            if (touch.position.x < screenWitdh / 3f)
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
            if (touch.position.x  > screenWitdh / 3f && touch.position.x < screenWitdh *2f/3f)
            {
                horizontalInput = -1f;
                Debug.Log("left");
            }
            else if (touch.position.x > screenWitdh *2/3)
            {
                horizontalInput = +1f;
                Debug.Log("right");
                Debug.Log(playerCanMove);
            }
        }
    }
    
}

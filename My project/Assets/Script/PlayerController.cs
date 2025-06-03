using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float runSpeed = 10f;
    public float jumpForce = 6f;
    public float checkRadius = 0.05f;
    public LayerMask groundLayer;
    public Vector2 buttonOffset;
    public float runFastTime = 0f;
    public float runFastScale = 1.3f;
    public float runFastStartTime;
    public float runFastEndTime;
    public int onlyone = 1;

    public Rigidbody2D rb;
    public PolygonCollider2D mybody;
    private bool isStinger;

    private bool isGround;
    private bool isJumping;
    private bool isWin;
    private bool isTouchingRunFast;

    //Ã¯‘æ‘§ ‰»Î
    public float jumpBufferTime = 0.2f;
    public float lastJumpPressTime = -10f;
    private bool jumpWasPressed = false;
    // Start is called before the first frame update
    void Start()
    {
        buttonOffset = new Vector2(0f, -0.5f);
        rb = GetComponent<Rigidbody2D>();
        mybody = GetComponent<PolygonCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        ProcessJumpInput();
        check();
        Move();
        tryJump();
        die();
        win();
    }
    void checkStinger()
    {
        isStinger = mybody.IsTouchingLayers(LayerMask.GetMask("Stinger"));
    }
    void checkGround()
    {
        isGround = Physics2D.OverlapCircle((Vector2)transform.position + buttonOffset, checkRadius, groundLayer);
    }
    void check()
    {
        isStinger = mybody.IsTouchingLayers(LayerMask.GetMask("Stinger"));
        isGround = Physics2D.OverlapCircle((Vector2)transform.position + buttonOffset, checkRadius, groundLayer);
        isWin = mybody.IsTouchingLayers(LayerMask.GetMask("winFlag"));
        isTouchingRunFast = mybody.IsTouchingLayers(LayerMask.GetMask("runFast"));
    }
    void ProcessJumpInput()
    {
        if(Input.GetButtonDown("Jump"))
        {
            jumpWasPressed = true;
            lastJumpPressTime = Time.time;
        }
    }
    void Move()
    {
        rb.velocity = new Vector2(runSpeed, rb.velocity.y);
    }
    void runFastStart()
    {
        if (isTouchingRunFast && (onlyone == 1))
        {
            onlyone = 0;
            runSpeed = runSpeed * runFastScale;
            runFastTime = 3f;
        }
    }
    void runFastDuration()
    {

    }

    void tryJump()
    {
        bool withinBuffer = (Time.time - lastJumpPressTime) <= jumpBufferTime;
        if (Input.GetButtonDown("Jump")&&isGround )
        {
            Vector2 JumpVel = new Vector2(0.0f, jumpForce);
            rb.velocity = Vector2.up * JumpVel;
            isJumping = true;
        }else if((jumpWasPressed && withinBuffer)&&isGround)
        {
            Vector2 JumpVel = new Vector2(0.0f, jumpForce);
            rb.velocity = Vector2.up * JumpVel;
            isJumping = true;

            jumpWasPressed = false;
            lastJumpPressTime = -10f;
        }
        
                  
    }
    void die()
    {
        if(isStinger)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
    void win()
    {
        if (isWin)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
    public void SetSpeed(float speed)
    {
        runSpeed = speed;
    }
    public void SetGravityScale(float scale)
    {
        GetComponent<Rigidbody2D>().gravityScale = scale;
    }
}

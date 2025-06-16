using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.ParticleSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Set in Inspector")]
    [Header("关于加速道具效果")]
    public float runSpeed = 10f;
    public float currentSpeed;// 当前速度
    public float boostSpeed = 13f;
    public float goFastTime = 0;
    public float boostSpeedTime = 3f;
    public bool isBoostedSpeed = false;

    [Header("关于跳跃能力增强道具")]
    public float boostJumpForce = 15f;
    public bool isBoostedJump = false;
    public float originalJumpForce = 6f;


    public float jumpForce = 6f;
    public float checkRadius = 0.05f;
    public LayerMask groundLayer;
    public Vector2 buttonOffset;
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

    //跳跃预输入
    public float jumpBufferTime = 0.2f;
    public float lastJumpPressTime = -10f;
    private bool jumpWasPressed = false;

    // 粒子系统缓存
    private ParticleSystem[] particles;
    private Color originalColor;

    // Start is called before the first frame update
    void Start()
    {
        originalJumpForce = jumpForce;
        currentSpeed = runSpeed;
        buttonOffset = new Vector2(0f, -0.5f);
        rb = GetComponent<Rigidbody2D>();
        mybody = GetComponent<PolygonCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        ProcessJumpInput();
        check();
        HandleBoost();
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
        rb.velocity = new Vector2(currentSpeed, rb.velocity.y);
    }

    void tryJump()
    {
        bool withinBuffer = (Time.time - lastJumpPressTime) <= jumpBufferTime;
        bool shouldJump = (Input.GetButtonDown("Jump") && isGround) ||
                        (jumpWasPressed && withinBuffer && isGround);

        if (shouldJump)
        {
            float currentJumpForce = isBoostedJump ? boostJumpForce : originalJumpForce;

            Vector2 JumpVel = new Vector2(0.0f, currentJumpForce);
            rb.velocity = Vector2.up * JumpVel;
            isJumping = true;

            // 如果是增强跳跃，仅重置标志位
            if (isBoostedJump)
            {
                isBoostedJump = false;
                // 不需要重置jumpForce，保持原始值
            }

            // 重置预输入状态
            jumpWasPressed = false;
            lastJumpPressTime = -10f;
        }

        /*if (Input.GetButtonDown("Jump")&&isGround )
        {
            if (isBoostedJump)
            {
                jumpForce = boostJumpForce;
            }
            Vector2 JumpVel = new Vector2(0.0f, jumpForce);
            rb.velocity = Vector2.up * JumpVel;
            isJumping = true;
            EndJumpBoost();
           
        }else if((jumpWasPressed && withinBuffer)&&isGround)
        {
            if (isBoostedJump)
            {
                jumpForce = boostJumpForce;
            }
            Vector2 JumpVel = new Vector2(0.0f, jumpForce);
            rb.velocity = Vector2.up * JumpVel;
            isJumping = true;

            jumpWasPressed = false;
            lastJumpPressTime = -10f;
            EndJumpBoost() ;
        }
        */


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
        rb.gravityScale = scale;
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("SpeedBoost"))
        {
            ActivateSpeedBoost();
            Destroy(collision.gameObject);
        }
        if (collision.CompareTag("JumpBoost"))
        {
            isBoostedJump = true;
            Destroy(collision.gameObject);
            print("跳跃增强");
        }
    }
    public void ActivateSpeedBoost()
    {
        goFastTime = boostSpeedTime;
        isBoostedSpeed = true;
        currentSpeed = boostSpeed;
    }
    private void HandleBoost()
    {
        if (isBoostedSpeed)
        {
            // 更新加速倒计时
            goFastTime -= Time.deltaTime;

            // 加速时间结束
            if (goFastTime <= 0)
            {
                EndSpeedBoost();
            }
        }
       
        
    }
    private void EndSpeedBoost()
    {
        isBoostedSpeed = false;
        currentSpeed = runSpeed;  // 恢复基础速度
        goFastTime = 0f;           // 重置计时器
    }
    private void EndJumpBoost()
    {
        isBoostedJump = false;
        jumpForce = 10f;
    }
}

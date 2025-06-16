using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.ParticleSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Set in Inspector")]
    [Header("���ڼ��ٵ���Ч��")]
    public float runSpeed = 10f;
    public float currentSpeed;// ��ǰ�ٶ�
    public float boostSpeed = 13f;
    public float goFastTime = 0;
    public float boostSpeedTime = 3f;
    public bool isBoostedSpeed = false;

    [Header("������Ծ������ǿ����")]
    public float boostJumpForce = 15f;
    public bool isBoostedJump = false;
    public float originalJumpForce = 6f;

    [Header("���ڼ��ٵ���")]
    public bool isFrozenSpeed = false;
    public float frozenSpeed = 5f;
    public float currentFrozenTime = 0;
    public float frozenSpeedTime = 2f;

    [Header("���ڷ��е���")]
    public bool isFlyMode = false;
    public float boostFlyVercitalHeight = 3f;
    public float flyForce = 8f;
    public float flyDuration = 20f;
    public float flyTimeLeft = 0;
    public float originalGravityScale;
    public float verticalFlySpeed = 5;

    [Header("���ڱ߽��ж�")]
    public float bottomY = -25;
    public bool isOverY = false;



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

    //��ԾԤ����
    public float jumpBufferTime = 0.2f;
    public float lastJumpPressTime = -10f;
    private bool jumpWasPressed = false;

    // ����ϵͳ����
    private ParticleSystem[] particles;
    private Color originalColor;

    // Start is called before the first frame update
    void Start()
    {
        originalGravityScale = rb.gravityScale;
        originalJumpForce = jumpForce;
        currentSpeed = runSpeed;
        buttonOffset = new Vector2(0f, -0.5f);
        rb = GetComponent<Rigidbody2D>();
        mybody = GetComponent<PolygonCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        check();
        HandleBoost();
        HandleFlyMode();
        if (!isFlyMode)
        {
            Move();
            ProcessJumpInput();
            tryJump();
        }
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
        isOverY = (transform.position.y<-25)?true:false;
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

            // �������ǿ��Ծ�������ñ�־λ
            if (isBoostedJump)
            {
                isBoostedJump = false;
                // ����Ҫ����jumpForce������ԭʼֵ
            }

            // ����Ԥ����״̬
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
        if(isStinger || isOverY)
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
            print("��Ծ��ǿ");
        }
        if (collision.CompareTag("FrozenSpeed"))
        {
            FrozenSpeed();
            Destroy(collision.gameObject);
        }
        if (collision.CompareTag("FlyFruit"))
        {
            StartFlyMode();
            Destroy(collision.gameObject);
            print("��������");
        }
    }
    public void FrozenSpeed()
    {
        currentFrozenTime = frozenSpeedTime;
        isFrozenSpeed = true;
        currentSpeed = frozenSpeed;
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
            // ���¼��ٵ���ʱ
            goFastTime -= Time.deltaTime;

            // ����ʱ�����
            if (goFastTime <= 0)
            {
                EndSpeedBoost();
            }
        }
        if(isFrozenSpeed)
        {
            currentFrozenTime -= Time.deltaTime;
            if(currentFrozenTime <= 0)
            {
                EndFrozenSpeed();
            }
        }  
    }
    void HandleFlyMode()
    {
        if (isFlyMode)
        {
            float verticalInput = Input.GetAxis("Vertical");
            if (verticalInput != 0)
            {
                rb.velocity = new Vector2(currentSpeed, verticalInput * verticalFlySpeed); ;
            }
            else
            {
                rb.velocity = new Vector2(currentSpeed, 0f);
            }
            flyTimeLeft -= Time.deltaTime;

            if (flyTimeLeft <= 0)
            {
                EndFlyMode();
            }
        }
    }
    public void StartFlyMode()
    {
        rb.velocity = new Vector2(rb.velocity.x, boostFlyVercitalHeight);
        isFlyMode = true;
        flyTimeLeft = flyDuration;

        rb.gravityScale = 0f;
        isStinger = false;
    }
    private void EndFlyMode()
    {
        isFlyMode = false;
        flyTimeLeft = 0f;

        // �ָ�����
        rb.gravityScale = originalGravityScale;

        // ��ʼ����ֱ�ٶ�
        rb.velocity = new Vector2(rb.velocity.x, 0f);
    }
    private void EndSpeedBoost()
    {
        isBoostedSpeed = false;
        currentSpeed = runSpeed;  // �ָ������ٶ�
        goFastTime = 0f;           // ���ü�ʱ��
    }
    private void EndFrozenSpeed()
    {
        isFrozenSpeed = false;
        currentSpeed = runSpeed;
        frozenSpeedTime = 0;
    }
    private void EndJumpBoost()
    {
        isBoostedJump = false;
        jumpForce = 10f;
    }

}

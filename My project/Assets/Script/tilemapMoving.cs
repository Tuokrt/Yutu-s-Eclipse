using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(TilemapCollider2D))]
public class tilemapMoving : MonoBehaviour
{
    [Header("垂直移动设置")]
    public float movementSpeed = 2f;           // 移动速度
    public float minMoveDistance = 1f;          // 最小移动距离
    public float maxMoveDistance = 3f;         // 最大移动距离
    public float directionChangeInterval = 3f;  // 方向变化间隔

    [Header("移动行为")]
    public bool enablePauses = true;            // 是否在端点暂停
    public float minPauseDuration = 0.5f;       // 最小暂停时间
    public float maxPauseDuration = 1.5f;       // 最大暂停时间

    [Header("可见性控制")]
    public bool showMovementPath = true;        // 显示移动路径

    // 私有变量
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float directionTimer;
    private float pauseTimer;
    private bool isMovingUp = true; // 当前移动方向（上或下）
    private bool isPaused;
    private float currentMoveDistance;

    void Start()
    {
        InitializeMovement();
    }

    // 初始化移动系统
    void InitializeMovement()
    {
        startPosition = transform.position;
        directionTimer = directionChangeInterval; // 立即触发首次方向变化
        pauseTimer = 0f;
        isPaused = false;
    }

    void Update()
    {
        if (isPaused)
        {
            HandlePause();
            return;
        }

        HandleDirectionChange();
        MovePlatform();
    }

    // 处理方向变化逻辑
    void HandleDirectionChange()
    {
        directionTimer += Time.deltaTime;

        // 到达方向变化时间间隔时
        if (directionTimer >= directionChangeInterval)
        {
            RandomizeMovement();
            directionTimer = 0f;
        }
    }

    // 处理暂停逻辑
    void HandlePause()
    {
        if (pauseTimer > 0f)
        {
            pauseTimer -= Time.deltaTime;
        }
        else
        {
            isPaused = false;
            // 继续移动前重新计算目标点
            CalculateNewTarget();
        }
    }

    // 随机化移动参数
    void RandomizeMovement()
    {
        // 在上下方向随机切换（60%概率改变方向）
        if (Random.value > 0.4f)
        {
            isMovingUp = !isMovingUp;
        }

        // 随机移动距离
        currentMoveDistance = Random.Range(minMoveDistance, maxMoveDistance);

        CalculateNewTarget();
    }

    // 计算新的目标位置
    void CalculateNewTarget()
    {
        // 总是计算垂直方向移动
        targetPosition = startPosition + new Vector3(
            0,
            isMovingUp ? currentMoveDistance : -currentMoveDistance,
            0
        );
    }

    // 移动平台
    void MovePlatform()
    {
        float step = movementSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            step
        );

        // 接近目标点时
        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            if (enablePauses)
            {
                // 随机暂停时间
                pauseTimer = Random.Range(minPauseDuration, maxPauseDuration);
                isPaused = true;
            }
            else
            {
                // 无暂停直接反向
                isMovingUp = !isMovingUp;
                CalculateNewTarget();
            }
        }
    }

    // 调试可视化
    void OnDrawGizmosSelected()
    {
        if (!showMovementPath)
            return;

        // 在场景视图中显示移动范围
        Gizmos.color = Color.cyan;

        // 起点位置
        Vector3 origin = Application.isPlaying ? startPosition : transform.position;

        // 显示垂直移动范围
        Gizmos.DrawLine(
            origin + Vector3.down * maxMoveDistance,
            origin + Vector3.up * maxMoveDistance
        );

        // 显示当前目标位置
        if (Application.isPlaying)
        {
            Gizmos.DrawWireSphere(targetPosition, 0.2f);
            Gizmos.DrawLine(transform.position, targetPosition);
        }
    }

    // 为外部提供控制接口
    public void IncreaseSpeed(float amount)
    {
        movementSpeed = Mathf.Max(0.5f, movementSpeed + amount);
    }

    public void PauseMovement()
    {
        isPaused = true;
    }

    public void ResumeMovement()
    {
        isPaused = false;
    }
}
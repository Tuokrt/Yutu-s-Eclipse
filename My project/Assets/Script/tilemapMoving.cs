using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(TilemapCollider2D))]
public class tilemapMoving : MonoBehaviour
{
    [Header("垂直移动设置")]
    public float movementSpeed = 2f;           // 移动速度
    public float moveDistance = 3f;             // 移动距离（向上向下相同）
    public bool startMovingDown = false;        // 是否从向下移动开始

    [Header("端点暂停设置")]
    public bool enablePauses = true;            // 是否在端点暂停
    public float pauseDuration = 1f;            // 端点暂停时间

    [Header("可见性控制")]
    public bool showMovementPath = true;        // 显示移动路径

    // 私有变量
    private Vector3 startPosition;
    private Vector3 topPosition;
    private Vector3 bottomPosition;
    private Vector3 targetPosition;
    private float pauseTimer;
    private bool isPaused;

    void Start()
    {
        InitializeMovement();
    }

    // 初始化移动系统
    void InitializeMovement()
    {
        startPosition = transform.position;
        topPosition = startPosition + new Vector3(0, moveDistance, 0);
        bottomPosition = startPosition + new Vector3(0, -moveDistance, 0);

        targetPosition = startMovingDown ? bottomPosition : topPosition;
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

        MovePlatform();
    }

    // 处理暂停逻辑
    void HandlePause()
    {
        pauseTimer -= Time.deltaTime;

        if (pauseTimer <= 0f)
        {
            isPaused = false;

            // 切换目标位置
            if (Vector3.Distance(transform.position, topPosition) < 0.01f)
            {
                targetPosition = bottomPosition;
            }
            else if (Vector3.Distance(transform.position, bottomPosition) < 0.01f)
            {
                targetPosition = topPosition;
            }
        }
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
                pauseTimer = pauseDuration;
                isPaused = true;
            }
            else
            {
                // 无暂停直接反向
                targetPosition = (targetPosition == topPosition) ? bottomPosition : topPosition;
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

        // 显示上下移动端点
        Vector3 topPoint = origin + Vector3.up * moveDistance;
        Vector3 bottomPoint = origin + Vector3.down * moveDistance;

        Gizmos.DrawWireSphere(topPoint, 0.2f);
        Gizmos.DrawWireSphere(bottomPoint, 0.2f);
        Gizmos.DrawLine(topPoint, bottomPoint);
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

    // 设置新的移动距离（用于关卡设计或难度调整）
    public void SetMoveDistance(float newDistance)
    {
        moveDistance = Mathf.Max(1f, newDistance);
        topPosition = startPosition + new Vector3(0, moveDistance, 0);
        bottomPosition = startPosition + new Vector3(0, -moveDistance, 0);

        // 重新计算当前最接近的目标点
        float toTop = Vector3.Distance(transform.position, topPosition);
        float toBottom = Vector3.Distance(transform.position, bottomPosition);

        targetPosition = toTop < toBottom ? topPosition : bottomPosition;
    }
}
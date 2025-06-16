using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(TilemapCollider2D))]
public class tilemapMoving : MonoBehaviour
{
    [Header("��ֱ�ƶ�����")]
    public float movementSpeed = 2f;           // �ƶ��ٶ�
    public float minMoveDistance = 1f;          // ��С�ƶ�����
    public float maxMoveDistance = 3f;         // ����ƶ�����
    public float directionChangeInterval = 3f;  // ����仯���

    [Header("�ƶ���Ϊ")]
    public bool enablePauses = true;            // �Ƿ��ڶ˵���ͣ
    public float minPauseDuration = 0.5f;       // ��С��ͣʱ��
    public float maxPauseDuration = 1.5f;       // �����ͣʱ��

    [Header("�ɼ��Կ���")]
    public bool showMovementPath = true;        // ��ʾ�ƶ�·��

    // ˽�б���
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float directionTimer;
    private float pauseTimer;
    private bool isMovingUp = true; // ��ǰ�ƶ������ϻ��£�
    private bool isPaused;
    private float currentMoveDistance;

    void Start()
    {
        InitializeMovement();
    }

    // ��ʼ���ƶ�ϵͳ
    void InitializeMovement()
    {
        startPosition = transform.position;
        directionTimer = directionChangeInterval; // ���������״η���仯
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

    // ������仯�߼�
    void HandleDirectionChange()
    {
        directionTimer += Time.deltaTime;

        // ���﷽��仯ʱ����ʱ
        if (directionTimer >= directionChangeInterval)
        {
            RandomizeMovement();
            directionTimer = 0f;
        }
    }

    // ������ͣ�߼�
    void HandlePause()
    {
        if (pauseTimer > 0f)
        {
            pauseTimer -= Time.deltaTime;
        }
        else
        {
            isPaused = false;
            // �����ƶ�ǰ���¼���Ŀ���
            CalculateNewTarget();
        }
    }

    // ������ƶ�����
    void RandomizeMovement()
    {
        // �����·�������л���60%���ʸı䷽��
        if (Random.value > 0.4f)
        {
            isMovingUp = !isMovingUp;
        }

        // ����ƶ�����
        currentMoveDistance = Random.Range(minMoveDistance, maxMoveDistance);

        CalculateNewTarget();
    }

    // �����µ�Ŀ��λ��
    void CalculateNewTarget()
    {
        // ���Ǽ��㴹ֱ�����ƶ�
        targetPosition = startPosition + new Vector3(
            0,
            isMovingUp ? currentMoveDistance : -currentMoveDistance,
            0
        );
    }

    // �ƶ�ƽ̨
    void MovePlatform()
    {
        float step = movementSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            step
        );

        // �ӽ�Ŀ���ʱ
        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            if (enablePauses)
            {
                // �����ͣʱ��
                pauseTimer = Random.Range(minPauseDuration, maxPauseDuration);
                isPaused = true;
            }
            else
            {
                // ����ֱͣ�ӷ���
                isMovingUp = !isMovingUp;
                CalculateNewTarget();
            }
        }
    }

    // ���Կ��ӻ�
    void OnDrawGizmosSelected()
    {
        if (!showMovementPath)
            return;

        // �ڳ�����ͼ����ʾ�ƶ���Χ
        Gizmos.color = Color.cyan;

        // ���λ��
        Vector3 origin = Application.isPlaying ? startPosition : transform.position;

        // ��ʾ��ֱ�ƶ���Χ
        Gizmos.DrawLine(
            origin + Vector3.down * maxMoveDistance,
            origin + Vector3.up * maxMoveDistance
        );

        // ��ʾ��ǰĿ��λ��
        if (Application.isPlaying)
        {
            Gizmos.DrawWireSphere(targetPosition, 0.2f);
            Gizmos.DrawLine(transform.position, targetPosition);
        }
    }

    // Ϊ�ⲿ�ṩ���ƽӿ�
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
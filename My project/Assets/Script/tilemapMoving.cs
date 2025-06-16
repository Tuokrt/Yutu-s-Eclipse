using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(TilemapCollider2D))]
public class tilemapMoving : MonoBehaviour
{
    [Header("��ֱ�ƶ�����")]
    public float movementSpeed = 2f;           // �ƶ��ٶ�
    public float moveDistance = 3f;             // �ƶ����루����������ͬ��
    public bool startMovingDown = false;        // �Ƿ�������ƶ���ʼ

    [Header("�˵���ͣ����")]
    public bool enablePauses = true;            // �Ƿ��ڶ˵���ͣ
    public float pauseDuration = 1f;            // �˵���ͣʱ��

    [Header("�ɼ��Կ���")]
    public bool showMovementPath = true;        // ��ʾ�ƶ�·��

    // ˽�б���
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

    // ��ʼ���ƶ�ϵͳ
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

    // ������ͣ�߼�
    void HandlePause()
    {
        pauseTimer -= Time.deltaTime;

        if (pauseTimer <= 0f)
        {
            isPaused = false;

            // �л�Ŀ��λ��
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
                pauseTimer = pauseDuration;
                isPaused = true;
            }
            else
            {
                // ����ֱͣ�ӷ���
                targetPosition = (targetPosition == topPosition) ? bottomPosition : topPosition;
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

        // ��ʾ�����ƶ��˵�
        Vector3 topPoint = origin + Vector3.up * moveDistance;
        Vector3 bottomPoint = origin + Vector3.down * moveDistance;

        Gizmos.DrawWireSphere(topPoint, 0.2f);
        Gizmos.DrawWireSphere(bottomPoint, 0.2f);
        Gizmos.DrawLine(topPoint, bottomPoint);
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

    // �����µ��ƶ����루���ڹؿ���ƻ��Ѷȵ�����
    public void SetMoveDistance(float newDistance)
    {
        moveDistance = Mathf.Max(1f, newDistance);
        topPosition = startPosition + new Vector3(0, moveDistance, 0);
        bottomPosition = startPosition + new Vector3(0, -moveDistance, 0);

        // ���¼��㵱ǰ��ӽ���Ŀ���
        float toTop = Vector3.Distance(transform.position, topPosition);
        float toBottom = Vector3.Distance(transform.position, bottomPosition);

        targetPosition = toTop < toBottom ? topPosition : bottomPosition;
    }
}
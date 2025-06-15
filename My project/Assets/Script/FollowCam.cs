using UnityEngine;

public class AdvancedFollowCam : MonoBehaviour
{
    public Transform target;
    [Range(0.1f, 2f)] public float smoothTime = 0.3f;
    public float yPosition = 0f;
    public float zPosition = -10f;
    public float xOffset = 2f;

    public Vector3 shouldVector3;

    [Header("�߽�����")]
    public float minX = -10f;
    public float maxX = 100f;

    [Header("�߼�����")]
    public bool compensateTimeScale = true;
    public bool addPredictiveOffset = true;
    public float predictionFactor = 0.3f;

    private Vector3 velocity;
    private Vector3 lastTargetPosition;
    private Vector3 targetPosition;

    /*
    void Start()
    {
        if (target == null)
        {
            Debug.LogError("Ŀ��δ����!");
            enabled = false;
            return;
        }

        lastTargetPosition = target.position;
        targetPosition = CalculateTargetPosition();
        transform.position = targetPosition;
    }

    void Update()
    {
        if (target == null) return;

        // ����Ԥ��ƫ��
        Vector3 predictionOffset = Vector3.zero;
        if (addPredictiveOffset && !Mathf.Approximately(Time.timeScale, 0))
        {
            Vector3 targetVelocity = (target.position - lastTargetPosition) / Time.deltaTime;
            predictionOffset = targetVelocity * predictionFactor * GetEffectiveDeltaTime();
        }

        lastTargetPosition = target.position;
        targetPosition = CalculateTargetPosition() + predictionOffset;

        // Ӧ��ƽ���ƶ�
        float actualSmoothTime = GetEffectiveSmoothTime();
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref velocity,
            actualSmoothTime
        );
    }

    Vector3 CalculateTargetPosition()
    {
        float clampedX = Mathf.Clamp(target.position.x + xOffset, minX, maxX);
        return new Vector3(clampedX, yPosition, zPosition);
    }

    float GetEffectiveDeltaTime()
    {
        return compensateTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
    }

    float GetEffectiveSmoothTime()
    {
        float delta = GetEffectiveDeltaTime();
        // ��ֹ�������
        return Mathf.Max(smoothTime, delta * 1.1f);
    }

    // ����ʱ�����߽�
    public void SetCameraLimits(float newMinX, float newMaxX)
    {
        minX = newMinX;
        maxX = newMaxX;
    }
    */
    public Vector3 offset = new Vector3(0, 0, 10);

    // Start is called before the first frame update
    void Start()
    {
        shouldVector3 = new Vector3(target.position.x + 2f, 0f, -10f); ;
        transform.position = shouldVector3;
    }

    // Update is called once per frame
    void Update()
    {
        shouldVector3 = new Vector3(target.position.x + 2f, 0f, -10f); ;
        transform.position = shouldVector3;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class DrawZone
{
    public Transform triggerPoint;
    public float radius;
    public int maxTiles;

}
public class PathDrawer : MonoBehaviour
{ 
    [Header("��Ƭ��ͼ����")]
    public Tilemap drawableTilemap;
    public TileBase pathTile;

    [SerializeField] List<DrawZone> drawZones = new List<DrawZone>();
    private DrawZone currentZone;

    [Header("��������")]
    public float minDrawDistance = 0.5f;
    public Color drawPreviewColor = new Color(0.2f, 0.8f, 0.7f);
    public float drawCooldown = 0.1f;

    public List<Transform> triggerPoints = new List<Transform>();
    public float triggerRadius = 2f;

    [Header("��Դ����")]
    public int maxPathTiles = 30; // ���ɻ�����Ƭ��
    public int tilesDrawn = 0;    // �ѻ�����Ƭ��

    private bool isDrawingMode = false;
    private Vector3 lastDrawPosition;
    

    public float slowdownFactor = 0.3f;
    private float lastDrawTime;
    private List<Vector3Int> previewCells = new List<Vector3Int>();
    private Transform currentTriggerPoint;

    private LineRenderer pathPreview;
    private PlayerController playerController;
    private float originalPlayerSpeed;
    private float originalGravityScale;

   

    // Start is called before the first frame update
    void Start()
    {
        GameObject previewObj = new GameObject("PathPreview");
        pathPreview = previewObj.AddComponent<LineRenderer>();
        pathPreview.startWidth = 0.1f;
        pathPreview.endWidth = 0.1f;
        pathPreview.material = new Material(Shader.Find("Sprites/Default"));
        pathPreview.startColor = drawPreviewColor;
        pathPreview.endColor = drawPreviewColor;
        pathPreview.positionCount = 0;
        pathPreview.enabled = false;

        playerController = FindObjectOfType<PlayerController>();

        // �������ԭʼ����
        if (playerController != null)
        {
            originalPlayerSpeed = playerController.runSpeed;
            originalGravityScale = playerController.GetComponent<Rigidbody2D>().gravityScale;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // �������Ƿ񵽴ﴥ����
        if (!isDrawingMode && playerController != null) 
        {
            foreach(Transform triggerPoint in triggerPoints)
            {
                if(Vector3.Distance(playerController.transform.position, triggerPoint.position)<triggerRadius)
                {
                    EnterDrawingMode(triggerPoint);
                    break;
                }
            }
        }

        // ����ģʽ�߼�
        if (isDrawingMode)
        {
            HandleDrawingInput();
            UpdatePreview();

            // �������Ƿ��뿪��������
            if (currentTriggerPoint != null &&
                Vector3.Distance(playerController.transform.position, currentTriggerPoint.position) > triggerRadius * 1.5f)
            {
                ExitDrawingMode();
            }
        }
    }
    void EnterDrawingMode(Transform triggerPoint)
    {
        isDrawingMode = true;
        currentTriggerPoint = triggerPoint;

        // ��������ж�
        if (playerController != null)
        {
            playerController.runSpeed = originalPlayerSpeed * slowdownFactor;
            playerController.GetComponent<Rigidbody2D>().gravityScale = originalGravityScale * slowdownFactor;
        }

        pathPreview.enabled = true;
        Debug.Log($"�������ģʽ! λ��: {triggerPoint.position}, ʣ����Ƭ: {maxPathTiles - tilesDrawn}");
    }
    void ExitDrawingMode()
    {
        isDrawingMode = false;

        // �ָ�����ж�
        if (playerController != null)
        {
            playerController.runSpeed = originalPlayerSpeed;
            playerController.GetComponent<Rigidbody2D>().gravityScale = originalGravityScale;
        }

        pathPreview.enabled = false;
        ClearPreview();
        currentTriggerPoint = null;
        Debug.Log("�˳�����ģʽ");
    }
    void HandleDrawingInput()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;

        // ��ʼ����
        if (Input.GetMouseButtonDown(0))
        {
            lastDrawPosition = mouseWorldPos;
            lastDrawTime = Time.time;
        }

        // ��������
        if (Input.GetMouseButton(0) && Time.time - lastDrawTime > drawCooldown)
        {
            if (Vector3.Distance(mouseWorldPos, lastDrawPosition) > minDrawDistance)
            {
                DrawPathSegment(lastDrawPosition, mouseWorldPos);
                lastDrawPosition = mouseWorldPos;
                lastDrawTime = Time.time;
            }
        }

        // ��������
        if (Input.GetMouseButtonUp(0))
        {
            ConfirmPath();
        }

        // ȡ������
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ClearPreview();
        }
    }
    void DrawPathSegment(Vector3 start, Vector3 end)
    {
        // ��Դ���Ƽ��
        if (previewCells.Count >= maxPathTiles - tilesDrawn)
        {
            Debug.LogWarning("·����Դ����!");
            return;
        }

        // ����·����
        float distance = Vector3.Distance(start, end);
        int segments = Mathf.CeilToInt(distance / (minDrawDistance * 0.5f));

        for (int i = 0; i <= segments; i++)
        {
            float t = i / (float)segments;
            Vector3 point = Vector3.Lerp(start, end, t);
            Vector3Int cellPos = drawableTilemap.WorldToCell(point);

            // ��ӵ�Ԥ���б�
            if (!previewCells.Contains(cellPos))
            {
                previewCells.Add(cellPos);

                // �ﵽ��Դ����
                if (previewCells.Count >= maxPathTiles - tilesDrawn)
                {
                    Debug.LogWarning("�Ѵﵽ���·������!");
                    break;
                }
            }
        }
    }
    void UpdatePreview()
    {
        // ��������Ⱦ��
        pathPreview.positionCount = previewCells.Count;
        for (int i = 0; i < previewCells.Count; i++)
        {
            pathPreview.SetPosition(i, drawableTilemap.GetCellCenterWorld(previewCells[i]));
        }
    }
    void ClearPreview()
    {
        previewCells.Clear();
        pathPreview.positionCount = 0;
    }
    void ConfirmPath()
    {
        if (previewCells.Count == 0) return;

        // ��Ԥ��·��ת��Ϊʵ����Ƭ
        foreach (Vector3Int cell in previewCells)
        {
            drawableTilemap.SetTile(cell, pathTile);
        }

        tilesDrawn += previewCells.Count;
        Debug.Log($"�ɹ�����·������{previewCells.Count}����Ƭ��ʣ����Դ: {maxPathTiles - tilesDrawn}");

        ClearPreview();

        ExitDrawingMode();
    }
    public void ResetPathResources()
    {
        tilesDrawn = 0;
        Debug.Log("·����Դ������");
    }

    // ���ӻ���������
    void OnDrawGizmosSelected()
    {
        if (triggerPoints == null) return;
        Gizmos.color = new Color(0.2f, 0.8f, 0.2f, 0.3f);
        foreach(Transform triggerPoint in triggerPoints) 
        {
            if (triggerPoint != null)
            {
                Gizmos.DrawWireSphere(triggerPoint.position, triggerRadius);
            }                   
        }
    }

}

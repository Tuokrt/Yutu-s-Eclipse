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
    [Header("瓦片地图设置")]
    public Tilemap drawableTilemap;
    public TileBase pathTile;

    [SerializeField] List<DrawZone> drawZones = new List<DrawZone>();
    private DrawZone currentZone;

    [Header("绘制设置")]
    public float minDrawDistance = 0.5f;
    public Color drawPreviewColor = new Color(0.2f, 0.8f, 0.7f);
    public float drawCooldown = 0.1f;

    public List<Transform> triggerPoints = new List<Transform>();
    public float triggerRadius = 2f;

    [Header("资源限制")]
    public int maxPathTiles = 30; // 最大可绘制瓦片数
    public int tilesDrawn = 0;    // 已绘制瓦片数

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

        // 保存玩家原始属性
        if (playerController != null)
        {
            originalPlayerSpeed = playerController.runSpeed;
            originalGravityScale = playerController.GetComponent<Rigidbody2D>().gravityScale;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // 检查玩家是否到达触发点
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

        // 绘制模式逻辑
        if (isDrawingMode)
        {
            HandleDrawingInput();
            UpdatePreview();

            // 检查玩家是否离开触发区域
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

        // 减缓玩家行动
        if (playerController != null)
        {
            playerController.runSpeed = originalPlayerSpeed * slowdownFactor;
            playerController.GetComponent<Rigidbody2D>().gravityScale = originalGravityScale * slowdownFactor;
        }

        pathPreview.enabled = true;
        Debug.Log($"进入绘制模式! 位置: {triggerPoint.position}, 剩余瓦片: {maxPathTiles - tilesDrawn}");
    }
    void ExitDrawingMode()
    {
        isDrawingMode = false;

        // 恢复玩家行动
        if (playerController != null)
        {
            playerController.runSpeed = originalPlayerSpeed;
            playerController.GetComponent<Rigidbody2D>().gravityScale = originalGravityScale;
        }

        pathPreview.enabled = false;
        ClearPreview();
        currentTriggerPoint = null;
        Debug.Log("退出绘制模式");
    }
    void HandleDrawingInput()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;

        // 开始绘制
        if (Input.GetMouseButtonDown(0))
        {
            lastDrawPosition = mouseWorldPos;
            lastDrawTime = Time.time;
        }

        // 持续绘制
        if (Input.GetMouseButton(0) && Time.time - lastDrawTime > drawCooldown)
        {
            if (Vector3.Distance(mouseWorldPos, lastDrawPosition) > minDrawDistance)
            {
                DrawPathSegment(lastDrawPosition, mouseWorldPos);
                lastDrawPosition = mouseWorldPos;
                lastDrawTime = Time.time;
            }
        }

        // 结束绘制
        if (Input.GetMouseButtonUp(0))
        {
            ConfirmPath();
        }

        // 取消绘制
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ClearPreview();
        }
    }
    void DrawPathSegment(Vector3 start, Vector3 end)
    {
        // 资源限制检查
        if (previewCells.Count >= maxPathTiles - tilesDrawn)
        {
            Debug.LogWarning("路径资源不足!");
            return;
        }

        // 计算路径点
        float distance = Vector3.Distance(start, end);
        int segments = Mathf.CeilToInt(distance / (minDrawDistance * 0.5f));

        for (int i = 0; i <= segments; i++)
        {
            float t = i / (float)segments;
            Vector3 point = Vector3.Lerp(start, end, t);
            Vector3Int cellPos = drawableTilemap.WorldToCell(point);

            // 添加到预览列表
            if (!previewCells.Contains(cellPos))
            {
                previewCells.Add(cellPos);

                // 达到资源上限
                if (previewCells.Count >= maxPathTiles - tilesDrawn)
                {
                    Debug.LogWarning("已达到最大路径长度!");
                    break;
                }
            }
        }
    }
    void UpdatePreview()
    {
        // 更新线渲染器
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

        // 将预览路径转换为实际瓦片
        foreach (Vector3Int cell in previewCells)
        {
            drawableTilemap.SetTile(cell, pathTile);
        }

        tilesDrawn += previewCells.Count;
        Debug.Log($"成功绘制路径，共{previewCells.Count}个瓦片，剩余资源: {maxPathTiles - tilesDrawn}");

        ClearPreview();

        ExitDrawingMode();
    }
    public void ResetPathResources()
    {
        tilesDrawn = 0;
        Debug.Log("路径资源已重置");
    }

    // 可视化触发区域
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

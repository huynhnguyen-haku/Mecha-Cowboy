using UnityEngine;
using UnityEngine.AI;

public class PathfindingIndicator : MonoBehaviour
{
    private Transform target; // Đích đến (bỏ SerializeField vì sẽ gán động)
    public LineRenderer lineRenderer; // Component LineRenderer để vẽ đường dẫn
    public float updateInterval = 0; // Khoảng thời gian cập nhật đường đi
    public float heightOffset = 0.5f; // Độ cao thêm vào để đường dẫn không chìm vào mặt đất
    public Color startColor = new Color(0, 0.8f, 1f, 1f); // Màu đầu (xanh dương nhạt)
    public Color endColor = new Color(0, 0.5f, 1f, 0.5f); // Màu cuối (xanh dương nhạt với alpha thấp)

    private NavMeshPath path; // Đường đi được tính toán
    private float timer; // Bộ đếm thời gian để cập nhật đường đi


    void Start()
    {
        // Khởi tạo NavMeshPath
        path = new NavMeshPath();

        // Thiết lập LineRenderer
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        }
        lineRenderer.startColor = startColor;
        lineRenderer.endColor = endColor;
        lineRenderer.startWidth = 0.15f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 0;
        lineRenderer.textureMode = LineTextureMode.Tile; // Đảm bảo texture lặp lại mượt mà
        lineRenderer.numCapVertices = 5; // Làm mượt hai đầu đường dẫn
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= updateInterval && target != null)
        {
            timer = 0f;
            UpdatePath();
        }
    }

    // Phương thức công khai để gán target động
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        if (target != null)
        {
            Debug.Log("PathfindingIndicator: Target set to " + target.name);
            UpdatePath(); // Cập nhật đường đi ngay lập tức khi gán target
        }
        else
        {
            Debug.LogWarning("PathfindingIndicator: Target is null!");
            lineRenderer.positionCount = 0; // Xóa đường dẫn nếu không có target
        }
    }

    void UpdatePath()
    {
        if (NavMesh.CalculatePath(transform.position, target.position, NavMesh.AllAreas, path))
        {
            Vector3[] smoothedPoints = SmoothPath(path.corners);
            lineRenderer.positionCount = smoothedPoints.Length;
            for (int i = 0; i < smoothedPoints.Length; i++)
            {
                lineRenderer.SetPosition(i, smoothedPoints[i] + Vector3.up * heightOffset);
            }
        }
        else
        {
            lineRenderer.positionCount = 0;
        }
    }

    Vector3[] SmoothPath(Vector3[] corners)
    {
        if (corners.Length < 2) return corners;

        Vector3[] smoothedPoints = new Vector3[corners.Length * 2 - 1];
        smoothedPoints[0] = corners[0];

        for (int i = 0; i < corners.Length - 1; i++)
        {
            Vector3 start = corners[i];
            Vector3 end = corners[i + 1];
            smoothedPoints[i * 2] = start;
            if (i < corners.Length - 1)
            {
                smoothedPoints[i * 2 + 1] = Vector3.Lerp(start, end, 0.5f); // Thêm điểm trung gian
            }
        }
        smoothedPoints[smoothedPoints.Length - 1] = corners[corners.Length - 1];

        return smoothedPoints;
    }
}
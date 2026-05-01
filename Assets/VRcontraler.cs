using UnityEngine;

public class VRLaser : MonoBehaviour
{
    public Transform leftRayOrigin;
    public Transform rightRayOrigin;
    public float maxRayDistance = 10f;
    public Color leftRayColor = Color.cyan;
    public Color rightRayColor = Color.yellow;

    private LineRenderer leftLine;
    private LineRenderer rightLine;

    void Start()
    {
        SetupLineRenderer(ref leftLine, "LeftRayLine", leftRayColor);
        SetupLineRenderer(ref rightLine, "RightRayLine", rightRayColor);
    }

    void Update()
    {
        UpdateVisibleRay(leftRayOrigin, leftLine);
        UpdateVisibleRay(rightRayOrigin, rightLine);

           /* 
        RaycastHit hit;

        if (Physics.Raycast(leftRayOrigin.position, leftRayOrigin.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, 5))
        {
            Debug.Log("Hit!");
            Debug.DrawRay(leftRayOrigin.position, leftRayOrigin.TransformDirection(Vector3.forward) * hit.distance, Color.blue);
        }

        */ 
    }

    void UpdateVisibleRay(Transform rayOrigin, LineRenderer line)
    {
        if (rayOrigin == null || line == null) return;

        Vector3 start = rayOrigin.position;
        Vector3 end = start + rayOrigin.forward * maxRayDistance;

        line.SetPosition(0, start);
        line.SetPosition(1, end);
    }

    void SetupLineRenderer(ref LineRenderer line, string objectName, Color color)
    {
        GameObject go = new GameObject(objectName);
        go.transform.SetParent(transform);
        line = go.AddComponent<LineRenderer>();
        line.material = new Material(Shader.Find("Sprites/Default"));
        // 调整激光的粗细，0.005f 是比较精致的线
        line.startWidth = 0.005f;
        line.endWidth = 0.005f;
        line.positionCount = 2;
        line.useWorldSpace = true;
        line.startColor = color;
        line.endColor = color;
        line.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        line.receiveShadows = false;
    }
}
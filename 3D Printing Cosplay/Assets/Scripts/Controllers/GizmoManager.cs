using UnityEngine;

public class GizmoManager : MonoBehaviour
{
    [Header("Gizmo Settings")]
    public Transform m_TargetObject; // The object being manipulated
    public float m_GizmoRadius = 1f; // Radius of the rotation arcs
    public float m_ArcThickness = 0.1f; // Thickness of the rotation arcs
    public int m_ArcSegments = 64; // Number of segments for smoothness
    public Color m_XAxisColor = Color.red; // X-axis arc color
    public Color m_YAxisColor = Color.green; // Y-axis arc color
    public Color m_ZAxisColor = Color.blue; // Z-axis arc color

    private Mesh m_XArcMesh, m_YArcMesh, m_ZArcMesh;
    private GameObject m_XArcObject, m_YArcObject, m_ZArcObject;
    private Camera m_MainCamera;

    private bool m_IsRotating = true;
    private Vector3 m_RotationAxis = Vector3.zero;

    public static GizmoManager s_Instance;
    
    private void Start()
    {

        s_Instance = this;
        
        // Get the main camera
        m_MainCamera = Camera.main;

        // Create meshes for each rotation axis
        m_XArcMesh = CreateArcMesh(Vector3.right);
        m_YArcMesh = CreateArcMesh(Vector3.up);
        m_ZArcMesh = CreateArcMesh(Vector3.forward);

        // Create and display the arcs (invisible GameObjects with meshes)
        m_XArcObject = CreateArcObject(m_XArcMesh, m_XAxisColor, Vector3.right);
        m_YArcObject = CreateArcObject(m_YArcMesh, m_YAxisColor, Vector3.up);
        m_ZArcObject = CreateArcObject(m_ZArcMesh, m_ZAxisColor, Vector3.forward);
    }

    public void InitializeGizmo(Transform target)
    {
        m_TargetObject = target;

        SetArcsAccordingToTarget();
    }

    private void SetArcsAccordingToTarget()
    {
        m_XArcObject.transform.SetParent(m_TargetObject);
        m_XArcObject.transform.localPosition = Vector3.zero;
        // m_XArcObject.transform.rotation = Quaternion.LookRotation(m_TargetObject.rotation.);
        
        m_YArcObject.transform.SetParent(m_TargetObject);
        m_YArcObject.transform.localPosition = Vector3.zero;

        m_ZArcObject.transform.SetParent(m_TargetObject);
        m_ZArcObject.transform.localPosition = Vector3.zero;

    }
    
    private void Update()
    {
        if (m_TargetObject == null) return;

        // Check for rotation interaction
        HandleRotationInteraction();
    }

    private void HandleRotationInteraction()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse button for rotation
        {
            Ray ray = m_MainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // Detect rotation based on which axis was hit
                if (hit.collider.CompareTag("RotationArc"))
                {
                    m_IsRotating = true;
                    m_RotationAxis = GetRotationAxis(hit.collider.gameObject.name);

                    // m_TargetObject.Rotate(rotationAxis, rotationAmount, Space.World);
                }
            }
        }


        if (Input.GetMouseButton(0) && m_IsRotating)
        {
            float rotationAmount = Input.GetAxis("Mouse X") * 10f;
            m_TargetObject.Rotate(m_RotationAxis, rotationAmount, Space.Self);
        }

        if (Input.GetMouseButtonUp(0) && m_IsRotating)
        {
            m_IsRotating = false;
        }
    }

    private Mesh CreateArcMesh(Vector3 axis)
    {
        Mesh mesh = new Mesh();
        int segments = m_ArcSegments;
        Vector3[] vertices = new Vector3[(segments + 1) * 2];
        int[] triangles = new int[segments * 6];

        float angleStep = 360f / segments;
        for (int i = 0; i <= segments; i++)
        {
            float angle = Mathf.Deg2Rad * i * angleStep;
            Vector3 innerVertex = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * m_GizmoRadius;
            Vector3 outerVertex = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * (m_GizmoRadius + m_ArcThickness);

            vertices[i * 2] = innerVertex;
            vertices[i * 2 + 1] = outerVertex;

            if (i < segments)
            {
                int startIndex = i * 2;
                triangles[i * 6] = startIndex;
                triangles[i * 6 + 1] = startIndex + 1;
                triangles[i * 6 + 2] = startIndex + 2;

                triangles[i * 6 + 3] = startIndex + 1;
                triangles[i * 6 + 4] = startIndex + 3;
                triangles[i * 6 + 5] = startIndex + 2;
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }

    private GameObject CreateArcObject(Mesh arcMesh, Color color, Vector3 rotationAxis)
    {
        var arcObject = new GameObject(GetAxisName(color))
        {
            tag = "RotationArc"
        };
        arcObject.transform.SetParent(m_TargetObject);
        arcObject.transform.localPosition = Vector3.zero;

        // Rotate the arc object to align with the correct axis
        arcObject.transform.rotation = Quaternion.LookRotation(rotationAxis);

        MeshRenderer meshRenderer = arcObject.AddComponent<MeshRenderer>();
        MeshFilter meshFilter = arcObject.AddComponent<MeshFilter>();

        meshFilter.mesh = arcMesh;

        // Set up the material for the arc
        Material arcMaterial = new Material(Shader.Find("Unlit/Color"));
        arcMaterial.color = color;
        meshRenderer.material = arcMaterial;

        // Add a MeshCollider for interaction, but without triggers
        MeshCollider meshCollider = arcObject.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = arcMesh;
        meshCollider.convex = true; // Set to convex to handle collisions properly

        return arcObject;
    }


    private string GetAxisName(Color color)
    {
        var axisName = "";
        
        if (color == Color.red)
            axisName = "X";
        else if (color == Color.green)
            axisName = "Y";
        else
            axisName = "Z";

        return axisName;
    }


    private Vector3 GetRotationAxis(string axis)
    {
        return axis switch
        {
            "X" => Vector3.forward * -1f,
            "Y" => Vector3.up,
            "Z" => Vector3.right,
            _ => Vector3.right
        };
    }
}

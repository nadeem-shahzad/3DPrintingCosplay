using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform m_Target; // The character to orbit around
    public Vector3 m_Offset = new Vector3(0, 2, -5); // Initial camera offset

    [Header("Orbit Settings")]
    public float m_OrbitSpeed = 100f; // Speed of the camera orbit
    public float m_MinVerticalAngle = -30f; // Minimum vertical angle
    public float m_MaxVerticalAngle = 60f; // Maximum vertical angle

    [Header("Zoom Settings")]
    public float m_ZoomSpeed = 5f; // Speed of zoom
    public float m_MinZoomDistance = 2f; // Minimum zoom distance
    public float m_MaxZoomDistance = 10f; // Maximum zoom distance

    private float m_CurrentDistance; // Current distance between camera and target
    private float m_CurrentYaw; // Current horizontal rotation
    private float m_CurrentPitch; // Current vertical rotation

    void Start()
    {
        if (m_Target == null)
        {
            Debug.LogError("Target is not assigned to CameraController!");
            enabled = false;
            return;
        }

        m_CurrentDistance = m_Offset.magnitude; // Initialize current distance
        Vector3 initialAngles = Quaternion.LookRotation(m_Offset).eulerAngles;
        m_CurrentYaw = initialAngles.y;
        m_CurrentPitch = initialAngles.x;
    }

    void Update()
    {
        // Zoom functionality
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        m_CurrentDistance -= scrollInput * m_ZoomSpeed;
        m_CurrentDistance = Mathf.Clamp(m_CurrentDistance, m_MinZoomDistance, m_MaxZoomDistance);

        // Orbit functionality (right mouse button held)
        if (Input.GetMouseButton(1)) // Right mouse button
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            m_CurrentYaw += mouseX * m_OrbitSpeed * Time.deltaTime;
            m_CurrentPitch -= mouseY * m_OrbitSpeed * Time.deltaTime;
            m_CurrentPitch = Mathf.Clamp(m_CurrentPitch, m_MinVerticalAngle, m_MaxVerticalAngle);
        }

        // Update camera position and rotation
        UpdateCameraPosition();
    }

    private void UpdateCameraPosition()
    {
        // Calculate new camera position
        Quaternion rotation = Quaternion.Euler(m_CurrentPitch, m_CurrentYaw, 0);
        Vector3 desiredPosition = m_Target.position + rotation * Vector3.back * m_CurrentDistance;

        // Apply position and look at target
        transform.position = desiredPosition;
        transform.LookAt(m_Target);
    }
}

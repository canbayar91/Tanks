using UnityEngine;

public class CameraControl : MonoBehaviour {

    public float m_DampTime = 0.2f;                 
    public float m_ScreenEdgeBuffer = 4f;           
    public float m_MinSize = 6.5f;
    [HideInInspector] public Transform[] m_Targets; 

    private Camera m_Camera;                        
    private float m_ZoomSpeed;                      
    private Vector3 m_MoveVelocity;                 
    private Vector3 m_DesiredPosition;              

    private void Awake() {
        m_Camera = GetComponentInChildren<Camera>();
    }

    private void FixedUpdate() {
        Move();
        Zoom();
    }

    private void Move() {

        // Find the average position of tanks
        FindAveragePosition();

        // Transition to the position
        transform.position = Vector3.SmoothDamp(transform.position, m_DesiredPosition, ref m_MoveVelocity, m_DampTime);
    }

    private void FindAveragePosition() {

        // Create a vector to store the position
        Vector3 averagePos = new Vector3();

        // Go through the tanks and add their positions
        int numTargets = 0;
        for (int i = 0; i < m_Targets.Length; i++)  {

            // Add to the average for active tanks
            if (m_Targets[i].gameObject.activeSelf)  {
                averagePos += m_Targets[i].position;
                numTargets++;
            }
        }

        // Get the average
        if (numTargets > 0) {
            averagePos /= numTargets;
        }

        // Update the desired position
        averagePos.y = transform.position.y;
        m_DesiredPosition = averagePos;
    }

    private void Zoom() {

        // Find the required size
        float requiredSize = FindRequiredSize();

        // Transition to the size
        m_Camera.orthographicSize = Mathf.SmoothDamp(m_Camera.orthographicSize, requiredSize, ref m_ZoomSpeed, m_DampTime);
    }

    private float FindRequiredSize() {

        // Find the position that the camera rig is moving towards
        Vector3 desiredLocalPos = transform.InverseTransformPoint(m_DesiredPosition);

        // Go through the tanks and update the maximum size
        float size = 0f;
        for (int i = 0; i < m_Targets.Length; i++) {

            // Update the size to the largest size for active objects
            if (m_Targets[i].gameObject.activeSelf) {
                Vector3 targetLocalPos = transform.InverseTransformPoint(m_Targets[i].position);
                Vector3 desiredPosToTarget = targetLocalPos - desiredLocalPos;
                size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.y));
                size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.x) / m_Camera.aspect);
            }
        }
        
        // Add the edge buffer to the size
        size += m_ScreenEdgeBuffer;
        size = Mathf.Max(size, m_MinSize);

        return size;
    }

    public void SetStartPositionAndSize() {

        // Find the desired position
        FindAveragePosition();

        // Set the camera position to the desired position
        transform.position = m_DesiredPosition;

        // Find and set the required size
        m_Camera.orthographicSize = FindRequiredSize();
    }
}
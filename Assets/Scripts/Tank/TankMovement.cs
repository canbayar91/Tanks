using UnityEngine;

public class TankMovement : MonoBehaviour {
	
    public int m_PlayerNumber = 1;         
    public float m_Speed = 12f;            
    public float m_TurnSpeed = 180f;       
    public AudioSource m_MovementAudio;    
    public AudioClip m_EngineIdling;       
    public AudioClip m_EngineDriving;      
    public float m_PitchRange = 0.2f;

    private string m_MovementAxisName;     
    private string m_TurnAxisName;         
    private Rigidbody m_Rigidbody;         
    private float m_MovementInputValue;    
    private float m_TurnInputValue;        
    private float m_OriginalPitch;         

    private void Awake() {
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    private void OnEnable() {
        // When the tank is turned on, make sure it's not kinematic
        m_Rigidbody.isKinematic = false;

        // Reset the input values
        m_MovementInputValue = 0f;
        m_TurnInputValue = 0f;
    }

    private void OnDisable() {

        // When the tank is turned off, set it to kinematic so it stops moving
        m_Rigidbody.isKinematic = true;
    }

    private void Start() {

        // The axes names are based on player number
        m_MovementAxisName = "Vertical" + m_PlayerNumber;
        m_TurnAxisName = "Horizontal" + m_PlayerNumber;

        // Store the original pitch of the audio source
        m_OriginalPitch = m_MovementAudio.pitch;
    }

    private void Update() {

        // Store the player's input
        m_MovementInputValue = Input.GetAxis(m_MovementAxisName);
        m_TurnInputValue = Input.GetAxis(m_TurnAxisName);

        // Make audio for the engine is playing
        EngineAudio();
    }

    private void EngineAudio() {

        // Check if the tank is moving
        if (Mathf.Abs(m_MovementInputValue) < 0.1f && Mathf.Abs(m_TurnInputValue) < 0.1f) {

            // Change to idle
            if (m_MovementAudio.clip == m_EngineDriving) {
                m_MovementAudio.clip = m_EngineIdling;
                m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                m_MovementAudio.Play();
            }

        } else {

            // Change to driving
            if (m_MovementAudio.clip == m_EngineIdling) {
                m_MovementAudio.clip = m_EngineDriving;
                m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                m_MovementAudio.Play();
            }
        }
    }

    private void FixedUpdate() {

        // Adjust the rigidbodies position and orientation
        Move();
        Turn();
    }

    private void Move() {

        // Create a movement vector in the direction of tank
        Vector3 movement = transform.forward * m_MovementInputValue * m_Speed * Time.deltaTime;

        // Apply the movement to the rigidbody
        m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
    }

    private void Turn() {

        // Determine the degrees for rotation
        float turn = m_TurnInputValue * m_TurnSpeed * Time.deltaTime;

        // Create the rotation quaternion
        Quaternion rotation = Quaternion.Euler(0f, turn, 0f);

        // Apply the rotation to the rigidbody
        m_Rigidbody.MoveRotation(m_Rigidbody.rotation * rotation);
    }
}
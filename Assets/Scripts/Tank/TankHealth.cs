using UnityEngine;
using UnityEngine.UI;

public class TankHealth : MonoBehaviour {

    public float m_StartingHealth = 100f;          
    public Slider m_Slider;                        
    public Image m_FillImage;                      
    public Color m_FullHealthColor = Color.green;  
    public Color m_ZeroHealthColor = Color.red;    
    public GameObject m_ExplosionPrefab;
    
    private AudioSource m_ExplosionAudio;          
    private ParticleSystem m_ExplosionParticles;   
    private float m_CurrentHealth;  
    private bool m_Dead;            

    private void Awake() {

        // Instantiate the explosion prefab and get a reference to the particle system
        m_ExplosionParticles = Instantiate(m_ExplosionPrefab).GetComponent<ParticleSystem>();

        // Get a reference to the audio source on the instantiated prefab
        m_ExplosionAudio = m_ExplosionParticles.GetComponent<AudioSource>();

        // Disable the prefab so it can be activated when it's required
        m_ExplosionParticles.gameObject.SetActive(false);
    }

    private void OnEnable() {

        // When the tank is enabled, reset the tank's health
        m_CurrentHealth = m_StartingHealth;
        m_Dead = false;

        // Update the health slider's value and color
        SetHealthUI();
    }

    public void TakeDamage(float amount) {

        // Reduce the health
        m_CurrentHealth -= amount;

        // Update UI elements
        SetHealthUI();

        // Call the death
        if (m_CurrentHealth <= 0f && !m_Dead) {
            OnDeath();
        }
    }

    private void SetHealthUI() {

        // Set the slider to the current health
        m_Slider.value = m_CurrentHealth;

        // Set the slider color
        m_FillImage.color = Color.Lerp(m_ZeroHealthColor, m_FullHealthColor, m_CurrentHealth / m_StartingHealth);
    }

    private void OnDeath() {

        // Play the particle system on the position of the tank
        m_ExplosionParticles.transform.position = transform.position;
        m_ExplosionParticles.gameObject.SetActive(true);
        m_ExplosionParticles.Play();
        m_ExplosionAudio.Play();

        // Destroy the tank and set the dead flag
        gameObject.SetActive(false);
        m_Dead = true;
    }
}
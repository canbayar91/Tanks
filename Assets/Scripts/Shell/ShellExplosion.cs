using UnityEngine;

public class ShellExplosion : MonoBehaviour {

    public LayerMask m_TankMask;
    public ParticleSystem m_ExplosionParticles;       
    public AudioSource m_ExplosionAudio;              
    public float m_MaxDamage = 100f;                  
    public float m_ExplosionForce = 1000f;            
    public float m_MaxLifeTime = 2f;                  
    public float m_ExplosionRadius = 5f;              

    private void Start() {
        Destroy(gameObject, m_MaxLifeTime);
    }

    private void OnTriggerEnter(Collider other)  {

        // Collect all colliders in the explosion radius that is in tank mask
        Collider[] colliders = Physics.OverlapSphere(transform.position, m_ExplosionRadius, m_TankMask);
        for (int i = 0; i < colliders.Length; i++) {

            // Damage every target tank
            Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody>();
            if (targetRigidbody) {

                // Apply explosion force
                targetRigidbody.AddExplosionForce(m_ExplosionForce, transform.position, m_ExplosionRadius);

                // Damage the target tank
                TankHealth targetHealth = targetRigidbody.GetComponent<TankHealth>();
                if (targetHealth) {
                    float damage = CalculateDamage(targetRigidbody.position);
                    targetHealth.TakeDamage(damage);
                }
            }
        }

        // Unparent the particles from the shell
        m_ExplosionParticles.transform.parent = null;

        // Play the explosion
        m_ExplosionParticles.Play();
        m_ExplosionAudio.Play();

        // Destroy the object that has the particles and the shell
        Destroy(m_ExplosionParticles.gameObject, m_ExplosionParticles.main.duration);
        Destroy(gameObject);
    }

    private float CalculateDamage(Vector3 targetPosition) {

        // Create a vector from the shell to the target
        Vector3 explosionToTarget = targetPosition - transform.position;

        // Calculate the proportion of the maximum distance
        float relativeDistance = (m_ExplosionRadius - explosionToTarget.magnitude) / m_ExplosionRadius;

        // Calculate and return the damage
        return Mathf.Max(0f, relativeDistance * m_MaxDamage);
    }
}
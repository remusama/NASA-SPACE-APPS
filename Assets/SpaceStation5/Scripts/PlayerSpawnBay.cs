using UnityEngine;

namespace SpaceStation5
{
    /// <summary>
    /// Interactive player spawn station / teleporter console inside Space Station 5.
    /// Provides holographic visualization, glowing spawn platform, status rings, and audio-visual teleportation cues.
    /// </summary>
    public class PlayerSpawnBay : MonoBehaviour
    {
        [Header("Spawn Settings")]
        public string sectorName = "Sector 01 - Primary Arrival Bay";
        public Transform spawnPoint;
        public Light beaconLight;
        public MeshRenderer holographicRing;
        public ParticleSystem arrivalParticles;

        [Header("Animation")]
        public float pulseSpeed = 2.0f;
        public float rotationSpeed = 30.0f;

        private Material ringMaterial;
        private Color baseEmissionColor;

        private void Start()
        {
            if (spawnPoint == null) spawnPoint = transform;

            if (holographicRing != null)
            {
                ringMaterial = Application.isPlaying ? holographicRing.material : holographicRing.sharedMaterial;
                if (ringMaterial != null && ringMaterial.HasProperty("_EmissionColor"))
                {
                    baseEmissionColor = ringMaterial.GetColor("_EmissionColor");
                }
            }
        }

        private void Update()
        {
            float dt = Time.deltaTime;

            // Rotate holographic ring
            if (holographicRing != null)
            {
                holographicRing.transform.Rotate(Vector3.up * (rotationSpeed * dt));

                // Pulsate hologram light
                if (ringMaterial != null && ringMaterial.HasProperty("_EmissionColor"))
                {
                    float glow = 0.7f + 0.3f * Mathf.Sin(Time.time * pulseSpeed);
                    ringMaterial.SetColor("_EmissionColor", baseEmissionColor * glow);
                }
            }

            // Pulse beacon light
            if (beaconLight != null)
            {
                beaconLight.intensity = 1.8f + 0.6f * Mathf.Sin(Time.time * pulseSpeed);
            }
        }

        public Vector3 GetSpawnPosition()
        {
            return spawnPoint != null ? spawnPoint.position : transform.position + Vector3.up * 0.1f;
        }

        public Quaternion GetSpawnRotation()
        {
            return spawnPoint != null ? spawnPoint.rotation : transform.rotation;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(0.2f, 0.9f, 1f, 0.7f);
            Vector3 pos = spawnPoint != null ? spawnPoint.position : transform.position;
            Gizmos.DrawWireSphere(pos + Vector3.up * 0.9f, 0.5f);
            Gizmos.DrawLine(pos, pos + Vector3.up * 1.8f);
            Gizmos.DrawRay(pos + Vector3.up * 1.6f, transform.forward * 1.2f);
        }
    }
}

using UnityEngine;

namespace SpaceStation5
{
    /// <summary>
    /// Master astrophysical controller for the supermassive Black Hole system.
    /// Simulates:
    /// 1. Relativistic Keplerian differential accretion disk rotation (inner rings orbit faster than outer rings).
    /// 2. Frame-dragging (Lense-Thirring effect) & subtle disk tilt precession.
    /// 3. Turbulent plasma energy pulsation & accretion shock luminescence.
    /// 4. Gravitational accretion lighting illuminating cosmic surroundings.
    /// Runs seamlessly in both Editor and Play modes.
    /// </summary>
    [ExecuteAlways]
    public class BlackHoleController : MonoBehaviour
    {
        [Header("Accretion Disk Hierarchy")]
        public Transform accretionDiskRoot;
        public Transform eventHorizon;

        [Header("Differential Keplerian Rotation")]
        [Tooltip("Master orbital speed multiplier.")]
        public float masterRotationSpeed = 1.0f;

        [Tooltip("Individual ring rotation speeds (deg/sec) from innermost (fastest) to outermost (slowest).")]
        public float[] ringSpeeds = new float[]
        {
            24.0f,  // accretion_disk (innermost, hottest zone)
            18.5f,  // accretion_disk.001
            13.0f,  // accretion_disk.002
            9.2f,   // accretion_disk.003
            6.5f,   // accretion_disk.004
            4.2f,   // accretion_disk.005 (outer boundary)
            -5.5f,  // accretion_disk_extra_01 (counter-streaming turbulent plasma)
            7.8f    // accretion_disk_extra_02 (outer shock filament)
        };

        [Header("Relativistic Frame-Dragging & Precession")]
        [Tooltip("Slow wobbling precession of the spinning Kerr black hole spin axis.")]
        public Vector3 precessionSpeed = new Vector3(0.012f, 0.025f, 0.008f);

        [Header("Plasma Energy & Luminescence Pulsation")]
        public bool enablePulsation = true;
        public float pulseFrequency = 0.65f;
        public float minIntensity = 0.85f;
        public float maxIntensity = 1.35f;

        [Header("Accretion Radiation Light")]
        public Light accretionLight;
        public float baseLightIntensity = 4.5f;

        private Transform[] ringTransforms = new Transform[8];
        private Material[] ringMaterials = new Material[8];
        private Material eventHorizonMaterial;

        private void OnEnable()
        {
            InitializeReferences();
        }

        private void Start()
        {
            InitializeReferences();
        }

        public void InitializeReferences()
        {
            // Auto-discover accretion root if not explicitly assigned
            if (accretionDiskRoot == null)
            {
                var ad = transform.Find("Accretion_disk_scale");
                if (ad != null) accretionDiskRoot = ad;
            }

            if (eventHorizon == null)
            {
                var eh = transform.Find("Event_Horizon");
                if (eh != null) eventHorizon = eh;
            }

            // Find all 8 accretion disk rings
            string[] ringNames = new string[]
            {
                "accretion_disk",
                "accretion_disk.001",
                "accretion_disk.002",
                "accretion_disk.003",
                "accretion_disk.004",
                "accretion_disk.005",
                "accretion_disk_extra_01",
                "accretion_disk_extra_02"
            };

            for (int i = 0; i < ringNames.Length; i++)
            {
                Transform t = null;
                if (accretionDiskRoot != null)
                {
                    t = accretionDiskRoot.Find(ringNames[i]);
                }
                if (t == null)
                {
                    t = transform.Find(ringNames[i]);
                }

                ringTransforms[i] = t;
                if (t != null)
                {
                    var mr = t.GetComponent<MeshRenderer>();
                    if (mr != null)
                    {
                        ringMaterials[i] = Application.isPlaying ? mr.material : mr.sharedMaterial;
                    }
                }
            }

            if (eventHorizon != null)
            {
                var mr = eventHorizon.GetComponent<MeshRenderer>();
                if (mr != null)
                {
                    eventHorizonMaterial = Application.isPlaying ? mr.material : mr.sharedMaterial;
                }
            }

            if (accretionLight == null)
            {
                accretionLight = GetComponentInChildren<Light>();
            }
        }

        private void Update()
        {
            float dt = Application.isPlaying ? Time.deltaTime : 0.016f;

            // 1. Keplerian Differential Rotation across concentric accretion rings
            for (int i = 0; i < ringTransforms.Length; i++)
            {
                if (ringTransforms[i] != null && i < ringSpeeds.Length)
                {
                    float speed = ringSpeeds[i] * masterRotationSpeed;
                    // Rotate around local disk normal (Z axis in the model's coordinate frame)
                    ringTransforms[i].Rotate(Vector3.forward, speed * dt, Space.Self);
                }
            }

            // 2. Relativistic Frame-Dragging & Precession
            if (accretionDiskRoot != null)
            {
                accretionDiskRoot.Rotate(precessionSpeed * dt, Space.World);
            }

            // 3. Turbulent Plasma Energy Wave Pulsation
            if (enablePulsation)
            {
                float t = Application.isPlaying ? Time.time : (float)UnityEditor.EditorApplication.timeSinceStartup;
                float wave = (Mathf.Sin(t * pulseFrequency * Mathf.PI * 2.0f) * 0.6f + 
                              Mathf.Cos(t * pulseFrequency * 1.7f * Mathf.PI) * 0.4f + 1.0f) * 0.5f;
                float intensity = Mathf.Lerp(minIntensity, maxIntensity, wave);

                if (accretionLight != null)
                {
                    accretionLight.intensity = baseLightIntensity * intensity;
                }
            }
        }
    }
}

using UnityEngine;

namespace SpaceStation5
{
    /// <summary>
    /// Animates the deep space sky dome and stellar background.
    /// Controls slow majestic celestial rotation, nebula drift, and twinkling star effects.
    /// Runs seamlessly in both Editor and Play modes.
    /// </summary>
    [ExecuteAlways]
    public class AnimatedSpaceEnvironment : MonoBehaviour
    {
        [Header("Celestial Rotation")]
        [Tooltip("Degrees per second around axes. Very slow for majestic orbital feel.")]
        public Vector3 rotationSpeed = new Vector3(0.015f, 0.04f, 0.008f);

        [Header("Material UV Drift")]
        [Tooltip("Speed of UV panning for cosmic dust and nebulae.")]
        public Vector2 uvDriftSpeed = new Vector2(0.0003f, 0.00015f);

        [Header("Nebula Glow Pulse")]
        public bool enablePulsing = true;
        public float pulseFrequency = 0.4f;
        public float minIntensity = 0.9f;
        public float maxIntensity = 1.25f;

        private Material skyMat;
        private Vector2 currentUvOffset;
        private Color baseColor = Color.white;

        private void Start()
        {
            InitMaterial();
        }

        private void OnEnable()
        {
            InitMaterial();
        }

        private void InitMaterial()
        {
            var renderer = GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                skyMat = Application.isPlaying ? renderer.material : renderer.sharedMaterial;
                if (skyMat != null && skyMat.HasProperty("_BaseColor"))
                {
                    baseColor = skyMat.GetColor("_BaseColor");
                }
            }
        }

        private void Update()
        {
            float dt = Application.isPlaying ? Time.deltaTime : 0.016f;

            // 1. Slow cosmic rotation
            transform.Rotate(rotationSpeed * dt, Space.World);

            // 2. Texture UV drift for deep space nebulae
            if (skyMat != null)
            {
                currentUvOffset += uvDriftSpeed * dt;
                if (skyMat.HasProperty("_BaseMap"))
                {
                    skyMat.SetTextureOffset("_BaseMap", currentUvOffset);
                }
                else if (skyMat.HasProperty("_MainTex"))
                {
                    skyMat.SetTextureOffset("_MainTex", currentUvOffset);
                }

                // 3. Subtle ambient star luminescence pulsation
                if (enablePulsing && skyMat.HasProperty("_BaseColor"))
                {
                    float t = Application.isPlaying ? Time.time : (float)UnityEditor.EditorApplication.timeSinceStartup;
                    float wave = (Mathf.Sin(t * pulseFrequency * Mathf.PI * 2f) + 1f) * 0.5f;
                    float intensity = Mathf.Lerp(minIntensity, maxIntensity, wave);
                    skyMat.SetColor("_BaseColor", baseColor * intensity);
                }
            }
        }
    }
}

using UnityEngine;

namespace SpaceStation5
{
    /// <summary>
    /// Master controller for dynamic celestial motion:
    /// 1. Station Centrifugal Spin (simulates artificial gravity & rotating stellar views) - UNTOUCHED.
    /// 2. Planetary Earth Axis & Cloud Layer Rotation.
    /// 3. Lunar Orbit & Tidal Spin (smooth orbital precession around Earth/Station).
    /// 4. Solar Orbit & Sun Source transit.
    /// 5. Supermassive Black Hole cosmic precession & distant gravitational drift.
    /// Runs seamlessly in both Editor and Play modes.
    /// </summary>
    [ExecuteAlways]
    public class CelestialOrbitController : MonoBehaviour
    {
        [Header("Station Centrifugal Rotation (Artificial Gravity)")]
        [Tooltip("Degrees per second of station spin around its spine axis (Y).")]
        public float stationSpinSpeed = 0.12f;
        public Transform stationTransform;

        [Header("Earth Planetary Dynamics")]
        [Tooltip("Degrees per second of Earth surface rotation.")]
        public float earthSpinSpeed = 0.02f;
        public Transform earthTransform;

        [Tooltip("Degrees per second of atmospheric cloud drift.")]
        public float cloudSpinSpeed = 0.032f;
        public Transform cloudTransform;

        [Header("Lunar Orbital Dynamics")]
        [Tooltip("Degrees per second of Moon's orbit around Earth.")]
        public float moonOrbitSpeed = 0.014f;
        [Tooltip("Tidally locked lunar axial rotation.")]
        public float moonSpinSpeed = 0.014f;
        public Transform moonSystemTransform;
        public Transform moonBodyTransform;

        [Header("Solar Orbit & Sun Source")]
        [Tooltip("Degrees per second of the solar orbit illumination vector.")]
        public float solarOrbitSpeed = 0.008f;
        public Transform sunLightTransform;
        public Transform sunVisualTransform;

        [Header("Black Hole Cosmic Precession")]
        [Tooltip("Slow majestic celestial drift across deep space.")]
        public float blackHoleDriftSpeed = 0.004f;
        public Transform blackHoleTransform;

        private void OnEnable()
        {
            FindReferences();
        }

        private void Start()
        {
            FindReferences();
        }

        public void FindReferences()
        {
            if (stationTransform == null)
            {
                var st = GameObject.Find("SpaceStation5_Root");
                if (st == null) st = GameObject.Find("SpaceStation5");
                if (st != null) stationTransform = st.transform;
            }

            if (earthTransform == null)
            {
                var ep = GameObject.Find("Surface");
                if (ep == null) ep = GameObject.Find("PlanetEarth_Body");
                if (ep != null) earthTransform = ep.transform;
            }

            if (cloudTransform == null)
            {
                var cl = GameObject.Find("Clouds");
                if (cl == null) cl = GameObject.Find("PlanetEarth_Clouds");
                if (cl != null) cloudTransform = cl.transform;
            }

            if (moonSystemTransform == null)
            {
                var ms = GameObject.Find("PlanetMoon_System");
                if (ms != null) moonSystemTransform = ms.transform;
            }

            if (moonBodyTransform == null)
            {
                var mb = GameObject.Find("PlanetMoon_Body");
                if (mb != null) moonBodyTransform = mb.transform;
            }

            if (sunLightTransform == null)
            {
                var dl = GameObject.Find("Directional Light");
                if (dl != null) sunLightTransform = dl.transform;
            }

            if (sunVisualTransform == null)
            {
                var sn = GameObject.Find("DistantSun_Radiant");
                if (sn != null) sunVisualTransform = sn.transform;
            }

            if (blackHoleTransform == null)
            {
                var bh = GameObject.Find("BlackHole_System");
                if (bh != null) blackHoleTransform = bh.transform;
            }
        }

        private void Update()
        {
            float dt = Application.isPlaying ? Time.deltaTime : 0.016f;

            // 1. Station Centrifugal Rotation (spins around its central spine axis)
            if (stationTransform != null && Mathf.Abs(stationSpinSpeed) > 0.0001f)
            {
                stationTransform.Rotate(Vector3.up, stationSpinSpeed * dt, Space.Self);
            }

            // 2. Earth Axial Rotation
            if (earthTransform != null)
            {
                earthTransform.Rotate(new Vector3(0.1f, 1.0f, 0.05f).normalized, earthSpinSpeed * dt, Space.Self);
            }

            // 3. Earth Cloud Drift (rotates slightly faster and skewed for realistic storm dynamics)
            if (cloudTransform != null)
            {
                cloudTransform.Rotate(new Vector3(0.08f, 1.0f, 0.08f).normalized, cloudSpinSpeed * dt, Space.Self);
            }

            // 4. Lunar Orbit & Tidal Rotation
            if (moonSystemTransform != null && Mathf.Abs(moonOrbitSpeed) > 0.0001f)
            {
                // Orbit around Earth/system origin
                moonSystemTransform.Rotate(new Vector3(0.15f, 1.0f, 0.1f).normalized, moonOrbitSpeed * dt, Space.World);
            }
            if (moonBodyTransform != null)
            {
                // Synchronous tidal spin
                moonBodyTransform.Rotate(Vector3.up, moonSpinSpeed * dt, Space.Self);
            }

            // 5. Solar Orbit Vector (drifts light direction smoothly)
            if (sunLightTransform != null && Mathf.Abs(solarOrbitSpeed) > 0.0001f)
            {
                sunLightTransform.Rotate(Vector3.up, solarOrbitSpeed * dt, Space.World);
            }

            // 6. Black Hole Cosmic Precession (slow, majestic motion)
            if (blackHoleTransform != null && Mathf.Abs(blackHoleDriftSpeed) > 0.0001f)
            {
                blackHoleTransform.Rotate(new Vector3(0.05f, 0.1f, 0.02f).normalized, blackHoleDriftSpeed * dt, Space.World);
            }
        }
    }
}

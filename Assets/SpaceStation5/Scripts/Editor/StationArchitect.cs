using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace SpaceStation5
{
    /// <summary>
    /// Master architectural builder for Space Station 5 with PBR textures and enhanced details.
    /// </summary>
    public class StationArchitect
    {
        public const float RingRadius = 500f;          // 1000m diameter
        public const float RingCorridorRadius = 490f;  // Walkable ring centerline
        public const float HubRadius = 100f;           // 200m diameter
        public const float HubInteriorRadius = 40f;    // 80m diameter command rotunda
        public const float SpineLength = 1200f;        // 1200m axial length
        public const float ArmWidth = 24f;             // Structural arm exterior width
        public const float CorridorWidth = 6f;         // Walkable corridor width
        public const float CorridorHeight = 4.2f;      // Walkable corridor ceiling height

        // Materials
        private Material matWhite;
        private Material matDarkTitanium;
        private Material matAluminium;
        private Material matGold;
        private Material matFloor;
        private Material matWall;
        private Material matCeiling;
        private Material matGlass;
        private Material matCyanLight;
        private Material matAmberLight;
        private Material matWarmWindow;
        private Material matRedAlert;
        private Material matTechScreen;
        private Material matSolarPanel;
        private Material matSkybox;
        private Material matEarth;
        private Material matEarthClouds;
        private Material matSun;
        private Material matWarmLounge;
        private Material matEmeraldFlora;
        private Material matCrewWarm;

        public void LoadMaterialsAndTextures()
        {
            Shader litShader = Shader.Find("Universal Render Pipeline/Lit");
            if (litShader == null) litShader = Shader.Find("Standard");

            // Textures
            Texture2D texHullAlbedo = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/SpaceStation5/Textures/T_Hull_Panels_Albedo.png");
            Texture2D texHullNormal = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/SpaceStation5/Textures/T_Hull_Panels_Normal.png");
            Texture2D texFloorAlbedo = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/SpaceStation5/Textures/T_Interior_Floor_Albedo.png");
            Texture2D texFloorNormal = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/SpaceStation5/Textures/T_Interior_Floor_Normal.png");
            Texture2D texWallAlbedo = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/SpaceStation5/Textures/T_Interior_Wall_Albedo.png");
            Texture2D texWallNormal = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/SpaceStation5/Textures/T_Interior_Wall_Normal.png");
            Texture2D texScreen = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/SpaceStation5/Textures/T_Tech_Screen_Emission.png");
            Texture2D texSolar = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/SpaceStation5/Textures/T_Solar_Panel_Albedo.png");
            Texture2D texStars = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/SpaceStation5/Textures/T_Deep_Space_Stars.png");

            // 1. M_Hull_White
            matWhite = GetOrCreateMaterial("Assets/SpaceStation5/Materials/M_Hull_White.mat", litShader);
            matWhite.SetColor("_BaseColor", new Color(0.95f, 0.95f, 0.97f));
            matWhite.SetFloat("_Metallic", 0.4f);
            matWhite.SetFloat("_Smoothness", 0.70f);
            if (texHullAlbedo != null)
            {
                matWhite.SetTexture("_BaseMap", texHullAlbedo);
                matWhite.SetTextureScale("_BaseMap", new Vector2(4f, 4f));
            }
            if (texHullNormal != null)
            {
                matWhite.SetTexture("_BumpMap", texHullNormal);
                matWhite.SetTextureScale("_BumpMap", new Vector2(4f, 4f));
                matWhite.EnableKeyword("_NORMALMAP");
                matWhite.SetFloat("_BumpScale", 1.2f);
            }

            // 2. M_Hull_DarkTitanium
            matDarkTitanium = GetOrCreateMaterial("Assets/SpaceStation5/Materials/M_Hull_DarkTitanium.mat", litShader);
            matDarkTitanium.SetColor("_BaseColor", new Color(0.18f, 0.20f, 0.23f));
            matDarkTitanium.SetFloat("_Metallic", 0.75f);
            matDarkTitanium.SetFloat("_Smoothness", 0.65f);
            if (texHullNormal != null)
            {
                matDarkTitanium.SetTexture("_BumpMap", texHullNormal);
                matDarkTitanium.SetTextureScale("_BumpMap", new Vector2(6f, 6f));
                matDarkTitanium.EnableKeyword("_NORMALMAP");
            }

            // 3. M_Hull_BrushedAluminium
            matAluminium = GetOrCreateMaterial("Assets/SpaceStation5/Materials/M_Hull_BrushedAluminium.mat", litShader);
            matAluminium.SetColor("_BaseColor", new Color(0.70f, 0.72f, 0.75f));
            matAluminium.SetFloat("_Metallic", 0.85f);
            matAluminium.SetFloat("_Smoothness", 0.55f);

            // 4. M_Hull_GoldTrim
            matGold = GetOrCreateMaterial("Assets/SpaceStation5/Materials/M_Hull_GoldTrim.mat", litShader);
            matGold.SetColor("_BaseColor", new Color(0.88f, 0.68f, 0.25f));
            matGold.SetFloat("_Metallic", 0.90f);
            matGold.SetFloat("_Smoothness", 0.80f);

            // 5. M_Interior_Floor
            matFloor = GetOrCreateMaterial("Assets/SpaceStation5/Materials/M_Interior_Floor.mat", litShader);
            matFloor.SetColor("_BaseColor", Color.white);
            matFloor.SetFloat("_Metallic", 0.2f);
            matFloor.SetFloat("_Smoothness", 0.35f);
            if (texFloorAlbedo != null)
            {
                matFloor.SetTexture("_BaseMap", texFloorAlbedo);
                matFloor.SetTextureScale("_BaseMap", new Vector2(2f, 6f));
            }
            if (texFloorNormal != null)
            {
                matFloor.SetTexture("_BumpMap", texFloorNormal);
                matFloor.SetTextureScale("_BumpMap", new Vector2(2f, 6f));
                matFloor.EnableKeyword("_NORMALMAP");
                matFloor.SetFloat("_BumpScale", 1.5f);
            }

            // 6. M_Interior_Wall
            matWall = GetOrCreateMaterial("Assets/SpaceStation5/Materials/M_Interior_Wall.mat", litShader);
            matWall.SetColor("_BaseColor", Color.white);
            matWall.SetFloat("_Metallic", 0.15f);
            matWall.SetFloat("_Smoothness", 0.50f);
            if (texWallAlbedo != null)
            {
                matWall.SetTexture("_BaseMap", texWallAlbedo);
                matWall.SetTextureScale("_BaseMap", new Vector2(1f, 1f));
            }
            if (texWallNormal != null)
            {
                matWall.SetTexture("_BumpMap", texWallNormal);
                matWall.SetTextureScale("_BumpMap", new Vector2(1f, 1f));
                matWall.EnableKeyword("_NORMALMAP");
            }

            // 7. M_Interior_Ceiling
            matCeiling = GetOrCreateMaterial("Assets/SpaceStation5/Materials/M_Interior_Ceiling.mat", litShader);
            matCeiling.SetColor("_BaseColor", new Color(0.22f, 0.24f, 0.28f));
            matCeiling.SetFloat("_Metallic", 0.5f);
            matCeiling.SetFloat("_Smoothness", 0.4f);

            // 8. M_Glass_Tinted
            matGlass = GetOrCreateMaterial("Assets/SpaceStation5/Materials/M_Glass_Tinted.mat", litShader);
            matGlass.SetColor("_BaseColor", new Color(0.1f, 0.3f, 0.45f, 0.35f));
            matGlass.SetFloat("_Metallic", 0.95f);
            matGlass.SetFloat("_Smoothness", 0.98f);
            if (matGlass.HasProperty("_Surface"))
            {
                matGlass.SetFloat("_Surface", 1);
                matGlass.SetFloat("_Blend", 0);
                matGlass.SetOverrideTag("RenderType", "Transparent");
                matGlass.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                matGlass.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                matGlass.SetInt("_ZWrite", 0);
                matGlass.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
            }

            // 9. Emissives
            matCyanLight = GetOrCreateEmissive("Assets/SpaceStation5/Materials/M_Emissive_Cyan.mat", litShader, new Color(0.1f, 0.85f, 1.0f), 3.0f);
            matAmberLight = GetOrCreateEmissive("Assets/SpaceStation5/Materials/M_Emissive_Amber.mat", litShader, new Color(1.0f, 0.6f, 0.1f), 3.0f);
            matWarmWindow = GetOrCreateEmissive("Assets/SpaceStation5/Materials/M_Emissive_WarmWindow.mat", litShader, new Color(1.0f, 0.85f, 0.55f), 2.5f);
            matRedAlert = GetOrCreateEmissive("Assets/SpaceStation5/Materials/M_Emissive_RedAlert.mat", litShader, new Color(1.0f, 0.15f, 0.1f), 4.0f);

            // 10. M_Tech_Screen
            matTechScreen = GetOrCreateMaterial("Assets/SpaceStation5/Materials/M_Tech_Screen.mat", litShader);
            matTechScreen.SetColor("_BaseColor", Color.white);
            if (texScreen != null)
            {
                matTechScreen.SetTexture("_BaseMap", texScreen);
                matTechScreen.SetTexture("_EmissionMap", texScreen);
                matTechScreen.SetColor("_EmissionColor", Color.white * 2.0f);
                matTechScreen.EnableKeyword("_EMISSION");
            }

            // 11. M_Solar_Panel
            matSolarPanel = GetOrCreateMaterial("Assets/SpaceStation5/Materials/M_Solar_Panel.mat", litShader);
            matSolarPanel.SetColor("_BaseColor", Color.white);
            matSolarPanel.SetFloat("_Metallic", 0.9f);
            matSolarPanel.SetFloat("_Smoothness", 0.9f);
            if (texSolar != null)
            {
                matSolarPanel.SetTexture("_BaseMap", texSolar);
                matSolarPanel.SetTextureScale("_BaseMap", new Vector2(4f, 4f));
            }

            // 12. M_Planet_Earth
            matEarth = GetOrCreateMaterial("Assets/SpaceStation5/Materials/M_Planet_Earth.mat", litShader);
            matEarth.SetColor("_BaseColor", Color.white);
            matEarth.SetFloat("_Metallic", 0.05f);
            matEarth.SetFloat("_Smoothness", 0.65f);
            Texture2D texEarth = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/SpaceStation5/Textures/T_Planet_Earth_Albedo.png");
            if (texEarth != null)
            {
                matEarth.SetTexture("_BaseMap", texEarth);
            }

            // 13. M_Planet_Earth_Clouds
            matEarthClouds = GetOrCreateMaterial("Assets/SpaceStation5/Materials/M_Planet_Earth_Clouds.mat", litShader);
            matEarthClouds.SetFloat("_Surface", 1);
            matEarthClouds.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            matEarthClouds.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            matEarthClouds.SetInt("_ZWrite", 0);
            matEarthClouds.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            matEarthClouds.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
            matEarthClouds.SetColor("_BaseColor", new Color(1f, 1f, 1f, 0.90f));
            Texture2D texClouds = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/SpaceStation5/Textures/T_Planet_Earth_Clouds.png");
            if (texClouds != null)
            {
                matEarthClouds.SetTexture("_BaseMap", texClouds);
            }

            // 14. M_Distant_Sun
            Shader unlitShader = Shader.Find("Universal Render Pipeline/Unlit") ?? Shader.Find("Unlit/Texture");
            matSun = GetOrCreateMaterial("Assets/SpaceStation5/Materials/M_Distant_Sun.mat", unlitShader);
            matSun.SetColor("_BaseColor", new Color(1.0f, 0.96f, 0.88f));
            Texture2D texSun = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/SpaceStation5/Textures/T_Sun_Corona.png");
            if (texSun != null)
            {
                matSun.SetTexture("_BaseMap", texSun);
            }

            // 15. Specialized Sector Illumination Materials
            matWarmLounge = GetOrCreateEmissive("Assets/SpaceStation5/Materials/M_Light_WarmLounge.mat", litShader, new Color(1.0f, 0.82f, 0.50f), 2.8f);
            matEmeraldFlora = GetOrCreateEmissive("Assets/SpaceStation5/Materials/M_Light_EmeraldFlora.mat", litShader, new Color(0.15f, 1.0f, 0.55f), 3.0f);
            matCrewWarm = GetOrCreateEmissive("Assets/SpaceStation5/Materials/M_Light_CrewWarm.mat", litShader, new Color(1.0f, 0.85f, 0.70f), 2.2f);

            EditorUtility.SetDirty(matWhite);
            EditorUtility.SetDirty(matFloor);
            EditorUtility.SetDirty(matWall);
            EditorUtility.SetDirty(matTechScreen);
            EditorUtility.SetDirty(matSolarPanel);
            EditorUtility.SetDirty(matEarth);
            EditorUtility.SetDirty(matEarthClouds);
            EditorUtility.SetDirty(matSun);
            AssetDatabase.SaveAssets();
        }

        private Material GetOrCreateMaterial(string path, Shader shader)
        {
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat == null)
            {
                mat = new Material(shader);
                AssetDatabase.CreateAsset(mat, path);
            }
            return mat;
        }

        private Material GetOrCreateEmissive(string path, Shader shader, Color col, float intensity)
        {
            Material mat = GetOrCreateMaterial(path, shader);
            Color finalEmissive = col * Mathf.LinearToGammaSpace(intensity);
            mat.SetColor("_BaseColor", col * 0.7f);
            mat.SetColor("_EmissionColor", finalEmissive);
            mat.EnableKeyword("_EMISSION");
            mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
            mat.SetFloat("_Metallic", 0.1f);
            mat.SetFloat("_Smoothness", 0.9f);
            return mat;
        }

        public GameObject BuildStation()
        {
            LoadMaterialsAndTextures();

            var existing = GameObject.Find("SpaceStation5");
            if (existing != null)
            {
                Object.DestroyImmediate(existing);
            }

            GameObject root = new GameObject("SpaceStation5");
            Undo.RegisterCreatedObjectUndo(root, "Build SpaceStation5");

            // Requirement 8 Organization
            GameObject exterior = CreateChild(root, "Exterior");
            GameObject interior = CreateChild(root, "Interior");
            GameObject navigation = CreateChild(root, "Navigation");
            GameObject lighting = CreateChild(root, "Lighting");
            GameObject environment = CreateChild(root, "Environment");
            GameObject gameplay = CreateChild(root, "Gameplay");

            // Exterior
            GameObject extRing = CreateChild(exterior, "MainRing");
            GameObject extHub = CreateChild(exterior, "CentralHub");
            GameObject extArms = CreateChild(exterior, "StructuralArms");
            GameObject extSpine = CreateChild(exterior, "CentralSpine");
            GameObject extDocking = CreateChild(exterior, "DockingBays");
            GameObject extEquip = CreateChild(exterior, "ExternalEquipment");

            // Interior
            GameObject intRing = CreateChild(interior, "RingCorridors");
            GameObject intRes = CreateChild(interior, "ResidentialSector");
            GameObject intCmd = CreateChild(interior, "CommandCenter");
            GameObject intSci = CreateChild(interior, "ScienceLabs");
            GameObject intEng = CreateChild(interior, "EngineeringSector");
            GameObject intLife = CreateChild(interior, "LifeSupport");
            GameObject intStorage = CreateChild(interior, "StorageSector");
            GameObject intObs = CreateChild(interior, "ObservationDecks");

            // Build Enhanced Architecture
            BuildExteriorRing(extRing);
            BuildExteriorHub(extHub);
            BuildExteriorArms(extArms);
            BuildExteriorSpine(extSpine);
            BuildExteriorDockingBays(extDocking);
            BuildOrbitalShuttle(extDocking);
            BuildExternalEquipment(extEquip);

            // Strip colliders from all exterior objects so they don't penetrate or push players out of interior corridors
            foreach (var col in exterior.GetComponentsInChildren<Collider>(true))
            {
                Object.DestroyImmediate(col);
            }

            // Walkable Interiors
            BuildInteriorRingCorridors(intRing);
            BuildInteriorCommandCenter(intCmd);
            BuildInteriorSpokeWalkways(intRing);
            BuildInteriorObservationDeck(intObs);
            BuildInteriorScienceLabs(intSci);
            BuildInteriorResidential(intRes);
            BuildInteriorEngineering(intEng);
            BuildStationConnectors(interior);

            // Lighting & Animated Skybox
            BuildStationLighting(lighting);
            BuildStationEnvironment(environment);

            // Navigation & Player (New Input System)
            BuildTransitPods(navigation);
            BuildPlayerSpawnBays(gameplay);
            BuildPlayerSetup(gameplay);

            return root;
        }

        private GameObject CreateChild(GameObject parent, string name)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent.transform, false);
            return go;
        }

        private GameObject CreateProceduralArch(GameObject parent, string name, Vector3 localPos, Quaternion localRot, float openWidth, float openHeight, float frameThickness, float depth, float chamfer, Material frameMat, Material lightMat, Color? lightColor = null, float lightIntensity = 1.4f)
        {
            GameObject arch = new GameObject(name);
            arch.transform.SetParent(parent.transform, false);
            arch.transform.localPosition = localPos;
            arch.transform.localRotation = localRot;

            // 1. Structural Beveled Frame
            GameObject frame = new GameObject("FrameStructure");
            frame.transform.SetParent(arch.transform, false);
            MeshFilter mf = frame.AddComponent<MeshFilter>();
            MeshRenderer mr = frame.AddComponent<MeshRenderer>();
            mf.sharedMesh = StationMeshBuilder.CreateBeveledArchFrameMesh(openWidth, openHeight, frameThickness, depth, chamfer);
            mr.sharedMaterial = frameMat;

            // 2. Embedded Architectural Light Channel
            if (lightMat != null)
            {
                GameObject strip = new GameObject("RecessedLightStrip");
                strip.transform.SetParent(arch.transform, false);
                MeshFilter smf = strip.AddComponent<MeshFilter>();
                MeshRenderer smr = strip.AddComponent<MeshRenderer>();
                smf.sharedMesh = StationMeshBuilder.CreateArchRecessedLightStripMesh(openWidth, openHeight, 0.12f, depth * 0.4f, chamfer);
                smr.sharedMaterial = lightMat;

                if (lightColor.HasValue)
                {
                    GameObject lightObj = new GameObject("ArchPointLight");
                    lightObj.transform.SetParent(arch.transform, false);
                    lightObj.transform.localPosition = new Vector3(0, openHeight - 0.2f, 0);

                    Light l = lightObj.AddComponent<Light>();
                    l.type = LightType.Point;
                    l.color = lightColor.Value;
                    l.range = 14f;
                    l.intensity = lightIntensity;
                }
            }

            return arch;
        }

        // ==========================================
        // EXTERIOR GENERATION WITH TEXTURES
        // ==========================================

        private void BuildExteriorRing(GameObject parent)
        {
            int segments = 36;
            float stepDeg = 360f / segments;

            for (int i = 0; i < segments; i++)
            {
                float angle = i * stepDeg;
                float rad = angle * Mathf.Deg2Rad;

                GameObject seg = new GameObject($"RingSegment_{i:D2}");
                seg.transform.SetParent(parent.transform, false);

                // Continuous Aerodynamic Curved Hull Section (seamlessly connecting segment to segment)
                MeshFilter mf = seg.AddComponent<MeshFilter>();
                MeshRenderer mr = seg.AddComponent<MeshRenderer>();
                mf.sharedMesh = StationMeshBuilder.CreateCurvedHullSectionMesh(RingRadius, angle, stepDeg, 16f, 15f, 3.5f, 8);
                mr.sharedMaterial = matWhite;

                // Tangent direction along ring circumference
                float midAng = (angle + stepDeg * 0.5f) * Mathf.Deg2Rad;
                Vector3 tangentDir = new Vector3(-Mathf.Sin(midAng), 0f, Mathf.Cos(midAng));
                Quaternion ringTangentRot = Quaternion.LookRotation(tangentDir, Vector3.up);

                // Mid-deck observation window arc (tangent along outer hull; skip where sector corridors open)
                if (i != 4 && i != 13 && i != 22 && i != 31)
                {
                    Vector3 winPos = new Vector3(Mathf.Cos(midAng) * (RingRadius + 16.1f), 0, Mathf.Sin(midAng) * (RingRadius + 16.1f));
                    GameObject win = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    win.name = "ObservationWindow";
                    win.transform.SetParent(seg.transform, false);
                    win.transform.position = winPos;
                    win.transform.rotation = ringTangentRot;
                    win.transform.localScale = new Vector3(0.3f, 3.2f, (2f * Mathf.PI * RingRadius / segments) * 0.70f);
                    win.GetComponent<MeshRenderer>().sharedMaterial = matWarmWindow;
                }

                // Solar heat radiator fins on alternating segments
                if (i % 2 == 0)
                {
                    Vector3 radPos = new Vector3(Mathf.Cos(midAng) * (RingRadius - 16.5f), 0, Mathf.Sin(midAng) * (RingRadius - 16.5f));
                    GameObject radFin = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    radFin.name = "RadiatorFin";
                    radFin.transform.SetParent(seg.transform, false);
                    radFin.transform.position = radPos;
                    radFin.transform.rotation = ringTangentRot;
                    radFin.transform.localScale = new Vector3(1.5f, 20f, (2f * Mathf.PI * RingRadius / segments) * 0.45f);
                    radFin.GetComponent<MeshRenderer>().sharedMaterial = matAluminium;
                }

                // Photovoltaic solar array wings on alternating segments (skip sector openings)
                if (i % 4 == 1 && i != 13)
                {
                    Vector3 solPos = new Vector3(Mathf.Cos(midAng) * (RingRadius + 24f), 0, Mathf.Sin(midAng) * (RingRadius + 24f));
                    GameObject solar = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    solar.name = "SolarPanelArray";
                    solar.transform.SetParent(seg.transform, false);
                    solar.transform.position = solPos;
                    solar.transform.rotation = ringTangentRot;
                    solar.transform.localScale = new Vector3(14f, 0.4f, (2f * Mathf.PI * RingRadius / segments) * 0.65f);
                    solar.GetComponent<MeshRenderer>().sharedMaterial = matSolarPanel;
                }
            }
        }

        private void BuildExteriorHub(GameObject parent)
        {
            GameObject mainHub = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            mainHub.name = "CoreHub_MainDeck";
            mainHub.transform.SetParent(parent.transform, false);
            mainHub.transform.localScale = new Vector3(HubRadius * 2f, 30f, HubRadius * 2f);
            mainHub.GetComponent<MeshRenderer>().sharedMaterial = matWhite;

            GameObject upperHub = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            upperHub.name = "CoreHub_UpperDeck";
            upperHub.transform.SetParent(parent.transform, false);
            upperHub.transform.localPosition = new Vector3(0, 35f, 0);
            upperHub.transform.localScale = new Vector3(HubRadius * 1.5f, 20f, HubRadius * 1.5f);
            upperHub.GetComponent<MeshRenderer>().sharedMaterial = matDarkTitanium;

            GameObject lowerHub = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            lowerHub.name = "CoreHub_LowerDeck";
            lowerHub.transform.SetParent(parent.transform, false);
            lowerHub.transform.localPosition = new Vector3(0, -35f, 0);
            lowerHub.transform.localScale = new Vector3(HubRadius * 1.5f, 20f, HubRadius * 1.5f);
            lowerHub.GetComponent<MeshRenderer>().sharedMaterial = matDarkTitanium;

            GameObject dome = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            dome.name = "CommandDome";
            dome.transform.SetParent(parent.transform, false);
            dome.transform.localPosition = new Vector3(0, 50f, 0);
            dome.transform.localScale = new Vector3(50f, 25f, 50f);
            dome.GetComponent<MeshRenderer>().sharedMaterial = matGlass;

            GameObject spire = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            spire.name = "HubAntennaSpire";
            spire.transform.SetParent(parent.transform, false);
            spire.transform.localPosition = new Vector3(0, 85f, 0);
            spire.transform.localScale = new Vector3(2f, 35f, 2f);
            spire.GetComponent<MeshRenderer>().sharedMaterial = matAluminium;

            GameObject hubGlowRing = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            hubGlowRing.name = "HubEquatorWindowBand";
            hubGlowRing.transform.SetParent(parent.transform, false);
            hubGlowRing.transform.localScale = new Vector3(HubRadius * 2.02f, 4f, HubRadius * 2.02f);
            hubGlowRing.GetComponent<MeshRenderer>().sharedMaterial = matWarmWindow;
        }

        private void BuildExteriorArms(GameObject parent)
        {
            float armLength = RingRadius - HubRadius;
            float midRadius = (RingRadius + HubRadius) * 0.5f;

            string[] armNames = { "NorthArm_000", "EastArm_090", "SouthArm_180", "WestArm_270" };
            float[] angles = { 0f, 90f, 180f, 270f };

            for (int i = 0; i < 4; i++)
            {
                float ang = angles[i];
                float rad = ang * Mathf.Deg2Rad;

                Vector3 pos = new Vector3(Mathf.Sin(rad) * midRadius, 0, Mathf.Cos(rad) * midRadius);
                Quaternion rot = Quaternion.Euler(0, ang, 0);

                GameObject arm = new GameObject(armNames[i]);
                arm.transform.SetParent(parent.transform, false);
                arm.transform.position = pos;
                arm.transform.rotation = rot;

                GameObject beam = GameObject.CreatePrimitive(PrimitiveType.Cube);
                beam.name = "ArmTrussBeam";
                beam.transform.SetParent(arm.transform, false);
                beam.transform.localScale = new Vector3(ArmWidth, 18f, armLength);
                beam.GetComponent<MeshRenderer>().sharedMaterial = matWhite;

                // Collars along arm
                for (int c = -3; c <= 3; c++)
                {
                    GameObject collar = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    collar.name = $"Collar_{c}";
                    collar.transform.SetParent(arm.transform, false);
                    collar.transform.localPosition = new Vector3(0, 0, c * 50f);
                    collar.transform.localScale = new Vector3(ArmWidth + 4f, 22f, 6f);
                    collar.GetComponent<MeshRenderer>().sharedMaterial = matGold;
                }

                // Coolant conduits
                GameObject pipe1 = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                pipe1.name = "CoolantPipe_Left";
                pipe1.transform.SetParent(arm.transform, false);
                pipe1.transform.localPosition = new Vector3(-ArmWidth * 0.55f, 0, 0);
                pipe1.transform.localRotation = Quaternion.Euler(90, 0, 0);
                pipe1.transform.localScale = new Vector3(3f, armLength * 0.5f, 3f);
                pipe1.GetComponent<MeshRenderer>().sharedMaterial = matDarkTitanium;

                GameObject pipe2 = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                pipe2.name = "CoolantPipe_Right";
                pipe2.transform.SetParent(arm.transform, false);
                pipe2.transform.localPosition = new Vector3(ArmWidth * 0.55f, 0, 0);
                pipe2.transform.localRotation = Quaternion.Euler(90, 0, 0);
                pipe2.transform.localScale = new Vector3(3f, armLength * 0.5f, 3f);
                pipe2.GetComponent<MeshRenderer>().sharedMaterial = matDarkTitanium;

                // Aerodynamic Gusset Fairing welding the Arm into the Ring
                GameObject fairing = new GameObject("RingJunctionFairing");
                fairing.transform.SetParent(arm.transform, false);
                fairing.transform.localPosition = new Vector3(0, 0, armLength * 0.5f - 40f);
                MeshFilter fmf = fairing.AddComponent<MeshFilter>();
                MeshRenderer fmr = fairing.AddComponent<MeshRenderer>();
                fmf.sharedMesh = StationMeshBuilder.CreateSpokeFairingGussetMesh(ArmWidth, ArmWidth * 2.0f, 20f, 40f);
                fmr.sharedMaterial = matWhite;

                // Structural Diagonal Triangular Bracing Trusses at the junction with the Ring
                GameObject trussL = GameObject.CreatePrimitive(PrimitiveType.Cube);
                trussL.name = "DiagonalTruss_L";
                trussL.transform.SetParent(arm.transform, false);
                trussL.transform.localPosition = new Vector3(-35f, 0, armLength * 0.42f);
                trussL.transform.localRotation = Quaternion.Euler(0, -35f, 0);
                trussL.transform.localScale = new Vector3(8f, 10f, 95f);
                trussL.GetComponent<MeshRenderer>().sharedMaterial = matWhite;

                GameObject trussR = GameObject.CreatePrimitive(PrimitiveType.Cube);
                trussR.name = "DiagonalTruss_R";
                trussR.transform.SetParent(arm.transform, false);
                trussR.transform.localPosition = new Vector3(35f, 0, armLength * 0.42f);
                trussR.transform.localRotation = Quaternion.Euler(0, 35f, 0);
                trussR.transform.localScale = new Vector3(8f, 10f, 95f);
                trussR.GetComponent<MeshRenderer>().sharedMaterial = matWhite;
            }
        }

        private void BuildExteriorSpine(GameObject parent)
        {
            GameObject spineCore = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            spineCore.name = "SpineCoreCylinder";
            spineCore.transform.SetParent(parent.transform, false);
            spineCore.transform.localScale = new Vector3(40f, SpineLength * 0.5f, 40f);
            spineCore.GetComponent<MeshRenderer>().sharedMaterial = matWhite;

            for (int y = -500; y <= 500; y += 200)
            {
                if (Mathf.Abs(y) < 100) continue;

                GameObject module = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                module.name = $"SpineServiceModule_{y}";
                module.transform.SetParent(parent.transform, false);
                module.transform.localPosition = new Vector3(0, y, 0);
                module.transform.localScale = new Vector3(55f, 25f, 55f);
                module.GetComponent<MeshRenderer>().sharedMaterial = matDarkTitanium;

                GameObject clamp = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                clamp.name = $"SpineClamp_{y}";
                clamp.transform.SetParent(parent.transform, false);
                clamp.transform.localPosition = new Vector3(0, y, 0);
                clamp.transform.localScale = new Vector3(60f, 4f, 60f);
                clamp.GetComponent<MeshRenderer>().sharedMaterial = matGold;
            }

            GameObject topAntenna = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            topAntenna.name = "TopSensorSpire";
            topAntenna.transform.SetParent(parent.transform, false);
            topAntenna.transform.localPosition = new Vector3(0, 620f, 0);
            topAntenna.transform.localScale = new Vector3(4f, 40f, 4f);
            topAntenna.GetComponent<MeshRenderer>().sharedMaterial = matAluminium;

            GameObject bottomCap = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            bottomCap.name = "BottomDeflectorCap";
            bottomCap.transform.SetParent(parent.transform, false);
            bottomCap.transform.localPosition = new Vector3(0, -600f, 0);
            bottomCap.transform.localScale = new Vector3(50f, 30f, 50f);
            bottomCap.GetComponent<MeshRenderer>().sharedMaterial = matDarkTitanium;
        }

        private void BuildExteriorDockingBays(GameObject parent)
        {
            GameObject dockHub = new GameObject("PrimaryDockingComplex_Top");
            dockHub.transform.SetParent(parent.transform, false);
            dockHub.transform.localPosition = new Vector3(0, 400f, 0);

            GameObject collar = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            collar.name = "DockingRing_Collar";
            collar.transform.SetParent(dockHub.transform, false);
            collar.transform.localScale = new Vector3(70f, 15f, 70f);
            collar.GetComponent<MeshRenderer>().sharedMaterial = matDarkTitanium;

            GameObject gateGuide = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            gateGuide.name = "MagneticGuideRing";
            gateGuide.transform.SetParent(dockHub.transform, false);
            gateGuide.transform.localPosition = new Vector3(0, 8f, 0);
            gateGuide.transform.localScale = new Vector3(60f, 1f, 60f);
            gateGuide.GetComponent<MeshRenderer>().sharedMaterial = matCyanLight;

            for (int i = 0; i < 4; i++)
            {
                float ang = i * 90f;
                GameObject clamp = GameObject.CreatePrimitive(PrimitiveType.Cube);
                clamp.name = $"DockingClamp_{i}";
                clamp.transform.SetParent(dockHub.transform, false);
                clamp.transform.localRotation = Quaternion.Euler(0, ang, 0);
                clamp.transform.localPosition = Quaternion.Euler(0, ang, 0) * new Vector3(40f, 5f, 0);
                clamp.transform.localScale = new Vector3(15f, 8f, 8f);
                clamp.GetComponent<MeshRenderer>().sharedMaterial = matGold;
            }
        }

        private void BuildOrbitalShuttle(GameObject parent)
        {
            GameObject shuttle = new GameObject("OrbitalShuttle_Orion");
            shuttle.transform.SetParent(parent.transform, false);
            shuttle.transform.localPosition = new Vector3(65f, 415f, 0);
            shuttle.transform.localRotation = Quaternion.Euler(0, 90f, 15f);

            GameObject fuselage = GameObject.CreatePrimitive(PrimitiveType.Cube);
            fuselage.name = "Fuselage";
            fuselage.transform.SetParent(shuttle.transform, false);
            fuselage.transform.localScale = new Vector3(7f, 6f, 24f);
            fuselage.GetComponent<MeshRenderer>().sharedMaterial = matWhite;

            GameObject nose = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            nose.name = "CockpitNose";
            nose.transform.SetParent(shuttle.transform, false);
            nose.transform.localPosition = new Vector3(0, 0, 14f);
            nose.transform.localRotation = Quaternion.Euler(90f, 0, 0);
            nose.transform.localScale = new Vector3(6f, 4f, 6f);
            nose.GetComponent<MeshRenderer>().sharedMaterial = matWhite;

            GameObject visor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            visor.name = "CockpitVisor";
            visor.transform.SetParent(shuttle.transform, false);
            visor.transform.localPosition = new Vector3(0, 2f, 10f);
            visor.transform.localScale = new Vector3(5.5f, 2f, 4f);
            visor.GetComponent<MeshRenderer>().sharedMaterial = matGlass;

            GameObject wingL = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wingL.name = "Wing_Left";
            wingL.transform.SetParent(shuttle.transform, false);
            wingL.transform.localPosition = new Vector3(-8f, -0.5f, -2f);
            wingL.transform.localRotation = Quaternion.Euler(0, -25f, 5f);
            wingL.transform.localScale = new Vector3(10f, 0.8f, 12f);
            wingL.GetComponent<MeshRenderer>().sharedMaterial = matWhite;

            GameObject wingR = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wingR.name = "Wing_Right";
            wingR.transform.SetParent(shuttle.transform, false);
            wingR.transform.localPosition = new Vector3(8f, -0.5f, -2f);
            wingR.transform.localRotation = Quaternion.Euler(0, 25f, -5f);
            wingR.transform.localScale = new Vector3(10f, 0.8f, 12f);
            wingR.GetComponent<MeshRenderer>().sharedMaterial = matWhite;

            GameObject fin = GameObject.CreatePrimitive(PrimitiveType.Cube);
            fin.name = "TailFin";
            fin.transform.SetParent(shuttle.transform, false);
            fin.transform.localPosition = new Vector3(0, 5.5f, -9f);
            fin.transform.localRotation = Quaternion.Euler(20f, 0, 0);
            fin.transform.localScale = new Vector3(0.8f, 7f, 6f);
            fin.GetComponent<MeshRenderer>().sharedMaterial = matWhite;

            GameObject thruster1 = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            thruster1.name = "EngineThruster_L";
            thruster1.transform.SetParent(shuttle.transform, false);
            thruster1.transform.localPosition = new Vector3(-2.2f, 0, -12.5f);
            thruster1.transform.localRotation = Quaternion.Euler(90f, 0, 0);
            thruster1.transform.localScale = new Vector3(2.4f, 2f, 2.4f);
            thruster1.GetComponent<MeshRenderer>().sharedMaterial = matCyanLight;

            GameObject thruster2 = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            thruster2.name = "EngineThruster_R";
            thruster2.transform.SetParent(shuttle.transform, false);
            thruster2.transform.localPosition = new Vector3(2.2f, 0, -12.5f);
            thruster2.transform.localRotation = Quaternion.Euler(90f, 0, 0);
            thruster2.transform.localScale = new Vector3(2.4f, 2f, 2.4f);
            thruster2.GetComponent<MeshRenderer>().sharedMaterial = matCyanLight;
        }

        private void BuildExternalEquipment(GameObject parent)
        {
            for (int i = 0; i < 4; i++)
            {
                float ang = i * 90f + 45f;
                float rad = ang * Mathf.Deg2Rad;
                Vector3 pos = new Vector3(Mathf.Sin(rad) * RingRadius, 18f, Mathf.Cos(rad) * RingRadius);

                GameObject beacon = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                beacon.name = $"NavigationBeacon_{i}";
                beacon.transform.SetParent(parent.transform, false);
                beacon.transform.position = pos;
                beacon.transform.localScale = new Vector3(2f, 2f, 2f);
                beacon.GetComponent<MeshRenderer>().sharedMaterial = matAmberLight;

                Light l = beacon.AddComponent<Light>();
                l.type = LightType.Point;
                l.color = new Color(1.0f, 0.6f, 0.1f);
                l.range = 50f;
                l.intensity = 2f;
            }
        }

        // ==========================================
        // WALKABLE INTERIOR GENERATION
        // ==========================================

        private void BuildInteriorRingCorridors(GameObject parent)
        {
            int count = 36;
            float step = 360f / count;

            for (int i = 0; i < count; i++)
            {
                float startAng = i * step;
                float midAng = (startAng + step * 0.5f) * Mathf.Deg2Rad;

                GameObject seg = new GameObject($"InteriorRingSeg_{i:D2}");
                seg.transform.SetParent(parent.transform, false);

                MeshFilter mf = seg.AddComponent<MeshFilter>();
                MeshRenderer mr = seg.AddComponent<MeshRenderer>();
                MeshCollider mc = seg.AddComponent<MeshCollider>();

                // Open inner wall on segments intersecting the 4 radial spokes (at 0, 90, 180, 270 degrees)
                // (segments 0, 35, 8, 9, 17, 18, 26, 27)
                bool opensToSpoke = (i == 0 || i == 35 || i == 8 || i == 9 || i == 17 || i == 18 || i == 26 || i == 27);

                // Open outer wall on segments intersecting the 4 specialized sectors (at 45, 135, 225, 315 degrees)
                // (segments 4, 13, 22, 31)
                bool opensToSector = (i == 4 || i == 13 || i == 22 || i == 31);

                Mesh mesh = StationMeshBuilder.CreateCurvedCorridorMesh(RingCorridorRadius, startAng, step, CorridorWidth, CorridorHeight, 8, opensToSpoke, opensToSector);
                mf.sharedMesh = mesh;
                mc.sharedMesh = mesh;
                mr.sharedMaterial = matWall;

                Vector3 midPos = new Vector3(Mathf.Cos(midAng) * RingCorridorRadius, 0.05f, Mathf.Sin(midAng) * RingCorridorRadius);
                Vector3 tangentDir = new Vector3(-Mathf.Sin(midAng), 0f, Mathf.Cos(midAng));
                Vector3 radialDir = new Vector3(Mathf.Cos(midAng), 0f, Mathf.Sin(midAng));
                Quaternion ringTangentRot = Quaternion.LookRotation(tangentDir, Vector3.up);

                // Detailed Floor Plate (1.0m thick base along tangent to prevent physics tunneling)
                GameObject floorPlate = GameObject.CreatePrimitive(PrimitiveType.Cube);
                floorPlate.name = "TreadFloorPlate";
                floorPlate.transform.SetParent(seg.transform, false);
                floorPlate.transform.position = new Vector3(midPos.x, -0.45f, midPos.z);
                floorPlate.transform.rotation = ringTangentRot;
                floorPlate.transform.localScale = new Vector3(CorridorWidth * 0.96f, 1.0f, (2f * Mathf.PI * RingCorridorRadius / count) * 1.02f);
                floorPlate.GetComponent<MeshRenderer>().sharedMaterial = matFloor;

                // Ceiling Strip Light along tangent
                GameObject lightStrip = GameObject.CreatePrimitive(PrimitiveType.Cube);
                lightStrip.name = "CeilingStripLight";
                lightStrip.transform.SetParent(seg.transform, false);
                lightStrip.transform.position = new Vector3(midPos.x, CorridorHeight - 0.1f, midPos.z);
                lightStrip.transform.rotation = ringTangentRot;
                lightStrip.transform.localScale = new Vector3(CorridorWidth * 0.7f, 0.1f, 1.8f);
                lightStrip.GetComponent<MeshRenderer>().sharedMaterial = matCyanLight;
                Object.DestroyImmediate(lightStrip.GetComponent<BoxCollider>());

                Light pLight = lightStrip.AddComponent<Light>();
                pLight.type = LightType.Point;
                pLight.color = new Color(0.88f, 0.96f, 1.0f);
                pLight.range = 16f;
                pLight.intensity = 1.4f;

                // Unified procedural beveled bulkhead rib arch (spans across the corridor)
                CreateProceduralArch(seg, "BulkheadRibArch", new Vector3(midPos.x, 0, midPos.z),
                    ringTangentRot,
                    CorridorWidth * 0.96f, CorridorHeight, 0.35f, 0.40f, 0.60f,
                    matDarkTitanium, (i % 2 == 0) ? matCyanLight : null, null, 0f);

                // Space Viewports on outer wall (ONLY on non-spoke and non-sector segments, thin glass along the outer wall)
                if (!opensToSpoke && !opensToSector && i % 3 == 0)
                {
                    Vector3 winPos = midPos + radialDir * (CorridorWidth * 0.49f) + Vector3.up * 1.8f;

                    GameObject windowFrame = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    windowFrame.name = "SpaceViewportGlass";
                    windowFrame.transform.SetParent(seg.transform, false);
                    windowFrame.transform.position = winPos;
                    windowFrame.transform.rotation = ringTangentRot;
                    windowFrame.transform.localScale = new Vector3(0.12f, 1.6f, 5.0f);
                    windowFrame.GetComponent<MeshRenderer>().sharedMaterial = matGlass;
                }
            }
        }

        private void BuildInteriorCommandCenter(GameObject parent)
        {
            GameObject hubRoom = new GameObject("CommandOperationsRotunda");
            hubRoom.transform.SetParent(parent.transform, false);

            MeshFilter mf = hubRoom.AddComponent<MeshFilter>();
            MeshRenderer mr = hubRoom.AddComponent<MeshRenderer>();
            MeshCollider mc = hubRoom.AddComponent<MeshCollider>();

            // Open north wall doorway (width CorridorWidth, height CorridorHeight) so player can walk into SpokeCorridor_North
            Mesh roomMesh = StationMeshBuilder.CreateRoomMeshWithDoorway(70f, 70f, 7f, CorridorWidth * 1.2f, CorridorHeight, StationMeshBuilder.DoorwayWall.North);
            mf.sharedMesh = roomMesh;
            mc.sharedMesh = roomMesh;
            mr.sharedMaterial = matWall;

            // Floor with textured non-slip plating (1.0m thick solid base)
            GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            floor.name = "RotundaFloorPlate";
            floor.transform.SetParent(hubRoom.transform, false);
            floor.transform.localPosition = new Vector3(0, -0.45f, 0);
            floor.transform.localScale = new Vector3(68f, 1.0f, 68f);
            floor.GetComponent<MeshRenderer>().sharedMaterial = matFloor;

            // Central Holotable
            GameObject holoBase = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            holoBase.name = "CentralHolotable";
            holoBase.transform.SetParent(hubRoom.transform, false);
            holoBase.transform.localPosition = new Vector3(0, 0.5f, 0);
            holoBase.transform.localScale = new Vector3(10f, 1f, 10f);
            holoBase.GetComponent<MeshRenderer>().sharedMaterial = matDarkTitanium;

            // Concentric Holo Rings
            GameObject holoRing1 = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            holoRing1.name = "HoloRing1";
            holoRing1.transform.SetParent(holoBase.transform, false);
            holoRing1.transform.localPosition = new Vector3(0, 0.6f, 0);
            holoRing1.transform.localScale = new Vector3(8f, 0.05f, 8f);
            holoRing1.GetComponent<MeshRenderer>().sharedMaterial = matCyanLight;
            Object.DestroyImmediate(holoRing1.GetComponent<CapsuleCollider>());

            GameObject holoSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            holoSphere.name = "StationHologramProjection";
            holoSphere.transform.SetParent(holoBase.transform, false);
            holoSphere.transform.localPosition = new Vector3(0, 2.5f, 0);
            holoSphere.transform.localScale = new Vector3(6f, 6f, 6f);
            holoSphere.GetComponent<MeshRenderer>().sharedMaterial = matCyanLight;
            Object.DestroyImmediate(holoSphere.GetComponent<SphereCollider>());

            // Tactical Workstation Desks with emissive radar screens
            for (int i = 0; i < 8; i++)
            {
                float a = i * 45f * Mathf.Deg2Rad;
                Vector3 deskPos = new Vector3(Mathf.Sin(a) * 22f, 0.6f, Mathf.Cos(a) * 22f);

                GameObject desk = GameObject.CreatePrimitive(PrimitiveType.Cube);
                desk.name = $"Workstation_{i}";
                desk.transform.SetParent(hubRoom.transform, false);
                desk.transform.localPosition = deskPos;
                desk.transform.localRotation = Quaternion.Euler(0, i * 45f + 180f, 0);
                desk.transform.localScale = new Vector3(4f, 1.2f, 1.8f);
                desk.GetComponent<MeshRenderer>().sharedMaterial = matDarkTitanium;

                // Monitor screen with tactical HUD texture
                GameObject screen = GameObject.CreatePrimitive(PrimitiveType.Cube);
                screen.name = "ScreenDisplay";
                screen.transform.SetParent(desk.transform, false);
                screen.transform.localPosition = new Vector3(0, 0.9f, 0.4f);
                screen.transform.localRotation = Quaternion.Euler(-15f, 0, 0);
                screen.transform.localScale = new Vector3(3.2f, 0.8f, 0.1f);
                screen.GetComponent<MeshRenderer>().sharedMaterial = matTechScreen;
                Object.DestroyImmediate(screen.GetComponent<BoxCollider>());
            }

            // Command Center ceiling dome light
            GameObject domeLight = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            domeLight.name = "RotundaDomeLight";
            domeLight.transform.SetParent(hubRoom.transform, false);
            domeLight.transform.localPosition = new Vector3(0, 6.9f, 0);
            domeLight.transform.localScale = new Vector3(25f, 0.2f, 25f);
            domeLight.GetComponent<MeshRenderer>().sharedMaterial = matCyanLight;
            Object.DestroyImmediate(domeLight.GetComponent<CapsuleCollider>());

            Light l = domeLight.AddComponent<Light>();
            l.type = LightType.Point;
            l.color = new Color(0.9f, 0.95f, 1.0f);
            l.range = 50f;
            l.intensity = 3.0f;
        }

        private void BuildInteriorSpokeWalkways(GameObject parent)
        {
            float spokeLen = RingCorridorRadius - 35f;

            string[] names = { "SpokeCorridor_North", "SpokeCorridor_East", "SpokeCorridor_South", "SpokeCorridor_West" };
            float[] angles = { 0f, 90f, 180f, 270f };

            for (int i = 0; i < 4; i++)
            {
                float ang = angles[i];
                GameObject spoke = new GameObject(names[i]);
                spoke.transform.SetParent(parent.transform, false);
                spoke.transform.rotation = Quaternion.Euler(0, ang, 0);
                spoke.transform.position = spoke.transform.rotation * new Vector3(0, 0, 35f);

                MeshFilter mf = spoke.AddComponent<MeshFilter>();
                MeshRenderer mr = spoke.AddComponent<MeshRenderer>();
                MeshCollider mc = spoke.AddComponent<MeshCollider>();

                Mesh straightMesh = StationMeshBuilder.CreateStraightCorridorMesh(spokeLen, CorridorWidth, CorridorHeight);
                mf.sharedMesh = straightMesh;
                mc.sharedMesh = straightMesh;
                mr.sharedMaterial = matWall;

                // Textured floor (1.0m thick solid base)
                GameObject floorPlate = GameObject.CreatePrimitive(PrimitiveType.Cube);
                floorPlate.name = "SpokeFloorPlate";
                floorPlate.transform.SetParent(spoke.transform, false);
                floorPlate.transform.localPosition = new Vector3(0, -0.45f, spokeLen * 0.5f);
                floorPlate.transform.localScale = new Vector3(CorridorWidth * 0.95f, 1.0f, spokeLen);
                floorPlate.GetComponent<MeshRenderer>().sharedMaterial = matFloor;

                // Continuous floor edge runway strips (left & right)
                GameObject runL = GameObject.CreatePrimitive(PrimitiveType.Cube);
                runL.name = "RunwayStrip_L";
                runL.transform.SetParent(spoke.transform, false);
                runL.transform.localPosition = new Vector3(-CorridorWidth * 0.46f, 0.06f, spokeLen * 0.5f);
                runL.transform.localScale = new Vector3(0.12f, 0.02f, spokeLen);
                runL.GetComponent<MeshRenderer>().sharedMaterial = matCyanLight;
                Object.DestroyImmediate(runL.GetComponent<BoxCollider>());

                GameObject runR = GameObject.CreatePrimitive(PrimitiveType.Cube);
                runR.name = "RunwayStrip_R";
                runR.transform.SetParent(spoke.transform, false);
                runR.transform.localPosition = new Vector3(CorridorWidth * 0.46f, 0.06f, spokeLen * 0.5f);
                runR.transform.localScale = new Vector3(0.12f, 0.02f, spokeLen);
                runR.GetComponent<MeshRenderer>().sharedMaterial = matCyanLight;
                Object.DestroyImmediate(runR.GetComponent<BoxCollider>());

                // Rhythmic ceiling light fixtures and soft ambient point lights every 12m along spoke
                for (float z = 8f; z < spokeLen - 8f; z += 12f)
                {
                    GameObject cLight = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cLight.name = $"CeilingLight_{z}";
                    cLight.transform.SetParent(spoke.transform, false);
                    cLight.transform.localPosition = new Vector3(0, CorridorHeight - 0.1f, z);
                    cLight.transform.localScale = new Vector3(CorridorWidth * 0.70f, 0.08f, 1.8f);
                    cLight.GetComponent<MeshRenderer>().sharedMaterial = matCyanLight;
                    Object.DestroyImmediate(cLight.GetComponent<BoxCollider>());

                    Light pLight = cLight.AddComponent<Light>();
                    pLight.type = LightType.Point;
                    pLight.color = new Color(0.92f, 0.96f, 1.0f);
                    pLight.range = 22f;
                    pLight.intensity = 2.4f;
                }

                // Open architectural bulkhead collar arches every 75 meters (WALKABLE - NO CENTER BLOCKING WALL)
                for (float z = 75f; z < spokeLen - 30f; z += 75f)
                {
                    CreateProceduralArch(spoke, $"BulkheadArchFrame_{z}", new Vector3(0, 0, z),
                        Quaternion.identity,
                        CorridorWidth * 0.95f, CorridorHeight, 0.50f, 0.70f, 0.65f,
                        matGold, matAmberLight, new Color(1.0f, 0.75f, 0.35f), 1.6f);
                }
            }
        }

        private void BuildInteriorObservationDeck(GameObject parent)
        {
            float rad = 45f * Mathf.Deg2Rad;
            Vector3 radialDir = new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad));
            Quaternion rot = Quaternion.LookRotation(radialDir, Vector3.up);

            float rRoomStart = RingCorridorRadius + CorridorWidth * 0.5f + 14f; // 507m (where connector ends)
            float roomLen = 22f;
            float roomWidth = 36f;
            float roomHeight = 6.0f;
            float rCenter = rRoomStart + roomLen * 0.5f; // 518m

            GameObject obs = new GameObject("SectorDelta_ObservationPromenade");
            obs.transform.SetParent(parent.transform, false);
            obs.transform.position = radialDir * rCenter;
            obs.transform.rotation = rot;

            MeshFilter mf = obs.AddComponent<MeshFilter>();
            MeshRenderer mr = obs.AddComponent<MeshRenderer>();
            MeshCollider mc = obs.AddComponent<MeshCollider>();

            // Open South wall doorway (at -roomLen/2, meeting connector pathway seamlessly) and open North wall for panoramic space glass
            Mesh roomMesh = StationMeshBuilder.CreateRoomMeshWithDoorway(roomWidth, roomLen, roomHeight, CorridorWidth * 1.05f, CorridorHeight, StationMeshBuilder.DoorwayWall.South, true);
            mf.sharedMesh = roomMesh;
            mc.sharedMesh = roomMesh;
            mr.sharedMaterial = matWall;

            // Thick base floor plate
            GameObject floorPlate = GameObject.CreatePrimitive(PrimitiveType.Cube);
            floorPlate.name = "ObservationFloorPlate";
            floorPlate.transform.SetParent(obs.transform, false);
            floorPlate.transform.localPosition = new Vector3(0, -0.45f, 0);
            floorPlate.transform.localScale = new Vector3(roomWidth, 1.0f, roomLen);
            floorPlate.GetComponent<MeshRenderer>().sharedMaterial = matFloor;

            // Grand Panoramic Space Window on North wall (+Z) looking out directly at Earth and the Sun!
            GameObject glassWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            glassWall.name = "PanoramicStarWindow";
            glassWall.transform.SetParent(obs.transform, false);
            glassWall.transform.localPosition = new Vector3(0, roomHeight * 0.5f, roomLen * 0.5f - 0.1f);
            glassWall.transform.localScale = new Vector3(roomWidth * 0.94f, roomHeight * 0.85f, 0.25f);
            glassWall.GetComponent<MeshRenderer>().sharedMaterial = matGlass;

            // Architectural Window Mullions / Beveled Pillars across the grand glass view
            for (float x = -roomWidth * 0.35f; x <= roomWidth * 0.36f; x += roomWidth * 0.25f)
            {
                GameObject mullion = GameObject.CreatePrimitive(PrimitiveType.Cube);
                mullion.name = $"WindowMullion_{x}";
                mullion.transform.SetParent(obs.transform, false);
                mullion.transform.localPosition = new Vector3(x, roomHeight * 0.5f, roomLen * 0.5f - 0.05f);
                mullion.transform.localScale = new Vector3(0.5f, roomHeight, 0.4f);
                mullion.GetComponent<MeshRenderer>().sharedMaterial = matDarkTitanium;
            }

            // Luxury observation seating facing the Earth and Sun
            for (int x = -12; x <= 12; x += 8)
            {
                GameObject sofa = GameObject.CreatePrimitive(PrimitiveType.Cube);
                sofa.name = $"LoungeSeating_{x}";
                sofa.transform.SetParent(obs.transform, false);
                sofa.transform.localPosition = new Vector3(x, 0.45f, 4.5f);
                sofa.transform.localScale = new Vector3(5.5f, 0.9f, 2.2f);
                sofa.GetComponent<MeshRenderer>().sharedMaterial = matDarkTitanium;

                // Observation console table in front of seating
                GameObject table = GameObject.CreatePrimitive(PrimitiveType.Cube);
                table.name = $"LoungeTable_{x}";
                table.transform.SetParent(obs.transform, false);
                table.transform.localPosition = new Vector3(x, 0.55f, 7.2f);
                table.transform.localScale = new Vector3(4.5f, 0.7f, 1.2f);
                table.GetComponent<MeshRenderer>().sharedMaterial = matGold;

                // Holographic star chart on table
                GameObject holo = GameObject.CreatePrimitive(PrimitiveType.Cube);
                holo.name = $"HoloStarChart_{x}";
                holo.transform.SetParent(obs.transform, false);
                holo.transform.localPosition = new Vector3(x, 0.92f, 7.2f);
                holo.transform.localScale = new Vector3(3.0f, 0.04f, 0.8f);
                holo.GetComponent<MeshRenderer>().sharedMaterial = matWarmLounge;
                Object.DestroyImmediate(holo.GetComponent<BoxCollider>());
            }

            // Warm sunset golden ambient lounge lighting
            GameObject obsLight = new GameObject("ObsLoungeLight_Center");
            obsLight.transform.SetParent(obs.transform, false);
            obsLight.transform.localPosition = new Vector3(0, roomHeight - 0.4f, 0);
            Light l = obsLight.AddComponent<Light>();
            l.type = LightType.Point;
            l.color = new Color(1.0f, 0.88f, 0.68f);
            l.range = 38f;
            l.intensity = 2.4f;

            // Recessed ceiling lighting panel
            GameObject ceilPnl = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ceilPnl.name = "CeilingLoungeGlow";
            ceilPnl.transform.SetParent(obs.transform, false);
            ceilPnl.transform.localPosition = new Vector3(0, roomHeight - 0.05f, 0);
            ceilPnl.transform.localScale = new Vector3(roomWidth * 0.7f, 0.08f, roomLen * 0.6f);
            ceilPnl.GetComponent<MeshRenderer>().sharedMaterial = matWarmLounge;
            Object.DestroyImmediate(ceilPnl.GetComponent<BoxCollider>());
        }

        private void BuildInteriorScienceLabs(GameObject parent)
        {
            float rad = 135f * Mathf.Deg2Rad;
            Vector3 radialDir = new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad));
            Quaternion rot = Quaternion.LookRotation(radialDir, Vector3.up);

            float rRoomStart = RingCorridorRadius + CorridorWidth * 0.5f + 14f; // 507m
            float roomLen = 20f;
            float roomWidth = 32f;
            float roomHeight = 5.2f;
            float rCenter = rRoomStart + roomLen * 0.5f; // 517m

            GameObject lab = new GameObject("SectorBeta_HydroponicsLab");
            lab.transform.SetParent(parent.transform, false);
            lab.transform.position = radialDir * rCenter;
            lab.transform.rotation = rot;

            MeshFilter mf = lab.AddComponent<MeshFilter>();
            MeshRenderer mr = lab.AddComponent<MeshRenderer>();
            MeshCollider mc = lab.AddComponent<MeshCollider>();

            // Open South wall doorway
            Mesh roomMesh = StationMeshBuilder.CreateRoomMeshWithDoorway(roomWidth, roomLen, roomHeight, CorridorWidth * 1.05f, CorridorHeight, StationMeshBuilder.DoorwayWall.South);
            mf.sharedMesh = roomMesh;
            mc.sharedMesh = roomMesh;
            mr.sharedMaterial = matWall;

            // Thick base floor plate
            GameObject floorPlate = GameObject.CreatePrimitive(PrimitiveType.Cube);
            floorPlate.name = "LabFloorPlate";
            floorPlate.transform.SetParent(lab.transform, false);
            floorPlate.transform.localPosition = new Vector3(0, -0.45f, 0);
            floorPlate.transform.localScale = new Vector3(roomWidth, 1.0f, roomLen);
            floorPlate.GetComponent<MeshRenderer>().sharedMaterial = matFloor;

            // Hydroponic bio-luminescent flora cylindrical tanks with emerald glow
            for (int i = -1; i <= 1; i++)
            {
                GameObject tank = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                tank.name = $"HydroponicBioTank_{i}";
                tank.transform.SetParent(lab.transform, false);
                tank.transform.localPosition = new Vector3(i * 8f, 2.0f, 4f);
                tank.transform.localScale = new Vector3(3.5f, 2.0f, 3.5f);
                tank.GetComponent<MeshRenderer>().sharedMaterial = matEmeraldFlora;

                Light tl = tank.AddComponent<Light>();
                tl.type = LightType.Point;
                tl.color = new Color(0.15f, 1.0f, 0.55f);
                tl.range = 14f;
                tl.intensity = 1.8f;
            }

            // Science terminal workbenches
            for (int i = -1; i <= 1; i += 2)
            {
                GameObject bench = GameObject.CreatePrimitive(PrimitiveType.Cube);
                bench.name = $"AnalysisBench_{i}";
                bench.transform.SetParent(lab.transform, false);
                bench.transform.localPosition = new Vector3(i * 12f, 0.6f, -2f);
                bench.transform.localScale = new Vector3(2.5f, 1.2f, 8f);
                bench.GetComponent<MeshRenderer>().sharedMaterial = matDarkTitanium;

                GameObject scrn = GameObject.CreatePrimitive(PrimitiveType.Cube);
                scrn.name = $"HoloMonitor_{i}";
                scrn.transform.SetParent(lab.transform, false);
                scrn.transform.localPosition = new Vector3(i * 12f, 1.6f, -2f);
                scrn.transform.localScale = new Vector3(0.1f, 0.8f, 2.4f);
                scrn.GetComponent<MeshRenderer>().sharedMaterial = matTechScreen;
                Object.DestroyImmediate(scrn.GetComponent<BoxCollider>());
            }

            // Laboratory overhead grow lights
            GameObject light = new GameObject("LabLight_Main");
            light.transform.SetParent(lab.transform, false);
            light.transform.localPosition = new Vector3(0, roomHeight - 0.3f, 0);
            Light l = light.AddComponent<Light>();
            l.type = LightType.Point;
            l.color = new Color(0.45f, 1.0f, 0.75f);
            l.range = 32f;
            l.intensity = 2.2f;
        }

        private void BuildInteriorResidential(GameObject parent)
        {
            float rad = 225f * Mathf.Deg2Rad;
            Vector3 radialDir = new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad));
            Quaternion rot = Quaternion.LookRotation(radialDir, Vector3.up);

            float rRoomStart = RingCorridorRadius + CorridorWidth * 0.5f + 14f; // 507m
            float roomLen = 18f;
            float roomWidth = 28f;
            float roomHeight = 4.8f;
            float rCenter = rRoomStart + roomLen * 0.5f; // 516m

            GameObject res = new GameObject("SectorAlpha_CrewLivingQuarters");
            res.transform.SetParent(parent.transform, false);
            res.transform.position = radialDir * rCenter;
            res.transform.rotation = rot;

            MeshFilter mf = res.AddComponent<MeshFilter>();
            MeshRenderer mr = res.AddComponent<MeshRenderer>();
            MeshCollider mc = res.AddComponent<MeshCollider>();

            // Open South wall doorway
            Mesh roomMesh = StationMeshBuilder.CreateRoomMeshWithDoorway(roomWidth, roomLen, roomHeight, CorridorWidth * 1.05f, CorridorHeight, StationMeshBuilder.DoorwayWall.South);
            mf.sharedMesh = roomMesh;
            mc.sharedMesh = roomMesh;
            mr.sharedMaterial = matWall;

            // Thick base floor plate
            GameObject floorPlate = GameObject.CreatePrimitive(PrimitiveType.Cube);
            floorPlate.name = "ResFloorPlate";
            floorPlate.transform.SetParent(res.transform, false);
            floorPlate.transform.localPosition = new Vector3(0, -0.45f, 0);
            floorPlate.transform.localScale = new Vector3(roomWidth, 1.0f, roomLen);
            floorPlate.GetComponent<MeshRenderer>().sharedMaterial = matFloor;

            // Crew sleeping pods
            for (int i = -1; i <= 1; i += 2)
            {
                GameObject pod = GameObject.CreatePrimitive(PrimitiveType.Cube);
                pod.name = $"CrewSleepPod_{i}";
                pod.transform.SetParent(res.transform, false);
                pod.transform.localPosition = new Vector3(i * 9f, 1.2f, 4f);
                pod.transform.localScale = new Vector3(4.0f, 2.4f, 6.5f);
                pod.GetComponent<MeshRenderer>().sharedMaterial = matDarkTitanium;

                // Pod warm reading light
                GameObject rLight = new GameObject($"PodLight_{i}");
                rLight.transform.SetParent(res.transform, false);
                rLight.transform.localPosition = new Vector3(i * 9f, 2.2f, 4f);
                Light pl = rLight.AddComponent<Light>();
                pl.type = LightType.Point;
                pl.color = new Color(1.0f, 0.85f, 0.65f);
                pl.range = 8f;
                pl.intensity = 1.2f;
            }

            // Crew recreation table
            GameObject table = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            table.name = "CrewRecTable";
            table.transform.SetParent(res.transform, false);
            table.transform.localPosition = new Vector3(0, 0.5f, -1.5f);
            table.transform.localScale = new Vector3(3.2f, 0.5f, 3.2f);
            table.GetComponent<MeshRenderer>().sharedMaterial = matAluminium;

            // Soft warm residential ceiling light
            GameObject resLight = new GameObject("ResLight_Main");
            resLight.transform.SetParent(res.transform, false);
            resLight.transform.localPosition = new Vector3(0, roomHeight - 0.3f, 0);
            Light l = resLight.AddComponent<Light>();
            l.type = LightType.Point;
            l.color = new Color(1.0f, 0.88f, 0.72f);
            l.range = 28f;
            l.intensity = 1.8f;
        }

        private void BuildInteriorEngineering(GameObject parent)
        {
            // 1. Central Engineering Tokamak Core under Hub
            GameObject eng = new GameObject("CentralHub_EngineeringCore");
            eng.transform.SetParent(parent.transform, false);
            eng.transform.localPosition = new Vector3(0, -15f, 0);

            MeshFilter mf = eng.AddComponent<MeshFilter>();
            MeshRenderer mr = eng.AddComponent<MeshRenderer>();
            MeshCollider mc = eng.AddComponent<MeshCollider>();

            Mesh roomMesh = StationMeshBuilder.CreateRoomMesh(50f, 50f, 8f);
            mf.sharedMesh = roomMesh;
            mc.sharedMesh = roomMesh;
            mr.sharedMaterial = matDarkTitanium;

            GameObject reactor = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            reactor.name = "TokamakFusionCore";
            reactor.transform.SetParent(eng.transform, false);
            reactor.transform.localPosition = new Vector3(0, 4f, 0);
            reactor.transform.localScale = new Vector3(12f, 12f, 12f);
            reactor.GetComponent<MeshRenderer>().sharedMaterial = matCyanLight;

            Light rl = reactor.AddComponent<Light>();
            rl.type = LightType.Point;
            rl.color = new Color(0.1f, 0.8f, 1.0f);
            rl.range = 35f;
            rl.intensity = 4.0f;

            // 2. Sector Gamma: Flight Operations & Tactical Guidance (at 315 deg, segment 31)
            float rad = 315f * Mathf.Deg2Rad;
            Vector3 radialDir = new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad));
            Quaternion rot = Quaternion.LookRotation(radialDir, Vector3.up);

            float rRoomStart = RingCorridorRadius + CorridorWidth * 0.5f + 14f; // 507m
            float roomLen = 22f;
            float roomWidth = 32f;
            float roomHeight = 5.6f;
            float rCenter = rRoomStart + roomLen * 0.5f; // 518m

            GameObject flightOps = new GameObject("SectorGamma_FlightOperations");
            flightOps.transform.SetParent(parent.transform, false);
            flightOps.transform.position = radialDir * rCenter;
            flightOps.transform.rotation = rot;

            MeshFilter fmf = flightOps.AddComponent<MeshFilter>();
            MeshRenderer fmr = flightOps.AddComponent<MeshRenderer>();
            MeshCollider fmc = flightOps.AddComponent<MeshCollider>();

            Mesh fMesh = StationMeshBuilder.CreateRoomMeshWithDoorway(roomWidth, roomLen, roomHeight, CorridorWidth * 1.05f, CorridorHeight, StationMeshBuilder.DoorwayWall.South);
            fmf.sharedMesh = fMesh;
            fmc.sharedMesh = fMesh;
            fmr.sharedMaterial = matWall;

            // Base floor plate
            GameObject fFloor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            fFloor.name = "FlightOpsFloorPlate";
            fFloor.transform.SetParent(flightOps.transform, false);
            fFloor.transform.localPosition = new Vector3(0, -0.45f, 0);
            fFloor.transform.localScale = new Vector3(roomWidth, 1.0f, roomLen);
            fFloor.GetComponent<MeshRenderer>().sharedMaterial = matFloor;

            // Tactical Telemetry Consoles & Holographic Radar Displays
            for (int x = -10; x <= 10; x += 10)
            {
                GameObject console = GameObject.CreatePrimitive(PrimitiveType.Cube);
                console.name = $"TacticalConsole_{x}";
                console.transform.SetParent(flightOps.transform, false);
                console.transform.localPosition = new Vector3(x, 0.6f, 3f);
                console.transform.localScale = new Vector3(6f, 1.2f, 2.5f);
                console.GetComponent<MeshRenderer>().sharedMaterial = matDarkTitanium;

                GameObject hud = GameObject.CreatePrimitive(PrimitiveType.Cube);
                hud.name = $"TacticalScreen_{x}";
                hud.transform.SetParent(flightOps.transform, false);
                hud.transform.localPosition = new Vector3(x, 1.6f, 3f);
                hud.transform.localScale = new Vector3(5.5f, 0.9f, 0.1f);
                hud.GetComponent<MeshRenderer>().sharedMaterial = matTechScreen;
                Object.DestroyImmediate(hud.GetComponent<BoxCollider>());
            }

            // Radar Hologram Globe in center
            GameObject radar = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            radar.name = "OrbitalRadarHolo";
            radar.transform.SetParent(flightOps.transform, false);
            radar.transform.localPosition = new Vector3(0, 2.2f, -2f);
            radar.transform.localScale = new Vector3(2.8f, 2.8f, 2.8f);
            radar.GetComponent<MeshRenderer>().sharedMaterial = matAmberLight;
            Object.DestroyImmediate(radar.GetComponent<SphereCollider>());

            // Red Alert & Amber Tactical Lighting
            GameObject fol = new GameObject("FlightOps_TacticalLight");
            fol.transform.SetParent(flightOps.transform, false);
            fol.transform.localPosition = new Vector3(0, roomHeight - 0.4f, 0);
            Light fl = fol.AddComponent<Light>();
            fl.type = LightType.Point;
            fl.color = new Color(1.0f, 0.45f, 0.25f);
            fl.range = 30f;
            fl.intensity = 2.4f;
        }


        // ==========================================
        // CONNECTING PATHWAYS & JUNCTIONS
        // ==========================================

        private void BuildStationConnectors(GameObject parent)
        {
            GameObject pathways = CreateChild(parent, "PathwaysAndJunctions");

            // 1. Spoke-to-Ring Junction Portals and Airlock Transitions
            // Spokes are at 0° (+X, East), 90° (+Z, North), 180° (-X, West), 270° (-Z, South)
            float[] spokeAngles = { 90f, 0f, 270f, 180f };
            string[] spokeDirs = { "North", "East", "South", "West" };

            for (int i = 0; i < 4; i++)
            {
                float ang = spokeAngles[i];
                float rad = ang * Mathf.Deg2Rad;
                Vector3 spokeDir = new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad));
                Quaternion rot = Quaternion.LookRotation(spokeDir, Vector3.up);

                // Hub-to-Spoke Inner Junction Vestibule
                GameObject hubVestibule = new GameObject($"JunctionVestibule_Hub_{spokeDirs[i]}");
                hubVestibule.transform.SetParent(pathways.transform, false);
                hubVestibule.transform.rotation = rot;
                hubVestibule.transform.position = spokeDir * 32f;

                GameObject vFloor = GameObject.CreatePrimitive(PrimitiveType.Cube);
                vFloor.name = "VestibuleFloorPlate";
                vFloor.transform.SetParent(hubVestibule.transform, false);
                vFloor.transform.localPosition = new Vector3(0, -0.45f, 0);
                vFloor.transform.localScale = new Vector3(CorridorWidth * 1.2f, 1.0f, 6.5f);
                vFloor.GetComponent<MeshRenderer>().sharedMaterial = matFloor;

                // Open portal frame into spoke (no blocking cube)
                CreateProceduralArch(hubVestibule, "AirlockPortalArch", Vector3.zero,
                    Quaternion.identity,
                    CorridorWidth * 1.05f, CorridorHeight, 0.50f, 0.70f, 0.70f,
                    matGold, matCyanLight, new Color(0.2f, 0.85f, 1.0f), 1.6f);

                // Spoke-to-Ring Outer Junction Vestibule (connects spoke end to ring inner wall at R = 487m)
                GameObject ringJunction = new GameObject($"JunctionVestibule_Ring_{spokeDirs[i]}");
                ringJunction.transform.SetParent(pathways.transform, false);
                ringJunction.transform.rotation = rot;
                ringJunction.transform.position = spokeDir * (RingCorridorRadius - CorridorWidth * 0.5f);

                GameObject rFloor = GameObject.CreatePrimitive(PrimitiveType.Cube);
                rFloor.name = "RingJunctionFloorPlate";
                rFloor.transform.SetParent(ringJunction.transform, false);
                rFloor.transform.localPosition = new Vector3(0, -0.45f, 0);
                rFloor.transform.localScale = new Vector3(CorridorWidth * 1.3f, 1.0f, CorridorWidth * 1.3f);
                rFloor.GetComponent<MeshRenderer>().sharedMaterial = matFloor;

                // Open portal frame into ring
                CreateProceduralArch(ringJunction, "RingPortalArch", Vector3.zero,
                    Quaternion.identity,
                    CorridorWidth * 1.05f, CorridorHeight, 0.50f, 0.70f, 0.70f,
                    matDarkTitanium, matCyanLight, new Color(0.2f, 0.85f, 1.0f), 1.8f);
            }

            // 2. Walkable Connector Corridors between Ring Outer Doorways and Specialized Sectors
            // Observation Promenade (Sector Delta at 45 deg)
            BuildSectorConnectorPathway(pathways, "Connector_ObservationDeck", 45f, 14f, "SECTOR DELTA: OBSERVATION PROMENADE", matWarmLounge, new Color(1.0f, 0.8f, 0.35f));

            // Science Labs (Sector Beta at 135 deg)
            BuildSectorConnectorPathway(pathways, "Connector_ScienceLabs", 135f, 14f, "SECTOR BETA: HYDROPONICS & BIO LABS", matEmeraldFlora, new Color(0.2f, 1.0f, 0.6f));

            // Residential Living Quarters (Sector Alpha at 225 deg)
            BuildSectorConnectorPathway(pathways, "Connector_CrewQuarters", 225f, 14f, "SECTOR ALPHA: CREW HABITATION", matCrewWarm, new Color(1.0f, 0.85f, 0.65f));

            // Flight Operations & Guidance (Sector Gamma at 315 deg)
            BuildSectorConnectorPathway(pathways, "Connector_FlightOperations", 315f, 14f, "SECTOR GAMMA: FLIGHT OPERATIONS & TACTICAL", matRedAlert, new Color(1.0f, 0.4f, 0.2f));

            // 3. Central Engineering Core Walkway / Lift Chute from Hub
            GameObject engChute = new GameObject("Pathway_HubToEngineeringCore");
            engChute.transform.SetParent(pathways.transform, false);
            engChute.transform.localPosition = new Vector3(0, -7.5f, -22f);

            GameObject liftShaft = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            liftShaft.name = "EngineeringGravLiftShaft";
            liftShaft.transform.SetParent(engChute.transform, false);
            liftShaft.transform.localScale = new Vector3(8f, 7.5f, 8f);
            liftShaft.GetComponent<MeshRenderer>().sharedMaterial = matDarkTitanium;

            GameObject gravField = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            gravField.name = "GravBeamField";
            gravField.transform.SetParent(engChute.transform, false);
            gravField.transform.localScale = new Vector3(6.5f, 7.4f, 6.5f);
            gravField.GetComponent<MeshRenderer>().sharedMaterial = matCyanLight;
            Object.DestroyImmediate(gravField.GetComponent<CapsuleCollider>());
        }

        private void BuildSectorConnectorPathway(GameObject parent, string name, float angleDeg, float sectorLength, string sectorSignText, Material archMat, Color lightColor)
        {
            float rad = angleDeg * Mathf.Deg2Rad;
            Vector3 radialDir = new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad));
            Quaternion rot = Quaternion.LookRotation(radialDir, Vector3.up);

            // Connects from Ring Outer Wall (R = 493m) outwards to Room Entry (R = 493 + sectorLength)
            float rStart = RingCorridorRadius + CorridorWidth * 0.5f; // 493m
            float rCenter = rStart + sectorLength * 0.5f; // 500m

            GameObject connector = new GameObject(name);
            connector.transform.SetParent(parent.transform, false);
            connector.transform.position = radialDir * rCenter;
            connector.transform.rotation = rot;

            // Straight octagonal corridor mesh
            GameObject meshObj = new GameObject("ConnectorCorridorMesh");
            meshObj.transform.SetParent(connector.transform, false);
            meshObj.transform.localPosition = new Vector3(0, 0, -sectorLength * 0.5f);
            MeshFilter mf = meshObj.AddComponent<MeshFilter>();
            MeshRenderer mr = meshObj.AddComponent<MeshRenderer>();
            MeshCollider mc = meshObj.AddComponent<MeshCollider>();
            Mesh straightMesh = StationMeshBuilder.CreateStraightCorridorMesh(sectorLength, CorridorWidth, CorridorHeight);
            mf.sharedMesh = straightMesh;
            mc.sharedMesh = straightMesh;
            mr.sharedMaterial = matWall;

            // Solid floor plate (prevents tunneling)
            GameObject ramp = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ramp.name = "ConnectorFloorPlate";
            ramp.transform.SetParent(connector.transform, false);
            ramp.transform.localPosition = new Vector3(0, -0.45f, 0);
            ramp.transform.localScale = new Vector3(CorridorWidth * 0.95f, 1.0f, sectorLength);
            ramp.GetComponent<MeshRenderer>().sharedMaterial = matFloor;

            // Ceiling Strip Light
            GameObject cLight = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cLight.name = "ConnectorCeilingLight";
            cLight.transform.SetParent(connector.transform, false);
            cLight.transform.localPosition = new Vector3(0, CorridorHeight - 0.1f, 0);
            cLight.transform.localScale = new Vector3(CorridorWidth * 0.6f, 0.08f, sectorLength * 0.8f);
            cLight.GetComponent<MeshRenderer>().sharedMaterial = archMat;
            Object.DestroyImmediate(cLight.GetComponent<BoxCollider>());

            Light pLight = cLight.AddComponent<Light>();
            pLight.type = LightType.Point;
            pLight.color = lightColor;
            pLight.range = 18f;
            pLight.intensity = 2.0f;

            // 1. Entrance Arch Frame at Ring Outer Wall (Z = -sectorLength * 0.5f)
            CreateProceduralArch(connector, "SectorEntranceArch", new Vector3(0, 0, -sectorLength * 0.5f),
                Quaternion.identity,
                CorridorWidth * 1.0f, CorridorHeight, 0.45f, 0.55f, 0.65f,
                matDarkTitanium, archMat, lightColor, 1.8f);

            // 2. Sliding Proximity Security Door mounted inside the entrance arch
            GameObject doorObj = new GameObject("SectorSecurityDoor");
            doorObj.transform.SetParent(connector.transform, false);
            doorObj.transform.localPosition = new Vector3(0, 0, -sectorLength * 0.5f);

            GameObject leftDoor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            leftDoor.name = "DoorLeaf_Left";
            leftDoor.transform.SetParent(doorObj.transform, false);
            leftDoor.transform.localPosition = new Vector3(-CorridorWidth * 0.25f, CorridorHeight * 0.5f, 0);
            leftDoor.transform.localScale = new Vector3(CorridorWidth * 0.5f, CorridorHeight, 0.15f);
            leftDoor.GetComponent<MeshRenderer>().sharedMaterial = matDarkTitanium;

            GameObject rightDoor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            rightDoor.name = "DoorLeaf_Right";
            rightDoor.transform.SetParent(doorObj.transform, false);
            rightDoor.transform.localPosition = new Vector3(CorridorWidth * 0.25f, CorridorHeight * 0.5f, 0);
            rightDoor.transform.localScale = new Vector3(CorridorWidth * 0.5f, CorridorHeight, 0.15f);
            rightDoor.GetComponent<MeshRenderer>().sharedMaterial = matDarkTitanium;

            StationDoor doorScript = doorObj.AddComponent<StationDoor>();
            doorScript.leftPanel = leftDoor.transform;
            doorScript.rightPanel = rightDoor.transform;
            doorScript.detectionRadius = 5.5f;
            doorScript.slideDistance = CorridorWidth * 0.45f;
            doorScript.openSpeed = 4.5f;

            // 3. Sector Navigation Sign above Entrance Arch
            GameObject sign = GameObject.CreatePrimitive(PrimitiveType.Cube);
            sign.name = "SectorNavigationSign";
            sign.transform.SetParent(connector.transform, false);
            sign.transform.localPosition = new Vector3(0, CorridorHeight - 0.2f, -sectorLength * 0.5f - 0.1f);
            sign.transform.localScale = new Vector3(3.6f, 0.55f, 0.08f);
            sign.GetComponent<MeshRenderer>().sharedMaterial = archMat;
            Object.DestroyImmediate(sign.GetComponent<BoxCollider>());

            // 4. Room Exit Arch Frame (Z = +sectorLength * 0.5f) entering the specialized sector
            CreateProceduralArch(connector, "SectorRoomExitArch", new Vector3(0, 0, sectorLength * 0.5f),
                Quaternion.identity,
                CorridorWidth * 1.0f, CorridorHeight, 0.45f, 0.55f, 0.65f,
                matDarkTitanium, archMat, lightColor, 1.8f);
        }

        // ==========================================
        // SPAWN BAYS & TELEPORTATION STATIONS
        // ==========================================

        private void BuildPlayerSpawnBays(GameObject parent)
        {
            GameObject spawnsRoot = CreateChild(parent, "PlayerSpawnStations");

            // 1. Primary Spawn Bay - Command Operations Rotunda (Center of Hub)
            CreateSpawnPlatform(spawnsRoot, "SpawnBay_CommandRotunda", new Vector3(0, 0.05f, 6.0f), Quaternion.identity, "Sector 00 - Command Center Central Hub", true);

            // 2. Secondary Spawn Bay - Observation Promenade (Sector Delta facing Earth & Sun)
            float radDelta = 45f * Mathf.Deg2Rad;
            Vector3 dirDelta = new Vector3(Mathf.Cos(radDelta), 0, Mathf.Sin(radDelta));
            Vector3 posDelta = dirDelta * 514f;
            Quaternion rotDelta = Quaternion.LookRotation(dirDelta, Vector3.up);
            CreateSpawnPlatform(spawnsRoot, "SpawnBay_ObservationPromenade", new Vector3(posDelta.x, 0.05f, posDelta.z), rotDelta, "Sector Delta - Observation Promenade", false);

            // 3. Crew Spawn Bay - Living Quarters (Sector Alpha)
            float radAlpha = 225f * Mathf.Deg2Rad;
            Vector3 dirAlpha = new Vector3(Mathf.Cos(radAlpha), 0, Mathf.Sin(radAlpha));
            Vector3 posAlpha = dirAlpha * 514f;
            Quaternion rotAlpha = Quaternion.LookRotation(dirAlpha, Vector3.up);
            CreateSpawnPlatform(spawnsRoot, "SpawnBay_CrewLivingQuarters", new Vector3(posAlpha.x, 0.05f, posAlpha.z), rotAlpha, "Sector Alpha - Crew Habitation", false);

            // 4. Transit Terminal Spawn Bay - High Speed Pod Station North
            CreateSpawnPlatform(spawnsRoot, "SpawnBay_TransitNorthTerminal", new Vector3(0, 0.05f, 38.0f), Quaternion.identity, "Sector Transit - North Transit Pod Dock", false);
        }

        private GameObject CreateSpawnPlatform(GameObject parent, string name, Vector3 position, Quaternion rotation, string sectorLabel, bool isPrimary)
        {
            GameObject spawnObj = new GameObject(name);
            spawnObj.transform.SetParent(parent.transform, false);
            spawnObj.transform.position = position;
            spawnObj.transform.rotation = rotation;

            // Outer hexagonal/cylindrical metallic pad (remove collider to prevent capsule edge catching)
            GameObject padBase = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            padBase.name = "SpawnPlatformBase";
            padBase.transform.SetParent(spawnObj.transform, false);
            padBase.transform.localPosition = new Vector3(0, 0.06f, 0);
            padBase.transform.localScale = new Vector3(3.2f, 0.1f, 3.2f);
            padBase.GetComponent<MeshRenderer>().sharedMaterial = matDarkTitanium;
            Object.DestroyImmediate(padBase.GetComponent<CapsuleCollider>());

            // Inner emissive energy ring
            GameObject energyRing = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            energyRing.name = "HolographicEnergyRing";
            energyRing.transform.SetParent(spawnObj.transform, false);
            energyRing.transform.localPosition = new Vector3(0, 0.12f, 0);
            energyRing.transform.localScale = new Vector3(2.5f, 0.04f, 2.5f);
            energyRing.GetComponent<MeshRenderer>().sharedMaterial = isPrimary ? matCyanLight : matAmberLight;
            Object.DestroyImmediate(energyRing.GetComponent<CapsuleCollider>());

            // Center spawn anchor node
            GameObject spawnAnchor = new GameObject("SpawnPointAnchor");
            spawnAnchor.transform.SetParent(spawnObj.transform, false);
            spawnAnchor.transform.localPosition = new Vector3(0, 0.15f, 0);

            // Overhead Teleporter / Arrival Illuminator
            GameObject emitterCap = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            emitterCap.name = "OverheadEmitterCap";
            emitterCap.transform.SetParent(spawnObj.transform, false);
            emitterCap.transform.localPosition = new Vector3(0, CorridorHeight - 0.2f, 0);
            emitterCap.transform.localScale = new Vector3(2.0f, 0.15f, 2.0f);
            emitterCap.GetComponent<MeshRenderer>().sharedMaterial = matDarkTitanium;
            Object.DestroyImmediate(emitterCap.GetComponent<CapsuleCollider>());

            // Vertical beacon light beam
            GameObject beaconObj = new GameObject("SpawnBeaconLight");
            beaconObj.transform.SetParent(spawnObj.transform, false);
            beaconObj.transform.localPosition = new Vector3(0, CorridorHeight * 0.5f, 0);

            Light beacon = beaconObj.AddComponent<Light>();
            beacon.type = LightType.Point;
            beacon.color = isPrimary ? new Color(0.2f, 0.9f, 1.0f) : new Color(1.0f, 0.7f, 0.2f);
            beacon.range = 8.0f;
            beacon.intensity = 2.2f;

            // Logic script
            PlayerSpawnBay bayScript = spawnObj.AddComponent<PlayerSpawnBay>();
            bayScript.sectorName = sectorLabel;
            bayScript.spawnPoint = spawnAnchor.transform;
            bayScript.beaconLight = beacon;
            bayScript.holographicRing = energyRing.GetComponent<MeshRenderer>();
            bayScript.rotationSpeed = isPrimary ? 35f : 20f;
            bayScript.pulseSpeed = 2.5f;

            return spawnObj;
        }

        // ==========================================
        // LIGHTING, ENVIRONMENT & GAMEPLAY
        // ==========================================

        private void BuildStationLighting(GameObject parent)
        {
            // Primary Solar Light Source: Directional Light aligned with the distant Sun
            // Positioned to illuminate Earth's oceans and continents from an atmospheric angle
            Vector3 sunPos = new Vector3(2400f, 2200f, -400f);
            Vector3 sunDir = (Vector3.zero - sunPos).normalized;

            GameObject sun = GameObject.Find("Directional Light");
            if (sun == null)
            {
                sun = new GameObject("Directional Light");
                sun.transform.SetParent(parent.transform, false);
                sun.AddComponent<Light>();
            }
            else
            {
                sun.transform.SetParent(parent.transform, true);
            }

            Light sunLight = sun.GetComponent<Light>();
            sunLight.type = LightType.Directional;
            sunLight.color = new Color(1.0f, 0.98f, 0.93f);
            sunLight.intensity = 2.4f; // Powerful radiant solar illumination
            sunLight.shadows = LightShadows.Soft;
            sun.transform.rotation = Quaternion.LookRotation(sunDir, Vector3.up);

            // Earth Albedo Secondary Fill Light (cool planetary bounce on dark hull side)
            Vector3 earthPos = new Vector3(1200f, -100f, 1200f);
            Vector3 earthDir = (Vector3.zero - earthPos).normalized;

            GameObject earthGlow = GameObject.Find("EarthRimLight");
            if (earthGlow == null)
            {
                earthGlow = new GameObject("EarthRimLight");
                earthGlow.transform.SetParent(parent.transform, false);
                Light erl = earthGlow.AddComponent<Light>();
                erl.type = LightType.Directional;
                erl.color = new Color(0.18f, 0.42f, 0.70f);
                erl.intensity = 0.65f;
                erl.shadows = LightShadows.None;
                earthGlow.transform.rotation = Quaternion.LookRotation(earthDir, Vector3.up);
            }
            else
            {
                Light erl = earthGlow.GetComponent<Light>();
                erl.intensity = 0.65f;
                erl.color = new Color(0.18f, 0.42f, 0.70f);
                earthGlow.transform.rotation = Quaternion.LookRotation(earthDir, Vector3.up);
            }
        }

        private void BuildStationEnvironment(GameObject parent)
        {
            // Deep space with soft sci-fi interior ambient bounce (prevents pitch black voids in corridors)
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.12f, 0.14f, 0.18f);

            // 1. Deep Space Star Dome
            GameObject skyDome = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            skyDome.name = "DeepSpaceStarDome";
            skyDome.transform.SetParent(parent.transform, false);
            skyDome.transform.position = Vector3.zero;
            skyDome.transform.localScale = new Vector3(9000f, 9000f, 9000f);
            Object.DestroyImmediate(skyDome.GetComponent<SphereCollider>());

            // Inverted normals so star texture is visible from inside looking out
            Mesh mesh = Object.Instantiate(skyDome.GetComponent<MeshFilter>().sharedMesh);
            Vector3[] normals = mesh.normals;
            for (int i = 0; i < normals.Length; i++) normals[i] = -normals[i];
            int[] triangles = mesh.triangles;
            for (int i = 0; i < triangles.Length; i += 3)
            {
                int temp = triangles[i];
                triangles[i] = triangles[i + 1];
                triangles[i + 1] = temp;
            }
            mesh.normals = normals;
            mesh.triangles = triangles;
            skyDome.GetComponent<MeshFilter>().sharedMesh = mesh;

            Material skyMat = new Material(Shader.Find("Universal Render Pipeline/Unlit") ?? Shader.Find("Unlit/Texture"));
            Texture2D texStars = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/SpaceStation5/Textures/T_Deep_Space_Stars.png");
            if (texStars != null)
            {
                skyMat.SetTexture("_BaseMap", texStars);
                skyMat.SetTextureScale("_BaseMap", new Vector2(8f, 4f));
            }
            skyDome.GetComponent<MeshRenderer>().sharedMaterial = skyMat;

            // ANIMATED SPACE: Add AnimatedSpaceEnvironment component for celestial rotation & UV drift
            AnimatedSpaceEnvironment animSpace = skyDome.AddComponent<AnimatedSpaceEnvironment>();
            animSpace.rotationSpeed = new Vector3(0.005f, 0.015f, 0.003f);
            animSpace.uvDriftSpeed = new Vector2(0.0002f, 0.0001f);
            animSpace.enablePulsing = true;
            animSpace.pulseFrequency = 0.35f;
            animSpace.minIntensity = 0.90f;
            animSpace.maxIntensity = 1.25f;

            // -------------------------------------------------------------
            // 2. DETAILED PLANET EARTH (VISIBLE FROM OBSERVATION DECK & RING)
            // -------------------------------------------------------------
            Vector3 earthPos = new Vector3(1200f, -100f, 1200f);
            GameObject earthRoot = new GameObject("PlanetEarth_System");
            earthRoot.transform.SetParent(parent.transform, false);
            earthRoot.transform.position = earthPos;

            // Earth Solid Surface Body
            GameObject earthBody = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            earthBody.name = "PlanetEarth_Body";
            earthBody.transform.SetParent(earthRoot.transform, false);
            earthBody.transform.localPosition = Vector3.zero;
            earthBody.transform.localScale = new Vector3(1200f, 1200f, 1200f);
            earthBody.transform.rotation = Quaternion.Euler(23.4f, 45f, 0f); // Earth axial tilt
            earthBody.GetComponent<MeshRenderer>().sharedMaterial = matEarth;
            Object.DestroyImmediate(earthBody.GetComponent<SphereCollider>());

            // Earth Atmospheric Cloud Layer (rotates independently for atmospheric swirl)
            GameObject earthClouds = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            earthClouds.name = "PlanetEarth_Clouds";
            earthClouds.transform.SetParent(earthRoot.transform, false);
            earthClouds.transform.localPosition = Vector3.zero;
            earthClouds.transform.localScale = new Vector3(1212f, 1212f, 1212f);
            earthClouds.transform.rotation = Quaternion.Euler(23.4f, 75f, 0f);
            earthClouds.GetComponent<MeshRenderer>().sharedMaterial = matEarthClouds;
            Object.DestroyImmediate(earthClouds.GetComponent<SphereCollider>());

            // Earth atmospheric clouds are rendered over the surface body seamlessly

            // -------------------------------------------------------------
            // 3. DISTANT RADIANT SUN (PRIMARY SOLAR SOURCE)
            // -------------------------------------------------------------
            Vector3 sunPos = new Vector3(2400f, 2200f, -400f);
            GameObject sunObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sunObj.name = "DistantSun_Radiant";
            sunObj.transform.SetParent(parent.transform, false);
            sunObj.transform.position = sunPos;
            sunObj.transform.localScale = new Vector3(450f, 450f, 450f);
            sunObj.GetComponent<MeshRenderer>().sharedMaterial = matSun;
            Object.DestroyImmediate(sunObj.GetComponent<SphereCollider>());

            // Solar Corona Flare (Facing station)
            GameObject sunAura = GameObject.CreatePrimitive(PrimitiveType.Quad);
            sunAura.name = "Sun_CoronaFlare";
            sunAura.transform.SetParent(sunObj.transform, false);
            sunAura.transform.localPosition = Vector3.zero;
            sunAura.transform.rotation = Quaternion.LookRotation(-sunPos.normalized, Vector3.up);
            sunAura.transform.localScale = new Vector3(3.2f, 3.2f, 3.2f);
            sunAura.GetComponent<MeshRenderer>().sharedMaterial = matSun;
            Object.DestroyImmediate(sunAura.GetComponent<MeshCollider>());

            // -------------------------------------------------------------
            // 4. CELESTIAL ORBIT & STATION DYNAMICS CONTROLLER
            // -------------------------------------------------------------
            CelestialOrbitController orbit = parent.AddComponent<CelestialOrbitController>();
            orbit.stationTransform = parent.transform;
            orbit.earthTransform = earthBody.transform;
            orbit.cloudTransform = earthClouds.transform;
            orbit.stationSpinSpeed = 0.12f; // Slow majestic rotation for artificial gravity & panoramic star drift
            orbit.earthSpinSpeed = 0.02f;
            orbit.cloudSpinSpeed = 0.035f;
            orbit.solarOrbitSpeed = 0.005f;
        }

        private void BuildTransitPods(GameObject parent)
        {
            // Transit Pod stationed at a designated ceiling monorail track with open passenger bay
            GameObject pod = new GameObject("HighSpeedTransitPod_North");
            pod.transform.SetParent(parent.transform, false);
            pod.transform.position = new Vector3(0, 0.05f, 40f);

            // Floor base plate
            GameObject podFloor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            podFloor.name = "PodTreadPlate";
            podFloor.transform.SetParent(pod.transform, false);
            podFloor.transform.localPosition = new Vector3(0, 0.05f, 0);
            podFloor.transform.localScale = new Vector3(3.6f, 0.08f, 5.5f);
            podFloor.GetComponent<MeshRenderer>().sharedMaterial = matFloor;

            // Canopy roof overhead (mounted near ceiling, leaving walkway 100% open)
            GameObject podCanopy = GameObject.CreatePrimitive(PrimitiveType.Cube);
            podCanopy.name = "PodMonorailCanopy";
            podCanopy.transform.SetParent(pod.transform, false);
            podCanopy.transform.localPosition = new Vector3(0, CorridorHeight - 0.35f, 0);
            podCanopy.transform.localScale = new Vector3(3.8f, 0.3f, 5.5f);
            podCanopy.GetComponent<MeshRenderer>().sharedMaterial = matDarkTitanium;

            // Side glass railings (thin, leaving central 2.6m completely walkable)
            GameObject railL = GameObject.CreatePrimitive(PrimitiveType.Cube);
            railL.name = "PodRail_L";
            railL.transform.SetParent(pod.transform, false);
            railL.transform.localPosition = new Vector3(-1.75f, 1.0f, 0);
            railL.transform.localScale = new Vector3(0.1f, 1.8f, 5.2f);
            railL.GetComponent<MeshRenderer>().sharedMaterial = matGlass;

            GameObject railR = GameObject.CreatePrimitive(PrimitiveType.Cube);
            railR.name = "PodRail_R";
            railR.transform.SetParent(pod.transform, false);
            railR.transform.localPosition = new Vector3(1.75f, 1.0f, 0);
            railR.transform.localScale = new Vector3(0.1f, 1.8f, 5.2f);
            railR.GetComponent<MeshRenderer>().sharedMaterial = matGlass;

            // Emissive interior guide light
            GameObject podLight = GameObject.CreatePrimitive(PrimitiveType.Cube);
            podLight.name = "PodCeilingLight";
            podLight.transform.SetParent(pod.transform, false);
            podLight.transform.localPosition = new Vector3(0, CorridorHeight - 0.52f, 0);
            podLight.transform.localScale = new Vector3(1.8f, 0.05f, 3.5f);
            podLight.GetComponent<MeshRenderer>().sharedMaterial = matCyanLight;
            Object.DestroyImmediate(podLight.GetComponent<BoxCollider>());

            Light pLight = podLight.AddComponent<Light>();
            pLight.type = LightType.Point;
            pLight.color = new Color(0.1f, 0.85f, 1.0f);
            pLight.range = 14f;
            pLight.intensity = 2.0f;

            StationTransitPod transit = pod.AddComponent<StationTransitPod>();
            transit.startPosition = new Vector3(0, 0.05f, 40f);
            transit.endPosition = new Vector3(0, 0.05f, RingCorridorRadius - 15f);
            transit.speed = 35f;
        }

        private void BuildPlayerSetup(GameObject parent)
        {
            GameObject oldCam = GameObject.Find("Main Camera");
            if (oldCam != null)
            {
                Object.DestroyImmediate(oldCam);
            }

            // Ensure XR Interaction Manager exists in the scene
            var existingMgr = Object.FindAnyObjectByType<UnityEngine.XR.Interaction.Toolkit.XRInteractionManager>();
            if (existingMgr == null)
            {
                GameObject mgrObj = new GameObject("XR Interaction Manager");
                mgrObj.AddComponent<UnityEngine.XR.Interaction.Toolkit.XRInteractionManager>();
                mgrObj.transform.SetParent(parent.transform, false);
            }

            // Find primary spawn bay if available (prioritize central Command Rotunda)
            Vector3 spawnPosition = new Vector3(0, 1.2f, 6.0f);
            Quaternion spawnRotation = Quaternion.identity;

            var spawnBays = Object.FindObjectsByType<PlayerSpawnBay>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            foreach (var bay in spawnBays)
            {
                if (bay.gameObject.name.Contains("CommandRotunda"))
                {
                    spawnPosition = bay.GetSpawnPosition() + Vector3.up * 1.0f;
                    spawnRotation = bay.GetSpawnRotation();
                    break;
                }
            }

            GameObject player = new GameObject("Player_Astronaut_VR");
            player.tag = "Player";
            player.transform.SetParent(parent.transform, false);
            player.transform.position = spawnPosition;
            player.transform.rotation = spawnRotation;

            CharacterController cc = player.AddComponent<CharacterController>();
            cc.height = 1.8f;
            cc.radius = 0.4f;
            cc.center = new Vector3(0, 0.9f, 0);

            // XR Origin configuration
            var xrOrigin = player.AddComponent<Unity.XR.CoreUtils.XROrigin>();
            xrOrigin.RequestedTrackingOriginMode = Unity.XR.CoreUtils.XROrigin.TrackingOriginMode.Floor;

            // Camera Offset hierarchy
            GameObject camOffset = new GameObject("Camera Offset");
            camOffset.transform.SetParent(player.transform, false);
            xrOrigin.CameraFloorOffsetObject = camOffset;

            // Main Camera (Head Tracking)
            GameObject camObj = new GameObject("Main Camera");
            camObj.tag = "MainCamera";
            camObj.transform.SetParent(camOffset.transform, false);
            camObj.transform.localPosition = new Vector3(0, 1.7f, 0);

            Camera cam = camObj.AddComponent<Camera>();
            cam.nearClipPlane = 0.05f;
            cam.farClipPlane = 25000f;
            camObj.AddComponent<AudioListener>();
            xrOrigin.Camera = cam;

            var uac = camObj.AddComponent<UnityEngine.Rendering.Universal.UniversalAdditionalCameraData>();
            uac.renderPostProcessing = true;

            var tpd = camObj.AddComponent<UnityEngine.InputSystem.XR.TrackedPoseDriver>();
            tpd.trackingType = UnityEngine.InputSystem.XR.TrackedPoseDriver.TrackingType.RotationAndPosition;

            // Flashlight on headlamp
            GameObject lightObj = new GameObject("Headlamp_Flashlight");
            lightObj.transform.SetParent(camObj.transform, false);
            lightObj.transform.localPosition = new Vector3(0.2f, -0.1f, 0.3f);

            Light flash = lightObj.AddComponent<Light>();
            flash.type = LightType.Spot;
            flash.color = new Color(0.95f, 0.98f, 1.0f);
            flash.range = 35f;
            flash.spotAngle = 55f;
            flash.intensity = 1.8f;

            // Load XRI Default Input Actions
            var xriActions = AssetDatabase.LoadAssetAtPath<UnityEngine.InputSystem.InputActionAsset>(
                "Assets/Samples/XR Interaction Toolkit/3.2.1/Starter Assets/XRI Default Input Actions.inputactions");

            if (xriActions != null)
            {
                var inputActionMgr = player.AddComponent<UnityEngine.XR.Interaction.Toolkit.Inputs.InputActionManager>();
                inputActionMgr.actionAssets = new List<UnityEngine.InputSystem.InputActionAsset> { xriActions };
            }

            // Left Hand Controller
            GameObject leftHand = new GameObject("Left Hand");
            leftHand.transform.SetParent(camOffset.transform, false);
            leftHand.transform.localPosition = new Vector3(-0.25f, 1.2f, 0.4f);
            var leftTpd = leftHand.AddComponent<UnityEngine.InputSystem.XR.TrackedPoseDriver>();
            leftTpd.trackingType = UnityEngine.InputSystem.XR.TrackedPoseDriver.TrackingType.RotationAndPosition;

            // Right Hand Controller
            GameObject rightHand = new GameObject("Right Hand");
            rightHand.transform.SetParent(camOffset.transform, false);
            rightHand.transform.localPosition = new Vector3(0.25f, 1.2f, 0.4f);
            var rightTpd = rightHand.AddComponent<UnityEngine.InputSystem.XR.TrackedPoseDriver>();
            rightTpd.trackingType = UnityEngine.InputSystem.XR.TrackedPoseDriver.TrackingType.RotationAndPosition;

            // Wire input action references to TrackedPoseDrivers
            if (xriActions != null)
            {
                var headPos = xriActions.FindAction("XRI Head/Position");
                var headRot = xriActions.FindAction("XRI Head/Rotation");
                var headState = xriActions.FindAction("XRI Head/Tracking State");
                if (headPos != null) tpd.positionInput = new UnityEngine.InputSystem.InputActionProperty(headPos);
                if (headRot != null) tpd.rotationInput = new UnityEngine.InputSystem.InputActionProperty(headRot);
                if (headState != null) tpd.trackingStateInput = new UnityEngine.InputSystem.InputActionProperty(headState);

                var leftPos = xriActions.FindAction("XRI Left/Position");
                var leftRot = xriActions.FindAction("XRI Left/Rotation");
                var leftState = xriActions.FindAction("XRI Left/Tracking State");
                if (leftPos != null) leftTpd.positionInput = new UnityEngine.InputSystem.InputActionProperty(leftPos);
                if (leftRot != null) leftTpd.rotationInput = new UnityEngine.InputSystem.InputActionProperty(leftRot);
                if (leftState != null) leftTpd.trackingStateInput = new UnityEngine.InputSystem.InputActionProperty(leftState);

                var rightPos = xriActions.FindAction("XRI Right/Position");
                var rightRot = xriActions.FindAction("XRI Right/Rotation");
                var rightState = xriActions.FindAction("XRI Right/Tracking State");
                if (rightPos != null) rightTpd.positionInput = new UnityEngine.InputSystem.InputActionProperty(rightPos);
                if (rightRot != null) rightTpd.rotationInput = new UnityEngine.InputSystem.InputActionProperty(rightRot);
                if (rightState != null) rightTpd.trackingStateInput = new UnityEngine.InputSystem.InputActionProperty(rightState);
            }

            // Visual Controller Models
            GameObject leftControllerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(
                "Assets/Samples/XR Interaction Toolkit/3.2.1/Starter Assets/Prefabs/Controllers/XR Controller Left.prefab");
            if (leftControllerPrefab != null)
            {
                GameObject leftVis = (GameObject)PrefabUtility.InstantiatePrefab(leftControllerPrefab, leftHand.transform);
                leftVis.name = "XR Controller Left Visual";
                leftVis.transform.localPosition = Vector3.zero;
                leftVis.transform.localRotation = Quaternion.identity;
            }

            GameObject rightControllerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(
                "Assets/Samples/XR Interaction Toolkit/3.2.1/Starter Assets/Prefabs/Controllers/XR Controller Right.prefab");
            if (rightControllerPrefab != null)
            {
                GameObject rightVis = (GameObject)PrefabUtility.InstantiatePrefab(rightControllerPrefab, rightHand.transform);
                rightVis.name = "XR Controller Right Visual";
                rightVis.transform.localPosition = Vector3.zero;
                rightVis.transform.localRotation = Quaternion.identity;
            }

            // Hand Interactors (Direct Interactor for grasping and UI Poke)
            GameObject directInterPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(
                "Assets/Samples/XR Interaction Toolkit/3.2.1/Starter Assets/Prefabs/Interactors/Direct Interactor.prefab");
            if (directInterPrefab != null)
            {
                GameObject leftDirect = (GameObject)PrefabUtility.InstantiatePrefab(directInterPrefab, leftHand.transform);
                leftDirect.name = "Direct Interactor";
                leftDirect.transform.localPosition = Vector3.zero;

                GameObject rightDirect = (GameObject)PrefabUtility.InstantiatePrefab(directInterPrefab, rightHand.transform);
                rightDirect.name = "Direct Interactor";
                rightDirect.transform.localPosition = Vector3.zero;
            }

            // Ray Interactor for UI navigation & distance interactions
            GameObject rayInterPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(
                "Assets/Samples/XR Interaction Toolkit/3.2.1/Starter Assets/Prefabs/Interactors/Ray Interactor.prefab");
            if (rayInterPrefab != null)
            {
                GameObject rightRay = (GameObject)PrefabUtility.InstantiatePrefab(rayInterPrefab, rightHand.transform);
                rightRay.name = "Ray Interactor";
                rightRay.transform.localPosition = Vector3.zero;
            }

            SpaceStationPlayerController fps = player.AddComponent<SpaceStationPlayerController>();
            fps.playerCamera = cam;
            fps.flashlight = flash;
            fps.spawnPosition = spawnPosition;
        }
    }
}


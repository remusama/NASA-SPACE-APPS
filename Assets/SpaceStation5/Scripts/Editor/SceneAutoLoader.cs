#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

namespace SpaceStation5.Editor
{
    [InitializeOnLoad]
    public static class SceneAutoLoader
    {
        private const string MainScenePath = "Assets/SpaceStation5/Scenes/SpaceStation5.unity";

        static SceneAutoLoader()
        {
            EditorApplication.delayCall += CheckAndRebuildIfEmpty;
        }

        private static void CheckAndRebuildIfEmpty()
        {
            // Never rebuild authoring content while entering or running Play Mode. The play-mode
            // scene loads asynchronously, so the checks below can see an empty scene and would
            // rebuild/dirty the real scene (and throw "This cannot be used during play mode").
            if (EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isPlaying)
            {
                return;
            }

            Scene activeScene = SceneManager.GetActiveScene();
            
            // Si la escena activa no es la principal, abrirla
            if (activeScene.path != MainScenePath)
            {
                if (File.Exists(MainScenePath))
                {
                    activeScene = EditorSceneManager.OpenScene(MainScenePath, OpenSceneMode.Single);
                }
            }

            // Si la escena está vacía o le falta la estación / cámaras
            if (GameObject.Find("SpaceStation5") == null || Object.FindAnyObjectByType<Camera>() == null)
            {
                Debug.Log("[SceneAutoLoader] Scene is missing station or camera. Running full automated rebuild for VR & SpaceStation5...");
                RebuildEntireExperience();
            }
        }

        [MenuItem("SpaceStation5/Reconstruir Escena Completa (VR + Estación + Agujero Negro)", priority = 0)]
        public static void RebuildEntireExperience()
        {
            Scene activeScene = SceneManager.GetActiveScene();
            if (activeScene.path != MainScenePath)
            {
                activeScene = EditorSceneManager.OpenScene(MainScenePath, OpenSceneMode.Single);
            }

            // 1. Construir la estación espacial completa (con setup VR y escritorio)
            Debug.Log("[SceneAutoLoader] 1/3: Construyendo arquitectura de Space Station 5...");
            StationArchitect architect = new StationArchitect();
            architect.BuildStation();

            // 2. Construir el universo majestuoso, Tierra, Sol, Luna y Agujero Negro supermasivo
            Debug.Log("[SceneAutoLoader] 2/3: Construyendo Universo cósmico y Agujero Negro...");
            UniverseEnvironmentArchitect.BuildUniverseAndBlackHole();

            // 3. Guardar la escena
            Debug.Log("[SceneAutoLoader] 3/3: Guardando escena...");
            EditorSceneManager.MarkSceneDirty(activeScene);
            EditorSceneManager.SaveScene(activeScene);

            // 4. Centrar vista de escena si está disponible
            if (SceneView.lastActiveSceneView != null)
            {
                GameObject player = GameObject.Find("Player_Astronaut_VR");
                if (player != null)
                {
                    SceneView.lastActiveSceneView.Frame(new Bounds(player.transform.position, Vector3.one * 10f), false);
                }
            }

            Debug.Log("<color=green>[SceneAutoLoader] ¡Escena de Realidad Virtual (VR) y Estación Espacial restauradas exitosamente!</color>");
        }

        [MenuItem("SpaceStation5/Cargar Escena Principal (SpaceStation5)", priority = 1)]
        public static void OpenMainScene()
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene(MainScenePath, OpenSceneMode.Single);
            }
        }
    }
}
#endif

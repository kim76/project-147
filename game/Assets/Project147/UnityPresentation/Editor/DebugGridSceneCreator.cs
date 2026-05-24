using Project147.UnityPresentation.Debug;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Project147.UnityPresentation.Editor
{
    public static class DebugGridSceneCreator
    {
        private const string ScenePath = "Assets/Project147/UnityPresentation/Debug/DebugGridScene.unity";

        [MenuItem("Project147/Debug/Create Grid Scene")]
        public static void CreateGridScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            CreateCamera();
            CreateLight();
            CreateController();

            EditorSceneManager.SaveScene(scene, ScenePath);
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Project147", "Debug grid scene created.", "OK");
        }

        private static void CreateCamera()
        {
            var cameraObject = new GameObject("Main Camera");
            var camera = cameraObject.AddComponent<Camera>();
            camera.orthographic = true;
            camera.orthographicSize = 5.5f;
            camera.transform.position = new Vector3(3.8f, 7.5f, -4.5f);
            camera.transform.rotation = Quaternion.Euler(60, 0, 0);
            cameraObject.tag = "MainCamera";
        }

        private static void CreateLight()
        {
            var lightObject = new GameObject("Directional Light");
            var light = lightObject.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.2f;
            lightObject.transform.rotation = Quaternion.Euler(50, -30, 0);
        }

        private static void CreateController()
        {
            var controllerObject = new GameObject("Debug Grid Controller");
            var controller = controllerObject.AddComponent<DebugGridSceneController>();

            AssignMaterial(controller, "openCellMaterial", CreateMaterial("Debug_OpenCell", new Color(0.22f, 0.24f, 0.28f)));
            AssignMaterial(controller, "blockedCellMaterial", CreateMaterial("Debug_BlockedCell", new Color(0.13f, 0.13f, 0.14f)));
            AssignMaterial(controller, "pathCellMaterial", CreateMaterial("Debug_PathCell", new Color(0.1f, 0.55f, 0.95f)));
            AssignMaterial(controller, "spawnCellMaterial", CreateMaterial("Debug_SpawnCell", new Color(0.2f, 0.8f, 0.35f)));
            AssignMaterial(controller, "goalCellMaterial", CreateMaterial("Debug_GoalCell", new Color(0.95f, 0.75f, 0.2f)));
            AssignMaterial(controller, "towerCellMaterial", CreateMaterial("Debug_TowerCell", new Color(0.9f, 0.2f, 0.25f)));
            AssignMaterial(controller, "alienMaterial", CreateMaterial("Debug_Alien", new Color(0.8f, 0.25f, 1f)));
        }

        private static Material CreateMaterial(string name, Color colour)
        {
            var path = $"Assets/Project147/UnityPresentation/Debug/{name}.mat";
            var existingMaterial = AssetDatabase.LoadAssetAtPath<Material>(path);

            if (existingMaterial != null)
            {
                existingMaterial.color = colour;
                EditorUtility.SetDirty(existingMaterial);
                return existingMaterial;
            }

            var material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            material.name = name;
            material.color = colour;

            AssetDatabase.CreateAsset(material, path);
            return material;
        }

        private static void AssignMaterial(Object target, string fieldName, Material material)
        {
            var serializedObject = new SerializedObject(target);
            serializedObject.FindProperty(fieldName).objectReferenceValue = material;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}

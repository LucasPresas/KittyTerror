using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace KittyTerror.EditorTools
{
    public static class EnvironmentSetupTool
    {
        private const string ArtRoot = "Assets/Art";
        private const string ModelsRoot = "Assets/Art/Models";
        private const string PropsModelsRoot = "Assets/Art/Models/Props";
        private const string CharactersModelsRoot = "Assets/Art/Models/Characters";
        private const string MaterialsRoot = "Assets/Art/Materials";
        private const string PropsMaterialsRoot = "Assets/Art/Materials/Props";
        private const string PrefabsRoot = "Assets/Prefabs";
        private const string PropsPrefabsRoot = "Assets/Prefabs/Props";

        private static readonly string[] PropModelNames =
        {
            "Armchair",
            "Chair",
            "Forniture1",
            "Table"
        };

        [MenuItem("Tools/Kitty Terror/Setup/Organize models + materials + prefabs")]
        public static void OrganizeEnvironmentAssets()
        {
            EnsureFolderTree();
            MoveKnownModels();
            CreatePropsPrefabsWithMaterialsAndColliders();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("[EnvironmentSetupTool] Listo: estructura de carpetas + prefabs de props + materiales + colliders.");
        }

        private static void EnsureFolderTree()
        {
            EnsureFolder("Assets", "Art");
            EnsureFolder(ArtRoot, "Models");
            EnsureFolder(ModelsRoot, "Props");
            EnsureFolder(ModelsRoot, "Characters");
            EnsureFolder(ArtRoot, "Materials");
            EnsureFolder(MaterialsRoot, "Props");
            EnsureFolder("Assets", "Prefabs");
            EnsureFolder(PrefabsRoot, "Props");
        }

        private static void MoveKnownModels()
        {
            MoveModelIfExists("Assets/Models/Armchair.fbx", $"{PropsModelsRoot}/Armchair.fbx");
            MoveModelIfExists("Assets/Models/Chair.fbx", $"{PropsModelsRoot}/Chair.fbx");
            MoveModelIfExists("Assets/Models/Forniture1.fbx", $"{PropsModelsRoot}/Forniture1.fbx");
            MoveModelIfExists("Assets/Models/Table.fbx", $"{PropsModelsRoot}/Table.fbx");

            // Opcional: personaje/brazos en carpeta de characters.
            MoveModelIfExists("Assets/Models/Brazos_player.fbx", $"{CharactersModelsRoot}/Brazos_player.fbx");
        }

        private static void MoveModelIfExists(string sourcePath, string destinationPath)
        {
            if (AssetExists(destinationPath))
            {
                return;
            }

            if (!AssetExists(sourcePath))
            {
                return;
            }

            string moveError = AssetDatabase.MoveAsset(sourcePath, destinationPath);
            if (!string.IsNullOrWhiteSpace(moveError))
            {
                Debug.LogWarning($"[EnvironmentSetupTool] No se pudo mover {sourcePath} -> {destinationPath}. Error: {moveError}");
            }
        }

        private static void CreatePropsPrefabsWithMaterialsAndColliders()
        {
            foreach (string modelName in PropModelNames)
            {
                string modelPath = $"{PropsModelsRoot}/{modelName}.fbx";
                if (!AssetExists(modelPath))
                {
                    continue;
                }

                Dictionary<string, Material> extractedMaterials = ExtractEmbeddedMaterials(modelPath, modelName);
                CreateOrUpdatePropPrefab(modelPath, modelName, extractedMaterials);
            }
        }

        private static Dictionary<string, Material> ExtractEmbeddedMaterials(string modelPath, string modelName)
        {
            var result = new Dictionary<string, Material>(StringComparer.OrdinalIgnoreCase);
            UnityEngine.Object[] allAssets = AssetDatabase.LoadAllAssetsAtPath(modelPath);

            foreach (Material embeddedMaterial in allAssets.OfType<Material>())
            {
                string safeMaterialName = SanitizeFileName(embeddedMaterial.name);
                string materialPath = $"{PropsMaterialsRoot}/{modelName}_{safeMaterialName}.mat";
                Material existing = AssetDatabase.LoadAssetAtPath<Material>(materialPath);

                if (existing != null)
                {
                    existing.CopyPropertiesFromMaterial(embeddedMaterial);
                    EditorUtility.SetDirty(existing);
                    result[embeddedMaterial.name] = existing;
                }
                else
                {
                    var externalMaterial = new Material(embeddedMaterial)
                    {
                        name = $"{modelName}_{embeddedMaterial.name}"
                    };

                    AssetDatabase.CreateAsset(externalMaterial, materialPath);
                    result[embeddedMaterial.name] = externalMaterial;
                }
            }

            return result;
        }

        private static void CreateOrUpdatePropPrefab(string modelPath, string modelName, Dictionary<string, Material> materialsByName)
        {
            GameObject modelAsset = AssetDatabase.LoadAssetAtPath<GameObject>(modelPath);
            if (modelAsset == null)
            {
                return;
            }

            var instance = PrefabUtility.InstantiatePrefab(modelAsset) as GameObject;
            if (instance == null)
            {
                return;
            }

            try
            {
                instance.name = modelName;
                instance.transform.position = Vector3.zero;
                instance.transform.rotation = Quaternion.identity;

                AssignExtractedMaterials(instance, materialsByName);
                EnsureBasicCollider(instance);

                string prefabPath = $"{PropsPrefabsRoot}/{modelName}.prefab";
                PrefabUtility.SaveAsPrefabAsset(instance, prefabPath);
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(instance);
            }
        }

        private static void AssignExtractedMaterials(GameObject root, Dictionary<string, Material> materialsByName)
        {
            if (materialsByName == null || materialsByName.Count == 0)
            {
                return;
            }

            Renderer[] renderers = root.GetComponentsInChildren<Renderer>(true);
            foreach (Renderer renderer in renderers)
            {
                Material[] sharedMaterials = renderer.sharedMaterials;
                bool changed = false;

                for (int i = 0; i < sharedMaterials.Length; i++)
                {
                    Material current = sharedMaterials[i];
                    if (current == null)
                    {
                        continue;
                    }

                    if (materialsByName.TryGetValue(current.name, out Material extracted))
                    {
                        sharedMaterials[i] = extracted;
                        changed = true;
                    }
                }

                if (changed)
                {
                    renderer.sharedMaterials = sharedMaterials;
                    EditorUtility.SetDirty(renderer);
                }
            }
        }

        private static void EnsureBasicCollider(GameObject root)
        {
            if (root.GetComponent<Collider>() != null)
            {
                return;
            }

            Bounds? combinedBounds = GetCombinedRendererBounds(root);
            if (!combinedBounds.HasValue)
            {
                return;
            }

            Bounds bounds = combinedBounds.Value;
            BoxCollider collider = root.AddComponent<BoxCollider>();
            collider.center = root.transform.InverseTransformPoint(bounds.center);
            collider.size = bounds.size;
        }

        private static Bounds? GetCombinedRendererBounds(GameObject root)
        {
            Renderer[] renderers = root.GetComponentsInChildren<Renderer>(true);
            if (renderers.Length == 0)
            {
                return null;
            }

            Bounds combined = renderers[0].bounds;
            for (int i = 1; i < renderers.Length; i++)
            {
                combined.Encapsulate(renderers[i].bounds);
            }

            return combined;
        }

        private static bool AssetExists(string path)
        {
            return AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path) != null || File.Exists(path);
        }

        private static void EnsureFolder(string parent, string childName)
        {
            string full = $"{parent}/{childName}";
            if (AssetDatabase.IsValidFolder(full))
            {
                return;
            }

            AssetDatabase.CreateFolder(parent, childName);
        }

        private static string SanitizeFileName(string fileName)
        {
            char[] invalid = Path.GetInvalidFileNameChars();
            foreach (char invalidChar in invalid)
            {
                fileName = fileName.Replace(invalidChar, '_');
            }

            return fileName;
        }
    }
}

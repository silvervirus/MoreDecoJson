using System.Collections.Generic;
using UnityEngine;
using Debug = UnityEngine.Debug;
using System.Diagnostics.CodeAnalysis;
using EggInfo.Loader;
using EggInfo.info;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Crafting;
using Nautilus.Handlers;
using Nautilus.Utility;
using static UWE.FreezeTime;
using PrefabUtils = Nautilus.Utility.PrefabUtils;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

namespace EggInfo.info
{
    public class BasicEggPrefab : CustomPrefab
    {
        TechType techtype;

        // Remove static keyword from the properties
        string tooltip = EggInfoData.Instance.Tooltip;
        string displayName = EggInfoData.Instance.FriendlyName;
        string internalName = EggInfoData.Instance.InternalName;
        string spriteName = EggInfoData.Instance.Spritename;
        string resourceId = EggInfoData.Instance.ResourceId;
        string objectname = EggInfoData.Instance.ObjectName;
        [SetsRequiredMembers]
        public BasicEggPrefab(string internalName, string displayName, string tooltip ,string objectname ,Atlas.Sprite spriteName)
            : base(internalName, displayName, tooltip ,spriteName)
        {
            this.SetPdaGroupCategory(TechGroup.InteriorModules, TechCategory.InteriorModule).SetBuildable();
            this.SetUnlock(TechType.Peeper);
            this.SetRecipe(new RecipeData(new Ingredient(TechType.Titanium, 3)));
            
            
        }
    }

    public class QPatch : MonoBehaviour
    {
        public void Start()
        {
            Debug.Log("Awake() method called.");

            StartCoroutine(LoadEggRequirements());
            CreateCustomGroups();
           
            Debug.Log("Awake() method execution completed.");
        }

        public IEnumerator LoadEggRequirements()
        {
            RequirementsLoader requirementsLoader = new RequirementsLoader();
            Dictionary<string, EggInfoData> eggInfoData = requirementsLoader.LoadEggInfo("DecoItems.json");

            if (eggInfoData != null)
            {
                foreach (var kvp in eggInfoData)
                {
                    string eggName = kvp.Key;
                    EggInfoData eggData = kvp.Value;
                    Debug.Log($"Processing egg: {eggName}");
                    Debug.Log($"Egg data: {eggData}");

                    // Create custom egg prefab
                    TechType techType;
                    BasicEggPrefab prefab = new BasicEggPrefab(eggData.InternalName, eggData.FriendlyName, eggData.Tooltip, eggData.ObjectName, RamuneLib.Utils.ImageUtils.GetSprite(eggData.Spritename));
                    techType = prefab.Info.TechType;

                    // Create CloneTemplate using ResourceId
                    CloneTemplate cloneTemplate = new CloneTemplate(prefab.Info, eggData.ResourceId);

                    // Modify the CloneTemplate if needed
                    cloneTemplate.ModifyPrefab += obj =>
                    {
                        // allow it to be placed inside bases and submarines on the ground, and can be rotated:
                        ConstructableFlags constructableFlags = ConstructableFlags.Inside | ConstructableFlags.Rotatable | ConstructableFlags.Ground | ConstructableFlags.Submarine | ConstructableFlags.Rotatable | ConstructableFlags.Wall;
                        ConstructableBounds constructableBounds = obj.AddComponent<ConstructableBounds>();
                        GameObject model = obj.transform.Find(eggData.ObjectName)?.gameObject;
                        PrefabUtils.AddConstructable(obj, prefab.Info.TechType, constructableFlags, model);

                        if (model == null)
                        {
                            Debug.LogError($"Failed to find GameObject with name '{eggData.ObjectName}' for Deco '{eggName}'.");
                             // Exit the coroutine or handle the error accordingly
                        }

                        // Log the GameObject found by the name
                        Debug.Log($"Found GameObject with name '{eggData.ObjectName}' for Deco '{eggName}': {model.name}");

                        // Handle different cases based on eggData properties
                        if (!eggData.IsArtifact && !eggData.IsTable && !eggData.IsAnArtifact)
                        {
                            // Log a message if both IsArtifact and IsTable are false
                            Debug.Log("Both IsArtifact and IsTable are false. This is expected behavior.");
                        }
                        else if (!eggData.IsAnArtifact)
                        {
                            // Configure object if it's not an artifact
                            obj.GetComponentInChildren<Animator>().enabled = false;

                            // Remove unnecessary components
                            GameObject.DestroyImmediate(obj.GetComponent<SkyApplier>());

                            // Apply shaders and collider
                            Nautilus.Utility.MaterialUtils.ApplySNShaders(obj);
                            GameObject.DestroyImmediate(obj.GetComponent<CapsuleCollider>());
                            var collider = obj.AddComponent<BoxCollider>();
                            collider.size = new Vector3(0.5f, 0.6f, 0.5f);
                            collider.center = new Vector3(0f, 0.3f, 0f);
                            collider.isTrigger = true;
                        }
                        else if (!eggData.IsArtifact)
                        {
                            // Configure object if it's not an artifact
                            GameObject.DestroyImmediate(obj.GetComponent<SkyApplier>());
                            Nautilus.Utility.MaterialUtils.ApplySNShaders(obj);
                            GameObject.DestroyImmediate(obj.GetComponent<CapsuleCollider>());
                            var collider = obj.AddComponent<BoxCollider>();
                            collider.size = new Vector3(0.5f, 0.6f, 0.5f);
                            collider.center = new Vector3(0f, 0.3f, 0f);
                            collider.isTrigger = true;
                        }
                        else if (!eggData.IsAnArtifact)
                        {
                            // Configure object if it's not an artifact
                            obj.GetComponentInChildren<Animator>().enabled = false;

                            // Remove unnecessary components
                            GameObject.DestroyImmediate(obj.GetComponent<SkyApplier>());

                            // Apply shaders and collider
                            Nautilus.Utility.MaterialUtils.ApplySNShaders(obj);
                            GameObject.DestroyImmediate(obj.GetComponent<CapsuleCollider>());
                            var collider = obj.AddComponent<BoxCollider>();
                            collider.size = new Vector3(0.5f, 0.6f, 0.5f);
                            collider.center = new Vector3(0f, 0.3f, 0f);
                            collider.isTrigger = true;
                        }
                        else if (!eggData.IsTable)
                        {
                            // Adjust scale if it's not a table
                            var cube = obj.transform.Find("Cube");
                            cube.localScale = Vector3.one / 1000; // Adjust scale as needed

                            // Adjust model scale
                            model.transform.localScale = Vector3.one / 1000; // Adjust scale as needed
                        }
                        else
                        {
                            // Apply shaders if neither condition is met
                            GameObject.DestroyImmediate(obj.GetComponent<SkyApplier>());
                            Nautilus.Utility.MaterialUtils.ApplySNShaders(obj);
                        }

                        // Add the prefab to the game world

                    };

                    prefab.SetGameObject(cloneTemplate);
                    prefab.SetPdaGroupCategory(TechGroup.InteriorModules, TechCategory.InteriorModule).SetBuildable();
                    prefab.SetUnlock(TechType.Peeper);
                    prefab.SetRecipe(new RecipeData(new Ingredient(TechType.Titanium, 3)));
                    prefab.Register();

                    // Pause the coroutine for a frame
                    yield return null;
                }
            }
            else
            {
                Debug.LogError("Failed to load egg requirements from JSON.");
            }
        }



        private void CreateCustomGroups()
        {
            // Create or get the tech group
            var groupName = "MoreDeco";
            TechGroup group;
            if (!EnumHandler.TryGetValue(groupName, out group))
            {
                group = EnumHandler.AddEntry<TechGroup>(groupName).WithPdaInfo($"MoreDeco");
            }

            // Create custom tech categories
            CreateCustomCategories(group);
        }

        private void CreateCustomCategories(TechGroup group)
        {
            RequirementsLoader requirementsLoader = new RequirementsLoader();
            Dictionary<string, EggInfoData> eggInfoData = requirementsLoader.LoadEggInfo("egg_requirements.json");

            if (eggInfoData != null)
            {
                foreach (var kvp in eggInfoData)
                {
                    EggInfoData eggData = kvp.Value;

                    // Generate a descriptive category name using the eggData's InternalName
                    string categoryName = $"Deco{eggData.InternalName}";

                    // Check if the category already exists
                    if (!EnumHandler.TryGetValue<TechCategory>(categoryName, out var category))
                    {
                        // Create and register the category
                        category = EnumHandler.AddEntry<TechCategory>(categoryName).WithPdaInfo($"{eggData.InternalName} Deco").RegisterToTechGroup(group);
                    }
                }
            }
            else
            {
                Debug.LogError("Failed to load egg requirements from JSON.");
            }
        }
    }
}
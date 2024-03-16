using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using EggInfo.Loader;
using EggInfo.info;
using Debug = UnityEngine.Debug;
using Nautilus.Utility;
using static HandReticle;
using PrefabUtils = Nautilus.Utility.PrefabUtils;

[BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
[BepInDependency("com.snmodding.nautilus", BepInDependency.DependencyFlags.HardDependency)]
public class QPatch : BaseUnityPlugin
{
    public const string PLUGIN_GUID = "SN_MoreDecoJson";
    public const string PLUGIN_NAME = "MoreDecoJson";
    public const string PLUGIN_VERSION = "1.0.0";


   public void Awake()
    {
        LoadEggRequirements();
        
        
    }

    private void LoadEggRequirements()
    {
        RequirementsLoader requirementsLoader = new RequirementsLoader();
        Dictionary<string, EggInfoData> eggInfoData = requirementsLoader.LoadEggInfo("DecoItems.json");

        if (eggInfoData != null)
        {
            foreach (var eggEntry in eggInfoData)
            {
                try
                {
                    string eggName = eggEntry.Key;
                    EggInfoData eggData = eggEntry.Value;

                    // Debug output for egg data
                    Debug.Log($"DecoName: {eggName}");
                    Debug.Log($"Friendly Name: {eggData.FriendlyName}");
                    Debug.Log($"Tooltip: {eggData.Tooltip}");
                    Debug.Log($"Internal Name: {eggData.InternalName}");
                    Debug.Log($"Sprite Name: {eggData.Spritename}");
                    Debug.Log($"Object Name: {eggData.ObjectName}");

                    // Extract egg information from eggData
                    string eggDisplayName = eggData.FriendlyName;
                    string eggDescription = eggData.Tooltip;
                    string eggInterName = eggData.InternalName;
                    string eggspriteName = eggData.Spritename;
                    string eggObjectName = eggData.ObjectName;

                    // Convert eggData.TechType to TechType to get the sprite
                    TechType eggTechType = GetTechType(eggspriteName);

                    // Split internal names and create/register the custom egg prefab for each TechType
                    foreach (string internalName in eggInterName.Split(','))
                    {
                        // Fetch the sprite
                        Atlas.Sprite eggSprite = RamuneLib.Utils.ImageUtils.GetSprite(eggspriteName);
                        // Create and register the custom egg prefab
                        BasicEggPrefab prefab = new BasicEggPrefab(internalName.Trim(), eggDisplayName, eggDescription,eggObjectName,RamuneLib.Utils.ImageUtils.GetSprite(eggspriteName));
                        

                        // Create CloneTemplate using ResourceId
                        CloneTemplate cloneTemplate = new CloneTemplate(prefab.Info, eggData.ResourceId);

                        // Modify the CloneTemplate if needed
                        cloneTemplate.ModifyPrefab += obj =>
                        {
                            // allow it to be placed inside bases and submarines on the ground, and can be rotated:
                            ConstructableFlags constructableFlags = ConstructableFlags.Inside | ConstructableFlags.Rotatable | ConstructableFlags.Ground | ConstructableFlags.Submarine | ConstructableFlags.Rotatable | ConstructableFlags.Wall;
                            ConstructableBounds constructableBounds = obj.AddComponent<ConstructableBounds>();
                            constructableBounds.bounds.size *= 0.9f;
                            GameObject model = obj.transform.Find(eggData.ObjectName)?.gameObject;
                           

                            if (model == null)
                            {
                                Debug.LogError($"Failed to find GameObject with name '{eggData.ObjectName}' for Deco '{eggName}'.");
                                // Exit the coroutine or handle the error accordingly
                            }

                            // Log the GameObject found by the name
                            Debug.Log($"Found GameObject with name '{eggData.ObjectName}' for Deco '{eggName}': {model.name}");

                            // Handle different cases based on eggData properties
                           
                             if (eggData.IsAnArtifact == true)
                            {
                               
                                obj.GetComponentInChildren<Animator>().enabled = false;
                                GameObject.DestroyImmediate(obj.GetComponent<SkyApplier>());
                                Nautilus.Utility.MaterialUtils.ApplySNShaders(obj);
                                GameObject.DestroyImmediate(obj.GetComponent<CapsuleCollider>());
                                var collider = obj.AddComponent<BoxCollider>();
                                collider.size = new Vector3(0.5f, 0.6f, 0.5f);
                                collider.center = new Vector3(0f, 0.3f, 0f);
                                collider.isTrigger = true;
                            }
                            else if (eggData.IsGunArtifact == true)
                            {

                                obj.transform.localPosition = new Vector3(0f, 1.85f, 0f);
                                obj.GetComponentInChildren<Animator>().enabled = false;
                                GameObject.DestroyImmediate(obj.GetComponent<EntityTag>());
                                GameObject.DestroyImmediate(obj.GetComponent<CapsuleCollider>());
                                GameObject.DestroyImmediate(obj.GetComponent<SkyApplier>());
                                Nautilus.Utility.MaterialUtils.ApplySNShaders(obj);
                                var collider = obj.AddComponent<BoxCollider>();
                                collider.size = new Vector3(0.5f, 0.6f, 0.5f);
                                collider.center = new Vector3(0f, 0.3f, 0f);
                                collider.isTrigger = true;
                                obj.AddComponent<SkyApplier>();
                            }
                            else if (eggData.IsArtifact == true)
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
                            
                            else if (eggData.IsTable == true)
                            {

                                var cude = obj.transform.Find("Cube");
                                cude.localScale = Vector3.one / 002;
                                model.transform.localScale = Vector3.one / 002;
                            }
                            else if (eggData.IsBasicArtifact == true)
                            {
                                // Apply shaders if neither condition is met
                                GameObject.DestroyImmediate(obj.GetComponent<SkyApplier>());
                                Nautilus.Utility.MaterialUtils.ApplySNShaders(obj);
                            }
                            else if (eggData.NothingNeeded == true)
                            {
                               
                            }
                            else if (eggData.Pen == true)
                            {
                                Nautilus.Utility.MaterialUtils.ApplySNShaders(obj);

                                BoxCollider mainModelCollider = model.EnsureComponent<BoxCollider>();
                                mainModelCollider.center = new Vector3(0f, 0f, 0.14f); // Adjust the center to the desired position
                                mainModelCollider.size = new Vector3(0.2f, 0.2f, 0.2f); // Increase the height (Y) to make it easier to interact with the curtains
                                model.layer = LayerMask.NameToLayer("Default");
                            }
                            else if (eggData.HasStorage == true)
                            {
                                PrefabUtils.AddStorageContainer(obj, eggData.FriendlyName, eggData.InternalName, eggData.Width,eggData.Height, true);
                            }
                            PrefabUtils.AddConstructable(obj, prefab.Info.TechType, constructableFlags, model);
                            
                        };

                        prefab.SetGameObject(cloneTemplate);
                        prefab.Register();
                        TechType techType = prefab.Info.TechType;
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error creating MoreDecoJson {eggEntry.Key}: {ex.Message}");
                }
            }
        }
        else
        {
            Debug.LogError("Failed to load MoreDecoJson requirements from JSON.");
        }
    }
    


    

    private TechType GetTechType(string techTypeStr)
    {
        if (Enum.TryParse(techTypeStr, out TechType result))
        {
            return result;
        }
        // Handle error case, maybe return a default TechType
        return TechType.None;
    }
}

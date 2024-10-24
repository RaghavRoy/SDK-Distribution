using UnityEditor;
using UnityEngine;
using Unity.XR.CoreUtils;
using UnityEngine.XR.OpenXR;
using UnityEditor.XR.OpenXR.Features;
using UnityEngine.XR.Hands.OpenXR;
using Qualcomm.Snapdragon.Spaces;
using UnityEngine.XR.OpenXR.Features.MetaQuestSupport;
using QCHT.Interactions.Core;
using UnityEngine.XR.OpenXR.Features.Interactions;

public class TargetDeviceSwitcher : Editor
{
    private const string PrefabsFolderPath = "Prefabs/";

    [MenuItem("JioXSpaces/Target Device/Spaces")]
    public static void ChangeToSkyworth()
    {
        //SwitchPrefab("SkyworthOrigin");
        ToggleFeaturesForSkyworth(true);
    }

    [MenuItem("JioXSpaces/Target Device/Oculus")]
    public static void ChangeToOculus()
    {
        //SwitchPrefab("OculusOrigin");
        ToggleFeaturesForSkyworth(false);
    }

    private static void SwitchPrefab(string prefabName)
    {
        // Load the prefab
        GameObject prefab = Resources.Load<GameObject>(PrefabsFolderPath + prefabName);
        if (prefab == null)
        {
            Debug.LogError($"Prefab {prefabName} not found in {PrefabsFolderPath}");
            return;
        }

        // Find and destroy the existing prefab with XROrigin
        XROrigin existingXROrigin = FindObjectOfType<XROrigin>();
        if (existingXROrigin != null)
        {
            DestroyImmediate(existingXROrigin.gameObject);
        }

        // Instantiate the new prefab
        PrefabUtility.InstantiatePrefab(prefab);
    }

    private static void ToggleFeaturesForSkyworth(bool status)
    {
        // Enable/disable Skyworth features
        OpenXRFeatureSetManager.GetFeatureSetWithId(BuildTargetGroup.Android, "com.jiox.skyworth").isEnabled = status;
        OpenXRSettings.GetSettingsForBuildTargetGroup(BuildTargetGroup.Android).GetFeature<BaseRuntimeFeature>().enabled = status;
        OpenXRSettings.GetSettingsForBuildTargetGroup(BuildTargetGroup.Android).GetFeature<HandTrackingFeature>().enabled = status;
        OpenXRSettings.GetSettingsForBuildTargetGroup(BuildTargetGroup.Android).GetFeature<HandCommonPosesInteraction>().enabled = status;

        // Enable/disable Oculus features
        OpenXRFeatureSetManager.GetFeatureSetWithId(BuildTargetGroup.Android, "com.jiox.oculus").isEnabled = !status;
        OpenXRSettings.GetSettingsForBuildTargetGroup(BuildTargetGroup.Android).GetFeature<MetaQuestFeature>().enabled = !status;
        OpenXRSettings.GetSettingsForBuildTargetGroup(BuildTargetGroup.Android).GetFeature<MetaHandTrackingAim>().enabled = !status;
        OpenXRSettings.GetSettingsForBuildTargetGroup(BuildTargetGroup.Android).GetFeature<HandTracking>().enabled = true;

        UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
    }
}
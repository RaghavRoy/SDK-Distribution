using UnityEditor;
using UnityEditor.XR.OpenXR.Features;
using UnityEngine.XR.Hands.OpenXR;


[OpenXRFeatureSet(
    FeatureIds = new string[] {   // The list of features that this feature group is defined for.
            "com.unity.openxr.feature.input.handtracking",
            MetaHandTrackingAim.featureId,
            "com.unity.openxr.feature.metaquest"
        },
    UiName = "Oculus",
    Description = "Feature group that allows for setting up the best environment for Oculus",
    // Unique ID for this feature group
    FeatureSetId = "com.jiox.oculus",
    SupportedBuildTargets = new BuildTargetGroup[] { BuildTargetGroup.Android }
)]
class OculusFeatureSet
{
}

[OpenXRFeatureSet(
    FeatureIds = new string[] {   // The list of features that this feature group is defined for.
            "com.unity.openxr.feature.input.handtracking",
            "com.qualcomm.snapdragon.spaces.base",
            "com.qualcomm.snapdragon.spaces.handtracking",
            "com.unity.openxr.feature.input.handinteractionposes"
        },
    UiName = "Skyworth",
    Description = "Feature group that allows for setting up the best environment for Skyworth",
    // Unique ID for this feature group
    FeatureSetId = "com.jiox.skyworth",
    SupportedBuildTargets = new BuildTargetGroup[] { BuildTargetGroup.Android }
)]
class SkyworthFeatureSet
{
}

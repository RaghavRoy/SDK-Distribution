using UnityEditor;
using UnityEngine;

public class XRSetupSwitcher : EditorWindow
{
    private GameObject openXRRig;
    private GameObject spacesRig;
    private GameObject openXRHandsPrefab;
    private GameObject openXRControllersPrefab;
    private GameObject spacesHandsPrefab;
    private GameObject spacesControllersPrefab;

    [MenuItem("Tools/XR Setup Switcher")]
    public static void ShowWindow()
    {
        GetWindow<XRSetupSwitcher>("XR Setup Switcher");
    }

    private void OnGUI()
    {
        GUILayout.Label("XR Setup Switcher", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.BeginVertical();
            {
                openXRRig = (GameObject)EditorGUILayout.ObjectField("OpenXR Rig", openXRRig, typeof(GameObject), true);
                openXRHandsPrefab = (GameObject)EditorGUILayout.ObjectField("OpenXR Hands Prefab", openXRHandsPrefab, typeof(GameObject), false);
                openXRControllersPrefab = (GameObject)EditorGUILayout.ObjectField("OpenXR Controllers Prefab", openXRControllersPrefab, typeof(GameObject), false);
                if (GUILayout.Button("Switch to OpenXR"))
                {
                    SwitchToOpenXR();
                }
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
            {
                spacesRig = (GameObject)EditorGUILayout.ObjectField("Spaces Rig", spacesRig, typeof(GameObject), true);
                spacesHandsPrefab = (GameObject)EditorGUILayout.ObjectField("Spaces Hands Prefab", spacesHandsPrefab, typeof(GameObject), false);
                spacesControllersPrefab = (GameObject)EditorGUILayout.ObjectField("Spaces Controllers Prefab", spacesControllersPrefab, typeof(GameObject), false);
                if (GUILayout.Button("Switch to Spaces SDK"))
                {
                    SwitchToSpaces();
                }
            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndHorizontal();
    }

    private void SwitchToOpenXR()
    {
        if (spacesRig != null)
        {
            spacesRig.SetActive(false);
        }

        if (openXRRig != null)
        {
            openXRRig.SetActive(true);
            ReplaceHandsAndControllers(openXRRig, openXRHandsPrefab, openXRControllersPrefab);
        }
    }

    private void SwitchToSpaces()
    {
        if (openXRRig != null)
        {
            openXRRig.SetActive(false);
        }

        if (spacesRig != null)
        {
            spacesRig.SetActive(true);
            ReplaceHandsAndControllers(spacesRig, spacesHandsPrefab, spacesControllersPrefab);
        }
    }

    private void ReplaceHandsAndControllers(GameObject rig, GameObject handsPrefab, GameObject controllersPrefab)
    {
        if (rig == null)
        {
            Debug.LogError("XR Rig is not assigned.");
            return;
        }

        Transform handsTransform = rig.transform.Find("Hands");
        Transform controllersTransform = rig.transform.Find("Controllers");

        if (handsTransform != null && handsPrefab != null)
        {
            DestroyImmediate(handsTransform.gameObject);
            Instantiate(handsPrefab, rig.transform);
        }

        if (controllersTransform != null && controllersPrefab != null)
        {
            DestroyImmediate(controllersTransform.gameObject);
            Instantiate(controllersPrefab, rig.transform);
        }
    }
}

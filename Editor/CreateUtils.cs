using System;
using System.Collections.Generic;
using System.ComponentModel;
using JetBrains.Annotations;
using Unity.XR.CoreUtils;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
using UnityEngine.XR.Interaction.Toolkit.Transformers;
using UnityEngine.XR.Interaction.Toolkit.UI;
using UnityEditor;
using Object = UnityEngine.Object;
using UnityEngine.XR.Interaction.Toolkit.Samples.Hands;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;
using JioXSDK.Interactions;
using System.Reflection;
using UnityEngine.Events;

namespace JioXSDK
{
    static class CreateUtils
    {
        internal enum HardwareTarget
        {
            AR,
            VR,
        }

        internal enum InputType
        {
            ActionBased,
            DeviceBased,
        }

        internal enum HandType
        {
            Left,
            Right,
        }

        const string jioXRFolderPath = "GameObject/JioXR/";

        const string k_LineMaterial = "Default-Line.mat";
        const string k_UILayerName = "UI";

       /* [MenuItem("GameObject/JioXR/Ray Interactor (Action-based)", false, 10), UsedImplicitly]
#pragma warning disable IDE0051 // Remove unused private members -- Editor Menu Item
        public static void CreateRayInteractorActionBased(MenuCommand menuCommand)
#pragma warning restore IDE0051
        {
            CreateInteractionManager();

            Finalize(CreateRayInteractor(menuCommand?.GetContextTransform(), InputType.ActionBased));
        }

        [MenuItem("GameObject/JioXR/Device-based/Ray Interactor", false, 10), UsedImplicitly]
#pragma warning disable IDE0051 // Remove unused private members -- Editor Menu Item
        public static void CreateRayInteractorDeviceBased(MenuCommand menuCommand)
#pragma warning restore IDE0051
        {
            CreateInteractionManager();

            Finalize(CreateRayInteractor(menuCommand?.GetContextTransform(), InputType.DeviceBased));
        }

        [MenuItem("GameObject/JioXR/Direct Interactor (Action-based)", false, 10), UsedImplicitly]
#pragma warning disable IDE0051 // Remove unused private members -- Editor Menu Item
        public static void CreateDirectInteractorActionBased(MenuCommand menuCommand)
#pragma warning restore IDE0051
        {
            CreateInteractionManager();

            Finalize(CreateDirectInteractor(menuCommand?.GetContextTransform(), InputType.ActionBased));
        }

        [MenuItem("GameObject/JioXR/Device-based/Direct Interactor", false, 10), UsedImplicitly]
#pragma warning disable IDE0051 // Remove unused private members -- Editor Menu Item
        public static void CreateDirectInteractorDeviceBased(MenuCommand menuCommand)
#pragma warning restore IDE0051
        {
            CreateInteractionManager();

            Finalize(CreateDirectInteractor(menuCommand?.GetContextTransform(), InputType.DeviceBased));
        }
        
        [MenuItem("GameObject/JioXR/Gaze Interactor (Action-based)", false, 10), UsedImplicitly]
#pragma warning disable IDE0051 // Remove unused private members -- Editor Menu Item
        public static void CreateGazeInteractorActionBased(MenuCommand menuCommand)
#pragma warning restore IDE0051
        {
            CreateInteractionManager();

            Finalize(CreateGazeInteractor(menuCommand?.GetContextTransform(), InputType.ActionBased));
        }

        [MenuItem("GameObject/JioXR/Socket Interactor", false, 10), UsedImplicitly]
#pragma warning disable IDE0051 // Remove unused private members -- Editor Menu Item
        public static void CreateSocketInteractor(MenuCommand menuCommand)
#pragma warning restore IDE0051
        {
            CreateInteractionManager();

            var socketInteractableGO = CreateAndPlaceGameObject("Socket Interactor", menuCommand?.GetContextTransform(),
                typeof(SphereCollider),
                typeof(XRSocketInteractor));

            var sphereCollider = socketInteractableGO.GetComponent<SphereCollider>();
            sphereCollider.isTrigger = true;
            sphereCollider.radius = GetScaledRadius(sphereCollider, 0.1f);
            Finalize(socketInteractableGO);
        }

        [MenuItem("GameObject/JioXR/Grab Interactable", false, 10), UsedImplicitly]
#pragma warning disable IDE0051 // Remove unused private members -- Editor Menu Item
        public static void CreateGrabInteractable(MenuCommand menuCommand)
#pragma warning restore IDE0051
        {
            CreateInteractionManager();

            var grabInteractableGO = CreateAndPlacePrimitive("Grab Interactable", menuCommand?.GetContextTransform(),
                PrimitiveType.Cube,
                typeof(XRGrabInteractable), typeof(XRGeneralGrabTransformer));

            var transform = grabInteractableGO.transform;
            var localScale = InverseTransformScale(transform, new Vector3(0.1f, 0.1f, 0.1f));
            transform.localScale = Abs(localScale);

            var boxCollider = grabInteractableGO.GetComponent<BoxCollider>();
            // BoxCollider does not support a negative effective size,
            // so ensure the size accounts for any negative scaling.
            boxCollider.size = Vector3.Scale(boxCollider.size, Sign(localScale));

            var rigidbody = grabInteractableGO.GetComponent<Rigidbody>();
            // Enable interpolation on the Rigidbody to smooth movement
            rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            
            Finalize(grabInteractableGO);
        }

        [MenuItem("GameObject/JioXR/Interactable Snap Volume", false, 10), UsedImplicitly]
#pragma warning disable IDE0051 // Remove unused private members -- Editor Menu Item
        public static void CreateInteractableSnapVolume(MenuCommand menuCommand)
#pragma warning restore IDE0051
        {
            CreateInteractionManager();

            var snapVolumeGO = CreateAndPlaceGameObject("Interactable Snap Volume", menuCommand?.GetContextTransform(),
                typeof(SphereCollider),
                typeof(XRInteractableSnapVolume));

            var sphereCollider = snapVolumeGO.GetComponent<SphereCollider>();
            sphereCollider.isTrigger = true;
            sphereCollider.radius = GetScaledRadius(sphereCollider, 0.2f);

            // The Reset method will not find the Interactable up the hierarchy because it runs before being re-parented,
            // so the initialization of the property is repeated here.
            var snapVolume = snapVolumeGO.GetComponent<XRInteractableSnapVolume>();
            var interactable = snapVolumeGO.GetComponentInParent<IXRInteractable>();
            snapVolume.interactableObject = interactable as Object;
            if (snapVolume.interactableObject != null)
            {
                var col = interactable.transform.GetComponent<Collider>();
                if (col != null && col.enabled && !col.isTrigger)
                    snapVolume.snapToCollider = col;
            }

            Finalize(snapVolumeGO);
        }

        [MenuItem("GameObject/JioXR/Interaction Manager", false, 10), UsedImplicitly]
#pragma warning disable IDE0051 // Remove unused private members -- Editor Menu Item
        public static void CreateInteractionManager(MenuCommand menuCommand)
#pragma warning restore IDE0051
        {
            Finalize(CreateInteractionManager(menuCommand?.GetContextTransform()));
        }

        [MenuItem("GameObject/JioXR/Locomotion System (Action-based)", false, 10), UsedImplicitly]
#pragma warning disable IDE0051 // Remove unused private members -- Editor Menu Item
        public static void CreateLocomotionSystemActionBased(MenuCommand menuCommand)
#pragma warning restore IDE0051
        {
            Finalize(CreateLocomotionSystem(menuCommand?.GetContextTransform(), InputType.ActionBased));
        }

        [MenuItem("GameObject/JioXR/Device-based/Locomotion System", false, 10), UsedImplicitly]
#pragma warning disable IDE0051 // Remove unused private members -- Editor Menu Item
        public static void CreateLocomotionSystemDeviceBased(MenuCommand menuCommand)
#pragma warning restore IDE0051
        {
            Finalize(CreateLocomotionSystem(menuCommand?.GetContextTransform(), InputType.DeviceBased));
        }

        [MenuItem("GameObject/JioXR/Teleportation Area", false, 10), UsedImplicitly]
#pragma warning disable IDE0051 // Remove unused private members -- Editor Menu Item
        public static void CreateTeleportationArea(MenuCommand menuCommand)
#pragma warning restore IDE0051
        {
            CreateInteractionManager();

            Finalize(CreateAndPlacePrimitive("Teleportation Area", menuCommand?.GetContextTransform(),
                PrimitiveType.Plane,
                typeof(TeleportationArea)));
        }

        [MenuItem("GameObject/JioXR/Teleportation Anchor", false, 10), UsedImplicitly]
#pragma warning disable IDE0051 // Remove unused private members -- Editor Menu Item
        public static void CreateTeleportationAnchor(MenuCommand menuCommand)
#pragma warning restore IDE0051
        {
            CreateInteractionManager();

            var anchorGO = CreateAndPlacePrimitive("Teleportation Anchor", menuCommand?.GetContextTransform(),
                PrimitiveType.Plane,
                typeof(TeleportationAnchor));

            var destinationGO = ObjectFactory.CreateGameObject("Anchor");
            Place(destinationGO, anchorGO.transform);

            var teleportationAnchor = anchorGO.GetComponent<TeleportationAnchor>();
            teleportationAnchor.teleportAnchorTransform = destinationGO.transform;
            Finalize(anchorGO);
        }
       */

        [MenuItem("GameObject/JioXR/UI Canvas", false, 10), UsedImplicitly]
#pragma warning disable IDE0051 // Remove unused private members -- Editor Menu Item
        public static void CreateXRUICanvas(MenuCommand menuCommand)
#pragma warning restore IDE0051
        {
            var parentOfNewGameObject = menuCommand?.GetContextTransform();

            var currentStage = StageUtility.GetCurrentStageHandle();
            var editingPrefabStage = currentStage != StageUtility.GetMainStageHandle();

            var canvasGO = CreateAndPlaceGameObject("Canvas", parentOfNewGameObject,
                typeof(Canvas),
                typeof(CanvasScaler),
                typeof(GraphicRaycaster),
                typeof(TrackedDeviceGraphicRaycaster));

            // Either inherit the layer of the parent object, or use the same default that GameObject/UI/Canvas uses.
            if (parentOfNewGameObject == null)
                canvasGO.layer = LayerMask.NameToLayer(k_UILayerName);

            var canvas = canvasGO.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;

            if (!editingPrefabStage)
                canvas.worldCamera = Camera.main;
            else
                Debug.LogWarning("You have just added an XR UI Canvas to a prefab." +
                    " To function properly with an XR Ray Interactor, you must also set the Canvas component's Event Camera in your scene.",
                    canvasGO);

            // Ensure there is at least one EventSystem setup properly
            var inputModule = currentStage.FindComponentOfType<XRUIInputModule>();
            if (inputModule == null || !inputModule.gameObject.scene.IsValid())
            {
                if (!editingPrefabStage)
                    CreateXRUIEventSystemWithParent(parentOfNewGameObject, out _);
                else
                    Debug.LogWarning("You have just added an XR UI Canvas to a prefab." +
                        " To function properly with an XR Ray Interactor, you must also add an XR UI Event System to your scene.",
                        canvasGO);
            }

            Finalize(canvasGO);
        }

        [MenuItem("GameObject/JioXR/UI Event System", false, 10), UsedImplicitly]
#pragma warning disable IDE0051 // Remove unused private members -- Editor Menu Item
        public static void CreateXRUIEventSystem(MenuCommand menuCommand)
#pragma warning restore IDE0051
        {
            var eventSystemGO = CreateXRUIEventSystemWithParent(menuCommand?.GetContextTransform(), out var changeSelectionOnly);

            // If there was no serialization change (it already existed), only update the selection.
            // Passing it to Undo.RegisterCreatedObjectUndo in Finalize would cause the GameObject to be destroyed
            // upon Undo, which should not happen. This matches the behavior of GameObject > UI > Event System.
            if (changeSelectionOnly)
                Selection.activeGameObject = eventSystemGO;
            else
                Finalize(eventSystemGO);
        }

        [MenuItem("GameObject/JioXR/JioXR Rig", false, 10), UsedImplicitly]
        public static void CreateXROriginForVR(MenuCommand menuCommand)
        {
            Finalize(CreateJioXROriginWithParent(menuCommand?.GetContextTransform(), HardwareTarget.VR, InputType.ActionBased));
        }

       /* [MenuItem("GameObject/JioXR/Device-based/JioXR Rig", false, 10), UsedImplicitly]
        public static void CreateXROriginDeviceBased(MenuCommand menuCommand)
        {
            Finalize(CreateJioXROriginWithParent(menuCommand?.GetContextTransform(), HardwareTarget.VR, InputType.DeviceBased));
        }*/

        /// <summary>
        /// Registers <paramref name="gameObject"/> on the Undo stack as the root of a newly created GameObject hierarchy and selects it.
        /// Components on <paramref name="gameObject"/> and its children, if destroyed and recreated via Undo/Redo, will be recreated
        /// in their state from when this method was called.
        /// </summary>
        /// <param name="gameObject">The newly created root GameObject.</param>
        static void Finalize(GameObject gameObject)
        {
            Undo.RegisterCreatedObjectUndo(gameObject, $"Create {gameObject.name}");
            Selection.activeGameObject = gameObject;
        }

        /// <summary>
        /// Create the <see cref="XRInteractionManager"/> if necessary.
        /// </summary>
        /// <param name="parent">The parent <see cref="Transform"/> to use.</param>
        static GameObject CreateInteractionManager(Transform parent = null)
        {
            var currentStage = StageUtility.GetCurrentStageHandle();

            var interactionManager = currentStage.FindComponentOfType<XRInteractionManager>();
            if (interactionManager == null || !interactionManager.gameObject.scene.IsValid())
                return CreateAndPlaceGameObject("XR Interaction Manager", parent, typeof(XRInteractionManager));

            return interactionManager.gameObject;
        }

        static GameObject CreateLocomotionSystem(Transform parent, InputType inputType, string name = "Locomotion System")
        {
            var locomotionSystemGO = CreateAndPlaceGameObject(name, parent,
                typeof(LocomotionSystem),
                typeof(TeleportationProvider),
                GetSnapTurnType(inputType));

            var locomotionSystem = locomotionSystemGO.GetComponent<LocomotionSystem>();

            var teleportationProvider = locomotionSystemGO.GetComponent<TeleportationProvider>();
            teleportationProvider.system = locomotionSystem;

            var snapTurnProvider = locomotionSystemGO.GetComponent<SnapTurnProviderBase>();
            snapTurnProvider.system = locomotionSystem;
            return locomotionSystemGO;
        }

        static GameObject CreateRayInteractor(Transform parent, InputType inputType, string name = "Ray Interactor")
        {
            var rayInteractableGO = CreateAndPlaceGameObject(name, parent,
                GetControllerType(inputType),
                typeof(XRRayInteractor),
                typeof(LineRenderer),
                typeof(XRInteractorLineVisual),
                typeof(SortingGroup));

            SetupLineRenderer(rayInteractableGO.GetComponent<LineRenderer>());

            // Add a Sorting Group with a custom sorting order to make it render in front of UGUI
            var sortingGroup = rayInteractableGO.GetComponent<SortingGroup>();
            sortingGroup.sortingOrder = 5;

            return rayInteractableGO;
        }

        static GameObject CreateGazeInteractor(Transform parent, InputType inputType, string name = "Gaze Interactor")
        {
            var gazeInteractableGO = CreateAndPlaceGameObject(name, parent,
                GetControllerType(inputType),
                typeof(XRGazeInteractor),
                typeof(SortingGroup), 
                typeof(LineRenderer), typeof(XRInteractorLineVisual), typeof(XRInteractorReticleVisual));

            // Add a Sorting Group with a custom sorting order to make it render in front of UGUI
            var sortingGroup = gazeInteractableGO.GetComponent<SortingGroup>();
            sortingGroup.sortingOrder = 5;
            sortingGroup.enabled = false;

            var gazeInteractor = gazeInteractableGO.GetComponent<XRGazeInteractor>();
            gazeInteractor.allowAnchorControl = false;


            return gazeInteractableGO;
        }

       

        static void SetupLineRenderer(LineRenderer lineRenderer)
        {
            var materials = new Material[1];
            materials[0] = AssetDatabase.GetBuiltinExtraResource<Material>(k_LineMaterial);
            lineRenderer.materials = materials;
            lineRenderer.loop = false;
            lineRenderer.widthMultiplier = 0.005f;
            lineRenderer.numCornerVertices = 4;
            lineRenderer.numCapVertices = 4;
            lineRenderer.shadowCastingMode = ShadowCastingMode.Off;
            lineRenderer.receiveShadows = false;
            lineRenderer.useWorldSpace = true;
        }

        static GameObject CreateDirectInteractor(Transform parent, InputType inputType, string name = "Direct Interactor")
        {
            var directInteractorGO = CreateAndPlaceGameObject(name, parent,
                GetControllerType(inputType),
                typeof(SphereCollider),
                typeof(XRDirectInteractor));

            var sphereCollider = directInteractorGO.GetComponent<SphereCollider>();
            sphereCollider.isTrigger = true;
            sphereCollider.radius = GetScaledRadius(sphereCollider, 0.1f);

            return directInteractorGO;
        }

        static GameObject CreateXRUIEventSystemWithParent(Transform parent, out bool changeSelectionOnly)
        {
            var currentStage = StageUtility.GetCurrentStageHandle();

            var inputModule = currentStage.FindComponentOfType<XRUIInputModule>();
            if (inputModule != null && inputModule.gameObject.scene.IsValid())
            {
                changeSelectionOnly = true;
                return inputModule.gameObject;
            }

            // Ensure there is at least one EventSystem setup properly
            var eventSystem = currentStage.FindComponentOfType<EventSystem>();
            GameObject eventSystemGO;
            if (eventSystem == null || !eventSystem.gameObject.scene.IsValid())
            {
                eventSystemGO = CreateAndPlaceGameObject("EventSystem", parent,
                    typeof(EventSystem),
                    typeof(XRUIInputModule));
            }
            else
            {
                eventSystemGO = eventSystem.gameObject;

                // Remove the Standalone Input Module if already implemented, since it will block the XRUIInputModule
                var standaloneInputModule = eventSystemGO.GetComponent<StandaloneInputModule>();
                if (standaloneInputModule != null)
                    Undo.DestroyObjectImmediate(standaloneInputModule);

                Undo.AddComponent<XRUIInputModule>(eventSystemGO);
            }

            changeSelectionOnly = false;
            return eventSystemGO;
        }

        static GameObject CreateXROriginWithParent(Transform parent, HardwareTarget target, InputType inputType)
        {
            CreateInteractionManager();

            var originGo = CreateAndPlaceGameObject("XR Origin (XR Rig)", parent, typeof(XROrigin));
            var offsetGo = CreateAndPlaceGameObject("Camera Offset", originGo.transform);
            var offsetTransform = offsetGo.transform;

            var xrCamera = XRMainCameraFactory.CreateXRMainCamera(target, inputType);
            Place(xrCamera.gameObject, offsetTransform);

            var origin = originGo.GetComponent<XROrigin>();
            origin.CameraFloorOffsetObject = offsetGo;
            origin.Camera = xrCamera;

            // Set the Camera Offset y position based on the default height.
            // This will make the Scene view of the Camera when not in Play mode more closely match
            // what the position will be when entering Play mode. In Device mode, it will be this value.
            // In Floor mode, it will get reset to 0, but will at least be higher than the XROrigin position.
            var desiredPosition = offsetTransform.localPosition;
            desiredPosition.y = origin.CameraYOffset;
            offsetGo.transform.localPosition = desiredPosition;

            AddXRControllersToOrigin(origin, inputType);

            if (inputType == InputType.ActionBased)
            {
                var inputActionManager = originGo.AddComponent<InputActionManager>();

                const string assetName = "XRI Default Input Actions";
                const string searchFilter = "\"" + assetName + "\" t:InputActionAsset";
                foreach (var guid in AssetDatabase.FindAssets(searchFilter))
                {
                    var path = AssetDatabase.GUIDToAssetPath(guid);
                    var asset = AssetDatabase.LoadAssetAtPath<InputActionAsset>(path);
                    // The search filter string will return all assets that contains the name,
                    // so ensure an exact match to the expected asset we want to set.
                    if (asset.name.Equals(assetName, StringComparison.OrdinalIgnoreCase))
                    {
                        inputActionManager.actionAssets = new List<InputActionAsset> { asset };
                        break;
                    }
                }
            }

            return originGo;
        }

        static void AddXRControllersToOrigin(XROrigin origin, InputType inputType)
        {
            var cameraOffsetTransform = origin.CameraFloorOffsetObject.transform;

            var leftHandRayInteractorGo = CreateRayInteractor(cameraOffsetTransform, inputType, "Left Controller");
            var leftHandController = leftHandRayInteractorGo.GetComponent<XRController>();
            if (leftHandController != null)
                leftHandController.controllerNode = XRNode.LeftHand;

            var rightHandRayInteractorGo = CreateRayInteractor(cameraOffsetTransform, inputType, "Right Controller");
            var rightHandController = rightHandRayInteractorGo.GetComponent<XRController>();
            if (rightHandController != null)
                rightHandController.controllerNode = XRNode.RightHand;

            Place(leftHandRayInteractorGo, cameraOffsetTransform);
            Place(rightHandRayInteractorGo, cameraOffsetTransform);
        }

        static Type GetControllerType(InputType inputType)
        {
            switch (inputType)
            {
                case InputType.ActionBased:
                    return typeof(ActionBasedController);
                case InputType.DeviceBased:
                    return typeof(XRController);
                default:
                    throw new InvalidEnumArgumentException(nameof(inputType), (int)inputType, typeof(InputType));
            }
        }

        static Type GetSnapTurnType(InputType inputType)
        {
            switch (inputType)
            {
                case InputType.ActionBased:
                    return typeof(ActionBasedSnapTurnProvider);
                case InputType.DeviceBased:
                    return typeof(DeviceBasedSnapTurnProvider);
                default:
                    throw new InvalidEnumArgumentException(nameof(inputType), (int)inputType, typeof(InputType));
            }
        }

        /// <summary>
        /// Gets the <see cref="Transform"/> associated with the <see cref="MenuCommand.context"/>.
        /// </summary>
        /// <param name="menuCommand">The object passed to custom menu item functions to operate on.</param>
        /// <returns>Returns the <see cref="Transform"/> of the object that is the target of a menu command,
        /// or <see langword="null"/> if there is no context.</returns>
        static Transform GetContextTransform(this MenuCommand menuCommand)
        {
            var context = menuCommand.context as GameObject;
#pragma warning disable IDE0031 // Use null propagation -- Do not use for UnityEngine.Object types
            return context != null ? context.transform : null;
#pragma warning restore IDE0031
        }

        static GameObject CreateAndPlaceGameObject(string name, Transform parent, params Type[] types)
        {
            var go = ObjectFactory.CreateGameObject(name, types);
            Place(go, parent);
            return go;
        }

        static GameObject CreateAndPlacePrimitive(string name, Transform parent, PrimitiveType primitiveType, params Type[] types)
        {
            var go = ObjectFactory.CreatePrimitive(primitiveType);
            go.name = name;
            go.SetActive(false);
            foreach (var type in types)
                ObjectFactory.AddComponent(go, type);
            go.SetActive(true);

            Place(go, parent);
            return go;
        }

        static void Place(GameObject go, Transform parent)
        {
            var transform = go.transform;

            if (parent != null)
            {
                Undo.SetTransformParent(transform, parent, "Reparenting");
                ResetTransform(transform);
                go.layer = parent.gameObject.layer;
            }
            else
            {
                // Puts it at the scene pivot, and otherwise world origin if there is no Scene view.
                var view = SceneView.lastActiveSceneView;
                if (view != null)
                    view.MoveToView(transform);
                else
                    transform.position = Vector3.zero;

                StageUtility.PlaceGameObjectInCurrentStage(go);
            }

            // Only at this point do we know the actual parent of the object and can modify its name accordingly.
            GameObjectUtility.EnsureUniqueNameForSibling(go);
        }

        static void ResetTransform(Transform transform)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;

            if (transform.parent is RectTransform)
            {
                var rectTransform = transform as RectTransform;
                if (rectTransform != null)
                {
                    rectTransform.anchorMin = Vector2.zero;
                    rectTransform.anchorMax = Vector2.one;
                    rectTransform.anchoredPosition = Vector2.zero;
                    rectTransform.sizeDelta = Vector2.zero;
                }
            }
        }

        /// <summary>
        /// Returns the absolute value of each component of the vector.
        /// </summary>
        /// <param name="value">The vector.</param>
        /// <returns>Returns the absolute value of each component of the vector.</returns>
        /// <seealso cref="Mathf.Abs(float)"/>
        static Vector3 Abs(Vector3 value) => new Vector3(Mathf.Abs(value.x), Mathf.Abs(value.y), Mathf.Abs(value.z));

        /// <summary>
        /// Returns the sign of each component of the vector.
        /// </summary>
        /// <param name="value">The vector.</param>
        /// <returns>Returns the sign of each component of the vector; 1 when the component is positive or zero, -1 when the component is negative.</returns>
        /// <seealso cref="Mathf.Sign"/>
        static Vector3 Sign(Vector3 value) => new Vector3(Mathf.Sign(value.x), Mathf.Sign(value.y), Mathf.Sign(value.z));

        /// <summary>
        /// Transforms a vector from world space to local space.
        /// Differs from <see cref="Transform.InverseTransformVector(Vector3)"/> in that
        /// this operation is unaffected by rotation.
        /// </summary>
        /// <param name="transform">The <see cref="Transform"/> the operation is relative to.</param>
        /// <param name="scale">The scale to transform.</param>
        /// <returns>Returns the scale in local space.</returns>
        static Vector3 InverseTransformScale(Transform transform, Vector3 scale)
        {
            var lossyScale = transform.lossyScale;
            return new Vector3(
                !Mathf.Approximately(lossyScale.x, 0f) ? scale.x / lossyScale.x : scale.x,
                !Mathf.Approximately(lossyScale.y, 0f) ? scale.y / lossyScale.y : scale.y,
                !Mathf.Approximately(lossyScale.z, 0f) ? scale.z / lossyScale.z : scale.z);
        }

        static float GetRadiusScaleFactor(SphereCollider collider)
        {
            // Copied from SphereColliderEditor
            var result = 0f;
            var lossyScale = collider.transform.lossyScale;

            for (var axis = 0; axis < 3; ++axis)
                result = Mathf.Max(result, Mathf.Abs(lossyScale[axis]));

            return result;
        }

        static float GetScaledRadius(SphereCollider collider, float radius)
        {
            var scaleFactor = GetRadiusScaleFactor(collider);
            return !Mathf.Approximately(scaleFactor, 0f) ? radius / scaleFactor : 0f;
        }


        #region JioXrRig

        #region Main Rig
        static GameObject CreateJioXROriginWithParent(Transform parent, HardwareTarget target, InputType inputType)
        {

            var originGo = CreateAndPlaceGameObject("JioXOrigin", parent, typeof(XROrigin)); //Create Parent GameObject of type XROrigin
            originGo.transform.localPosition = Vector3.zero;
            var offsetGo = CreateAndPlaceGameObject("Camera Offset", originGo.transform); //Create Camera Offset GameObject with JioXOrigin as parent
            var offsetTransform = offsetGo.transform;
            var origin = originGo.GetComponent<XROrigin>();
            var interActionManager = CreateInteractionManager(originGo.transform); //creates interaction manager if not already present
            var xrCamera = XRMainCameraFactory.CreateXRMainCamera(target, inputType); //Creates main camera and an additional camera

           

            Place(xrCamera.gameObject, offsetTransform);
            origin.CameraFloorOffsetObject = offsetGo;
            origin.Camera = xrCamera;
            origin.RequestedTrackingOriginMode = XROrigin.TrackingOriginMode.Device;
            origin.CameraYOffset = 0;


            var leftController = CreateController(offsetTransform, HandType.Left); //Add Left controller
            var rightController = CreateController(offsetTransform, HandType.Right); //Add Right Controller

            //var gazeController = CreateJioXrGazeInteractor(offsetTransform, inputType, xrCamera.transform, "Gaze Controller"); //Add Gaze Controller
            //gazeController.GetComponent<XRGazeInteractor>().interactionManager = interActionManager.GetComponent<XRInteractionManager>();
            
            SetupInputInteractionManager(inputType, originGo);

            SetupControllerHandGazeHandler(originGo, leftController, rightController);

            AddEventsSystem(originGo.transform);

            return originGo;
        }

        private static void SetupInputInteractionManager(InputType inputType, GameObject originGo)
        {
            if (inputType == InputType.ActionBased)
            {
                var inputActionManager = originGo.AddComponent<InputActionManager>();

                const string assetName = "XRI Default Input Actions";
                var asset = LoadAssetByName<InputActionAsset>(assetName);
               if(asset != null) inputActionManager.actionAssets = new List<InputActionAsset> { asset };

            }
        }

        private static void SetupControllerHandGazeHandler(GameObject originGo, GameObject leftController, GameObject rightController, GameObject gazeController = null)
        {
            ControllerHandGazeHandler handGazeHandler = originGo.AddComponent<ControllerHandGazeHandler>();

            string rightControllerMapName = "XR Right Controller";
            string leftControllerMapName = "XR Left Controller";

            InputActionAsset actionAsset = LoadAssetByName<InputActionAsset>("XRI Default Input Actions");

            SetPrivateField(handGazeHandler, "_leftABController", leftController.GetComponent<ActionBasedController>());
            SetPrivateField(handGazeHandler, "_rightABController", rightController.GetComponent<ActionBasedController>());
           if(gazeController != null) SetPrivateField(handGazeHandler, "_gazeInteractor", gazeController.GetComponent<XRGazeInteractor>());

            if (actionAsset != null)
            {
                SetPrivateField(handGazeHandler, "_leftControllerTracking", AssignAction(actionAsset, "Is Tracked", leftControllerMapName));
                SetPrivateField(handGazeHandler, "_rightControllerTracking", AssignAction(actionAsset, "Is Tracked", rightControllerMapName));
            }
        }

        #endregion

        #region JioXrHands
        static GameObject CreateJioXRHands(Transform parent, XRInteractionManager xRInteractionManager)
        {
            var xrHandsParent = CreateAndPlaceGameObject("XR Hands", parent); //Create a parent GameObject XR Hands


            return null;
        }

        static GameObject CreateHands(Transform parent, HandType hand, XRInteractionManager xRInteractionManager)
        {
            var handObject = CreateAndPlaceGameObject(hand == HandType.Left ? "Left Hand" : "Right Hand", parent, typeof(PokeGestureDetector), typeof(AudioSource), typeof(XRInteractionGroup), typeof(MetaSystemGestureDetector));

           // handObject.GetComponent<PokeGestureDetector>().SetHandedness(hand == HandType.Left ? UnityEngine.XR.Hands.Handedness.Left : UnityEngine.XR.Hands.Handedness.Right);


            var audioClip = LoadAssetByName<AudioClip>("Button Pop");
            if (audioClip != null) handObject.GetComponent<AudioSource>().clip = audioClip;

            #region Add interaction Group references

            var xrInteractionGroup = handObject.GetComponent<XRInteractionGroup>();
            xrInteractionGroup.interactionManager = xRInteractionManager;
            //xrInteractionGroup.AddGroupMember


            #endregion


            return null;
        }
        
        private static GameObject CreatePokeInteractor(HandType hand)
        {
            return null;
        }

        #endregion

        #region jioXr gaze controller

        static GameObject CreateJioXrGazeInteractor(Transform parent, InputType inputType, Transform rayOrigin, string name = "Gaze Interactor")
        {
            var gazeInteractableGO = CreateAndPlaceGameObject(name, parent,
                GetControllerType(inputType),
                typeof(XRGazeInteractor),
                typeof(XRInteractorLineVisual), typeof(XRInteractorReticleVisual));

            // Configure the Sorting Group
            /*var sortingGroup = gazeInteractableGO.GetComponent<SortingGroup>();
            sortingGroup.sortingOrder = 5;
            sortingGroup.enabled = false;*/

            // Configure Line Renderer
            var lineRenderer = gazeInteractableGO.GetComponent<LineRenderer>();
            lineRenderer.enabled = false;

            // Configure Gaze Interactor
            var gazeInteractor = gazeInteractableGO.GetComponent<XRGazeInteractor>();
            gazeInteractor.allowAnchorControl = false;
            gazeInteractor.rayOriginTransform = rayOrigin;
            gazeInteractor.disableVisualsWhenBlockedInGroup = false;

            // Configure Line Visual
            ConfigureLineVisual(gazeInteractableGO, rayOrigin);

            // Configure Reticle Visual
            ConfigureReticleVisual(gazeInteractableGO);

            // Configure Controller and Actions
            SetupGazeActionBasedController(gazeInteractableGO);

            CreateReticleObject(gazeInteractableGO.transform);

            return gazeInteractableGO;
        }

        private static void SetupGazeActionBasedController(GameObject gazeInteractableGO)
        {
            var controller = gazeInteractableGO.GetComponent<ActionBasedController>();
            controller.enableInputActions = true;

            string actionMapName = "XRI RightHand";
            string actionInteractionMapName = "XRI RightHand Interaction";
            string actionLocomotionMapName = "XRI RightHand Locomotion";

            InputActionAsset actionAsset = LoadAssetByName<InputActionAsset>("XRI Default Input Actions");

            if (actionAsset == null) return;
            // Assign actions using a helper method
            controller.positionAction = AssignAction(actionAsset, "Aim Position", actionMapName);
            controller.rotationAction = AssignAction(actionAsset, "Aim Rotation", actionMapName);
            controller.isTrackedAction = AssignAction(actionAsset, "Is Tracked", actionMapName);
            controller.trackingStateAction = AssignAction(actionAsset, "Tracking State", actionMapName);

            // Assign interaction actions
            controller.selectAction = AssignAction(actionAsset, "Select", actionInteractionMapName);
            controller.selectActionValue = AssignAction(actionAsset, "Select Value", actionInteractionMapName);
            controller.activateAction = AssignAction(actionAsset, "Activate", actionInteractionMapName);
            controller.activateActionValue = AssignAction(actionAsset, "Activate Value", actionInteractionMapName);
            controller.uiPressActionValue = AssignAction(actionAsset, "UI Press Value", actionInteractionMapName);
            controller.uiScrollAction = AssignAction(actionAsset, "UI Scroll", actionInteractionMapName);
            controller.hapticDeviceAction = AssignAction(actionAsset, "Haptic Device", actionMapName);
            controller.directionalAnchorRotationAction = AssignAction(actionAsset, "Teleport Direction", actionLocomotionMapName);
            controller.translateAnchorAction = AssignAction(actionAsset, "Translate Anchor", actionInteractionMapName);
            controller.rotateAnchorAction = AssignAction(actionAsset, "Rotate Anchor", actionInteractionMapName);
            controller.uiPressAction.action.AddBinding("<MetaAimHand>{LeftHand}/indexPressed");
            controller.uiPressAction.action.AddBinding("<HandTrackingDevice>{LeftHand}/triggerPressed");
            controller.uiPressAction.action.AddBinding("<MetaAimHand>{RightHand}/indexPressed");
            controller.uiPressAction.action.AddBinding("<HandTrackingDevice>{RightHand}/triggerPressed");
        }

        private static void ConfigureLineVisual(GameObject gazeInteractableGO, Transform rayOrigin)
        {
            var lineVisual = gazeInteractableGO.GetComponent<XRInteractorLineVisual>();
            lineVisual.lineOriginTransform = rayOrigin;
            lineVisual.validColorGradient = GetGradient();
            lineVisual.smoothMovement = true;
            lineVisual.followTightness = 10;
            lineVisual.snapThresholdDistance = 10;
            lineVisual.lineBendRatio = 0.01f;
            lineVisual.enabled = false;
        }

        private static void ConfigureReticleVisual(GameObject gazeInteractableGO)
        {
            var reticuleVisual = gazeInteractableGO.GetComponent<XRInteractorReticleVisual>();
            reticuleVisual.maxRaycastDistance = 100;
            reticuleVisual.prefabScalingFactor = 0.1f;
            reticuleVisual.undoDistanceScaling = true;
            reticuleVisual.alignPrefabWithSurfaceNormal = true;
            reticuleVisual.drawWhileSelecting = true;
            reticuleVisual.raycastMask = -1;
            reticuleVisual.endpointSmoothingTime = 0;
            reticuleVisual.drawOnNoHit = true;
        }

        private static GameObject CreateReticleObject(Transform parent)
        {
            var reticleObj = CreateAndPlaceGameObject("ReticleObject", parent, typeof(GazeFeedback));
            var maskObj = CreateAndPlaceGameObject("mask", reticleObj.transform, typeof(SpriteMask));
            var outerGazeObj = CreateAndPlaceGameObject("outerGaze", reticleObj.transform, typeof(SpriteRenderer));
            var innerGazeObj = CreateAndPlaceGameObject("innerGaze", reticleObj.transform, typeof(SpriteRenderer));
            var gazeFeedback = reticleObj.GetComponent<GazeFeedback>();

            reticleObj.transform.localPosition = new Vector3(0f, 0f, 10f);
           
            SetPrivateField(gazeFeedback, "maskCircle", maskObj.transform);
            SetPrivateField(gazeFeedback, "innerCircle", innerGazeObj.transform);

            var maskCirleMask = maskObj.GetComponent<SpriteMask>();
            var maskSprite = LoadAssetByName<Sprite>("Ellipse 1");
            if(maskSprite != null) maskCirleMask.sprite = maskSprite;
            maskCirleMask.alphaCutoff = 0f;
            maskCirleMask.spriteSortPoint = SpriteSortPoint.Center;
            maskCirleMask.renderingLayerMask = 0;
            maskObj.transform.localScale = new Vector3(0.83f, 0.83f, 0.83f);

            var outerGazeSpriteRenderer = outerGazeObj.GetComponent<SpriteRenderer>();
            var outGazeMaskSprite = LoadAssetByName<Sprite>("Ellipse 2");
            if (outGazeMaskSprite != null) outerGazeSpriteRenderer.sprite = outGazeMaskSprite;
            outerGazeSpriteRenderer.color = Color.white;
            var outerGazeMat = LoadAssetByName<Material>("Sprites - Default");
            if (outerGazeMat != null) outerGazeSpriteRenderer.sharedMaterial = outerGazeMat;
            outerGazeSpriteRenderer.drawMode = SpriteDrawMode.Simple;
            outerGazeSpriteRenderer.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
            outerGazeSpriteRenderer.spriteSortPoint =  SpriteSortPoint.Center;
            outerGazeObj.transform.localScale = Vector3.one;

            var innerGazeSpriteRenderer = innerGazeObj.GetComponent<SpriteRenderer>();
            var innerGazeMaskSprite = LoadAssetByName<Sprite>("Ellipse 3");
            if (innerGazeMaskSprite != null) innerGazeSpriteRenderer.sprite = innerGazeMaskSprite;
            innerGazeSpriteRenderer.color = new Color32 (133, 61, 214, 255);
            var innerGazeMat = LoadAssetByName<Material>("Sprites - Default");
            if (innerGazeMat != null) innerGazeSpriteRenderer.sharedMaterial = innerGazeMat;
            innerGazeSpriteRenderer.drawMode = SpriteDrawMode.Simple;
            innerGazeSpriteRenderer.maskInteraction = SpriteMaskInteraction.None;
            innerGazeSpriteRenderer.spriteSortPoint = SpriteSortPoint.Center;
            innerGazeObj.transform.localScale = new Vector3(0.85f, 0.85f, 0.85f);

            return reticleObj;

        }

        #endregion

        #region JioXr Controllers

        static GameObject CreateController(Transform parent,HandType hand)
        {

            var controllerObj = CreateAndPlaceGameObject(hand == HandType.Left ? "Left Controller" : "Right Controller", parent, typeof(ActionBasedController),
                typeof(ActionBasedControllerManager),
                typeof(XRInteractionGroup));

            
            string actionMapName = hand == HandType.Right ? "XRI RightHand" : "XRI LeftHand";
            string actionInteractionMapName = $"{actionMapName} Interaction";
            string actionLocomotionMapName = $"{actionMapName} Locomotion";
            string controllerMapName = hand == HandType.Right ? "XR Right Controller" : "XR Left Controller";

            InputActionAsset actionAsset = LoadAssetByName<InputActionAsset>("XRI Default Input Actions");

            SetupActionBasedController(controllerObj, actionMapName, actionInteractionMapName, actionLocomotionMapName, controllerMapName, actionAsset);
       
            var directInteractor = AddDirectInteractor(controllerObj.transform);
            var pokeInteractor = AddPokeInteractor(controllerObj.transform);
            var rayInteractor = AddRayInteractor(controllerObj.transform);
            var teleportInteractor = AddTeleportInteractor(controllerObj.transform);
            var interactionGroup = controllerObj.GetComponent<XRInteractionGroup>();
            
            SetupInteractionGroup(directInteractor, pokeInteractor, rayInteractor, interactionGroup);


           var stabilizer =  AddStabilizer(controllerObj.transform, rayInteractor.GetComponent<XRRayInteractor>(), hand == HandType.Left ? "Left Stabilizer" : "RightStabilizer");

            SetupActionBasedControllerManager(controllerObj, actionInteractionMapName, actionLocomotionMapName, actionAsset, interactionGroup, directInteractor.GetComponent<XRDirectInteractor>(), rayInteractor.GetComponent<XRRayInteractor>(), teleportInteractor.GetComponent<XRRayInteractor>(), stabilizer.GetComponent<XRTransformStabilizer>());


            //CreateControllerModel(controllerObj.transform, hand == HandType.Left ? "XR Controller Left" : "XR Controller Right");
            return controllerObj;
        }

        private static void SetupInteractionGroup(GameObject directInteractor, GameObject pokeInteractor, GameObject rayInteractor, XRInteractionGroup interactionGroup)
        {
            interactionGroup.startingGroupMembers.Add(pokeInteractor.GetComponent<XRPokeInteractor>());
            interactionGroup.startingGroupMembers.Add(directInteractor.GetComponent<XRDirectInteractor>());
            interactionGroup.startingGroupMembers.Add(rayInteractor.GetComponent<XRRayInteractor>());
            interactionGroup.AddGroupMember(pokeInteractor.GetComponent<XRPokeInteractor>());
            interactionGroup.AddGroupMember(directInteractor.GetComponent<XRDirectInteractor>());
            interactionGroup.AddGroupMember(rayInteractor.GetComponent<XRRayInteractor>());

            interactionGroup.AddInteractionOverrideForGroupMember(directInteractor.GetComponent<XRDirectInteractor>(), pokeInteractor.GetComponent<XRPokeInteractor>());
        }

        private static void SetupActionBasedController(GameObject controllerObj, string actionMapName, string actionInteractionMapName, string actionLocomotionMapName, string controllerMapName, InputActionAsset actionAsset)
        {
            var actionBasedController = controllerObj.GetComponent<ActionBasedController>();

            actionBasedController.updateTrackingType = XRBaseController.UpdateType.UpdateAndBeforeRender;
            actionBasedController.enableInputTracking = true;

            if (actionAsset == null) return;
            // Assign actions using a helper method
            actionBasedController.positionAction = AssignAction(actionAsset, "Position", actionMapName);
            actionBasedController.rotationAction = AssignAction(actionAsset, "Rotation", actionMapName);
            actionBasedController.isTrackedAction = AssignAction(actionAsset, "Is Tracked", controllerMapName);
            actionBasedController.trackingStateAction = AssignAction(actionAsset, "Tracking State", controllerMapName);

            // Assign interaction actions
            actionBasedController.selectAction = AssignAction(actionAsset, "Select", actionInteractionMapName);
            actionBasedController.selectActionValue = AssignAction(actionAsset, "Select Value", actionInteractionMapName);
            actionBasedController.activateAction = AssignAction(actionAsset, "Activate", actionInteractionMapName);
            actionBasedController.activateActionValue = AssignAction(actionAsset, "Activate Value", actionInteractionMapName);
            actionBasedController.uiPressActionValue = AssignAction(actionAsset, "UI Press Value", actionInteractionMapName);
            actionBasedController.uiScrollAction = AssignAction(actionAsset, "UI Scroll", actionInteractionMapName);
            actionBasedController.hapticDeviceAction = AssignAction(actionAsset, "Haptic Device", actionMapName);
            actionBasedController.directionalAnchorRotationAction = AssignAction(actionAsset, "Teleport Direction", actionLocomotionMapName);
            actionBasedController.translateAnchorAction = AssignAction(actionAsset, "Translate Anchor", actionInteractionMapName);
            actionBasedController.rotateAnchorAction = AssignAction(actionAsset, "Rotate Anchor", actionInteractionMapName);
            actionBasedController.uiPressAction = AssignAction(actionAsset, "UI Press", actionInteractionMapName);
            actionBasedController.scaleToggleAction = AssignAction(actionAsset, "Scale Toggle", actionInteractionMapName);
            actionBasedController.scaleDeltaAction = AssignAction(actionAsset, "Scale Delta", actionInteractionMapName);

            actionBasedController.enableInputActions = true;
        }

        private static void SetupActionBasedControllerManager(GameObject controllerObj, string actionInteractionMapName, string actionLocomotionMapName, InputActionAsset actionAsset, XRInteractionGroup interactionGroup, XRDirectInteractor directInteractor, XRRayInteractor rayInteractor, XRRayInteractor teleportInteractor, XRTransformStabilizer stabilizer)
        {
            var actionBasedControllerManager = controllerObj.GetComponent<ActionBasedControllerManager>();

            SetPrivateField(actionBasedControllerManager, "m_ManipulationInteractionGroup", interactionGroup);
            SetPrivateField(actionBasedControllerManager, "m_DirectInteractor", directInteractor);
            SetPrivateField(actionBasedControllerManager, "m_RayInteractor", rayInteractor);
            SetPrivateField(actionBasedControllerManager, "m_TeleportInteractor", teleportInteractor);



            SetPrivateField(actionBasedControllerManager, "m_TeleportModeActivate", GetCorrectInputActionRef(actionAsset, "Teleport Mode Activate", actionLocomotionMapName));
            SetPrivateField(actionBasedControllerManager, "m_TeleportModeCancel", GetCorrectInputActionRef(actionAsset, "Teleport Mode Cancel", actionLocomotionMapName));
            SetPrivateField(actionBasedControllerManager, "m_Turn", GetCorrectInputActionRef(actionAsset, "Turn", actionLocomotionMapName));
            SetPrivateField(actionBasedControllerManager, "m_SnapTurn", GetCorrectInputActionRef(actionAsset, "Snap Turn", actionLocomotionMapName));
            SetPrivateField(actionBasedControllerManager, "m_Move", GetCorrectInputActionRef(actionAsset, "Move", actionLocomotionMapName));
            SetPrivateField(actionBasedControllerManager, "m_UIScroll", GetCorrectInputActionRef(actionAsset, "UI Scroll", actionInteractionMapName));

            actionBasedControllerManager.smoothMotionEnabled = true;
            actionBasedControllerManager.uiScrollingEnabled = true;
            //var listener = stabilizer.aimTarget;
            //SetPrivateField(actionBasedControllerManager, "m_RayInteractorChanged", listener);
        }


        static GameObject AddPokeInteractor(Transform parent)
        {
            var pokeInteractorObj = CreateAndPlaceGameObject("Poke Interactor", parent, typeof(XRPokeInteractor));
            var pokePoint = CreateAndPlaceGameObject("Poke Point", pokeInteractorObj.transform, typeof(MeshRenderer), typeof(MeshFilter));
            var cylinder = CreateAndPlaceGameObject("Cylinder", pokePoint.transform, typeof(MeshRenderer), typeof(MeshFilter));

            SetupPokeInteractor(pokeInteractorObj, pokePoint);

            ApplyMeshMaterialAndTransform(pokePoint, "Sphere", "No name", new Vector3(0.1f, 0.1f, 0.1f), new Vector3(-0.005f, -0.01f, 0.025f),
                new Vector3(5f, 10f, 0f), ShadowCastingMode.On, false, LightProbeUsage.BlendProbes, true, 1);

            ApplyMeshMaterialAndTransform(cylinder, "Cylinder", "lambert1", new Vector3(0.5f, 3.6f, 0.5f), new Vector3(0f, -0.01f, -4f),
                new Vector3(90, 0, 0), ShadowCastingMode.On, false, LightProbeUsage.BlendProbes, true, 1);


            return pokeInteractorObj;
        }

        private static void SetupPokeInteractor(GameObject pokeInteractorObj, GameObject pokePoint)
        {
            var pokeInterator = pokeInteractorObj.GetComponent<XRPokeInteractor>();

            pokeInterator.interactionLayerMask = 0;
            pokeInterator.attachTransform = pokePoint.transform;
            pokeInterator.disableVisualsWhenBlockedInGroup = true;
            pokeInterator.keepSelectedTargetValid = true;
            pokeInterator.pokeDepth = 0.1f;
            pokeInterator.pokeWidth = 0.0075f;
            pokeInterator.pokeSelectWidth = 0.015f;
            pokeInterator.pokeHoverRadius = 0.015f;
            pokeInterator.pokeInteractionOffset = 0.005f;
            pokeInterator.physicsLayerMask = -1;
            pokeInterator.physicsTriggerInteraction = QueryTriggerInteraction.Ignore;
            pokeInterator.requirePokeFilter = true;
            pokeInterator.enableUIInteraction = true;
            pokeInterator.debugVisualizationsEnabled = false;
        }

        static GameObject AddDirectInteractor(Transform parent)
        {
            var directInteractorObj = CreateAndPlaceGameObject("Direct Interactor", parent, typeof(XRDirectInteractor), typeof(SphereCollider));

            #region Configure Direct Interactor

            var directInteractor = directInteractorObj.GetComponent<XRDirectInteractor>();
            directInteractor.disableVisualsWhenBlockedInGroup = true;
            directInteractor.improveAccuracyWithSphereCollider = true;

            #endregion

            var sphereColider = directInteractorObj.GetComponent<SphereCollider>();
            sphereColider.isTrigger = true;

            return directInteractorObj;
        }

        static GameObject AddRayInteractor(Transform parent)
        {
            var rayInteractableObj = CreateAndPlaceGameObject("Ray Interactor", parent,
                typeof(XRRayInteractor),
                typeof(LineRenderer),
                typeof(XRInteractorLineVisual),
                typeof(SortingGroup));

            #region Configure Ray Interactor

            var rayInteractor = rayInteractableObj.GetComponent<XRRayInteractor>();
            rayInteractor.enableUIInteraction = true;
            rayInteractor.blockUIOnInteractableSelection = true;
            rayInteractor.allowAnchorControl = true;
            rayInteractor.anchorRotationMode = XRRayInteractor.AnchorRotationMode.RotateOverTime;
            rayInteractor.rotateSpeed = 180f;
            rayInteractor.scaleMode = UnityEngine.XR.Interaction.Toolkit.ScaleMode.Input;
            rayInteractor.disableVisualsWhenBlockedInGroup = true;

            #endregion

            #region Configure Line Renderer
            var lineRenderer = rayInteractableObj.GetComponent<LineRenderer>();
            var materials = new Material[1];
            materials[0] = AssetDatabase.GetBuiltinExtraResource<Material>(k_LineMaterial);
            lineRenderer.materials = materials;
            lineRenderer.loop = false;
            lineRenderer.widthMultiplier = 0.005f;
            lineRenderer.numCornerVertices = 4;
            lineRenderer.numCapVertices = 4;
            lineRenderer.shadowCastingMode = ShadowCastingMode.Off;
            lineRenderer.receiveShadows = false;
            lineRenderer.useWorldSpace = true;
            #endregion


            #region Configure XR Line Visual
            var xrLineVisual = rayInteractableObj.GetComponent<XRInteractorLineVisual>();
            xrLineVisual.lineWidth = 0.005f;
            xrLineVisual.overrideInteractorLineOrigin = true;
            xrLineVisual.lineOriginTransform = parent;
            xrLineVisual.treatSelectionAsValidState = true;
            xrLineVisual.overrideInteractorLineLength = true;
            xrLineVisual.lineLength = 10f;
            xrLineVisual.autoAdjustLineLength = true;
            xrLineVisual.minLineLength = 0.5f;
            xrLineVisual.useDistanceToHitAsMaxLineLength = true;
            xrLineVisual.lineRetractionDelay = 0.5f;
            xrLineVisual.lineLengthChangeSpeed = 12f;
            xrLineVisual.stopLineAtFirstRaycastHit = true;
            xrLineVisual.stopLineAtSelection = true;
            xrLineVisual.snapEndpointIfAvailable = true;
            xrLineVisual.lineBendRatio = 0.5f;

            #endregion

            var sortingGroup = rayInteractableObj.GetComponent<SortingGroup>();
            sortingGroup.sortingOrder = 30005;

            return rayInteractableObj;
        }

        static GameObject AddTeleportInteractor(Transform parent)
        {
            var teleportInteractorObj = CreateAndPlaceGameObject("Teleport Interactor", parent,
                typeof(XRRayInteractor),
                typeof(LineRenderer),
                typeof(XRInteractorLineVisual),
                typeof(SortingGroup),
                typeof(ActionBasedController));

            #region Configure Ray Interactor

            var rayInteractor = teleportInteractorObj.GetComponent<XRRayInteractor>();
            rayInteractor.interactionLayerMask = 1;
            rayInteractor.enableUIInteraction = false;
            rayInteractor.blockUIOnInteractableSelection = false;
            rayInteractor.allowAnchorControl = true;
            rayInteractor.anchorRotationMode = XRRayInteractor.AnchorRotationMode.MatchDirection;
            rayInteractor.translateSpeed = 0f;
            rayInteractor.scaleMode = UnityEngine.XR.Interaction.Toolkit.ScaleMode.None;
            rayInteractor.disableVisualsWhenBlockedInGroup = true;

            #endregion

            #region Configure Line Renderer
            var lineRenderer = teleportInteractorObj.GetComponent<LineRenderer>();
            var materials = new Material[1];
            materials[0] = AssetDatabase.GetBuiltinExtraResource<Material>(k_LineMaterial);
            lineRenderer.materials = materials;
            lineRenderer.loop = false;
            lineRenderer.widthMultiplier = 0.005f;
            lineRenderer.numCornerVertices = 4;
            lineRenderer.numCapVertices = 4;
            lineRenderer.shadowCastingMode = ShadowCastingMode.Off;
            lineRenderer.receiveShadows = false;
            lineRenderer.useWorldSpace = true;
            #endregion

            #region Configure XR Line Visual
            var xrLineVisual = teleportInteractorObj.GetComponent<XRInteractorLineVisual>();
            xrLineVisual.lineWidth = 0.005f;
            xrLineVisual.overrideInteractorLineOrigin = true;
            xrLineVisual.lineOriginTransform = parent;
            xrLineVisual.treatSelectionAsValidState = true;
            xrLineVisual.overrideInteractorLineLength = true;
            xrLineVisual.lineLength = 10f;
            xrLineVisual.autoAdjustLineLength = true;
            xrLineVisual.minLineLength = 0.5f;
            xrLineVisual.useDistanceToHitAsMaxLineLength = true;
            xrLineVisual.lineRetractionDelay = 0.5f;
            xrLineVisual.lineLengthChangeSpeed = 12f;
            xrLineVisual.stopLineAtFirstRaycastHit = true;
            xrLineVisual.stopLineAtSelection = true;
            xrLineVisual.snapEndpointIfAvailable = true;
            xrLineVisual.lineBendRatio = 0.5f;

            #endregion

            var sortingGroup = teleportInteractorObj.GetComponent<SortingGroup>();
            sortingGroup.sortingOrder = 30005;

            #region Configure Action Based Controller
            var controller = teleportInteractorObj.GetComponent<ActionBasedController>();

            controller.updateTrackingType = XRBaseController.UpdateType.UpdateAndBeforeRender;
            controller.enableInputTracking = false;

            string actionMapName = "XRI LeftHand";
            string actionInteractionMapName = $"{actionMapName} Interaction";
            string actionLocomotionMapName = $"{actionMapName} Locomotion";

            InputActionAsset actionAsset = LoadAssetByName<InputActionAsset>("XRI Default Input Actions");

            if (actionAsset != null)
            {
                controller.selectAction = AssignAction(actionAsset, "Select", actionInteractionMapName);
                controller.selectActionValue = AssignAction(actionAsset, "Select Value", actionInteractionMapName);
                controller.activateAction = AssignAction(actionAsset, "Activate", actionInteractionMapName);
                controller.activateActionValue = AssignAction(actionAsset, "Activate Value", actionInteractionMapName);
                controller.hapticDeviceAction = AssignAction(actionAsset, "Haptic Device", actionMapName);
                controller.directionalAnchorRotationAction = AssignAction(actionAsset, "Teleport Direction", actionLocomotionMapName);
                controller.rotateAnchorAction = AssignAction(actionAsset, "Rotate Anchor", actionInteractionMapName);
            }
            #endregion

            return teleportInteractorObj;
        }

        static GameObject AddStabilizer(Transform controller, XRRayInteractor rayInteractor, string name)
        {
            var stabilizerObj = CreateAndPlaceGameObject(name, controller.transform.parent, typeof(XRTransformStabilizer));
            var stabilizerObjAttach = CreateAndPlaceGameObject(name + " Attach", stabilizerObj.transform);

            var stabilizer = stabilizerObj.GetComponent<XRTransformStabilizer>();
            stabilizer.targetTransform = controller;
            stabilizer.aimTarget = rayInteractor;
            stabilizer.useLocalSpace = true;
            stabilizer.angleStabilization = 20f;
            stabilizer.positionStabilization = 0.25f;
            rayInteractor.attachTransform = stabilizerObjAttach.transform;
            rayInteractor.rayOriginTransform = stabilizerObj.transform;

            stabilizerObj.transform.localPosition = new Vector3(0f, -0.443f, -0.15f);

            return stabilizerObj;
        }

        static GameObject CreateControllerModel(Transform parent, string name)
        {
            var controllerModelParent = CreateAndPlaceGameObject(name, parent);
            controllerModelParent.transform.rotation = Quaternion.Euler(0f, 180f, 0f);

            var bumper = CreateAndPlaceGameObject("Bumper", controllerModelParent.transform, typeof(MeshRenderer), typeof(MeshFilter));
            var buttonHome = CreateAndPlaceGameObject("Button_Home", controllerModelParent.transform, typeof(MeshRenderer), typeof(MeshFilter));
            var controllerBase = CreateAndPlaceGameObject("Controller_Base", controllerModelParent.transform, typeof(MeshRenderer), typeof(MeshFilter));
            var touchPad = CreateAndPlaceGameObject("TouchPad", controllerModelParent.transform, typeof(MeshRenderer), typeof(MeshFilter));
            var trigger = CreateAndPlaceGameObject("Trigger", controllerModelParent.transform, typeof(MeshRenderer), typeof(MeshFilter));
            
            var thumbStickButtons = CreateAndPlaceGameObject("XRController_LT_Thumbstick_Buttons", controllerModelParent.transform);
            var buttonA = CreateAndPlaceGameObject("Button_A", thumbStickButtons.transform, typeof(MeshRenderer), typeof(MeshFilter));
            var buttonB = CreateAndPlaceGameObject("Button_B", thumbStickButtons.transform, typeof(MeshRenderer), typeof(MeshFilter));
            var thumbStick = CreateAndPlaceGameObject("Thumbstick", thumbStickButtons.transform, typeof(MeshRenderer), typeof(MeshFilter));
            var thumbstickBase = CreateAndPlaceGameObject("Thumbstick_Base", thumbStickButtons.transform, typeof(MeshRenderer), typeof(MeshFilter));


            ApplyMeshMaterialAndTransform(bumper, "Bumper", "No Name",Vector3.one, new Vector3(-0.01263656f, -0.028557f, 0.02732661f), new Vector3(0, -15, 0), ShadowCastingMode.On,
                false, LightProbeUsage.BlendProbes, true, 1);

            ApplyMeshMaterialAndTransform(buttonHome, "Button_Home", "No Name",new Vector3(1.01935f,1.01935f, 1.01935f), new Vector3(-0.01263656f, -0.028557f, 0.02732661f), new Vector3(21.182f, 0f, 0f), ShadowCastingMode.On,
                false, LightProbeUsage.BlendProbes, true, 1);

            ApplyMeshMaterialAndTransform(controllerBase, "Controller_Base", "No Name", Vector3.one, Vector3.zero, Vector3.zero, ShadowCastingMode.On,
                false, LightProbeUsage.BlendProbes, true, 1);

            ApplyMeshMaterialAndTransform(touchPad, "TouchPad", "No Name", Vector3.one, Vector3.zero, Vector3.zero, ShadowCastingMode.On,
                    false, LightProbeUsage.BlendProbes, true, 1);

            ApplyMeshMaterialAndTransform(trigger, "Trigger", "No Name", Vector3.one, Vector3.zero, Vector3.zero, ShadowCastingMode.On,
                false, LightProbeUsage.BlendProbes, true, 1);

            ApplyMeshMaterialAndTransform(buttonA, "Button_A", "No Name", Vector3.one, Vector3.zero, Vector3.zero, ShadowCastingMode.On,
                false, LightProbeUsage.BlendProbes, true, 1);

            ApplyMeshMaterialAndTransform(buttonB, "Button_B", "No Name", Vector3.one, Vector3.zero, Vector3.zero, ShadowCastingMode.On,
                false, LightProbeUsage.BlendProbes, true, 1);

            ApplyMeshMaterialAndTransform(thumbStick, "ThumbStick", "No Name", new Vector3(1.342947f,1.342947f,1.343947f), Vector3.zero, Vector3.zero, ShadowCastingMode.On,
                false, LightProbeUsage.BlendProbes, true, 1);

            ApplyMeshMaterialAndTransform(thumbstickBase, "ThumbStick_Base", "No Name", Vector3.one, Vector3.zero, Vector3.zero, ShadowCastingMode.On,
                false, LightProbeUsage.BlendProbes, true, 1);

            return controllerModelParent;
        }

      
            /// <summary>
            /// Sets the mesh, material, render settings, and transform properties of a GameObject.
            /// </summary>
            /// <param name="targetObject">The GameObject to modify.</param>
            /// <param name="meshName">The name of the mesh to load and assign.</param>
            /// <param name="materialName">The name of the material to load and assign.</param>
            /// <param name="localScale">The local scale of the transform.</param>
            /// <param name="localPosition">The local position of the transform.</param>
            /// <param name="localRotation">The local rotation of the transform (in Euler angles).</param>
            /// <param name="shadowCastingMode">The shadow casting mode for the MeshRenderer.</param>
            /// <param name="staticShadowCaster">Whether the object should cast static shadows.</param>
            /// <param name="lightProbeUsage">The light probe usage setting.</param>
            /// <param name="allowOcclusionWhenDynamic">Allow occlusion when dynamic setting for the MeshRenderer.</param>
            /// <param name="renderingLayerMask">The rendering layer mask of the MeshRenderer.</param>
            private static void ApplyMeshMaterialAndTransform(GameObject targetObject, string meshName, string materialName,
                Vector3 localScale, Vector3 localPosition, Vector3 localRotation,
                ShadowCastingMode shadowCastingMode, bool staticShadowCaster, LightProbeUsage lightProbeUsage,
                bool allowOcclusionWhenDynamic, uint renderingLayerMask)
            {
                // Apply Mesh
                var meshFilter = targetObject.GetComponent<MeshFilter>();
                var mesh = LoadAssetByName<Mesh>(meshName);
                if (mesh != null) meshFilter.sharedMesh = mesh;

                // Apply Material
                var meshRenderer = targetObject.GetComponent<MeshRenderer>();
                var material = LoadAssetByName<Material>(materialName);
                if (material != null) meshRenderer.sharedMaterial = material;

                // Apply Renderer settings
                meshRenderer.shadowCastingMode = shadowCastingMode;
                meshRenderer.staticShadowCaster = staticShadowCaster;
                meshRenderer.lightProbeUsage = lightProbeUsage;
                meshRenderer.allowOcclusionWhenDynamic = allowOcclusionWhenDynamic;
                meshRenderer.renderingLayerMask = renderingLayerMask;

                // Apply Transform properties
                var targetTransform = targetObject.transform;
                targetTransform.localScale = localScale;
                targetTransform.localPosition = localPosition;
                targetTransform.localEulerAngles = localRotation;
            }
        



        #endregion

        #region JioXr Event System

        private static void AddEventsSystem(Transform parent)
        {
            var eventSystemObj = CreateAndPlaceGameObject("EventSystem", parent, typeof(EventSystem), typeof(XRUIInputModule), typeof(FrameRate), typeof(HandInteractionEvents));

            var eventSystem = eventSystemObj.GetComponent<EventSystem>();
            eventSystem.sendNavigationEvents = true;
            eventSystem.pixelDragThreshold = 10;

            var XRUIInputModule = eventSystemObj.GetComponent<XRUIInputModule>();
            XRUIInputModule.clickSpeed = 0.3f;
            XRUIInputModule.moveDeadzone = 0.6f;
            XRUIInputModule.clickSpeed = 0.5f;
            XRUIInputModule.clickSpeed = 0.1f;
            XRUIInputModule.trackedDeviceDragThresholdMultiplier = 2f;
            XRUIInputModule.trackedScrollDeltaMultiplier = 5f;
            XRUIInputModule.activeInputMode = XRUIInputModule.ActiveInputMode.InputSystemActions;


            XRUIInputModule.enableXRInput = true;
            XRUIInputModule.enableTouchInput = true;
            XRUIInputModule.enableMouseInput = true;

            var actionMapName = "XRI UI";
            var actionAsset = LoadAssetByName<InputActionAsset>("XRI Default Input Actions");

            if(actionAsset != null)
            {
                XRUIInputModule.pointAction = GetCorrectInputActionRef(actionAsset, "Point", actionMapName);
                XRUIInputModule.leftClickAction = GetCorrectInputActionRef(actionAsset, "Click", actionMapName);
                XRUIInputModule.middleClickAction = GetCorrectInputActionRef(actionAsset, "MiddleClick", actionMapName);
                XRUIInputModule.rightClickAction = GetCorrectInputActionRef(actionAsset, "RightClick", actionMapName);
                XRUIInputModule.scrollWheelAction = GetCorrectInputActionRef(actionAsset, "ScrollWheel", actionMapName);
                XRUIInputModule.navigateAction = GetCorrectInputActionRef(actionAsset, "Navigate", actionMapName);
                XRUIInputModule.submitAction = GetCorrectInputActionRef(actionAsset, "Submit", actionMapName);
                XRUIInputModule.cancelAction = GetCorrectInputActionRef(actionAsset, "Cancel", actionMapName);
            }

            XRUIInputModule.enableBuiltinActionsAsFallback = true;
            XRUIInputModule.enableGamepadInput = true;
            XRUIInputModule.enableJoystickInput = true;
            XRUIInputModule.horizontalAxis = "Horizontal";
            XRUIInputModule.verticalAxis = "Vertical";
            XRUIInputModule.submitButton = "Submit";
            XRUIInputModule.cancelButton = "Cancel";


            var handInteractionEvents = eventSystemObj.GetComponent<HandInteractionEvents>();
            var jioXinputAsset = LoadAssetByName<InputActionAsset>("JioX Inputs");
            if(jioXinputAsset != null) SetPrivateField(handInteractionEvents, "inputActionAsset", jioXinputAsset);
        }

        #endregion

        #region JioXr Utility
        private static InputActionProperty AssignAction(InputActionAsset actionAsset, string actionName, string actionMapName)
        {
            var actionReference = GetCorrectInputActionRef(actionAsset, actionName, actionMapName);
            if (actionReference != null)
            {
                return new InputActionProperty(actionReference); // Return the new InputActionProperty
            }
            else
            {
                Debug.LogError($"Action '{actionName}' not found in action map '{actionMapName}'.");
                return default; // Return default if not found
            }
        }

        private static InputActionReference GetCorrectInputActionRef(InputActionAsset actionAsset, string actionName, string actionMapName)
        {
            InputAction action = FindInputActionByName(actionAsset, actionName, actionMapName);
            return action != null ? CreateInputActionReference(action) : null;
        }

        private static InputAction FindInputActionByName(InputActionAsset actionAsset, string actionName, string actionMapName)
        {
            Debug.Log($"Action maps count {actionAsset.actionMaps.Count}");
            foreach (var actionMap in actionAsset.actionMaps)
            {
                Debug.Log($"Action map name {actionMap.name}");
                if (actionMap.name.Equals(actionMapName))
                {
                    return actionMap.FindAction(actionName);
                }
            }
            return null; // Return null if not found
        }

        private static InputActionReference CreateInputActionReference(InputAction action)
        {
            // Create an instance of InputActionReference and assign the action
            var inputActionRef = ScriptableObject.CreateInstance<InputActionReference>();
            inputActionRef.Set(action); // Set the InputAction in the InputActionReference
            return inputActionRef;
        }

        static Gradient GetGradient() //TODO: need to update
        {
            Gradient gradient = new Gradient();

            // Set up the color keys (from the provided data)
            GradientColorKey[] colorKeys = new GradientColorKey[2];
            colorKeys[0].color = new Color(0.0f, 0.0f, 0.0f, 1.0f); // key0
            colorKeys[0].time = 0.0f; // ctime0 (normalized to 0 to 1)

            colorKeys[1].color = new Color(1.0f, 1.0f, 1.0f, 0.0f); // key1
            colorKeys[1].time = 1.0f; // ctime1 (65535 normalized to 1.0)

            // Set up the alpha keys
            GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
            alphaKeys[0].alpha = 1.0f; // a = 1.0f for key0
            alphaKeys[0].time = 0.0f; // atime0 (normalized)

            alphaKeys[1].alpha = 0.0f; // a = 0.0f for key1
            alphaKeys[1].time = 1.0f; // atime1 (normalized to 1.0)

            // Assign the color keys and alpha keys to the gradient
            gradient.SetKeys(colorKeys, alphaKeys);

            return gradient;
        }
        private static T LoadAssetByName<T>(string assetName) where T : UnityEngine.Object
        {
            // Construct the search filter for the specified asset type.
            string searchFilter = "\"" + assetName + "\" t:" + typeof(T).Name;

            // Search for all assets matching the filter.
            foreach (var guid in AssetDatabase.FindAssets(searchFilter))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath<T>(path);

                // Ensure the asset name matches the expected asset (ignores case).
                if (asset != null && asset.name.Equals(assetName, StringComparison.OrdinalIgnoreCase))
                {
                    return asset;
                }
            }

            // Return null if no matching asset is found.
            return null;
        }

        public static void SetPrivateField(object target, string fieldName, object value)
        {
            // Get the type of the target object
            Type type = target.GetType();

            // Use reflection to get the field and set its value
            FieldInfo fieldInfo = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (fieldInfo != null)
            {
                // Check if the field is a UnityEvent or UnityEvent<T>
                Type fieldType = fieldInfo.FieldType;

                // Case 1: If the field is a UnityEvent
                if (typeof(UnityEvent).IsAssignableFrom(fieldType))
                {
                    UnityEvent unityEvent = (UnityEvent)fieldInfo.GetValue(target);
                    if (unityEvent != null)
                    {
                        UnityAction action = value as UnityAction;
                        if (action != null)
                        {
                            unityEvent.AddListener(action);
                            Debug.Log($"Successfully added listener to {fieldName} in {type.Name}");
                        }
                        else
                        {
                            Debug.LogError("Provided value is not a UnityAction.");
                        }
                    }
                    else
                    {
                        Debug.LogError($"UnityEvent {fieldName} is null.");
                    }
                }
                // Case 2: If the field is a UnityEvent<T> (Generic)
                else if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(UnityEvent<>))
                {
                    // Get the generic argument type (i.e., the type parameter T for UnityEvent<T>)
                    Type eventType = fieldType.GetGenericArguments()[0];

                    // Get the UnityEvent<T> instance
                    object unityEvent = fieldInfo.GetValue(target);

                    if (unityEvent != null)
                    {
                        // Create the delegate for the UnityAction<T> dynamically
                        MethodInfo addListenerMethod = fieldType.GetMethod("AddListener");
                        if (addListenerMethod != null)
                        {
                            // The value (listener) passed in must be a method that matches the signature (e.g., UnityAction<T>)
                            MethodInfo listenerMethod = value.GetType().GetMethod("Invoke");

                            if (listenerMethod != null)
                            {
                                // Create a delegate of the appropriate type (i.e., UnityAction<T>)
                                Delegate action = Delegate.CreateDelegate(typeof(UnityAction<>).MakeGenericType(eventType), value, listenerMethod);

                                // Invoke AddListener on UnityEvent<T> with the action as the parameter
                                addListenerMethod.Invoke(unityEvent, new object[] { action });

                                Debug.Log($"Successfully added listener to {fieldName} in {type.Name}");
                            }
                            else
                            {
                                Debug.LogError("Failed to find the Invoke method on the listener.");
                            }
                        }
                        else
                        {
                            Debug.LogError($"AddListener method not found for {fieldName}");
                        }
                    }
                    else
                    {
                        Debug.LogError($"UnityEvent {fieldName} is null.");
                    }
                }
                else
                {
                    // Set normal field value if not UnityEvent
                    if (value.GetType() == fieldInfo.FieldType)
                    {
                        fieldInfo.SetValue(target, value);
                        Debug.Log($"Successfully set {fieldName} to {value} in {type.Name}");
                    }
                    else
                    {
                        Debug.LogError($"Type mismatch: expected {fieldInfo.FieldType.Name}, but got {value.GetType().Name}");
                    }
                }
            }
            else
            {
                Debug.LogError($"Field {fieldName} not found in {type.Name}.");
            }
        }

        public static void SetPrivateField(object target, string fieldName, object listenerInstance, string listenerMethodName)
        {
            // Get the type of the target object
            Type targetType = target.GetType();

            // Use reflection to get the field and set its value
            FieldInfo fieldInfo = targetType.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (fieldInfo != null)
            {
                // Check if the field is a UnityEvent or UnityEvent<T>
                Type fieldType = fieldInfo.FieldType;

                // Case 1: If the field is a UnityEvent
                if (typeof(UnityEvent).IsAssignableFrom(fieldType))
                {
                    UnityEvent unityEvent = (UnityEvent)fieldInfo.GetValue(target);
                    if (unityEvent != null)
                    {
                        UnityAction action = (UnityAction)Delegate.CreateDelegate(typeof(UnityAction), listenerInstance, listenerMethodName);
                        unityEvent.AddListener(action);
                        Debug.Log($"Successfully added listener to {fieldName} in {targetType.Name}");
                    }
                    else
                    {
                        Debug.LogError($"UnityEvent {fieldName} is null.");
                    }
                }
                // Case 2: If the field is a UnityEvent<T> (Generic)
                else if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(UnityEvent<>))
                {
                    // Get the generic argument type (i.e., the type parameter T for UnityEvent<T>)
                    Type eventType = fieldType.GetGenericArguments()[0];

                    // Get the UnityEvent<T> instance
                    object unityEvent = fieldInfo.GetValue(target);

                    if (unityEvent != null)
                    {
                        // Find the method on the listenerInstance that matches the listenerMethodName
                        MethodInfo listenerMethod = listenerInstance.GetType().GetMethod(listenerMethodName);

                        if (listenerMethod != null)
                        {
                            // Ensure the method signature matches UnityAction<T> (takes one parameter of type T)
                            ParameterInfo[] parameters = listenerMethod.GetParameters();
                            if (parameters.Length == 1 && parameters[0].ParameterType == eventType)
                            {
                                // Create the delegate of the appropriate type (i.e., UnityAction<T>)
                                Delegate action = Delegate.CreateDelegate(typeof(UnityAction<>).MakeGenericType(eventType), listenerInstance, listenerMethod);

                                // Use reflection to call AddListener on UnityEvent<T>
                                MethodInfo addListenerMethod = fieldType.GetMethod("AddListener");
                                addListenerMethod.Invoke(unityEvent, new object[] { action });

                                Debug.LogError($"Successfully added listener to {fieldName} in {targetType.Name}");
                            }
                            else
                            {
                                Debug.LogError($"Method parameter mismatch: expected {eventType}, but got {parameters[0].ParameterType}");
                            }
                        }
                        else
                        {
                            Debug.LogError($"Listener method {listenerMethodName} not found in {listenerInstance.GetType().Name}");
                        }
                    }
                    else
                    {
                        Debug.LogError($"UnityEvent {fieldName} is null.");
                    }
                }
                else
                {
                    Debug.LogError($"Field {fieldName} is not a UnityEvent or UnityEvent<T>.");
                }
            }
            else
            {
                Debug.LogError($"Field {fieldName} not found in {targetType.Name}.");
            }
        }
        #endregion

        #endregion

#if AR_FOUNDATION_PRESENT
#if AR_FOUNDATION_5_0_OR_NEWER
        [MenuItem("GameObject/JioXR/XR Origin (AR)", false, 10), UsedImplicitly]
#pragma warning disable IDE0051 // Remove unused private members -- Editor Menu Item
        static void CreateXROriginForAR(MenuCommand menuCommand)
#pragma warning restore IDE0051
        {
            Finalize(CreateXROriginWithParent(menuCommand?.GetContextTransform(), HardwareTarget.AR, InputType.ActionBased));
        }
#endif

        [MenuItem("GameObject/JioXR/AR Gesture Interactor", false, 10), UsedImplicitly]
#pragma warning disable IDE0051 // Remove unused private members -- Editor Menu Item
        static void CreateARGestureInteractor(MenuCommand menuCommand)
#pragma warning restore IDE0051
        {
            CreateInteractionManager();

            Finalize(CreateAndPlaceGameObject("AR Gesture Interactor",
                menuCommand?.GetContextTransform(),
                typeof(ARGestureInteractor)));
        }

        [MenuItem("GameObject/JioXR/AR Placement Interactable", false, 10), UsedImplicitly]
#pragma warning disable IDE0051 // Remove unused private members -- Editor Menu Item
        static void CreateARPlacementInteractable(MenuCommand menuCommand)
#pragma warning restore IDE0051
        {
            CreateInteractionManager();

            Finalize(CreateAndPlaceGameObject("AR Placement Interactable",
                menuCommand?.GetContextTransform(),
                typeof(ARPlacementInteractable)));
        }

        [MenuItem("GameObject/JioXR/AR Selection Interactable", false, 10), UsedImplicitly]
#pragma warning disable IDE0051 // Remove unused private members -- Editor Menu Item
        static void CreateARSelectionInteractable(MenuCommand menuCommand)
#pragma warning restore IDE0051
        {
            CreateInteractionManager();

            Finalize(CreateAndPlaceGameObject("AR Selection Interactable",
                menuCommand?.GetContextTransform(),
                typeof(ARSelectionInteractable)));
        }

        [MenuItem("GameObject/JioXR/AR Translation Interactable", false, 10), UsedImplicitly]
#pragma warning disable IDE0051 // Remove unused private members -- Editor Menu Item
        static void CreateARTranslationInteractable(MenuCommand menuCommand)
#pragma warning restore IDE0051
        {
            CreateInteractionManager();

            Finalize(CreateAndPlaceGameObject("AR Translation Interactable",
                menuCommand?.GetContextTransform(),
                typeof(ARTranslationInteractable)));
        }

        [MenuItem("GameObject/JioXR/AR Scale Interactable", false, 10), UsedImplicitly]
#pragma warning disable IDE0051 // Remove unused private members -- Editor Menu Item
        static void CreateARScaleInteractable(MenuCommand menuCommand)
#pragma warning restore IDE0051
        {
            CreateInteractionManager();

            Finalize(CreateAndPlaceGameObject("AR Scale Interactable",
                menuCommand?.GetContextTransform(),
                typeof(ARScaleInteractable)));
        }

        [MenuItem("GameObject/JioXR/AR Rotation Interactable", false, 10), UsedImplicitly]
#pragma warning disable IDE0051 // Remove unused private members -- Editor Menu Item
        static void CreateARRotationInteractable(MenuCommand menuCommand)
#pragma warning restore IDE0051
        {
            CreateInteractionManager();

            Finalize(CreateAndPlaceGameObject("AR Rotation Interactable",
                menuCommand?.GetContextTransform(),
                typeof(ARRotationInteractable)));
        }

        [MenuItem("GameObject/JioXR/AR Annotation Interactable", false, 10), UsedImplicitly]
#pragma warning disable IDE0051 // Remove unused private members -- Editor Menu Item
        static void CreateARAnnotationInteractable(MenuCommand menuCommand)
#pragma warning restore IDE0051
        {
            CreateInteractionManager();

            Finalize(CreateAndPlaceGameObject("AR Annotation Interactable",
                menuCommand?.GetContextTransform(),
                typeof(ARAnnotationInteractable)));
        }

#endif // AR_FOUNDATION_PRESENT
    }
}

using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace JioXSDK
{
    [System.Serializable]
    public class InteractionUIDataField<T>
    {
        [HideInInspector] public bool isActive;
        public T initialValue;
        public T finalValue;
        public float duration;
    }

    public class InteractionUIState : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        // Serialized fields
        [SerializeField] private int _gazeTransition, _pokeTransition;

        // Data for Poke Hover
        [SerializeField] private InteractionUIDataField<float> zElevationPokeHoverData;
        [SerializeField] private InteractionUIDataField<Material> materialSwapPokeHoverData;
        [SerializeField] private InteractionUIDataField<Texture> textureSwapPokeHoverData;
        [SerializeField] private InteractionUIDataField<Color> colorChangePokeHoverData;
        [SerializeField] private InteractionUIDataField<Vector3> scaleChangePokeHoverData;

        // Data for Gaze Hover
        [SerializeField] private InteractionUIDataField<float> zElevationGazeHoverData;
        [SerializeField] private InteractionUIDataField<Material> materialSwapGazeHoverData;
        [SerializeField] private InteractionUIDataField<Texture> textureSwapGazeHoverData;
        [SerializeField] private InteractionUIDataField<Color> colorChangeGazeHoverData;
        [SerializeField] private InteractionUIDataField<Vector3> scaleChangeGazeHoverData;

        // Data for Poke Pressed
        [SerializeField] private InteractionUIDataField<float> zElevationPokePressedData;
        [SerializeField] private InteractionUIDataField<Material> materialSwapPokePressedData;
        [SerializeField] private InteractionUIDataField<Texture> textureSwapPokePressedData;
        [SerializeField] private InteractionUIDataField<Color> colorChangePokePressedData;
        [SerializeField] private InteractionUIDataField<Vector3> scaleChangePokePressedData;

        // Data for Gaze Pressed
        [SerializeField] private InteractionUIDataField<float> zElevationGazePressedData;
        [SerializeField] private InteractionUIDataField<Material> materialSwapGazePressedData;
        [SerializeField] private InteractionUIDataField<Texture> textureSwapGazePressedData;
        [SerializeField] private InteractionUIDataField<Color> colorChangeGazePressedData;
        [SerializeField] private InteractionUIDataField<Vector3> scaleChangeGazePressedData;

        // Private variables
        private RectTransform rectTransform;
        private Image image;

        private float defaultZ = 0;
        private Color defaultColor = Color.clear;
        private Vector3 defaultScale = Vector3.one;

        public bool isPokeEnabled = false;
        public bool isHovering = false;

        private bool isDragged = false;
        private bool isZPosAnimationInProgress = false;
        private bool isColorAnimationInProgress = false;
        private bool isScaleAnimationInProgress = false;

        private CancellationTokenSource zPos_cts, color_cts, scale_cts;

        private CustomScroll parentScroll;
        private CustomScrollbar parentScrollbar;
        private CustomScrollbar scrollbarComponent;

        private void Awake()
        {
            InitializeAsyncCancellationTokens();
            rectTransform = GetComponent<RectTransform>();
            image = GetComponent<Image>();
            parentScroll = GetComponentInParent<CustomScroll>();
            parentScrollbar = GetComponentInParent<CustomScrollbar>();
            scrollbarComponent = GetComponent<CustomScrollbar>();
        }

        private void Start()
        {
            defaultZ = rectTransform.anchoredPosition3D.z;
            defaultColor = image.color;
            defaultScale = rectTransform.localScale;
            SetInitialStateAnimations(true);  // Hover state
            SetInitialStateAnimations(false); // Press state
        }

        private void OnDestroy()
        {
            DisposeTokens();
        }

        private void InitializeAsyncCancellationTokens()
        {
            zPos_cts = new CancellationTokenSource();
            color_cts = new CancellationTokenSource();
            scale_cts = new CancellationTokenSource();
        }

        private void DisposeTokens()
        {
            zPos_cts.Cancel();
            color_cts.Cancel();
            scale_cts.Cancel();
            zPos_cts.Dispose();
            color_cts.Dispose();
            scale_cts.Dispose();
        }

        // Refactored hover/press state initialization
        private void SetInitialStateAnimations(bool isHover)
        {
            InteractionUIDataField<float> zElevationData = isHover ? zElevationPokeHoverData : zElevationPokePressedData;
            InteractionUIDataField<Color> colorChangeData = isHover ? colorChangePokeHoverData : colorChangePokePressedData;
            InteractionUIDataField<Vector3> scaleChangeData = isHover ? scaleChangePokeHoverData : scaleChangePokePressedData;
            InteractionUIDataField<Material> materialSwapData = isHover ? materialSwapPokeHoverData : materialSwapPokePressedData;
            InteractionUIDataField<Texture> textureSwapData = isHover ? textureSwapPokeHoverData : textureSwapPokePressedData;

            SetInitialValue(ref zElevationData, rectTransform.anchoredPosition3D.z);
            SetInitialValue(ref colorChangeData, image.color);
            SetInitialValue(ref scaleChangeData, rectTransform.localScale);
            SetInitialValue(ref materialSwapData, image.material);
            //SetInitialValue(ref textureSwapData, image.material.GetTexture("_Background"));
        }

        private void SetInitialValue<T>(ref InteractionUIDataField<T> dataField, T initialValue)
        {
            if (dataField.isActive)
                dataField.initialValue = initialValue;
        }

        // Event handlers
        public void OnPointerEnter(PointerEventData eventData)
        {
            isHovering = true;
            TriggerAnimations(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isHovering = false;
            TriggerAnimations(false);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (isHovering) TriggerAnimations(true, true); // Trigger pressed state
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (isHovering && !isDragged) TriggerAnimations(false, true); // Return to hover state
            isDragged = false;
        }

        // Triggers animations based on hover or press
        private void TriggerAnimations(bool isFinal, bool isPressed = false)
        {
            bool isPoke = isPokeEnabled;
            StartZPositionLoop(isPoke ? (isPressed ? zElevationPokePressedData : zElevationPokeHoverData) : (isPressed ? zElevationGazePressedData : zElevationGazeHoverData), isFinal);
            StartColorLoop(isPoke ? (isPressed ? colorChangePokePressedData : colorChangePokeHoverData) : (isPressed ? colorChangeGazePressedData : colorChangeGazeHoverData), isFinal);
            StartScaleLoop(isPoke ? (isPressed ? scaleChangePokePressedData : scaleChangePokeHoverData) : (isPressed ? scaleChangeGazePressedData : scaleChangeGazeHoverData), isFinal);

            MaterialSwap(isPoke ? (isPressed ? materialSwapPokePressedData : materialSwapPokeHoverData) : (isPressed ? materialSwapGazePressedData : materialSwapGazeHoverData), isFinal);
            TextureSwap(isPoke ? (isPressed ? textureSwapPokePressedData : textureSwapPokeHoverData) : (isPressed ? textureSwapGazePressedData : textureSwapGazeHoverData), isFinal);
        }

        // Animation loops refactored (unchanged)
        // ZPositionLoop, ColorLoop, ScaleLoop, MaterialSwap, TextureSwap remain unchanged

        private void MaterialSwap(InteractionUIDataField<Material> materialSwapData, bool isFinal)
        {
            if (materialSwapData.isActive)
            {
                image.material = isFinal ? materialSwapData.finalValue : materialSwapData.initialValue;
            }
        }

        private void TextureSwap(InteractionUIDataField<Texture> textureSwapData, bool isFinal)
        {
            if (textureSwapData.isActive)
            {
                image.material.SetTexture("_Background", isFinal ? textureSwapData.finalValue : textureSwapData.initialValue);
            }
        }

        private async void ZPositionLoop(InteractionUIDataField<float> zElevationData, bool isFinal)
        {
            if (zElevationData.isActive)
            {
                Vector3 final = rectTransform.anchoredPosition3D;
                final.z = isFinal ? zElevationData.finalValue : zElevationData.initialValue;
                isZPosAnimationInProgress = true;
                while (isZPosAnimationInProgress)
                {
                    rectTransform.anchoredPosition3D = Vector3.Lerp(rectTransform.anchoredPosition3D, final, Time.deltaTime / zElevationData.duration);
                    await Task.Delay(Mathf.RoundToInt(Time.deltaTime * 1000), zPos_cts.Token);
                    if (Vector3.Distance(rectTransform.anchoredPosition3D, final) < 0.01f)
                    {
                        isZPosAnimationInProgress = false;
                    }
                }
            }
        }

        private async void ColorLoop(InteractionUIDataField<Color> changeColorData, bool isFinal)
        {
            if (changeColorData.isActive)
            {
                Color final = isFinal ? changeColorData.finalValue : changeColorData.initialValue;
                isColorAnimationInProgress = true;
                while (isColorAnimationInProgress)
                {
                    image.color = Color.Lerp(image.color, final, Time.deltaTime / changeColorData.duration);
                    await Task.Delay(Mathf.RoundToInt(Time.deltaTime * 1000), color_cts.Token);
                    if (image.color.Equals(final))
                    {
                        isColorAnimationInProgress = false;
                    }
                }
            }
        }

        private async void ScaleLoop(InteractionUIDataField<Vector3> changeScaleData, bool isFinal)
        {
            if (changeScaleData.isActive)
            {
                Vector3 final = isFinal ? changeScaleData.finalValue : changeScaleData.initialValue;
                isScaleAnimationInProgress = true;
                while (isScaleAnimationInProgress)
                {
                    rectTransform.localScale = Vector3.Lerp(rectTransform.localScale, final, Time.deltaTime / changeScaleData.duration);
                    await Task.Delay(Mathf.RoundToInt(Time.deltaTime * 1000), scale_cts.Token);
                    if (Vector3.Distance(rectTransform.localScale, final) < 0.01f)
                    {
                        isScaleAnimationInProgress = false;
                    }
                }
            }
        }

        private async void StartZPositionLoop(InteractionUIDataField<float> zElevationData, bool isFinal)
        {
            isZPosAnimationInProgress = false;
            await Task.Delay(Mathf.RoundToInt(Time.deltaTime * 1000), zPos_cts.Token);
            ZPositionLoop(zElevationData, isFinal);
        }

        private async void StartColorLoop(InteractionUIDataField<Color> changeColorData, bool isFinal)
        {
            isColorAnimationInProgress = false;
            await Task.Delay(Mathf.RoundToInt(Time.deltaTime * 1000), color_cts.Token);
            ColorLoop(changeColorData, isFinal);
        }

        private async void StartScaleLoop(InteractionUIDataField<Vector3> changeScaleData, bool isFinal)
        {
            isScaleAnimationInProgress = false;
            await Task.Delay(Mathf.RoundToInt(Time.deltaTime * 1000), scale_cts.Token);
            ScaleLoop(changeScaleData, isFinal);
        }
    }
}

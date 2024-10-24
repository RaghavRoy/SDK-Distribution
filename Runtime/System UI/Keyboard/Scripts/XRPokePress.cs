using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.State;
using UnityEngine.XR.Interaction.Toolkit.Filtering;
using UnityEngine;
using TMPro;


namespace JioXSDK
{
    public class XRPokePress : MonoBehaviour
    {
        [SerializeField] private Transform _followTransform;

        [SerializeField] private float _smoothSpeed = 9f;

        [SerializeField] private float _maxDistance = 20f;

        private Vector3 _initPos;
        private Vector3 _targetPos;
        private IPokeStateDataProvider _pokeStateDataProvider;

        // [SerializeField] private Transform _textfollowTransform;

        // [SerializeField] private Transform _hoverfollowTransform;

        private void Awake() => _pokeStateDataProvider = GetComponentInParent<IPokeStateDataProvider>();

        private void Start()
        {
            if (_followTransform == null)
                return;

            _initPos = _followTransform.localPosition;
            _pokeStateDataProvider.pokeStateData.SubscribeAndUpdate(OnPokeDataUpdated);
        }

        private void LateUpdate()
        {
            UpdateTransformPosition();
        }

        private void UpdateTransformPosition()
        {
            _followTransform.localPosition =
                SmoothPosition(_followTransform.localPosition, _targetPos);
            // _hoverfollowTransform.localPosition = _followTransform.localPosition;
            // _textfollowTransform.localPosition = _followTransform.localPosition;
        }

        private Vector3 SmoothPosition(Vector3 initPos, Vector3 targetPos)
        {
            var interpolateTime = Time.deltaTime * _smoothSpeed;
            return Vector3.Lerp(initPos, targetPos, interpolateTime);
        }

        private void OnPokeDataUpdated(PokeStateData data)
        {
            var pokeTransform = data.target;
            var hasToFollowPoke = pokeTransform != null && pokeTransform.IsChildOf(transform);

            if (hasToFollowPoke)
            {
                var position = pokeTransform.InverseTransformPoint(data.axisAlignedPokeInteractionPoint);
                var maxDistanceReached = position.sqrMagnitude > Mathf.Sqrt(_maxDistance);
                if (maxDistanceReached)
                    position = Vector3.ClampMagnitude(position, _maxDistance);

                _targetPos = position;

            }
            else
                _targetPos = _initPos;
        }
    }
}

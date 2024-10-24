using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
namespace JioXSDK
{
    public abstract class Keybutton : MonoBehaviour, IPointerExitHandler, IPointerDownHandler,
        IPointerUpHandler, IPointerEnterHandler
    {
        [FormerlySerializedAs("_selectButton")][SerializeField] private GameObject _selectState;
        [SerializeField] private GameObject _hoverState;
        [SerializeField] private AudioSource _audioSource;
        public UnityEvent<Keybutton> inputEvent;

        private void OnDisable()
        {
            if (_selectState) _selectState.SetActive(false);
            if (_hoverState) _hoverState.SetActive(false);
        }


        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            if (_hoverState) _hoverState.SetActive(true);

        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            if (_selectState) _selectState.SetActive(false);
            if (_hoverState) _hoverState.SetActive(false);
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            if (_selectState)
            {
                _selectState.SetActive(true);

            };
            if (_audioSource) _audioSource.Play();
            inputEvent?.Invoke(this);
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            if (_selectState) _selectState.SetActive(false);
            if (_hoverState) _hoverState.SetActive(false);
        }
    }

    public enum KeySpecial
    {
        None,
        Delete,
        Shift,
        Enter,
        DeleteAll,
        SwitchObject,
        Hide
    }

}

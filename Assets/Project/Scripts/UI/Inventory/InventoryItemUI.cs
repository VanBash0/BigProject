using BigProject.Systems;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace BigProject.UI
{
    public class InventoryItemUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
    {
        [SerializeField] private Image _image;

        private Transform _defaultParent;
        private Camera _camera;
        private Item _item;
        private GameObject _noteObject;

        public event Action<bool> OnStartDrag;

        private void Awake()
        {
            _defaultParent = transform.parent;
        }

        public void SetItem(Item item, Camera camera, Image noteImage)
        {
            _image.sprite = item._itemSprite;
            _camera = camera;
            _item = item;
            if (item._noteSprite != null && noteImage != null)
            {
                noteImage.sprite = item._noteSprite;
                _noteObject = noteImage.gameObject;
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            transform.SetParent(transform.root); //, false); - leads to change of scale
            transform.SetAsLastSibling();
            _image.raycastTarget = false;
            OnStartDrag?.Invoke(true);
        }

        public void OnDrag(PointerEventData eventData)
        {
            transform.position = Mouse.current.position.ReadValue();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Ray ray = _camera.ScreenPointToRay(mousePosition);
            
            // hit something
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.TryGetComponent<IUsesItem>(out IUsesItem interactableObject))
                {
                    if (interactableObject.DoesUseItem(_item))
                    { 
                        interactableObject.UseItem(_item);
                        return;
                    }
                }
            }

            // didn't hit anything, returning item to its inventory slot
            transform.SetParent(_defaultParent);
            _image.raycastTarget = true;
            OnStartDrag?.Invoke(false);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_noteObject == null)
            {
                return;
            }

            _noteObject.SetActive(!_noteObject.activeInHierarchy);
        }

        private void OnDestroy()
        {
            if (_noteObject != null)
            {
                _noteObject = null;
            }
        }
    }
}
using System;
using Core;
using Cysharp.Threading.Tasks;
using Features;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CustomBehaviours.Elements
{
    public class Model3DView : IModelView , IPointerEnterHandler , IPointerExitHandler , IPointerMoveHandler
    {
        [SerializeField] CameraFocus camera;
        [SerializeField] private RawImage image;
        private Camera _camera;
        private LogHoverSystem _logHoverSystem => Locator<LogHoverSystem>.Instance;
        private void Start()
        {
            Locator<ManagerDataViewService>.Instance.RegisterOnFileChanged(OnModelChanged);
            _camera = camera.GetComponent<Camera>();
        }
        
        private void OnDestroy()
        {
            Locator<ManagerDataViewService>.Instance.UnregisterOnFileChanged(OnModelChanged);
        }

        public override void OnModelChanged(FileModel model)
        {
            var target = Locator<ManagerDataViewService>.Instance.transform;
            camera.SetTarget(target);
            SetupView(false).Forget();
        }
        
        public override async UniTask SetupView(bool enable = true)
        {
            Locator<ManagerDataViewService>.Instance.CutView(ViewType.Model3D, 0);
            camera.GetComponent<Camera>().enabled = true;
            await UniTask.NextFrame();
            camera.GetComponent<Camera>().enabled = enable;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            SetupView().Forget();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            camera.GetComponent<Camera>().enabled = false;
        }

        public void OnPointerMove(PointerEventData eventData)
        {
            RectTransform rectTransform = image.GetComponent<RectTransform>();
            Vector2 localPoint;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    rectTransform, 
                    eventData.position, 
                    eventData.pressEventCamera, 
                    out localPoint))
            {
                // Normalize local point to 0..1 range
                Vector2 size = rectTransform.rect.size;
                Vector2 normalizedPoint = new Vector2(
                    (localPoint.x + size.x * 0.5f) / size.x,
                    (localPoint.y + size.y * 0.5f) / size.y);

                // Invert Y if necessary
                normalizedPoint.y = 1.0f - normalizedPoint.y;

                // Create a ray from the camera through the mouse position
                Ray ray = _camera.ViewportPointToRay(normalizedPoint);

                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    Debug.Log("Hit object: " + hit.collider.gameObject.name);
                    _logHoverSystem.Log(hit.collider.gameObject.name);
                    // Handle the hit object as needed
                }
            }
        }
    }
}
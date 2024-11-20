using System;
using Core;
using Cysharp.Threading.Tasks;
using Features;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CustomBehaviours.Elements
{
    public class Model2DView : IModelView, IPointerEnterHandler , IPointerExitHandler, IPointerMoveHandler
    {
        [SerializeField] private ViewType viewType;
        [SerializeField] private Camera targetCamera;
        [SerializeField] private RawImage image;
        [SerializeField] private Scrollbar slider;
        [SerializeField] private int MaxSlice = 512;
        [SerializeField] private TMP_Text maxSliceText;
        [SerializeField] private int width = 900;
        [SerializeField] private int height = 500;
        
        private Rect renderTextureRect;
        
        private ManagerDataViewService _managerDataViewService=> Locator<ManagerDataViewService>.Instance;
        private LogHoverSystem _logHoverSystem => Locator<LogHoverSystem>.Instance;
        private void Start()
        {
            _managerDataViewService.RegisterOnFileChanged(OnModelChanged);
            
            var renderTexture = new RenderTexture(width, height, 24);
            targetCamera.targetTexture = renderTexture;
            image.texture = renderTexture;
            slider.onValueChanged.AddListener(OnValueChanged);
            maxSliceText.text = $"{Mathf.RoundToInt(slider.value*MaxSlice)}";
            renderTextureRect = new Rect(image.rectTransform.position.x - image.rectTransform.rect.width/2,
                image.rectTransform.position.y-image.rectTransform.rect.height/2,
                image.rectTransform.rect.width, image.rectTransform.rect.height);
        }
        //
        // void Update()
        // {
        //     Vector2 mousePosition = Input.mousePosition;
        //     mousePosition.y = Screen.height - mousePosition.y; // Invert Y axis
        //
        //     if (renderTextureRect.Contains(mousePosition))
        //     {
        //         Debug.Log("Mouse is over the render texture");
        //         float normalizedX = (mousePosition.x - renderTextureRect.x) / renderTextureRect.width;
        //         float normalizedY = (mousePosition.y - renderTextureRect.y) / renderTextureRect.height;
        //
        //         // Adjust Y coordinate if necessary
        //         normalizedY = 1.0f - normalizedY;
        //
        //         Vector3 viewportPoint = new Vector3(normalizedX, normalizedY, 0);
        //
        //         Ray ray = targetCamera.ViewportPointToRay(viewportPoint);
        //
        //         if (Physics.Raycast(ray, out RaycastHit hit))
        //         {
        //             Debug.Log("Hit object: " + hit.collider.gameObject.name);
        //             // Handle the hit object as needed
        //         }
        //     }
        // }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(renderTextureRect.center, renderTextureRect.size);
        }

        private void OnValueChanged(float value)
        {
            _managerDataViewService.CutView(viewType, value*MaxSlice);
            maxSliceText.text = $"{Mathf.RoundToInt(value*MaxSlice)}";

        }

        private void OnDestroy()
        {
            _managerDataViewService.UnregisterOnFileChanged(OnModelChanged);
        }

        public override void OnModelChanged(FileModel model)
        {
            SetupView(false).Forget();
        }

        public override async UniTask SetupView(bool enable = true)
        {
            _managerDataViewService.CutView(viewType, slider.value*512);
            targetCamera.GetComponent<Camera>().enabled = true;
            await UniTask.NextFrame();
            targetCamera.GetComponent<Camera>().enabled = enable;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            SetupView().Forget();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            targetCamera.GetComponent<Camera>().enabled = false;
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
                Ray ray = targetCamera.ViewportPointToRay(normalizedPoint);

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
using System;
using Core;
using Features;
using UnityEngine;

namespace CustomBehaviours
{
    public class MainContainerController : MonoBehaviour
    {
        [SerializeField]private RectTransform model3DView;
        [SerializeField]private RectTransform[] model2DView;
        
        private void Awake()
        {
        }
        
        private void Start()
        {
            
        }
        
        public void OnVisible3DSwitchChanged(bool visible)
        {
            model3DView.gameObject.SetActive(visible);
        }
        
        public void OnVisible2DSwitchChanged(bool visible)
        {
            foreach (var view in model2DView)
            {
                view.gameObject.SetActive(visible);
            }
            
            model3DView.anchorMax = visible ? new Vector2(0.5f, 1) : new Vector2(1, 1);
            model3DView.anchorMin = visible ? new Vector2(0f, 0.5f) : new Vector2(0, 0);
            
        }
    }
}
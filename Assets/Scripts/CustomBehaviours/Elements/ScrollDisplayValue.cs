using TMPro;
using UnityEngine;

namespace CustomBehaviours.Elements
{
    public class ScrollDisplayValue: MonoBehaviour
    {
        [SerializeField] private TMP_Text valueMax;
        [SerializeField] private TMP_Text valueMin;
        
        public void SetMaxValue(int value)
        {
            valueMax.text = value.ToString();
        }
        
        public void SetMinValue(int value)
        {
            valueMin.text = value.ToString();
        }
        
        public void SetValues(int min, int max)
        {
            SetMinValue(min);
            SetMaxValue(max);
        }
    }
}
using System;
using Core;
using TMPro;
using UnityEngine;

namespace Features
{
    public class LogHoverSystem : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;

        private void Awake()
        {
            Locator<LogHoverSystem>.Set(this);
            Log("");
        }

        public void Log(string message)
        {
            _text.text = message;
        }
    }
}
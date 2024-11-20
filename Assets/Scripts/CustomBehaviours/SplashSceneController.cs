using System;
using Commands;
using Core;
using Features;
using UnityEngine;

namespace CustomBehaviours
{
    public class SplashSceneController : MonoBehaviour
    {
        private void Start()
        {
            LoadMainScreen();
        }
        
        private async void LoadMainScreen()
        {
            await new LoadMainScreenCommand().ExecuteAsync();
        }
    }
}
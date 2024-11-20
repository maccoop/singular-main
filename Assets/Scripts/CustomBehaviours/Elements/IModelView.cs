using System;
using Cysharp.Threading.Tasks;
using Features;
using UnityEngine;

namespace CustomBehaviours.Elements
{
    public class IModelView : MonoBehaviour
    {
         public virtual void OnModelChanged(FileModel model) {}
         public virtual async UniTask SetupView(bool enable = true) {}
    }
}
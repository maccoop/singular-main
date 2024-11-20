using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Features
{
    public class ManagerDataViewService : MonoBehaviour
    {
        Material _material;
        public List<FileModel> Files { get; set; }
        
        private Action<FileModel> _onFileChanged;
        
        private int currentFileIndex = 0;
        private static readonly int Plane1Normal = Shader.PropertyToID("_PlaneNormal");
        private static readonly int Plane1Position = Shader.PropertyToID("_PlanePosition");
        
        Dictionary<string,Color> _modelsMap ;
        private static readonly int Color1 = Shader.PropertyToID("_Color");
        private static readonly int CrossColor = Shader.PropertyToID("_CrossColor");
        

        private void Awake()
        {
            Locator<ManagerDataViewService>.Set(this);
        }

        public void Start()
        {
            Files = new List<FileModel>();
            _modelsMap = new Dictionary<string, Color>();
        }
        
        public void AddFile(FileModel file)
        {
            lock (Files)
            {
                file.Model.transform.parent = transform;
                Renderer renderer = file.Model.GetComponentInChildren<Renderer>();
                MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
                renderer.GetPropertyBlock(propertyBlock);

                string key = file.Name.Split("_")[0];
                if (!_modelsMap.ContainsKey(key))
                {
                    var newColor =  UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 1f, 1f);
                    _modelsMap.Add(key, newColor);
                }
                var color = _modelsMap[key];
                propertyBlock.SetColor(Color1, color);
                propertyBlock.SetColor(CrossColor, color);
                renderer.SetPropertyBlock(propertyBlock);
                
                file.Model.AddComponent<MeshColliderAdder>();
                
                Files.Add(file);
                _onFileChanged?.Invoke(null);
            }
        }

        public void StartView()
        {
            Debug.Log("Starting view...");
            if (Files.Count > 0)
            {
                _onFileChanged?.Invoke(Files.First());
            }

            foreach (var file in Files)
            {
                file.Model.SetActive(true);
            }
        }
        
        public void RegisterOnFileChanged(Action<FileModel> action)
        {
            _onFileChanged += action;
        }
        
        public void UnregisterOnFileChanged(Action<FileModel> action)
        {
            _onFileChanged -= action;
        }

        public void CutView(ViewType viewType, float sliderValue)
        {
            if (_material == null)
            {
                if (Files.Count == 0) return;
                _material = Files.First().Model.GetComponentInChildren<Renderer>().sharedMaterial;
            }

            Vector4 cuttingPlane = Vector3.left;
            var position = Vector3.left * sliderValue;
            if(viewType == ViewType.Transversal)
            {
                cuttingPlane = Vector3.up;
                position = Vector3.up * sliderValue;
            }
            else if(viewType == ViewType.Coronal)
            {
                cuttingPlane = Vector3.forward;
                position = Vector3.forward * sliderValue;
            }
            else if(viewType == ViewType.Sagittal)
            {
                cuttingPlane = Vector3.left;
                position = Vector3.left * sliderValue * -1f;
            }
            _material.SetVector(Plane1Normal, cuttingPlane);
            _material.SetVector(Plane1Position, position);
        }
    }
}
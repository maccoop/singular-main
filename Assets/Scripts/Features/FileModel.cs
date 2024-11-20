using UnityEngine;

namespace Features
{
    public class FileModel
    {
        public GameObject Model { get; set; }
        public string Name { get; set; }
        public TextureMap Sagittal { get; set; }
        public TextureMap Coronal { get; set; }
        public TextureMap Transverse { get; set; }
    }

    public struct TextureMap
    {
        public string Name;
        public Texture2D Texture;
    }
}
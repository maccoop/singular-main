using UnityEngine;
using UnityEngine.UI;

namespace CustomBehaviours
{
    public class CameraRenderTexture : MonoBehaviour
    {
        public Camera targetCamera; // The camera to render to texture
        public RawImage targetRawImage; // The UI RawImage to display the texture
        public int textureWidth = 1920; // Width of the render texture
        public int textureHeight = 1080; // Height of the render texture

        void Start()
        {
            // Create a new RenderTexture
            RenderTexture renderTexture = new RenderTexture(textureWidth, textureHeight, 24);
        
            // Assign the RenderTexture to the camera
            targetCamera.targetTexture = renderTexture;
        
            // Assign the RenderTexture to the RawImage
            targetRawImage.texture = renderTexture;
        }
    }
}
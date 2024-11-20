using UnityEngine;

namespace Features
{
    public class MeshColliderAdder : MonoBehaviour
    {
        void Start()
        {
                // Add the MeshCollider component
                MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();

                // Assign the mesh to the collider
                MeshFilter meshFilter = gameObject.GetComponentInChildren<MeshFilter>();
                if (meshFilter != null)
                {
                    meshCollider.sharedMesh = meshFilter.mesh;
                }
                else
                {
                    SkinnedMeshRenderer skinnedMeshRenderer = gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
                    if (skinnedMeshRenderer != null)
                    {
                        meshCollider.sharedMesh = skinnedMeshRenderer.sharedMesh;
                    }
                    else
                    {
                        Debug.LogError("No MeshFilter or SkinnedMeshRenderer found on the GameObject.");
                    }
                }

                // Optional settings
                meshCollider.convex = true; // Set to true if needed
                meshCollider.isTrigger = true; // Set to true if using as a trigger
            
        }
    }
}
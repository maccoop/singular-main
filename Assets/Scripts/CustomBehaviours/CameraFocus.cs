using Core;
using UnityEngine;

namespace CustomBehaviours
{
    [RequireComponent(typeof(Camera))]
    public class CameraFocus : MonoBehaviour
    {
        public Transform target; // The object to focus on
        public float distance = 5.0f; // Initial distance from the target
        public float xSpeed = 120.0f; // Rotation speed around the X axis
        public float ySpeed = 120.0f; // Rotation speed around the Y axis
        public float yMinLimit = -20f; // Minimum vertical angle
        public float yMaxLimit = 80f; // Maximum vertical angle
        public float distanceMin = 1.0f; // Minimum distance from target
        public float distanceMax = 15.0f; // Maximum distance from target
        public float smoothTime = 2f; // Smoothing factor
        public float padding = 1.1f; // Padding to ensure object fits in view
        public Vector3 offset = Vector3.zero;

        public ViewType viewType;

        private float currentDistance;
        private float desiredDistance;
        private float x = 0.0f;
        private float y = 0.0f;

        private Camera _camera;

        void Start()
        {
            _camera = GetComponent<Camera>();
            if (target == null)
            {
                Debug.LogWarning("CameraOrbit script requires a target object.");
                return;
            }

            // Initialize angles based on current rotation
            Vector3 angles = transform.eulerAngles;
            x = angles.y;
            y = angles.x;

            // Calculate initial distance to fit the object in view
            desiredDistance = CalculateInitialDistance();
            currentDistance = desiredDistance;

            // Position the camera
            UpdateCameraPosition();
        }

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }

        void LateUpdate()
        {
            if (target == null)
                return;

            // Get user input
            float inputX = Input.GetAxis("Horizontal");
            float inputY = Input.GetAxis("Vertical");

            // Adjust rotation angles based on input
            x += inputX * xSpeed * Time.deltaTime;
            y -= inputY * ySpeed * Time.deltaTime; // Invert Y input for intuitive control

            // Clamp vertical rotation
            y = ClampAngle(y, yMinLimit, yMaxLimit);

            // Adjust distance with mouse scroll wheel (optional)
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            desiredDistance -= scroll * Time.deltaTime * 1000f; // Adjust scroll sensitivity
            desiredDistance = Mathf.Clamp(desiredDistance, distanceMin, distanceMax);

            // Smoothly interpolate distance
            currentDistance = Mathf.Lerp(currentDistance, desiredDistance, Time.deltaTime * smoothTime);

            // Update camera position
            UpdateCameraPosition();
        }

        void UpdateCameraPosition()
        {
            Quaternion rotation = Quaternion.Euler(y, x, 0);

            if (viewType == ViewType.Coronal)
                rotation = Quaternion.Euler(0, 180, 0);
            else if (viewType == ViewType.Sagittal)
                rotation = Quaternion.Euler(0, 90, 0);
            else if (viewType == ViewType.Transversal)
                rotation = Quaternion.Euler(90, 0, 0);
            // Convert angles to rotation

            // Calculate position based on rotation and distance
            Vector3 negDistance = new Vector3(0.0f, 0.0f, -currentDistance);
            Vector3 position = rotation * negDistance + target.position;

            // Set camera rotation and position


            transform.rotation = rotation;
            transform.position = position + offset;

            // Ensure the object fits within the camera's view
            AdjustCameraToFitObject();
        }

        float CalculateInitialDistance()
        {
            Bounds bounds = CalculateBounds(target);
            float objectSize = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
            float distance = (objectSize * padding) / Mathf.Tan(Mathf.Deg2Rad * _camera.fieldOfView / 2.0f);
            distance = Mathf.Clamp(distance, distanceMin, distanceMax);
            return distance;
        }

        void AdjustCameraToFitObject()
        {
            // Recalculate distance to ensure the object fits in view
            Bounds bounds = CalculateBounds(target);
            float objectSize = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
            float requiredDistance = (objectSize * padding) / Mathf.Tan(Mathf.Deg2Rad * _camera.fieldOfView / 2.0f);

            desiredDistance = Mathf.Clamp(requiredDistance, distanceMin, distanceMax);
        }

        Bounds CalculateBounds(Transform target)
        {
            var renderers = target.GetComponentsInChildren<Renderer>();

            if (renderers.Length == 0)
            {
                return new Bounds(target.position, Vector3.zero);
            }

            Bounds bounds = renderers[0].bounds;

            foreach (Renderer renderer in renderers)
            {
                bounds.Encapsulate(renderer.bounds);
            }

            return bounds;
        }

        float ClampAngle(float angle, float min, float max)
        {
            return angle;
            angle = Mathf.Repeat(angle + 360f, 360f);
            return Mathf.Clamp(angle, min, max);
        }
    }
}
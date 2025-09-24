using UnityEngine;

namespace Resoulnance.Scene_Arena.Camera
{
    public class CameraFollow : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private Vector3 offset = new Vector3(0f, 7f, -9f);
        [SerializeField] private Transform target; // Referência ao jogador

        [Header("Suavizar camera")]
        [SerializeField] private bool useSmoothFollow = true;
        [SerializeField] private float smoothSpeed = 5f;

        private void LateUpdate()
        {
            if (target == null) return;

            Vector3 desiredPosition = target.position + offset;

            if (useSmoothFollow)
            {
                Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
                transform.position = smoothedPosition;
            }
            else
            {
                transform.position = desiredPosition;
            }
        }

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }
    }
}

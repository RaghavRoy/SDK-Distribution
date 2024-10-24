using UnityEngine;

namespace JioXSDK.FOVSample
{
    public class Test : MonoBehaviour
    {
        [SerializeField] private Transform point;
        [Space]
        [SerializeField] private Transform lookAtPointer;
        [SerializeField] private Transform computePointer;

        private void OnDrawGizmos()
        {

            float rayVisibleDuration = 0.1f;
            lookAtPointer.LookAt(point);
            computePointer.rotation = Quaternion.LookRotation(point.position - computePointer.position);

            Debug.DrawRay(lookAtPointer.position, lookAtPointer.forward * 100, Color.blue, rayVisibleDuration);
            Debug.DrawRay(computePointer.position, computePointer.forward * 100, Color.red, rayVisibleDuration);
        }
    }
}
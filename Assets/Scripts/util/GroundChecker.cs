using UnityEngine;

namespace util
{
    public class GroundChecker : MonoBehaviour
    {
        public bool isGrounded;

        void OnTriggerEnter(Collider col)
        {
            if (col.gameObject.CompareTag("Ground"))
            {
                isGrounded = true;
            }
        }

        void OnTriggerExit(Collider col)
        {
            if (col.gameObject.CompareTag("Ground"))
            {
                isGrounded = false;
            }
        }
    }
}

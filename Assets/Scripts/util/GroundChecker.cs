using System;
using characters.scriptables;
using UnityEngine;

namespace util
{
    public class GroundChecker : MonoBehaviour
    {
        private Character character;
        private AudioManager audioManager;
        public bool isGrounded;

        void OnTriggerEnter(Collider col)
        {
            if (col.gameObject.CompareTag("Ground") || (col.gameObject.tag.Contains("P") && col.gameObject.tag.Length == 2))
            {
                isGrounded = true;
                audioManager.Play(AudioType.JUMP_END);
            }
        }

        private void OnTriggerStay(Collider col) {
            if (col.gameObject.CompareTag("Ground") || (col.gameObject.tag.Contains("P") && col.gameObject.tag.Length == 2))
            {
                isGrounded = true;
            }
        }

        void OnTriggerExit(Collider col)
        {
            if (col.gameObject.CompareTag("Ground") || (col.gameObject.tag.Contains("P") && col.gameObject.tag.Length == 2))
            {
                isGrounded = false;
            }
        }

        internal void Init(Character character, AudioManager audioManager)
        {
            this.audioManager = audioManager;
            this.character = character;
        }
    }
}

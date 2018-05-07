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
            if (col.gameObject.CompareTag("Ground"))
            {
                isGrounded = true;
                audioManager.Play(AudioType.JUMP_END);
            }
        }

        void OnTriggerExit(Collider col)
        {
            if (col.gameObject.CompareTag("Ground"))
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityPUBG.Scripts.Entities
{
    public class PlayerSoundPlayer : MonoBehaviour
    {
        public enum SoundType { Hit, Shoot, MeleeAttack}

        [SerializeField]
        private AudioSource effectSoundPlayer;

        [SerializeField]
        private AudioClip hitSound;
        [SerializeField]
        private AudioClip shootSound;
        [SerializeField]
        private AudioClip meleeAttackSound;

        private void Awake()
        {
            if(effectSoundPlayer == null)
            {
                Debug.Log("No Audio Source");
            }
        }

        public void PlayEffectSound(SoundType soundType)
        {
            switch(soundType)
            {
                case SoundType.Hit:
                    effectSoundPlayer.PlayOneShot(hitSound);
                    break;
                case SoundType.Shoot:
                    effectSoundPlayer.PlayOneShot(shootSound);
                    break;
                case SoundType.MeleeAttack:
                    effectSoundPlayer.PlayOneShot(meleeAttackSound);
                    break;
            }
        }
    }
}
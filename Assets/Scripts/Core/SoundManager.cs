using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace w4ndrv.Sound
{
    using FishNet.Object;

    public enum EBackgroundType
    {
        None,
        StartGame,
        InGame
    }

    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance;
        [Header("Background Sound")]
        [SerializeField] private AudioSource _backgroundSource;
        [SerializeField] private AudioClip _audioStartGame;
        [SerializeField] private AudioClip _audioInGame;

        [Header("SFX")]
        [SerializeField] private AudioSource _sfxSource;

        // Start is called before the first frame update
        void Start()
        {
            if (Instance == null)
                Instance = this;

            PlayBackground(EBackgroundType.StartGame);
        }

        public void PlayBackground(EBackgroundType backgroundType)
        {
            _backgroundSource.Stop();
            switch (backgroundType)
            {
                case EBackgroundType.StartGame:
                    _backgroundSource.clip = _audioStartGame;
                    break;
                case EBackgroundType.InGame:
                    _backgroundSource.clip = _audioInGame;
                    break;
            }
            _backgroundSource.Play();
        }

        public void PlaySFX(AudioClip audioClip)
        {
            _sfxSource.PlayOneShot(audioClip);
        }
    }
}

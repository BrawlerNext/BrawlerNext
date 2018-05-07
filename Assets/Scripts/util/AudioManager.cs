using characters.scriptables;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AudioManager
{
    private AudioSource soundSource;
    private Character character;

    public AudioManager(AudioSource soundSource, Character character)
    {
        this.soundSource = soundSource;
        this.character = character;
    }

    public void Play(AudioType type)
    {
        if (type != AudioType.NONE)
        {
            foreach (AudioEntry audioEntry in character.clips)
            {
                if (audioEntry.Type == type)
                {
                    soundSource.PlayOneShot(audioEntry.Clip);
                    return;
                }
            }

            Debug.LogWarning("Audio type: " + type.ToString() + " not found in " + character.ToString() + " clips!");
        }
    }

    internal void PlayOnce(AudioType type)
    {
        if (type != AudioType.NONE)
        {
            foreach (AudioEntry audioEntry in character.clips)
            {
                if (audioEntry.Type == type)
                {
                    if (!soundSource.isPlaying) {
                        soundSource.clip = audioEntry.Clip;
                        soundSource.Play();
                    }
                    return;
                }
            }

            Debug.LogWarning("Audio type: " + type.ToString() + " not found in " + character.ToString() + " clips!");
        }
    }
}

[Serializable]
public class AudioEntry
{
    public AudioType Type;
    public AudioClip Clip;
}

public enum AudioType
{
    SOFT_HIT,
    HARD_HIT,
    SHIELD_UP,
    SOFT_PUNCH,
    HARD_PUNCH,
    JUMP,
    DEATH,
    BURN,
    SHIELD_HIT,
    SHIELD_DOWN,
    NONE,
    DASH,
    RUN,
    JUMP_END
}
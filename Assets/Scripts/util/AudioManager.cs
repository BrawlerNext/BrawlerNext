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
    DEFEND,
    SOFT_PUNCH,
    HARD_PUNCH,
    JUMP,
    DEATH,
    BURN,
    NONE
}

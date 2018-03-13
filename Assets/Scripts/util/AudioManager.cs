using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AudioManager : MonoBehaviour
{
	
	public AudioEntry[] Clips;
	
	private AudioSource SoundSource;

	private static AudioManager instance;

	private void Awake()
	{
		AudioManager.instance = new AudioManager();

		instance.Clips = Clips;
		instance.SoundSource = GameObject.FindObjectOfType<AudioSource>(); 
	}

	public static AudioManager Instance()
	{
		return instance;
	}

	public static void Play(AudioType type)
	{
		foreach (AudioEntry audioEntry in instance.Clips)
		{
			if (audioEntry.Type == type)
			{
				instance.SoundSource.PlayOneShot(audioEntry.Clip);
			}
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
	HIT,
	DEFEND,
	PUNCH,
	JUMP,
	DEATH
}

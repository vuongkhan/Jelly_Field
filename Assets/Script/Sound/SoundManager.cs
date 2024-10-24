using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public AudioClip[] soundEffects;
    private Dictionary<string, AudioClip> soundEffectDict;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeSoundEffects();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeSoundEffects()
    {
        soundEffectDict = new Dictionary<string, AudioClip>();

        foreach (AudioClip clip in soundEffects)
        {
            if (clip != null)
            {
                soundEffectDict.Add(clip.name, clip);
            }
        }
    }

    private AudioSource CreateTemporaryAudioSource()
    {
        GameObject audioObject = new GameObject("TempAudio");
        AudioSource audioSource = audioObject.AddComponent<AudioSource>();
        Destroy(audioObject, 5f);
        return audioSource;
    }

    public void PlaySoundEffect(string soundName)
    {
        if (soundEffectDict.ContainsKey(soundName))
        {
            AudioClip clip = soundEffectDict[soundName];
            if (clip != null)
            {
                AudioSource tempSource = CreateTemporaryAudioSource();
                tempSource.PlayOneShot(clip);
            }
        }
    }
}

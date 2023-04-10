using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance = null;

    public static AudioManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = (AudioManager)FindObjectOfType(typeof(AudioManager));
            }
            return instance;
        }
    }

    [SerializeField] private AudioCollection audioCollection;
    private List<AudioSource> sfxPool = new List<AudioSource>();
    private AudioMixerGroup[] groups => audioCollection.audioMixer.FindMatchingGroups(string.Empty);

   public List<AudioSource> SfxPool { get { return sfxPool; } } 


    void Start()
    {
        for (int i = 0; i < 21; i++)
        {
            AddAudioSource("SFX", false, 1.0f, true, false, sfxPool);
        }
    }

    
    void Update()
    {
        
    }



    public void PlayOneShotSound(List<AudioSource> list, AudioClip clip, Transform soundOrigin)
    {
        bool audioIsAvailable = false;

        foreach (AudioSource source in list)
        {
            if (source.isPlaying == false)
            {
                audioIsAvailable = true;
                source.name = clip.name;
                source.gameObject.transform.position = soundOrigin.position;
                source.clip = clip;
                source.Play();
                break;
            }
        }
        if (audioIsAvailable == false)
        {
            AudioSource.PlayClipAtPoint(clip, soundOrigin.position);
            print("Stop playing with my patience");
        }
    }

    private void AddAudioSource(string mixerGroup, bool awake, float spatialBlend, bool spatialize, bool loop, List<AudioSource> list)
    {
        GameObject go = new GameObject(mixerGroup);
        AudioSource sfxAudioSource = go.AddComponent<AudioSource>();
        go.transform.parent = transform;

        foreach (AudioMixerGroup group in groups)
        {
            if (group.name == mixerGroup)
            {
                sfxAudioSource.outputAudioMixerGroup = group;
            }
        }
        sfxAudioSource.playOnAwake = awake;
        sfxAudioSource.spatialBlend = spatialBlend;
        sfxAudioSource.spatialize = spatialize;
        sfxAudioSource.loop = loop;
        list.Add(sfxAudioSource);

    }
}

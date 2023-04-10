using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;

[CreateAssetMenu(menuName = "AudioCollection")]
public class AudioCollection : ScriptableObject
{
    public AudioMixer audioMixer = null;    

    public List<AudioClip> fireSound = new List<AudioClip>();

    public List<AudioClip> reloadSound = new List<AudioClip>();
}

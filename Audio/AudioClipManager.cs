using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioClipManager : MonoBehaviour
{
    [System.Serializable]
    public struct ClipLibEntry
    {
        public string Key;
        public AudioClip Audio;
    }

    [HideInInspector]
    public AudioSource Audio;

    public Dictionary<string, AudioClip> SoundLibrary;
    public List<ClipLibEntry> SoundLibraryEntries;

    private void Start() {
        Audio = gameObject.GetComponent<AudioSource>();

        SoundLibrary = new Dictionary<string, AudioClip>();

        foreach (ClipLibEntry clip in SoundLibraryEntries) {
            SoundLibrary.Add(clip.Key, clip.Audio);
        }
    }

    public void PlayBGM(AudioClip song) {
        Audio.Stop();
        Audio.clip = song;
        Audio.Play();
    }

    public void StopBGM() {
        Audio.Stop();
    }

    public void PlaySFX(AudioClip clip, float volume = 1) {
        Audio.PlayOneShot(clip, Mathf.Clamp(volume, 0, 1));
    }

    public void PlaySFX(string sound, float volume = 1) {
        PlaySFX(SoundLibrary[sound], volume);
    }

    public void SetVolume(float volume) {
        volume = Mathf.Clamp(volume, 0, 1);
        Audio.volume = volume;
    }
}
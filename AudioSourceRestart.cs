using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;

public class AudioSourceRestart : MonoBehaviour
{
    [SerializeField] private Component[] coms;

    private void Start()
    {
        AudioSource[] own = GetComponents<AudioSource>();
        AudioSource[] children = GetComponentsInChildren<AudioSource>();
        var list = new List<Component>();
        foreach (var s in own)
            if (s != null) list.Add(s);
        foreach (var s in children)
            if (s != null && !list.Contains(s)) list.Add(s);
        coms = list.ToArray();
        AudioSettings.OnAudioConfigurationChanged += OnAudioConfigurationChanged;

    }

    async void OnAudioConfigurationChanged(bool deviceWasChanged)
    {
        StartCoroutine(RestartAudioSourcesCoroutine());
        return;
        if (deviceWasChanged)
        {
            Debug.Log("Audio Configuration Changed!");
            RecreateAudioClip();
            foreach (AudioSource com in coms)
            {
                if (com != null)
                {
                    com.Play();
                }
            }
            Debug.Log("AudioSources Restarted!");
        }
    }

    IEnumerator RestartAudioSourcesCoroutine()
    {
        yield return new WaitForSeconds(1f);
        foreach (AudioSource com in coms)
        {
            if (com != null && com.playOnAwake)
            {
                com.Stop();
                com.Play();
            }
        }
        Debug.Log("AudioSources Restarted after delay!");
    }

    async void RecreateAudioClip()
    {
        foreach (AudioSource com in coms)
        {
            if (com != null && com.clip != null)
            {
                if (com.clip.loadState != AudioDataLoadState.Loaded || com.clip.samples == 0)
                {
                    Debug.LogWarning("Clip not loaded or has 0 samples. Cannot clone.");
                    return;
                }

                float[] samples = new float[com.clip.samples * com.clip.channels];
                com.clip.GetData(samples, 0);

                AudioClip newClip = AudioClip.Create(
                    com.clip.name + "_clone",
                    com.clip.samples,
                    com.clip.channels,
                    com.clip.frequency,
                    false
                );

                newClip.SetData(samples, 0);
                com.clip = newClip;
            }

        }
        Debug.Log("AudioSources Recreated!");
    }
}
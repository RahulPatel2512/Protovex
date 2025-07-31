using System;
using System.Collections.Generic;
using Game.Scripts.Utility;
using UnityEngine;

[DefaultExecutionOrder(-99)]
public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource Prefab;
    [SerializeField] private List<AudioClip> Clips = new();

    private AudioSource Music;
    [SerializeField] private List<AudioSource> Sounds = new();
    private Dictionary<string, AudioRuntime> Runtimes = new();

    private void Awake()
    {
        Runtimes = Clips.ToDictionary(o => o.name.ToLower(), o => new AudioRuntime { Clip = o });
    }

    private void Update()
    {
        foreach (var (name, runtime) in Runtimes) runtime.Delay -= Time.deltaTime;
    }

    public void SetPaused(bool paused)
    {
        foreach (var sfx in Sounds)
        {
            if (!sfx.loop) continue;

            if (paused) sfx.Pause();
            else sfx.UnPause();
        }

        if (paused) Music?.Pause();
        else Music?.UnPause();
    }

    public AudioSource PlayMusic(string name, float volume = 1f)
    {
        var runtime = Runtimes.GetValueOrDefault(name.ToLower());
        if (runtime == null) return null;

        if (Music != null)
        {
            var source = Music;
        }

        {
            var source = Instantiate(Prefab, transform);
            source.gameObject.SetActive(true);
            source.name = name;
            source.clip = runtime.Clip;
            source.loop = true;
            source.Play();
            source.mute = IsMusicMuted;
            source.volume = volume;
            Music = source;

            return source;
        }
    }

    public AudioSource PlaySound(string name, bool loop = false, float repeatDelay = 0)
    {
        var runtime = Runtimes.GetValueOrDefault(name.ToLower());
        if (runtime == null) return null;

        if (runtime.Delay <= 0)
        {
            runtime.Delay = repeatDelay;
            var source = Prefab.Spawn(transform);
            source.name = name;
            source.clip = runtime.Clip;
            source.loop = loop;
            source.Play();
            source.mute = IsSoundMuted;
            Sounds.Add(source);
            if (!loop) Scheduler.Invoke(() => { RemoveSound(source); }, runtime.Clip.length);

            return source;
        }

        return null;
    }

    public void RemoveSound(AudioSource source)
    {
        source.Despawn();
        Sounds.Remove(source);
    }

    private bool IsSoundMuted, IsMusicMuted;

    public void MuteSound(bool mute = true)
    {
        IsSoundMuted = mute;
        foreach (var sfx in Sounds) sfx.mute = mute;
    }

    public void MuteMusic(bool mute = true)
    {
        IsMusicMuted = mute;
        if (Music != null) Music.mute = mute;
    }
}

[Serializable]
public class AudioRuntime
{
    public AudioClip Clip;
    public float Delay;
}
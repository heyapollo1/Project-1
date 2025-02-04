using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.Audio;
using UnityEngine.Events;

[System.Serializable]
public class SoundRequest
{
    public string ClipPrefix;
    public Vector3 Position; //location of sound, for soundFX NOT UI/MUSIC    
    public float Volume = 1.0f;   // Default volume
    public float MinPitch = 1.0f;
    public float MaxPitch = 1.0f;
    public int Priority = 0;      // Lower is less important
    public string Group = "Default"; // Group for limiting total amt that can play at same time
}

public class AudioManager : BaseManager
{
    public static AudioManager Instance;

    [Header("Audio Mixer")]
    public AudioMixer audioMixer;

    [Header("Audio Sources")]
    public AudioSource musicSource; // Background music
    public AudioSource uiSource;    // UI sounds(global)
    private List<AudioSource> sfxSources = new List<AudioSource>(); // Pool for SFX
    private const int maxSfxSources = 10;

    [Header("Sound Settings")]
    public float minSoundDistance = 4.0f; // for soundFX with positioning
    public float maxSoundDistance = 15.0f;
    public float fadeDuration = 1.5f;
    public string currentMusic;

    private Dictionary<string, List<AudioClip>> audioCategories = new Dictionary<string, List<AudioClip>>();
    private Dictionary<string, int> groupLimits = new Dictionary<string, int> { { "Default", 10 }, { "Gunfire", 10 }, { "Footsteps", 5 } };
    private Dictionary<string, int> activeGroupCounts = new Dictionary<string, int>();


    protected override void OnInitialize()
    {
        if (audioMixer == null)
        {
            audioMixer = Resources.Load<AudioMixer>("Prefabs/MainAudioMixer");
        }

        InitializeAudio();
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void InitializeAudio()
    {
        PreloadAudioCategories();

        InitializeAudioSource(ref musicSource, "Music");
        InitializeAudioSource(ref uiSource, "UI");
        InitializeAndPrewarmSFXPool(maxSfxSources);

        audioMixer.SetFloat("MusicVolume", -10f);
        audioMixer.SetFloat("SFXVolume", 0f);
        audioMixer.SetFloat("UIVolume", 0f);

        EventManager.Instance.StartListening("PauseAudio", PauseAllAudio);
        EventManager.Instance.StartListening("ResumeAudio", ResumeAllAudio);
    }

    private void OnDestroy()
    {
        EventManager.Instance.StopListening("PauseAudio", PauseAllAudio);
        EventManager.Instance.StopListening("ResumeAudio", ResumeAllAudio);
    }

    private void PreloadAudioCategories()
    {
        AudioCategory[] categories = Resources.LoadAll<AudioCategory>("AudioCategories");
        foreach (var category in categories)
        {
            if (!audioCategories.ContainsKey(category.categoryPrefix))
            {
                audioCategories[category.categoryPrefix] = category.clips;
            }
        }
    }

    private void InitializeAudioSource(ref AudioSource source, string mixerGroup)
    {
        source = gameObject.AddComponent<AudioSource>();
        source.outputAudioMixerGroup = audioMixer.FindMatchingGroups(mixerGroup)[0];
        source.spatialBlend = 0f;
        source.playOnAwake = false;
        source.loop = false;
    }

    private void InitializeAndPrewarmSFXPool(int poolSize)
    {
        for (int i = 0; i < poolSize; i++)
        {
            AudioSource sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("SFX")[0];

            // Use 2D blend for fake 2D effects
            sfxSource.spatialBlend = 0f; // Full2D sound
            sfxSource.playOnAwake = false;

            sfxSources.Add(sfxSource);
        }
    }

    //Sound FX with positioning, call
    public static void TriggerSound(string clipPrefix, Vector3? position = null, float volume = 1.0f, float minPitch = 1.05f, float maxPitch = 0.95f, int priority = 0, string group = "Default")
    {
        SoundRequest request = new SoundRequest
        {
            ClipPrefix = clipPrefix,
            Position = position ?? Vector3.zero,
            Volume = volume,
            MinPitch = minPitch,
            MaxPitch = maxPitch,
            Priority = priority,
            Group = group
        };

        Instance.SourceAudioClip(request);
    }

    private void SourceAudioClip(SoundRequest request)
    {
        if (!audioCategories.TryGetValue(request.ClipPrefix, out var clips) || clips.Count == 0)
        {
            Debug.LogWarning($"PlaySound: No clips found for prefix '{request.ClipPrefix}'");
            return;
        }

        if (activeGroupCounts.TryGetValue(request.Group, out int count) && count >= groupLimits.GetValueOrDefault(request.Group, 10))//check if max amt of sound is being played
        {
            Debug.LogWarning($"PlaySound: Group '{request.Group}' limit reached.");
            return;
        }

        AudioClip clip = clips[Random.Range(0, clips.Count)];
        if (clip == null) return;

        PlaySFX(clip, request);
    }

    private void PlaySFX(AudioClip clip, SoundRequest request)//SFX, with positioning
    {
        AudioSource source = GetAvailableSFXSource(request.Priority); //filter priority, aka explosions higher priority
        if (source == null) return;

        Vector3 listenerPosition = Camera.main.transform.position;
        float distance = Vector3.Distance(request.Position, listenerPosition);
        float adjustedVolume = Mathf.Clamp01(1.0f - (distance - minSoundDistance) / (maxSoundDistance - minSoundDistance));
        adjustedVolume *= request.Volume;

        source.clip = clip;
        source.volume = adjustedVolume;
        source.pitch = Random.Range(request.MinPitch, request.MaxPitch);
        source.PlayOneShot(clip);

        activeGroupCounts[request.Group] = activeGroupCounts.GetValueOrDefault(request.Group, 0) + 1;
        StartCoroutine(ReleaseGroupCount(request.Group, clip.length));
    }

    private AudioSource GetAvailableSFXSource(int priority)
    {
        AudioSource leastImportant = null;
        int lowestPriority = int.MaxValue;

        foreach (var source in sfxSources)
        {
            if (!source.isPlaying) return source;
            if (source.priority < lowestPriority)
            {
                leastImportant = source;
                lowestPriority = source.priority;
            }
        }

        if (priority > lowestPriority && leastImportant != null)
        {
            leastImportant.Stop();
            return leastImportant;
        }

        Debug.LogWarning("All SFX sources are busy!");
        return null;
    }

    private IEnumerator ReleaseGroupCount(string group, float delay)
    {
        yield return new WaitForSeconds(delay);
        activeGroupCounts[group] = Mathf.Max(0, activeGroupCounts[group] - 1);
    }

    //PLay BG Music
    public void PlayBackgroundMusic(string categoryPrefix, float volume = 1.0f)
    {
        if (!audioCategories.TryGetValue(categoryPrefix, out var clips) || clips.Count == 0)
        {
            Debug.LogWarning($"No music found: {categoryPrefix}");
            return;
        }

        AudioClip clip = clips[Random.Range(0, clips.Count)];//random variations in group?

        if (musicSource.isPlaying)
        {
            FadeOutSound(musicSource, fadeDuration, () =>
            {
                musicSource.clip = clip;
                musicSource.volume = volume;
                musicSource.loop = true;
                musicSource.Play();
            });
        }
        else
        {
            // Play immediately if no music is currently playing
            musicSource.clip = clip;
            musicSource.volume = volume;
            musicSource.loop = true;
            musicSource.Play();
        }

        currentMusic = categoryPrefix;
    }

    //UI Sounds, global
    public void PlayUISound(string clipPrefix, float volume = 1.0f)
    {
        if (!audioCategories.TryGetValue(clipPrefix, out var clips) || clips.Count == 0)
        {
            Debug.LogWarning($"No UI clips found for category: {clipPrefix}");
            return;
        }

        AudioClip clip = clips[Random.Range(0, clips.Count)];
        if (clip == null) return;

        uiSource.PlayOneShot(clip, volume);
    }

    public void FadeOutSound(AudioSource source, float duration, UnityAction onComplete = null)
    {
        StartCoroutine(FadeOutCoroutine(source, duration, onComplete));
    }

    private IEnumerator FadeOutCoroutine(AudioSource source, float duration, UnityAction onComplete = null)
    {
        float startVolume = source.volume;

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            source.volume = Mathf.Lerp(startVolume, 0, t / duration);
            yield return null;
        }

        source.volume = 0;
        source.Stop();
        onComplete?.Invoke();
    }

    public void ResetAudio()
    {
        foreach (var source in sfxSources)
        {
            source.Stop();
            source.clip = null; // Ensure the clip is cleared
        }
        uiSource.Stop();
    }

    public void SetVolume(string type, float volume)
    {
        if (volume <= 0.01f)
        {
            audioMixer.SetFloat(type, -80f);  // Mute
        }
        else
        {
            float dbVolume = Mathf.Log10(volume) * 20;
            audioMixer.SetFloat(type, dbVolume);
        }
    }

    public void ToggleMute(bool isMuted)
    {
        audioMixer.SetFloat("MainVolume", isMuted ? -80f : Mathf.Log10(1f) * 20);
    }

    public void TurnOffMusic()
    {
        if (musicSource.isPlaying)
        {
            musicSource.Stop();
        }
    }

    public void PauseMusic()
    {
        if (musicSource.isPlaying)
        {
            musicSource.Pause();
        }
    }

    public void PauseAllAudio()
    {
        if (musicSource.isPlaying)
        {
            musicSource.Pause();
        }

        foreach (var sfxSource in sfxSources)
        {
            if (sfxSource.isPlaying)
            {
                sfxSource.Pause();
            }
        }
    }

    public void ResumeAllAudio()
    {
        if (musicSource.time > 0)
        {
            musicSource.UnPause();
        }

        foreach (var sfxSource in sfxSources)
        {
            sfxSource.UnPause();
        }
    }
}
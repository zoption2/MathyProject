using System.Collections;
using UnityEngine.Audio;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

/// <summary>
/// Insanely basic audio system which supports 3D sound.
/// Ensure you change the 'Sounds' audio source to use 3D spatial blend if you intend to use 3D sounds.
/// </summary>
public class AudioSystem : StaticInstance<AudioSystem>
{
    #region Fields

    [Header("Components:")]

    [SerializeField] private AudioSource soundsSource;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioMixerGroup Mixer;

    [Header("Config:")]

    [SerializeField] private float audioFadeDuration = 0.5f;

    #endregion

    protected override void Awake()
    {
        base.Awake();

        GameSettingsManager.OnSoundsEnabled.AddListener(EnableSounds);
        GameSettingsManager.OnMusicEnabled.AddListener(EnableMusic);
    }

    #region Music Methods

    public void PlayMusic(AudioClip clip)
    {
        musicSource.clip = clip;
        musicSource.Play();
    }

    public void FadeMusic(float volume, float duration, bool stopOnComplete)
    {
        musicSource.DOFade(volume, duration).OnComplete(() => OnFadeComplete(stopOnComplete));
    }

    protected virtual void OnFadeComplete(bool stopOnComplete)
    {
        if (stopOnComplete)
        {
            musicSource.Stop();
        }
        musicSource.volume = 1;
    }

    private void EnableMusic(bool isEnable)
    {
        float volume = isEnable ? 0 : -80;
        Mixer.audioMixer.DOSetFloat("MusicVolume", volume, audioFadeDuration);
    }

    #endregion

    #region Sound Methods

    public void PlaySound(AudioClip clip, Vector3 pos, float vol = 1)
    {
        soundsSource.transform.position = pos;
        PlaySound(clip, vol);
    }

    public void PlaySound(AudioClip clip, float vol = 1)
    {
        soundsSource.PlayOneShot(clip, vol);
    }

    public void PlaySound(AudioClip clip, float vol = 1, float speed = 1)
    {
        StartCoroutine(PlaySoundSpeed(clip, vol, speed));
    }

    private IEnumerator PlaySoundSpeed(AudioClip clip, float vol = 1, float speed = 1)
    {
        soundsSource.pitch = speed;
        soundsSource.PlayOneShot(clip, vol);
        yield return new WaitWhile(() => soundsSource.isPlaying);
        soundsSource.pitch = 1;
    }

    public void StopSounds()
    {
        soundsSource.Stop();
    }

    private void EnableSounds(bool isEnable)
    {
        float volume = isEnable ? 0 : -80;
        Mixer.audioMixer.DOSetFloat("UISoundsVolume", volume, audioFadeDuration);
    }

#endregion
}
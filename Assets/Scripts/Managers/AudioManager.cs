using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class AudioManager : StaticInstance<AudioManager>
{
    #region FIELDS

    [SerializeField] private AudioClip bgMusic;
    [SerializeField] private AudioClip gameplayBGMusic;
    [SerializeField] private AudioClip scrollSound;
    [SerializeField] private AudioClip panelSound;    
    [SerializeField] private List<AudioClip> buttonClicks;
    [SerializeField] private List<AudioClip> correctVariants;
    [SerializeField] private List<AudioClip> wrongVariants;

    [SerializeField] private List<AudioClip> digitsEn;
    [SerializeField] private List<AudioClip> digitsRu;
    [SerializeField] private List<AudioClip> digitsUa;
    private List<AudioClip> digitsSounds;

    public AudioClip gradeSliderSound;

    private int nextClickIndex;
    private int nextCorrectIndex;
    private int nextWrongIndex;

    #endregion

    protected override void Awake()
    {
        base.Awake();
        Initialize();
    }

    private void Initialize()
    {
        LocalizationManager.OnLanguageChanged.AddListener(Localize);
    }

    private void Localize()
    {
        string localeCode = LocalizationSettings.SelectedLocale.Identifier.Code;
        switch (localeCode)
        {
            case "en":
                digitsSounds = digitsEn;
                break;
            case "ru":
                digitsSounds = digitsRu;
                break;
            case "uk":
                digitsSounds = digitsUa;
                break;
            default:
                goto case "en";
        }
    }

    public void PlayMusic()
    {
        AudioSystem.Instance.PlayMusic(bgMusic);
    }

    public void PlayTaskMusic()
    {
        AudioSystem.Instance.PlayMusic(gameplayBGMusic);
    }

    public void PlayDigitSound(int digit)
    {
        AudioSystem.Instance.PlaySound(digitsSounds[digit], 1f);
    }

    public void ButtonClickSound()
    {
        var clickSound = buttonClicks[nextClickIndex];
        AudioSystem.Instance.PlaySound(clickSound, 0.5f);
        nextClickIndex = (nextClickIndex + 1) % buttonClicks.Count;
    }

    public void CorrectVariantSound()
    {
        var correctSound = correctVariants[nextCorrectIndex];
        AudioSystem.Instance.PlaySound(correctSound, 0.5f, Random.Range(0.9f, 1f));
        nextCorrectIndex = (nextCorrectIndex + 1) % correctVariants.Count;
    }

    public void WrongVariantSound()
    {
        var wrongSound = wrongVariants[nextWrongIndex];
        AudioSystem.Instance.PlaySound(wrongSound, 0.5f, Random.Range(1.4f, 1.5f));
        nextWrongIndex = (nextWrongIndex + 1) % wrongVariants.Count;
    }

    public void GradeSliderSound()
    {
        AudioSystem.Instance.StopSounds();
        AudioSystem.Instance.PlaySound(gradeSliderSound, 0.5f, 1f);
    }

    public void ScrollPanelSound()
    {
        AudioSystem.Instance.PlaySound(scrollSound, 0.5f, Random.Range(0.75f, 1f));
    }
    public void PanelSound()
    {
        AudioSystem.Instance.PlaySound(panelSound, 0.5f, Random.Range(0.9f, 1.1f));
    }

    public float GetAudioLength(AudioClip audioClip)
    {
        float length = audioClip.length;
        return length;
    }
}

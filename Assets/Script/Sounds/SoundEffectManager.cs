using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SoundsEffectLibrary), typeof(AudioSource))]
public class SoundEffectManager : MonoBehaviour
{
    public static SoundEffectManager Instance;

    private AudioSource audioSource;
    private AudioSource randomPitchaudioSource;
    private AudioSource voiceAudioSource;
    private SoundsEffectLibrary soundEffectLibrary;

    [SerializeField] private Slider sfxSlider;

    private const string SFX_VOLUME_KEY = "sfxVolume";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            AudioSource[] audioSources = GetComponents<AudioSource>();

            if (audioSources.Length < 3)
            {
                Debug.LogError(
                    "SoundEffectManager necesita 3 AudioSource en el mismo GameObject. " +
                    "Actualmente tiene: " + audioSources.Length
                );

                return;
            }

            audioSource = audioSources[0];
            randomPitchaudioSource = audioSources[1];
            voiceAudioSource = audioSources[2];

            soundEffectLibrary = GetComponent<SoundsEffectLibrary>();

            if (soundEffectLibrary == null)
            {
                Debug.LogError("No se encontró SoundsEffectLibrary en SoundEffectManager.");
                return;
            }

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Play(string soundName, bool randomPitch = false)
    {
        AudioClip clip = soundEffectLibrary.GetRandomClip(soundName);

        if (clip != null)
        {
            if (randomPitch)
            {
                randomPitchaudioSource.pitch = Random.Range(0.5f, 1.3f);
                randomPitchaudioSource.PlayOneShot(clip);
            }
            else
            {
                audioSource.PlayOneShot(clip);
            }
        }
    }

    public void PlayVoice(AudioClip clip, float pitch = 1f)
    {
        voiceAudioSource.pitch = pitch;
        voiceAudioSource.PlayOneShot(clip);
    }

    private void Start()
    {
        float savedVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 0.5f);

        // Ponemos el valor guardado en el Slider
        sfxSlider.value = savedVolume;

        // Aplicamos el volumen a los AudioSource
        SetVolumen(savedVolume);

        // Escuchamos cambios del Slider
        sfxSlider.onValueChanged.AddListener(OnValueChanged);
    }

    public void SetVolumen(float volume)
    {
        // Guardar volumen
        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, volume);
        PlayerPrefs.Save();

        // Aplicar volumen
        audioSource.volume = volume;
        randomPitchaudioSource.volume = volume;
        voiceAudioSource.volume = volume;
    }

    public void OnValueChanged(float value)
    {
        SetVolumen(value);
    }
}

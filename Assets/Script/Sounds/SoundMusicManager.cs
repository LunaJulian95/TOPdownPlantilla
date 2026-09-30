using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SoundsEffectLibrary))]
public class SoundMusicManager : MonoBehaviour
{
    public static SoundMusicManager Instance;

    private AudioSource audioSource;
    private AudioSource audioSource2;

    private SoundsEffectLibrary musicLibrary;

    [Header("Configuracion del audio")]
    [SerializeField] private float crossFadeDuration = 1.5f;

    [Header("Volumen")]
    [SerializeField] private Slider musicSlider;

    private bool isPlayingSource1 = true;

    // Clave utilizada para guardar el volumen
    private const string MusicVolumeKey = "MusicVolume";


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            AudioSource[] sources = GetComponents<AudioSource>();

            if (sources.Length < 2)
            {
                Debug.LogError(
                    "SoundMusicManager necesita 2 AudioSource."
                );

                return;
            }

            audioSource = sources[0];
            audioSource2 = sources[1];

            audioSource.loop = true;
            audioSource2.loop = true;

            musicLibrary = GetComponent<SoundsEffectLibrary>();

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    private void Start()
    {
        // Cargar volumen guardado
        float savedVolume =
            PlayerPrefs.GetFloat(MusicVolumeKey, 0.5f);

        // Actualizar Slider
        if (musicSlider != null)
        {
            musicSlider.value = savedVolume;

            musicSlider.onValueChanged.AddListener(
                OnValueChanged
            );
        }

        // Aplicar volumen a los AudioSource
        SetVolumen(savedVolume);
    }


    public void PlayMusicWithCrossFade(string newMusicName)
    {
        AudioClip newClip =
            musicLibrary.GetRandomClip(newMusicName);


        // Si no encontramos la música
        if (newClip == null)
        {
            Debug.LogWarning(
                "No se encontró música con el nombre: "
                + newMusicName
            );

            return;
        }


        // Saber qué AudioSource está reproduciendo
        AudioSource activeAudioSource =
            isPlayingSource1
                ? audioSource
                : audioSource2;


        // Evitar reproducir nuevamente la misma canción
        if (activeAudioSource.clip == newClip)
        {
            return;
        }


        if (isPlayingSource1)
        {
            // Preparar segundo AudioSource
            audioSource2.clip = newClip;

            audioSource2.volume = 0f;

            audioSource2.Play();


            StartCoroutine(
                CrossfadeRoutine(
                    audioSource,
                    audioSource2,
                    crossFadeDuration
                )
            );
        }
        else
        {
            // Preparar primer AudioSource
            audioSource.clip = newClip;

            audioSource.volume = 0f;

            audioSource.Play();


            StartCoroutine(
                CrossfadeRoutine(
                    audioSource2,
                    audioSource,
                    crossFadeDuration
                )
            );
        }


        // Cambiar AudioSource activo
        isPlayingSource1 = !isPlayingSource1;
    }


    private IEnumerator CrossfadeRoutine(
        AudioSource fadeOutSource,
        AudioSource fadeInSource,
        float fadeDuration)
    {
        float currentTime = 0f;


        // Volumen elegido por el jugador
        float maxVolume = musicSlider != null
            ? musicSlider.value
            : PlayerPrefs.GetFloat(
                MusicVolumeKey,
                0.5f
            );


        fadeInSource.volume = 0f;


        while (currentTime < fadeDuration)
        {
            currentTime += Time.deltaTime;


            float t =
                currentTime / fadeDuration;


            fadeOutSource.volume =
                Mathf.Lerp(
                    maxVolume,
                    0f,
                    t
                );


            fadeInSource.volume =
                Mathf.Lerp(
                    0f,
                    maxVolume,
                    t
                );


            yield return null;
        }


        fadeOutSource.volume = 0f;

        fadeOutSource.Stop();


        fadeInSource.volume = maxVolume;
    }


    // =====================================================
    // VOLUMEN
    // =====================================================

    public void SetVolumen(float volume)
    {
        // Limitar entre 0 y 1
        volume = Mathf.Clamp01(volume);


        // Cambiar volumen de ambos AudioSource
        if (audioSource != null)
        {
            audioSource.volume = volume;
        }


        if (audioSource2 != null)
        {
            audioSource2.volume = volume;
        }


        // Guardar configuración
        PlayerPrefs.SetFloat(
            MusicVolumeKey,
            volume
        );

        PlayerPrefs.Save();
    }


    public void OnValueChanged(float volume)
    {
        SetVolumen(volume);
    }
}
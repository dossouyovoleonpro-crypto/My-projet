using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    public AudioClip musicClip; // Assigné dans l'inspecteur
    private AudioSource audioSource;

    void Awake()
    {
        // Vérifie s'il existe déjà un BackgroundMusic (singleton)
        if (FindObjectsOfType<BackgroundMusic>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject); // Persiste entre les scènes
    }

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = musicClip;
        audioSource.loop = true;       // Boucle infinie
        audioSource.playOnAwake = true; // Démarre automatiquement
        audioSource.volume = 0.5f;     // Volume modifiable à ton goût
        audioSource.Play();
    }
}

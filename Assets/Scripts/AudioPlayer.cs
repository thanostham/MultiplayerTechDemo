using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    [SerializeField] private float volume = 0.8f;
    public void PlaySound(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.Play();
        Destroy(gameObject, clip.length + 0.1f);
    }
}

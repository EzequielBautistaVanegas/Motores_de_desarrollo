using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class AudioZone : MonoBehaviour
{
    [Header("Sonido de la zona")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip clip;
    [SerializeField][Range(0f, 1f)] private float volume = 1f;

    [Header("Música principal (se activa al terminar el clip)")]
    [SerializeField] private AudioSource musicSource;   
    [SerializeField] private float musicDelay = 0f;     
    [Header("Comportamiento")]
    [SerializeField] private bool playOnlyOnce = true;
    [SerializeField] private string playerTag = "Player";

    private bool hasPlayed;

    void Reset()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;
        if (playOnlyOnce && hasPlayed) return;
        if (audioSource == null || clip == null) return;

        hasPlayed = true;
        StartCoroutine(PlayZoneThenMusic());
    }

    IEnumerator PlayZoneThenMusic()
    {
        audioSource.PlayOneShot(clip, volume);

        
        yield return new WaitForSeconds(clip.length + musicDelay);

        if (musicSource != null && !musicSource.isPlaying)
            musicSource.loop = true;
            musicSource.Play();
    }
}
using UnityEngine;
using KittyTerror.Events;
using static Unity.VisualScripting.Member;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    public AudioClipRegistry registry;

    private void OnEnable()
    {
        EventBus<AudioPlayEvent>.OnRaised += PlaySound;
    }

    private void OnDisable()
    {
        EventBus<AudioPlayEvent>.OnRaised -= PlaySound;
    }

    private void PlaySound(AudioPlayEvent e)
    {
        Debug.Log($"[AudioManager] Reproducir: {e.ClipId}");

        AudioClip clip = registry.GetClip(e.ClipId);
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
            
    }

}

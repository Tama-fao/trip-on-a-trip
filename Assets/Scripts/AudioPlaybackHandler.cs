using UnityEngine;

public class AudioPlaybackHandler : MonoBehaviour {
    private AudioSource _audioSource;

    private void Awake() {
        // Add an AudioSource component to the GameObject if it doesn't exist
        _audioSource = gameObject.GetComponent<AudioSource>();
        if (_audioSource == null) {
            _audioSource = gameObject.AddComponent<AudioSource>();
        }
    }


     public bool IsPlaying()
    {
        return _audioSource.isPlaying;
    }

    public void PlayAudioClip(AudioClip audioClip) {
        if (audioClip == null) {
            Debug.LogError("AudioClip is null. Cannot play audio.");
            return;
        }

        _audioSource.clip = audioClip;
        _audioSource.Play();
    }
}
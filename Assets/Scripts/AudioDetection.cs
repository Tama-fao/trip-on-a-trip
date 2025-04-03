using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioDetection : MonoBehaviour
{
    [Tooltip("The AudioSource component to monitor")]
    public AudioSource audioSource;

    [Tooltip("The name of the animator parameter to set")]
    public string animatorParameterName = "isTalking";

    private Animator animator;
    private bool wasPlaying = false;

 void Start()
    {
        // Get references if not set in inspector
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        animator = GetComponent<Animator>();

        // Log warnings if components are missing
        if (audioSource == null)
        {
            Debug.LogWarning("No AudioSource component found!", this);
        }

        if (animator == null)
        {
            Debug.LogWarning("No Animator component found! isTalking parameter won't be set.", this);
        }
    }

    void Update()
    {
        // Check if we have the required components
        if (audioSource == null) return;

        // Check if playback state changed
        bool isPlaying = audioSource.isPlaying;
        
        if (isPlaying != wasPlaying)
        {
            wasPlaying = isPlaying;
            
            // Update animator parameter if available
            if (animator != null && !string.IsNullOrEmpty(animatorParameterName))
            {
                animator.SetBool(animatorParameterName, isPlaying);
            }
            
            // Optional: Log state change
            // Debug.Log(isPlaying ? "Started playing audio" : "Stopped playing audio");
        }
    }

    // Optional: Public method to check current state
    public bool IsAudioPlaying()
    {
        return audioSource != null && audioSource.isPlaying;
    }


}
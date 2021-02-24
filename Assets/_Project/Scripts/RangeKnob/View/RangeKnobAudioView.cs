using UnityEngine;

public class RangeKnobAudioView : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource = null;
    [SerializeField] private AudioClip _clickAudio = null;
    
    public void PlayClickSound()
    {
        if (_audioSource == null || _clickAudio == null)
        {
            Debug.LogError("Audio source or clip is null!");
            return;
        }
        _audioSource.PlayOneShot(_clickAudio);
    }
}
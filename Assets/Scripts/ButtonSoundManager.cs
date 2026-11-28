using UnityEngine;

public class ButtonSoundManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip buttonSound;

    public void PlayButtonSound()
    {
        audioSource.PlayOneShot(buttonSound);
    }
}
using UnityEngine;

public class SoundManager : MonoBehaviour
{

    public AudioSource soundObject;
    public static SoundManager instance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake() {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void playSound(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        AudioSource audioSource = Instantiate(soundObject, spawnTransform.position, Quaternion.identity);

        Debug.Log($"playing {audioClip.name}");

        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.Play();
        Destroy(audioSource.gameObject, audioSource.clip.length);

    }
}

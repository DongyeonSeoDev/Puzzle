using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound : MonoBehaviour
{
    private AudioSource audioSource = null;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void SoundPlay(AudioClip audio, float soundTime)
    {
        audioSource.clip = audio;
        audioSource.Play();

        Invoke("EndSound", soundTime);
    }

    private void EndSound()
    {
        audioSource.Stop();

        gameObject.SetActive(false);
    }
}

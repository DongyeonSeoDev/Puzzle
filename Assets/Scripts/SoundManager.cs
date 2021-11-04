using System.Collections.Generic;
using UnityEngine;

public enum eSoundType
{
    GAMEOVER,
    GAMECLEAR,
    TOUCHSOUND
}

public class SoundManager : MonoBehaviour
{
    private static SoundManager instance = null;

    public static SoundManager Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.Log("SoundManager instance가 null 입니다.");
                return null;
            }

            return instance;
        }
    }

    [SerializeField] private AudioClip[] clips = null;
    [SerializeField] private float[] soundTime = null;
    [SerializeField] private Sound sound = null;

    private List<Sound> sounds = new List<Sound>();

    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("SoundManager instance가 중복입니다.");
            Destroy(this);
        }

        instance = this;
    }
    
    private void CreateSound()
    {
        Sound soundObject = Instantiate(sound, transform);
        soundObject.gameObject.SetActive(false);
        sounds.Add(soundObject);
    }

    private Sound GetSound()
    {
        Sound sound = null;

        for (int i = 0; i < sounds.Count; i++)
        {
            if (!sounds[i].gameObject.activeSelf)
            {
                sound = sounds[i];
                break;
            }
        }

        if (sound is null)
        {
            CreateSound();
            sound = sounds[sounds.Count - 1];
        }

        return sound;
    }

    public static void SoundPlay(eSoundType soundType)
    {
        Sound audio = Instance.GetSound();

        audio.gameObject.SetActive(true);
        audio.SoundPlay(Instance.clips[(int)soundType], Instance.soundTime[(int)soundType]);
    }
}

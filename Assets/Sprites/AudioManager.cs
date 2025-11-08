using System;
using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    /// <summary>
    /// BGM and Sound Effect's List
    /// </summary>
    public Sound[] musicSounds, sfxSounds;

    /// <summary>
    /// The location of play thne Audio
    /// </summary>
    public AudioSource musicSource, sfxSource;

    public bool IsPlaying() {  return musicSource.isPlaying; }

    /// <summary>
    /// Keep it alive in every Scenes
    /// </summary>
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
    }

    /// <summary>
    /// ChangeCrip("MainBGM", "BGM/TitleBGM");
    /// </summary>
    /// <param name="name"></param>
    /// <param name="path"></param>
    public void ChangeCrip(string name, string path)
    {
        // 在 musicSounds 数组里查名字
        Sound s = Array.Find(musicSounds, x => x.name == name);

        if (s == null)
        {
            Debug.LogWarning($"[ChangeClip] Sound '{name}' not found in musicSounds.");
            return;
        }

        // 从 Resources 里加载
        AudioClip clip = Resources.Load<AudioClip>(path);

        if (clip == null)
        {
            Debug.LogWarning($"[ChangeClip] AudioClip not found at path: Resources/{path}");
            return;
        }

        // 替换音频
        s.clip = clip;
    }

    public float CripLenght(string name)
    {
        Sound s = Array.Find(musicSounds, x => x.name == name);

        if (s == null)
        {
            Debug.LogWarning("Sound not found: " + name);
            return 0f;
        }

        return s.clip.length; // ✅ 返回音频时长（单位：秒）
    }


    /// <summary>
    /// Adjusts the volume of the specified BGM.
    /// </summary>
    /// <param name ="name">The name of the BGM </param>
    /// <param name="targetVolume">The target volume (range 0.0 ~ 1.0)</param>
    public void PlayMusic(string name, float targetVolume)
    {
        Sound s = Array.Find(musicSounds, x => x.name == name);
        if (s == null)
        {
            Debug.Log("Sound not found");
        }
        else
        {
            musicSource.clip = s.clip;
            musicSource.volume = targetVolume;
            musicSource.Play();
        }

    }

    public void PauseMusic(string name)
    {
        Sound s = Array.Find(musicSounds, x => x.name == name);
        if (s == null)
        {
            Debug.Log("Sound not found");
        }
        else
        {
            musicSource.clip = s.clip;
            musicSource.Pause();
        }
    }

    public void UnPauseMusic(string name)
    {
        Sound s = Array.Find(musicSounds, x => x.name == name);
        if (s == null)
        {
            Debug.Log("Sound not found");
        }
        else
        {
            musicSource.clip = s.clip;
            musicSource.UnPause();
        }
    }


    /// <summary>
    /// End the BGM
    /// </summary>
    /// <param name="name">The name of the BGM</param>
    public void EndMusic(string name)
    {
        Sound s = Array.Find(musicSounds, x => x.name == name);
        if (s == null)
        {
            Debug.Log("Sound not found");
        }
        else
        {
            musicSource.clip = s.clip;
            musicSource.Stop();
        }
    }

    /// <summary>
    /// Adjusts the volume of the specified Audio Effect.
    /// </summary>
    /// <param name="name">The name of the Audio Effect</param>
    /// <param name="targetVolume">The target volume (range 0.0 ~ 1.0)</param>
    public void PlaySFX(string name, float targetVolume)
    {
        Sound s = Array.Find(sfxSounds, x => x.name == name);

        if (s == null)
        {
            Debug.Log("Sound not found");
        }

        else
        {
            sfxSource.volume = targetVolume;
            sfxSource.PlayOneShot(s.clip);
        }
    }

    //public void SfxLoop(string name)
    //{
    //    Sound s = Array.Find(sfxSounds, x => x.name == name);

    //    if (s == null)
    //    {
    //        Debug.Log("Sound not found");
    //        return;
    //    }

    //    sfxSource.clip = s.clip;
    //    sfxSource.loop = true;
    //    sfxSource.Play();
    //}
    //public void SfxStop()
    //{
    //    sfxSource.Stop();
    //    sfxSource.loop = false;
    //}

}

using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

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
    public void ChangeCrip(string name, string cripName)
    {
        // 在 musicSounds 数组里查名字
        Sound s = Array.Find(musicSounds, x => x.name == name);

        if (s == null)
        {
            Debug.LogWarning($"[ChangeClip] Sound '{name}' not found in musicSounds.");
            return;
        }

        if (!cripName.EndsWith(".mp3"))cripName += ".mp3";
        string path = Path.Combine(Application.streamingAssetsPath, "StageMusic", cripName);

        StartCoroutine(LoadAudioAndSet(s, path));
    }

    IEnumerator LoadAudioAndSet(Sound s, string path)
    {
        UnityWebRequest req = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.MPEG);
        req.disposeDownloadHandlerOnDispose = false;

        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"[ChangeClip] Failed to load clip: {path}\n{req.error}");
            yield break;
        }

        AudioClip clip = DownloadHandlerAudioClip.GetContent(req);

        clip.hideFlags = HideFlags.DontUnloadUnusedAsset;

        s.clip = clip;  // ✅ 正确赋值
        Debug.Log($"[ChangeClip] Loaded and changed BGM: {clip.name}");
    }

    public float CripLenght(string name)
    {
        Sound s = Array.Find(musicSounds, x => x.name == name);
        if (s == null || s.clip == null)
            return 0f; // ✅ 防止访问销毁后 clip

        return s.clip.length;
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

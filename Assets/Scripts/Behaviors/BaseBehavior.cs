using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public abstract class BaseBehavior : MonoBehaviour
{
    public class VisibilityEvent : UnityEvent<bool> { }

    protected Loggger log;
    protected AudioSource audioSource;

    #region properties

    #endregion

    protected virtual void Start()
    {
        if (log == null)
            log = LoggerProvider.get(this);
        Init();
    }

    protected virtual void Init(){}
    
    public virtual void PlaySfx()
    {
        this.audioSource.Play();
    }

    public void PlaySfx(string fileName, bool loop = false)
    {
        /*
        var audio = this.GetResource<AudioClip>($"Audio/SFX/{fileName}");
        if (audio != null)
        {
            this.audioSource.loop = loop;
            if (loop)
            {
                if (!this.audioSource.isPlaying || !this.audioSource.clip.Equals(audio))
                {
                    this.audioSource.clip = audio;
                    this.audioSource.Play();
                }
            }
            else
            {
                this.audioSource.PlayOneShot(audio);
            }
        }
        */
    }

    public void StopAudio()
    {
        if (this.audioSource == null || !this.audioSource.isPlaying)
        {
            return;
        }

        this.audioSource.Stop();
    }
/*
    protected T GetResource<T>(string resourcePath) where T : UnityEngine.Object
    {
        return this.Context != null ? this.File.GetResource<T>(resourcePath) : Resources.Load<T>(resourcePath);
    }

    protected List<T> GetResources<T>(string resourcePath) where T : UnityEngine.Object
    {
        return this.Context != null ? this.File.GetResources<T>(resourcePath) : Resources.LoadAll<T>(resourcePath).ToList();
    }
    */
}

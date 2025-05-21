using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public bool muteAllAudio;

    public Queue<AudioSource> audioSourcesCollection = new Queue<AudioSource>();

    [System.Serializable]
    public class AudioByType
    {
        public Audioenum audioEnum;
        public List<AudioSource> audioActive;
    }
    [SerializeField] List<AudioByType> audiosActive = new List<AudioByType>();
    int poolSize = 4;

    public AudioCollection audioCollection;


    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void StopAudio(Audioenum audioToPlay)
    {
        for (int i = 0; i < audiosActive.Count; i++)
        {
            if (audiosActive[i].audioEnum == audioToPlay)
            {
                AudioByType audioTypeToStop = audiosActive[i];

                foreach (var item in audioTypeToStop.audioActive)
                {
                    item.Stop();
                    audioSourcesCollection.Enqueue(item);
                }

                audiosActive.RemoveAt(i);
                i--;
            }
        }
    }


    public void PlayAudio(Audioenum _audioEnum, bool loop = false)
    {
        if (muteAllAudio || audioCollection == null) return;
        AudioData collection = audioCollection.audioData.Find(x => x.audioName == _audioEnum);
        if (collection.Ismute)
            return;
        AudioSource audioSource = GetAudioSource();
        audioSource.Stop();
        audioSource.clip = collection.clip;
        audioSource.loop = loop;
        audioSource.volume = collection.volume;
        audioSource.pitch = collection.pitch;
        audioSource.Play();

        AudioByType audioType = audiosActive.Find(x => x.audioEnum == _audioEnum);

        if (audioType != null)
        {
            audioType.audioActive.Add(audioSource);
        }
        else
        {
            audiosActive.Add(new AudioByType
            {
                audioEnum = _audioEnum,
                audioActive = new List<AudioSource> { audioSource }
            });
        }
        if (!loop)
        {
            StartCoroutine(EnqueueAudioSource(audioSource));
        }

    }
    IEnumerator EnqueueAudioSource(AudioSource source)
    {
        if (source.clip == null)
        {
            foreach (var audiotype in audiosActive)
            {
                if (audiotype.audioActive.Contains(source))
                {
                    audiotype.audioActive.Remove(source);
                }
            }
            yield break;
        }

        yield return new WaitForSeconds(source.clip.length + 0.5f);

        if (source.clip != null)
        {
            source.Stop();
            if (!audioSourcesCollection.Contains(source))
                audioSourcesCollection.Enqueue(source);
            source.clip = null;

            foreach (var audiotype in audiosActive)
            {
                if (audiotype.audioActive.Contains(source))
                {
                    audiotype.audioActive.Remove(source);
                }
            }
        }
    }
    AudioSource GetAudioSource()
    {
        AudioSource audioSource = null;

        if (audioSourcesCollection.Count == 0)
        {
            CreateNewAudioSource();
        }
        audioSource = audioSourcesCollection.Dequeue();
        return audioSource;
    }

    void CreateNewAudioSource()
    {
        GameObject go = new GameObject("Audio", typeof(AudioSource));
        go.hideFlags = HideFlags.HideAndDontSave;
        go.transform.SetParent(transform);
        AudioSource audiosource = go.GetComponent<AudioSource>();
        audiosource.loop = false;
        audiosource.volume = 1;
        audiosource.playOnAwake = false;
        audioSourcesCollection.Enqueue(audiosource);
    }


    public bool MuteStatus;
    public void SetVolumeMute()
    {
        muteAllAudio = true;

        foreach (var item in audioSourcesCollection)
        {
            item.mute = true;
        }

        foreach (var collection in audiosActive)
        {
            foreach (var audio in collection.audioActive)
            {
                audio.mute = true;
            }
        }
    }

    public void SetVolumeUnMute()
    {
        muteAllAudio = false;

        foreach (var item in audioSourcesCollection)
        {
            item.mute = false;
        }

        foreach (var collection in audiosActive)
        {
            foreach (var audio in collection.audioActive)
            {
                audio.mute = false;
            }
        }
    }
}

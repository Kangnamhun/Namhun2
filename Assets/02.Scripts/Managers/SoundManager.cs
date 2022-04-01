using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//bgmó�� 100�ۼ�Ʈ ������ϴ� ���常 ���� -->���� ������Ʈ�� ��Ȱ��ȭ�ǰų� �ı��Ǹ� �Ҹ��� ����°� ����
//ex) ���� �Ҹ�, ������ �Ҹ� �� ���� �����ؾ��Ѵ�. 
public class SoundManager 
{
    AudioSource[] _audioSources = new AudioSource[(int)Define.Sound.MaxCount]; //�з���
    public AudioSource _rotatorFireBallSource;
    Dictionary<string, AudioClip> _audipClips = new Dictionary<string, AudioClip>();
    float allVolume;
    float bGMVolume;
    float effectVolume;
    public void Init()
    {
        GameObject root = GameObject.Find("@Sound");
        if(root == null)
        {
            root = new GameObject { name = "@Sound" };
            Object.DontDestroyOnLoad(root);

            string[] soundNames = System.Enum.GetNames(typeof(Define.Sound));
            for(int i =0; i< soundNames.Length-1; i++) // maxcount �־���
            {
                GameObject go = new GameObject { name = soundNames[i] };
                _audioSources[i] = go.AddComponent<AudioSource>();
                go.transform.parent = root.transform;
            }
            _audioSources[(int)Define.Sound.BGM].loop = true;
            _audioSources[(int)Define.Sound.LoopEffect].loop = true;
        }
        allVolume = 1;
        bGMVolume = 1;
        effectVolume = 1;
    }

    public void Clear()
    {
        foreach(AudioSource audioSource in _audioSources)
        {
            audioSource.clip = null;
            audioSource.Stop();
        }
        _audipClips.Clear();
    }
   public void Play(string path, Define.Sound type = Define.Sound.Effect,float pitch = 1.0f) // ���� 2�� ����
   {
        AudioClip audioClip = GetOrAddAudioClip(path, type);
        Play(audioClip, type, pitch);

    }
    public void Play(AudioClip audioClip, Define.Sound type = Define.Sound.Effect, float pitch = 1.0f)
    {
        if (audioClip == null)
            return;
        if (type == Define.Sound.BGM)
        {
          
            AudioSource audioSource = _audioSources[(int)Define.Sound.BGM];

            if (audioSource.isPlaying) // bgm�� �������̶��
                audioSource.Stop();

            audioSource.pitch = pitch;
            audioSource.clip = audioClip;
            audioSource.Play();
        }
        else if(type ==Define.Sound.Effect) //effect
        {

            AudioSource audioSource = _audioSources[(int)Define.Sound.Effect];
            audioSource.pitch = pitch;
            audioSource.PlayOneShot(audioClip);
            
        }
        else if (type == Define.Sound.Moving)
        {

            if (audioClip == _audioSources[(int)Define.Sound.Moving].clip)
            {
                return;
            }
            AudioSource audioSource = _audioSources[(int)Define.Sound.Moving];
            audioSource.clip = audioClip;
            audioSource.pitch = pitch;
            audioSource.Play();
        }
        else 
        {
            AudioSource audioSource = _audioSources[(int)Define.Sound.LoopEffect];
            audioSource.clip = audioClip;
            audioSource.pitch = pitch;
            audioSource.Play();
        }
    }
    public void StopSound(Define.Sound type)
    {
        _audioSources[(int)type].clip = null;

    }
    public void SnowBallSound(string path ,float volume)
    {
        AudioSource audioSource = _audioSources[(int)Define.Sound.LoopEffect];
        audioSource.volume = volume;
    }
    public AudioClip GetOrAddAudioClip(string path, Define.Sound type = Define.Sound.Effect)
    {
        if (path.Contains("Sounds/") == false)
            path = $"Sounds/{path}";
        AudioClip audioClip = null;

        if (type == Define.Sound.BGM)
        {
            audioClip = Managers.Resource.Load<AudioClip>(path); // bgm�� ���ֺ��ϴ°� �ƴ϶�
        }


        else if(type== Define.Sound.Effect)//effect                �޸� �Ƴ���
        {
            if (_audipClips.TryGetValue(path, out audioClip) == false)
            {
                audioClip = Managers.Resource.Load<AudioClip>(path);
                _audipClips.Add(path, audioClip);
            }
        }
        else if (type ==Define.Sound.Moving)
        {
            if (_audipClips.TryGetValue(path, out audioClip) == false)
            {
                audioClip = Managers.Resource.Load<AudioClip>(path);
                _audipClips.Add(path, audioClip);
            }
        }
        else
        {
            if (_audipClips.TryGetValue(path, out audioClip) == false)
            {
                audioClip = Managers.Resource.Load<AudioClip>(path);
                _audipClips.Add(path, audioClip);
            }
        }
        if (audioClip == null)
        {
#if UNITY_EDITOR
            Debug.Log("AudioClip Missing! ");
#endif
        }

        return audioClip;
    }





    public void AllSoundCtrl(float value)
    {
         allVolume = value;
        _audioSources[(int)Define.Sound.BGM].volume = value * bGMVolume;
        _audioSources[(int)Define.Sound.Effect].volume = value * effectVolume;
        _audioSources[(int)Define.Sound.LoopEffect].volume = value * effectVolume;
        _audioSources[(int)Define.Sound.Moving].volume = value * effectVolume;
    }   
    public void BGMSoundCtrl(float value)
    {
        bGMVolume = value;
         _audioSources[(int)Define.Sound.BGM].volume = value * allVolume;
        
    }
    public void EffectSoundCtrl(float value)
    {
        effectVolume = value;
        _rotatorFireBallSource.volume = value * allVolume;
        _audioSources[(int)Define.Sound.Effect].volume = value * allVolume;
        _audioSources[(int)Define.Sound.LoopEffect].volume = value * allVolume;
        _audioSources[(int)Define.Sound.Moving].volume = value * allVolume;
    }
}

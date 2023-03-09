using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 사운드 플레이어 클래스
/// 싱글톤
/// 1개의 기본 SoundObject사용, 2개 이상 동시 사운드 처리할 경우 동적 생성
/// </summary>
public class SoundPlayer : MonoBehaviour
{
    public static SoundPlayer Instance { get; private set; }

    [SerializeField]
    protected AudioSource audioSource_bgm = null;

    [SerializeField]
    protected SoundObject audio_basic = null;

    [SerializeField]
    protected AudioClip[] audioClips_Effect = null;

    protected List<SoundObject> list_Sound = new List<SoundObject> ();

    protected Dictionary<SoundType, AudioClip> dic_EffectSound = new Dictionary<SoundType, AudioClip>();

    protected readonly string SOUND_NAME = "_Sound";

    protected readonly float BGM_VOLUME = 0.25f;


    private void Awake()
    {
        Instance = this;

        dic_EffectSound.Clear ();

        list_Sound.Add(audio_basic);

        for(int i = 0; i < audioClips_Effect.Length; i++)
        {
            //Enum.Parse는 Type enumType(Enum의 이름), string value(해당 Enum의 멤버 string 이름))
            //을 통해 object로 리턴하고 캐스팅해 원하는 타입으로 받을 수 있다
            //등록된 오디오클립의 이름이 Enum의 멤버라고 했으므로 오디오클립 파일명과 Enum멤버의 이름이 같아야한다
            dic_EffectSound.Add((SoundType)System.Enum.Parse(typeof(SoundType), audioClips_Effect[i].name), audioClips_Effect[i]);
        }
    }

    /// <summary>
    /// 이 메서드는 이 MonoBehavior가 사라질 때 호출
    /// </summary>
    private void OnDestroy()
    {
        Instance = null;
    }

    ///
    public bool IsBgmPlaying
    {
        get
        {
            return audioSource_bgm.isPlaying;
        }
    }

    public bool IsPause()
    {
        if (Time.timeScale <= 0)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 사운드 재생 여부
    /// </summary>
    /// <returns></returns>
    public bool IsPlaying()
    {
        foreach (SoundObject sound in list_Sound)
        {
            if (sound.IsPlaying)
            {
                return true;
            }
            
        }
        return false;
    }

    /// <summary>
    /// 사운드 재생 여부
    /// </summary>
    /// <param name="clip"></param>
    /// <returns></returns>
    public bool IsPlaying(AudioClip clip)
    {
        foreach (SoundObject sound in list_Sound)
        {
            if(sound.AudioClip == clip)
            {
                if(sound.IsPlaying)
                {
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// 사운드 타입에 대한 사운드 클립을 리턴하는 메서드
    /// </summary>
    /// <param name="type">어떤 타입에 대한걸 원하는지</param>
    /// <returns>사운드 타입에 대응하는 오디오 클립</returns>
    public AudioClip GetSoundClip(SoundType type)
    {
        if (dic_EffectSound.ContainsKey(type))
        {
            return dic_EffectSound[type];
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// 배경음악 재생하는 메서드
    /// </summary>
    /// <param name="clip">어떤 오디오 클립인지</param>
    /// <param name="volume">볼륨을 몇으로 할지</param>
    /// <param name="isloop">루프를 걸지</param>
    public void PlayBGM(AudioClip clip, float volume, bool isloop)
    {
        if(clip == null)
        {
            Debug.Log("BGM clip is null");
            return;
        }
        if(IsBgmPlaying)
        {
            audioSource_bgm.Stop();
        }

        audioSource_bgm.clip = clip;
        audioSource_bgm.volume = volume;
        audioSource_bgm.loop = isloop;
        audioSource_bgm.Play();
    }

    /// <summary>
    /// 배경음악 재생하는 메서드
    /// </summary>
    public void PlayBGM(AudioClip clip, float volume)
    {
        PlayBGM(clip, volume, true);
    }

    /// <summary>
    /// 배경음악 재생하는 메서드
    /// </summary>
    public void PlayBGM(AudioClip clip)
    {
        PlayBGM(clip, BGM_VOLUME);
    }

    public void PlayBGM()
    {
        PlayBGM(audioSource_bgm.clip);
    }

    /// <summary>
    /// 재생 중 삭제
    /// </summary>
    public void StopBGM()
    {
        audioSource_bgm.Stop();
    }

    /// <summary>
    /// 재생 중 멈춤
    /// </summary>
    public void PauseBGM()
    {
        if(audioSource_bgm.isPlaying)
        {
            audioSource_bgm.Pause();
        }
    }

    /// <summary>
    /// 다시 재생
    /// </summary>
    public void UnpauseBGM()
    {
        audioSource_bgm.UnPause();
    }

    /// <summary>
    /// 새로운 사운드 오브젝트 생성해주는 메서드
    /// </summary>
    /// <param name="clip">해당 오브젝트에 들어갈 오디오 클립</param>
    /// <returns>생성된 사운드 오브젝트 리턴</returns>
    protected SoundObject CreateSoundObject(AudioClip clip)
    {
        GameObject Obj = new GameObject(SOUND_NAME);
        Obj.transform.SetParent(audio_basic.transform);
        SoundObject soundObj = Obj.AddComponent<SoundObject>();
        soundObj.AudioClip = clip;
        return soundObj;
    }

    

    public void UnpauseAllSound(bool includeBGM = true)
    {
        foreach(SoundObject soundObject in list_Sound)
        {
            soundObject.UnPause();
        }
        if(includeBGM)
        {
            audioSource_bgm.UnPause();
        }
    }

    public void PauseAllSound(bool includeBGM = true)
    {
        foreach(SoundObject soundObj in list_Sound)
        {
            soundObj.Pause();
        }
        if (includeBGM)
        {
            audioSource_bgm.Pause();
        }
    }

    public bool StopSound(AudioClip clip)
    {
        for(int i = 0; i < list_Sound.Count; i++)
        {
            if (list_Sound[i].AudioClip == clip)
            {
                list_Sound[i].Stop();

                if(i != 0)
                {
                    list_Sound.Remove(list_Sound[i]);
                }
                return true;
            }
        }

        return false;
    }

    public bool StopAllSound(bool includeBGM)
    {
        foreach(SoundObject soundObject in list_Sound)
        {
            soundObject.Stop();
        }

        if(includeBGM)
        {
            audioSource_bgm.Stop();
        }

        ClearAllSound();

        return true;
    }

    protected void ClearAllSound()
    {
        list_Sound.Clear();
        list_Sound.Add(audio_basic);
    }

    public bool StopAllSOund()
    {
        return StopAllSound(false);
    }


    /// <summary>
    /// 사운드 플레이, 하나 이상 재생중일 때 새로운 오브젝트 생성, 아니면 기존 basic에서 재생
    /// </summary>
    /// <param name="clip">재생할 오디오 클립</param>
    /// <param name="volume">볼륨은 몇으로 할지</param>
    /// <param name="delaySeconds">몇초간 딜레이 후에 재생할지</param>
    /// <param name="isLoop">루프를 돌지</param>
    /// <param name="isStoppable">멈출수 있는지</param>
    /// <param name="finishListener">끝날 때 실행할 델리게이트</param>
    public void PlaySound(AudioClip clip, float volume, float delaySeconds, bool isLoop, bool isStoppable, System.Action finishListener)
    {
        if (clip == null)
        {
            Debug.Log("No Sound Clip");
            return;
        }

        if (IsPlaying() || IsPause())
        {
            SoundObject obj_Sound = CreateSoundObject(clip);

            list_Sound.Add(obj_Sound);
            obj_Sound.Play(volume, delaySeconds, isLoop, isStoppable, () =>
            {
                list_Sound.Remove(obj_Sound);
                if (finishListener != null)
                {
                    finishListener();
                }
            });
        }
        else
        {
            audio_basic.AudioClip = clip;
            audio_basic.Play(volume, delaySeconds, isLoop, isStoppable, () =>
            {
                if (finishListener != null)
                {
                    finishListener();
                }
            });

        }
    }

    public void PlaySound(AudioClip clip, float volume, float delaySeconds, bool isLoop, System.Action finishListener)
    {
        PlaySound(clip, volume, delaySeconds, isLoop, true, finishListener);
    }

    public void PlaySound(AudioClip clip, float volume, float delaySeconds, System.Action finishListener)
    {
        PlaySound(clip, volume, delaySeconds, false, finishListener);
    }

    public void PlaySound(AudioClip clip, float volume, System.Action finishListener)
    {
        PlaySound(clip, volume, 0, false, finishListener);
    }

    public void PlaySound(AudioClip clip, System.Action finishListener)
    {
        PlaySound(clip, 1f, finishListener);
    }

    public void PlaySound(SoundType soundType, float volume)
    {
        if (soundType == SoundType.None)
        {
            return;
        }

        PlaySound(GetSoundClip(soundType), volume, null);
    }

    public void PlaySound(SoundType soundType, bool isStoppable)
    {
        if(soundType == SoundType.None)
        {
            return;
        }

        PlaySound(GetSoundClip(soundType), 1.0f, 0, false, isStoppable, null);
    }

    public void PlaySound(AudioClip clip)
    {
        PlaySound(clip, null);
    }

    public void PlaySound(SoundType soundType)
    {
        PlaySound(soundType, true);
    }

    public IEnumerator CoPlaySound(AudioClip clip, float minWaitSec = 0.5f)
    {
        float waitSec = (clip != null && clip.length > minWaitSec) ? clip.length : minWaitSec;
        PlaySound(clip, null);
        yield return new WaitForSeconds(waitSec);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ParticleObject : MonoBehaviour
{
    [SerializeField]
    protected ParticleSystem particleSystemSource;

    /// <summary>
    /// 파티클 시스템의 프로퍼티를 변경하려면 MainModule을 통해 접근해야 한다고 한다
    /// </summary>
    public ParticleSystem ParticleSystemSource
    {
        get { return particleSystemSource; }
    }

    private void Awake()
    {
        particleSystemSource = GetComponent<ParticleSystem>();
    }

    /// <summary>
    /// 파티클 시스템 재생, 프로퍼티를 주고 상태를 변경하기 위해서는 ParticleSystem.Module을 타입으로 변수로 만들어서 변경할 것
    /// 다만 유니티 가이드에 전역 프로퍼티라 시스템 전체에 영향을 준다고 안내되어있음
    /// </summary>
    public void Play()
    {
        particleSystemSource.Play();
        StartCoroutine(DestroyAfterPlay());
    }

    public void Play(float playTime)
    {
        particleSystemSource.Play();
        StartCoroutine(DestroyAfterPlay(playTime));
        
    }


    IEnumerator DestroyAfterPlay()
    {
        float lifeTime = particleSystemSource.main.duration;
        yield return new WaitForSeconds(lifeTime);
        DestroyParticle();
    }

    IEnumerator DestroyAfterPlay(float playTime)
    {
        float lifeTime = playTime;
        yield return new WaitForSeconds(lifeTime);
        DestroyParticle();
    }

    public void Pause()
    {
        particleSystemSource.Pause();
    }


    public void Stop()
    {
        particleSystemSource.Stop();
        DestroyParticle();
    }

    public void DestroyParticle()
    {
        try
        {
            Destroy(this.gameObject);
        }
        catch(System.Exception e)
        {
            Debug.Log(e.ToString());
        }
    }
}

using System.Collections;
using UnityEngine;

public class BGMManager : MonoBehaviour
{
    public static BGMManager instance;
    
    // BGM 재생을 위한 오디오 소스와 클립
    public AudioSource bgmAudioSource;
    public AudioClip backgroundMusicClip;
    public float fadeOutTime = 2f; // 페이드 아웃 지속 시간
    
    private void Awake()
    {
        // 싱글톤 패턴 구현
        if (instance == null)
        {
            instance = this;
            // DontDestroyOnLoad(gameObject); // 씬 전환 시에도 유지하려면 주석 해제
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        // AudioSource 컴포넌트 확인 및 생성
        if (bgmAudioSource == null)
        {
            bgmAudioSource = GetComponent<AudioSource>();
            if (bgmAudioSource == null)
            {
                bgmAudioSource = gameObject.AddComponent<AudioSource>();
            }
        }
        
        // BGM 재생 시작
        if (backgroundMusicClip != null)
        {
            bgmAudioSource.clip = backgroundMusicClip;
            bgmAudioSource.loop = true;
            bgmAudioSource.volume = 0.5f;
            bgmAudioSource.Play();
        }
    }
    
    // BGM을 페이드 아웃하며 정지
    public void StopBGMWithFade()
    {
        StartCoroutine(FadeOutBGM());
    }
    
    // 볼륨을 점진적으로 줄이는 코루틴
    IEnumerator FadeOutBGM()
    {
        if (bgmAudioSource == null) yield break;
        
        float startVolume = bgmAudioSource.volume;
        float timer = 0f;
        
        while (timer < fadeOutTime)
        {
            timer += Time.unscaledDeltaTime; // Time.timeScale이 0일 때도 작동
            bgmAudioSource.volume = Mathf.Lerp(startVolume, 0f, timer / fadeOutTime);
            yield return null;
        }
        
        bgmAudioSource.Stop();
        bgmAudioSource.volume = 0f;
    }
}
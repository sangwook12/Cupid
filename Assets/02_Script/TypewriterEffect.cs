using System;
using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// TextMeshPro 텍스트를 한 글자씩 보여주는 타이핑 효과.
/// maxVisibleCharacters 방식이라 <color>, <b> 같은 리치 텍스트 태그가 있어도 깨지지 않는다.
/// </summary>
[RequireComponent(typeof(TMP_Text))]
public class TypewriterEffect : MonoBehaviour
{
    [Header("속도")]
    [SerializeField] private float charsPerSecond = 30f;

    [Header("문장부호에서 잠깐 멈추기 (초)")]
    [SerializeField] private float commaPause = 0.15f;     // , 、
    [SerializeField] private float sentencePause = 0.35f;  // . ! ? …

    private TMP_Text tmp;
    private Coroutine routine;

    public bool IsTyping { get; private set; }

    /// <summary>타이핑이 끝났을 때(스킵 포함) 호출됨</summary>
    public event Action OnFinished;

    private void Awake()
    {
        tmp = GetComponent<TMP_Text>();
    }

    public void Play(string text)
    {
        Stop();
        tmp.text = text;
        tmp.maxVisibleCharacters = 0;
        routine = StartCoroutine(TypeRoutine());
    }

    /// <summary>타이핑 중이면 즉시 전체 문장을 보여준다.</summary>
    public void Skip()
    {
        if (!IsTyping) return;
        Stop();
        tmp.maxVisibleCharacters = int.MaxValue;
        OnFinished?.Invoke();
    }

    private void Stop()
    {
        if (routine != null) StopCoroutine(routine);
        routine = null;
        IsTyping = false;
    }

    private IEnumerator TypeRoutine()
    {
        IsTyping = true;

        // 텍스트 메시를 먼저 갱신해야 태그를 뺀 실제 글자 수를 정확히 알 수 있다.
        tmp.ForceMeshUpdate();
        TMP_TextInfo info = tmp.textInfo;
        int total = info.characterCount;
        float interval = 1f / Mathf.Max(1f, charsPerSecond);

        for (int i = 1; i <= total; i++)
        {
            tmp.maxVisibleCharacters = i;

            char c = info.characterInfo[i - 1].character;
            float wait = interval + PauseFor(c);

            // 일시정지(timeScale = 0) 중에도 연출이 돌도록 Realtime 사용
            yield return new WaitForSecondsRealtime(wait);
        }

        IsTyping = false;
        routine = null;
        OnFinished?.Invoke();
    }

    private float PauseFor(char c)
    {
        switch (c)
        {
            case ',':
            case '、':
                return commaPause;
            case '.':
            case '!':
            case '?':
            case '…':
                return sentencePause;
            default:
                return 0f;
        }
    }
}

using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class CutsceneLine
{
    public string speaker;                 // 화자 이름 (비우면 이름칸 숨김)
    [TextArea(2, 5)] public string text;   // 대사
}

/// <summary>
/// 대사 배열을 순서대로 타이핑 효과와 함께 보여주는 컷신 컨트롤러.
/// 화면 전체를 덮는 투명 Button의 onClick에 Advance()를 연결하면
/// 새 Input System / 기존 Input Manager 어느 쪽이든 동작한다.
/// </summary>
public class CutsceneController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TypewriterEffect typewriter;
    [SerializeField] private TMP_Text speakerText;
    [SerializeField] private GameObject nextIndicator;   // ▼ 같은 "다음" 표시 (선택)

    [Header("대사")]
    [SerializeField] private CutsceneLine[] lines;

    [Header("컷신이 끝나면")]
    public UnityEvent onCutsceneEnd;

    private int index = -1;
    private bool playing;

    private void OnEnable()  => typewriter.OnFinished += HandleLineFinished;
    private void OnDisable() => typewriter.OnFinished -= HandleLineFinished;

    private void Start()
    {
        StartCutscene();
    }

    public void StartCutscene()
    {
        index = -1;
        playing = true;
        ShowNextLine();
    }

    /// <summary>클릭/터치/스페이스 등 "다음" 입력이 들어왔을 때 호출</summary>
    public void Advance()
    {
        if (!playing) return;

        if (typewriter.IsTyping)
            typewriter.Skip();      // 타이핑 중이면 한 번에 다 보여주기
        else
            ShowNextLine();         // 다 나왔으면 다음 대사로
    }

    private void ShowNextLine()
    {
        index++;
        if (index >= lines.Length)
        {
            EndCutscene();
            return;
        }

        CutsceneLine line = lines[index];

        if (speakerText != null)
        {
            bool hasSpeaker = !string.IsNullOrEmpty(line.speaker);
            speakerText.gameObject.SetActive(hasSpeaker);
            speakerText.text = line.speaker;
        }

        if (nextIndicator != null) nextIndicator.SetActive(false);
        typewriter.Play(line.text);
    }

    private void HandleLineFinished()
    {
        if (nextIndicator != null) nextIndicator.SetActive(true);
    }

    private void EndCutscene()
    {
        playing = false;
        if (nextIndicator != null) nextIndicator.SetActive(false);
        onCutsceneEnd?.Invoke();
    }
}

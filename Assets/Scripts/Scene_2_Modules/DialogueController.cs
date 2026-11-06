using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class DialogueLine
{
    public string speaker;
    public Sprite icon;                    // 화자 아이콘 (없으면 비움)
    [TextArea(2, 4)] public string text;
}

public class DialogueController : MonoBehaviour
{
    [SerializeField] private List<DialogueLine> lines = new List<DialogueLine>();
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private TMP_Text speakerName;
    [SerializeField] private Image speakerIcon;
    [SerializeField] private GameObject nextIndicator;
    [SerializeField] private float charsPerSecond = 30f;

    public event Action Finished;

    private int index = -1;
    private Coroutine typing;
    private bool isTyping;

    private void Start()
    {
        Next();
    }

    // ClickArea의 OnClick에 연결
    public void Next()
    {
        // 타이핑 중이면 한 번에 전체 대사를 보여 줌
        if (isTyping)
        {
            FinishTyping();
            return;
        }

        index++;
        if (index >= lines.Count)
        {
            SetIndicator(false);
            Finished?.Invoke();
            return;
        }

        ShowLine(lines[index]);
    }

    private void ShowLine(DialogueLine line)
    {
        speakerName.text = line.speaker;
        speakerIcon.sprite = line.icon;
        speakerIcon.enabled = line.icon != null;

        SetIndicator(false);
        typing = StartCoroutine(Typewriter(line.text));
    }

    private IEnumerator Typewriter(string text)
    {
        isTyping = true;
        dialogueText.text = text;
        dialogueText.maxVisibleCharacters = 0;

        int total = text.Length;
        float elapsed = 0f;
        int shown = 0;

        while (shown < total)
        {
            elapsed += Time.deltaTime;
            shown = Mathf.Min(total, Mathf.FloorToInt(elapsed * charsPerSecond));
            dialogueText.maxVisibleCharacters = shown;
            yield return null;
        }

        FinishTyping();
    }

    private void FinishTyping()
    {
        if (typing != null)
        {
            StopCoroutine(typing);
            typing = null;
        }

        isTyping = false;
        dialogueText.maxVisibleCharacters = int.MaxValue;
        SetIndicator(true);
    }

    private void SetIndicator(bool on)
    {
        if (nextIndicator != null) nextIndicator.SetActive(on);
    }
}

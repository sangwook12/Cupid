using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct CutSubmission
{
    public int CutIndex;
    public string ClueLeft;
    public string ClueRight;
    public string Action;
}

public class StoryboardController : MonoBehaviour
{
    public event Action<List<CutSubmission>> Submitted;

    private DropSlot[] slots;

    private void Awake()
    {
        // MainPanel에 붙여서 StoryBoard와 ClueTray의 칸을 모두 찾음
        slots = GetComponentsInChildren<DropSlot>(true);
    }

    // 4컷의 현재 배치를 수집 (보관함 칸은 CutIndex가 0이라 제외됨)
    public List<CutSubmission> Collect()
    {
        var result = new List<CutSubmission>();

        for (int cut = 1; cut <= 4; cut++)
        {
            var sub = new CutSubmission { CutIndex = cut };

            foreach (var slot in slots.Where(s => s.CutIndex == cut))
            {
                string id = slot.Items.Count > 0 ? slot.Items[0].Id : null;

                if (slot.SlotType == ItemType.Action) sub.Action = id;
                else if (slot.SlotIndex == 0) sub.ClueLeft = id;
                else sub.ClueRight = id;
            }

            result.Add(sub);
        }

        return result;
    }

    // SubmitButton의 OnClick에 연결
    public void Submit()
    {
        Submitted?.Invoke(Collect());
    }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class DropSlot : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private ItemType slotType = ItemType.Clue;  // Any = 아무거나 받음
    [SerializeField] private int cutIndex = 1;                   // 보관함은 0
    [SerializeField] private int slotIndex = 0;                  // 단서 칸: 0 = 왼쪽, 1 = 오른쪽
    [SerializeField] private int maxItems = 1;                   // -1 = 무제한
    [SerializeField] private Vector2 iconSize = new Vector2(64, 64);

    [SerializeField] private Color emptyColor = new Color32(0xE8, 0xD9, 0xB5, 0xFF);
    [SerializeField] private Color filledColor = new Color32(0xE8, 0xD9, 0xB5, 0xFF);
    [SerializeField] private Color validColor = new Color32(0x5A, 0x7D, 0x3A, 0xFF);
    [SerializeField] private Color invalidColor = new Color32(0xB5, 0x45, 0x2E, 0xFF);

    private readonly List<DraggableIcon> items = new List<DraggableIcon>();
    private Image image;

    public ItemType SlotType => slotType;
    public int CutIndex => cutIndex;
    public int SlotIndex => slotIndex;
    public IReadOnlyList<DraggableIcon> Items => items;

    private void Awake()
    {
        image = GetComponent<Image>();
        RefreshColor();
    }

    private bool HasSpace => maxItems < 0 || items.Count < maxItems;

    private bool Accepts(DraggableIcon icon)
    {
        if (icon == null || !HasSpace) return false;
        return slotType == ItemType.Any || icon.Type == slotType;
    }

    public void Add(DraggableIcon icon)
    {
        items.Add(icon);
        icon.SetHome(this);

        var r = (RectTransform)icon.transform;
        r.SetParent(transform, false);
        r.sizeDelta = iconSize;
        r.anchoredPosition = Vector2.zero;   // 칸 중앙에 스냅

        RefreshColor();
    }

    public void Remove(DraggableIcon icon)
    {
        if (!items.Remove(icon)) return;
        icon.SetHome(null);
        RefreshColor();
    }

    public void OnDrop(PointerEventData e)
    {
        var icon = e.pointerDrag != null ? e.pointerDrag.GetComponent<DraggableIcon>() : null;
        if (!Accepts(icon)) return;   // 거부되면 DraggableIcon.OnEndDrag에서 원래 칸으로 복귀
        Add(icon);
    }

    public void OnPointerEnter(PointerEventData e)
    {
        var icon = e.pointerDrag != null ? e.pointerDrag.GetComponent<DraggableIcon>() : null;
        if (icon == null) return;
        image.color = Accepts(icon) ? validColor : invalidColor;
    }

    public void OnPointerExit(PointerEventData e)
    {
        RefreshColor();
    }

    private void RefreshColor()
    {
        image.color = items.Count > 0 ? filledColor : emptyColor;
    }
}

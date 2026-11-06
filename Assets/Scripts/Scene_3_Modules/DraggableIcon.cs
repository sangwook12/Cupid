using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
public class DraggableIcon : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private ItemType type = ItemType.Clue;
    [SerializeField] private RectTransform dragLayer;   // 비워두면 "DragLayer"를 자동으로 찾음

    public ItemType Type => type;
    public string Id { get; private set; }
    public DropSlot Home { get; private set; }          // 현재 놓인 칸 (없으면 null)

    private CanvasGroup group;
    private RectTransform rect;
    private DropSlot returnSlot;

    private void Awake()
    {
        group = GetComponent<CanvasGroup>();
        rect = (RectTransform)transform;

        if (dragLayer == null)
        {
            var found = GameObject.Find("DragLayer");
            if (found != null) dragLayer = (RectTransform)found.transform;
        }
    }

    // RunGenerator가 아이콘을 만든 직후 호출
    public void Init(ItemType t, string id, DropSlot startSlot)
    {
        type = t;
        Id = id;
        startSlot.Add(this);
    }

    // DropSlot이 호출
    public void SetHome(DropSlot slot) => Home = slot;

    public void OnBeginDrag(PointerEventData e)
    {
        returnSlot = Home;
        if (Home != null) Home.Remove(this);

        group.blocksRaycasts = false;                    // 아래 칸이 드롭을 받도록
        if (dragLayer != null) rect.SetParent(dragLayer, true);
    }

    public void OnDrag(PointerEventData e)
    {
        rect.position = e.position;
    }

    public void OnEndDrag(PointerEventData e)
    {
        group.blocksRaycasts = true;

        // 어떤 칸에도 안 들어갔으면 원래 칸으로 복귀
        if (Home == null && returnSlot != null)
            returnSlot.Add(this);
    }
}

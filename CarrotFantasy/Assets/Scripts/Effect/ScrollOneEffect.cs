using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

/// <summary>
/// ScrollView实现翻书效果
/// </summary>
public class ScrollOneEffect : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    public int totalItem; // 单元格总数
    public int cellLen; // 单元格长度
    public int cellSpacing; // 单元格之间间隔
    public int leftPadding; // content左偏移
    public Text pageText; // 页数显示
    public bool needSendMessage = false;

    private ScrollRect scrollRect;
    private RectTransform content;

    private float beginMousePos;
    private float endMousePos;
    private float moveOneLength; // 移动一个单元格需要的长度
    private Vector3 currentLocalPos; // 当前content的局部坐标
    private Vector3 originLocalPos; // content初始的局部坐标(第一页的坐标)
    private Vector2 originContentSize; // content 初始的大小
    private int currentIndex; // 当前页数的index
    private int pageNum;

    void Awake()
    {
        scrollRect = GetComponent<ScrollRect>();
        content = scrollRect.content;
        moveOneLength = cellLen + cellSpacing;
        originLocalPos = content.localPosition;
        originContentSize = content.sizeDelta;
        currentLocalPos = content.localPosition;
        currentIndex = 1;
        pageNum = content.childCount;
        UpdatePageUI();
    }

    public void InitScrollView()
    {
        currentIndex = 1;
        content.localPosition = originLocalPos;
        currentLocalPos = originLocalPos;
        UpdatePageUI();
    }

    public void UpdatePageUI()
    {
        if (pageText != null) pageText.text = currentIndex + "/" + pageNum;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        beginMousePos = Input.mousePosition.x; // 开始拖拽记录鼠标位置
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        endMousePos = Input.mousePosition.x;

        float offsetX = beginMousePos - endMousePos;
        float moveDistance = 0; // 当次需要滑动的距离
        if (offsetX > 0) // 右滑
        {
            if (currentIndex == totalItem) return;
            moveDistance = -moveOneLength;
            currentIndex++;
            if(needSendMessage) UpdatePanelMessage(1);
        }
        else // 左滑
        {
            if (currentIndex == 1) return;
            moveDistance = moveOneLength;
            currentIndex--;
            if (needSendMessage) UpdatePanelMessage(-1);
        }

        if (moveDistance != 0) // 鼠标有滑动时实现翻书上一页或者下一页
        {
            UpdatePageUI();
            currentLocalPos += new Vector3(moveDistance, 0, 0);
            DOTween.To(() => content.localPosition, lerpValue => content.localPosition = lerpValue, currentLocalPos, 0.1f).SetEase(Ease.InOutQuint);
            GameManager.Instance.AudioManager.PlayPageAudioEffect();
        }
    }

    /// <summary>
    /// 翻页的方法  toRight值为1时向右翻,-1时向左翻
    /// </summary>
    public void ToNextPage(int toRight)
    {
        if ((toRight == 1 && currentIndex == totalItem) || (toRight == -1 && currentIndex == 1)) return;

        currentIndex += toRight;
        float moveDistance = -moveOneLength * toRight;

        currentLocalPos += new Vector3(moveDistance, 0, 0);
        DOTween.To(() => content.localPosition, lerpValue => content.localPosition = lerpValue, currentLocalPos, 0.1f).SetEase(Ease.OutQuint);
    }

    //设置Content长度
    public void SetContentLength(int itemNum)
    {
        content.sizeDelta = new Vector2(content.sizeDelta.x + ((itemNum - 1) * (cellLen + cellSpacing)), content.sizeDelta.y);
        totalItem = itemNum;
    }

    // 初始化Content大小
    public void InitContentLength()
    {
        content.sizeDelta = originContentSize;
    }
    /// <summary>
    /// 更新页面的消息(toRight等于1表示向右滑动, 等于-1表示向左滑动)
    /// </summary>
    public void UpdatePanelMessage(int toRight)
    {
        gameObject.SendMessageUpwards("ToNextLevel", toRight);
    }
}

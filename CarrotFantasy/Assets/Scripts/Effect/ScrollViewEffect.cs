using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

/// <summary>
/// 根据滑动比例实现翻书效果,可实现多滑
/// </summary>
public class ScrollViewEffect : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    public int totalItem; // 单元格总数
    public int cellLen; // 单元格长度
    public int cellSpacing; // 单元格之间间隔
    public int leftPadding; // content左偏移

    private ScrollRect scrollRect;

    private float contentLength; // ScrollView容器长度
    private float beginMousePosX;
    private float endMousePosX;
    private float lastProportion; // 上一个单元格所在的比例
    private float upperLimit; // 滑动上限
    private float lowerLimit; // 滑动下限
    private float firstMoveDis; // 第一个单元格移动需要的距离
    private float oneMoveDis; // 移动一个单元格需要的距离
    private float oneProportion; // 一个单元格所占的比例
    private int currentIndex; // 当前所在单元格的索引

    private void Awake()
    {
        // 初始化数据和引用
        scrollRect = GetComponent<ScrollRect>();
        contentLength = scrollRect.content.rect.xMax - (2 * leftPadding + cellLen);
        firstMoveDis = cellLen / 2 + leftPadding;
        oneMoveDis = cellLen + cellSpacing;
        oneProportion = oneMoveDis / contentLength;
        lowerLimit = firstMoveDis / contentLength;
        upperLimit = 1 - lowerLimit;
        currentIndex = 1;
        scrollRect.horizontalNormalizedPosition = 0;
    }

    #region 鼠标拖拽监听事件

    public void OnBeginDrag(PointerEventData eventData)
    {
        beginMousePosX = Input.mousePosition.x;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        endMousePosX = Input.mousePosition.x;
        float offsetX = 2 * (beginMousePosX - endMousePosX);  // 鼠标拖动的偏移量(世界坐标长度)
        if (Mathf.Abs(offsetX) >= firstMoveDis) // 执行滑动的先决条件
        {
            if (offsetX > 0) // 右滑
            {
                if (currentIndex >= totalItem) return;
                int moveCount = (int)((offsetX - firstMoveDis) / oneMoveDis) + 1; // 需要移动的单元格数目
                currentIndex += moveCount;
                if (currentIndex > totalItem) currentIndex = totalItem;
                lastProportion += oneProportion * moveCount; // 要变化至的比例
                if (lastProportion > upperLimit) lastProportion = 1;
            }
            else // 左滑
            {
                if (currentIndex <= 1) return;
                int moveCount = (int)((offsetX + firstMoveDis) / oneMoveDis) - 1;
                currentIndex += moveCount;
                if (currentIndex < 1) currentIndex = 1;
                lastProportion += oneProportion * moveCount;
                if (lastProportion < lowerLimit) lastProportion = 0;
            }
        }

        DOTween.To(() => scrollRect.horizontalNormalizedPosition, lerpValue => scrollRect.horizontalNormalizedPosition = lerpValue, lastProportion, 0.1f).SetEase(Ease.InOutQuint);
    }

    #endregion
}

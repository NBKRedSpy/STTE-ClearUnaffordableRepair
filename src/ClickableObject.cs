using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

namespace ClearUnaffordableRepair
{
    public class ClickableObject : MonoBehaviour, IPointerClickHandler
    {

        public event EventHandler<PointerEventData> OnLeftClick;
        public event EventHandler<PointerEventData> OnRightClick;
        public event EventHandler<PointerEventData> OnMiddleClick;

        public object Data;

        public void OnPointerClick(PointerEventData eventData)
        {
            switch (eventData.button)
            {
                case PointerEventData.InputButton.Left:
                    if (OnLeftClick != null) OnLeftClick(this, eventData);
                    break;
                case PointerEventData.InputButton.Right:
                    if (OnRightClick != null) OnRightClick(this, eventData);
                    break;
                case PointerEventData.InputButton.Middle:
                    if (OnMiddleClick != null) OnMiddleClick(this, eventData);
                    break;
            }
        }
    }
}

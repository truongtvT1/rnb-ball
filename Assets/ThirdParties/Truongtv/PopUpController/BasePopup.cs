using System;
using DG.Tweening;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Truongtv.PopUpController
{
    
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(GraphicRaycaster))]
    public class BasePopup:MonoBehaviour
    {
        [SerializeField] private PopupDisplayType displayType = PopupDisplayType.Zoom;
        [SerializeField] private Image shadowBackground;
        
        protected const float DURATION = 0.5f;
        private Canvas _canvasPopup;
        private CanvasGroup _canvasGroup;
        private PopupController _popupController;
        private RectTransform _rectTransform;
        public Action closeAction;
        public Action closeCompleteAction;
        public Action openAction;
        public Action openCompleteAction;
        public void Show(PopupController controller)
        {
            _popupController = controller;
            var sortingOrder =_popupController.CalculatingSortingOrder(this);
            Debug.Log("sortingOrder = "+sortingOrder);
            SetSortingOrder(sortingOrder);

            if (displayType == PopupDisplayType.Random)
            {
                displayType = (PopupDisplayType) Random.Range(1, 7);
            }
            openAction?.Invoke();
            switch (displayType)
            {
                case PopupDisplayType.None:
                    break;
                case PopupDisplayType.Zoom:
                    ZoomIn(openCompleteAction);
                    break;
                case PopupDisplayType.LeftToRight:
                case PopupDisplayType.RightToLeft:
                case PopupDisplayType.UptoBottom:
                case PopupDisplayType.BottomToUp:
                    Move(openCompleteAction);
                    break;
                case PopupDisplayType.FadeIn:
                    FadeIn(openCompleteAction);
                    break;
            }

            // if (shadowBackground != null)
            // {
            //     if (MenuPopupController.Instance != null)
            //     {
            //         shadowBackground.sprite = MenuPopupController.Instance.GetBackgroundImg();
            //     }
            //     else
            //     {
            //         shadowBackground.sprite = GamePlayPopupController.Instance.GetBackgroundImg();
            //     }
            // }
        }

        

        public void Close()
        {
            void CloseAction()
            {
                _popupController.ReleaseStack();
                gameObject.SetActive(false);
                closeCompleteAction?.Invoke();
            }
            closeAction?.Invoke();
            switch (displayType)
            {
                case PopupDisplayType.None:
                    CloseAction();
                    break;
                case PopupDisplayType.Zoom:
                    ZoomOut(CloseAction);
                    break;
                case PopupDisplayType.LeftToRight:
                case PopupDisplayType.RightToLeft:
                case PopupDisplayType.UptoBottom:
                case PopupDisplayType.BottomToUp:
                    MoveBack(CloseAction);
                    break;
                case PopupDisplayType.FadeIn:
                    FadeOut(CloseAction);
                    break;
            }
        }

        #region PopupAnim

        private void FadeIn(Action complete = null)
        {
            // var skels = GetComponentsInChildren<SkeletonGraphic>();
            // foreach (var skel in skels)
            // {
            //     skel.color = new Color(skel.color.r,skel.color.g,skel.color.b,1);
            // }
            CanvasGroup().alpha = 0;
            CanvasGroup().DOFade(1,DURATION)
                .SetEase(Ease.OutBack)
                .SetUpdate(UpdateType.Normal,true)
                // .OnUpdate(() =>
                // {
                //     foreach (var skel in skels)
                //     {
                //         skel.color = new Color(skel.color.r,skel.color.g,skel.color.b,CanvasGroup().alpha);
                //     }
                // })
                .onComplete = ()=>
            {
                _popupController.LockScene(false);
                complete?.Invoke();
            };
        }

        private void FadeOut(Action action)
        {
            // var skels = GetComponentsInChildren<SkeletonGraphic>();
            CanvasGroup().DOFade(0, DURATION)
                .SetEase(Ease.InBack)
                .SetUpdate(UpdateType.Normal,true)
                .OnUpdate(() =>
                {
                    // foreach (var skel in skels)
                    // {
                    //     skel.color = new Color(skel.color.r,skel.color.g,skel.color.b,CanvasGroup().alpha);
                    // }
                })
                .onComplete = action.Invoke;
        }
        private void ZoomIn(Action complete = null)
        {
            transform.localScale = Vector3.zero;
            transform.DOScale(Vector3.one, DURATION)
                .SetEase(Ease.OutBack).SetUpdate(UpdateType.Normal,true)
                .onComplete = ()=>
            {
                _popupController.LockScene(false);
                complete?.Invoke();
            };
        }

        private Vector3 _startPosition;
        private void Move(Action complete = null)
        {
            var type = displayType;
            var size = _popupController.GetComponent<RectTransform>().rect.size;
            switch (type)
            {
                case PopupDisplayType.LeftToRight:
                    _startPosition = new Vector3(-size.x-100f, 0, 0);
                    break;
                case PopupDisplayType.RightToLeft:
                    _startPosition = new Vector3(size.x+100f, 0, 0);
                    break;
                case PopupDisplayType.UptoBottom:
                    _startPosition = new Vector3(0, size.y+100f, 0);
                    break;
                case PopupDisplayType.BottomToUp:
                    _startPosition = new Vector3(0, -size.y-100f, 0);
                    break;
            }
            RectTransform().localPosition = _startPosition;
            RectTransform()
                .DOLocalMove(Vector3.zero, DURATION)
                .SetEase(Ease.OutQuad).SetUpdate(UpdateType.Normal,true)
                .onComplete =()=>
            {
                _popupController.LockScene(false);
                complete?.Invoke();
            };
        }
        
        private void MoveBack(Action action)
        {
            RectTransform().DOLocalMove(_startPosition, DURATION)
                .SetEase(Ease.InQuad).SetUpdate(UpdateType.Normal,true)
                .onComplete = action.Invoke;
        }

        private void ZoomOut(Action action)
        {
            transform.localScale = Vector3.one;
            transform.DOScale(Vector3.zero, DURATION)
                .SetEase(Ease.InBack).SetUpdate(UpdateType.Normal,true)
                .onComplete = action.Invoke;
        }

        

        #endregion
        
        private Canvas CanvasPopup()
        {
            if (_canvasPopup == null)
                _canvasPopup = GetComponent<Canvas>();
            return _canvasPopup;
        }

        private CanvasGroup CanvasGroup()
        {
            if (_canvasGroup == null)
                _canvasGroup = GetComponent<CanvasGroup>();
            return _canvasGroup;
        }
        
        public int GetSortingOrder()
        {
            return CanvasPopup().sortingOrder;
        }

        private void SetSortingOrder(int value)
        {
            CanvasPopup().sortingOrder = value;
        }
        private RectTransform RectTransform()
        {
            if (_rectTransform == null)
                _rectTransform = GetComponent<RectTransform>();
            return _rectTransform;
        }
    }

    public enum PopupDisplayType
    {
        None = 0,
        Zoom = 1,
        LeftToRight= 2,
        RightToLeft=3,
        UptoBottom=4,
        BottomToUp=5,
        FadeIn = 6,
        Random=7
    }
}
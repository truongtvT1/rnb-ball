using System;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using Spine.Unity;
using ThirdParties.Truongtv.SoundManager;
using UnityEngine;
using CharacterController = GamePlay.Characters.CharacterController;

namespace GamePlay.Door
{
    public class Cage : ObjectTrigger
    {
        [SerializeField] private SimpleAudio simpleAudio;
        [SerializeField] private AudioClip open,setKey;
        [SerializeField] private Transform gate;
        [SerializeField] private float openY;
        [SerializeField] private GameObject redKeyObj,blueKeyObj;
        [SerializeField] private SkeletonAnimation skeletonRed,skeletonBlue;
        [SerializeField,OnValueChanged(nameof(OnValueChange)),ValueDropdown(nameof(GetAllSkinName))] private string trySkinName;
        private bool _isPlayRedKey, _isPlayBlueKey, _isRedOpen, _isBlueOpen;
        void Start()
        {

            OnValueChange();
            // redBall.skeletonDataAsset.GetSkeletonData().Skins.Items
        }

        protected override void TriggerEnter(string triggerTag, Transform triggerObject)
        {
            var sequence = DOTween.Sequence();
            if (triggerTag.Equals(TagManager.RED_TAG) && KeyCollector.Instance.IsKeyCollected(TagManager.RED_TAG))
            {
                redKeyObj.transform.position = KeyCollector.Instance.redKey.transform.position;
                redKeyObj.transform.localScale = new Vector3(2,2,2);
                redKeyObj.SetActive(true);
                sequence.Append(redKeyObj.transform.DOLocalMoveX(0, 0.75f).SetEase(Ease.OutBack));
                sequence.Join(redKeyObj.transform.DOLocalMoveY(0,0.75f).SetEase(Ease.Linear));
                sequence.Join(redKeyObj.transform.DOScale(1,0.75f).SetEase(Ease.Linear));
                PlayKeySound(TagManager.RED_TAG);
                _isRedOpen = true;
                GamePlayController.Instance.OpenGate(TagManager.RED_TAG); 
            }
            if (triggerTag.Equals(TagManager.BLUE_TAG) && KeyCollector.Instance.IsKeyCollected(TagManager.BLUE_TAG))
            {
                blueKeyObj.transform.position = KeyCollector.Instance.blueKey.transform.position;
                blueKeyObj.transform.localScale = new Vector3(2,2,2);
                blueKeyObj.SetActive(true);
                sequence.Append(blueKeyObj.transform.DOLocalMoveX(0, 0.75f).SetEase(Ease.OutBack));
                sequence.Join(blueKeyObj.transform.DOLocalMoveY(0,0.75f).SetEase(Ease.Linear));
                sequence.Join(blueKeyObj.transform.DOScale(1,0.75f).SetEase(Ease.Linear));
                PlayKeySound(TagManager.BLUE_TAG);
                _isBlueOpen = true;
                GamePlayController.Instance.OpenGate(TagManager.BLUE_TAG); 
            }
            triggerObject.GetComponent<CharacterController>().PlayCallAnim();
            if (_isRedOpen && _isBlueOpen)
            {
                simpleAudio.Play(open);
                OpenGate();
            }
            sequence.Play();
        }
        protected override void TriggerExit(string triggerTag, Transform triggerObject)
        {
            if (triggerTag.Equals(TagManager.RED_TAG))
            {
                redKeyObj.SetActive(false);
                _isRedOpen = false;
                GamePlayController.Instance.CloseGate(TagManager.RED_TAG);
            }
            if (triggerTag.Equals(TagManager.BLUE_TAG))
            {
                blueKeyObj.SetActive(false);
                _isBlueOpen = false;
                GamePlayController.Instance.CloseGate(TagManager.BLUE_TAG);
            }
            triggerObject.GetComponent<CharacterController>().PlayIdle();
        }
        private void PlayKeySound(string characterTag)
        {
            if (characterTag.Equals(TagManager.BLUE_TAG) && !_isPlayBlueKey)
            {
                simpleAudio.Play(setKey);
                _isPlayBlueKey = true;
            }
            if (characterTag.Equals(TagManager.RED_TAG) && !_isPlayRedKey)
            {
                simpleAudio.Play(setKey);
                _isPlayRedKey = true;
            }
           
        }
        private void OnValueChange()
        {
            skeletonRed.initialSkinName = trySkinName + "_1";
            skeletonBlue.initialSkinName = trySkinName + "_2";
            skeletonRed.Initialize(true);
            skeletonBlue.Initialize(true);
        }
        private List<string> GetAllSkinName()
        {
            var result = new List<string>();
            var totalSkin = skeletonRed.SkeletonDataAsset.GetSkeletonData(true).Skins.Items;
            foreach (var skin in totalSkin)
            {
                result.Add(skin.Name.Split('_')[0]);
            }

            result.Remove("default");
            return result;
        }

        private void OpenGate(Action onStart = null, Action onComplete = null)
        {
            gate.DOLocalMoveY(openY, 1f)
                .SetEase(Ease.Linear)
                .OnStart(() => { onStart?.Invoke();})
                .OnComplete(() => { onComplete?.Invoke();});
        }
    }
}
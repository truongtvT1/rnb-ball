using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace GamePlay
{
 
    [RequireComponent(typeof(Collider2D))]
    public class ObjectTrigger : MonoBehaviour
    {
        [SerializeField,ValueDropdown(nameof(GetAllTriggerTag))] protected List<string> collisionTags;
        protected Collider2D Collider2D;
        [SerializeField] private bool useDirectly;

        [SerializeField, ShowIf(nameof(useDirectly))]
        private UnityEvent<string,Transform> OnEnter,OnStay,OnExit;
        private void Awake()
        {
            Collider2D = GetComponent<Collider2D>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collisionTags.Contains(collision.tag)) {
                TriggerEnter(collision.tag,collision.transform);
                if(useDirectly)
                    OnEnter.Invoke(collision.tag,collision.transform);
            }
        }
        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collisionTags.Contains(collision.tag)) {
                TriggerStay(collision.tag,collision.transform);
                if(useDirectly)
                    OnStay.Invoke(collision.tag,collision.transform);
            }
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collisionTags.Contains(collision.tag)) {
                TriggerExit(collision.tag,collision.transform);
                if(useDirectly)
                    OnExit.Invoke(collision.tag,collision.transform);
            }
        }

        protected virtual void TriggerEnter(string triggerTag,Transform triggerObject) { 
        }
        protected virtual void TriggerExit(string triggerTag,Transform triggerObject) { 
        }

        protected virtual void TriggerStay(string triggerTag,Transform triggerObject)
        {
            
        }
        protected string[] GetAllTriggerTag()
        {
            return TagManager.GetAllTagHandle();
        }
        
        
    }
}

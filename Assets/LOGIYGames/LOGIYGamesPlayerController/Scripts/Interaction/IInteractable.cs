using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace LOGIYGames
{
    public interface IInteractable
    {
        InteractionViewData GetViewData();
        IReadOnlyList<InteractionAction> GetActions(InteractionContext context);

        void OnFocusGained();

        void OnFocusLost();
    }
    public abstract class InteractableObject : MonoBehaviour, IInteractable
    {
        [SerializeField] InteractionViewData Data;
        public UnityEvent OnFocusGainedEvent = new UnityEvent();
        public UnityEvent OnFocusLostEvent = new UnityEvent();
        protected List<InteractionAction> Actions = new();
        virtual protected void Start()
        {
           
        }
        [SerializeField]
        protected bool isActive = true;


        public virtual IReadOnlyList<InteractionAction> GetActions(InteractionContext context)
        {
            return Actions
                .Where(action =>
                    action != null &&
                    action.CanExecute(context))
                .OrderByDescending(action => action.Priority)
                .Reverse()
                .ToList();
        }

        public InteractionViewData GetViewData()
        {
            return Data;
        }

        public virtual void OnFocusGained()
        {
            OnFocusGainedEvent.Invoke();
        }

        public virtual void OnFocusLost()
        {
            OnFocusLostEvent.Invoke();
        }
    }
    public class InteractionAction
    {
        public InteractionViewData ViewData;

        public string Id;

        public int Priority;

        public Func<InteractionController,bool> IsAvailable;

        public Action<InteractionContext> Execute;

        public InteractionAction(InteractionViewData viewData, string id, int priority, Func<InteractionController, bool> isAvailable, Action<InteractionContext> execute)
        {
            ViewData = viewData;
            Id = id;
            Priority = priority;
            IsAvailable = isAvailable;
            Execute = execute;
        }

        public bool CanExecute(InteractionContext context)
        {
            return IsAvailable == null || IsAvailable(context.Interactor);
        }
    }

    [Serializable]
    public struct InteractionContext
    {
        public InteractionController Interactor;
    }

    [Serializable]
    public struct InteractionViewData
    {
        public Sprite Icon;

        public string Text;

    }

}
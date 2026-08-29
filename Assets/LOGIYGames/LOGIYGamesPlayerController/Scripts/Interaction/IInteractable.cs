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
        IReadOnlyList<InteractionAction> GetActions(
            InteractionContext context);

        bool CanInteract(
            InteractionContext interactionContext);

        void OnFocusGained();

        void OnFocusLost();
    }
    public abstract class InteractableObject : MonoBehaviour, IInteractable
    {
        [SerializeField] InteractionViewData Data;
        public UnityEvent OnFocusGainedEvent = new UnityEvent();
        public UnityEvent OnFocusLostEvent = new UnityEvent();
        [SerializeField]
        protected List<InteractionAction> Actions = new();
        virtual protected void Start()
        {
            Actions.Add(new InteractionAction() { Priority = int.MaxValue, ViewData = new InteractionViewData { Text = "Close" } });

        }
        [SerializeField]
        protected bool isActive = true;

        public virtual bool CanInteract(
            InteractionContext interactionContext)
        {
            if (!isActive)
                return false;

            return GetActions(interactionContext).Count > 0;
        }

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

    [Serializable]
    public class InteractionAction
    {
        public InteractionViewData ViewData;

        public string Id;

        public int Priority;

        [SerializeReference]
        public List<IInteractionActionCondition> Conditions = new();

        public UnityEvent<InteractionContext> Subscribes = new();

        public bool CanExecute(InteractionContext context)
        {
            if (Conditions == null || Conditions.Count == 0)
                return true;

            return Conditions.All(condition =>
                condition != null &&
                condition.Evaluate(context));
        }

        public void Execute(InteractionContext context)
        {
            Subscribes?.Invoke(context);
        }
    }

    [Serializable]
    public struct InteractionContext
    {
        public Interactor Interactor;
    }

    [Serializable]
    public struct InteractionViewData
    {
        public Sprite Icon;

        public string Text;

        public Transform Anchor;
    }

    public interface IInteractionActionCondition
    {
        bool Evaluate(InteractionContext actionContext);
    }

    [Serializable]
    public class HasItemCondition : IInteractionActionCondition
    {
        [SerializeField]
        private float ItemId;

        public bool Evaluate(InteractionContext actionContext)
        {
            // TODO: Проверка наличия предмета.
            return true;
        }
    }

    [Serializable]
    public class OpenDoorCondition : IInteractionActionCondition
    {
        public Door door;

        public bool Evaluate(InteractionContext actionContext)
        {
            if (door == null)
                return false;

            return !door.IsOpened && !door.IsLocked;
        }
    }
    [Serializable]
    public class CloseDoorCondition : IInteractionActionCondition
    {
        public Door door;

        public bool Evaluate(InteractionContext actionContext)
        {
            if (door == null)
                return false;

            return door.IsOpened;
        }
    }
    [Serializable]
    public class LockDoorCondition : IInteractionActionCondition
    {
        public Door door;

        public bool Evaluate(InteractionContext actionContext)
        {
            if (door == null)
                return false;

            return !door.IsOpened && !door.IsLocked;
        }
    }
    [Serializable]
    public class UnlockDoorCondition : IInteractionActionCondition
    {
        public Door door;

        public bool Evaluate(InteractionContext actionContext)
        {
            if (door == null)
                return false;

            return !door.IsOpened && door.IsLocked;
        }
    }
}
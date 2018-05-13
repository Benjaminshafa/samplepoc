using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace pocSolution
{
    public class Subscription<Tmessage> : IDisposable
    {
        public readonly MethodInfo MethodInfo;
        private readonly EventHub _eventAggregator;
        public readonly WeakReference TargetObjet;
        public readonly bool IsStatic;

        private bool _isDisposed;
        public Subscription(Action<Tmessage> action, EventHub eventAggregator)
        {
            MethodInfo = action.Method;
            if (action.Target == null)
                IsStatic = true;
            TargetObjet = new WeakReference(action.Target);
            _eventAggregator = eventAggregator;
        }

        Subscription()
        {
            if (!_isDisposed)
                Dispose();
        }

        public void Dispose()
        {
            _eventAggregator.UnSbscribe(this);
            _isDisposed = true;
        }

        public Action<Tmessage> CreatAction()
        {
            if (TargetObjet.Target != null && TargetObjet.IsAlive)
                return (Action<Tmessage>)Delegate.CreateDelegate(typeof(Action<Tmessage>), TargetObjet.Target, MethodInfo);
            if (this.IsStatic)
                return (Action<Tmessage>)Delegate.CreateDelegate(typeof(Action<Tmessage>), MethodInfo);

            return null;
        }
    }

    public class EventHub
    {
        private readonly object _lockedObject = new object();
        private readonly Dictionary<Type, IList> subscriber;

        public EventHub()
        {
            subscriber = new Dictionary<Type, IList>();
        }

        public void Publish<TMessageType>(TMessageType message)
        {
            Type type = typeof(TMessageType);
            lock (_lockedObject)
            {
                if (subscriber.ContainsKey(type))
                {
                    IList subList;
                    lock (_lockedObject)
                    {
                        subList = new List<Subscription<TMessageType>>(subscriber[type].Cast<Subscription<TMessageType>>());
                    }

                    foreach (Subscription<TMessageType> sub in subList)
                    {
                        var action = sub.CreatAction();
                        action?.Invoke(message);
                    }
                }
            }
        }

        public Subscription<TMessageType> Subscribe<TMessageType>(Action<TMessageType> action)
        {
            Type t = typeof(TMessageType);
            var actiondetail = new Subscription<TMessageType>(action, this);

            lock (_lockedObject)
            {
                IList actionlst;
                if (!subscriber.TryGetValue(t, out actionlst))
                {
                    actionlst = new List<Subscription<TMessageType>> {actiondetail};
                    subscriber.Add(t, actionlst);
                }
                else
                {
                    actionlst.Add(actiondetail);
                }
            }

            return actiondetail;
        }

        public void UnSbscribe<TMessageType>(Subscription<TMessageType> subscription)
        {
            Type t = typeof(TMessageType);
            if (subscriber != null && subscriber.ContainsKey(t))
            {
                lock (_lockedObject)
                {
                    subscriber[t].Remove(subscription);
                }
            }
        }

    }
}

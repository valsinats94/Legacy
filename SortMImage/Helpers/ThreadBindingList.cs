using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;

namespace SortMImage.Helpers
{
    /// <summary>
    /// List that is updating form another thread
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ThreadBindingList<T> : BindingList<T>
    {
        SynchronizationContext ctx = SynchronizationContext.Current;

        protected override void OnAddingNew(AddingNewEventArgs e)
        {

            if (ctx == null)
            {
                BaseAddingNew(e);
            }
            else
            {
                ctx.Send(delegate
                {
                    BaseAddingNew(e);
                }, null);
            }
        }

        void BaseAddingNew(AddingNewEventArgs e)
        {
            base.OnAddingNew(e);
        }

        protected override void OnListChanged(ListChangedEventArgs e)
        {
            if (ctx == null)
            {
                BaseListChanged(e);
            }
            else
            {
                ctx.Send(delegate
                {
                    BaseListChanged(e);
                }, null);
            }
        }

        void BaseListChanged(ListChangedEventArgs e)
        {
            base.OnListChanged(e);
        }

        public void AddRange(Collection<T> collection)
        {
            foreach (T item in collection)
            {
                base.Add(item);
            }
        }
    }
}

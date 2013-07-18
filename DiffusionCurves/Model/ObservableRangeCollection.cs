using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace DiffusionCurves.Model
{
    /// <summary> 
    /// Represents a dynamic data collection that provides notifications when items get added, removed, or when the whole list is refreshed. 
    /// </summary> 
    /// <typeparam name="T"></typeparam> 
    [Serializable]
    public class ObservableRangeCollection<T> : ObservableCollection<T>
    {
        /// <summary>
        /// Class implements the ObservableCollection for later use.
        /// </summary>
        [Serializable]
        private sealed class SObservableCollection : ObservableCollection<string>
        {
            #region Private fields

            [field: NonSerialized]
            public override event NotifyCollectionChangedEventHandler CollectionChanged;

            [field: NonSerialized]
            protected override event PropertyChangedEventHandler PropertyChanged;

            #endregion

            /// <summary>
            /// Handler for when the collection has been changed.
            /// </summary>
            /// <param name="e"></param>
            protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
            {
                NotifyCollectionChangedEventHandler handler = CollectionChanged;

                if (handler != null)
                {
                    handler(this, e);
                }
            }

            /// <summary>
            /// Handler for when the property has been changed.
            /// </summary>
            /// <param name="e"></param>
            protected override void OnPropertyChanged(PropertyChangedEventArgs e)
            {
                PropertyChangedEventHandler handler = PropertyChanged;

                if (handler != null)
                {
                    handler(this, e);
                }
            }
        }

        /// <summary> 
        /// Adds the elements of the specified collection to the end of the ObservableCollection(Of T). 
        /// </summary> 
        public void AddRange(IEnumerable<T> collection)
        {
            if (collection == null) throw new ArgumentNullException("collection");

            int oldSize = Items.Count;

            foreach (var i in collection) Items.Add(i);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, collection.ToList(), oldSize - 1));
        }

        /// <summary> 
        /// Removes the first occurence of each item in the specified collection from ObservableCollection(Of T). 
        /// </summary> 
        public void RemoveRange(IEnumerable<T> collection)
        {
            if (collection == null) throw new ArgumentNullException("collection");

            foreach (var i in collection) Items.Remove(i);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, collection.ToList()));
        }

        /// <summary> 
        /// Clears the current collection and replaces it with the specified item. 
        /// </summary> 
        public void Replace(T item)
        {
            ReplaceRange(new T[] { item });
        }

        /// <summary> 
        /// Clears the current collection and replaces it with the specified collection. 
        /// </summary> 
        public void ReplaceRange(IEnumerable<T> collection)
        {
            if (collection == null) throw new ArgumentNullException("collection");

            Items.Clear();
            foreach (var i in collection) Items.Add(i);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        /// <summary> 
        /// Initializes a new instance of the System.Collections.ObjectModel.ObservableCollection(Of T) class. 
        /// </summary> 
        public ObservableRangeCollection()
            : base() { }

        /// <summary> 
        /// Initializes a new instance of the System.Collections.ObjectModel.ObservableCollection(Of T) class that contains elements copied from the specified collection. 
        /// </summary> 
        /// <param name="collection">collection: The collection from which the elements are copied.</param> 
        /// <exception cref="System.ArgumentNullException">The collection parameter cannot be null.</exception> 
        public ObservableRangeCollection(IEnumerable<T> collection)
            : base(collection) { }

    }
}

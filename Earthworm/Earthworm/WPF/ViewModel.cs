using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using ESRI.ArcGIS.Geodatabase;
using Earthworm.AO;

namespace Earthworm.WPF
{
    /// <summary>
    /// Represents a basic view model for a table that can be bound to WPF data grids.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ViewModel<T> : INotifyPropertyChanged where T : MappableFeature, new()
    {
        private readonly ITable _table;
        private readonly bool _editInSession;
        private Func<ITable, IEnumerable<T>> _mapping;

        /// <summary>
        /// The function that maps a table to a collection of typed objects.  If this is set to null, the default mapping is applied with no filtering.
        /// </summary>
        public Func<ITable, IEnumerable<T>> Mapping
        {
            get { return _mapping; }
            set { _mapping = value ?? (t => t.Map<T>()); }
        }

        /// <summary>
        /// Represents the method that will handle the PropertyChanged event raised when a property is changed on a component.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Called from a property setter to notify the framework that a member has changed.
        /// </summary>
        /// <param name="propertyName"></param>
        protected void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Collection of mappable features of the specified type.
        /// </summary>
        public ObservableCollection<T> Items { get; private set; }

        /// <summary>
        /// Initializes a new instance of the ViewModel class.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="mapping">A function that maps a table to a collection of typed objects.  If set to null (default), all records will be returned without any filtering.</param>
        /// <param name="editInSession">If set to true (default), any editing action will be executed within its own workspace edit session.</param>
        public ViewModel(ITable table, Func<ITable, IEnumerable<T>> mapping, bool editInSession)
        {
            _table = table;
            _editInSession = editInSession;

            Mapping = mapping;

            Items = new ObservableCollection<T>(Mapping(_table));
        }

        /// <summary>
        /// Initializes a new instance of the ViewModel class.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="mapping">A function that maps a table to a collection of typed objects.  If set to null (default), all records will be returned without any filtering.</param>
        public ViewModel(ITable table, Func<ITable, IEnumerable<T>> mapping)
            : this(table, mapping, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ViewModel class.
        /// </summary>
        /// <param name="table"></param>
        public ViewModel(ITable table)
            : this(table, null, true)
        {
        }

        #region Private

        private void Edit(Action action)
        {
            if (!_editInSession)
            {
                action();
                return;
            }

            Exception ex;
            ((IDataset)_table).Workspace.Edit(action, out ex);

            if (ex != null)
                throw ex;
        }

        #endregion

        #region SaveAll

        /// <summary>
        /// Updates all "dirty" items in the underlying database table.  Items that are not bound to the table will be inserted as new rows.
        /// </summary>
        public virtual void SaveAll()
        {
            Edit(() =>
            {
                for (int i = 0; i < Items.Count; i++) //Do not convert this to a foreach loop.
                    Save(Items[i]);
            });
        }

        /// <summary>
        /// Indicates that changes to one or more items have not been committed to the underlying database table.
        /// </summary>
        /// <returns></returns>
        public virtual bool CanSaveAll()
        {
            return Items.Any(CanSave);
        }

        /// <summary>
        /// Represents a command that updates all "dirty" items in the underlying database table.  Items that are not bound to the table will be inserted.
        /// </summary>
        public ICommand SaveAllCommand
        {
            get { return new RelayCommand(SaveAll, CanSaveAll); }
        }

        #endregion

        #region Save

        /// <summary>
        /// Updates the item (only if dirty) in the underlying database table.  If the item is not bound to the table, it will be inserted as a new row.
        /// </summary>
        /// <param name="item"></param>
        public virtual void Save(T item)
        {
            if (!item.IsDataBound)
            {
                Edit(() =>
                {
                    T item2 = item.InsertInto(_table);
                    int index = Items.IndexOf(item);
                    Items.RemoveAt(index);
                    Items.Insert(index, item2);
                });
            }
            else if (item.IsDirty)
            {
                Edit(item.Update);
            }
        }

        /// <summary>
        /// Indicates that changes to this item have not been committed to the underlying database table.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual bool CanSave(T item)
        {
            if (item == null)
                return false;

            return !item.IsDataBound || item.IsDirty;
        }

        /// <summary>
        /// Represents a command that updates the item (only if dirty) in the underlying database table.  If the item is not bound to the table, it will be inserted as a new row.
        /// </summary>
        public ICommand SaveCommand
        {
            get { return new RelayCommand(o => Save((T)o), o => CanSave(o as T)); }
        }

        #endregion

        #region Create

        /// <summary>
        /// Adds a new item into the collection.  The new item is not bound to the underlying database table until it is saved.
        /// </summary>
        public virtual void Create()
        {
            Items.Add(NotificationProxy.Create<T>());
        }

        /// <summary>
        /// Indicates that a new item can be added to the collection.  This always returns true unless overridden to behave differently.
        /// </summary>
        /// <returns></returns>
        public virtual bool CanCreate()
        {
            return true;
        }

        /// <summary>
        /// Represents a command that adds a new item into the collection.  The new item is not bound to the underlying database table until it is saved.
        /// </summary>
        public ICommand CreateCommand
        {
            get { return new RelayCommand(Create, CanCreate); }
        }

        #endregion

        #region Delete

        /// <summary>
        /// Removes an item from the collection.  If the item is data bound, it will be deleted from the underlying database table.
        /// </summary>
        /// <param name="item"></param>
        public virtual void Delete(T item)
        {
            if (item.IsDataBound)
                Edit(item.Delete);

            Items.Remove(item);
        }

        /// <summary>
        /// Indicates if the item can be removed from the collection.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual bool CanDelete(T item)
        {
            return item != null;
        }

        /// <summary>
        /// Represents a command that removes an item from the collection.  If the item is data bound, it will be deleted from the underlying database table.
        /// </summary>
        public ICommand DeleteCommand
        {
            get { return new RelayCommand(o => Delete((T)o), o => CanDelete(o as T)); }
        }

        #endregion

        #region Requery

        /// <summary>
        /// Clears all items and re-populates the collection querying the underlying database table with any filters specified by the Mapping function.
        /// </summary>
        public virtual void Requery()
        {
            Items.Clear();

            foreach (T item in Mapping(_table))
                Items.Add(item);
        }

        /// <summary>
        /// Indicates that the items can be re-queried.  This always returns true unless overridden to behave differently.
        /// </summary>
        /// <returns></returns>
        public virtual bool CanRequery()
        {
            return true;
        }

        /// <summary>
        /// Represents a command that clears all items and re-populates the collection querying the underlying database table with any filters specified by the Mapping function.
        /// </summary>
        public ICommand RequeryCommand
        {
            get { return new RelayCommand(Requery, CanRequery); }
        }

        #endregion
    }
}

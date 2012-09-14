using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace IMAPI2
{
    public class ReadOnlySelectableCollection<T> : ReadOnlyCollection<T>
    {
        private int _selectedIndex;

        public event EventHandler SelectedIndexChanged;

        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                if ((_selectedIndex > (this.Count-1)) || (_selectedIndex < -1))
                    throw new IndexOutOfRangeException("SelectedIndex must be in range (this.Count = " + this.Count.ToString() + ").");
                _selectedIndex = value;
                SelectedIndexChanged(this, new EventArgs());
            }
        }
        public T SelectedItem
        {
            get
            {
                if (_selectedIndex == -1)
                {
                    return default(T);
                }
                else
                {
                    return this[_selectedIndex];
                }
            }
            set
            {
                _selectedIndex = this.IndexOf(value);                
            }
        }        

        public ReadOnlySelectableCollection(IList<T> list) : base(list)
        {
            _selectedIndex = -1;
        }
    }
}

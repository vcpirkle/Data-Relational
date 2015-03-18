using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace DataRelational.WpfTestApp.Helpers
{
    public class CalculatedField<TResultType, TParentType> :INotifyPropertyChanged where TParentType : DataObject 
    {
        private TParentType _Parent;
        private string _Name;
        private string[] _ParticipatingProperties;
        private Func<TParentType, TResultType> _Calculation;

        public event PropertyChangedEventHandler PropertyChanged;

        public CalculatedField(TParentType Parent, string Name, string[] ParticpatingProperties, Func<TParentType, TResultType> Calculation)
        {
            _Parent = Parent;
            _Name = Name;
            _ParticipatingProperties = ParticpatingProperties;
            _Calculation = Calculation;

            _Parent.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e) {
                if (_ParticipatingProperties.Contains(e.PropertyName))
                {
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("Value")); 
                    }
                }
            };
        }

        public TResultType Value
        {
            get { return _Calculation(_Parent); }
        }
    }
}

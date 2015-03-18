using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataRelational;
using  DataRelational.WpfTestApp.Helpers;
using DataRelational.Attributes;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace DataRelational.WpfTestApp.Models
{
    [Serializable, DataContract]
    [SavableObject(SaveBehavior=SaveBehavior.Dirty, CacheBehavior=CacheBehavior.Cahce)]
    public class Person : HistoricalCachableDataObject
    {
        #region Fields
        private Salutation _Salutation;
        private string _FirstName;
        private string _LastName;
        private string _MiddleName;
        private CalculatedField<string, Person> _FullName;
        private DataRelationship<List<Person>> _Parents;
        private DataRelationship<List<Person>> _Children;
        #endregion Fields

        #region Properties
        [DataMember, DataField]
        public Salutation Salutation
        {
            get { return _Salutation; }
            set
            {
                if (_Salutation != value)
                {
                    _Salutation = value;
                    RaisePropertyChanged("Salutation");
                }
            }
        }

        [DataMember]
        [DataField(FieldSize = 50)]
        public string FirstName
        {
            get { return _FirstName; }
            set
            {
                if (_FirstName != value)
                {
                    _FirstName = value;
                    RaisePropertyChanged("FirstName");
                }
            }
        }

        [DataMember]
        [DataField(FieldSize = 50)]
        public string MiddleName
        {
            get { return _MiddleName; }
            set
            {
                if (_MiddleName != value)
                {
                    _MiddleName = value;
                    RaisePropertyChanged("MiddleName");
                }
            }
        }

        [DataMember]
        [DataField(FieldSize = 50)]
        public string LastName
        {
            get { return _LastName; }
            set
            {
                if (_LastName != value)
                {
                    _LastName = value;
                    RaisePropertyChanged("LastName");
                }
            }
        }

        public CalculatedField<string, Person> FullName
        {
            get 
            {
                return _FullName; 
            }
        }

        public DataRelationship<List<Person>> Parents
        {
            get { return _Parents; }
            set
            {
                _Parents = value;
                RaisePropertyChanged("Parents");
            }
        }

        public DataRelationship<List<Person>> Children
        {
            get { return _Children; }
            set
            {    
                _Children = value;
                RaisePropertyChanged("Children");                
            }
        }
        #endregion Properties
        
        #region Initialization
        public override void OnInitializing(bool LoadedByBuilder)
        {
            base.OnInitializing(LoadedByBuilder);

            if (!LoadedByBuilder)
            {
                this.Parents = new List<Person>();
                this.Children = new List<Person>();
            }

            _FullName = new CalculatedField<string, Person>(this, "FullName", new string[] { "FirstName", "MiddleName", "LastName" },
                  p => p.FirstName + " " + p.MiddleName + " " + p.LastName);

            _FullName.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e) { this.RaisePropertyChanged("FullName"); };
        }
        #endregion Constructor
    }

    /// <summary>
    /// The greeting to append to the front of a name
    /// </summary>
    public enum Salutation : byte
    {
        /// <summary>
        /// No salutation
        /// </summary>
        None = 0,
        /// <summary>
        /// Mr.
        /// </summary>
        Mr = 1,
        /// <summary>
        /// Ms.
        /// </summary>
        Ms = 2,
        /// <summary>
        /// Mrs.
        /// </summary>
        Mrs = 3,
    }
}

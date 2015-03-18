using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataRelational.WpfTestApp.Models;
using System.Collections.ObjectModel;
using MvvmFoundation.Wpf;
using DataRelational.WpfTestApp.Helpers;
using System.Windows.Input;

namespace DataRelational.WpfTestApp.ViewModels
{
    public class PeopleViewModel : Base 
    {
        private ObservableCollection<Person> _People;
        private List<Salutation> _Salutations;
        private Person _Selected;
        private RelayCommand _AddPersonCommand;
        private RelayCommand _SaveCommand;

        public PeopleViewModel()
        {            
            _People = new ObservableCollection<Person>();
            _People.AddRange(DataManager.GetObjects<Person>());

            _AddPersonCommand = new RelayCommand(() => {
                var p = new Person();
                _People.Add(p);
                Selected = p;
            });

            _SaveCommand = new RelayCommand(
                () => { DataManager.Save(_People.OfType<IDataObject>()); },
                () => _People.Where(p => p._Dirty).Count() > 0);
           
            _Salutations = new List<Salutation>();
            _Salutations.AddRange(Enum.GetValues(typeof(Salutation)).OfType<Salutation>());
            
            if (IsDesignMode)
            {
                _People.Add(new Person() { FirstName = "Brandon", LastName = "Smith", Salutation=Salutation.Mr});
                _People.Add(new Person() { FirstName = "Misty", LastName = "Smith", Salutation = Salutation.Ms});
                _People.Add(new Person() { FirstName = "Victor", LastName = "Pirkle", Salutation = Salutation.Mr});
                _Selected = _People[0];
            }
        }

        public List<Salutation> Salutations
        {
            get { return _Salutations; }
        }

        public ObservableCollection<Person> People
        {
            get { return _People; }
        }

        public Person Selected
        {
            get { return _Selected; }
            set
            {
                _Selected = value;
                RaisePropertyChanged("Selected");
            }
        }

        public ICommand AddPersonCommand
        {
            get 
            {
                return _AddPersonCommand; 
            }
        }

        public ICommand SaveCommand
        {
            get
            {
                return _SaveCommand;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WritersToolbox.models;
namespace WritersToolbox.viewmodels
{
    /// <summary>
    /// Die SearchViewModel Klasse bzw. Präsentations-Logik ist eine aggregierte Datenquelle,
    /// die verschiedene Daten aus allen suchrelevanten Entities und ihre entsprechenden Eigenschaften bereitstellt.
    /// </summary>
    class SearchViewModel : INotifyPropertyChanged
    {
        private WritersToolboxDatebase wtb = null;
        /// <summary>
        /// Enthält alle Suchtreffer, nachdem eine Suche ausgeführt wurde.
        /// </summary>
        public ObservableCollection<ISearchable> ResultList { get; set; }
        /// <summary>
        /// Anzahl der Suchtreffer.
        /// </summary>
        public int ResultCount { get; set; }

        /// <summary>
        /// Erzeugt eine neue Instanz der Präsentationslogik. Diese Klasse muss im Normalfall allerdings nur einmal
        /// initialisiert werden.
        /// </summary>
        public SearchViewModel() {
            wtb = WritersToolboxDatebase.getInstance();
            ResultList = new ObservableCollection<ISearchable>();
            ResultCount = 0;
        }

        /// <summary>
        /// Läd alle Treffer auf die angegebene Abfrage.
        /// </summary>
        /// <param name="query">Abfragestring</param>
        public void loadByQuery(String query) {
            ResultList = new ObservableCollection<ISearchable>();
            var sqlNotes = from n in this.wtb.GetTable<MemoryNote>()
                          select n;
            foreach (var n in sqlNotes) {
                if (n.matchesQuery(query)) { ResultList.Add(n); }   
            }
            var sqlTypes = from t in this.wtb.GetTable<models.Type>()
                                 select t;
            foreach (var t in sqlTypes)
            {
                if (t.matchesQuery(query)) { ResultList.Add(t); }
            }
            var sqlTypeObjects = from to in this.wtb.GetTable<TypeObject>()
                                 select to;
            foreach (var to in sqlTypeObjects) {
                if (to.matchesQuery(query)) { ResultList.Add(to); }
            }
            var sqlBooks = from b in this.wtb.GetTable<Book>()
                           select b;
            foreach (var b in sqlBooks) {
                if (b.matchesQuery(query)) { ResultList.Add(b); }
            }
            var sqlTomes = from t in this.wtb.GetTable<Tome>()
                           select t;
            foreach (var t in sqlTomes) {
                if (t.matchesQuery(query)) { ResultList.Add(t); }
            }
            var sqlChapters = from c in this.wtb.GetTable<Chapter>()
                           select c;
            foreach (var c in sqlChapters)
            {
                if (c.matchesQuery(query)) { ResultList.Add(c); }
            }
            var sqlEvents = from e in this.wtb.GetTable<Event>()
                           select e;
            foreach (var e in sqlEvents)
            {
                if (e.matchesQuery(query)) { ResultList.Add(e); }
            }
            this.NotifyPropertyChanged("ResultList");
            ResultCount = ResultList.Count;
            this.NotifyPropertyChanged("ResultCount");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        // Used to notify the app that a property has changed.
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}

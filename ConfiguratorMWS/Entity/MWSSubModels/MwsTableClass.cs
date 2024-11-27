

using System.Collections.ObjectModel;

namespace ConfiguratorMWS.Entity.MWSSubModels
{
    public class MwsTableClass
    {
        public ObservableCollection<MwsRowClass> Rows { get; set; }

        public MwsTableClass() {
            Rows = new ObservableCollection<MwsRowClass>();
        }

        public MwsTableClass Clone()
        {
            var clonedTable = new MwsTableClass();
            foreach (var row in Rows)
            {
                // Клонируем каждую строку
                clonedTable.Rows.Add(row.Clone());
            }
            return clonedTable;
        }

    }
}

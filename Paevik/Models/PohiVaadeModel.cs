using SQLite;
using System;

namespace Paevik.Models
{
    public class PohiVaadeModel
    {
        //Model millega andmebaasi ja selle läbi listviewi elemente sisestada
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string KPItem { get; set; }
        public string ToitItem { get; set; }
        public int KcalItem { get; set; }

    }
}

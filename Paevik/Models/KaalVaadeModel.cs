using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Paevik.Models
{
    public class KaalVaadeModel
    {
        //Model millega andmebaasi ja selle läbi listviewi elemente sisestada
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string KaalKPItem { get; set; }
        public float KaalItem { get; set; }
    }
}
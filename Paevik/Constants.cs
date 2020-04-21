using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Paevik
{
    public static class Constants
    {
        public const string andmebaasiNimi = "AndmebaasSQLite.db3";

        public const SQLite.SQLiteOpenFlags Flags =
            // open the database in read/write mode
            SQLite.SQLiteOpenFlags.ReadWrite |
            // create the database if it doesn't exist
            SQLite.SQLiteOpenFlags.Create |
            // enable multi-threaded database access
            SQLite.SQLiteOpenFlags.SharedCache;

        //Annab meile andmebaasi asukoha
        public static string andmebaasiAsukoht
        {
            get
            {
                var basePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                return Path.Combine(basePath, andmebaasiNimi);
            }
        }
    }
}

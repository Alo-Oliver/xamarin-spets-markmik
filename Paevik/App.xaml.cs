using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

//progeVers = "1.0.0";
//Autor = "Alo-Oliver Alas";
//progeNimi = "Xamarin spets märkmik";

namespace Paevik
{
    public partial class App : Application
    {
        static Andmebaas database;
        public App()
        {
            InitializeComponent();
            
            MainPage = new MainPage();
        }

        public static Andmebaas Database
        {
            get
            {
                if (database == null)
                {
                    database = new Andmebaas();
                }
                return database;
            }
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}

using Paevik.Views;
using SQLite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Paevik
{
    public partial class MainPage : ContentPage
    {
        private object praeguValitud = null;
        public static ObservableCollection<Models.PohiVaadeModel> PohiVaadeElemendid { get; set; }
        public static ObservableCollection<Models.PohiVaadeModel> Sorteerimine;
        public static int valitudID;
        public static string valitudKP;
        public static string valitudToit;
        public static int valitudKcal;
        public static int valitudIndex;
        public static Color taust;

        public MainPage()
        {
            InitializeComponent();
            PohiVaadeElemendid = new ObservableCollection<Models.PohiVaadeModel> { };

            TaidaList();        

            pohiList.ItemsSource = PohiVaadeElemendid;
        }

        //võtame andmebaasist itemid, arvutame päeva kokkuvõtted ja lisame kõik ObservableCollectionisse
        async void TaidaList()
        {

            if (await App.Database.GetItemsAsync() != null)
            {
                var items = await App.Database.GetItemsAsync();
                foreach (var item in items)
                {
                        PohiVaadeElemendid.Add(item);
                }
                
                Tootlemine();
            }
        }

        //Kuupäeva alusel nimekirja järjestamine, kokkuvõtete arvutamine ja lisamine
        public static void Tootlemine()
        {
            string kpv = "";
            string pealkiri = "KOKKU:";
            int kokku = 0;
            int ajID = 90000000;
            int ajIndex = 0;
            

            //Kuupäeva alusel nimekirja järjestamine
            Sorteerimine = new ObservableCollection<Models.PohiVaadeModel>(PohiVaadeElemendid.OrderByDescending(x => x.KPItem));

            //eemaldame kokkuvõtted
            foreach(var item in Sorteerimine.ToList())
            {
                if(item.Id >= 90000000)
                {
                    //Võtame praegu valitud itemi indexi(uue itemi valimiseks)
                    int selectedIndex = Sorteerimine.IndexOf(item);
                    //eemaldame ObservableCollectionist elemendi
                    Sorteerimine.Remove(item);
                }
            }

            //Arvutame uued kokkuvõtted
            PohiVaadeElemendid.Clear();
            foreach (var item in Sorteerimine.ToList())
            {
                if(kpv != item.KPItem)
                {
                    kpv = item.KPItem;
                    kokku = item.KcalItem;
                    ajID++;

                    var andmeItem = new Models.PohiVaadeModel { Id = ajID, KPItem = kpv, ToitItem = pealkiri, KcalItem = kokku };

                    PohiVaadeElemendid.Add(andmeItem);
                    ajIndex = PohiVaadeElemendid.IndexOf(andmeItem);
                }
                else
                {
                    kokku = kokku + item.KcalItem;

                    var andmeItem = new Models.PohiVaadeModel { Id = ajID, KPItem = kpv, ToitItem = pealkiri, KcalItem = kokku };
                    PohiVaadeElemendid[ajIndex] = andmeItem;
                }

                PohiVaadeElemendid.Add(item);
            }
        }



        private async void LisaNupp_Vajutus(object sender, EventArgs e)
        {
            pohiList.SelectedItem = null;
            praeguValitud = null;
            //Muutuja andmesisestuslehe välja kutsumiseks
            var sisestaVaade = new SisestaVaade(false);

            //Andmesisestuslehe väljakutsumine
            await Navigation.PushModalAsync(sisestaVaade);
        }
        
        //Juhul kui item millele vajutad on valitud, deselectib selle.
        private void Listview_Vajutus(object sender, EventArgs e)
        {
            object uusValitud = pohiList.SelectedItem;

            if (uusValitud == praeguValitud)
            {
                yleKirjutamine();
                pohiList.SelectedItem = null;
                praeguValitud = null;
            }
            else praeguValitud = uusValitud;
        }

        //kustutab kirje kui midagi on valitud
        private async void KustutaKirje_Vajutus(object sender, EventArgs e)
        {
            if(pohiList.SelectedItem != null)
            {
                //määrame itemiks listviewis valitud itemi ja kustutame selle andmebaasist
                var andmeItem = (Models.PohiVaadeModel)pohiList.SelectedItem;

                //kokkuvõtet ei kustutata andmebaasist kuna neid ei kanta andmebaasi
                if(andmeItem.Id < 90000000)
                {
                    await App.Database.DeleteItemAsync(andmeItem);
                }

                //Võtame praegu valitud itemi indexi(uue itemi valimiseks)
                int selectedIndex = PohiVaadeElemendid.IndexOf(andmeItem);
                //eemaldame ObservableCollectionist elemendi
                PohiVaadeElemendid.Remove(andmeItem);

                //peale kustutamist muudame indexit ja võtame järgmise itemi fookusesse, et oleks mugavam mass-kustutamist teha
                if (selectedIndex >= PohiVaadeElemendid.Count) selectedIndex--;

                if (selectedIndex >= 0)
                    pohiList.SelectedItem = PohiVaadeElemendid[selectedIndex];
            
            }
        }

        private async void KaalNupp_Vajutus(object sender, EventArgs e)
        {
            pohiList.SelectedItem = null;
            praeguValitud = null;
            //Muutuja andmesisestuslehe välja kutsumiseks
            var kaalVaade = new KaalVaade();

            //Andmesisestuslehe väljakutsumine
            await Navigation.PushModalAsync(kaalVaade);
        }

        private void KirjutaYleNupp_Vajutus(object sender, EventArgs e)
        {
            yleKirjutamine();
        }

        //Õige MVVM puhul peaksid kõik nupule vajutamised ja programmi loogika olema funktsioonid nagu all toodud(teises failis veel), aga ei hakka tervet koodi enam ümber tegema, märkus tulevikuks.
        private async void yleKirjutamine()
        {
            if (pohiList.SelectedItem != null)
            {
                //määrame itemiks listviewis valitud itemi
                var andmeItem = (Models.PohiVaadeModel)pohiList.SelectedItem;
                //Kokkuvõtet ei saa muuta
                if (andmeItem.Id < 90000000)
                {
                    //Võtame praegu valitud itemi indexi(itemi üle kirjutamiseks)
                    valitudIndex = PohiVaadeElemendid.IndexOf(andmeItem);
                    //Lisame muutujatele väärtused mida kasutame järgmises vaates
                    valitudID = andmeItem.Id;
                    valitudKcal = andmeItem.KcalItem;
                    valitudKP = andmeItem.KPItem;
                    valitudToit = andmeItem.ToitItem;

                    //Muutuja andmesisestuslehe välja kutsumiseks
                    var KirjutaYleVaade = new SisestaVaade(true);

                    //Andmesisestuslehe väljakutsumine
                    await Navigation.PushModalAsync(KirjutaYleVaade);

                    pohiList.SelectedItem = null;
                    praeguValitud = null;
                }
            }
        }

        private void pohiList_Focused(object sender, FocusEventArgs e)
        {

        }
    }
}

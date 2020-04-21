using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Paevik.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class KaalVaade : ContentPage
    {
        private object praeguValitud = null;
        public static ObservableCollection<Models.KaalVaadeModel> KaalItems { get; set; }
        public static ObservableCollection<Models.KaalVaadeModel> Sorteerimine;
        public static int KvalitudIndex;
        public static int KvalitudID;
        public static string KvalitudKP;
        public static float KvalitudKaal;


        public KaalVaade()
        {
            InitializeComponent();

            KaalItems = new ObservableCollection<Models.KaalVaadeModel>{ };
            TaidaList();
            //Määrame itemsSource, et asjad oleks kuvatud.
            kaalList.ItemsSource = KaalItems;
        }


        //võtame andmebaasist itemid
        async void TaidaList()
        {
            if (await App.Database.GetKaalItemsAsync() != null)
            {
                var items = await App.Database.GetKaalItemsAsync();
                foreach (var item in items)
                {
                    KaalItems.Add(item);
                }
                KaalTootlemine();
            }
        }

        public static void KaalTootlemine()
        {
            //Kuupäeva alusel nimekirja järjestamine
            Sorteerimine = new ObservableCollection<Models.KaalVaadeModel>(KaalItems.OrderByDescending(x => x.KaalKPItem));
            KaalItems.Clear();
            foreach(var item in Sorteerimine.ToList())
            {
                KaalItems.Add(item);
            }
        }

            private async void LisaKaalNupp_Vajutus(object sender, EventArgs e)
        {
            kaalList.SelectedItem = null;
            praeguValitud = null;
            //Muutuja andmesisestuslehe välja kutsumiseks
            var sisestaKaalVaade = new SisestaKaalVaade(false);

            //Andmesisestuslehe väljakutsumine
            await Navigation.PushModalAsync(sisestaKaalVaade);
        }

        private async void TrendNupp_Vajutus(object sender, EventArgs e)
        {
            kaalList.SelectedItem = null;
            praeguValitud = null;
            //Muutuja Trendlehe välja kutsumiseks
            var trend = new Trend();

            //Trendlehe väljakutsumine
            await Navigation.PushModalAsync(trend);
        }

        //Juhul kui item millele vajutad on valitud, deselectib selle.
        private void Listview_Vajutus(object sender, EventArgs e)
        {
            object uusValitud = kaalList.SelectedItem;

            if (uusValitud == praeguValitud)
            {
                YleKirjutus();
                kaalList.SelectedItem = null;
                praeguValitud = null;
            }
            else praeguValitud = uusValitud;
        }

        //kustutab kirje kui midagi on valitud
        private async void KustutaKaalKirje_Vajutus(object sender, EventArgs e)
        {
            if (kaalList.SelectedItem != null)
            {
                //määrame itemiks listviewis valitud itemi ja kustutame selle andmebaasist
                var andmeItem = (Models.KaalVaadeModel)kaalList.SelectedItem;
                //Võtame praegu valitud itemi indexi(uue itemi valimiseks)
                int selectedIndex = KaalItems.IndexOf(andmeItem);
                //kustutan kirje andmebaasist ja observableCollectionist
                await App.Database.DeleteKaalItemAsync(andmeItem);
                KaalItems.Remove(andmeItem);
                
                //peale kustutamist muudame indexit ja võtame järgmise itemi fookusesse, et oleks mugavam mass-kustutamist teha
                if (selectedIndex >= KaalItems.Count) selectedIndex--;

                if(selectedIndex >= 0)
                    kaalList.SelectedItem = KaalItems[selectedIndex];
            }
        }

        private void KirjutaYleNupp_Vajutus(object sender, EventArgs e)
        {
            YleKirjutus();
        }

        private async void YleKirjutus()
        {
            if (kaalList.SelectedItem != null)
            {
                //määrame itemiks listviewis valitud itemi
                var andmeItem = (Models.KaalVaadeModel)kaalList.SelectedItem;
                //Võtame praegu valitud itemi indexi(itemi üle kirjutamiseks)
                KvalitudIndex = KaalItems.IndexOf(andmeItem);
                //Lisame muutujatele väärtused mida kasutame järgmises vaates
                KvalitudID = andmeItem.Id;
                KvalitudKP = andmeItem.KaalKPItem;
                KvalitudKaal = andmeItem.KaalItem;

                //Muutuja andmesisestuslehe välja kutsumiseks
                var KirjutaYleVaade = new SisestaKaalVaade(true);

                //Andmesisestuslehe väljakutsumine
                await Navigation.PushModalAsync(KirjutaYleVaade);

                kaalList.SelectedItem = null;
                praeguValitud = null;
            }
        }
    }
}

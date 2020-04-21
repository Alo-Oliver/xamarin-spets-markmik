using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Paevik.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SisestaKaalVaade : ContentPage
    {
        //bool yleKirjutus;
        //muutujad
        
        string kuupaev;
        float kaal;
        bool rewrite = false;
        int ID;
        int index;

        public SisestaKaalVaade(bool mlem)
        {
            InitializeComponent();

            //lülitab ülekirjutamise sisse ja võtab listview-s valitud väärtused
            if (mlem == true)
            {
                SisendKuupaev.Date = DateTime.Parse(KaalVaade.KvalitudKP);
                SisendKaal.Text = KaalVaade.KvalitudKaal.ToString();
                ID = KaalVaade.KvalitudID;
                index = KaalVaade.KvalitudIndex;
                rewrite = true;

                LisaKirje.Text = "../";
            }
        }

        //Lehe tekkimisel valib kaal numbrivälja
        protected override void OnAppearing()
        {
            base.OnAppearing();
            SisendKaal.Focus();
        }

        //Sisesta nupule vajutamise event
        async void LisaKirje_Vajutus(object sender, EventArgs e)
        {
            //Sisestab väljadel olevad andmed
            Andmesisestus();
            //Navigeerib tagasi põhilehele
            await Navigation.PopModalAsync();
        }

        async void Andmesisestus()
        {
            //Kontrollib Nulle
            CheckForNulls();

            //sisestab väljadel olevad andmed, kui ID on olemas siis kirjutab andmed baasis üle
            if (rewrite == true)
            {
                var andmeItem = new Models.KaalVaadeModel { Id = ID, KaalKPItem = kuupaev, KaalItem = kaal };
                await App.Database.SaveKaalItemAsync(andmeItem);
                KaalVaade.KaalItems[index] = andmeItem;
                KaalVaade.KaalTootlemine();
                rewrite = false;
            }
            else
            {
                //Sisestab väljadel olevad andmed
                var andmeItem = new Models.KaalVaadeModel { KaalKPItem = kuupaev, KaalItem = kaal };
                await App.Database.SaveKaalItemAsync(andmeItem);
                KaalVaade.KaalItems.Add(andmeItem);
                KaalVaade.KaalTootlemine();
            }

        }

        //Kontrollib, et sisend ei oleks NULL
        void CheckForNulls()
        {
            if (SisendKuupaev.Date == null)
            {
                kuupaev = DateTime.Now.Date.ToString("yyyy-MM-dd");
            }
            else kuupaev = SisendKuupaev.Date.ToString("yyyy-MM-dd");

            if (SisendKaal.Text == null)
            {
                kaal = 0;
            }
            else kaal = float.Parse(SisendKaal.Text);
        }

        private async void SisendKaal_Completed(object sender, EventArgs e)
        {
            //Sisestab andmed
            Andmesisestus();
            //Navigeerib tagasi põhilehele
            await Navigation.PopModalAsync();
        }

        private async void Tagasi_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Paevik
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SisestaVaade : ContentPage
    {
        //muutujad
        string kuupaev;
        string toit;
        int kcal;
        int ID;
        int index;
        public bool rewrite = false;

        public SisestaVaade(bool mlem)
        {
            InitializeComponent();

            //lülitab ülekirjutamise sisse ja võtab listview-s valitud väärtused
            if (mlem == true)
            {
                SisendKuupaev.Date = DateTime.Parse(MainPage.valitudKP);
                SisendToit.Text = MainPage.valitudToit;
                SisendKcal.Text = MainPage.valitudKcal.ToString();
                ID = MainPage.valitudID;
                index = MainPage.valitudIndex;
                rewrite = true;

                LisaKirje.Text = "../";
            }
        }

        //Lehe tekkimisel valib toit tekstivälja
        protected override void OnAppearing()
        {
            base.OnAppearing();
            SisendToit.Focus();
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
            if(rewrite == true)
            {
                var andmeItem = new Models.PohiVaadeModel { Id = ID, KPItem = kuupaev, ToitItem = toit, KcalItem = kcal };
                await App.Database.SaveItemAsync(andmeItem);
                MainPage.PohiVaadeElemendid[index] = andmeItem;
                MainPage.Tootlemine();
                rewrite = false;
            }
            else
            {
                var andmeItem = new Models.PohiVaadeModel { KPItem = kuupaev, ToitItem = toit, KcalItem = kcal };
                await App.Database.SaveItemAsync(andmeItem);
                MainPage.PohiVaadeElemendid.Add(andmeItem);
                MainPage.Tootlemine();
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

            if (SisendToit.Text == null)
            {
                toit = "";
            }
            else toit = SisendToit.Text;

            if (SisendKcal.Text == null)
            {
                kcal = 0;
            }
            else kcal = Int32.Parse(SisendKcal.Text);
        }

        //lülitab fookuse kcal sisendile
        private void SisendToit_Completed(object sender, EventArgs e)
        {
            SisendKcal.Focus();
        }

        private async void SisendKcal_Completed(object sender, EventArgs e)
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
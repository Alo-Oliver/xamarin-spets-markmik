using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microcharts;
using Microcharts.Forms;
using SkiaSharp;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Entry = Microcharts.ChartEntry;

namespace Paevik.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Trend : ContentPage
    {
        float maxValue;
        float minValue;
        static List<Entry> entries = new List<Entry> { };

        public Trend()
        {
            InitializeComponent();

            //Tühjendab graafiku ja lisab aktuualse seisu
            entries.Clear();
            TaidaGraafik();

            //graafik
            TrendKaart.Chart = new LineChart {
                Entries = entries,
                LineMode = LineMode.Straight,
                MinValue = minValue,
                MaxValue = maxValue,
                LabelTextSize = 40,
                LineSize = 6,
                PointSize = 20,
                ValueLabelOrientation = Orientation.Horizontal,
            };
            
        }

        //Lisame graafikusse elemendid
        void TaidaGraafik()
        {
            if(KaalVaade.KaalItems != null)
            {
                //ajutised muutujad
                bool hasItem = false;
                string currentDate;
                DateTime newDate;
                float paevaKaal = 0;

                //võtab ainult viimase 30 päeva sisendid
                for (int i = 30; i >= 0; i --)
                {
                    newDate = DateTime.Now.AddDays(-i);
                    currentDate = newDate.ToString("yyyy-MM-dd");

                    //Kui ühel päeval on tehtud mitu sisendit, siis arvutab nende keskmise. Kui üks siis võtab selle.
                    foreach (var item in KaalVaade.KaalItems)
                    {
                        if (item.KaalKPItem == currentDate)
                        {
                            hasItem = true;
                            if(paevaKaal == 0)
                            {
                                paevaKaal = item.KaalItem;
                            }
                            else
                            {
                                paevaKaal = (paevaKaal + item.KaalItem) / 2;
                            }
                        }
                    }

                    //Kui praeguse loop kuupäevale vastab element, siis sisestab selle graafikusse
                    if(hasItem == true)
                    {
                        entries.Add
                        (
                            new Entry(paevaKaal)
                            {
                                Color = SKColor.Parse("#34c9eb"),
                                Label = currentDate,
                                ValueLabel = paevaKaal.ToString(),
                            }
                        );

                        //Kohandab graafiku ülem ja alam piiri
                        if (maxValue < paevaKaal || maxValue == 0)
                        {
                            maxValue = paevaKaal;
                        }
                        if (minValue > paevaKaal || minValue == 0)
                        {
                            minValue = paevaKaal - 1;
                        }

                        //reset, et järgmist kuupäeva töötleks korralikult
                        paevaKaal = 0;
                        hasItem = false;
                    }

                }

            }
        }

    }
}
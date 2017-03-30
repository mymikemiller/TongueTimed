using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace TTAndroid
{
    [Activity(Label = "Specify Vocab")]
    public class ChooseWordsActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Vocab);

            var btnOk = FindViewById<Button>(Resource.Id.ok);
            var txtVocab = FindViewById<EditText>(Resource.Id.vocab);

            Bundle extras =  Intent.Extras;
            if (extras != null)
            {

                String vocab = extras.GetString("initialVocab");
                txtVocab.SetText(vocab, TextView.BufferType.Editable);
            }

            btnOk.Click += delegate {
                Intent myIntent = new Intent(this, typeof(TongueTimed));
                myIntent.PutExtra("vocab", txtVocab.Text);
                SetResult(Result.Ok, myIntent);
                Finish();
            };

            // Create your application here
        }
    }
}
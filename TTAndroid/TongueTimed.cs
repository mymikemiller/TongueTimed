using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Speech.Tts;
using Android.Util;
//using Android.View.View;

using Java.Util;
using System.Collections.Generic;
using TTAndroid;
using TTLib;
using GIRLib;
using System.IO;

namespace TTAndroid
{
    [Activity(Label = "TongueTimed", MainLauncher = true, Icon = "@drawable/icon")]
    public class TongueTimed : Activity, TextToSpeech.IOnInitListener, TextToSpeech.IOnUtteranceCompletedListener
    {

        private TextToSpeech tts;
        private Button btnPlay;
        private Button btnReset;
        private EditText txtText;
        private Button btnChooseWords;

        const String TEACHER_CURRENT_INDEX = "TEACHER_CURRENT_INDEX";
        const String PLAYING = "PLAYING";

        private Dictionary<string, Locale> mSupportedLocales = new Dictionary<string, Locale>()
        {
            { "en", Locale.Us},
            { "de", Locale.German}
        };

        private bool mPlaying = false;

        private Teacher mTeacher;
        private SayablePair mCurrentSayablePair;


        /** Called when the activity is first created. */
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            tts = new TextToSpeech(this, this);

            using (StreamReader sr = new StreamReader(Assets.Open("BasicGermanPhrases.txt")))
            {
                mTeacher = new Teacher(sr);
            }
            
            txtText = (EditText)FindViewById(Resource.Id.txtText);
            btnPlay = (Button)FindViewById(Resource.Id.btnPlay);
            btnReset = (Button)FindViewById(Resource.Id.btnReset);
            btnChooseWords = FindViewById<Button>(Resource.Id.chooseWords);

            if (bundle != null)
            {
                mTeacher.CurrentGirIndicesIndex = bundle.GetInt(TEACHER_CURRENT_INDEX, 0);
                SetPlaying(bundle.GetBoolean(PLAYING, false));
            }

            btnChooseWords.Click += delegate {
                var myIntent = new Intent(this, typeof(TTAndroid.ChooseWordsActivity));
                String initialVocab = mTeacher.GetVocab();
                myIntent.PutExtra("initialVocab", initialVocab);
                StartActivityForResult(myIntent, 0);
            };
            
            btnReset.Click += delegate
            {
                mTeacher.Reset();
                SetPlaying(false);
            };
            btnPlay.Click += delegate
            {
                TogglePlay();
            };
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            outState.PutInt(TEACHER_CURRENT_INDEX, mTeacher.CurrentGirIndicesIndex);
            outState.PutBoolean(PLAYING, mPlaying);
        }

        private void SetPlaying(Boolean playing)
        {
            Boolean previousPlaying = mPlaying;
            mPlaying = playing;
            if (mPlaying)
            {
                btnPlay.Text = "Pause";
                if (!previousPlaying)
                {
                    ScheduleSay(1000);
                }
            }
            else
            {
                btnPlay.Text = "Play";
            }
        }

        private void TogglePlay()
        {
            SetPlaying(!mPlaying);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (resultCode == Result.Ok)
            {
                String vocab = data.GetStringExtra("vocab");
                mTeacher = new Teacher(vocab);
            }
        }

        private void sayNext()
        {
            if (mCurrentSayablePair == null)
            {
                mCurrentSayablePair = mTeacher.GetNextSayablePair();
                if (mCurrentSayablePair != null)
                {
                    txtText.Text = mCurrentSayablePair.SourceSayable.Line;
                    say(mCurrentSayablePair.SourceSayable);
                }
            }
            else
            {
                txtText.Text += ": " + mCurrentSayablePair.TargetSayable.Line;
                say(mCurrentSayablePair.TargetSayable);
                mCurrentSayablePair = null;
            }
        }

        private void SetLanguage(Locale locale)
        {
            LanguageAvailableResult result = tts.SetLanguage(locale);

            if (result == LanguageAvailableResult.MissingData
                    || result == LanguageAvailableResult.NotSupported)
            {
                Console.WriteLine("TTS: This Language is not supported");
                btnPlay.Enabled = false;
            }
            else
            {
                btnPlay.Enabled = true;
                //speakOut();
            }
        }

        protected override void OnDestroy()
        {
            // Don't forget to shutdown tts!
            if (tts != null)
            {
                tts.Stop();
                tts.Shutdown();
            }
            base.OnDestroy();
        }


        public void OnInit(OperationResult status)
        {

            if (status == OperationResult.Success)
            {

                SetLanguage(Locale.Us);
                tts.SetOnUtteranceCompletedListener(this);

            }
            else
            {
                Console.WriteLine("TTS: Initilization Failed!");
            }

        }

        private void say(Sayable sayable)
        {
            SetLanguage(mSupportedLocales[sayable.Language]);

            String text = sayable.Line;

            // This is necessary for OnUtteranceCompleted to be called
            Dictionary<String, String> parameters = new Dictionary<string, string>()
            {
                { TextToSpeech.Engine.KeyParamUtteranceId, "utteranceId"}
            };

            tts.Speak(text, QueueMode.Flush, parameters);
        }
        private void ScheduleSay(int milliseconds)
        {
            System.Timers.Timer t = new System.Timers.Timer();
            t.Interval = milliseconds;
            t.Elapsed += new System.Timers.ElapsedEventHandler(t_Elapsed);
            t.Start();
        }

        public void OnUtteranceCompleted(string utteranceId)
        {
            if (!mPlaying)
                return;

            ScheduleSay(1000);
        }
        protected void t_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            System.Timers.Timer t = (System.Timers.Timer)sender;
            t.Stop();
            RunOnUiThread(() => sayNext());
        }
    }
}


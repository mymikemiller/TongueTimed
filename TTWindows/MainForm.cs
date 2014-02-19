using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TTLib;
using GIRLib;

namespace TTWindows
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private async void btnTranslate_Click(object sender, EventArgs e)
        {
            Translator translator = new Translator();
            //VocabList vocabList = translator.Translate(txtInput.Text);
            //vocabList.WriteToFile("Achtung.txt");
            VocabList vocabList = new VocabList("Achtung.txt");


            //VocabList vocabList = new VocabList("Top2000ToGerman.txt", 0, 10);

            Console.WriteLine("Starting GIR");

            DateTime start = DateTime.Now;
            List<int> girIndeces = GIR.GetGIR(vocabList.GetVocabList().Count);//vocabList.GetVocabList());
            DateTime end = DateTime.Now;

            Console.WriteLine("Finished GIR in " + (end - start).ToString());
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Translator translator = new Translator();
            //VocabList vocabList = translator.Translate("Word Families", txtInput.Text);
            //vocabList.WriteToFile("Top2000ToGerman.txt");
            /*
            VocabList vocabList = new VocabList("Top2000ToGerman.txt", 0, 10);

            Console.WriteLine("Starting GIR");

            DateTime start = DateTime.Now;
            List<int> girIndeces = GIR.GetGIR(vocabList.GetVocabList().Count);//vocabList.GetVocabList());
            DateTime end = DateTime.Now;

            Console.WriteLine("Finished GIR in " + (end - start).ToString());
            */
            /*
            foreach(Vocab vocab in girObjects)
            {
                Console.WriteLine(vocab.ToString());
            }
             * */
            
        }
    }
}

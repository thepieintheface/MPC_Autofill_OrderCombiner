using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace OrderCombiner
{
    public partial class Form1 : Form
    {
        string outputFile = "";
        string mainFilePath = "";
        string firstFileContent = "";
        string secondFileContent = "";
        int count = 0;
        int[] size = { 18, 36, 54, 72, 90, 108, 126, 144, 162, 180, 198, 216, 234, 396, 504, 612 };
        public Form1()
        {  
            InitializeComponent();


        }

        string EditCount(string toBeEdited)
        {   
            int editAfter = toBeEdited.IndexOf("<quantity>") + 10;
            int editBefore = toBeEdited.IndexOf("</quantity>");
            StringBuilder editedText = new StringBuilder(toBeEdited);
            editedText.Remove(editAfter, (editBefore - editAfter));
            editedText.Insert(editAfter, count);
            return editedText.ToString();
        }
        string EditBracket(string toBeEdited)
        {
            int editAfter = toBeEdited.IndexOf("<bracket>") + 9;
            int editBefore = toBeEdited.IndexOf("</bracket>");
            StringBuilder editedText = new StringBuilder(toBeEdited);
            editedText.Remove(editAfter, (editBefore - editAfter));
            int setBracket = 0;
            for (int i = 0; i < size.Length; i++)
            {
                if (count > size[i])
                {
                    setBracket = size[i + 1];
                }
            }
            editedText.Insert(editAfter, setBracket);
            return editedText.ToString();
        }
        string ChopAfter(string toBeChopped)
        {
            string choppedText = "";
            int chopAfter = toBeChopped.LastIndexOf("</fronts>");
            if (chopAfter > 0)
            {
                choppedText = toBeChopped.Substring(0, chopAfter);
            }
            return choppedText;
        }
        string ChopBefore(string toBeChopped)
        {
            string choppedText = "";
            int chopBefore = toBeChopped.IndexOf("<fronts>");
            if (chopBefore > 0)
            {
                choppedText = toBeChopped.Substring(toBeChopped.IndexOf("<fronts>") + 8);
            }
            return choppedText;
        }
        char getFirstDigit(int num)
        {
            char[] numHolder = num.ToString().ToCharArray();
            return numHolder[0];
        }
        char getSecondDigit(int num)
        {
            char[] numHolder = num.ToString().ToCharArray();
            return numHolder[1];
        }
        char getThirdDigit(int num)
        {
            char[] numHolder = num.ToString().ToCharArray();
            return numHolder[2];
        }
        string ParseString(string inputString)
        {
            StringBuilder buildingString = new StringBuilder(inputString);
            List<int> deletions = new List<int>();



            for (int i = 1; i < buildingString.Length - 2; i++)
            {

                if (Char.IsDigit(buildingString[i]) && (buildingString[i - 1] == '[' || buildingString[i - 1] == ','))
                {
                    //only working way i found to convert int to char

                    buildingString[i] = getFirstDigit(count);

                    if (Char.IsDigit(buildingString[i + 1]) && count > 9)
                    {

                        // checks second number

                        buildingString[i + 1] = getSecondDigit(count);

                        if (Char.IsDigit(buildingString[i + 2]))
                        {
                            //checks 3rd number


                            buildingString[i + 2] = getThirdDigit(count);





                            i++;
                        }
                        else if (Char.IsDigit(buildingString[i + 2]) && count <= 99)
                        {

                            deletions.Add(i + 2);


                        }
                        else if (!Char.IsDigit(buildingString[i + 2]) && count > 99)
                        {
                            buildingString.Insert(i + 2, count.ToString()[2]);
                            i++;

                        }

                        i++;
                    }
                    else if (Char.IsDigit(buildingString[i + 1]) && count <= 9)
                    {

                        deletions.Add(i + 1);


                    }
                    else if (!Char.IsDigit(buildingString[i + 1]) && count > 9)
                    {
                        buildingString.Insert(i + 1, count.ToString()[1]);
                        i++;

                    }
                    Console.WriteLine("cycle: " + i + "count: " + count);



                    count++;
                }

            }



            for (int i = deletions.Count - 1; i >= 0; i--)
            {

                buildingString.Remove(deletions[i], 1);
            }
            return buildingString.ToString();
        }
        void SelectFiles()
        {

            firstFileContent = openFile();
            secondFileContent = openFile();

            
            if(firstFileContent.Length > 10 && secondFileContent.Length > 10)
            {
                firstFileContent = ChopAfter(firstFileContent);
                secondFileContent = ChopBefore(secondFileContent);

                outputFile = (firstFileContent + secondFileContent);
                outputFile = ParseString(outputFile);
                outputFile = EditCount(outputFile);
                outputFile = EditBracket(outputFile);
            }
            
        }
        string openFile()
        {
            string returnString = "";
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "xml files (*.xml)|*.xml";
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                   

                    var fileStream = openFileDialog.OpenFile();

                    using (StreamReader reader = new StreamReader(fileStream))
                    {
                        returnString = reader.ReadToEnd();
                    }
                }
            }
            return returnString;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            if (firstFileContent.Length > 10 && secondFileContent.Length > 10)
            {
                XDocument doc = XDocument.Parse(outputFile);
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "XML-File | *.xml";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    doc.Save(saveFileDialog.FileName);
                }
            }

            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SelectFiles();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}

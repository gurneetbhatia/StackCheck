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
using System.Diagnostics;
using System.Globalization;
using Algorithmia;
//using AlgorithmiaPipe;
using CsvHelper;
using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json.Linq;

namespace oof1
{
    public partial class Form1 : Form
    {
        private string input = string.Empty;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            //String path = @"C:\Users\greta_000\OneDrive\hackaton Broah\oof1\InputText.txt";
            //File.WriteAllText(path, textBox1.Text);

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string input = textBox1.Text;
            string code = convert(input);

            List<string>[] output = MainAlgorithm.Run(code);
            List<string> goodAnswers = output[0];
            List<string> badAnswers = output[1];
            List<string> extraData = output[2];
            float score = float.Parse(extraData.ElementAt(0));
            float total = float.Parse(extraData.ElementAt(1));
            textBox2.Text = (score * 100 / total) + "%";
            string goodAnsOut = "";
            for(int i = 0; i < goodAnswers.Count; i++)
            {
                goodAnsOut += goodAnswers.ElementAt(i) + "\n\n";
            }
            textBox3.Text = goodAnsOut;
            string badAnsOut = "";
            for (int i = 0; i < badAnswers.Count; i++)
            {
                badAnsOut += badAnswers.ElementAt(i)+"\n\n";
            }
            textBox4.Text = badAnsOut;

            

        }


        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            //String path = @"C:\Users\greta_000\OneDrive\hackaton Broah\oof1\InputText.txt";
            //File.Create(path);
        }

        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private string convert(string input)
        {
            string code = "";
            using (StringReader reader = new StringReader(input))
            {
                string line;
                string line1 = "";
                string[] words = { };
                line = reader.ReadLine();
                while (line != null)
                {
                    words = line.Split(' ');
                    for (int j = 0; j < words.Length; j++)
                    {
                        if (j < words.Length - 1 && words[j + 1].Contains("="))
                            line1 = line1 + " * ";
                        else
                            line1 = line1 + words[j] + " ";
                    }
                    code = code + line1 + "\n";
                    line1 = "";
                    line = reader.ReadLine();
                }
            }
            return code;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }

    static class MainAlgorithm
    {
        //static List data;
        public static List<string>[] Run(string input)
        {
            /*var input = "{"
             + "  \"document\": \"This is horrible code. It looks like it was written by a MMU student. I would not trust you to solve a kindergarten maths problem.\""
             + "}";
            JArray result = (Newtonsoft.Json.Linq.JArray)getSentiment(input).result;
            Console.WriteLine((float)result[0]["sentiment"]);

            using (var reader = new StreamReader("/Users/inderdeepbhatia/Projects/HTBTest1/HTBTest1/data_please.csv"))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var records = csv.GetRecords<dynamic>();
                data = records.ToArray();
            }
            var codeInput = "'Object obj = new Object();'";
            var codeCSV = "'Object ob = new Object();'";
            var matchRatio = "python3 sequence.py " + codeInput + " " + codeCSV;
            Console.WriteLine(run_cmd("python3 /Users/inderdeepbhatia/Projects/HTBTest1/HTBTest1", codeInput + " " + codeCSV));*/
            //HTBTest1.SequenceMatcher sequenceMatcher = new HTBTest1.SequenceMatcher();

            //var input = "Object obj = new Object();";

            List<string> goodAnswers = new List<string>();
            List<string> badAnswers = new List<string>();
            List<string> extraData = new List<string>();

            using (var reader = new StreamReader("/Users/inderdeepbhatia/Projects/HTBTest1/HTBTest1/data_please.csv"))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var data = csv.GetRecords<dynamic>().ToList();
                //data.GetEnumerator().MoveNext();
                //Console.WriteLine(data.ElementAt(0).QuestionBody);
                double score = 0.0;
                double total = 0.0;
                for (int i = 0; i < data.Count; i++)
                {
                    var row = data.ElementAt(i);
                    string hasCode = row.HasCode;
                    double newScore = 0.0;
                    if (hasCode == "True")
                    {
                        string answerBody = row.AnswerBody;
                        string commentBody = row.CommentText;
                        int answerScore = int.Parse(row.AnswerScore);
                        double distance = (double)StringDistance.LevenshteinDistance(input, answerBody);
                        double matchPercentage = (input.Length - (distance)) / input.Length;
                        if (matchPercentage > 0.6)
                        {
                            newScore = getScore(answerBody, commentBody, matchPercentage, answerScore);
                            total += matchPercentage * answerScore;
                            if (newScore >= 0)
                            {
                                goodAnswers.Add(commentBody);
                            }
                            else
                            {
                                badAnswers.Add(commentBody);
                            }
                        }
                    }
                    else
                    {
                        String questionBody = row.QuestionBody;
                        String answerBody = row.AnswerBody;
                        int answerScore = int.Parse(row.AnswerScore);
                        double distance = (double)StringDistance.LevenshteinDistance(input, questionBody);
                        double matchPercentage = (input.Length - distance) / input.Length;
                        if (matchPercentage > 0.6)
                        {
                            newScore = getScore(questionBody, answerBody, matchPercentage, answerScore);
                            total += matchPercentage * answerScore;
                            if (newScore >= 0)
                            {
                                goodAnswers.Add(answerBody);
                            }
                            else
                            {
                                badAnswers.Add(answerBody);
                            }
                        }
                    }
                    score += newScore;
                    //Console.WriteLine("Current Score: " + score);
                }
                extraData.Add(""+score);
                extraData.Add("" +total);
            }
            return new List<string>[] { goodAnswers, badAnswers, extraData };

            /*String answerBody = "Object obj = new Object();";
            String commentBody = "That is acceptable code, but can use more memory than what you might be intending to";
            double matchPercentage = 0.8;
            int answerScore = 30;
            WriteLine(getScore(answerBody, commentBody, matchPercentage, answerScore));*/

        }

        static String run_cmd(string cmd, string args)
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = cmd;
            start.Arguments = args;
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            using (Process process = Process.Start(start))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                    return result;
                }
            }
        }

        static double getScore(String answerBody, String commentBody, double matchPercentage, int answerScore)
        {
            if (answerScore == 0)
            {
                answerScore = 1;
            }
            commentBody = "{"
             + "  \"document\": \"" + commentBody + "\""
             + "}";
            double commentSentiment;
            if (getSentiment(commentBody).result.GetType().ToString().Equals("System.Int64"))
            {
                commentSentiment = 0.01;
            }
            else
            {
                //WriteLine(getSentiment(commentBody).result.GetType());
                JArray result = (JArray)getSentiment(commentBody).result;
                commentSentiment = (double)result[0]["sentiment"];
            }
            //WriteLine(commentSentiment);
            var score = matchPercentage * answerScore * commentSentiment;

            return score;
        }

        static AlgorithmResponse getSentiment(String input)
        {
            var client = new Client("sim+5Uw+MCHUKHUA9GZhJeOlihU1");
            var algorithm = client.algo("nlp/SentimentAnalysis/1.0.5");
            algorithm.setOptions(timeout: 300); // optional
            var response = algorithm.pipeJson<object>(input);
            return response;
        }
    }

    public static class ShellHelper
    {
        public static string Bash(this string cmd)
        {
            var escapedArgs = cmd.Replace("\"", "\\\"");

            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"{escapedArgs}\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            process.Start();
            string result = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return result;
        }

    }

    public static class StringDistance
    {

        private static void Run()

        {
            //Console.WriteLine("Mama", "Mom");
            var codeInput = "'Object obj = new Object();'";
            var codeCSV = "'Object ob = new Object();'";
            double distance = (double)StringDistance.LevenshteinDistance(codeInput, codeCSV);
            //Console.WriteLine((codeInput.Length - (distance)) / codeInput.Length + "%");
            //Console.WriteLine((6 - StringDistance.LevenshteinDistance("climax", "volmax")) / 100 + "%");
            //Console.WriteLine(StringDistance.LevenshteinDistance("Ram", "Raman"));
            //Console.WriteLine(StringDistance.LevenshteinDistance("Mama", "Mom"));


        }


        public static int LevenshteinDistance(string s, string t)
        {

            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            if (n == 0)
            {
                return m;
            }

            if (m == 0)
            {
                return n;
            }

            for (int i = 0; i <= n; d[i, 0] = i++)
            {
            }

            for (int j = 0; j <= m; d[0, j] = j++)
            {
            }


            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            return d[n, m];
        }

    }

}

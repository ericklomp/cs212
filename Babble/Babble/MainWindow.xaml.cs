using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Collections;
using System.Management.Instrumentation;
using System.Security.Cryptography;

namespace Babble
{
    /// Babble framework
    /// Starter code for CS212 Babble assignment
    public partial class MainWindow : Window
    {
        private string input;               // input file
        private string[] words;             // input file broken into array of words
        private int wordCount = 200;        // number of words to babble
        Dictionary<string, ArrayList> hashTable = new Dictionary<string, ArrayList>();

        public MainWindow()
        {
            InitializeComponent();
        }

        //Loads a file and creates an array of all the words in it
        private void loadButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.FileName = "Sample"; // Default file name
            ofd.DefaultExt = ".txt"; // Default file extension
            ofd.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension

            // Show open file dialog box
            if ((bool) ofd.ShowDialog())
            {
                textBlock1.Text = "Loading file " + ofd.FileName + "\n";
                input = System.IO.File.ReadAllText(ofd.FileName);  // read file
                words = Regex.Split(input, @"\s+");       // split into array of words
            }
            makehashTable();
        }

        //analyzeInput shows a messagebox displaying the new order selected
        private void analyzeInput(int order)
        {
            if (order > 0)
            {
                 MessageBox.Show("Analyzing at order: " + (order + 1));
            }
        }

        //babbleButton_Click even creates the randomizewd text using a hashtable
        private void babbleButton_Click(object sender, RoutedEventArgs e)
        {
            textBlock1.Text = "";
            int order = orderComboBox.SelectedIndex + 1;
            string key = "";
            if (order > 0)
            {
                Random RNG = new Random();
                int rand;

                //created the key
                key = words[0];
                for(int i = 1; i < order; i++)
                {
                    key += " " + words[i];
                }

                string randText = key;

                //loops till at least wordCount number of words
                while (randText.Count(char.IsWhiteSpace) < wordCount - 1)
                {
                    if (hashTable.ContainsKey(key))
                    {
                        rand = RNG.Next(hashTable[key].Count);
                        randText += " " + (string)hashTable[key][rand];

                        if (order > 1)
                        {
                            //removes the first words from the kety and adds on the new word picked.
                            key = key.Substring(key.IndexOf(" ") + 1) + " " + (string)hashTable[key][rand];
                        }
                        else
                        {
                            //chanes key to the new word picked 
                            key = (string)hashTable[key][rand];
                        }
                    }
                    else
                    {
                        //created new key
                        key = words[0];
                        for (int i = 1; i < order; i++)
                        {
                            key += " " + words[i];
                        }
                        randText += key;
                    }
                }
                textBlock1.Text = randText;
            }
        }

        //calls makehashTable() when order is changed
        private void orderComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            makehashTable();
        }

        //creates a hash table
        public void makehashTable()
        {
            hashTable.Clear();
            analyzeInput(orderComboBox.SelectedIndex);
            int order = orderComboBox.SelectedIndex + 1;
            string key = "";
            if (order > 0 && words != null)
            {
                textBlock1.Text = "";
                    for (int i = 0; i < words.Count() - order; i++)
                    {
                        key = "";
                        key = words[i];
                    for (int j = 1; j < order; j++)
                    {
                        key = key + " " + words[i + j];
                    }
                    if (!hashTable.ContainsKey(key))
                    {
                        hashTable.Add(key, new ArrayList());
                    }
                    hashTable[key].Add(words[i + 1]);

                    }
                    textBlock1.Text += "Number of words: " + words.Count();
                    textBlock1.Text += "\nNumber of keys: " + hashTable.Keys.Count();
                }
                    
            }
        }
    }

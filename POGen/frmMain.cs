using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using Netology.Localization;

namespace POGen
{
    public partial class frmMain : Form
    {
        string Filename;
        string SelectedPath;
        CatalogCollection catalogList;
        Dictionary<string, string> verbatimList;
    

        public frmMain()
        {
            InitializeComponent();
            catalogList = new CatalogCollection();
            verbatimList = new Dictionary<string,string>();
        }

        private void buttonGenerate_Click(object sender, EventArgs e)
        {
            catalogList.Clear();

            DialogResult res = saveFileDialog1.ShowDialog();

            if (res != DialogResult.OK)
            {
                return;               
            }

            labelStatus.Text = "Location helper code, adding in files...";
            Filename = saveFileDialog1.FileName;

            DirectoryWalker dw = new DirectoryWalker(
                new DirectoryWalker.ProcessDirCallback(PrintDir),
                new DirectoryWalker.ProcessFileCallback(PrintFile));

            dw.Walk(SelectedPath, "*.aspx,*.ascx,*.master,*.cs");

            labelStatus.Text = "POT file preparing...";
            if (catalogList.Count > 0)
            {
                GeneratePOTFile();
            }

            labelStatus.Text = "POT file generated!..";
        }

        public void PrintDir(DirectoryInfo d, int level, object obj)
        {
            labelFolderName.Text = d.Name;
            labelFolderName.Refresh();
        }

        public void PrintFile(FileInfo f, int level, object obj)
        {
            labelFileName.Text = f.Name;
            labelFileName.Refresh();
            
            listBox1.Items.Add(labelFileName.Text);            

            try
            {
                bool isCodeBehind = (f.Extension == ".cs");
                string regexpattern = string.Empty;
                string replacepattern = string.Empty;

                FileStream fs = f.Open(FileMode.Open, FileAccess.Read);
                StreamReader sr = new StreamReader(fs);
                string s = sr.ReadToEnd();
  
                fs.Close();
                sr.Close();

                System.Text.RegularExpressions.RegexOptions options = ((System.Text.RegularExpressions.RegexOptions.IgnorePatternWhitespace | System.Text.RegularExpressions.RegexOptions.Multiline)
                        | System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                string findPOCallRegexPattern = "_\\((\\\"(?:\\\\\\\"|[^\\\\\\\"^])*\\\")\\)";

                if (isCodeBehind) // *.cs files....
                {
                    // Get Verbatim String Started...
                    // Verbatim string finder regex pattern ->  (?m:@\"((?: \"\"|[^\"])+)\")
                    string verbatimregexPattern = "(?m:@\\\"((?: \\\"\\\"|[^\\\"])+)\\\")";
                    Regex verbatim = new Regex(verbatimregexPattern, options);
                    if (verbatim.IsMatch(s))
                    {
                        MatchCollection matchCol = verbatim.Matches(s);
                        verbatimList.Clear();
                        foreach (Match match in matchCol)
                        {
                            string replaceFlag = string.Format("_VERBATIM_{0}_VERBATIM_", Guid.NewGuid());
                            verbatimList.Add(replaceFlag, match.Value);
                            s = s.Replace(match.Value, replaceFlag);
                        }
                    }

                    // Clean Old PO Method Call Definations...
                    Regex cleanReg = new Regex(findPOCallRegexPattern, options);
                    s = cleanReg.Replace(s, "$1");

                    // Regex Patterns...
                    regexpattern = "(\\\"(?:\\\\\\\"|[^\\\"])*\\\")"; //{"xxxx"} then => {_("xxxx")}
                    replacepattern = "_($1)";                    
                }
                else // *.aspx, *.ascx, *.master files....
                {
                    // Regex Patterns...
                    regexpattern = "((?<=(?:ErrorMessage|Text)\\=)\\\"[^\"]+\\\")"; //{ErrorMessage="xxxx"} then => {ErrorMessage=' <% _("xxxx") %>'}
                    replacepattern = "'<%= _($1) %>'";
                }

                // Find & Replace All Messages...
                Regex reg = new Regex(regexpattern, options);
                s = reg.Replace(s, replacepattern);

                // Put Verbatim String 
                foreach (KeyValuePair<string, string> verbatimItem in verbatimList)
                {
                    s = s.Replace(verbatimItem.Key, verbatimItem.Value);
                }

                // Attention!! Start

                if (isCodeBehind) // Only Write Code File...
                {                    
                    StreamWriter sw = new StreamWriter(f.FullName);
                    sw.Write(s);
                    sw.Close();
                }

                // This code closed. Because _("xxx") method call not supported on aspx files...
                // This Code Only Generate PO File..

                ////StreamWriter sw = new StreamWriter(f.FullName, false, System.Text.Encoding.UTF8);
                //StreamWriter sw = new StreamWriter(f.FullName);
                //sw.Write(s);
                //sw.Close();

                // Attention!! End


                // Find PO Method Call List...
                Regex findAllReg = new Regex(findPOCallRegexPattern, options);
                MatchCollection matches = findAllReg.Matches(s);

                // Add POT Catalog Array..
                foreach (Match mm in matches)
                {
                    if (!catalogList.ContainsKey(mm.Value))
                    {
                        Catalog catalog = new Catalog();
                        catalog.MessageId = mm.Value.Substring(3, mm.Value.Length - 5);
                        catalog.Message = string.Empty;
                        catalog.FileNames.Add(string.Format("{0}:{1}", f.FullName, mm.Index));

                        catalogList.Add(mm.Value, catalog);
                    }
                    else
                    {
                        catalogList[mm.Value].FileNames.Add(string.Format("{0}:{1}", f.FullName, mm.Index));
                    }
                }
            }
            catch
            {
                throw;
                //TODO: ExceptionLog
            }

            
        }

        private void GeneratePOTFile()
        {
            StreamWriter potFile = new StreamWriter(Filename, false, Encoding.UTF8);

            string headerText = @"# SOME DESCRIPTIVE TITLE.
# Copyright (C) YEAR THE PACKAGE'S COPYRIGHT HOLDER
# This file is distributed under the same license as the PACKAGE package.
# FIRST AUTHOR <EMAIL@ADDRESS>, YEAR.
#
msgid """"
msgstr """"
""Project-Id-Version: Pogen 0.1\n""
""Report-Msgid-Bugs-To: meddah@netology.org\n""
""POT-Creation-Date: {0}\n""
""PO-Revision-Date: {0}\n""
""Last-Translator: \n""
""Language-Team: Netology Localization Team <meddah@l10n.netology.org>\n""
""MIME-Version: 1.0\n""
""Content-Type: text/plain; charset=utf-8\n""
""Content-Transfer-Encoding: 8bit\n""


";
            potFile.Write(string.Format(headerText, DateTime.Now.ToString()));
             

            foreach (KeyValuePair<string, Catalog> catalog in catalogList)
            {
                foreach(string filepath in catalog.Value.FileNames)
                {
                    potFile.Write(string.Format("#: {0}\r\n", filepath));
                }               

                potFile.Write(string.Format("msgid \"{0}\"\r\n", catalog.Value.MessageId));
                potFile.Write(string.Format("msgstr \"{0}\"\r\n", catalog.Value.Message));
                potFile.Write("\r\n");
            }

            potFile.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.SelectedPath = textboxLookup.Text;
            DialogResult res = folderBrowserDialog1.ShowDialog();

            if (res == DialogResult.OK)
            {
                SelectedPath = folderBrowserDialog1.SelectedPath;
                textboxLookup.Text = folderBrowserDialog1.SelectedPath;
                buttonGenerate.Enabled = true;
            }
            
        }

        private void textboxLookup_TextChanged(object sender, EventArgs e)
        {
            if (textboxLookup.Text.Length > 0)
            {
                buttonGenerate.Enabled = true;
            }
            else
            {
                buttonGenerate.Enabled = false;
            }
        }
    }
}
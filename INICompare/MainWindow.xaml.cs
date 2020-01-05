using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
//using System.Windows.Shapes;
using System.IO;
using System.Text.RegularExpressions;

namespace INICompare
{
    using INI = Dictionary<string, Dictionary<string, string>>;
    using INIBlock = Dictionary<string, string>;

    public partial class MainWindow : Window
    {
        Microsoft.Win32.OpenFileDialog _openFileDialog = new Microsoft.Win32.OpenFileDialog();

        #region INI1Path
        ///<summary> File path for INI file 1.</summary>
        public string INI1Path
        {
            get { return (string)GetValue(INI1PathProperty); }
            set { SetValue(INI1PathProperty, value); }
        }

        ///<summary>The backing store for the INI1Path property.</summary>
        public static readonly DependencyProperty INI1PathProperty = DependencyProperty.Register(nameof(INI1Path), typeof(string), typeof(MainWindow), new UIPropertyMetadata(default(string)));
        #endregion
        #region INI2Path
        ///<summary> File path for INI file 2.</summary>
        public string INI2Path
        {
            get { return (string)GetValue(INI2PathProperty); }
            set { SetValue(INI2PathProperty, value); }
        }

        ///<summary>The backing store for the INI2Path property.</summary>
        public static readonly DependencyProperty INI2PathProperty = DependencyProperty.Register(nameof(INI2Path), typeof(string), typeof(MainWindow), new UIPropertyMetadata(default(string)));
        #endregion
        #region INI1PathBrush
        ///<summary> Colored Brush for INI 1 visual display. </summary>
        public Brush INI1PathBrush
        {
            get { return (Brush)GetValue(INI1PathBrushProperty); }
            set { SetValue(INI1PathBrushProperty, value); }
        }

        ///<summary>The backing store for the INI1PathColor property.</summary>
        public static readonly DependencyProperty INI1PathBrushProperty = DependencyProperty.Register(nameof(INI1PathBrush), typeof(Brush), typeof(MainWindow), new UIPropertyMetadata(default(Brush)));
        #endregion
        #region INI2PathBrush
        ///<summary> Colored Brush for INI 2 visual display. </summary>
        public Brush INI2PathBrush
        {
            get { return (Brush)GetValue(INI2PathBrushProperty); }
            set { SetValue(INI2PathBrushProperty, value); }
        }

        ///<summary>The backing store for the INI2PathBrush property.</summary>
        public static readonly DependencyProperty INI2PathBrushProperty = DependencyProperty.Register(nameof(INI2PathBrush), typeof(Brush), typeof(MainWindow), new UIPropertyMetadata(default(Brush)));
        #endregion
        #region InComparison
        ///<summary> True when comparison mode is on. </summary>
        public bool InComparison
        {
            get { return (bool)GetValue(InComparisonProperty); }
            set { SetValue(InComparisonProperty, value); }
        }

        ///<summary>The backing store for the InComparison property.</summary>
        public static readonly DependencyProperty InComparisonProperty = DependencyProperty.Register(nameof(InComparison), typeof(bool), typeof(MainWindow), new UIPropertyMetadata(default(bool)));
        #endregion

        INI INI1File = null;
        INI INI2File = null;

        Brush FILEEXISTS = Brushes.LightGreen;
        Brush FILENOTEXIST = Brushes.LightPink;


        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _openFileDialog.DefaultExt = ".ini";
            _openFileDialog.Filter = "Configuration Files (.ini)|*.ini|All files (*.*)|*.*";



#if DEBUG
            bool testing = false;
            if (testing)
            {
                _openFileDialog.InitialDirectory = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                INI1Path = Path.Combine(_openFileDialog.InitialDirectory, "test1.ini");
                INI2Path = Path.Combine(_openFileDialog.InitialDirectory, "test2.ini");
                //INI1PathTB.ScrollToEnd();
                //INI2PathTB.ScrollToEnd();
                INI1PathBrush = LoadINI(INI1Path, out INI1File) ? FILEEXISTS : FILENOTEXIST;
                INI2PathBrush = LoadINI(INI2Path, out INI2File) ? FILEEXISTS : FILENOTEXIST;
                ShowUnCompared();
            }
#endif
        }

        private void LoadINI1_Click(object sender, RoutedEventArgs e)
        {
            if (!(_openFileDialog.ShowDialog() ?? false)) return;
            INI1Path = _openFileDialog.FileName;
            //INI1Path = OpenFileDialog();
            //INI1PathTB.ScrollToEnd();
            INI1PathBrush = LoadINI(INI1Path, out INI1File) ? FILEEXISTS : FILENOTEXIST;
            ShowUnCompared();
        }

        private void LoadINI2_Click(object sender, RoutedEventArgs e)
        {
            if (!(_openFileDialog.ShowDialog() ?? false)) return;
            INI2Path = _openFileDialog.FileName;
            //INI2Path = OpenFileDialog();
            //INI2PathTB.ScrollToEnd();
            INI2PathBrush = LoadINI(INI2Path, out INI2File) ? FILEEXISTS : FILENOTEXIST;
            ShowUnCompared();
        }

        private void Switch_Click(object sender, RoutedEventArgs e)
        {
            var pathSwap = INI1Path;
            INI1Path = INI2Path;
            INI2Path = pathSwap;

            var iniSwap = INI1File;
            INI1File = INI2File;
            INI2File = iniSwap;

            var colorSwap = INI1PathBrush;
            INI1PathBrush = INI2PathBrush;
            INI2PathBrush = colorSwap;

            //re-run compare instead so the colors update accordingly
            if (InComparison)
                CompareINIs(INI1File, INI2File);
            else
                ShowUnCompared();
            //var document1Swap = INI1RichTextBox.Document;
            //var document2Swap = INI2RichTextBox.Document;
            //INI1RichTextBox.Document = new FlowDocument();
            //INI2RichTextBox.Document = new FlowDocument();
            //INI1RichTextBox.Document = document2Swap;
            //INI2RichTextBox.Document = document1Swap;


        }

        private void ShowUnCompared()
        {
            InComparison = false;
            if (INI1File != null)
                INI1RichTextBox.Document = new FlowDocument(INIToRichText(INI1File)) { PageWidth = 1000 };
            if (INI2File != null)
                INI2RichTextBox.Document = new FlowDocument(INIToRichText(INI2File)) { PageWidth = 1000 };
        }

        //private string OpenFileDialog()
        //{
        //    return _openFileDialog.ShowDialog() ?? false ? _openFileDialog.FileName : "";
        //}

        private bool LoadINI(string path, out INI file)
        {
            if (!File.Exists(path))
            {
                file = null;
                return false;
            }

            Regex iniHeader = new Regex(@"^\[[^\]\r\n]+]");
            Regex iniKeyValue = new Regex(@"^([^=;\r\n]+)=([^;\r\n]*)");

            var lines = File.ReadAllLines(path);
            file = new INI();

            var duplicateKeys = new List<Tuple<string, string, string, string>>(); //block, key, old value, new value

            Dictionary<string, string> currentBlock = null;
            foreach (var line in lines)
            {
                try
                {
                    if (iniHeader.IsMatch(line))
                        file.Add(line.Trim(new char[] { ' ', ']', '[', '\t' }), currentBlock = new INIBlock());

                    if (iniKeyValue.IsMatch(line))
                    {
                        if (currentBlock == null)
                            file.Add("", currentBlock = new INIBlock());

                        var match = iniKeyValue.Match(line);
                        var key = match.Groups[1].Value.Trim();
                        var value = match.Groups[2].Value.Trim();

                        if (currentBlock.ContainsKey(key))
                        {
                            var currentBlockName = file.FirstOrDefault(kv => kv.Value == currentBlock).Key;
                            duplicateKeys.Add(Tuple.Create(currentBlockName, key, currentBlock[key], value));

                            currentBlock[key] = value;
                        }
                        else
                        {
                            currentBlock.Add(key, value);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("This INI could not be parsed. The line being parsed was:\n" + line +
                        "\n\nDetailed exception information below:\n" + ex.InnerException
                        , "Exception Alert", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    file = null;
                    return false;
                }
            }
            if (duplicateKeys.Count > 0)
                MessageBox.Show(string.Format("{0} values were overwritten by same keys in the same section. Only the last set value for the key will be used.\n[Section] Key = Early Value -> Later Value\n{1}"
                    , duplicateKeys.Count, String.Join("\n", duplicateKeys.Select(dup => string.Format("[{0}] {1} = {2} -> {3}", dup.Item1, dup.Item2, dup.Item3, dup.Item4))))
                    , "Duplicate Key-Values Overwritten", MessageBoxButton.OK, MessageBoxImage.Information);
            return true;

        }

        private Paragraph INIToRichText(INI ini)
        {
            var paragraph = new Paragraph();
            if (ini == null)
                return paragraph;

            foreach (var section in ini)
            {
                if (section.Key.Length > 0)
                    paragraph.AppendLine(string.Format("[{0}]", section.Key), bold: true);
                foreach (var keyvalue in section.Value)
                    paragraph.AppendLine(string.Format("{0} = {1}", keyvalue.Key, keyvalue.Value));
                paragraph.AppendLine();
            }
            return paragraph;
        }

        private void Compare_Click(object sender, RoutedEventArgs e)
        {
            InComparison = true;
            CompareINIs(INI1File, INI2File);
        }

        private void CompareINIs(INI file1, INI file2)
        {
            if (file1 == null || file2 == null)
                return;

            var paragraph1 = new Paragraph();
            var paragraph2 = new Paragraph();

            var matchedSections = new List<string>();
            var missedSectionsAccountedFor = new List<string>();

            foreach (var section in file1)
            {
                if (file2.ContainsKey(section.Key))
                {
                    //this section is in both INIs compare it and output
                    matchedSections.Add(section.Key);

                    //but first, lets take a moment to find any sections from file2 that may have been skipped at this point. Specifically sections that are not in file1 at all so we can write them out.
                    var file2SectionKeys = file2.Keys.ToList();
                    var file2SectionKeysNotSeenYet = file2SectionKeys.GetRange(0, file2SectionKeys.IndexOf(section.Key))
                                                    .Where(sectionKey => !matchedSections.Contains(sectionKey) //haven't matched it before
                                                    && !missedSectionsAccountedFor.Contains(sectionKey)        //haven't fixed it before
                                                    && !file1.ContainsKey(sectionKey));                        //won't be getting to it later

                    foreach (var missingSectionKey in file2SectionKeysNotSeenYet)
                    {
                        //this section is not in the first INI, display it as added

                        if (missingSectionKey.Length > 0)
                        {
                            paragraph1.AppendLine();
                            paragraph2.AppendLine(string.Format("[{0}]", missingSectionKey), Brushes.LightGreen, bold: true);
                        }
                        foreach (var keyvalue in file2[missingSectionKey])
                        {
                            //none of the lines can match
                            paragraph1.AppendLine();
                            paragraph2.AppendLine(string.Format("{0} = {1}", keyvalue.Key, keyvalue.Value), Brushes.LightGreen);
                        }
                        paragraph1.AppendLine();
                        paragraph2.AppendLine();

                        missedSectionsAccountedFor.Add(missingSectionKey);
                    }


                    //okay, now we continue with this section
                    var matchedKeys = new List<string>();
                    var missedKeysAccountedFor = new List<string>();

                    if (section.Key.Length > 0)
                    {
                        paragraph1.AppendLine(string.Format("[{0}]", section.Key), bold: true);
                        paragraph2.AppendLine(string.Format("[{0}]", section.Key), bold: true);
                    }
                    foreach (var keyvalue in section.Value)
                    {
                        //now check each line
                        if (file2[section.Key].ContainsKey(keyvalue.Key))
                        {
                            //this key is in both sections
                            matchedKeys.Add(keyvalue.Key);

                            //lets take a moment to find any keys from section2 that may have been skipped at this point. Specifically, keys are not in section1 at all, so we can write them out.
                            var section2Keys = file2[section.Key].Keys.ToList();
                            var section2KeysNotSeenYet = section2Keys.GetRange(0, section2Keys.IndexOf(keyvalue.Key))
                                                        .Where(key => !matchedKeys.Contains(key) //haven't matched it before
                                                        && !missedKeysAccountedFor.Contains(key) //haven't taken care of it before
                                                        && !section.Value.ContainsKey(key));     //won't be getting to it

                            foreach (var missingKey in section2KeysNotSeenYet)
                            {
                                paragraph1.AppendLine();
                                paragraph2.AppendLine(string.Format("{0} = {1}", missingKey, file2[section.Key][missingKey]), Brushes.LightGreen);
                                missedKeysAccountedFor.Add(missingKey);
                            }

                            //okay, now we continue with this new match

                            //commented out, would only color the part of the INI line that changed, now its set to do the whole line
                            //paragraph1.Append(string.Format("{0} = ", keyvalue.Key));
                            //paragraph2.Append(string.Format("{0} = ", keyvalue.Key));

                            if (keyvalue.Value == file2[section.Key][keyvalue.Key])
                            {
                                //values are the same
                                //paragraph1.AppendLine(string.Format("{0}", keyvalue.Value));
                                //paragraph2.AppendLine(string.Format("{0}", keyvalue.Value));
                                paragraph1.AppendLine(string.Format("{0} = {1}", keyvalue.Key, keyvalue.Value));
                                paragraph2.AppendLine(string.Format("{0} = {1}", keyvalue.Key, keyvalue.Value));
                            }
                            else
                            {
                                //values are different
                                //paragraph1.AppendLine(string.Format("{0}", keyvalue.Value), Brushes.LightGray);
                                //paragraph2.AppendLine(string.Format("{0}", file2[section.Key][keyvalue.Key]), Brushes.LightGray);
                                paragraph1.Append(string.Format("{0} = ", keyvalue.Key), Brushes.LightGray);
                                paragraph2.Append(string.Format("{0} = ", keyvalue.Key), Brushes.LightGray);
                                paragraph1.AppendLine(string.Format("{0}", keyvalue.Value), Brushes.Gray, Brushes.White);
                                paragraph2.AppendLine(string.Format("{0}", file2[section.Key][keyvalue.Key]), Brushes.Gray, Brushes.White);
                            }
                        }
                        else
                        {
                            //this key is only in the first section
                            paragraph1.AppendLine(string.Format("{0} = {1}", keyvalue.Key, keyvalue.Value), Brushes.LightPink);
                            paragraph2.AppendLine();
                        }
                    }
                    foreach (var keyvalue in file2[section.Key])
                    {
                        if (matchedKeys.Contains(keyvalue.Key) || missedKeysAccountedFor.Contains(keyvalue.Key))
                            continue;
                        
                        //if it got here then this key is not in the first INI, display it as added

                        //TODO: make this DRY, this exact code is used above for missing keys in-between other keys

                        paragraph1.AppendLine();
                        paragraph2.AppendLine(string.Format("{0} = {1}", keyvalue.Key, keyvalue.Value), Brushes.LightGreen);

                        missedKeysAccountedFor.Add(keyvalue.Key); //don't think it matters at this point
                    }
                    paragraph1.AppendLine();
                    paragraph2.AppendLine();
                }
                else
                {
                    //this section is not in the second INI, display it as removed

                    if (section.Key.Length > 0)
                    {
                        paragraph1.AppendLine(string.Format("[{0}]", section.Key), Brushes.LightPink, bold: true);
                        paragraph2.AppendLine();
                    }
                    foreach (var keyvalue in section.Value)
                    {
                        //none of the lines can match
                        paragraph1.AppendLine(string.Format("{0} = {1}", keyvalue.Key, keyvalue.Value), Brushes.LightPink);
                        paragraph2.AppendLine();
                    }
                    paragraph1.AppendLine();
                    paragraph2.AppendLine();
                }
            }
            foreach (var section2 in file2)
            {
                if (matchedSections.Contains(section2.Key) || missedSectionsAccountedFor.Contains(section2.Key))
                    continue;

                //if it got here then this section is not in the first INI, display it as added

                //TODO: make this DRY, this exact code is used above for missing sections in-between other sections

                if (section2.Key.Length > 0)
                {
                    paragraph1.AppendLine();
                    paragraph2.AppendLine(string.Format("[{0}]", section2.Key), Brushes.LightGreen, bold: true);
                }
                foreach (var keyvalue in section2.Value)
                {
                    //none of the lines can match
                    paragraph1.AppendLine();
                    paragraph2.AppendLine(string.Format("{0} = {1}", keyvalue.Key, keyvalue.Value), Brushes.LightGreen);
                }
                paragraph1.AppendLine();
                paragraph2.AppendLine();

                missedSectionsAccountedFor.Add(section2.Key);
            }

            INI1RichTextBox.Document = new FlowDocument(paragraph1);
            INI2RichTextBox.Document = new FlowDocument(paragraph2);
        }

        private void Uncompare_Click(object sender, RoutedEventArgs e)
        {
            ShowUnCompared();
            //CalculatePermawidth();
        }

        //int ini1PermaWidth;

        private void CalculatePermawidth()
        {
            //INI1RichTextBox.Document.PageWidth = 100000;
            //double pageHeight = INI1RichTextBox.Document.PageHeight;
            //double actualHeight = INI1RichTextBox.ActualHeight;

        }

        private void INI1RichTextBox_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //double height = INI1RichTextBox.Document.PageWidth = INI1RichTextBox.ExtentWidth;
        }

        private void INI2RichTextBox_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        }

        private void INIRichTextBox_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (!InComparison)
                return;
            var textToSync = (sender == INI1RichTextBox) ? INI2RichTextBox : INI1RichTextBox;
            textToSync.ScrollToVerticalOffset(e.VerticalOffset);
            //textToSync.ScrollToHorizontalOffset(e.HorizontalOffset);
        }
    }
}

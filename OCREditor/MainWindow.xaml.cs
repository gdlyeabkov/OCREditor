using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Shapes;
using Windows.Globalization;
using Windows.Graphics.Imaging;
using Windows.Media.Ocr;

namespace OCREditor
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Initialize();
        
        }

        public void Initialize ()
        {
            
        }

        public void RecognizeHandler (object sender, RoutedEventArgs e)
        {
            Recognize();
        }

        public async void Recognize ()
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.Title = "Выберите изображение для распознавания";
            ofd.Filter = "Png documents (.png)|*.png";
            bool? res = ofd.ShowDialog();
            bool isOpened = res != false;
            if (isOpened)
            {
                string path = ofd.FileName;
                string languageCode = "ru";
                /*
                Language language = new Language(languageCode);
                OcrEngine ocrEngine = OcrEngine.TryCreateFromLanguage(language);
                */
                OcrEngine ocrEngine = OcrEngine.TryCreateFromUserProfileLanguages();
                using (var fileStream = File.OpenRead(path))
                {
                    var randomAccessStream = fileStream.AsRandomAccessStream();
                    var imageDecoder = await Windows.Graphics.Imaging.BitmapDecoder.CreateAsync(randomAccessStream);
                    SoftwareBitmap softwareBmp = await imageDecoder.GetSoftwareBitmapAsync();
                    var ocrResult = await ocrEngine.RecognizeAsync(softwareBmp);
                    var recognizedTexts = ocrResult.Lines;
                    int countRecognizedTexts = recognizedTexts.Count;
                    bool isHaveResults = countRecognizedTexts >= 1;
                    if (isHaveResults)
                    {
                        recognizedTextBox.Text = "";
                        foreach (OcrLine recognizedText in recognizedTexts)
                        {
                            string recognizedTextLabelContent = recognizedText.Text;
                            recognizedTextBox.Text += recognizedTextLabelContent;
                            int recognizedTextIndex = recognizedTexts.ToList().IndexOf(recognizedText);
                            bool isAddNewLine = recognizedTextIndex < countRecognizedTexts;
                            if (isAddNewLine)
                            {
                                recognizedTextBox.Text += System.Environment.NewLine;
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Не удалось распознать текст на этом изображении попробуйте открыть другое.", "Внимание");
                    }
                }
            }
        }

        private void ExportToTextFileHandler(object sender, RoutedEventArgs e)
        {
            ExportToTextFile();
        }

        public void ExportToTextFile()
        {
            Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog();
            sfd.Title = "Выберите папку для экспорта распознанного текста";
            sfd.FileName = "index.txt";
            sfd.DefaultExt = ".chapter";
            sfd.Filter = "Text documents (.txt)|*.txt";
            bool? res = sfd.ShowDialog();
            bool isSaved = res != false;
            if (isSaved)
            {
                string path = sfd.FileName;
                string recognizedTextBoxTextContent = recognizedTextBox.Text;
                File.WriteAllText(path, recognizedTextBoxTextContent);
            }
        }

    }
}

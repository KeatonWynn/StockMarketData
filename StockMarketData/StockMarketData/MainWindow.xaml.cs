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
using System.Windows.Shapes;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;


namespace StockMarketData
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // variables
        private static readonly string userHMF = Environment.UserName;
        private static readonly string download_dir = "C:/Users/" + userHMF + "/Downloads";
        private static readonly string chromedriver_dir = "C:/Users/" + userHMF + "/ImportFiles/";
        private static readonly string date_format = "MM/dd/yyyy h:mm tt";
        private string selected_folder;


        //empties textbox upon click
        public void Textbox_FileName_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
            tb.GotFocus -= Textbox_FileName_GotFocus;
        }

        //Download Directory button click action 
        public void SelectDirectory_Button_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = download_dir;
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                //if folder selected, will add more functionality later
                selected_folder = dialog.FileName;
            }
            SelectDirectory_Button.Content = selected_folder;
        }
        
        //Destination Directory button click action
        public void SelectDestination_Button_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = download_dir;
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                //if folder selected, will add more functionality later
                selected_folder = dialog.FileName;
            }
            SelectDestination_Button.Content = selected_folder + "\\" + Textbox_FileName.Text;
        }

        private void StartDownload_Button_Click(object sender, RoutedEventArgs e)
        {

            string StockText = Stock_TextBox.Text.ToUpper();
            string Selected_Website = ComboBoxWebsite.Text;
            string yahoo_url = $"https://finance.yahoo.com/quote/{StockText}/history?p={StockText}";
            Log_TextBox.AppendText($"\n" + DateTime.Now.ToString(date_format) + ": Fetching data for " + StockText);

            SeleniumEnhanced.SeleniumEnhanced SE = new SeleniumEnhanced.SeleniumEnhanced();
            SE.StartWebDriver();            


            switch (Selected_Website)
            {
                case "Yahoo":
                    Console.WriteLine($"Starting {StockText} Download");
                    SE.driver.Navigate().GoToUrl(yahoo_url);
                    Console.WriteLine($"Downloading File");
                    System.Threading.Thread.Sleep(5000);
                    SE.GetFunction("Click", "//*[@id=\"Col1-1-HistoricalDataTable-Proxy\"]/section/div[1]/div[2]/span[2]/a/span", "Download");
                    SE.Quit();
                    break;
                default:
                    Console.WriteLine("No option selected");
                    break;
            

            }

            Log_TextBox.AppendText($"\n" + DateTime.Now.ToString(date_format) + ": Data download complete");


        }
    }
}

using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;


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
        private string selected_download_folder;
        private string selected_dest_folder;


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
                selected_download_folder = dialog.FileName;
            }
            SelectDirectory_Button.Content = selected_download_folder;
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
                selected_dest_folder = dialog.FileName;
            }
            SelectDestination_Button.Content = selected_dest_folder;
        }

        private void StartDownload_Button_Click(object sender, RoutedEventArgs e)
        {

            string StockText = Stock_TextBox.Text.ToUpper().Replace(" ", string.Empty);
            string Selected_Website = ComboBoxWebsite.Text;
            string yahoo_url = $"https://finance.yahoo.com/quote/{StockText}/history?p={StockText}";
            bool completed_flag = false;
            Log_TextBox.AppendText($"\n" + DateTime.Now.ToString(date_format) + ": Fetching data for " + StockText);
            Log_TextBox.ScrollToEnd();

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
                    completed_flag = true;
                    break;
                default:
                    Console.WriteLine("No option selected");
                    break;
            

            }
            if (completed_flag)
            {
                var get_last_mod_file = Directory.GetFiles(download_dir)
                                    .Select(f => new FileInfo(f))
                                    .OrderByDescending(fi => fi.LastWriteTime)
                                    .First()
                                    .FullName; // get last modified file

                string last_mod_file = (string)get_last_mod_file;
                string file_extension = last_mod_file.Substring((last_mod_file.Length - 4), 4); // dynamically get file extension


                File.Copy(last_mod_file, selected_dest_folder + "\\" + Textbox_FileName.Text + file_extension, true);

                Log_TextBox.AppendText($"\n" + DateTime.Now.ToString(date_format) + ": Data download complete");
                Log_TextBox.ScrollToEnd();
                //Log_TextBox.AppendText((string)mostRecentlyModified);
            }

            else
            {
                Log_TextBox.AppendText($"\n" + DateTime.Now.ToString(date_format) + ": ***ERROR*** Fetching data for " + StockText );
            }

        }

        private void Exit_Button_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}

using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.IO;
using System.Linq;
using System.Threading;
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

            //xpath variables
            string xpath_datepicker_filter = "//*[@id=\"Col1-1-HistoricalDataTable-Proxy\"]/section/div[1]/div[1]/div[1]/span[2]/span/input";
            string xpath_datepicker_start = "//*[@id=\"Col1-1-HistoricalDataTable-Proxy\"]/section/div[1]/div[1]/div[1]/span[2]/div/input[1]";
            string xpath_datepicker_end = "//*[@id=\"Col1-1-HistoricalDataTable-Proxy\"]/section/div[1]/div[1]/div[1]/span[2]/div/input[2]";
            string xpath_download_file_link = "//*[@id=\"Col1-1-HistoricalDataTable-Proxy\"]/section/div[1]/div[2]/span[2]/a/span";
            string xpath_datepicker_done = "//*[@id=\"Col1-1-HistoricalDataTable-Proxy\"]/section/div[1]/div[1]/div[1]/span[2]/div/div[3]/button[1]";


            string StockText = Stock_TextBox.Text.ToUpper().Replace(" ", string.Empty);
            string Selected_Website = ComboBoxWebsite.Text;
            string yahoo_url = $"https://finance.yahoo.com/quote/{StockText}/history?p={StockText}";
            string file_name = Textbox_FileName.Text;
            string datepicker_start_value, datepicker_end_value;
            bool datepicker_start_flag, datepicker_end_flag, completed_flag = false;
                       
            //DatePicker flags and values
            datepicker_start_flag = StartDate_DatePicker != null ? true : false;
            datepicker_end_flag = EndDate_DatePicker != null ? true : false;
            datepicker_start_value = StartDate_DatePicker != null ? StartDate_DatePicker.Text : null;
            datepicker_end_value = EndDate_DatePicker != null ? EndDate_DatePicker.Text : null;

            Log_TextBox.AppendText($"\n" + DateTime.Now.ToString(date_format) + ": Fetching data for " + StockText);
            Log_TextBox.ScrollToEnd();
            Thread trd = new Thread(() =>
            {
                SeleniumEnhanced.SeleniumEnhanced SE = new SeleniumEnhanced.SeleniumEnhanced();
                SE.StartWebDriver();


                switch (Selected_Website)
                {
                    case "Yahoo":
                        Console.WriteLine($"Starting {StockText} Download");
                        SE.driver.Navigate().GoToUrl(yahoo_url);
                        Console.WriteLine($"Downloading File");
                        System.Threading.Thread.Sleep(5000);

                        //if both dates are selected, filter for date range
                        if (datepicker_start_flag == true && datepicker_end_flag == true)
                        {
                            //datepicker option
                            SE.GetFunction("Click", xpath_datepicker_filter, "DatePricker");

                            //set start date values
                            SE.GetFunction("Click", xpath_datepicker_start, "Start Date");
                            SE.GetFunction("Clear", xpath_datepicker_start, "Start Date");
                            SE.GetFunction("SendKeys", xpath_datepicker_start, "Start Date", datepicker_start_value);

                            //set end date values
                            SE.GetFunction("Click", xpath_datepicker_end, "End Date");
                            SE.GetFunction("Clear", xpath_datepicker_end, "End Date");
                            SE.GetFunction("SendKeys", xpath_datepicker_end, "End Date", datepicker_end_value);

                            //done button
                            SE.GetFunction("Click", xpath_datepicker_done, "Done button");

                        }


                        SE.GetFunction("Click", xpath_download_file_link, "Download");
                        System.Threading.Thread.Sleep(3000);
                        SE.Quit();
                        completed_flag = true;
                        break;
                    default:
                        Console.WriteLine("No option selected");
                        break;


                }
             
            if (completed_flag == true)
                {
                    var get_last_mod_file = Directory.GetFiles(download_dir)
                                        .Select(f => new FileInfo(f))
                                        .OrderByDescending(fi => fi.LastWriteTime)
                                        .First()
                                        .FullName; // get last modified file

                    string last_mod_file = (string)get_last_mod_file;
                    string file_extension = last_mod_file.Substring((last_mod_file.Length - 4), 4); // dynamically get file extension


                    System.IO.File.Copy(last_mod_file, selected_dest_folder + "\\" + file_name + file_extension, true);

                    //will figure out later
                    /*
                    Log_TextBox.AppendText($"\n" + DateTime.Now.ToString(date_format) + ": Data download complete");
                    Log_TextBox.ScrollToEnd();

                    if (completed_flag != true)
                    {
                        Log_TextBox.AppendText($"\n" + DateTime.Now.ToString(date_format) + ": ***ERROR*** Fetching data for " + StockText);
                    }
                    */
                }

               
            });

           

            //checks if website is selected and stock value is non-null value before starting thread
            if (StockText.Length > 0 && Selected_Website != "Select Website")
            {
                trd.Start();
            }

            
            
        }

        private void Exit_Button_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}

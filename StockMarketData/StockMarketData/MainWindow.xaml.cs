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

        string SelectFolderText;
        
        //empties textbox upon click
        public void Textbox_FileName_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
            tb.GotFocus -= Textbox_FileName_GotFocus;
        }

        //Select Directory button click action 
        public void SelectDirectory_Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog o = new OpenFileDialog();

        }
        

        private void StartDownload_Button_Click(object sender, RoutedEventArgs e)
        {
            SeleniumEnhanced.SeleniumEnhanced SE = new SeleniumEnhanced.SeleniumEnhanced();
            SE.StartWebDriver();

            string StockText = Stock_TextBox.Text.ToUpper();
            string Selected_Website = ComboBoxWebsite.Text;
            string yahoo_url = $"https://finance.yahoo.com/quote/{StockText}/history?p={StockText}" ;


            switch (Selected_Website)
            {
                case "Yahoo":
                    Console.WriteLine($"Starting {StockText} Download");
                    SE.driver.Navigate().GoToUrl(yahoo_url);
                    Console.WriteLine($"Downloading File");
                    SE.GetFunction("Click", "//*[@id=\"Col1-1-HistoricalDataTable-Proxy\"]/section/div[1]/div[2]/span[2]/a/span", "Download");
                    SE.Quit();
                    break;
                default:
                    Console.WriteLine("No option selected");
                    break;
            

            }
                
                


        }
    }
}

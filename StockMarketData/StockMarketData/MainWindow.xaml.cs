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

        

        public void Textbox_FileName_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
            tb.GotFocus -= Textbox_FileName_GotFocus;
        }

        private void StartDownload_Button_Click(object sender, RoutedEventArgs e)
        {
            SeleniumEnhanced.SeleniumEnhanced SE = new SeleniumEnhanced.SeleniumEnhanced();
            SE.StartWebDriver();

            string StockText = Stock_TextBox.Text.ToUpper();
            string Selected_Website = ComboBoxWebsite.Text;


            switch (Selected_Website)
            {
                case "Yahoo":
                    SE.driver.Navigate().GoToUrl("https://finance.yahoo.com/quote/SPY/history?p=SPY");
                    break;
                default:
                    Console.WriteLine("No option selected");
                    break;
            

            }
                
                


        }
    }
}

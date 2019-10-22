using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SeleniumEnhanced
{
    class SeleniumEnhanced
    {

        private static string userHMF = Environment.UserName;
        private static string download_dir = "C:/Users/" + userHMF + "/Downloads";
        private static string chromedriver_dir = "C:/Users/" + userHMF + "/ImportFiles/";
        public ChromeDriver driver;
        private const int def_max_tries = 10;
        private const int def_sec_to_wait = 2;
        private const bool def_show_message = true;
        private const bool def_exit_on_error = true;
        private bool failure_flag = false;
                
        
        public void StartWebDriver(bool headless = false)
        {
            try
            {
                var chromeOptions = new ChromeOptions();
                chromeOptions.AddArguments("headless");

                //toggle headless here
                driver = headless == true ? new ChromeDriver(chromedriver_dir, chromeOptions) : new ChromeDriver(chromedriver_dir);
                
                driver.Manage().Window.Maximize();
                
            }
            
            catch(OpenQA.Selenium.DriverServiceNotFoundException)
            {
                Console.WriteLine("File not found - chromedriver.exe missing from directory below:");
                Console.WriteLine(chromedriver_dir);
                Console.WriteLine("Press any key to exit...");
                System.Environment.Exit(1);

            }
              
        }

        public void Quit()
        {
            try
            {
                driver.Close();
                driver.Quit();
            }

            catch (OpenQA.Selenium.WebDriverException)
            {
                //just continue
            }
        }

        public string GetText(string xpath)
        {
            string text = driver.FindElementByXPath(xpath).Text;
            return text;
        }

        public void Click(string xpath)
        {
            driver.FindElementByXPath(xpath).Click();
        }

        public void SendKeys(string xpath, string keys_to_send)
        {
            driver.FindElementByXPath(xpath).SendKeys(keys_to_send);
        }


        public string GetFunction(
            string function, 
            string xpath, 
            string catchoutput, 
            string keys_to_send = null, 
            int max_tries = def_max_tries, 
            bool show_messages = true, 
            int seconds_to_wait = def_sec_to_wait, 
            bool exit_on_error = true
            )
        {
            string txt = null; // there might be a better solution, this will return null if there is no text that needs to be extracted
            int wait = 1000 * seconds_to_wait;
            string function_clean = function.ToUpper().Replace(" ", "");


            for (int tries = 1; tries <= max_tries && failure_flag != true; tries++)
            {
                try
                {
                    switch (function_clean)
                    {
                        case "CLICK":
                            driver.FindElementByXPath(xpath).Click();
                            break;
                        case "SENDKEYS":
                            driver.FindElementByXPath(xpath).SendKeys(keys_to_send);
                            break;
                        case "GETTEXT":
                            txt = driver.FindElementByXPath(xpath).Text;
                            break;
                        case "CLEAR":
                            driver.FindElementByXPath(xpath).Clear();
                            break;
                        default:
                            Console.WriteLine("Warning: function not found.");
                            break;

                    }
                    break; //exit loop
                }
                catch (OpenQA.Selenium.NoSuchElementException)
                {
                    if (show_messages)
                    {
                        Console.WriteLine(catchoutput + ": try " + (tries) + " out of " + max_tries);
                        Thread.Sleep(wait);
                    }

                    if (tries == max_tries)
                    {
                        if (exit_on_error)
                        {
                            if (show_messages)
                            {
                                Console.WriteLine("STOPPING PROCESS - FATAL ERROR: " + catchoutput + " | " + "Action:" + function_clean);
                            }
                            Quit();
                        }
                    }
                }
                catch (OpenQA.Selenium.WebDriverException)
                {
                    //will figure out later, once driver is closed it'll continute to run
                }
            }
            return txt;
        }


    }
}
        


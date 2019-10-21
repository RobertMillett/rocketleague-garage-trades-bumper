using OpenQA.Selenium.Chrome;
using System.Threading;
using System.Collections.Generic;
using System;
using System.Configuration;

namespace TradesBumper
{
    class Program
    {
        static void Main(string[] args)
        {            
            var name = GetUserDetails(SettingTypes.Name);

            var emailAddress = GetUserDetails(SettingTypes.EmailAddress);

            var password = GetUserDetails(SettingTypes.Password);

            Console.WriteLine($"Continue with the following details? \nName: {name}\nEmail Address: {emailAddress}\nPassword: {password}\ny/n");
            var confirmation = Console.ReadLine();
            if (confirmation != "y")
            {
                Console.WriteLine("Program will exit...\n\nClear all settings before exit? y/n");

                var clearSetttings = Console.ReadLine();

                if (clearSetttings == "y")
                {
                    Properties.Settings.Default.Name = "";
                    Properties.Settings.Default.EmailAddress = "";
                    Properties.Settings.Default.Password = "";
                    Properties.Settings.Default.Save();
                }

                Environment.Exit(0);
            }
            
            ChromeOptions options = new ChromeOptions();
            options.AddUserProfilePreference("profile.default_content_setting_values.images", 2);

            var driver = new ChromeDriver(options)
            {
                Url = $"https://rocket-league.com/trades/{name}"
            };

            driver.Manage().Timeouts().PageLoad = new TimeSpan(0, 10, 0);

            try
            {
                driver.FindElementById("acceptPrivacyPolicy").Click();

                driver.FindElementById("header-email").SendKeys(emailAddress);
                driver.FindElementById("header-password").SendKeys(password);
                driver.FindElementByClassName("rlg-btn-primary").Click();

                var editLinks = driver.FindElementsByXPath("//a[contains(text(),'Edit trade')]");

                var editUrls = new List<string>();
                foreach (var link in editLinks)
                {
                    editUrls.Add(link.GetAttribute("href"));
                }

                foreach (var url in editUrls)
                {
                    driver.Url = url;
                    driver.FindElementByCssSelector(".rlg-btn-trade-form.g-recaptcha.rlg-btn-primary.prevent").Click();
                    Thread.Sleep(15000);
                }

                driver.Quit();
            }
            catch (Exception ex)
            {
            }
        }

        public static string GetUserDetails(SettingTypes settingType)
        {
            string result = null;

            switch (settingType)
            {
                case SettingTypes.Name:
                    result = Properties.Settings.Default.Name;
                    break;
                case SettingTypes.EmailAddress:
                    result = Properties.Settings.Default.EmailAddress;
                    break;
                case SettingTypes.Password:
                    result = Properties.Settings.Default.Password;
                    break;
            }

            while (string.IsNullOrEmpty(result))
            {
                Console.WriteLine($"Please enter your RocketLeague-Garage Account {Enum.GetName(typeof(SettingTypes), settingType)}");

                if (settingType == SettingTypes.Name)
                    Console.WriteLine("This is at the end of the URL when you visit My Trades");

                result = Console.ReadLine();

                Console.WriteLine($"You've entered: '{result}'. Is this correct? y/n");

                var confirmationInput = Console.ReadLine();

                if (confirmationInput.ToLower() == "y")
                {
                    switch(settingType)
                    {
                        case SettingTypes.Name:
                            Properties.Settings.Default.Name = result;
                            Properties.Settings.Default.Save();
                            break;
                        case SettingTypes.EmailAddress:
                            Properties.Settings.Default.EmailAddress = result;
                            Properties.Settings.Default.Save();
                            break;
                        case SettingTypes.Password:
                            Properties.Settings.Default.Password = result;
                            Properties.Settings.Default.Save();
                            break;
                    }

                    break;
                }

                result = null;
            }

            return result;
        }
    }
}

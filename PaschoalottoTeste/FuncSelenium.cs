using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;

namespace PaschoalottoTeste
{
    internal class FuncSelenium
    {
        public static EdgeDriver OpenNavigator()
        {
            var edgeOptions = new EdgeOptions();
            var edgeDriverPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"EdgeWebDriver\"); // Caminho para o MicrosoftWebDriver.exe
            var driver = new EdgeDriver(edgeDriverPath, edgeOptions);

            return driver;
        }

        

    }
}

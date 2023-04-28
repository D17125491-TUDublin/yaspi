namespace yaspi.test.ui;
using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;

public class T001_LoginTest
{
    protected static string URL = "https://127.0.0.1:7144/";
    protected string _username = "NXESIVWEVUUOLSMRIV@TPWLB.COM";
    protected string _password = "Password.123";

    protected IWebDriver _getChromeDriver()
    {
        ChromeOptions options = new ChromeOptions();
        options.AddArgument("--ignore-certificate-errors");
        var driver = new ChromeDriver(options);

        return driver;
    }
    protected IWebDriver _getFirefoxDriver()
    {
        FirefoxOptions options = new FirefoxOptions();
        options.AddAdditionalOption("acceptInsecureCerts", true);
        var driver = new FirefoxDriver(options);
        return driver;
    }

    private IWebDriver _getEdgeDriver()
    {
        EdgeOptions options = new EdgeOptions();
        options.AcceptInsecureCertificates = true;
        var driver = new EdgeDriver(options);
        return driver;
    }

   

    [Fact]
    public void ChromeTest()
    {
        var driver = _getChromeDriver();
        try
        {
            performTest(driver);
        }
        finally
        {
            driver.Quit();
        }
    }

    private void performTest(IWebDriver driver)
    {
        performLogin(driver);
        Assert.Contains("Home Page", driver.Title);
    }

    [Fact]
    public void FirefoxTest()
    {
        var driver = _getFirefoxDriver();
        try
        {
            performTest(driver);
        }
        finally
        {
            driver.Quit();
        }
    }

    [Fact]
    public void EdgeTest()
    {
        var driver = _getEdgeDriver();
        try
        {
            performTest(driver);
        }
        finally
        {
            driver.Quit();
        }
    }

    protected void performLogin(IWebDriver driver)
    {
        driver.Navigate().GoToUrl(URL);
        var emailInput = driver.FindElement(By.Id("Input_Email"));
        var passwordInput = driver.FindElement(By.Id("Input_Password"));
        var loginButton = driver.FindElement(By.Id("login-submit"));
        emailInput.SendKeys(_username);
        passwordInput.SendKeys(_password);
        loginButton.Click();
        Thread.Sleep(1000);
        driver.FindElement(By.Id("logout-button"));
    }
}

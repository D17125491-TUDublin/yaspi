namespace yaspi.test.ui;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;

public class T002_SendMessageTest 
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
            performLoginTest(driver);
        }
        finally
        {
            driver.Quit();
        }
    }

    [Fact]
    public void FirefoxTest()
    {
        var driver = _getFirefoxDriver();
        try
        {
            performLoginTest(driver);
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
            performLoginTest(driver);
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
    }
    private void performLoginTest(IWebDriver driver)
    {
        performLogin(driver);
        var messageInput = driver.FindElement(By.Id("message-input"));
        var sendButton = driver.FindElement(By.Id("Submit"));
        messageInput.SendKeys("Hello World");
        sendButton.Click();
        Thread.Sleep(1000);
        var result = driver.FindElement(By.Id("result-message"));
        Assert.Contains("Message sent successfully!", result.Text);
    }
}

namespace yaspi.test.ui;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;

public class SeleniumCheck
{
    // [Fact]
    // public void ChromeTest()
    // {
    //     using var driver = new ChromeDriver();
    //     driver.Navigate().GoToUrl("https://www.google.com");
    //     var searchBox = driver.FindElement(By.Name("q"));
    //     var acceptButton = driver.FindElement(By.Id("L2AGLb"));
    //     acceptButton.Click();
    //     searchBox.SendKeys("Selenium");
    //     searchBox.Submit();

    //     Assert.Contains("Selenium", driver.Title);
    // }

    // [Fact]
    // public void FirefoxTest()
    // {
    //     using var driver = new FirefoxDriver();
        
    //     driver.Navigate().GoToUrl("https://www.google.com");
    //     var searchBox = driver.FindElement(By.Name("q"));
    //     var acceptButton = driver.FindElement(By.Id("L2AGLb"));
    //     acceptButton.Click();
    //     searchBox.SendKeys("Selenium");
    //     searchBox.Submit();
    //     Thread.Sleep(2000);
    //     Assert.Contains("Selenium", driver.Title);
    // }

    // [Fact]
    // public void EdgeTest()
    // {
    //     using var driver = new EdgeDriver();
    //     driver.Navigate().GoToUrl("https://www.google.com");
    //     var searchBox = driver.FindElement(By.Name("q"));
    //     var acceptButton = driver.FindElement(By.Id("L2AGLb"));
    //     acceptButton.Click();
    //     searchBox.SendKeys("Selenium");
    //     searchBox.Submit();

    //     Assert.Contains("Selenium", driver.Title);
    // }
}

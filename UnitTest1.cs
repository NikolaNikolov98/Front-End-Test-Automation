using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;


namespace TestProject
{
    public class Tests
    {
        private IWebDriver driver;

        private readonly string BaseUrl = "https://d1dzr3dh7g0qgk.cloudfront.net/";

        private string TaskName;

        private string TaskDescription;

        private Random random;

        [OneTimeSetUp]
        public void Setup()
        {
            random = new Random();
            driver = new FirefoxDriver();

            /*
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddUserProfilePreference("password_manager_enabled", false);
            chromeOptions.AddUserProfilePreference("credentials_enable_service", false);
            chromeOptions.AddUserProfilePreference("profile.password_manager_leak_detection", false);
            chromeOptions.AddUserProfilePreference("profile.password_manager_enabled", false);
            chromeOptions.AddArgument("--disable-save-password-bubble");
            driver = new ChromeDriver(chromeOptions);*/

            driver.Manage().Window.Maximize();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(15);


            //Login to the system
            driver.Navigate().GoToUrl(BaseUrl + "User/LoginRegister");
            driver.FindElement(By.XPath("//a[text()='Login']")).Click();


            driver.FindElement(By.XPath("//input[@type='email']")).SendKeys("Nikola@Nikola");
            driver.FindElement(By.XPath("//input[@type='password']")).SendKeys("Nikola");

            driver.FindElement(By.XPath("//button[@class='btn btn-primary btn-block mb-4']")).Click();
        }

        [Test, Order(1)]
        public void AddTaskWithoutNameTest()
        {
            //Arrange
            
            TaskDescription = "Descriptionnnnnnnnnnnnnnnnnnnnnnnnnnnnnnn_" + random.Next(999, 99999).ToString();


            //driver.Navigate().GoToUrl($"{BaseUrl}/Task/ToDo");
            driver.Navigate().GoToUrl(BaseUrl + "Task/ToDo");
            driver.FindElement(By.XPath("//a[text()='Create Task']")).Click();

            //Act
            driver.FindElement(By.XPath("//input[@name='TaskName']")).SendKeys("");
            driver.FindElement(By.CssSelector("[name='Description']")).SendKeys(TaskDescription);

            driver.FindElement(By.XPath("//input[@name='StartDate']")).SendKeys("02/12/2025 10:37");
            driver.FindElement(By.XPath("//input[@name='EndDate']")).SendKeys("31/12/2025 10:37");
            driver.FindElement(By.XPath("//select[@name='Status']")).SendKeys("To-Do");


            driver.FindElement(By.XPath("//button[@type='submit']")).Click();

            //Assert
            Assert.That(driver.Url, Is.EqualTo(BaseUrl + "Task/Create"));

           
        }
        
        [Test, Order(2)]
        public void AddTaskWithRandomNameTest()
        {
            //Arrange
            
            TaskName  = "Tilte_" + random.Next(999, 99999).ToString();



            //driver.Navigate().GoToUrl($"{BaseUrl}/Task/ToDo");
            driver.Navigate().GoToUrl(BaseUrl + "Task/ToDo");
            driver.FindElement(By.XPath("//a[text()='Create Task']")).Click();

            //Act
            driver.FindElement(By.XPath("//input[@name='TaskName']")).SendKeys(TaskName);
            driver.FindElement(By.CssSelector("[name='Description']")).SendKeys(TaskDescription);

            driver.FindElement(By.XPath("//input[@name='StartDate']")).SendKeys("02/12/2025 10:37");
            driver.FindElement(By.XPath("//input[@name='EndDate']")).SendKeys("31/12/2025 10:37");


            //driver.FindElement(By.XPath("//select[@name='Status']")).SendKeys("To-Do");
            var statusSelect = driver.FindElement(By.CssSelector("select[name='Status']"));
            var select = new SelectElement(statusSelect);
            select.SelectByValue("10");  // selects "To-Do"

            driver.FindElement(By.XPath("//button[@type='submit']")).Click();


            Assert.That(driver.Url, Is.EqualTo(BaseUrl + "Task/ToDo"));

            var lastCreatedTask = driver.FindElement(By.XPath("//div[@class='card text-center'][last()]//h5[@class='card-title']"));

            Assert.That(lastCreatedTask.Text, Is.EqualTo(TaskName));


           
          
        }
        
        [Test, Order(3)]
        public void EditLastAddedTaskTest()
        {
            //Arrange

           

            //driver.Navigate().GoToUrl($"{BaseUrl}/Task/ToDo");
            driver.Navigate().GoToUrl(BaseUrl + "Task/ToDo");
            
            var lastCreatedTask = driver.FindElement(By.XPath("//div[@class='card text-center'][last()]//a[text()='Edit']"));


            lastCreatedTask.Click();

            var editedName = TaskName + "Edited";



            driver.FindElement(By.XPath("//input[@name='TaskName']")).Clear();
            driver.FindElement(By.XPath("//input[@name='TaskName']")).SendKeys(editedName);
            driver.FindElement(By.XPath("//button[@type='submit']")).Click();

            Assert.That(driver.Url, Is.EqualTo(BaseUrl + "Task/ToDo"));


            var lastEditedTaskName = driver.FindElement(By.XPath("//div[@class='card text-center'][last()]//h5[@class='card-title']")).Text;

            
            Assert.That(lastEditedTaskName, Is.EqualTo(editedName));



        }
        
        [Test, Order(4)]
        public void MoveLastAddedTaskTest()
        {
            //Arrange
            //driver.Navigate().GoToUrl($"{BaseUrl}/Task/ToDo");
            driver.Navigate().GoToUrl(BaseUrl + "Task/ToDo");

           var lastCreatedTaskName = driver.FindElement(By.XPath("//div[@class='card text-center'][last()]//h5[@class='card-title']")).Text;


            var inProgersStatus = driver.FindElement(By.XPath("//div[@class='card text-center'][last()]//a[@class='btn btn-primary']"));
            inProgersStatus.Click();

            Assert.That(driver.PageSource.Contains(lastCreatedTaskName), Is.False);

           
        }
        
        [Test, Order(5)]
        public void DeleteLastAddedTaskTest()
        {
            driver.Navigate().GoToUrl(BaseUrl + "Task/InProgress ");

            var lastMovedTask_Name = driver.FindElement(By.XPath("//div[@class='card text-center'][last()]//h5[@class='card-title']")).Text;

            var lastMovedTaskDeleteButton = driver.FindElement(By.XPath("//div[@class='card text-center'][last()]//a[@class='btn btn-danger']"));
            lastMovedTaskDeleteButton.Click();

            //div[@class='card text-center'][last()]//a[@class='btn btn-info']

            driver.FindElement(By.XPath("//button[@type='submit']")).Click();
            //wait

              var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
              var TimeWiseElement = wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//a[@class='navbar-brand']//span[text()='TimeWise']")));


            Assert.That(driver.PageSource.Contains(lastMovedTask_Name), Is.False);






        }
    
       

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            driver.Quit();
            driver.Dispose();
        }
    }
}
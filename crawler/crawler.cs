using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows.Forms;


namespace crawler
{
    public partial class crawler : Form
    {

        volatile bool working ;
        volatile bool nextthread = true;
        volatile bool nextkeyword = false;
        volatile bool chrome = true;
        List<Keywords> KeywordList = new List<Keywords>();
        List<SelfThread>   ThreadList  = new List<SelfThread>();
        System.Globalization.CultureInfo fair = new System.Globalization.CultureInfo("fa-IR");
        public crawler()
        {
            InitializeComponent();
        }
        
        private void crawler_Load(object sender, EventArgs e)
        {
            foreach (Process Proc in Process.GetProcesses()) 
                if (Proc.ProcessName.Contains("chromedriver")|| Proc.ProcessName.Contains("geckodriver"))  
                    Proc.Kill();

            //this.TopMost = true;
            if (CheckForInternetConnection())
            {
                lblip.Text = GetPublicIP();               
                refreshgrid2();
                refreshgrid1();
         
            }
            else
            {
                lblip.Text = "You Are Not have Internet Access Please Connect To Internet and Retry";
                button1.Enabled = false;
                button2.Enabled = false;
            }

        }
        private static bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                using (client.OpenRead("http://google.com/generate_204"))
                    return true;
            }
            catch
            {
                return false;
            }
        }

        private static string GetPublicIP()
        {
            try
            {

                return new System.Net.WebClient().DownloadString("https://ipinfo.io/ip").Replace("\n", "");
            }
            catch
            {

                return "You Are Not have Internet Access Please Connect To Internet and Retry";
            }

        }
        private void refreshgrid2()
        {
            while(true)
            {
                string jsonpath = Environment.CurrentDirectory + @"\keyword.json";
                FileInfo f = new FileInfo(jsonpath);
                if (!IsFileLocked(f))
                {
                    string json = File.ReadAllText(jsonpath);
                    KeywordList.Clear();
                    KeywordList = JsonConvert.DeserializeObject<List<Keywords>>(json);
                    dynamic dynamicObject = JsonConvert.DeserializeObject(json);
                    dataGridView2.DataSource = dynamicObject;
                    dataGridView2.Refresh();
                    break;
                }
                else
                {
                    Thread.Sleep(20000);
                }
            }
        }

        private void refreshgrid1()
        {          
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(ThreadList);
            dynamic a = JsonConvert.DeserializeObject(json);            
            dataGridView1.DataSource = a;           
        }
        
        private void writegrid2()
        {
            try
            {
                start:
                string jsonpath = Environment.CurrentDirectory + @"\keyword.json";
                FileInfo f = new FileInfo(jsonpath);
                if (!IsFileLocked(f)&&KeywordList.Count>0)
                {
                    string dynamic = JsonConvert.SerializeObject(KeywordList);
                    System.IO.File.WriteAllText(jsonpath, dynamic);
                }
                else
                {
                    Thread.Sleep(20000);
                    goto start;
                }
               
            }
           
            catch
            {

            }
        }
        private void Elapsedtime()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            while (1 == 1)
            {
                TimeSpan ts = stopWatch.Elapsed;
                lblelapsedtime.Text = string.Format("{0}:{1}:{2}", ts.Hours, ts.Minutes, ts.Seconds);
                Thread.Sleep(1000);
            }

        }

        public DateTime startd;
        public List<Thread> threads = new List<Thread>();
        private void button1_Click(object sender, EventArgs e)
        {
            working = true;
            startd = DateTime.Now;
            lblstarttime.Text = DateTime.Now.ToString();
            timer2.Start();
            Thread SelfThread = new Thread(start);
            threads.Add(SelfThread);
            SelfThread.Start();
            timer1.Start();
        }
        private void start()
        {
            while (working)
            {
            start:
               
                foreach (Keywords key in KeywordList.OrderBy(c => c.count))
                {
                    Pages.key = key;
                    Pages.Row = 1;
                    Pages.Page = 1;
                    Pages.TotalRow = 1;


                    while (!nextkeyword)
                    {
                        if (Pages.key == key && Pages.Page > 10)
                        {
                            nextkeyword = true;
                        }
                        if (nextthread == true && ThreadList.Where(c => c.Status == "Running").Count() <= 5)
                        {
                            ParameterizedThreadStart threadStart = new ParameterizedThreadStart(startthread);
                            Thread SelfThread = new Thread(threadStart);
                            SelfThread.Start(new PrintNumberParameters() { keywords = key.keyword });

                        }
                        Thread.Sleep(5000);
                    }
                    KeywordList[key.ID - 1].count++;
                    writegrid2();
                    nextkeyword = false;
                }
                goto start;
            }
        }
        
        private void startthread(object data)
        {
            nextthread = false;
            PrintNumberParameters parameters = (PrintNumberParameters)data;
            SelfThread SelfThread = new SelfThread();
            SelfThread.ThreadNO = ThreadList.Count + 1;
            SelfThread.Started = DateTime.Now;
            SelfThread.Status = "Running";
            SelfThread.Keyword = parameters.keywords;
            SelfThread.Page = 1;
            SelfThread.Row = 0;
            SelfThread.Google = true;
            SelfThread.PublicIpAddress = GetPublicIP();
            ThreadList.Add(SelfThread);

            IWebDriver driver;
             
            
            if (!File.Exists(@"C:\Program Files\Mozilla Firefox\firefox.exe"))
            {
                chrome = true;
            }
            if (chrome == false)
            {
                FirefoxDriverService service = FirefoxDriverService.CreateDefaultService();
                FirefoxOptions firefoxOptions = new FirefoxOptions();
                firefoxOptions.AcceptInsecureCertificates = true;
                service.FirefoxBinaryPath = @"C:\Program Files\Mozilla Firefox\firefox.exe";
                service.HideCommandPromptWindow=true;               
                driver = new FirefoxDriver(service, firefoxOptions);
                SelfThread.Browser = "Firefox";
                chrome = true;
            }
            else
            {
                ChromeDriverService driverService = ChromeDriverService.CreateDefaultService();
                driverService.HideCommandPromptWindow = true;

                ChromeOptions options = new ChromeOptions();
                options.AcceptInsecureCertificates = true;

                options.BinaryLocation = @"C:\Program Files\Google\Chrome\Application\chrome.exe"; 

                driver = new ChromeDriver(driverService, options);
                SelfThread.Browser = "Chrome";

                chrome = false;
            }

            try
            {
                
                driver.Manage().Window.Minimize();
                driver.Navigate().GoToUrl("https://www.google.com/");
                             
                IWebElement element = driver.FindElement(By.Name("q"));
                element.SendKeys(parameters.keywords);

                Thread.Sleep(2000);
                IJavaScriptExecutor executor = (IJavaScriptExecutor)driver;
                executor.ExecuteScript("document.getElementsByName('btnK')[0].click();");

                
                ReadOnlyCollection<IWebElement> elemnt = driver.FindElements(By.Id("rc-anchor-container"));
                if (elemnt.Count>0)
                {
                    MessageBox.Show("گوگل برنامه را ربات شناخته ابتدا وی پی ان را تغیر دهید و مجدد امتحان کنید");
                    goto exit;
                }
                Thread.Sleep(1000);
                if (Pages.key.keyword == parameters.keywords && Pages.Page > 1)
                {
                    if (Pages.Page > 10)
                    {
                        nextkeyword = true;
                        goto exit;
                    }
                    for (int i = 1; i < Pages.Page; i++)
                    {
                        ReadOnlyCollection<IWebElement> searchResult = driver.FindElements(By.Id("pnnext"));
                        if (searchResult.Count > 0)
                        {
                            Thread.Sleep(2000);
                            IWebElement next = driver.FindElement(By.Id("pnnext"));
                            next.Click();
                            SelfThread.Page++;              
                        }
                        else
                        {
                            nextkeyword = true;
                            goto exit;
                        }
                    }
                }
                if (Pages.key.keyword != parameters.keywords )
                {
                    goto exit;
                }

                search:
                if (SelfThread.Page > 10)
                {
                    nextkeyword = true;
                    goto exit;
                }
                ReadOnlyCollection<IWebElement> searchResults = driver.FindElements(By.PartialLinkText("joorkadeh"));     
                if (searchResults.Count > 0)
                {

                    while (searchResults[Pages.Row - 1].GetAttribute("href").Contains("whois"))
                    {
                        Pages.Row++;
                        if (Pages.TotalRow < Pages.Row)
                        {
                            Pages.Page++;
                            Pages.Row = 1;
                            goto exit;
                        }
                        
                    }
                    Pages.TotalRow = searchResults.Count;
                    SelfThread.Page = Pages.Page;
                    SelfThread.Row = Pages.Row;
                    
                    Thread.Sleep(1000);
                    if (searchResults[Pages.Row - 1]==null)
                    {
                        Pages.Row = 1;
                        Pages.Page++;
                        goto exit;
                    }
                    searchResults[Pages.Row - 1].Click();
                    SelfThread.Url = driver.Url;
                    Thread.Sleep(3000);
                    string a = driver.Url;
                    int b = 0;
                    while(a.Contains("google"))                   
                    {
                        if (b>10)
                        {
                            goto exit;
                        }
                        
                        searchResults[Pages.Row - 1].Click();
                        Thread.Sleep(1000);
                        a = driver.Url;
                        b++;
                    }
                   
                    
                    Pages.Row++;
                    if (Pages.TotalRow < Pages.Row)
                    {
                        Pages.Page++;
                        Pages.Row = 1;        
                    }
                   
                }
                else
                {
                    ReadOnlyCollection<IWebElement> searchResult = driver.FindElements(By.Id("pnnext"));
                    if (searchResult.Count > 0)
                    {
                        IWebElement next = driver.FindElement(By.Id("pnnext"));
                        next.Click();
                        Pages.Page++;
                        Pages.Row = 1;
                        SelfThread.Page = SelfThread.Page++;
                        SelfThread.Row = 1;
                        Thread.Sleep(2000);
                        goto search;
                    }
                    else
                    {
                        nextkeyword = true;
                        goto exit;
                    }
                }
               
                IJavaScriptExecutor javaScript = driver as IJavaScriptExecutor;
                Int64 winheight = (Int64)javaScript.ExecuteScript("return document.body.scrollHeight");
                Stopwatch sw = new Stopwatch();
                sw.Start();
                int down = 1;
                int up = 0;
                nextthread = true;
                while (sw.Elapsed.TotalMinutes < 5)
                {
                    Int64 b = (Int64)javaScript.ExecuteScript("return window.pageYOffset");
                    b = b + 1500;
                    if (b < winheight && down == 1)
                    {
                        down = 1;
                    }
                    else
                    if (b >= winheight && up == 1)
                    {
                        down = 0;
                    }
                    if (down == 1 && b >= winheight)
                    {
                        down = 0;
                        up = 1;
                    }
                    else if (down == 0 && b <= 1500)
                    {
                        down = 1;
                        up = 0;
                    }
                    if (down == 1)
                    {
                        javaScript.ExecuteScript(" window.scrollTo(0,window.pageYOffset+1) ");

                    }
                    else
                    {
                        javaScript.ExecuteScript(" window.scrollTo(0,window.pageYOffset-1) ");
                    }
                    //Ping pingSender = new Ping();
                    //PingReply reply = pingSender.Send(SelfThread.Url);

                }
                ReadOnlyCollection<IWebElement> eelement = driver.FindElements(By.LinkText("https://dptek.ir"));
                if (eelement.Count > 0)
                {
                    IWebElement next = driver.FindElement(By.LinkText("https://dptek.ir"));
                    next.Click();
                    Thread.Sleep(200000);
                }
                sw.Stop();
            exit:
              
                if (!driver.Url.Contains("joorkadeh"))
                {
                    
                    driver.Navigate().GoToUrl("https://dptek.ir");
                    SelfThread.Url = driver.Url;
                    SelfThread.Google = false;
                    Thread.Sleep(200000);
                    ReadOnlyCollection<IWebElement> eelent = driver.FindElements(By.PartialLinkText("https://dptek.ir/Home/Product"));
                    if (eelent.Count > 0)
                    {
                        eelent[0].Click();
                        SelfThread.Url = driver.Url + driver.Url;
                        Thread.Sleep(200000);
                        if (driver.Url == "https://dptek.ir")
                        {
                            driver.Navigate().GoToUrl("https://dptek.ir/Home/Product?productId=12");
                            SelfThread.Url = driver.Url + driver.Url;
                        }
                        Thread.Sleep(200000);
                    }
                }
                SelfThread.Status = "Ended";
                SelfThread.Ended = DateTime.Now;
                TimeSpan timeSpan = SelfThread.Ended-SelfThread.Started;
                if (timeSpan.TotalMilliseconds<200000)
                {
                    
                }
                SelfThread.TimeDiff = timeSpan.ToString();
                driver.Close();
                driver.Dispose();

                nextthread = true;
                if (SelfThread.Page > 10)
                {
                    nextkeyword = true;
                }
            }
            catch(Exception exception)
            {
                SelfThread.Status = "Stopped";
                SelfThread.Exeption = exception.Message;
                SelfThread.Ended = DateTime.Now;
                TimeSpan timeSpan = SelfThread.Ended- SelfThread.Started ;
                SelfThread.TimeDiff = timeSpan.TotalMinutes.ToString();
                driver.Dispose();

                nextthread = true;
                if (SelfThread.Page > 10)
                {
                    nextkeyword = true;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            timer2.Stop();
            working = false;
            lblendtime.Text = DateTime.Now.ToString();
            foreach(var a in threads.Where(a=>a.IsAlive==true))
            {
                a.Abort();
            }
            foreach (Process Proc in Process.GetProcesses())
                if (Proc.ProcessName.Contains("chromedriver") || Proc.ProcessName.Contains("geckodriver"))
                    Proc.Kill();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            refreshgrid1();
            refreshgrid2();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            TimeSpan a = DateTime.Now - startd;
            lblelapsedtime.Text = a.ToString();
            lbltotalthread.Text = ThreadList.Count.ToString();
            lbltotalkey.Text = KeywordList.Count.ToString();
            lbltotalpages.Text = ThreadList.Where(c => !string.IsNullOrEmpty(c.Url)).Count().ToString();
            lbltotalkeyc.Text = ThreadList.Select(c => c.Keyword).Distinct().Count().ToString();
            lblthreadfailed.Text= ThreadList.Where(c => c.Status=="Stopped").Count().ToString();
            lblcrawled.Text = ThreadList.Where(c => c.Status == "Ended"&&c.Google==false).Count().ToString();
            lblrunnig.Text = ThreadList.Where(c => c.Status == "Running" ).Count().ToString();
            lblgooglecrawled.Text = ThreadList.Where(c => c.Google==true&& c.Status != "Stopped").Count().ToString();

        }
        protected virtual bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch (IOException)
            {
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            return false;
        }

       

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex==4 && e.Value.ToString()=="Stopped")
            {
                e.CellStyle.BackColor = Color.Red;
            }
            if (e.ColumnIndex == 4 && e.Value.ToString() == "Running")
            {
                e.CellStyle.BackColor = Color.Green;
            }
            if (e.ColumnIndex == 4 && e.Value.ToString() == "Ended")
            {
                e.CellStyle.BackColor = Color.Yellow;
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1) return;
        }

        private void crawler_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach (Process Proc in Process.GetProcesses())
                if (Proc.ProcessName.Contains("chromedriver") || Proc.ProcessName.Contains("geckodriver"))
                    Proc.Kill();
        }

    }
}

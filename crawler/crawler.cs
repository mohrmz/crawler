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
            KillOldDriverProcesses();

            if (!CheckForInternetConnection())
            {
                lblip.Text = "دسترسی به اینترنت وجود ندارد.";
                button1.Enabled = false;
                button2.Enabled = false;
                return;
            }

            lblip.Text = GetPublicIP();
            LoadKeywords();
            UpdateGrids();
        }

        private void KillOldDriverProcesses()
        {
            foreach (Process proc in Process.GetProcesses())
            {
                if (proc.ProcessName.Contains("chromedriver") || proc.ProcessName.Contains("geckodriver"))
                {
                    try { proc.Kill(); }
                    catch { /* Ignored */ }
                }
            }
        }

        private void LoadKeywords()
        {
            string jsonPath = Path.Combine(Environment.CurrentDirectory, "keyword.json");

            while (IsFileLocked(new FileInfo(jsonPath)))
            {
                Thread.Sleep(1000); // No infinite loop!
            }

            if (!File.Exists(jsonPath)) return;

            string json = File.ReadAllText(jsonPath);
            KeywordList = JsonConvert.DeserializeObject<List<Keywords>>(json) ?? new List<Keywords>();
        }

        private void UpdateGrids()
        {
            dataGridView2.DataSource = null;
            dataGridView2.DataSource = KeywordList.Select(k => new { k.ID, k.keyword, k.count }).ToList();

            dataGridView1.DataSource = null;
            dataGridView1.DataSource = ThreadList.Select(t => new {
                t.ThreadNO,
                t.Keyword,
                t.Status,
                t.Page,
                t.Row,
                t.Url,
                t.Browser,
                t.PublicIpAddress,
                t.TimeDiff
            }).ToList();
        }


        private static bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                using (client.OpenRead("http://clients3.google.com/generate_204"))
                {
                    return true;
                }
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
                using (var client = new WebClient())
                {
                    return client.DownloadString("https://ipinfo.io/ip").Trim();
                }
            }
            catch
            {
                return "خطا در دریافت آی‌پی عمومی";
            }
        }

        private void StartCrawlingLoop()
        {
            while (working)
            {
                var orderedKeywords = KeywordList.OrderBy(k => k.count).ToList();

                foreach (var keyword in orderedKeywords)
                {
                    if (!working) break;

                    Pages.key = keyword;
                    Pages.Row = 1;
                    Pages.Page = 1;
                    Pages.TotalRow = 1;

                    ProcessKeyword(keyword);
                    keyword.count++;
                    SaveKeywords();
                }
            }
        }

        private void ProcessKeyword(Keywords keyword)
        {
            nextkeyword = false;

            while (!nextkeyword && working)
            {
                if (Pages.key == keyword && Pages.Page > 10)
                {
                    nextkeyword = true;
                    break;
                }

                if (nextthread && ThreadList.Count(t => t.Status == "Running") < 5)
                {
                    var param = new PrintNumberParameters { keywords = keyword.keyword };
                    var thread = new Thread(RunCrawlerThread);
                    threads.Add(thread);
                    thread.Start(param);
                }

                Thread.Sleep(3000); // نرخ طبیعی‌تر
            }
        }

        private void RunCrawlerThread(object param)
        {
            nextthread = false;

            var parameters = param as PrintNumberParameters;
            var threadModel = new SelfThread
            {
                ThreadNO = ThreadList.Count + 1,
                Started = DateTime.Now,
                Status = "Running",
                Keyword = parameters.keywords,
                Page = 1,
                Row = 0,
                Google = true,
                PublicIpAddress = GetPublicIP(),
                Browser = chrome ? "Chrome" : "Firefox"
            };
            ThreadList.Add(threadModel);

            IWebDriver driver = null;

            try
            {
                driver = InitializeDriver(); // جداشده برای مدیریت مرورگر

                // باز کردن گوگل
                driver.Navigate().GoToUrl("https://www.google.com");
                Thread.Sleep(RandomDelay());

                var input = driver.FindElement(By.Name("q"));
                input.SendKeys(parameters.keywords);
                Thread.Sleep(RandomDelay());

                input.SendKeys(OpenQA.Selenium.Keys.Enter);
                Thread.Sleep(RandomDelay());

                // بررسی کپچا
                if (driver.PageSource.Contains("captcha") || driver.FindElements(By.Id("rc-anchor-container")).Count > 0)
                {
                    MessageBox.Show("شناسایی توسط گوگل. لطفاً IP یا VPN را تغییر دهید.");
                    return;
                }

                // بررسی لینک هدف
                SimulateSearch(driver, threadModel, parameters.keywords);

                threadModel.Status = "Ended";
                threadModel.Ended = DateTime.Now;
                threadModel.TimeDiff = (threadModel.Ended - threadModel.Started).ToString(@"hh\:mm\:ss");
            }
            catch (Exception ex)
            {
                threadModel.Status = "Stopped";
                threadModel.Exeption = ex.Message;
                threadModel.Ended = DateTime.Now;
                threadModel.TimeDiff = (threadModel.Ended - threadModel.Started).ToString(@"hh\:mm\:ss");
            }
            finally
            {
                driver?.Quit();
                nextthread = true;

                if (threadModel.Page > 10)
                {
                    nextkeyword = true;
                }
            }
        }

        private IWebDriver InitializeDriver()
        {
            if (!chrome && File.Exists(@"C:\Program Files\Mozilla Firefox\firefox.exe"))
            {
                var service = FirefoxDriverService.CreateDefaultService();
                var options = new FirefoxOptions
                {
                    AcceptInsecureCertificates = true
                };
                service.FirefoxBinaryPath = @"C:\Program Files\Mozilla Firefox\firefox.exe";
                service.HideCommandPromptWindow = true;
                chrome = true;
                return new FirefoxDriver(service, options);
            }
            else
            {
                var driverService = ChromeDriverService.CreateDefaultService();
                var options = new ChromeOptions
                {
                    AcceptInsecureCertificates = true
                };
                options.BinaryLocation = @"C:\Program Files\Google\Chrome\Application\chrome.exe"; // تنظیم مسیر دقیق
                driverService.HideCommandPromptWindow = true;
                chrome = false;
                return new ChromeDriver(driverService, options);
            }
        }

        private int RandomDelay(int min = 1200, int max = 3000)
        {
            return new Random().Next(min, max);
        }


        private void SimulateSearch(IWebDriver driver, SelfThread thread, string keyword)
        {
            const string targetText = "joorkadeh";
            const int maxPages = 10;

            for (int page = 1; page <= maxPages; page++)
            {
                var results = driver.FindElements(By.PartialLinkText(targetText)).ToList();
                thread.Page = page;
                thread.Row = 1;

                // فیلتر لینک‌های whois یا بی‌فایده
                var validResults = results.Where(r =>
                    !string.IsNullOrEmpty(r.GetAttribute("href")) &&
                    !r.GetAttribute("href").Contains("whois")).ToList();

                if (validResults.Any())
                {
                    var link = validResults.First();
                    link.Click();
                    Thread.Sleep(RandomDelay(2000, 4000));

                    string newUrl = driver.Url;
                    int tries = 0;

                    // باز کلیک در صورت باز نشدن سایت هدف
                    while (newUrl.Contains("google") && tries < 5)
                    {
                        link.Click();
                        Thread.Sleep(RandomDelay());
                        newUrl = driver.Url;
                        tries++;
                    }

                    thread.Url = newUrl;
                    SimulateUserScroll(driver);
                    return;
                }

                // اگر لینک پیدا نشد و صفحه بعدی هست → رفتن به صفحه بعد
                var nextPage = driver.FindElements(By.Id("pnnext")).FirstOrDefault();
                if (nextPage != null)
                {
                    nextPage.Click();
                    Thread.Sleep(RandomDelay(2500, 3500));
                }
                else
                {
                    nextkeyword = true;
                    return;
                }
            }

            nextkeyword = true;
        }

        private void SimulateUserScroll(IWebDriver driver)
        {
            var js = (IJavaScriptExecutor)driver;
            int totalHeight = Convert.ToInt32(js.ExecuteScript("return document.body.scrollHeight"));

            int scroll = 0;
            int direction = 1;

            Stopwatch sw = Stopwatch.StartNew();
            while (sw.Elapsed.TotalSeconds < 20)
            {
                js.ExecuteScript($"window.scrollTo(0, {scroll});");
                Thread.Sleep(RandomDelay(300, 600));

                scroll += direction * 250;

                if (scroll >= totalHeight || scroll <= 0)
                    direction *= -1;
            }
        }


        private void button2_Click(object sender, EventArgs e)
        {
            working = false; // همه حلقه‌ها خودشون چک می‌کنن

            timer1.Stop();
            timer2.Stop();

            lblendtime.Text = DateTime.Now.ToString();

            // بستن همه مرورگرها با بستن driver در thread خودش انجام میشه

            // منتظر بمان تا همه threadها کارشون تموم شه
            foreach (var thread in threads)
            {
                if (thread.IsAlive)
                {
                    thread.Join(); // صبر کن تا تموم شه
                }
            }

            KillOldDriverProcesses();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            TimeSpan duration = DateTime.Now - startd;
            lblelapsedtime.Text = duration.ToString(@"hh\:mm\:ss");

            lbltotalthread.Text = ThreadList.Count.ToString();
            lbltotalkey.Text = KeywordList.Count.ToString();
            lbltotalpages.Text = ThreadList.Count(t => !string.IsNullOrEmpty(t.Url)).ToString();
            lbltotalkeyc.Text = ThreadList.Select(t => t.Keyword).Distinct().Count().ToString();
            lblthreadfailed.Text = ThreadList.Count(t => t.Status == "Stopped").ToString();
            lblcrawled.Text = ThreadList.Count(t => t.Status == "Ended" && t.Google == false).ToString();
            lblrunnig.Text = ThreadList.Count(t => t.Status == "Running").ToString();
            lblgooglecrawled.Text = ThreadList.Count(t => t.Google && t.Status != "Stopped").ToString();
        }

        private void LogThreadResultToFile(SelfThread thread)
        {
            string logDir = Path.Combine(Environment.CurrentDirectory, "logs");
            Directory.CreateDirectory(logDir);

            string logFile = Path.Combine(logDir, $"log_{DateTime.Now:yyyy-MM-dd}.txt");

            string content = $"[{DateTime.Now:HH:mm:ss}] Thread #{thread.ThreadNO} | Keyword: {thread.Keyword} | " +
                             $"Status: {thread.Status} | Page: {thread.Page} | Url: {thread.Url} | Time: {thread.TimeDiff}";

            if (!string.IsNullOrWhiteSpace(thread.Exeption))
                content += $" | Error: {thread.Exeption}";

            File.AppendAllText(logFile, content + Environment.NewLine);
        }



        private void SaveKeywords()
        {
            string jsonPath = Path.Combine(Environment.CurrentDirectory, "keyword.json");

            if (IsFileLocked(new FileInfo(jsonPath))) return;

            string json = JsonConvert.SerializeObject(KeywordList, Formatting.Indented);
            File.WriteAllText(jsonPath, json);
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

                Thread.Sleep(10000);
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

        private void timer1_Tick(object sender, EventArgs e)
        {
            refreshgrid1();
            refreshgrid2();
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

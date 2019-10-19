﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Net;
using System.Timers;
using System.Threading;
using System.Diagnostics;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace crawler
{
    public partial class crawler : Form
    {

        bool working = true;
        bool nextthread = true;
        bool nextkeyword = false;
        List<Keywords> KeywordList = new List<Keywords>();
        List<thread> ThreadList = new List<thread>();
        public crawler()
        {
            InitializeComponent();

        }

        private void crawler_Load(object sender, EventArgs e)
        {
            if (CheckForInternetConnection())
            {

                lblip.Text = GetPublicIP();
                lblstarttime.Text = DateTime.Now.ToString();
                refreshgrid2();
                refreshgrid1();
                //dataGridView2.DataBind();
                //Thread timer = new Thread(start);
                //timer.Start();

            }
            else
            {
                lblip.Text = "You Are Not have Internet Access Please Connect To Internet and Retry";
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
            return new System.Net.WebClient().DownloadString("https://ipinfo.io/ip").Replace("\n", "");
        }
        private void refreshgrid2()
        {
            string json = File.ReadAllText(Environment.CurrentDirectory + @"\keyword.json");
            KeywordList.Clear();
            KeywordList = JsonConvert.DeserializeObject<List<Keywords>>(json);
            dynamic dynamicObject = JsonConvert.DeserializeObject(json);
            dataGridView2.DataSource = dynamicObject;
            dataGridView2.Refresh();
        }
        private void refreshgrid1()
        {
            dataGridView1.DataSource = ThreadList;
            dataGridView1.Refresh();
        }
        private void writegrid2()
        {
            string jsonpath = Environment.CurrentDirectory + @"\keyword.json";
            string dynamic = JsonConvert.SerializeObject(KeywordList);
            System.IO.File.WriteAllText(jsonpath, dynamic);
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
        public class Keywords
        {
            public int ID { get; set; }
            public string keyword { get; set; }
            public int count { get; set; }
        }
        public class PrintNumberParameters
        {
            public Keywords keywords { get; set; }

        }
        public class thread
        {
            public int ThreadNO { get; set; }
            public DateTime Started { get; set; }
            public DateTime Ended { get; set; }
            public string Status { get; set; }
            public Keywords Keyword { get; set; }
            public int Page { get; set; }
            public int Row { get; set; }
            public string Url { get; set; }

        }
        public static class Pages
        {
            public static int Row { get; set; }
            public static int TotalRow { get; set; }
            public static int Page { get; set; }
            public static Keywords key { get; set; }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            working = true;

            foreach(Keywords key in KeywordList.OrderBy(c=>c.count))
            {
                Pages.key = key;
                Pages.Row = 1;
                Pages.Page = 1;
                Pages.TotalRow = 1;
                if (nextkeyword==true)
                {
                    nextkeyword = false;
                    goto exit;
                }
                while (working)
                {
                    
                    if (Pages.key==key && Pages.Page>10)
                    {
                        goto exit;
                    }
                    if (nextthread == true)
                    {
                        refreshgrid1();
                        ParameterizedThreadStart threadStart = new ParameterizedThreadStart(startthread);
                        Thread thread = new Thread(threadStart);
                        thread.Start(new PrintNumberParameters() { keywords = key });

                    }
                    Thread.Sleep(5000);
                }
                exit:
                KeywordList[key.ID-1].count++;
                writegrid2();
                refreshgrid2();
                refreshgrid1();
            }
            lblendtime.Text = DateTime.Now.ToString();

        }

        private void startthread(object data)
        {
            try
            {
                nextthread = false;
               
                PrintNumberParameters parameters = (PrintNumberParameters)data;

                thread thread = new thread();
                thread.ThreadNO = ThreadList.Count + 1;
                thread.Started = DateTime.Now;
                thread.Status = "running";
                thread.Keyword = parameters.keywords;
                thread.Page = 1;
                thread.Row = 0;

                ThreadList.Add(thread);


                IWebDriver driver = new ChromeDriver();
                driver.Navigate().GoToUrl("https://www.google.com/");

                IWebElement element = driver.FindElement(By.Name("q"));
                element.SendKeys(parameters.keywords.keyword);

                Thread.Sleep(2000);
                IJavaScriptExecutor executor = (IJavaScriptExecutor)driver;
                executor.ExecuteScript("document.getElementsByName('btnK')[0].click();");
                Thread.Sleep(2000);

                if (Pages.key == parameters.keywords && Pages.Page > 1)
                {
                    if (Pages.Page > 10)
                    {
                        goto exit;
                    }
                    for (int i = 1; i < Pages.Page; i++)
                    {
                        ReadOnlyCollection<IWebElement> searchResult = driver.FindElements(By.Id("pnnext"));
                        if (searchResult.Count > 0)
                        {
                            IWebElement next = driver.FindElement(By.Id("pnnext"));
                            next.Click();
                        }
                        else
                        {
                            nextkeyword = true;
                            goto exit;
                        }
                    }
                }

            search:
                ReadOnlyCollection<IWebElement> searchResults = driver.FindElements(By.PartialLinkText("joorkadeh"));
                Pages.TotalRow = searchResults.Count;
                if (searchResults.Count > 0)
                {
                    thread.Page = Pages.Page;
                    thread.Row = Pages.Row;
                    thread.Url = searchResults[Pages.Row - 1].GetAttribute("href");
                    searchResults[Pages.Row - 1].Click();

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

                }
                sw.Stop();
            exit:
                thread.Status = "stopped";
                thread.Ended = DateTime.Now;
                nextthread = true;
                if (Pages.Page > 10)
                {
                    nextkeyword = true;
                }
                //refreshgrid1();
                driver.Close();
                driver.Dispose();
            }
            catch
            {
                nextthread = true;
                if (Pages.Page>11)
                {
                    nextkeyword = true;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            working = false;
            lblendtime.Text = DateTime.Now.ToString();
        }
    }
}

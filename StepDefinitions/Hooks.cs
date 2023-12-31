﻿using System.Diagnostics;
using Microsoft.Playwright;
using System.IO;
using static System.Net.Mime.MediaTypeNames;
using TechTalk.SpecFlow;

namespace SpecFlowProjectWithPlaywright.StepDefinitions
{
    [Binding]
    public sealed class Hooks
    {

        private readonly ScenarioContext _scenarioContext;
        public Hooks(ScenarioContext scenarioContext) => _scenarioContext = scenarioContext;

        [BeforeScenario("web")]
        public async Task BeforeScenarioWithTag()
        {
            string? path = Path.Combine(Environment.CurrentDirectory, $"./TestOutPut/VideoRecording/{_scenarioContext.ScenarioInfo.Title}");

            Directory.CreateDirectory(path);
            var videoRecording = $"{_scenarioContext.ScenarioInfo.Title}_{DateTime.Now:yyyyMMdd_HHmmss}.mp4";
            var videoRecordingtFullPath = Path.Combine(path, videoRecording);

            IPlaywright? playwright = await Playwright.CreateAsync();
            // Create a Chromium browser instance
            IBrowser? browser = await playwright.Chromium.LaunchAsync(new()
            {
                Headless = false
            });

            //browser context will record video
            IBrowserContext? browserContext = await browser.NewContextAsync(new()
            {
                RecordVideoDir = videoRecordingtFullPath,
                RecordVideoSize = new RecordVideoSize() { Width = 1920, Height = 1080 }
            });
            //browser context will trace logging 
            await browserContext.Tracing.StartAsync(new()
            {
                Screenshots = true,
                Snapshots = true,
                Sources = true
            });

            IPage? newPageAsync = await browserContext.NewPageAsync();
            _scenarioContext.ScenarioContainer.RegisterInstanceAs(newPageAsync);

            _scenarioContext.ScenarioContainer.RegisterInstanceAs(playwright);
            _scenarioContext.ScenarioContainer.RegisterInstanceAs(browser);
            _scenarioContext.ScenarioContainer.RegisterInstanceAs(browserContext);
        }

        [AfterScenario("web")]
        public async Task AfterScenario()
        {
            // Take a screenshot and save it in the test output folder
            string? screenshotPath = Path.Combine(Environment.CurrentDirectory, $"./TestOutPut/screenshots/{_scenarioContext.ScenarioInfo.Title}");
            CreateDirectory(screenshotPath);
            var screenshotFileName = $"{_scenarioContext.ScenarioInfo.Title}_{DateTime.Now:yyyyMMdd_HHmmss}.png";
            var screenshotFullPath = Path.Combine(screenshotPath, screenshotFileName);
            IBrowserContext browserContext = _scenarioContext.ScenarioContainer.Resolve<IBrowserContext>();

            var tracingPath = Path.Combine(Environment.CurrentDirectory, $"./TestOutPut/Trace/{_scenarioContext.ScenarioInfo.Title}/");
            await browserContext.Tracing.StopAsync(new()
            {
                Path = Path.Combine(tracingPath + $"{DateTime.Now:yyyyMMdd_HHmmss}.zip")
            });
            IPage page = _scenarioContext.ScenarioContainer.Resolve<IPage>();
            await page.ScreenshotAsync(new PageScreenshotOptions { Path = screenshotFullPath });

            var VideoRecordingpath = Path.Combine(Environment.CurrentDirectory, $"./TestOutPut/VideoRecording/{_scenarioContext.ScenarioInfo.Title}");
            CreateDirectory(VideoRecordingpath);

            page.Video?.SaveAsAsync(VideoRecordingpath);

            await page.CloseAsync();
            await browserContext.CloseAsync();
            var browser = _scenarioContext.ScenarioContainer.Resolve<IBrowser>();
            await browser.CloseAsync();
        }

        public string CreateDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                Console.WriteLine("Created new Directory:", path);
            }
            else
                Console.WriteLine("File {0} already Existing:", path);
            return path;
        }
    }
}

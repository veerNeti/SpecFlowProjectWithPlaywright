using Microsoft.Playwright;
using Xunit;
using static System.Net.Mime.MediaTypeNames;

namespace SpecFlowProjectWithPlaywright.StepDefinitions
{
    [Binding]
    public class SampleFeatureSteps
    {
        private readonly IPage _page;
        private readonly ILoginPg _loginPg;
        public SampleFeatureSteps(ScenarioContext scenarioContext)
        {
            _page= scenarioContext.ScenarioContainer.Resolve<IPage>();
             _loginPg = new LoginPg(page: _page);
        }

        [Given(@"I navigate to ""(.*)""")]
        public async Task GivenINavigateTo(string url)
        {
            await _page.GotoAsync(url);
        }


        [Given(@"User completes the login pg")]
        public async Task GivenUserCompletesTheLoginPg()
        {
            await _loginPg.CompleteLoginOperation("standard_user", "secret_sauce");
        }

        [Then(@"user should be routed to store")]
        public async Task ThenUserShouldBeRoutedToStore()
        {
            await _page.Locator("#item_4_title_link").ClickAsync();

            bool isVisibleAsync = await _page.GetByText("Sauce Labs Backpack").IsVisibleAsync();
            Assert.True(isVisibleAsync);
        }
    }
}

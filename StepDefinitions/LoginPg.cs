using Microsoft.Playwright;

namespace SpecFlowProjectWithPlaywright.StepDefinitions
{
    public interface ILoginPg
    {
        Task CompleteLoginOperation(string user, string pass);
    }
    public class LoginPg : ILoginPg
    {
        private readonly IPage _page;
        //ctor
        public LoginPg(IPage page) => _page = page;

        private ILocator _username => _page.Locator("[data-test='username']");
        private ILocator _password => _page.Locator("[data-test='password']");
        private ILocator _LoginButton => _page.GetByRole(AriaRole.Button, new() { Name = "LOGIN" });

        #region Actions
        public async Task CompleteLoginOperation(string user, string pass)
        {
            await _username.FillAsync(user);
            await _password.FillAsync(pass);
            await _LoginButton.ClickAsync();
        }
        #endregion Actions
    }
}

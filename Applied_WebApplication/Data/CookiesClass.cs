using Microsoft.AspNetCore.Http;
namespace Applied_WebApplication.Data
{
    public class CookiesClass
    {
        private readonly IHttpContextAccessor AppContextAccessor;
        public CookiesClass(IHttpContextAccessor httpContextAccessor)
        {
            AppContextAccessor = httpContextAccessor;
        }

        public string GetUserName(string UserName)
        {
            CookieOptions options = new CookieOptions();
            options.Expires = DateTime.Now.AddSeconds(30);
            AppContextAccessor.HttpContext.Response.Cookies.Append("someKey", "someValue", options);
            return "";
        }

        public void UserCookie()
        {
            var cookieOptions = new CookieOptions();
            cookieOptions.Expires = DateTime.Now.AddDays(1);
            cookieOptions.Path = "/";
            AppContextAccessor.HttpContext.Response.Cookies.Append("UserName", "Admin", cookieOptions);
        }

        public string GetUserName()
        {
            string SessionUserID = "_Name";
            var username = AppContextAccessor.HttpContext.Session.GetString(SessionUserID);
            return username;
        }

    }
}

using System;
using System.Threading.Tasks;
using Statiq.App;
using Statiq.Common;
using Statiq.Feeds;
using Statiq.Web;

namespace site
{
    internal static class Program
    {
        private static Task<int> Main(string[] args)
        {
            return Bootstrapper
                .Factory
                .CreateDefault(args)
                .AddHostingCommands()
                .AddSetting(Keys.LinkLowercase, true)
                .AddSetting(Keys.LinksUseHttps, true)
                .AddSetting(Keys.Host, "rodneylittlesii.com")
                .AddSetting(Keys.Title, "Rodney Littles, II")
                .AddSetting(FeedKeys.Image, "favicon.ico")
                .AddSetting(FeedKeys.Author, "Rodney Littles, II")
                .AddSetting(FeedKeys.Image, "favicon.ico")
                .AddSetting(FeedKeys.Description, "This is a journey into code.")
                .AddSetting(FeedKeys.Copyright, DateTime.UtcNow.Year.ToString())
                .RunAsync();
        }
    }
}

using System.Linq;
using site.Extensions;
using Statiq.Common;
using Statiq.Core;
using Statiq.Feeds;
using Statiq.Handlebars;
using Statiq.Html;
using Statiq.Markdown;
using Statiq.Yaml;

namespace site.Pipelines
{
    public class MePipeline : ApplyLayoutPipeline
    {
        public MePipeline()
        {
            Dependencies.AddRange(nameof(BlogPostPipeline), nameof(TagsPipeline));

            InputModules = new ModuleList
            {
                new ReadFiles("_me.hbs")
            };

            ProcessModules = new ModuleList
            {
                new ExtractFrontMatter(new ParseYaml()),
                new SetDestination(Config.FromValue(new NormalizedPath("./about.html"))),
                new RenderHandlebars()
            };

            OutputModules = new ModuleList
            {
                new WriteFiles()
            };
        }
    }
}
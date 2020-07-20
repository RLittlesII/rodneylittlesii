using System.Linq;
using site.Extensions;
using Statiq.Common;
using Statiq.Core;
using Statiq.Feeds;
using Statiq.Handlebars;
using Statiq.Yaml;

namespace site.Pipelines
{
    public class IndexPipeline : ApplyLayoutPipeline
    {
        public IndexPipeline()
        {
            Dependencies.AddRange(nameof(BlogPostPipeline), nameof(TagsPipeline));

            InputModules = new ModuleList
            {
                new ReadFiles("_index.hbs")
            };

            ProcessModules = new ModuleList
            {
                new ExtractFrontMatter(new ParseYaml()),
                new SetDestination(Config.FromValue(new NormalizedPath("./index.html"))),
                new RenderHandlebars()
                    .WithModel(Config.FromDocument((doc, context) => new
                    {
                        intro = doc.GetString("intro"),
                        description = context.Settings.GetString(FeedKeys.Description),
                        posts =
                            context
                                .Outputs
                                .FromPipeline(nameof(BlogPostPipeline))
                                .OrderByDescending(x => x.GetDateTime(FeedKeys.Published))
                                .Take(4)
                                .Select(x => x.AsPost(context)),
                        olderPosts =
                            context
                                .Outputs
                                .FromPipeline(nameof(BlogPostPipeline))
                                .OrderByDescending(x => x.GetDateTime(FeedKeys.Published))
                                .Skip(3)
                                .Take(4)
                                .Select(x => x.AsPost(context)), 
                        tags =
                            context
                                .Outputs
                                .FromPipeline(nameof(TagsPipeline))
                                .OrderByDescending(x => x.GetChildren().Count)
                                .ThenBy(x => x.GetString(Keys.GroupKey))
                                .Take(5)
                                .Select(x => x.AsTag(context)),
                        socialMediaLinks =
                            doc.GetChildren("socialMediaLinks")
                               .Select(x => x.AsDynamic())
                    }))
            };

            OutputModules = new ModuleList
            {
                new WriteFiles()
            };
        }
    }
}
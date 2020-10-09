using Nuke.Common;
using Wyam.Core.Execution;
using Wyam.Common.Execution;
using Wyam.Common.IO;
using Wyam.Common.Meta;
using Wyam.Core.Modules.IO;
using Wyam.Core.Modules.Control;
using Wyam.Core.Modules.Metadata;
using Wyam.Configuration;
using System.Linq;
using Wyam.Common.Modules;
using System;
using Wyam.Common.Documents;
using System.Collections.Generic;
using Wyam.Common.Configuration;
using System.IO;
using Wyam.Blog;
using Wyam.Common.Shortcodes;
using Wyam.Core.Modules.Extensibility;


class ConfigurationEngineBase : IEngine
{
    private readonly Engine engine;

    public ConfigurationEngineBase(Engine engine)
    {
        this.engine = engine;
    }

    public IFileSystem FileSystem => engine.FileSystem;

    public ISettings Settings => engine.Settings;

    public IPipelineCollection Pipelines => engine.Pipelines;

    public IShortcodeCollection Shortcodes => engine.Shortcodes;

    public IDocumentCollection Documents => engine.Documents;

    public INamespacesCollection Namespaces => engine.Namespaces;

    public IRawAssemblyCollection DynamicAssemblies => engine.DynamicAssemblies;

    public string ApplicationInput { get => engine.ApplicationInput; set => engine.ApplicationInput = value; }
    public IDocumentFactory DocumentFactory { get => engine.DocumentFactory; set => engine.DocumentFactory = value; }
}

class WyamConfiguration : ConfigurationEngineBase
{
    public WyamConfiguration(Engine engine, Build build) : base(engine)
    {
        var configurator = new Configurator(engine);
        configurator.Recipe = new Blog();
        configurator.Theme = "SolidState";
        configurator.Configure("");

        engine.ApplicationInput = NukeBuild.RootDirectory / "input";

        Settings[BlogKeys.Title ] = "Rodney Littles II";
        Settings[BlogKeys.Image] = "favicon.ico";
        Settings[BlogKeys.Description] = "This is a journey into code.";
        Settings[BlogKeys.Intro] = "Where I clone repositories, dig through code, and learn ... things.";
        Settings[BlogKeys.CaseInsensitiveTags] = true;
        Settings[BlogKeys.MarkdownConfiguration] = "advanced+bootstrap+emojis";
        Settings[BlogKeys.IncludeDateInPostPath] = false;
        Settings[BlogKeys.MetaRefreshRedirects] = true;
        Settings[BlogKeys.NetlifyRedirects] = true;
        Settings[Keys.LinksUseHttps] = true;
        Settings[Keys.Host] = "rodneylittlesii.com/";
        
        FileSystem.InputPaths.AddRange(new DirectoryPath[] { "src" });

// Use deep wild cards for posts
        var list = ((IModuleList)Blog.BlogPosts["MarkdownPosts"]);
        list.Remove(list.First());
        list.Insert(0,
            new ReadFiles(ctx => $"{ctx.DirectoryPath(BlogKeys.PostsPath).FullPath}/**/*.md")
        );

        list = ((IModuleList)Blog.BlogPosts["RazorPosts"]);
        list.Remove(list.First());
        list.Insert(0,
            new ReadFiles(ctx => $"{ctx.DirectoryPath(BlogKeys.PostsPath).FullPath}/{{!_,!index,}}**/*.cshtml")
        );

// draft support
        if (!Settings.Get<bool>("Drafts")) {
            var p = Blog.BlogPosts[BlogKeys.Published] as ModuleCollection;
            if (p[0].GetType() == typeof(Where)) p.RemoveAt(0);
            p.Insert(0, new Where((doc, ctx) =>
            {
                if (doc.Get<bool>("Draft"))
                {
                    return false;
                }
                return true;
            }));
        }

// Folder in posts is a topic 
        if (Blog.BlogPosts.Contains("TopicPath")) Blog.BlogPosts.Remove("TopicPath");
        Blog.BlogPosts.Add("TopicPath", new Meta(Keys.RelativeFilePath, (doc, ctx) =>
        {
            var published = doc.Get<DateTime>(BlogKeys.Published);
            var fileName = doc.Bool("FrontMatterPublished")
                ? doc.FilePath(Keys.SourceFileName).ChangeExtension("html").FullPath
                : doc.FilePath(Keys.SourceFileName).ChangeExtension("html").FullPath.Substring(11);
            var fileFolder = doc.DirectoryPath(Keys.RelativeFileDir).FullPath;
            var folder = ctx.DirectoryPath(BlogKeys.PostsPath).FullPath;
            if (fileFolder.Length > folder.Length)
            {
                var topic = fileFolder.Substring(folder.Length).Trim('/', '\\');
                if (topic.Length > 0)
                {
                    fileName = $"{topic}/{fileName}";
                }
            }

            return ctx.Bool(BlogKeys.IncludeDateInPostPath)
                ? $"{ctx.DirectoryPath(BlogKeys.PostsPath).FullPath}/{published:yyyy}/{published:MM}/{fileName}"
                : $"{ctx.DirectoryPath(BlogKeys.PostsPath).FullPath}/{fileName}";
        }));
    }
}

class FileNameComparer : EqualityComparer<string>
{
    public override bool Equals(string x, string y)
    {
        return Path.GetFileNameWithoutExtension(x) == Path.GetFileNameWithoutExtension(y);
    }

    public override int GetHashCode(string obj)
    {
        return obj == null ? 0 : Path.GetFileNameWithoutExtension(obj).GetHashCode();
    }
}

class SelectMany : IModule
{
    private readonly Func<IDocument, IEnumerable<IDocument>> _config;

    public SelectMany(Func<IDocument, IEnumerable<IDocument>> config)
    {
        _config = config;
    }

    public IEnumerable<IDocument> Execute(IReadOnlyList<IDocument> inputs, IExecutionContext context)
    {
        return inputs.SelectMany(_config);
    }
}
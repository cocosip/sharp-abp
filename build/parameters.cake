#load "./util.cake"
#load "./paths.cake"
#load "./packages.cake"
#load "./version.cake"
#load "./credentials.cake"

public class BuildParameters
{
    public string Target { get; private set; }
    public string Configuration { get; private set; }
    public bool IsLocalBuild { get; private set; }
    public bool IsRunningOnUnix { get; private set; }
    public bool IsRunningOnWindows { get; private set; }
    public bool IsRunningOnTravisCI { get; private set; }
    public bool IsRunningOnAppVeyor { get; private set; }
    public bool IsRunningOnAzurePipelines { get; private set; }
    public bool IsRunningOnAzurePipelinesHosted { get; private set;}
    public bool IsPullRequest { get; private set; }
    public bool IsMasterBranch { get; private set; }
    public bool IsDevelopBranch { get; private set; }
    public bool IsTagged { get; private set; }
    public bool IsPublishBuild { get; private set; }
    public bool IsReleaseBuild { get; private set; }
    public bool SkipGitVersion { get; private set; }
    public bool SkipOpenCover { get; private set; }
    public bool SkipSigning { get; private set; }
    public BuildCredentials GitHub { get; private set; }
    public CoverallsCredentials Coveralls { get; private set; }
    public ReleaseNotes ReleaseNotes { get; private set; }
    public BuildVersion Version { get; private set; }
    public BuildPaths Paths { get; private set; }
    public BuildPackages Packages { get; private set; }

    public DirectoryPathCollection Projects { get; set; }
    public DirectoryPathCollection TestProjects { get; set; }
    public FilePathCollection ProjectFiles { get; set; }
    public FilePathCollection TestProjectFiles { get; set; }
    public string[] PackageIds { get; private set; }

    public bool ShouldPublish
    {
        get
        {
            return !IsLocalBuild && !IsPullRequest && IsTagged && (IsRunningOnTravisCI || IsRunningOnAppVeyor || IsRunningOnAzurePipelines|| IsRunningOnAzurePipelinesHosted)&&IsRunningOnWindows;
        }
    }

    public bool ShouldPublishToNuGet
    {
        get
        {
            return !IsLocalBuild && !IsPullRequest && IsTagged && (IsRunningOnTravisCI || IsRunningOnAppVeyor || IsRunningOnAzurePipelines || IsRunningOnAzurePipelinesHosted)&&IsRunningOnWindows;
        }
    }

    public void Initialize(ICakeContext context)
    {
        var versionFile = context.File("./build/version.props");
        var content = System.IO.File.ReadAllText(versionFile.Path.FullPath);

        XmlDocument doc = new XmlDocument();
        doc.LoadXml(content);

        var versionMajor = doc.DocumentElement.SelectSingleNode("/Project/PropertyGroup/VersionMajor").InnerText;
        var versionMinor = doc.DocumentElement.SelectSingleNode("/Project/PropertyGroup/VersionMinor").InnerText;
        var versionPatch = doc.DocumentElement.SelectSingleNode("/Project/PropertyGroup/VersionPatch").InnerText;
        var versionQuality = doc.DocumentElement.SelectSingleNode("/Project/PropertyGroup/VersionQuality").InnerText;
        versionQuality = string.IsNullOrWhiteSpace(versionQuality) ? null : versionQuality;

        var suffix = doc.DocumentElement.SelectSingleNode("/Project/PropertyGroup/VersionSuffix").InnerText;

        //如果本地发布,就加dev,如果是nuget发布,就加preview
        if (IsLocalBuild)
        {
            suffix += "dev-" + Util.CreateStamp();
        }
        // else
        // {
        //     //需要发布到Nuget
        //     if (ShouldPublishToNuGet && !string.IsNullOrWhiteSpace(versionQuality))
        //     {
        //         suffix = string.IsNullOrWhiteSpace(suffix) ? "Pre" : suffix;
        //     }
		// 	else
		// 	{
		// 	    suffix = "";
		// 	}
        // }
        suffix = string.IsNullOrWhiteSpace(suffix) ? null : suffix;
        context.Information($"Suffix:{suffix}");

        Version =
            new BuildVersion(int.Parse(versionMajor), int.Parse(versionMinor), int.Parse(versionPatch), versionQuality);
        Version.Suffix = suffix;

        Paths = BuildPaths.GetPaths(context, Configuration, Version.VersionWithSuffix());

        Packages = BuildPackages.GetPackages(
            Paths.Directories.NugetRoot,
            Version.VersionWithSuffix(),
            PackageIds,
            new string[] {});
    }

    public static BuildParameters GetParameters(ICakeContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException("context");
        }

        var target = context.Argument("target", "Default");
        var buildSystem = context.BuildSystem();

        var projects = context.GetDirectories("./framework/*/src/*");

        var projectFiles = context.GetFiles("./framework/*/src/*/*.csproj");

        var testProjects = context.GetDirectories("./framework/*/test/*");

        var testProjectFiles = context.GetFiles("./framework/*/test/*/*.csproj");

        var packageIdsFiles = context.GetFiles("./framework/*/src/*/*.csproj");

        var parameters = new BuildParameters
        {
            Target = target,
            Configuration = context.Argument("configuration", "Release"),
            IsLocalBuild = buildSystem.IsLocalBuild,
            IsRunningOnUnix = context.IsRunningOnUnix(),
            IsRunningOnWindows = context.IsRunningOnWindows(),
            IsRunningOnTravisCI = buildSystem.TravisCI.IsRunningOnTravisCI,
            IsRunningOnAppVeyor =  buildSystem.AppVeyor.IsRunningOnAppVeyor,
            IsRunningOnAzurePipelines = buildSystem.TFBuild.IsRunningOnAzurePipelines,
            IsRunningOnAzurePipelinesHosted= buildSystem.TFBuild.IsRunningOnAzurePipelinesHosted,
            IsPullRequest = IsThePullRequest(buildSystem),
            IsMasterBranch = IsTheMasterBranch(buildSystem),
            IsDevelopBranch = IsTheDevelopBranch(buildSystem),
            IsTagged = IsBuildTagged(buildSystem),
            GitHub = null, // BuildCredentials.GetGitHubCredentials(context),
            Coveralls = null, //CoverallsCredentials.GetCoverallsCredentials(context),
            ReleaseNotes = null, //context.ParseReleaseNotes("./README.md"),
            IsPublishBuild = IsPublishing(target),
            IsReleaseBuild = IsReleasing(target),
            SkipSigning = StringComparer.OrdinalIgnoreCase.Equals("True", context.Argument("skipsigning", "True")),
            SkipGitVersion = StringComparer.OrdinalIgnoreCase.Equals("True", context.EnvironmentVariable("SKIP_GITVERSION")),
            SkipOpenCover = true, //StringComparer.OrdinalIgnoreCase.Equals("True", context.EnvironmentVariable("CAKE_SKIP_OPENCOVER"))
            Projects = projects,
            TestProjects = testProjects,
            ProjectFiles = projectFiles,
            TestProjectFiles = testProjectFiles,
            PackageIds = Util.GetPackageIds(context, packageIdsFiles)
        };
        context.Information($"Cake BuildParameters:-------------begin--------------");
        context.Information($"IsLocalBuild:{parameters.IsLocalBuild}");
        context.Information($"IsRunningOnUnix:{parameters.IsRunningOnUnix}");
        context.Information($"IsRunningOnWindows:{parameters.IsRunningOnWindows}");
        context.Information($"IsRunningOnTravisCI:{parameters.IsRunningOnTravisCI}");
        context.Information($"IsRunningOnAppVeyor:{parameters.IsRunningOnAppVeyor}");
        context.Information($"IsPullRequest:{parameters.IsPullRequest}");
        context.Information($"IsMasterBranch:{parameters.IsMasterBranch}");
        context.Information($"IsRunningOnAzurePipelines:{parameters.IsRunningOnAzurePipelines}");
        context.Information($"IsRunningOnAzurePipelinesHosted:{parameters.IsRunningOnAzurePipelinesHosted}");
        context.Information($"IsTagged:{parameters.IsTagged}");
        context.Information($"ShouldPublish:{parameters.ShouldPublish}");
        context.Information($"ShouldPublishToNuGet:{parameters.ShouldPublishToNuGet}");
        context.Information($"Cake BuildParameters:---------------end---------------");
        return parameters;
    }

    private static bool IsThePullRequest(BuildSystem buildSystem)
    {
        return (buildSystem.TravisCI.IsRunningOnTravisCI && StringComparer.OrdinalIgnoreCase.Equals("true", buildSystem.TravisCI.Environment.Repository.PullRequest)) || (buildSystem.AppVeyor.IsRunningOnAppVeyor && buildSystem.AppVeyor.Environment.PullRequest.IsPullRequest) || ((buildSystem.TFBuild.IsRunningOnAzurePipelines||buildSystem.TFBuild.IsRunningOnAzurePipelinesHosted) &&buildSystem.TFBuild.Environment.PullRequest.IsPullRequest);
    }

       private static bool IsTheMasterBranch(BuildSystem buildSystem)
    {
        return (buildSystem.TravisCI.IsRunningOnTravisCI && StringComparer.OrdinalIgnoreCase.Equals("master", buildSystem.TravisCI.Environment.Build.Branch)) || (buildSystem.AppVeyor.IsRunningOnAppVeyor && StringComparer.OrdinalIgnoreCase.Equals("master", buildSystem.AppVeyor.Environment.Repository.Branch)) || ((buildSystem.TFBuild.IsRunningOnAzurePipelines||buildSystem.TFBuild.IsRunningOnAzurePipelinesHosted) &&StringComparer.OrdinalIgnoreCase.Equals("master", buildSystem.TFBuild.Environment.Repository.SourceBranchName));
    }

    private static bool IsTheDevelopBranch(BuildSystem buildSystem)
    {
        return (buildSystem.TravisCI.IsRunningOnTravisCI && (StringComparer.OrdinalIgnoreCase.Equals("develop", buildSystem.TravisCI.Environment.Build.Branch) || StringComparer.OrdinalIgnoreCase.Equals("dev", buildSystem.TravisCI.Environment.Build.Branch))) || (buildSystem.AppVeyor.IsRunningOnAppVeyor && (StringComparer.OrdinalIgnoreCase.Equals("develop", buildSystem.AppVeyor.Environment.Repository.Branch) || StringComparer.OrdinalIgnoreCase.Equals("dev", buildSystem.AppVeyor.Environment.Repository.Branch))) || ((buildSystem.TFBuild.IsRunningOnAzurePipelines||buildSystem.TFBuild.IsRunningOnAzurePipelinesHosted) && (StringComparer.OrdinalIgnoreCase.Equals("develop", buildSystem.TFBuild.Environment.Repository.SourceBranchName)||StringComparer.OrdinalIgnoreCase.Equals("dev", buildSystem.TFBuild.Environment.Repository.SourceBranchName)));
    }

    private static bool IsBuildTagged(BuildSystem buildSystem)
    {
        return (buildSystem.IsRunningOnAppVeyor && buildSystem.AppVeyor.Environment.Repository.Tag.IsTag) ||
				(buildSystem.IsRunningOnTravisCI && !string.IsNullOrWhiteSpace(buildSystem.TravisCI.Environment.Build.Tag))||
                ((buildSystem.TFBuild.IsRunningOnAzurePipelines||buildSystem.TFBuild.IsRunningOnAzurePipelinesHosted) && buildSystem.TFBuild.Environment.Repository.SourceBranch.StartsWith("refs/tags"));
    }

    private static bool IsReleasing(string target)
    {
        var targets = new [] { "Publish", "Publish-NuGet", "Publish-Chocolatey", "Publish-HomeBrew", "Publish-GitHub-Release" };
        return targets.Any(t => StringComparer.OrdinalIgnoreCase.Equals(t, target));
    }

    private static bool IsPublishing(string target)
    {
        var targets = new [] { "ReleaseNotes", "Create-Release-Notes" };
        return targets.Any(t => StringComparer.OrdinalIgnoreCase.Equals(t, target));
    }
}

namespace Nubot.Plugins.Samples.TeamCity
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Model;

    public class TeamCityMessageBuilder
    {
        private readonly int _expectedBuildCount;

        public TeamCityMessageBuilder(int expectedBuildCount)
        {
            _expectedBuildCount = expectedBuildCount;
        }

        public string BuildMessage(IList<TeamCityModel> buildStatuses, string teamCityBaseUrl, out bool notify)
        {
            var success = buildStatuses.Count == _expectedBuildCount &&
                          buildStatuses.All(buildStatus => IsSuccesfullBuild(buildStatus.build));

            notify = !success;

            return success ? BuildSuccessMessage(buildStatuses.First().build, teamCityBaseUrl) :
                             BuildFailureMessage(buildStatuses.Select(m => m.build).ToList(), teamCityBaseUrl);
        }

        private static bool IsSuccesfullBuild(Build b)
        {
            return b.buildResult.Equals("success", StringComparison.InvariantCultureIgnoreCase);
        }

        private static string BuildSuccessMessage(Build build, string teamCityBaseUrl)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder
                .Append(
                    $@"<img src='{teamCityBaseUrl}/img/buildStates/buildSuccessful.png' height='16' width='16'/><strong>Successfully</strong> built {build.projectName} branch {build.branchName} with build number <a href=""{build.buildStatusUrl}""><strong>{build.buildNumber}</strong></a>");

            return stringBuilder.ToString();
        }

        private static string BuildFailureMessage(List<Build> builds, string teamCityBaseUrl)
        {
            var failedBuilds = builds.Where(b => !IsSuccesfullBuild(b)).ToList();

            var build = builds.First();
            var stringBuilder = new StringBuilder();

            stringBuilder
                .Append(
                    $@"<img src='{teamCityBaseUrl}/img/buildStates/buildFailed.png' height='16' width='16'/><strong>Failed</strong> to build {build.projectName} branch {build.branchName} with build number <a href=""{build.buildStatusUrl}""><strong>{build.buildNumber}</strong></a>. Failed build(s) ");

            stringBuilder.Append(
                string.Join(", ",
                    failedBuilds.Select(fb => $@"<a href=""{fb.buildStatusUrl}""><strong>{fb.buildName}</strong></a>")));

            return stringBuilder.ToString();
        }
    }
}
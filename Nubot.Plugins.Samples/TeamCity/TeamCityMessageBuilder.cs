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

        public string BuildMessage(IList<TeamCityModel> buildStatuses, out bool notify)
        {
            var success = buildStatuses.Count == _expectedBuildCount &&
                          buildStatuses.All(buildStatus => IsSuccesfullBuild(buildStatus.build));

            notify = !success;

            return success ? BuildSuccessMessage(buildStatuses.First().build) :
                             BuildFailureMessage(buildStatuses.Select(m => m.build).ToList());
        }

        private static bool IsSuccesfullBuild(Build b)
        {
            return b.buildResult.Equals("success", StringComparison.InvariantCultureIgnoreCase);
        }

        private static string BuildSuccessMessage(Build build)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder
                .AppendFormat( //todo externalize this in settings
                    @"<img src='http://ci.innoveo.com/img/buildStates/buildSuccessful.png' height='16' width='16'/><strong>Successfully</strong> built {0} branch {1} with build number <a href=""{2}""><strong>{3}</strong></a>",
                    build.projectName, build.branchName, build.buildStatusUrl, build.buildNumber);

            return stringBuilder.ToString();
        }

        private static string BuildFailureMessage(List<Build> builds)
        {
            var failedBuilds = builds.Where(b => !IsSuccesfullBuild(b)).ToList();

            var build = builds.First();
            var stringBuilder = new StringBuilder();

            stringBuilder
                .AppendFormat( //todo externalize this in settings
                    @"<img src='http://ci.innoveo.com/img/buildStates/buildFailed.png' height='16' width='16'/><strong>Failed</strong> to build {0} branch {1} with build number <a href=""{2}""><strong>{3}</strong></a>. Failed build(s) ",
                    build.projectName, build.branchName, build.buildStatusUrl, build.buildNumber);


            stringBuilder.Append(
                string.Join(", ",
                    failedBuilds.Select(fb => string.Format(@"<a href=""{0}""><strong>{1}</strong></a>", fb.buildStatusUrl, fb.buildName))));

            return stringBuilder.ToString();
        }
    }
}
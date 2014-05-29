namespace Nubot.Plugins.Samples.Stash
{
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Text;
    using Interfaces;
    using Models;
    using Nancy;
    using Nancy.ModelBinding;

    [Export(typeof(IRobotPlugin))]
    public class StashListener : HttpPluginBase
    {
        [ImportingConstructor]
        public StashListener(IRobot robot)
            : base("Stash Listener", "/stash", robot)
        {
            Post["/"] = x =>
            {
                var model = this.Bind<StashModel>();

                Robot.Message(BuildMessage(model));

                return HttpStatusCode.OK;
            };
        }

        private static string BuildMessage(StashModel model)
        {
            var authorNames = model.Changesets.Values.Select(v => v.ToCommit.Author.Name);
            var branches = model.RefChanges.Select(r => r.RefId.Replace("refs/heads/", ""));

            var stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("{0} committed to {1} branch at {2}/{3}", string.Join(", ", authorNames), model.RefChanges.Count(), model.Repository.Project.Name, model.Repository.Name);
            stringBuilder.AppendLine();
            stringBuilder.AppendFormat(@"On branch ""{0}""", string.Join(", ", branches));
            stringBuilder.AppendLine();

            foreach (var changeset in model.Changesets.Values)
            {
                stringBuilder.AppendFormat(@"- {0} ({1})", changeset.ToCommit.Message, changeset.ToCommit.DisplayId);
                stringBuilder.AppendLine();               
            }

            return stringBuilder.ToString();
        }

        /*
            Laurent Kempé committed to 1 branch at Skye/skye-editor
            On branch "feature/SKYE-1565-Refactoring-BindingContextCache"
            - SKYE-1565 Fix broken unit tests (a3ed1d2c4aa7)
            - SKYE-1565 Change the "Where" data and add searched text color highl... (06175ade7fb6)
         */

    }
}
namespace Nubot.Plugins.AppHarbor
{
    using System.ComponentModel.Composition;
    using Interfaces;
    using Models;

    [Export(typeof(IRobotPlugin))]
    public class AppharborListener : RobotPluginBase
    {
        public AppharborListener()
            : base("Appharbor Listener")
        {
            Robot.Router.Post<AppharborModel>("/appharbor", ((m, n) =>
            {
                var model = (AppharborModel)m;

                var message = string.Format("Your application {0} has been deployed with status: {1}", model.Application.Name, model.Build.Status);
                Robot.Message(message);

                return 200; //return HttpStatusCode.OK;
            }));
        }
    }
}


/*

 http://localhost:8080/appharbor
 
 Content-Type: application/json
  
 
{
  "application": {
    "name": "Foo"
  }, 
  "build": {
    "commit": {
      "id": "77d991fe61187d205f329ddf9387d118a09fadcd", 
      "message": "Implement foo"
    }, 
    "status": "succeeded"
  }
}
 
 */
namespace Nubot.Adapters
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;
    using agsXMPP;
    using agsXMPP.protocol.client;
    using agsXMPP.protocol.iq.roster;
    using agsXMPP.protocol.x.muc;
    using HipchatApiV2;
    using HipchatApiV2.Enums;
    using Interfaces;
    using Nubot.Interfaces.Message;

    [Export(typeof(IAdapter)),
    ExportMetadata("Name", "Hipchat"),
    ExportMetadata("Version", "0.1.0")]
    public class Hipchat : AdapterBase
    {
        private ConcurrentDictionary<string, string> _roster = new ConcurrentDictionary<string, string>();
        private XmppClientConnection _client;
        private List<Jid> JoinedRoomJids { get; set; }

        [ImportingConstructor]
        public Hipchat(IRobot robot)
            : base("Hipchat", robot)
        {
            JoinedRoomJids = new List<Jid>();
        }

        public override void Start()
        {
            _client = new XmppClientConnection(Robot.Settings.Get("HipChatServer"))
            {
                AutoResolveConnectServer = false
            };

            _client.OnLogin += OnLogin;
            _client.OnMessage += OnMessage;
            _client.OnRosterStart += OnRosterStart;
            _client.OnRosterItem += OnRosterItem;

            Robot.Logger.WriteLine("Connecting...");
            _client.Resource = Robot.Settings.Get("HipChatResource");
            _client.Open(Robot.Settings.Get("HipChatUser"), Robot.Settings.Get("HipChatPassword"));
            Robot.Logger.WriteLine("Connected.");
        }

        public override void Message(IMessage<string> message)
        {
            JoinedRoomJids.ForEach(jid => _client.Send(new Message(jid, _client.MyJID, MessageType.groupchat, message.Content)));
        }

        public override bool SendNotification(IMessage<Notification> notify)
        {
            var content = notify.Content;
            var client = new HipchatClient(content.AuthToken);
            return client.SendNotification(content.Room, content.HtmlMessage, RoomColors.Green, content.Notify.HasValue ? content.Notify.Value : false);
        }

        private void OnRosterItem(object sender, RosterItem item)
        {
            _roster.TryAdd(item.Jid.User, item.Name);
        }

        private void OnRosterStart(object sender)
        {
            _roster = new ConcurrentDictionary<string, string>();
        }

        private void OnLogin(object sender)
        {
            var mucManager = new MucManager(_client);

            var rooms = Robot.Settings.Get("HipChatRooms").Split(',');

            var roomJids = rooms.Select(room => new Jid(room + "@" + Robot.Settings.Get("HipChatConferenceServer")));

            foreach (var jid in roomJids)
            {
                mucManager.JoinRoom(jid, Robot.Settings.Get("HipChatRoomNick"));
                JoinedRoomJids.Add(jid);
            }
        }

        private void OnMessage(object sender, Message msg)
        {
            if (String.IsNullOrEmpty(msg.Body)) return;

            var user = GetUser(msg);

            if (MessageIsFromRobot(user)) return;

            Robot.Receive(msg.Body);
        }

        private bool MessageIsFromRobot(string user)
        {
            return user == Robot.Settings.Get("HipChatRoomNick");
        }

        private string GetUser(Message msg)
        {
            string user;
            if (msg.Type == MessageType.groupchat)
            {
                user = msg.From.Resource;
            }
            else
            {
                if (!_roster.TryGetValue(msg.From.User, out user))
                {
                    user = "Unknown User";
                }
            }
            return user;
        }
    }
}
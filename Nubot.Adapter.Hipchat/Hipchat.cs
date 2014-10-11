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
    using Message = agsXMPP.protocol.client.Message;

    [Export(typeof(IAdapter))]
    public class Hipchat : AdapterBase
    {
        private ConcurrentDictionary<string, string> _roster = new ConcurrentDictionary<string, string>();
        private XmppClientConnection _client;
        private readonly string _conferenceServer;
        private List<Jid> JoinedRoomJids { get; set; }

        [ImportingConstructor]
        public Hipchat(IRobot robot)
            : base("Hipchat", robot)
        {
            JoinedRoomJids = new List<Jid>();

            _conferenceServer = Robot.Settings.Get("HipChatConferenceServer");
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

        public override void Send(Envelope envelope, params string[] messages)
        {
            if (messages == null || !messages.Any()) return;

            var to = new Jid(envelope.User.Room);

            foreach (var message in messages)
            {
                _client.Send(new Message(to, string.Equals(to.Server, _conferenceServer, StringComparison.InvariantCultureIgnoreCase) ? MessageType.groupchat : MessageType.chat, message));
            }
        }

        public override bool SendNotification(string roomName, string authToken, string htmlMessage, bool notify = false)
        {
            var client = new HipchatClient(authToken);
            return client.SendNotification(roomName, htmlMessage, RoomColors.Green, notify);
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

            var roomJids = rooms.Select(room => new Jid(room + "@" + _conferenceServer));

            foreach (var jid in roomJids)
            {
                mucManager.JoinRoom(jid, Robot.Settings.Get("HipChatRoomNick"));
                JoinedRoomJids.Add(jid);
            }
        }

        private void OnMessage(object sender, Message msg)
        {
            if (String.IsNullOrEmpty(msg.Body)) return;

            var adapterUser = GetUser(msg);

            if (MessageIsFromRobot(adapterUser)) return;

            var user = new Interfaces.User(msg.Id, adapterUser, msg.From.Bare, Name);

            Robot.Receive(new TextMessage(user, msg.Body));
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
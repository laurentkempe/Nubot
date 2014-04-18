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
    using Interfaces;

    [Export(typeof(IAdapter))]
    public class Hipchat : IAdapter
    {
        private ConcurrentDictionary<string, string> _roster = new ConcurrentDictionary<string, string>();
        private XmppClientConnection _client;
        private readonly IRobot _robot;
        private List<Jid> JoinedRoomJids { get; set; }

        [ImportingConstructor]
        public Hipchat(IRobot robot)
        {
            _robot = robot;

            JoinedRoomJids = new List<Jid>();
        }

        public void Start()
        {
            _client = new XmppClientConnection(_robot.Settings.Get("HipChatServer"))
            {
                AutoResolveConnectServer = false
            };

            _client.OnLogin += OnLogin;
            _client.OnMessage += OnMessage;
            _client.OnRosterStart += OnRosterStart;
            _client.OnRosterItem += OnRosterItem;

            _robot.Logger.WriteLine("Connecting...");
            _client.Resource = _robot.Settings.Get("HipChatResource");
            _client.Open(_robot.Settings.Get("HipChatUser"), _robot.Settings.Get("HipChatPassword"));
            _robot.Logger.WriteLine("Connected.");
        }

        public void Message(string message)
        {
            JoinedRoomJids.ForEach(jid => _client.Send(new Message(jid, _client.MyJID, MessageType.groupchat, message)));
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

            var rooms = _robot.Settings.Get("HipChatRooms").Split(',');

            var roomJids = rooms.Select(room => new Jid(room + "@" + _robot.Settings.Get("HipChatConferenceServer")));

            foreach (var jid in roomJids)
            {
                mucManager.JoinRoom(jid, _robot.Settings.Get("HipChatRoomNick"));
                JoinedRoomJids.Add(jid);
            }
        }

        private void OnMessage(object sender, Message msg)
        {
            if (String.IsNullOrEmpty(msg.Body)) return;

            var user = GetUser(msg);

            if (MessageIsFromRobot(user)) return;

            _robot.Receive(msg.Body);
        }

        private bool MessageIsFromRobot(string user)
        {
            return user == _robot.Settings.Get("HipChatRoomNick");
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
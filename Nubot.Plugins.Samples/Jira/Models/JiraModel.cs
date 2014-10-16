namespace Nubot.Plugins.Samples.Jira.Models
{
    using System;

    // ReSharper disable InconsistentNaming

    public class JiraModel
    {
        public string webhookEvent { get; set; }
        public User user { get; set; }
        public Issue issue { get; set; }
        public Changelog changelog { get; set; }
        public CommentDetails comment { get; set; }
        public long timestamp { get; set; }
    }
    
    public class User
    {
        public string self { get; set; }
        public string name { get; set; }
        public string emailAddress { get; set; }
        public Avatarurls avatarUrls { get; set; }
        public string displayName { get; set; }
        public bool active { get; set; }
    }

    public class Avatarurls
    {
        public string _16x16 { get; set; }
        public string _24x24 { get; set; }
        public string _32x32 { get; set; }
        public string _48x48 { get; set; }
    }

    public class Issue
    {
        public string id { get; set; }
        public string self { get; set; }
        public string key { get; set; }
        public Fields fields { get; set; }
    }

    public class Fields
    {
        public string summary { get; set; }
        public Progress progress { get; set; }
        public Timetracking timetracking { get; set; }
        public Issuetype issuetype { get; set; }
        public object customfield_10080 { get; set; }
        public object timespent { get; set; }
        public User reporter { get; set; }
        public DateTime created { get; set; }
        public DateTime updated { get; set; }
        public Priority priority { get; set; }
        public string description { get; set; }
        public object[] issuelinks { get; set; }
        public object[] subtasks { get; set; }
        public Status status { get; set; }
        public object customfield_10090 { get; set; }
        public object[] labels { get; set; }
        public int workratio { get; set; }
        public object customfield_10220 { get; set; }
        public object customfield_10522 { get; set; }
        public Project project { get; set; }
        public object environment { get; set; }
        public object customfield_10050 { get; set; }
        public DateTime lastViewed { get; set; }
        public Aggregateprogress aggregateprogress { get; set; }
        public Component[] components { get; set; }
        public Comment comment { get; set; }
        public object timeoriginalestimate { get; set; }
        public object customfield_10820 { get; set; }
        public object customfield_10110 { get; set; }
        public Votes votes { get; set; }
        public object resolution { get; set; }
        public Fixversion[] fixVersions { get; set; }
        public object resolutiondate { get; set; }
        public Creator creator { get; set; }
        public object aggregatetimeoriginalestimate { get; set; }
        public string customfield_10120 { get; set; }
        public string customfield_10121 { get; set; }
        public object duedate { get; set; }
        public Watches watches { get; set; }
        public Worklog worklog { get; set; }
        public object customfield_10100 { get; set; }
        public User assignee { get; set; }
        public object aggregatetimeestimate { get; set; }
        public Version[] versions { get; set; }
        public object timeestimate { get; set; }
        public object customfield_10073 { get; set; }
        public object customfield_10072 { get; set; }
        public object aggregatetimespent { get; set; }
        public object customfield_10070 { get; set; }
    }

    public class Progress
    {
        public int progress { get; set; }
        public int total { get; set; }
    }

    public class Timetracking
    {
    }

    public class Issuetype
    {
        public string self { get; set; }
        public string id { get; set; }
        public string description { get; set; }
        public string iconUrl { get; set; }
        public string name { get; set; }
        public bool subtask { get; set; }
    }

    public class Priority
    {
        public string self { get; set; }
        public string iconUrl { get; set; }
        public string name { get; set; }
        public string id { get; set; }
    }

    public class Status
    {
        public string self { get; set; }
        public string description { get; set; }
        public string iconUrl { get; set; }
        public string name { get; set; }
        public string id { get; set; }
        public Statuscategory statusCategory { get; set; }
    }

    public class Statuscategory
    {
        public string self { get; set; }
        public int id { get; set; }
        public string key { get; set; }
        public string colorName { get; set; }
        public string name { get; set; }
    }

    public class Project
    {
        public string self { get; set; }
        public string id { get; set; }
        public string key { get; set; }
        public string name { get; set; }
        public Avatarurls avatarUrls { get; set; }
    }

    public class Aggregateprogress
    {
        public int progress { get; set; }
        public int total { get; set; }
    }

    public class Comment
    {
        public int startAt { get; set; }
        public int maxResults { get; set; }
        public int total { get; set; }
        public CommentDetails[] comments { get; set; }
    }

    public class CommentDetails
    {
        public string self { get; set; }
        public string id { get; set; }
        public User author { get; set; }
        public string body { get; set; }
        public User updateAuthor { get; set; }
        public DateTime created { get; set; }
        public DateTime updated { get; set; }
    }
    
    public class Votes
    {
        public string self { get; set; }
        public int votes { get; set; }
        public bool hasVoted { get; set; }
    }

    public class Creator
    {
        public string self { get; set; }
        public string name { get; set; }
        public string emailAddress { get; set; }
        public Avatarurls avatarUrls { get; set; }
        public string displayName { get; set; }
        public bool active { get; set; }
    }

    public class Watches
    {
        public string self { get; set; }
        public int watchCount { get; set; }
        public bool isWatching { get; set; }
    }

    public class Worklog
    {
        public int startAt { get; set; }
        public int maxResults { get; set; }
        public int total { get; set; }
        public object[] worklogs { get; set; }
    }

    public class Component
    {
        public string self { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
    }

    public class Fixversion
    {
        public string self { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public bool archived { get; set; }
        public bool released { get; set; }
        public string releaseDate { get; set; }
    }

    public class Version
    {
        public string self { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public bool archived { get; set; }
        public bool released { get; set; }
        public string releaseDate { get; set; }
    }

    public class Changelog
    {
        public string id { get; set; }
        public ChangelogItem[] items { get; set; }
    }

    public class ChangelogItem
    {
        public string field { get; set; }
        public string fieldtype { get; set; }
        public string from { get; set; }
        public string fromString { get; set; }
        public string to { get; set; }
        public string toString { get; set; }
    }

    // ReSharper restore InconsistentNaming
}
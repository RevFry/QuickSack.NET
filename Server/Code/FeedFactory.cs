using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Extensions.Caching.Memory;
using QuickSack.Shared;

namespace QuickSack.Server.Code
{
    public interface IFeedFactory
    {
        List<FeedItem> GetFeedItems();
    }

    public class FeedFactory : IFeedFactory
    {
        private readonly IMemoryCache memoryCache;
        public FeedFactory(IMemoryCache cache) => memoryCache = cache;

        public List<FeedItem> GetFeedItems()
        {
            var feedItems = memoryCache.GetOrCreate("FeedKey", entry =>
                {
                    string url_notes = "https://briandunaway.com/category/filmsack-notes/feed/";
                    string url = "http://feeds.frogpants.com/filmsack_feed.xml";
                    string url_archive = "http://feeds.frogpants.com/filmsack_feed_old.xml";

                    List<NoteItem> notes = GetNotesFeed(url_notes);

                    List<FeedItem> items = GetFeed(url);

                    items = MergeNotes(items, notes);

                    items.AddRange(GetFeed(url_archive));

                    entry.SlidingExpiration = TimeSpan.FromHours(1);
                    return items;
                });

            return feedItems;
        }

        protected List<FeedItem> MergeNotes(List<FeedItem> feeds, List<NoteItem> notes)
        {
            feeds.ForEach(feed =>
            {
                if (feed.Title.IndexOf(':') < 0){ return; }
                var feedTitle = feed.Title.ToLower().Split(':')[1].Trim();
                var noteCandidates = notes.Where(n => n.Title.Contains(feedTitle)).ToList();
                if (noteCandidates.Any())
                {
                    feed.ShowNotesUrl = noteCandidates.First().Link;
                }
            });
            return feeds;
        }

        protected List<NoteItem> GetNotesFeed(string url)
        {
            List<NoteItem> noteItems = new();

            XDocument document = XDocument.Load(url);

            noteItems = document.Root.Descendants().First(i => i.Name.LocalName == "channel")
                .Elements().Where(i => i.Name.LocalName == "item")
                .Select(x => new NoteItem
                {
                    Title = x.Elements()?.Where(i => i.Name.LocalName == "title")?.FirstOrDefault()?.Value?.ToLower() ?? string.Empty,
                    Link = x.Elements()?.Where(i => i.Name.LocalName == "link")?.FirstOrDefault()?.Value ?? string.Empty
                }).ToList();

            return noteItems;
        }

        private List<FeedItem> GetFeed(string url)
        {
            List<FeedItem> feedItems = new();
            
            XNamespace itunes = "http://www.itunes.com/dtds/podcast-1.0.dtd";
            XDocument document = XDocument.Load(url);

            feedItems = document.Root.Descendants().First(i => i.Name.LocalName == "channel")
                .Elements().Where(i => i.Name.LocalName == "item")
                .Select(x => new FeedItem
                {
                    Title = x.Elements()?.Where(i => i.Name.LocalName == "title")?.FirstOrDefault()?.Value ?? string.Empty,
                    Description = x.Elements()?.Where(i => i.Name == itunes + "summary")?.FirstOrDefault()?.Value ?? string.Empty,
                    Link = x.Elements()?.Where(i => i.Name.LocalName == "enclosure")?.FirstOrDefault()?.Attribute("url")?.Value ?? string.Empty,
                    PublishDate = DateTime.Parse(x.Elements()?.Where(i => i.Name.LocalName == "pubDate")?.FirstOrDefault()?.Value ?? DateTime.Now.ToString())
                }).ToList();
            
            return feedItems;
        }

    }
}

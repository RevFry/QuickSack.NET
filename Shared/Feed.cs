namespace QuickSack.Shared;

public class FeedItem
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string Link { get; set; }
    public DateTime PublishDate { get; set; }
    public string ShowNotesUrl { get; set; } = string.Empty;
    public List<string> Tags
    {
        get
        {
            List<string> results = Title.ToLower().Replace(':', ' ').Split(' ').ToList();
            results.AddRange(Description.ToLower().Split(' ').ToList());
            results.Add(PublishDate.Year.ToString());
            return results;
        }
    }
}

public class NoteItem
{
    public string Title { get; set; }
    public string Link { get; set; }
}

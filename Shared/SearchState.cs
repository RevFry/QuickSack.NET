namespace QuickSack.Shared;

public class SearchState
{
    public string SearchText { get; private set; } = "";

    public List<string> SearchTags => SearchText.ToLower().Split(' ').ToList();

    public event Action OnChange;

    public void SetSearch(string search)
    {
        SearchText = search;
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();

    public SearchState()
    {
    }
}

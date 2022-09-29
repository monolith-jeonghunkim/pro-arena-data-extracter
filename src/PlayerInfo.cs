public class PlayerInfo
{
    public int? Id { get; set; }
    public int? Kind { get; set; }
    public Profile? profile { get; set; }
    public Statistics? statistics { get; set; }
    public string? SerializedStoredItem { get; set; }
}
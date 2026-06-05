namespace InterReact;

[Message]
public sealed record IneligibilityReason
{
    public string Id { get; init; } = "";

    public string Description { get; init; } = "";
}

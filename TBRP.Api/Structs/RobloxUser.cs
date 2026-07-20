namespace TBRP.Api.Structs;

// ReSharper disable once ClassNeverInstantiated.Global
// ReSharper disable once UnusedAutoPropertyAccessor.Global
public class RobloxUser
{
    public required long Id { get; init; }
    public required string Name { get; init; }
    public required string DisplayName { get; init; }
    public required bool HasVerifiedBadge { get; init; }
}
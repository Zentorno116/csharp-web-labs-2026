namespace SignalRChat.Models;

public record RoomSummaryDto(
    string Name,
    string Description,
    string OwnerUsername,
    bool IsDefault,
    int OnlineCount);

public record RoomUserDto(
    string Username,
    string AvatarColor,
    string Role,
    bool IsMuted,
    DateTime? MutedUntilUtc);

public record MessageDto(
    string Id,
    string RoomName,
    string Sender,
    string SenderRole,
    string AvatarColor,
    string Text,
    DateTime TimestampUtc,
    bool IsSystem,
    bool IsDeleted);

public record RoomSnapshotDto(
    string Name,
    string Description,
    string OwnerUsername,
    bool IsDefault,
    string CurrentUserRole,
    List<RoomUserDto> Users,
    List<MessageDto> Messages,
    List<string> BannedUsers);

public record OperationResult(bool Success, string? Error = null);

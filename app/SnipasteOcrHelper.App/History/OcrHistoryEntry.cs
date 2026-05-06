namespace SnipasteOcrHelper.History;

public sealed record OcrHistoryEntry(
    DateTimeOffset Timestamp,
    string FileName,
    OcrHistoryStatus Status,
    string Detail);

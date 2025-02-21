using System;
using System.Collections.Generic;

namespace FinanManager.Models;

public partial class AuditLog
{
    public int AuditId { get; set; }

    public string TableName { get; set; } = null!;

    public string RecordId { get; set; } = null!;

    public string Operation { get; set; } = null!;

    public string? OldValues { get; set; }

    public string? NewValues { get; set; }

    public string? ChangedColumns { get; set; }

    public int UserId { get; set; }

    public string? IpAddress { get; set; }

    public string? SessionId { get; set; }

    public string? ApplicationName { get; set; }

    public string? Reason { get; set; }

    public DateTime Timestamp { get; set; }

    public virtual User User { get; set; } = null!;
}

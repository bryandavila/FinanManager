using System;
using System.Collections.Generic;

namespace FinanManager.Models;

public partial class FailedLoginAttempt
{
    public int AttemptId { get; set; }

    public int UserId { get; set; }

    public string UserEmail { get; set; } = null!;

    public DateTime? AttemptDate { get; set; }

    public virtual User User { get; set; } = null!;
}

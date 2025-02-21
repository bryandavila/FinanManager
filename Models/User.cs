using System;
using System.Collections.Generic;

namespace FinanManager.Models;

public partial class User
{
    public int Users_Id { get; set; }

    public string Name { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public DateTime? CreatedDate { get; set; }

    public int UserStatus { get; set; }

    public string UserEmail { get; set; } = null!;

    public int role_ID { get; set; }

    public virtual ICollection<Accounting> Accountings { get; set; } = new List<Accounting>();

    public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();

    public virtual ICollection<FailedLoginAttempt> FailedLoginAttempts { get; set; } = new List<FailedLoginAttempt>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual Role Role { get; set; } = null!;
}

using System;
using System.Collections.Generic;

namespace FinanManager.Models;

public partial class ErrorMessage
{
    public int ErrorMessagesId { get; set; }

    public string ErrorMsg { get; set; } = null!;

    public string ErrorType { get; set; } = null!;
}

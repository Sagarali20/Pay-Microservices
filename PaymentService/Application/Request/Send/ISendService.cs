using System.Data.Common;
﻿using Common;
using PaymentService.Application.Request.Send.Command;

namespace PaymentService.Application.Request.Send
{
    public interface ISendService
    {
        Task<Result> SendMoney(AddOrEditTransaction request);
        Task<Result> SendNotification(SendNotification request);
    }
}

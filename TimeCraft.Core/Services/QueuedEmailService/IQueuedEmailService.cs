﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeCraft.Core.Services.QueuedEmailService
{
    public interface IQueuedEmailService<T> : ICrudOperations<T> where T : class
    {
    }
}

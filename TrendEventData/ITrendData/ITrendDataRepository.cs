﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrendEventData
{
    public interface ITrendDataRepository
    {
        
            Task TrendDataInsertionAsync(TrendDataTableEntity entity);
           // Task<List<TrendDataTableEntity>> TrendDataRetrievalAsync(string partitionKey, DateTime startTime, DateTime endTime);
       

    }
}

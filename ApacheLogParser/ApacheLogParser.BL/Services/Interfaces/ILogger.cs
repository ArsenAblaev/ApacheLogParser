﻿namespace ApacheLogParser.BL.Services.Interfaces
{
   public interface ILogger
   {
       void Error(string message);
       void Info(string message);
       void Debug(string messaga);
   }
}

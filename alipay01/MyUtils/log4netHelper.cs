
using log4net;
using System;
using System.Reflection;
namespace mvcDemo01.MyUtils
{
    public class log4netHelper
	    {
        public static void debug(string loggerName, string message)
	        {
                log4net.ILog log = LogManager.GetLogger(loggerName);
	            if (log.IsDebugEnabled)
	            {
	                log.Debug(message);
	            }
	            log = null;
	        }
        public static void error(string loggerName, string message)
	        {
                log4net.ILog log = LogManager.GetLogger(loggerName);
	            if (log.IsErrorEnabled)
	            {
	                log.Error(message);
	            }
	            log = null;
	        }


            public static void fatal(string loggerName, string message)
	        {

                log4net.ILog log = LogManager.GetLogger(loggerName);
	            if (log.IsFatalEnabled)
	            {
	                log.Fatal(message);
	            }
	            log = null;
	        }
            public static void info(string loggerName, string message)
	        {
                log4net.ILog log = LogManager.GetLogger(loggerName);
	            if (log.IsInfoEnabled)
	            {
	                log.Info(message);
	            }
	            log = null;
	        }
	
	        public static void warn(string loggerName,string message)
	        {
                log4net.ILog log = log4net.LogManager.GetLogger(loggerName);
	            if (log.IsWarnEnabled)
	            {
	                log.Warn(message);
	            }
	            log = null;
	        } 
	    }
}
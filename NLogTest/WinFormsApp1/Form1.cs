using System;
using System.Windows.Forms;
using NLog;
using NLog.Targets;

/// <summary>
/// Testing NLog to database.
/// </summary>
namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            // Enable:ThrowExceptions
            LogManager.ThrowExceptions = true;

            var config = new NLog.Config.LoggingConfiguration();

            // Targets where to log to: File
            var logfile = new NLog.Targets.FileTarget("logfile") { FileName = @"c:\temp\NsLogfile.txt" };

            // Targets where to log to:  Console
            var logconsole = new NLog.Targets.ConsoleTarget("logconsole");

            //Targets where to log to : DataBase.
            var logDB = new NLog.Targets.DatabaseTarget("LogDB");

            //Set Provider for Oracle: Oracle ManagedDataAccess.Core.
            logDB.DBProvider = "Oracle.ManagedDataAccess.Client.OracleConnection, Oracle.ManagedDataAccess";

            //!!!Connectstring important!
            logDB.ConnectionString = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=172.20.20.224)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=mil3)));User Id=MAST;Password=MAST;";
            
            //!!! SQL Command logging.
            logDB.CommandText = "INSERT INTO  HIS.LOGTABLE (TIME_STAMP,LOGLEVEL,LOGGER,MESSAGE) VALUES (:TIME_STAMP, :LOGLEVEL, :LOGGER, :MESSAGE)";
            
            // all of the parameters.
            logDB.Parameters.Add(new DatabaseParameterInfo { Name = ":TIME_STAMP", Layout = "${date}"});
            logDB.Parameters.Add(new DatabaseParameterInfo { Name = ":LOGLEVEL", Layout = "${level}" });
            logDB.Parameters.Add(new DatabaseParameterInfo { Name = ":LOGGER", Layout = "${logger}" });
            logDB.Parameters.Add(new DatabaseParameterInfo { Name = ":MESSAGE", Layout = "${message}" });

            // Add rule:
            config.AddRule(LogLevel.Info, LogLevel.Fatal, logconsole);
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);
            config.AddRule(LogLevel.Trace, LogLevel.Fatal, logDB);

            // Apply config           
            NLog.LogManager.Configuration = config;
        }

        private void button1_Click(object sender, EventArgs e)
        {
             NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
            try
            {
                Logger.Debug("action: debug");
                Logger.Trace("action: trace to Oracle..., But Success!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Logger.Error(ex, "Goodbye cruel world");
            }
        }
    }
}

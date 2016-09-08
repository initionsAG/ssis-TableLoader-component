using System;
using System.Collections.Generic;
using System.Text;

namespace TableLoader.Log
{
    /// <summary>
    /// Statistics/Status for each TableLoader thread
    /// </summary>
    public class Status
    {
        /// <summary>
        /// TableLoader status
        /// </summary>
        public enum TableLoaderStatusType
        {
            started,
            finished,
            preExecStarted,
            preExecFinished,
            processInputStarted,
            processInpitFinished,
            postExecStarted,
            postExecFinished
        }

        /// <summary>
        /// Sql status
        /// </summary>
        public enum StatusType
        {
            dataTableCreated,
            bulkCopyThreadCreated,
            bulkCopyThreadFinished,
            dbJobQueued,
            dbJobFinished,
            bulkCopyFinished,
            dbThreadStatistic,
            dbThreadProcessingDataTable,
            dbThreadCountTempTable

        }

        /// <summary>
        /// Isag events
        /// </summary>
        private IsagEvents _events;

        /// <summary>
        /// statistic dictionary
        /// Key: thread number
        /// Value: TableLoader statistic
        /// </summary>
        private Dictionary<int, TableLoaderStatisic> _statistic = new Dictionary<int, TableLoaderStatisic>();

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="events">Isag events</param>
        public Status(IsagEvents events)
        {
            _events = events;
        }

        /// <summary>
        /// Get statistic by thread number
        /// </summary>
        /// <param name="threadNr">thread number</param>
        /// <returns>statistic for a thread</returns>
        private TableLoaderStatisic GetStatisticForThreadNumber(int threadNr)
        {
            if (!_statistic.ContainsKey(threadNr)) 
            {
                _statistic.Add(threadNr, new TableLoaderStatisic(threadNr));
            }

            return _statistic[threadNr];
        }

        /// <summary>
        /// Log statistic by fireing an SSIS event
        /// </summary>
        public void LogStatistic()
        {
            foreach (int key in _statistic.Keys)
            {
                _events.Fire(IsagEvents.IsagEventType.Status, _statistic[key].ToString());
            }
        }

        /// <summary>
        /// Is used for statistics? (otherwise it is only logged) 
        /// </summary>
        /// <param name="statusType"></param>
        /// <returns>Is used for statistics?</returns>
        private bool IsStatisticEvent(Status.StatusType statusType)
        {
            return statusType != StatusType.bulkCopyFinished && statusType != StatusType.dbThreadStatistic && statusType != StatusType.dbThreadProcessingDataTable;
        }

        /// <summary>
        /// ADd statistic
        /// </summary>
        /// <param name="statusEvent">status event</param>
        private void AddStatistic(StatusEvent statusEvent)
        {
            TableLoaderStatisic statistic = GetStatisticForThreadNumber(statusEvent.Nr);
            switch (statusEvent.StatusType)
            {
                case StatusType.dataTableCreated:
                    statistic.DataTableCreated = true;
                    statistic.CountDataTable = statusEvent.Count;
                    break;
                case StatusType.bulkCopyThreadCreated:
                    statistic.BulkCopyThreadCreated = true;
                    break;
                case StatusType.bulkCopyThreadFinished:
                    statistic.BulkCopyThreadFinished = true;
                    break;
                case StatusType.dbJobQueued:
                    statistic.DbCommandJobCreated = true;
                    break;
                case StatusType.dbJobFinished:
                    statistic.CountRowsAffected = statusEvent.Count;
                    statistic.DbCommandJobFinished = true;
                    break;
                case StatusType.dbThreadCountTempTable:
                    statistic.CountTempTable = statusEvent.Count;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Add status
        /// </summary>
        /// <param name="nr">thread number</param>
        /// <param name="count">number of data rows</param>
        /// <param name="statusType">status type</param>
        /// <param name="eventType">event type</param>
        public void AddStatus(int nr, int count, StatusType statusType, IsagEvents.IsagEventType eventType)
        {
            AddStatus(nr, count, statusType, DateTime.Now, eventType);
        }

        /// <summary>
        /// Add status
        /// </summary>
        /// <param name="nr">thread number</param>
        /// <param name="count">number of data rows</param>
        /// <param name="statusType">status type</param>
        /// <param name="timestamp">timestamp</param>
        /// <param name="eventType">event type</param>
        public void AddStatus(int nr, int count, StatusType statusType, DateTime timestamp, IsagEvents.IsagEventType eventType)
        {
            AddStatus(new StatusEvent(nr, count, statusType, timestamp, eventType));
        }

        /// <summary>
        /// Add status
        /// </summary>
        /// <param name="statusEvent">status event</param>
        public void AddStatus(StatusEvent statusEvent)
        {
            LogStatus(statusEvent);
            if (IsStatisticEvent(statusEvent.StatusType)) AddStatistic(statusEvent);
        }


        /// <summary>
        /// Add TableLoader status
        /// </summary>
        /// <param name="tlStatus">TableLoader status type</param>
        public void AddTableLoaderStatus(TableLoaderStatusType tlStatus)
        {
            LogTableLoaderStatus(tlStatus);
        }

        /// <summary>
        /// Log status
        /// </summary>
        /// <param name="statusEvent">status event</param>
        private void LogStatus(StatusEvent statusEvent)
        {
            string message = "";

            switch (statusEvent.StatusType)
            {
                case StatusType.dataTableCreated:
                    message = "DataTable {0} created with {1} rows. [{2}]";
                    break;
                case StatusType.bulkCopyThreadCreated:
                    message = "BulkCopy Thread {0} created (Datatable {0} : {1} rows) [{2}]";
                    break;
                case StatusType.bulkCopyThreadFinished:
                    message = "BulkCopy Thread {0} finished [{2}].";
                    break;
                case StatusType.dbJobQueued:
                    message = "BulkCopy Thread {0} in DB Command Queue eingetragen [{2}]";
                    break;
                case StatusType.dbJobFinished:
                    message = "DB Command Job {0}: DB Command for TempTable " + statusEvent.Param1 + " of BulkCopy Thread {0} finished, {1} rows have been affected [{2}]";
                    break;
                case StatusType.bulkCopyFinished:
                    message = "BulkCopy Thread {0}: {1} rows written to temporary table " + statusEvent.Param1 + " [{2}]";
                    break;
                case StatusType.dbThreadStatistic:
                    message = "{0}/{1} DB Commands finished [{2}]";
                    break;
                case StatusType.dbThreadProcessingDataTable:
                    message = "DB Command Job {0}: Processing TempTable " + statusEvent.Param1 + " from BulkCopy Thread {0} [{2}]";
                    break;
                case StatusType.dbThreadCountTempTable:
                    message = "DB Command Job {0}: TempTable " + statusEvent.Param1 + " contains {1} rows. [{2}]";
                    break;
                default:
                    break;
            }

            message = "XXXX" + string.Format(message, statusEvent.Nr.ToString(), statusEvent.Count.ToString(), GetTimestamp(statusEvent.Timestamp));
            _events.Fire(statusEvent.EventType, message);
        }

        /// <summary>
        /// Log TableLoader status
        /// </summary>
        /// <param name="tlStatus">TableLoader status</param>
        private void LogTableLoaderStatus(TableLoaderStatusType tlStatus)
        {
            string message = "";
            switch (tlStatus)
            {
                case TableLoaderStatusType.started:
                    message = "TableLoader started";
                    break;
                case TableLoaderStatusType.finished:
                    message = "TableLoader finished";
                    break;
                case TableLoaderStatusType.preExecStarted:
                    message = "TableLoader Pre Execution Phase started";
                    break;
                case TableLoaderStatusType.preExecFinished:
                    message = "TableLoader Pre Execution Phase finished";
                    break;
                case TableLoaderStatusType.processInputStarted:
                    message = "TableLoader Process Input started";
                    break;
                case TableLoaderStatusType.processInpitFinished:
                    message = "TableLoader Process Input finished";
                    break;
                case TableLoaderStatusType.postExecStarted:
                    message = "TableLoader Post Execution Phase started";
                    break;
                case TableLoaderStatusType.postExecFinished:
                    message = "TableLoader Post Execution Phase finieshed";
                    break;
                default:
                    break;
            }

            message += string.Format(" [{0}]", GetTimestamp());

            _events.Fire(IsagEvents.IsagEventType.Status, message);
        }

        /// <summary>
        /// Get current date, time and milliseconds as string
        /// </summary>
        /// <returns>current date, time and milliseconds as string</returns>
        private string GetTimestamp()
        {
            return GetTimestamp(DateTime.Now);
        }

        /// <summary>
        /// Get current date, time and milliseconds
        /// </summary>
        /// <param name="timestamp">current datetime</param>
        /// <returns>current date, time and milliseconds as string</returns>
        private string GetTimestamp(DateTime timestamp)
        {
            return DateTime.Now.ToString() + ":" + DateTime.Now.Millisecond.ToString();
        }
    }

    /// <summary>
    /// Statistic for TableLoader
    /// </summary>
    class TableLoaderStatisic
    {
        /// <summary>
        /// Thread number (statistic is valid for that thread)
        /// </summary>
        public int ThreadNumber { get; set; }
        /// <summary>
        /// Has datatable been created?
        /// </summary>
        public bool DataTableCreated { get; set; }
        /// <summary>
        /// Bulkcopy thread created?
        /// </summary>
        public bool BulkCopyThreadCreated { get; set; }
        /// <summary>
        /// Bulkcopy thread finished?
        /// </summary>
        public bool BulkCopyThreadFinished { get; set; }
        /// <summary>
        /// Db command job created?
        /// </summary>
        public bool DbCommandJobCreated { get; set; }
        /// <summary>
        /// Db command job finished?
        /// </summary>
        public bool DbCommandJobFinished { get; set; }
        /// <summary>
        /// Number of rows in datatable
        /// </summary>
        public int CountDataTable { get; set; }
        /// <summary>
        /// number of rows in temporary table 
        /// </summary>
        public int CountTempTable { get; set; }
        /// <summary>
        /// numer of rows affected by db command
        /// </summary>
        public int CountRowsAffected { get; set; }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="threadNr">thread number</param>
        public TableLoaderStatisic(int threadNr)
        {
            ThreadNumber = threadNr;
            DataTableCreated = false;
            BulkCopyThreadCreated = false;
            BulkCopyThreadFinished = false;
            DbCommandJobCreated = false;
            DbCommandJobFinished = false;
            CountDataTable = -1;
            CountTempTable = -1;
            CountRowsAffected = -1;
        }

        /// <summary>
        /// Description of statistic
        /// </summary>
        /// <returns>description of statistic</returns>
        public override string ToString()
        {
            string result;

            result = "{0} | DT:{1} | BCcr:{2} | BCfin:{3} | DBCcr:{4} | DBfin:{5} | Count for DT/TT/RowsAffected: {6}/{7}/{8}";
            result = string.Format(result, ThreadNumber.ToString(), DataTableCreated.ToString(), BulkCopyThreadCreated.ToString(), BulkCopyThreadFinished.ToString(), DbCommandJobCreated.ToString(),
                                   DbCommandJobFinished.ToString(), CountDataTable.ToString(), CountTempTable.ToString(), CountRowsAffected.ToString());
            return result;
        }
    }
}

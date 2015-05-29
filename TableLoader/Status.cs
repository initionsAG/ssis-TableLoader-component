using System;
using System.Collections.Generic;
using System.Text;

namespace TableLoader
{
    public class Status
    {

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

        private IsagEvents _events;
        private Dictionary<int, TableLoaderStatisic> _statistic = new Dictionary<int, TableLoaderStatisic>();

        public Status(IsagEvents events)
        {
            _events = events;
        }

        private TableLoaderStatisic GetStatisticcForThreadNumber(int threadNr)
        {
            if (!_statistic.ContainsKey(threadNr)) 
            {
                _statistic.Add(threadNr, new TableLoaderStatisic(threadNr));
            }

            return _statistic[threadNr];
        }

        public void LogStatistic()
        {
            foreach (int key in _statistic.Keys)
            {
                _events.Fire(IsagEvents.IsagEventType.Status, _statistic[key].ToString());
            }
        }
        private bool IsStatisticEvent(Status.StatusType statusType)
        {
            return statusType != StatusType.bulkCopyFinished && statusType != StatusType.dbThreadStatistic && statusType != StatusType.dbThreadProcessingDataTable;
        }

        private void AddStatistic(StatusEvent statusEvent)
        {
            TableLoaderStatisic statistic = GetStatisticcForThreadNumber(statusEvent.Nr);
            switch (statusEvent.StatusType)
            {
                case StatusType.dataTableCreated:
                    statistic.DataTableCreated = true;
                    statistic.CountDateTable = statusEvent.Count;
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




        public void AddStatus(int nr, int count, StatusType statusType, IsagEvents.IsagEventType eventType)
        {
            AddStatus(nr, count, statusType, DateTime.Now, eventType);
        }

        public void AddStatus(int nr, int count, StatusType statusType, DateTime timestamp, IsagEvents.IsagEventType eventType)
        {
            AddStatus(new StatusEvent(nr, count, statusType, timestamp, eventType));
        }

        public void AddStatus(StatusEvent statusEvent)
        {
            LogStatus(statusEvent);
            if (IsStatisticEvent(statusEvent.StatusType)) AddStatistic(statusEvent);
        }



        public void AddTableLoaderStatus(TableLoaderStatusType tlStatus)
        {
            LogTableLoaderStatus(tlStatus);
        }



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

        private string GetTimestamp()
        {
            return GetTimestamp(DateTime.Now);
        }
        private string GetTimestamp(DateTime timestamp)
        {
            return DateTime.Now.ToString() + ":" + DateTime.Now.Millisecond.ToString();
        }
    }

    class TableLoaderStatisic
    {
        public int ThreadNumber { get; set; }
        public bool DataTableCreated { get; set; }
        public bool BulkCopyThreadCreated { get; set; }
        public bool BulkCopyThreadFinished { get; set; }
        public bool DbCommandJobCreated { get; set; }
        public bool DbCommandJobFinished { get; set; }
        public int CountDateTable { get; set; }
        public int CountTempTable { get; set; }
        public int CountRowsAffected { get; set; }

        public TableLoaderStatisic(int threadNr)
        {
            ThreadNumber = threadNr;
            DataTableCreated = false;
            BulkCopyThreadCreated = false;
            BulkCopyThreadFinished = false;
            DbCommandJobCreated = false;
            DbCommandJobFinished = false;
            CountDateTable = -1;
            CountTempTable = -1;
            CountRowsAffected = -1;
        }

        public override string ToString()
        {
            string result;

            result = "{0} | DT:{1} | BCcr:{2} | BCfin:{3} | DBCcr:{4} | DBfin:{5} | Count for DT/TT/RowsAffected: {6}/{7}/{8}";
            result = string.Format(result, ThreadNumber.ToString(), DataTableCreated.ToString(), BulkCopyThreadCreated.ToString(), BulkCopyThreadFinished.ToString(), DbCommandJobCreated.ToString(),
                                   DbCommandJobFinished.ToString(), CountDateTable.ToString(), CountTempTable.ToString(), CountRowsAffected.ToString());
            return result;
        }
    }
}

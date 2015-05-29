using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.SqlServer.Dts.Pipeline;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using System.Data.SqlClient;
using Microsoft.SqlServer.Dts.Runtime.Wrapper;
using System.Data;
using System.Transactions;
using System.EnterpriseServices;
using System.Windows.Forms;
using Microsoft.SqlServer.Dts.Runtime;
using System.Collections;
using System.Data.Common;
using System.Data.OleDb;
using System.ComponentModel;
using System.Threading;
using TableLoader.ComponentFramework.Mapping;

namespace TableLoader
{


    /// <summary>
    /// 08.04.2011, Dennis Weise
    ///     - Branch von TableLoader Version 2.3.4
    ///     - Neue Assembly erstellt
    /// 08.04.2011, Dennis Weise
    ///     - Die Konfiguration wird wieder in den Custom Properties statt in der DB gespeichert
    /// 11.04.2011, Dennis Weise
    ///     - Standardkonfiguration neu implementiert
    /// 11.04.2011, Dennis Weise
    ///     - Nicht mehr benötigten Code entfernt
    ///     - Event Messages sind nun alle auf englisch
    /// 11.04.2011, Dennis Weise
    ///     - Farbe der Controls auf Transparent, bzw. Window gesetzt, so dass die weißen Hintergründe auf nicht Windows 7 Rechnern verschwinden
    /// 11.04.2011, Dennis Weise
    ///     - Ist eine Standardkonfiguration ausgewählt, so werden die Controls für die Standardwerte deaktiviert
    /// 12.04.2011, Dennis Weise
    ///     - Version 3.0.0
    /// 12.04.2011, Dennis Weise
    ///     - MessageBoxen in der GUI: Text wird wieder angezeigt, Ins englische übersetzt
    /// 19.04.2011, Dennis Weise
    ///     - Bei Prüfung auf doppelt zugeordnete Output Spalten werden Leerstrings (= nicht zugeordnet) ignoriert
    /// 19.04.2011, Dennis Weise
    ///     - Version 3.0.1
    /// 20.05.2011, Dennis Weise
    ///     - Es ist nun möglich die Zuweisungen zu OutputColumns aller selektierten Zeilen zu entfernen
    /// 20.05.2011, Dennis Weise
    ///     - Version 3.0.2
    /// 21.06.2011, Dennis Weise
    ///     - Es kann nun nach Spalten sortiert werden
    ///     - Wenn die Standardkonfigurationen nicht geladen werden können, wird eine Fehlermeldung nur dann angezeigt, 
    ///       wenn der TableLoader diesbezgl. auf AutoLoad steht
    ///     - Version 3.0.3
    /// 27.06.2011, Dennis Weise
    ///     - Das Entfernen von Zuordnungen zwischen Input- und OuputColumns funktionierte aufgrund der Sortierfunktion nicht mehr korrekt
    /// 27.06.2011, Dennis Weise
    ///     - Version 3.0.4
    /// 19.10.2011, Dennis Weise
    ///     - Version 3.0.5 
    ///     - DB Command Merge: DB Command wird nun auch in Threads abgearbeiten
    /// 19.10.2011, Dennis Weise
    ///     - Automap: Kann nun auf Selektion eingeschränkt werden
    /// 19.10.2011, Dennis Weise
    ///     - UpdateErrorStatus() wird nun in BulkCopyThread.WaitForAll aufgerufen
    /// 04.11.2011, Dennis Weise
    ///     - tablockx für Merge hinzugefügt
    /// 04.11.2011, Dennis Weise
    ///     - Version 3.0.6 
    /// 18.11.2011, Dennis Weise
    ///     - Threads: Bei einem Timeout wird der BulkCopy, bzw. das DbCommand erneut ausgeführt
    /// 18.11.2011, Dennis Weise
    ///     - Version 3.0.7
    /// 09.01.2012, Dennis Weise
    ///     - Beim Öffnen des TableLoaders wird geprüft ob der eingestellte Tabellenname sich bzgl. der Groß-/Kleinschreibung geändert hat und automatisch korrigiert
    /// 09.01.2012, Dennis Weise
    ///     - Version 3.0.8
    /// 23.01.2012, Dennis Weise
    ///     - Überarbeitung des Multithreading
    ///     - SqlConnections werden nun für alle Operation ausser Pre-/Postsql anhand des Connectionstrings des ConnectionManagers erzeugt
    ///     - Mutlithreading funktioniert nun wieder für alle DBCommands
    ///     - Wählt man bei Merge/Merge2005 mehr als einen Key aus, so zeigt SSIS eine Warnung an
    ///     - Die Möglichkeit die Anzahl der Zeilen zu beschränken wurde entfernt.
    ///     - Die Einstellung "Transaktion" hat keine Wirkung
    /// 09.01.2012, Dennis Weise
    ///     - Version 3.0.9
    /// 25.01.2012, Dennis Weise
    ///     - Funktion ohne Threads hinzugefügt (wie in TL 1.x)
    /// 25.01.2012, Dennis Weise
    ///     - Standardkonfiguration bzgl. neuer Funktion angepaßt
    /// 25.01.2012, Dennis Weise
    ///     - Logging für LogCube angepaßt
    ///     - Ist "FastLoad" ausgewählt, wird nicht mehr der DbCommand-Teil von "TxAll" aufgerufen
    /// 27.01.2012, Dennis Weise
    ///     - DBCommand Insert: Wurden Function&Insert gewählt funktioniert dieses sofern man @(<Spaltenname>) als Referenz auf eine Spalte eingibt
    /// 27.01.2012, Dennis Weise
    ///     - TxAll&BulkInsert&internal Transaction: Abfrage der DB zum Erzeugen der DataTable wird innerhalb der Transaktion durchgeführt.
    /// 31.01.2012, Dennis Weise
    ///     - @(Spaltenname) als Standard für Referenzierung einer Spalte in Function- und Defaultwert eingeführt
    ///     - Update (table based) - Insert (Row based): Es wurde Merge statt Update ausgeführt
    ///     - Version 3.0.14
    /// 16.04.2012, Dennis Weise
    ///     - Das DbCommadn "Merge" funktioniert nun auch wenn keine Spalte für "Insert" markiert sind
    /// 16.04.2012, Dennis Weise
    ///     - Version 3.0.15
    /// 16.04.2012, Dennis Weise
    ///     - createSetup.bat: Anapassung an WiX 3.5
    /// 18.04.2012, Dennis Weise
    ///     - Labels in der der GUI angepaßt
    /// 29.06.2012, Dennis Weise
    ///     - Fehler im BulkCopy-Thread führten nicht immer zum Abbruch
    ///     - Ein DT_DATE wird nun als Date in die temporäre Tabelle geschrieben
    /// 29.06.2012, Dennis Weise
    ///     - Version 3.0.16
    /// 20.07.2012, Dennis Weise
    ///     - Ein DT_DATE wird nun als datetime2 in die temporäre Tabelle geschrieben
    /// 20.07.2012, Dennis Weise
    ///     - Version 3.0.17
    /// 27.07.2012, Dennis Weise
    ///     - Filter für die Auwahl der Zieltabelle eingebaut
    /// 27.07.2012, Dennis Weise
    ///     - Version 3.0.18
    /// 10.08.2012, Dennis Weise 
    ///     - DB Command Bulk Insert (rowlock) hinzugefügt
    ///     - Bulk Copy Threads für DB Command Bulk Insert bekommen nun wieder den Status "Finished" wenn sie erfolgreich durchgelaufen sind
    /// 10.08.2012, Dennis Weise
    ///     - Version 3.0.19
    /// 14.09.2012, Dennis Weise
    ///     - Merge/Merge2005: auf der TempTable wird ein index erstellt
    /// 14.09.2012, Dennis Weise
    ///     - Version 3.0.20
    /// 21.09.2012, Dennis Weise 
    ///     - DT_Decimal: Beim Mapping zum SQL Server wird nun eine precision von 29 angenommen
    ///     - Option Reattempts hinzugefügt: Gibts die Anzahl der Wiederholungsversuche bei Timeouts beim BulkCopy/DB Command an
    /// 21.09.2012, Dennis Weise
    ///     - Version 3.0.21
    /// 28.09.2012, Dennis Weise
    ///     - Direkt nach dem Hinzufügen einer Zeile im Mapping konnte man den Function Editor der Zeile nicht öffnen
    /// 28.09.2012, Dennis Weise
    ///     - Nach dem Entfernen eines Mappings (Zuordnung einer Output Column zu einer Input Column) funktioniert AutoMap für die Zeile wieder 
    /// 12.10.2012, Dennis Weise
    ///     - DB Command Merge: Das "ON"-Statement im Merge  berücksichtigt nun auch Function/Default 
    /// 12.10.2012, Dennis Weise
    ///     - Version 3.0.22
    /// 07.12.2012, Dennis Weise
    ///     - Umstellung auf das ComponentFramework
    ///     - Einfügen von Variablen: Erweiterung der GUI, Einfügen mit Namespace
    /// 07.12.2012, Dennis Weise
    ///     - Version 3.0.23
    /// 11.01.2013, Dennis Weise
    ///     - Zusätzliche Abbruchbedingung beim Warten auf DB Commands hinzugefügt
    /// 11.01.2013, Dennis Weise
    ///     - Version 3.0.24
    /// 18.01.2013, Dennis Weise
    ///     - Zusätzliche Abbruchbedingung beim Warten auf DB Commands: Es werden wieder alle Fehlermeldungen ausgegeben
    /// 18.01.2013, Dennis Weise
    ///     - Version 3.0.25
    /// 26.02.2013, Dennis Weise
    ///     - Option zum Deaktivieren von TabLockx hinzugefügt
    ///     - drop Table für die Kombination TxAll/BulkInsert entfernt
    ///     - Korrektur Gui: FunctionEditor
    /// 26.02.2013, Dennis Weise
    ///     - PreSql kann nun außerhalb einer TxAll-Transaction ausgeführt werden
    /// 26.02.2013, Dennis Weise
    ///     - Mapping: Das drücken der Space-Taste ändert den Status der aktiven Checkbox
    /// 28.02.2013, Dennis Weise
    ///     - Der TableLoader verliert seine Mappings nicht mehr automatisch, wenn der InputPath entfernt wird
    /// 28.02.2013, Dennis Weise
    ///     - Version 3.0.26
    /// 16.04.2013, Dennis Weise
    ///     - setup an WiX 3.7 angepasst
    /// 08.05.2013, Dennis Weise
    ///     - BulkCopy für DB Command BulkInsert: ColumnMapping hinzugefügt  
    ///     - Version 3.0.27
    /// 08.05.2013, Dennis Weise
    ///     - Korrektur ColumnMapping BulkInsert 
    ///     - Version 3.0.27
    /// 31.05.2013, Dennis Weise 
    ///     - Unterstützung von SSIS 2012 vorbereitet
    /// 31.05.2013, Dennis Weise 
    ///     - Version 3.1.0
    /// 25.06.2013, Dennis Weise 
    ///     - Beim BulkInsert müssen nicht mehr alle Zielspalten bedient werden
    ///     - Eckige Klammern für die Spalten im Create-Statement für den Index der Temporären Tabelle hinzugefügt 
    /// 25.06.2013, Dennis Weise 
    ///     - Version 3.1.1
    /// 08.07.2013, Dennis Weise 
    ///     - Korrektur BulkInsert: Abweichungen zwischen SSIS Column Name und SQL Column Name sind nun wieder erlaubt
    /// 08.07.2013, Dennis Weise 
    ///     - Version 3.1.2
    /// 13.09.2013, Dennis Weise 
    ///     - Korrektur bzgl. Multithreading im PostExecute
    ///     - Logging erweitert (Cube Statistiken funktionieren nun evtl nicht mehr)
    ///     - Version 3.1.6
    /// 01.11.2013, Dennis Weise 
    ///     - SCD Funktionalität hinzugefügt
    /// 01.11.2013, Dennis Weise 
    ///     - Version 3.1.7
    /// 06.12.2013, Dennis Weise 
    ///     - Index für SCD BKs und Attributpaare hinzugefügt
    ///     - Layouts für das Mapping hinzugefügt (je nach Layout werden einige Spalten ausgeblendet)
    ///     - Mapping: Spalten mit einer Checkbox wurden in ihrer Breite begrenzt
    /// 06.12.2013, Dennis Weise 
    ///     - Version 3.1.8
    /// 14.02.2014, Dennis Weise 
    ///     - Korrektur: Die SSIS BK-Spalten mussten bisher den gleichen Namen wie in der Zielspalte haben
    ///     - In den SCD Tabellen werden ID und BK als FK_[Name Zieltabelle]_ID und FK_[Name Zieltabelle]_[Name BK Spalte] eingetragen
    /// 14.02.2014, Dennis Weise 
    ///     - Version 3.1.9
    /// 11.04.2014, Dennis Weise 
    ///     - Nach dem Hinzufügen zum DFT konnte der TL nur im Advanced Editor bearbeitet werden
    /// 11.04.2014, Dennis Weise 
    ///     - Version 3.1.10
    /// </summary> 
    [DtsPipelineComponent(DisplayName = "TableLoader 3",
        ComponentType = ComponentType.DestinationAdapter,
        CurrentVersion = 0,
        IconResource = "TableLoader.Resources.TableLoader.ico",
        UITypeName = "TableLoader.TableLoaderUI, TableLoader3, Version=1.0.0.0, Culture=neutral, PublicKeyToken=1bfbf132955f2db6")]
    public class TableLoader : PipelineComponent
    {

        #region Members & Properties

        //Runtime & Designtime
        private SqlConnection _mainConn = null;
        private SqlConnection Conn
        {
            get
            {
                if (_mainConn != null && _mainConn.State == System.Data.ConnectionState.Closed) _mainConn.Open();
                return _mainConn;
            }
        }

        private SqlConnection _bulkConn = null;
        private SqlConnection BulkConn
        {
            get
            {
                InitProperties(true);

                if (!_IsagCustomProperties.UseExternalTransaction) return Conn;

                if (_bulkConn != null && _bulkConn.State == System.Data.ConnectionState.Closed) _bulkConn.Open();
                return _bulkConn;
            }
        }

        private IsagCustomProperties _IsagCustomProperties;

        //Runtime
        private TlDbCommand _dbCommand;
        private TxAll _txAll;
        private ThreadHandler _threadHandler;
        //SqlTransaction _dbTransaction = null;
        private IsagEvents _events;
        private List<ColumnInfo> _columnInfos = null; //Wird im PreSql gefüllt, damit Daten in ProcessInput schnell zur Verfügung stehen
        private Dictionary<string, string> _columnMapping; //Für das BulkCopy Mapping
        private DataTable _dtBuffer = null; //Speicher für die einkommenden Zeile. Wird für das BulkCopy in die TempTable benötigt.
        private long _chunkCounterBulk; //Gibt an wieviele Zeilen in die DataTable _rowsToWrite geschrieben werden, bevor
        //ein BulkCopy in die TempTable durchgeführt wird
        private long _chunkCounterDbCommand;
        string _tempTableName = "";
        private Status _status; 

        private bool _PreSqlFinished = false;


        #endregion

        #region DesignTime

        /// <summary>
        /// Einmaliges Initialisieren der Komponente
        /// </summary>
        public override void ProvideComponentProperties()
        {
            base.ProvideComponentProperties();

            _IsagCustomProperties = new IsagCustomProperties();
            _IsagCustomProperties.SetDefaultValues();
            ComponentMetaDataTools.UpdateVersion(this, ComponentMetaData);

            //Metadaten Version auf DLL-Version setzen 
            DtsPipelineComponentAttribute componentAttr =
                 (DtsPipelineComponentAttribute)Attribute.GetCustomAttribute(this.GetType(), typeof(DtsPipelineComponentAttribute), false);
            int binaryVersion = componentAttr.CurrentVersion;
            ComponentMetaData.Version = binaryVersion;

            //Clear out base implmentation
            this.ComponentMetaData.RuntimeConnectionCollection.RemoveAll();
            this.ComponentMetaData.InputCollection.RemoveAll();
            this.ComponentMetaData.OutputCollection.RemoveAll();

            //Input
            IDTSInput100 input = this.ComponentMetaData.InputCollection.New();
            input.Name = Constants.INPUT_NAME;
            input.Description = Constants.INPUT_NAME;
            input.HasSideEffects = true;

            //New connection managers
            IDTSRuntimeConnection100 conn = this.ComponentMetaData.RuntimeConnectionCollection.New();
            conn.Name = Constants.CONNECTION_MANAGER_NAME_MAIN;
            conn.Description = "Main Connection to SQL Server";

            //Custom Properties hinzufügen
            IDTSCustomProperty100 prop = ComponentMetaData.CustomPropertyCollection.New();
            prop.Name = Constants.PROP_CONFIG;

            _IsagCustomProperties.Save(ComponentMetaData);
            //Configuration.SaveConfiguration(VariableDispenser, ComponentMetaData, _IsagCustomProperties);
        }


        /// <summary>
        /// Findet Validate() einen Fehler in den Metadaten, so können die Fehler hier behoben werden.
        /// </summary>
        public override void ReinitializeMetaData()
        {
            base.ReinitializeMetaData();
            this.ComponentMetaData.RemoveInvalidInputColumns();
            InitProperties(false);
            Mapping.UpdateInputIdProperties(ComponentMetaData, _IsagCustomProperties);

            _IsagCustomProperties.RebuildMappings(ComponentMetaData, _events);
        }

        public override void OnInputPathAttached(int inputID)
        {
            base.OnInputPathAttached(inputID);
            InitProperties(false);

            //Initialisierung falls zuvor noch keine Inputcolumns angebunden waren
            if (_IsagCustomProperties.ColumnConfigList.Count == 0)
            {
                IDTSInput100 input = ComponentMetaData.InputCollection.GetObjectByID(inputID);
                input.InputColumnCollection.RemoveAll();

                _IsagCustomProperties.RebuildMappings(ComponentMetaData, _events);
            }
        }

     


        #endregion

        /// <summary>
        /// Prüfung ob die Konfiguration korrekt ist
        /// </summary>
        /// <returns></returns>
        public override DTSValidationStatus Validate()
        {
            InitProperties(false);
            Mapping.UpdateInputIdProperties(ComponentMetaData, _IsagCustomProperties);

            DTSValidationStatus status = base.Validate();
            if (status != DTSValidationStatus.VS_ISVALID) return status;

            if (!_IsagCustomProperties.IsValid(ComponentMetaData, _events)) return DTSValidationStatus.VS_NEEDSNEWMETADATA;

            if (!this.ComponentMetaData.AreInputColumnsValid) return DTSValidationStatus.VS_NEEDSNEWMETADATA;

            return DTSValidationStatus.VS_ISVALID;

        }

        #region Run Time

        #region Connection & Transaction
        /// <summary>
        /// Initialsiert die DB Connection Main und falls vom Benutzer aktiviert auch die Bulk-Connection.
        /// 
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        public override void AcquireConnections(object transaction)
        {
            InitProperties(true);


            IDTSRuntimeConnection100 runtimeConn = null;

            //Main
            try
            {
                runtimeConn = this.ComponentMetaData.RuntimeConnectionCollection[Constants.CONNECTION_MANAGER_NAME_MAIN];
            }
            catch (Exception) { }

            if (runtimeConn == null || runtimeConn.ConnectionManager == null)
            {
                _events.Fire(IsagEvents.IsagEventType.ErrorConnectionNotInitialized,
                             "ADO.NET [{0}] DB Connection Manager has not been initialized.",
                             Constants.CONNECTION_MANAGER_NAME_MAIN);
            }
            else
            {
                object tempConn = this.ComponentMetaData.RuntimeConnectionCollection[Constants.CONNECTION_MANAGER_NAME_MAIN].ConnectionManager.AcquireConnection(transaction);

                if (tempConn is SqlConnection) _mainConn = (SqlConnection)tempConn;
                else _events.Fire(IsagEvents.IsagEventType.ErrorWrongConnection,
                     "Only ADO.NET SQL Server connections are supported for the ADO.NET [{0}] Connection.",
                     Constants.CONNECTION_MANAGER_NAME_MAIN);
            }

            runtimeConn = null;

            //Bulk
            if (!_IsagCustomProperties.UseExternalTransaction && _mainConn != null) _bulkConn = _mainConn;
            else
            {
                try
                {
                    runtimeConn = this.ComponentMetaData.RuntimeConnectionCollection[Constants.CONNECTION_MANAGER_NAME_BULK];
                }
                catch (Exception) { }

                if (runtimeConn == null || runtimeConn.ConnectionManager == null)
                {
                    _events.Fire(IsagEvents.IsagEventType.ErrorConnectionNotInitialized,
                    "ADO.NET [{0}] DB Connection Manager has not been initialized.",
                    Constants.CONNECTION_MANAGER_NAME_BULK);
                    //Events.Fire(ComponentMetaData, Events.Type.Error, "ADO.NET [Bulk] Connection Manager has not been initialized.");
                }
                else
                {
                    object tempConn = this.ComponentMetaData.RuntimeConnectionCollection[Constants.CONNECTION_MANAGER_NAME_BULK].ConnectionManager.AcquireConnection(transaction);

                    if (tempConn is SqlConnection) _bulkConn = (SqlConnection)tempConn;
                    else
                        _events.Fire(IsagEvents.IsagEventType.ErrorWrongConnection,
                        "Only ADO.NET SQL Server connections are supported for the ADO.NET [{0}] Connection.",
                        Constants.CONNECTION_MANAGER_NAME_BULK);
                    //Events.Fire(ComponentMetaData, Events.Type.Error, "Only ADO.NET SQL Server connections are supported for the ADO.NET [Bulk] Connection.");
                }
            }

        }

        public override void ReleaseConnections()
        {
            base.ReleaseConnections();
        }

        private void CloseConnections()
        {
            if (BulkConn != null && BulkConn.State != System.Data.ConnectionState.Closed)
                BulkConn.Close();
            if (!_IsagCustomProperties.UseExternalTransaction && Conn != null && Conn.State != System.Data.ConnectionState.Closed)
                Conn.Close();
        }

        #endregion

        #region Destination & Temporary Table

        private DataTable CreateDataTableForDestinationTable()
        {
            return CreateDataTableForDestinationTable(null);
        }

        /// <summary>
        /// Erstellen der DataTable anhand der Spalten der Zieltabelle
        /// </summary>
        /// <returns></returns>
        private DataTable CreateDataTableForDestinationTable(SqlTransaction transaction)
        {
            DataTable dt = new DataTable();

            SqlCommand cmd = Conn.CreateCommand();
            cmd.CommandText = "select TOP 0 * from " + _IsagCustomProperties.DestinationTable;
            if (transaction != null) cmd.Transaction = transaction;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);


            Dictionary<string, ColumnInfo> relevantColumnInfos = new Dictionary<string, ColumnInfo>();

            //Nur die Spalten, die in die Zieltabelle eingefügt werden sollen, sind relevant
            foreach (ColumnInfo info in _columnInfos)
            {
                if (info.Insert) relevantColumnInfos.Add(info.DestColumnName, info);
            }
         
            //Nicht verwendete Spalten der Zieltabelle aus DataTable entfernen
            for (int i = dt.Columns.Count - 1; i >= 0; i--)
            {
                if (!relevantColumnInfos.ContainsKey(dt.Columns[i].ColumnName) &&
                    !relevantColumnInfos.ContainsKey(dt.Columns[i].ColumnName.ToUpper()))
                {
                    dt.Columns.RemoveAt(i);
                }
            }

            //ColumnInfos anhand der DataTable neu sortieren
            _columnInfos = new List<ColumnInfo>();
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                if (relevantColumnInfos.ContainsKey(dt.Columns[i].ColumnName)) 
                    _columnInfos.Add(relevantColumnInfos[dt.Columns[i].ColumnName]);
                else if (relevantColumnInfos.ContainsKey(dt.Columns[i].ColumnName.ToUpper()))
                    _columnInfos.Add(relevantColumnInfos[dt.Columns[i].ColumnName.ToUpper()]);                
            }

            if (_columnInfos.Count != relevantColumnInfos.Count)
                _events.Fire(IsagEvents.IsagEventType.Error, "Destination Table conatins less columns than configured!");

            if (dt.Columns.Count != _columnInfos.Count)
                _events.Fire(IsagEvents.IsagEventType.Error, "Column Count for Configuration and Datatable is not equal!");

            for (int i = 0; i < dt.Columns.Count; i++)
            {
                string inputColumnName = _columnInfos[i].DestColumnName;
                string outputColumnName = dt.Columns[i].ColumnName;

                if (inputColumnName != outputColumnName && inputColumnName.ToUpper() != outputColumnName.ToUpper())
                    _events.Fire(IsagEvents.IsagEventType.Error, "Mapping Error: Cannot map input column " + inputColumnName + " to output column " + outputColumnName);
            }
           
           

            //foreach (ColumnConfig config in _IsagCustomProperties.ColumnConfigList)
            //{
            //    if (config.Insert) colConfigByOutputColumnName.Add(config.OutputColumnName, config);
             
            //    if (!config.Insert && dt.Columns.Contains(config.OutputColumnName))
            //    {
            //        dt.Columns.Remove(config.OutputColumnName);
            //        _events.Fire(IsagEvents.IsagEventType.Warning, "Removing: " + config.OutputColumnName);
            //    }
            //    else if (!config.Insert && dt.Columns.Contains(config.OutputColumnName.ToUpper()))
            //    {
            //        dt.Columns.Remove(config.OutputColumnName.ToUpper());
            //        _events.Fire(IsagEvents.IsagEventType.Warning, "Removing: " + config.OutputColumnName.ToUpper());
            //    }
            //    else if (config.Insert && (!dt.Columns.Contains(config.OutputColumnName) || !dt.Columns.Contains(config.OutputColumnName.ToUpper())))
            //    {
            //        _events.Fire(IsagEvents.IsagEventType.Error, "Destination Table does not contain column : " + config.OutputColumnName);
            //    }
            //}

            //foreach (ColumnConfig config in _IsagCustomProperties.ColumnConfigList)
            //{
            //    if (!config.Insert && dt.Columns.Contains(config.OutputColumnName))
            //    {
            //        dt.Columns.Remove(config.OutputColumnName);
            //        _events.Fire(IsagEvents.IsagEventType.Warning, "Removing: " + config.OutputColumnName);
            //    }
            //}

            //List<ColumnInfo> colInfoList = new List<ColumnInfo>();

            //for (int i = 0; i < dt.Columns.Count; i++)
            //{
            //    foreach (ColumnInfo info in _columnInfos)
            //    {
            //        if (info.DestColumnName.ToUpper() == dt.Columns[i].ColumnName.ToUpper()) colInfoList.Add(info);
            //    }
            //}

            //_columnInfos = colInfoList;

            //TODO: columnInfo umsortieren

            return dt;
        }

        /// <summary>
        /// Erzeugt die DataTable anhand der Properties.
        /// (für alle DB Commands außer BulkInsert)
        /// </summary>
        /// <param name="input">SSIS input</param>
        /// <returns>DataTable für das BulkCopy</returns>
        private DataTable CreateDataTableForTempTable(IDTSInput100 input)
        {
            DataTable dt = new DataTable();

            foreach (ColumnConfig config in _IsagCustomProperties.ColumnConfigList)
            {
                Type typeNet;
                if (config.HasInput) typeNet = SqlCreator.GetNetDataType(input.InputColumnCollection.GetObjectByID(config.InputColumnId).DataType);
                else typeNet = Type.GetType(config.DataTypeOutputNet);

                if (config.IsInputColumnUsed) dt.Columns.Add(config.InputColumnName, typeNet);
            }

            return dt;
        }


        private void AddDataRowToTempDataTable(PipelineBuffer buffer)
        {
            DataRow row = _dtBuffer.NewRow();

            int destIndex = 0;
            for (int i = 0; i < _columnInfos.Count; i++)
            {
                ColumnInfo col = _columnInfos[i];

                if (col.IsUsed)
                {
                    object value = buffer[col.BufferIndex];
                    if (value != null) row[destIndex] = value;
                    destIndex++;
                }
            }

            _dtBuffer.Rows.Add(row);
        }

        #endregion

        #region PreExecute

        public override void PreExecute()
        {
            _status = new Status(_events);
            _status.AddTableLoaderStatus(Status.TableLoaderStatusType.started);
            _status.AddTableLoaderStatus(Status.TableLoaderStatusType.preExecStarted);


            //System.Windows.Forms.Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException,false);
            //AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            if (_IsagCustomProperties == null || _events == null) InitProperties(true);

            _chunkCounterBulk = 1;
            _dbCommand = new TlDbCommand(_IsagCustomProperties, _events, ComponentMetaData, VariableDispenser);

            if (_IsagCustomProperties.UseMultiThreading)
            {
                CreateThreadHandler();
            }
            else
            {
                _txAll = new TxAll(_events, Conn, _IsagCustomProperties, _dbCommand, BulkConn, ComponentMetaData);
                if (!_IsagCustomProperties.ExcludePreSqlFromTransaction)  _txAll.CreateTransaction();
                _chunkCounterDbCommand = 1;
                _tempTableName = _txAll.GetTempTableName();
                _txAll.CreateTempTable(_tempTableName);               
            }

            CreateMapping();

            IDTSInput100 input = this.ComponentMetaData.InputCollection[Constants.INPUT_NAME];

            if (_IsagCustomProperties.UseTempTable) _dtBuffer = CreateDataTableForTempTable(input);
            else if (_IsagCustomProperties.UseMultiThreading) _dtBuffer = CreateDataTableForDestinationTable();
            else _dtBuffer = CreateDataTableForDestinationTable(_txAll.Transaction);

            if (!_IsagCustomProperties.UseMultiThreading) _txAll.DtBuffer = _dtBuffer;

            _status.AddTableLoaderStatus(Status.TableLoaderStatusType.preExecFinished);
        }

 




        private void CreateMapping()
        {
            _columnInfos = new List<ColumnInfo>(this.ComponentMetaData.InputCollection[0].InputColumnCollection.Count);
            IDTSInput100 input = this.ComponentMetaData.InputCollection[Constants.INPUT_NAME];
            foreach (IDTSInputColumn100 col in input.InputColumnCollection)
            {
                // Find the position in buffers that this column will take, and add it to the map.
                ColumnConfig config = _IsagCustomProperties.GetColumnConfigByInputColumnName(col.Name);
                _columnInfos.Add(new ColumnInfo(col.Name, col.DataType,
                    this.BufferManager.FindColumnByLineageID(input.Buffer, col.LineageID),
                    col.Length, col.Precision, col.Scale, config.IsInputColumnUsed, config.OutputColumnName, config.Insert));
            }

            if (_IsagCustomProperties.UseBulkInsert)
            {
                _columnMapping = new Dictionary<string, string>();
                foreach (ColumnConfig config in _IsagCustomProperties.ColumnConfigList)
                {
                    if (config.Insert ) _columnMapping.Add(config.OutputColumnName, config.InputColumnName);
                }
            }
        }


        #endregion

        /// <summary>
        /// Processes Input Phase
        /// 
        /// </summary>
        /// <param name="inputID">Input ID</param>
        /// <param name="buffer">Input buffer.</param>
        public override void ProcessInput(int inputID, PipelineBuffer buffer)
        {
            if (_IsagCustomProperties == null || _events == null) InitProperties(true);

            _status.AddTableLoaderStatus(Status.TableLoaderStatusType.processInputStarted);

            try
            {
                base.ProcessInput(inputID, buffer);

                if (!_PreSqlFinished)
                {
                   
                    ExecPreOrPostExecuteStatement(IsagEvents.IsagEventType.PreSql);
                    if (!_IsagCustomProperties.UseMultiThreading && _IsagCustomProperties.ExcludePreSqlFromTransaction) _txAll.CreateTransaction();
                    _PreSqlFinished = true;
                }

                long chunkSize = _IsagCustomProperties.ChunckSizeBulk;

                while (buffer.NextRow())
                {
                    AddDataRowToTempDataTable(buffer);


                    //ChunkSizeDbCommand wird nur bei TxAll und DbCommand != BulkInsert verwendet
                    if (_IsagCustomProperties.IsTransactionAvailable && !_IsagCustomProperties.UseBulkInsert &&
                        _chunkCounterDbCommand == _IsagCustomProperties.ChunkSizeDbCommand )
                    {
                        _chunkCounterBulk = 0;
                        _chunkCounterDbCommand = 1;

                        _txAll.ExecuteBulkCopy(_tempTableName);
                        _txAll.ExecuteDbCommand(_tempTableName);
                        _txAll.TruncateTempTable(_tempTableName);
                        _dtBuffer.Clear();
                    }
                    else _chunkCounterDbCommand++;

                    if (_chunkCounterBulk == chunkSize)
                    {
                        _chunkCounterBulk = 1;

                        if (_IsagCustomProperties.UseMultiThreading)
                        {
                            StartBulkCopyThread();

                            _threadHandler.LogThreadStatistic();
                        }
                        else
                        {
                            _txAll.ExecuteBulkCopy(_tempTableName);
                            _dtBuffer.Clear();
                        }
                    }
                    else _chunkCounterBulk++;
                }
            }
            catch (Exception ex)
            {
                _events.FireError(new string[] { "ProcessInput", ex.Message });
                throw ex;
            }


            _status.AddTableLoaderStatus(Status.TableLoaderStatusType.processInpitFinished);
        }




        #region PostExecute

        /// <summary>
        /// Post Execute Phase
        /// 
        /// </summary>
        public override void PostExecute()
        {
            if (_IsagCustomProperties == null || _events == null) InitProperties(true);

            _status.AddTableLoaderStatus(Status.TableLoaderStatusType.postExecStarted);

            try
            {
                try
                {
                    base.PostExecute();

                    _PreSqlFinished = false;

                    if (_IsagCustomProperties.UseMultiThreading)
                    {
                        if (_dtBuffer.Rows.Count > 0)
                        {
                            StartBulkCopyThread();
                        }

                        _threadHandler.LogThreadStatistic();
                        _threadHandler.WaitForBulkCopyThreads();
                        _threadHandler.LogThreadStatistic();
                        _threadHandler.WaitForDbCommands();
                        _threadHandler.LogThreadStatistic();
                    }
                    else
                    {
                        if (_dtBuffer.Rows.Count > 0)
                        {
                            _txAll.ExecuteBulkCopy(_tempTableName);                            
                            _dtBuffer.Clear();
                        }

                        if (!_IsagCustomProperties.UseBulkInsert)
                        {
                            _txAll.ExecuteDbCommand(_tempTableName);
                            _txAll.DropTempTable(_tempTableName, IsagEvents.IsagEventType.TempTableDrop);
                        }

                    }

                    ExecPreOrPostExecuteStatement(IsagEvents.IsagEventType.PostSql);

                    if (!_IsagCustomProperties.UseMultiThreading) _txAll.Commit();
                }
                catch (Exception ex)
                {
                    _events.FireError(new string[] { "PostExecute", ex.Message });
                    throw ex;
                }



            }
            catch (Exception ex)
            {
                if (!_IsagCustomProperties.UseMultiThreading)  _txAll.Rollback();

                _events.FireError(new string[] { "PostExecute", ex.Message });
                throw ex;
            }
            finally
            {
                CloseConnections();
            }

            _status.LogStatistic();
            _status.AddTableLoaderStatus(Status.TableLoaderStatusType.postExecFinished);
            _status.AddTableLoaderStatus(Status.TableLoaderStatusType.finished);

            
        }

      




        #endregion

        #endregion

        #region Threads

        private void CreateThreadHandler()
        {
            IsagEvents.IsagEventType eventType = IsagEvents.IsagEventType.MergeBegin;
            string[] sqlTemplate = null;
            _dbCommand.GetDbCommandDefinition(out eventType, out sqlTemplate);

            _threadHandler = new ThreadHandler(ComponentMetaData.InputCollection[Constants.INPUT_NAME], _IsagCustomProperties,
                                               GetConnectionStringForThread(), Conn, _events, eventType, sqlTemplate, _status);
        }

        private void StartBulkCopyThread()
        {
            DataTable dt = _dtBuffer;
            _dtBuffer = dt.Clone();

            if (_IsagCustomProperties.DbCommand == IsagCustomProperties.DbCommandType.BulkInsert)
                _threadHandler.AddBulkCopyThread(_IsagCustomProperties.DestinationTable, dt, !_IsagCustomProperties.DisableTablock);
            else if (_IsagCustomProperties.DbCommand == IsagCustomProperties.DbCommandType.BulkInsertRowLock)
                _threadHandler.AddBulkCopyThread(_IsagCustomProperties.DestinationTable, dt, false);
            else _threadHandler.AddBulkCopyThread(_IsagCustomProperties.CreateTempTableName(), dt, !_IsagCustomProperties.DisableTablock);
        }

        private string GetConnectionStringForThread()
        {
            string cstr = Conn.ConnectionString;

            if (cstr.Contains("Integrated Security=True")) return cstr;
            else
            {
                int start = cstr.IndexOf("User ID=");
                int end = cstr.IndexOf(";", start);
                cstr = cstr.Substring(0, start) + "Integrated Security=True" + cstr.Substring(end);
            }

            return cstr;

        }

        #endregion

        #region Pre/Post Sql Statement

        /// <summary>
        /// Führt das Pre- oder PostExecute Statement aus
        /// </summary>
        /// <param name="cmdType"> Events.IsagEventType.PreSql oder Events.IsagEventType.PostSql </param>
        private void ExecPreOrPostExecuteStatement(IsagEvents.IsagEventType cmdType)
        {
            string sql = "";
            string cmdTypeName = "";

            try
            {
                if (cmdType == IsagEvents.IsagEventType.PreSql && _IsagCustomProperties.HasPreSql)
                {
                    sql = _dbCommand.GetExecuteStatementFromTemplate(_IsagCustomProperties.PreSql);
                    cmdTypeName = "PreSql";
                }
                else if (cmdType == IsagEvents.IsagEventType.PostSql && _IsagCustomProperties.HasPostSql)
                {
                    sql = _dbCommand.GetExecuteStatementFromTemplate(_IsagCustomProperties.PostSql);
                    cmdTypeName = "PostSql";
                }
                else if (cmdType != IsagEvents.IsagEventType.PreSql && cmdType != IsagEvents.IsagEventType.PostSql) throw new Exception("Unknown CommandType in ExecPrePostExecuteStatement: " + cmdType);

                // Pre-, bzw. Post-SQL ausführen

                if (sql.Length > 0)
                {
                    SqlTransaction transaction = _IsagCustomProperties.UseMultiThreading ? null : _txAll.Transaction;
                    int rowsAffected = Common.ExecSql(sql, Conn, _IsagCustomProperties.TimeOutDb, transaction);
                    _events.Fire(IsagEvents.IsagEventType.Sql,
                                              "[ExecSql:" + cmdType.ToString() + "]: {0} rows were affected by the Sql Command.",
                                              new string[] { rowsAffected.ToString(), ((int)cmdType).ToString() });
                    _events.Fire(cmdType, cmdTypeName + " Statement has been executed.");
                }
            }
            catch (Exception ex)
            {
                _events.FireError(new string[] { cmdTypeName, ex.Message });
                throw;
            }



        }
  



        #endregion

       


        private void InitProperties(bool needsStandardConfiguration)
        {
            try
            {
                _IsagCustomProperties = IsagCustomProperties.Load(ComponentMetaData, needsStandardConfiguration);
            }
            catch (Exception ex)
            {
                _events.FireError(new string[] { "InitProperties", "Load", ex.Message });
            }


            _events = new IsagEvents(ComponentMetaData, VariableDispenser, _IsagCustomProperties.DestinationTable, _IsagCustomProperties.CustumLoggingTemplate, _IsagCustomProperties.LogLevel);
            Logging.Events = _events;
        }

        /// <summary>
        /// Upgrade einer vorhandenen TL Instanz:
        /// 
        /// - Version setzen FileVerison --> Property Version, DLL CurrentVersion --> ComponentMetaData.Version
        /// - auf Version mit XML Konfiguration updaten
        /// </summary>
        /// <param name="pipelineVersion"></param>
        public override void PerformUpgrade(int pipelineVersion)
        {
            //Würde zu Fehler führen: 
            //base.PerformUpgrade(pipelineVersion);

            DtsPipelineComponentAttribute componentAttr =
                (DtsPipelineComponentAttribute)Attribute.GetCustomAttribute(this.GetType(), typeof(DtsPipelineComponentAttribute), false);
            int binaryVersion = componentAttr.CurrentVersion;
            int metadataVersion = ComponentMetaData.Version;

            System.Reflection.Assembly asm = System.Reflection.Assembly.GetExecutingAssembly();
            System.Diagnostics.FileVersionInfo fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(asm.Location);


            if (binaryVersion <= metadataVersion)
            {
                throw new Exception("The installed TableLoader Version is too old!");
            }
            else
            {

            }




        }



        /// <summary>
        /// Daten, die in PreExecute für ProcessInput gesammelt werden
        /// </summary>
        class ColumnInfo
        {
            private string _columnName = string.Empty;
            private DataType _dataType = DataType.DT_STR;
            private int _bufferIndex = 0;
            private int _precision = 0;
            private int _scale = 0;
            private int _length;
            private bool _isUsed = false;
            private string _destColumnName = string.Empty;
            private bool _insert = false;

            public ColumnInfo(string columnName, DataType dataType, int bufferIndex,
                              int length, int precision, int scale, bool isUsed, string destColumnName, bool insert)
            {
                _columnName = columnName;
                _dataType = dataType;
                _bufferIndex = bufferIndex;
                _precision = precision;
                _scale = scale;
                _length = length;
                _isUsed = isUsed;
                _destColumnName = destColumnName;
                _insert = insert;
            }

            public int BufferIndex
            { get { return _bufferIndex; } }

            public DataType ColumnDataType
            { get { return _dataType; } }

            public string ColumnName
            { get { return _columnName; } }

            public int Precision
            { get { return _precision; } }

            public int Length
            { get { return _length; } }

            public int Scale
            { get { return _scale; } }

            public bool IsUsed
            { get { return _isUsed; } }

            public string DestColumnName
            { get { return _destColumnName; } }

            public bool Insert
            { get { return _insert; } }
        }

    }


}

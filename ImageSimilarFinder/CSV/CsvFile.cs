using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Data;
using System.Xml.Serialization;

namespace ElencySolutions.CsvHelper
{

    /// <summary>
    /// Class to hold csv data
    /// </summary>
    [Serializable]
    public sealed class CsvFile
    {
        public string File_Name { get;private set; }
        #region Properties

        /// <summary>
        /// Gets the file headers
        /// </summary>
        public readonly List<string> Headers = new List<string>();

        /// <summary>
        /// Gets the records in the file
        /// </summary>
        public readonly CsvRecords Records = new CsvRecords();

        /// <summary>
        /// Gets the header count
        /// </summary>
        public int HeaderCount
        {
            get
            {
                return Headers.Count;
            }
        }

        /// <summary>
        /// Gets the record count
        /// </summary>
        public int RecordCount
        {   
            get
            {
                return Records.Count;   
            }
        }

        #endregion Properties

        #region Indexers

        /// <summary>
        /// Gets a record at the specified index
        /// </summary>
        /// <param name="recordIndex">Record index</param>
        /// <returns>CsvRecord</returns>
        public CsvRecord this[int recordIndex]
        {
            get
            {
                if (recordIndex > (Records.Count - 1))
                    throw new IndexOutOfRangeException(string.Format("There is no record at index {0}.", recordIndex));

                return Records[recordIndex];
            }
        }

        /// <summary>
        /// Gets the field value at the specified record and field index
        /// </summary>
        /// <param name="recordIndex">Record index</param>
        /// <param name="fieldIndex">Field index</param>
        /// <returns></returns>
        public string this[int recordIndex, int fieldIndex]
        {
            get
            {
                if (recordIndex > (Records.Count - 1))
                    throw new IndexOutOfRangeException(string.Format("There is no record at index {0}.", recordIndex));

                CsvRecord record = Records[recordIndex];
                if (fieldIndex > (record.Fields.Count - 1))
                    throw new IndexOutOfRangeException(string.Format("There is no field at index {0} in record {1}.", fieldIndex, recordIndex));

                return record.Fields[fieldIndex];
            }
            set
            {
                if (recordIndex > (Records.Count - 1))
                    throw new IndexOutOfRangeException(string.Format("There is no record at index {0}.", recordIndex));

                CsvRecord record = Records[recordIndex];

                if (fieldIndex > (record.Fields.Count - 1))
                    throw new IndexOutOfRangeException(string.Format("There is no field at index {0}.", fieldIndex));

                record.Fields[fieldIndex] = value;
            }
        }

        /// <summary>
        /// Gets the field value at the specified record index for the supplied field name
        /// </summary>
        /// <param name="recordIndex">Record index</param>
        /// <param name="fieldName">Field name</param>
        /// <returns></returns>
        public string this[int recordIndex, string fieldName]
        {
            get
            {
                if (recordIndex > (Records.Count - 1))
                    throw new IndexOutOfRangeException(string.Format("There is no record at index {0}.", recordIndex));

                CsvRecord record = Records[recordIndex];

                int fieldIndex = -1;

                for (int i = 0; i < Headers.Count; i++)
                {
                    if (string.Compare(Headers[i], fieldName) != 0) 
                        continue;

                    fieldIndex = i;
                    break;
                }

                if (fieldIndex == -1)
                    throw new ArgumentException(string.Format("There is no field header with the name '{0}'", fieldName));

                if (fieldIndex > (record.Fields.Count - 1))
                    throw new IndexOutOfRangeException(string.Format("There is no field at index {0} in record {1}.", fieldIndex, recordIndex));

                return record.Fields[fieldIndex];
            }
            set
            {
                if (recordIndex > (Records.Count - 1))
                    throw new IndexOutOfRangeException(string.Format("There is no record at index {0}.", recordIndex));

                CsvRecord record = Records[recordIndex];

                int fieldIndex = -1;

                for (int i = 0; i < Headers.Count; i++)
                {
                    if (string.Compare(Headers[i], fieldName) != 0)
                        continue;

                    fieldIndex = i;
                    break;
                }

                if (fieldIndex == -1)
                    throw new ArgumentException(string.Format("There is no field header with the name '{0}'", fieldName));

                if (fieldIndex > (record.Fields.Count - 1))
                    throw new IndexOutOfRangeException(string.Format("There is no field at index {0} in record {1}.", fieldIndex, recordIndex));

                record.Fields[fieldIndex] = value;
            }
        }

        #endregion Indexers

        #region Methods

        /// <summary>
        /// Populates the current instance from the specified file
        /// </summary>
        /// <param name="filePath">File path</param>
        /// <param name="hasHeaderRow">True if the file has a header row, otherwise false</param>
        public void Populate(string filePath, bool hasHeaderRow)
        {
            File_Name = filePath;
            Populate(filePath, null, hasHeaderRow, false);
        }

        /// <summary>
        /// Populates the current instance from the specified file
        /// </summary>
        /// <param name="filePath">File path</param>
        /// <param name="hasHeaderRow">True if the file has a header row, otherwise false</param>
        /// <param name="trimColumns">True if column values should be trimmed, otherwise false</param>
        public void Populate(string filePath, bool hasHeaderRow, bool trimColumns)
        {
            Populate(filePath, null, hasHeaderRow, trimColumns);
        }

        /// <summary>
        /// Populates the current instance from the specified file
        /// </summary>
        /// <param name="filePath">File path</param>
        /// <param name="encoding">Encoding</param>
        /// <param name="hasHeaderRow">True if the file has a header row, otherwise false</param>
        /// <param name="trimColumns">True if column values should be trimmed, otherwise false</param>
        public void Populate(string filePath, Encoding encoding, bool hasHeaderRow, bool trimColumns)
        {
            using (CsvReader reader = new CsvReader(filePath, encoding){HasHeaderRow = hasHeaderRow, TrimColumns = trimColumns})
            {
                PopulateCsvFile(reader);
            }
        }

        /// <summary>
        /// Populates the current instance from a stream
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="hasHeaderRow">True if the file has a header row, otherwise false</param>
        public void Populate(Stream stream, bool hasHeaderRow)
        {
            Populate(stream, null, hasHeaderRow, false);
        }

        /// <summary>
        /// Populates the current instance from a stream
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="hasHeaderRow">True if the file has a header row, otherwise false</param>
        /// <param name="trimColumns">True if column values should be trimmed, otherwise false</param>
        public void Populate(Stream stream, bool hasHeaderRow, bool trimColumns)
        {
            Populate(stream, null, hasHeaderRow, trimColumns);
        }

        /// <summary>
        /// Populates the current instance from a stream
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="encoding">Encoding</param>
        /// <param name="hasHeaderRow">True if the file has a header row, otherwise false</param>
        /// <param name="trimColumns">True if column values should be trimmed, otherwise false</param>
        public void Populate(Stream stream, Encoding encoding, bool hasHeaderRow, bool trimColumns)
        {
            using (CsvReader reader = new CsvReader(stream, encoding){HasHeaderRow = hasHeaderRow, TrimColumns = trimColumns})
            {
                PopulateCsvFile(reader);
            }
        }

        /// <summary>
        /// Populates the current instance from a string
        /// </summary>
        /// <param name="hasHeaderRow">True if the file has a header row, otherwise false</param>
        /// <param name="csvContent">Csv text</param>
        public void Populate(bool hasHeaderRow, string csvContent)
        {
            Populate(hasHeaderRow, csvContent, null, false);
        }

        /// <summary>
        /// Populates the current instance from a string
        /// </summary>
        /// <param name="hasHeaderRow">True if the file has a header row, otherwise false</param>
        /// <param name="csvContent">Csv text</param>
        /// <param name="trimColumns">True if column values should be trimmed, otherwise false</param>
        public void Populate(bool hasHeaderRow, string csvContent, bool trimColumns)
        {
            Populate(hasHeaderRow, csvContent, null, trimColumns);
        }

        /// <summary>
        /// Populates the current instance from a string
        /// </summary>
        /// <param name="hasHeaderRow">True if the file has a header row, otherwise false</param>
        /// <param name="csvContent">Csv text</param>
        /// <param name="encoding">Encoding</param>
        /// <param name="trimColumns">True if column values should be trimmed, otherwise false</param>
        public void Populate(bool hasHeaderRow, string csvContent, Encoding encoding, bool trimColumns)
        {
            using (CsvReader reader = new CsvReader(encoding, csvContent){HasHeaderRow = hasHeaderRow, TrimColumns = trimColumns})
            {
                PopulateCsvFile(reader);
            }
        }

        /// <summary>
        /// Populates the current instance using the CsvReader object
        /// </summary>
        /// <param name="reader">CsvReader</param>
        private void PopulateCsvFile(CsvReader reader)
        {
            Headers.Clear();
            Records.Clear();

            bool addedHeader = false;

            while (reader.ReadNextRecord())
            {
                if (reader.HasHeaderRow && !addedHeader)
                {
                    reader.Fields.ForEach(field => Headers.Add(field));
                    addedHeader = true;
                    continue;
                }

                CsvRecord record = new CsvRecord();
                reader.Fields.ForEach(field => record.Fields.Add(field));
                Records.Add(record);
            }
        }

        public override string ToString()
        {
            StringBuilder output = new StringBuilder();
            var writer = new StringWriter(output);

            XmlSerializer serializer = new XmlSerializer(typeof(CsvRecords));
            serializer.Serialize(writer, Records );

            string s = output.ToString();// new JavaScriptSerializer().Serialize(this);
            return s;
        }
        
        /// <summary>
        /// Reads a csv file format into a data table.  This method
        /// will always assume that the table has a header row as this will be used
        /// to determine the columns.
        /// </summary>
        /// <param name="columnTypes">Array of column types</param>
        /// <returns></returns>
        public DataTable ReadIntoDataTable(System.Type[] columnTypes, string TableName)
        {
            DataTable dataTable = new DataTable(TableName);

            for (int i = 0; i < Headers.Count; i++)
                dataTable.Columns.Add(Headers[i], (columnTypes.Length > 0 ? columnTypes[i] : typeof(string)));

            foreach (var item in Records)
            {

                DataRow row = dataTable.NewRow();

                for (int i = 0; i < item.FieldCount; i++)
                    row[i] = item.Fields[i];

                dataTable.Rows.Add(row);
            }

            return dataTable;
        }

        /// <summary>
        /// Convert csvfile object into a data table.  This method
        /// will always assume that the table has a header row as this will be used
        /// to determine the columns , and all columns are string.
        /// </summary>
        /// <param name="columnTypes">Array of column types</param>
        /// <returns></returns>
        public DataTable ToDataTable(string TableName)
        {
            Type[] t = { };
            return ReadIntoDataTable(t, TableName);

        }
        #endregion Methods


        public static List<string> DummayHeaders(uint HowMany)
        {
            List<string> _Headers = new List<string>();

            for (int i = 0; i < HowMany; i++)
            {
                _Headers.Add("");
            }
            return _Headers;
        }

        public string WorksheetName { get; set; }

        public string SheetName { get; set; }
    }

    /// <summary>
    /// Class for a collection of CsvRecord objects
    /// </summary>
    [Serializable]
    public sealed class CsvRecords : List<CsvRecord>
    {
        public override string ToString()
        {
            StringBuilder output = new StringBuilder();
            var writer = new StringWriter(output);

            XmlSerializer serializer = new XmlSerializer(typeof(CsvRecords ));
            serializer.Serialize(writer, this );

            string s = output.ToString();// new JavaScriptSerializer().Serialize(this);
            return s;
        }
    }

    /// <summary>
    /// Csv record class
    /// </summary>
    [Serializable]
    public sealed class CsvRecord
    {
        #region Properties

        /// <summary>
        /// Gets the Fields in the record
        /// </summary>
        public readonly List<string> Fields = new List<string>();

        /// <summary>
        /// Gets the number of fields in the record
        /// </summary>
        public int FieldCount
        {
            get
            {
                return Fields.Count;
            }
        }

        #endregion Properties

        public static List<string> DummayFields(uint HowMany)
        {
            List<string> _Fields = new List<string>();

            for (int i = 0; i < HowMany; i++)
            {
                _Fields.Add("");
            }
            return _Fields;
        }
    }
}

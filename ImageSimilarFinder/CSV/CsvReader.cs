using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace ElencySolutions.CsvHelper
{
    /// <summary>
    /// Class to read csv content from various sources
    /// </summary>
    public sealed class CsvReader : IDisposable
    {

        #region Members

        private FileStream _fileStream;
        private Stream _stream;
        private StreamReader _streamReader;
        private StreamWriter _streamWriter;
        private Stream _memoryStream;
        private Encoding _encoding;
        private readonly StringBuilder _columnBuilder = new StringBuilder(100);
        private readonly Type _type = Type.File;

        #endregion Members

        #region Properties

        /// <summary>
        /// Gets or sets whether column values should be trimmed
        /// </summary>
        public bool TrimColumns { get; set; }

        /// <summary>
        /// Gets or sets whether the csv file has a header row
        /// </summary>
        public bool HasHeaderRow { get; set; }

        /// <summary>
        /// Returns a collection of fields or null if no record has been read
        /// </summary>
        public List<string> Fields { get; private set; }

        /// <summary>
        /// Gets the field count or returns null if no fields have been read
        /// </summary>
        public int? FieldCount
        {
            get
            {
                return (Fields != null ? Fields.Count : (int?)null);
            }
        }

        #endregion Properties

        #region Enums

        /// <summary>
        /// Type enum
        /// </summary>
        private enum Type
        {
            File,
            Stream,
            String
        }

        #endregion Enums

        #region Constructors
        ~CsvReader()
        {
            Dispose();
        }
        /// <summary>
        /// Initialises the reader to work from a file
        /// </summary>
        /// <param name="filePath">File path</param>
        public CsvReader(string filePath)
        {
            _type = Type.File;
            Initialise(filePath, Encoding.Default);
        }

        /// <summary>
        /// Initialises the reader to work from a file
        /// </summary>
        /// <param name="filePath">File path</param>
        /// <param name="encoding">Encoding</param>
        public CsvReader(string filePath, Encoding encoding)
        {
            _type = Type.File;
            Initialise(filePath, encoding);
        }

        /// <summary>
        /// Initialises the reader to work from an existing stream
        /// </summary>
        /// <param name="stream">Stream</param>
        public CsvReader(Stream stream)
        {
            _type = Type.Stream;
            Initialise(stream, Encoding.Default);
        }

        /// <summary>
        /// Initialises the reader to work from an existing stream
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="encoding">Encoding</param>
        public CsvReader(Stream stream, Encoding encoding)
        {
            _type = Type.Stream;
            Initialise(stream, encoding);
        }

        /// <summary>
        /// Initialises the reader to work from a csv string
        /// </summary>
        /// <param name="encoding"></param>
        /// <param name="csvContent"></param>
        public CsvReader(Encoding encoding, string csvContent)
        {
            _type = Type.String;
            Initialise(encoding, csvContent);  
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Initialises the class to use a file
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="encoding"></param>
        private void Initialise(string filePath, Encoding encoding)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException(string.Format("The file '{0}' does not exist.", filePath));

            _fileStream = File.Open (filePath, FileMode.Open , FileAccess.Read , FileShare.ReadWrite );
            Initialise(_fileStream, encoding);
        }

        /// <summary>
        /// Initialises the class to use a stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="encoding"></param>
        private void Initialise(Stream stream, Encoding encoding)
        {
            int x = 0;x = 3;
            dynamic d;
            d = "";
            d = 0+x;
            if (stream == null)
                throw new ArgumentNullException("The supplied stream is null.");

            _stream = stream;
            _stream.Position = 0;
            _encoding = (encoding ?? Encoding.Default);
            _streamReader = new StreamReader(_stream, _encoding);
        }

        /// <summary>
        /// Initialies the class to use a string
        /// </summary>
        /// <param name="encoding"></param>
        /// <param name="csvContent"></param>
        private void Initialise(Encoding encoding, string csvContent)
        {
            if (csvContent == null)
                throw new ArgumentNullException("The supplied csvContent is null.");

            _encoding = (encoding ?? Encoding.Default);

            _memoryStream = new MemoryStream(csvContent.Length);
            _streamWriter = new StreamWriter(_memoryStream);
            _streamWriter.Write(csvContent);
            _streamWriter.Flush();
            Initialise(_memoryStream, encoding);           
        }

        /// <summary>
        /// Reads the next record
        /// </summary>
        /// <returns>True if a record was successfuly read, otherwise false</returns>
        public bool ReadNextRecord()
        {
            Fields = null;
            string line = _streamReader.ReadLine();

            if (line == null)
                return false;

            ParseLine(line);
            return true;
        }

        /// <summary>
        /// Reads a csv file format into a data table.  This method
        /// will always assume that the table has a header row as this will be used
        /// to determine the columns.
        /// </summary>
        /// <returns></returns>
        public DataTable ReadIntoDataTable()
        {
            return ReadIntoDataTable(new System.Type[] {});
        }

        /// <summary>
        /// Reads a csv file format into a data table.  This method
        /// will always assume that the table has a header row as this will be used
        /// to determine the columns.
        /// </summary>
        /// <param name="columnTypes">Array of column types</param>
        /// <returns></returns>
        public DataTable ReadIntoDataTable(System.Type[] columnTypes)
        {
            DataTable dataTable = new DataTable();
            bool addedHeader = false;
            _stream.Position = 0;

            while (ReadNextRecord())
            {
                if (!addedHeader)
                {
                    for (int i = 0; i < Fields.Count; i++)
                        dataTable.Columns.Add(Fields[i], (columnTypes.Length > 0 ? columnTypes[i] : typeof(string)));

                    addedHeader = true;
                    continue;
                }

                DataRow row = dataTable.NewRow();

                for (int i = 0; i < Fields.Count; i++)
                    row[i] = Fields[i];

                dataTable.Rows.Add(row);
            }

            return dataTable;
        }

        public Dictionary<string,List<string>> ReadIntoDictionary(bool csv_has_headers)
        {
            Dictionary<string, List<string>> dic = new Dictionary<string, List<string>>();

            List<string> items = new List<string>();

            if (csv_has_headers)
                ReadNextRecord();
            while (ReadNextRecord())
            {

                if (!dic.ContainsKey(Fields[0]))
                {
                    items = new List<string>();
                    items.Add(Fields[1]);
                    dic.Add(Fields[0], items);
                }
                else
                {
                    dic[Fields[0]].Add(Fields[1]);
                }

            }

            return dic ;
        }
        const char SeperateChar = ',';
        /// <summary>
        /// Parses a csv line
        /// </summary>
        /// <param name="line">Line</param>
        private void ParseLine(string line)
        {
            Fields = new List<string>();
            bool inQuotes = false;
            string data = "";
            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                if (c == '"')
                {
                    if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')//found two "
                    {
                        data += '"';
                        i++;
                        continue;
                    }
                    inQuotes = !inQuotes;
                }
                else if (inQuotes)
                    data += c;
                else if (c == SeperateChar)
                {
                    Fields.Add(data);
                    data = "";
                }
                else data += c;
            }
            Fields.Add(data);
        }

        /// <summary>
        /// Disposes of all unmanaged resources
        /// </summary>
        public void Dispose()
        {
            if (_streamReader != null)
            {
                _streamReader.Close();
                _streamReader.Dispose();
            }

            if (_streamWriter != null)
            {
                _streamWriter.Close();
                _streamWriter.Dispose();
            }

            if (_memoryStream != null)
            {
                _memoryStream.Close();
                _memoryStream.Dispose();
            }

            if (_fileStream != null)
            {
                _fileStream.Close();
                _fileStream.Dispose();
            }

            if ((_type == Type.String || _type == Type.File) && _stream != null)
            {
                _stream.Close();
                _stream.Dispose();
            }
        }

        #endregion Methods

    }
}

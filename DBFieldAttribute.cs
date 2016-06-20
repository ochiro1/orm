using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace ORM
{
    public enum DBFieldDirection { Read, ReadWrite, Write }
    [AttributeUsage(AttributeTargets.Property)]
    public class DBFieldAttribute : Attribute
    {
        private string _strField;

        public string Field
        {
            get { return _strField; }
            set { _strField = value; }
        }

        private SqlDbType _SqlDbType;

        public SqlDbType SqlDbType
        {
            get { return _SqlDbType; }
            set { _SqlDbType = value; }
        }

        private DBFieldDirection _direction = DBFieldDirection.ReadWrite;
        /// <summary>
        /// Read - only read from db, do not write. ReadWrite - read and write from/to db. Write - only write to db, do not read. Purpose of this property - some fields pull aggregate/virtual (calculated) data - not meant to be written back to db.
        /// </summary>
        public DBFieldDirection Direction
        {
            get { return _direction; }
            set { _direction = value; }
        }



        public DBFieldAttribute() { }

        public DBFieldAttribute(string strField, SqlDbType dbType)
        {
            _strField = strField;
            _SqlDbType = dbType;
            _direction = DBFieldDirection.ReadWrite;
        }

        public DBFieldAttribute(string strField, SqlDbType dbType, DBFieldDirection direction)
        {
            _strField = strField;
            _SqlDbType = dbType;
            _direction = direction;
        }
    }
}

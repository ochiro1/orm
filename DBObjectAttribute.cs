using System;
using System.Collections.Generic;
using System.Text;

namespace ORM
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DBObjectAttribute : Attribute
    {
        public DBObjectAttribute() { }

        private string _strWriteProcedureName = String.Empty;

        /// <summary>
        /// Gets or sets name of the stored procedure used to write object to the database
        /// </summary>
        public string WriteProcedureName
        {
            get { return _strWriteProcedureName; }
            set { _strWriteProcedureName = value; }
        }

        private string _strDeleteProcedureName = String.Empty;

        /// <summary>
        /// Gets or sets name of the stored procedure used to delete object in the database
        /// </summary>
        public string DeleteProcedureName
        {
            get { return _strDeleteProcedureName; }
            set { _strDeleteProcedureName = value; }
        }

        private string _strReadProcedureName = String.Empty;

        /// <summary>
        /// Gets or sets name of stored procedure used to read object from the database
        /// </summary>
        public string ReadProcedureName
        {
            get { return _strReadProcedureName; }
            set { _strReadProcedureName = value; }
        }

        private string _strSaveMultipleProcedureName = String.Empty;

        /// <summary>
        /// Gets or sets name of Stored Procedure Used for Saving Multiple Like DB Objects At Once.
        /// </summary>
        public string SaveMultipleProcedureName
        {
            get { return this._strSaveMultipleProcedureName; }
            set { this._strSaveMultipleProcedureName = value; }
        }

        private string _strSaveXmlProcedureName = String.Empty;

        public string SaveXmlProcedureName
        {
            get { return _strSaveXmlProcedureName; }
            set { _strSaveXmlProcedureName = value; }
        }
    }
}

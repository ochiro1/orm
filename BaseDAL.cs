using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;
using Microsoft.ApplicationBlocks.Data;
using Microsoft.ApplicationBlocks.ExceptionManagement;

namespace ORM
{
    public class BaseDAL
    {
        public virtual string GetConnectionString() { return string.Empty; }

        #region SaveObject

        public void SaveObject(object theObject)
        {
            string procName = String.Empty;
            foreach (DBObjectAttribute attribute in theObject.GetType().GetCustomAttributes(typeof(DBObjectAttribute), false))
                procName = attribute.WriteProcedureName;

            SqlHelper.ExecuteNonQuery(
                GetConnectionString(),
                CommandType.StoredProcedure,
                procName,
                DataMapper.BuildParameters(theObject));
        }
        #endregion

        #region SaveObjectsWithXml

        public void SaveObjectsWithXml<T>(IList<T> listOfObjects)
        {
            if (listOfObjects == null || listOfObjects.Count == 0)
                return;

            string procName = String.Empty;
            foreach (DBObjectAttribute attribute in listOfObjects[0].GetType().GetCustomAttributes(typeof(DBObjectAttribute), false))
                procName = attribute.SaveXmlProcedureName;

            if (String.IsNullOrEmpty(procName))
            {
                Exception ex = new Exception("SaveXmlProcedureName not defined for type " + listOfObjects[0].GetType().ToString());
                Microsoft.ApplicationBlocks.ExceptionManagement.ExceptionManager.Publish(ex);
                throw ex;
            }

            try
            {
                SqlHelper.ExecuteNonQuery(
                    GetConnectionString(),
                    CommandType.StoredProcedure,
                    procName,
                    CreateParameter("@xml", SqlDbType.Text, DataMapper.BuildXml(listOfObjects)));
            }
            catch (Exception ex)
            {
                Microsoft.ApplicationBlocks.ExceptionManagement.ExceptionManager.Publish(ex);
                throw;
            }
        }

        #endregion

        #region DeleteObject(s)
        #region DeleteObject(Type type, Guid guidID) (Obsolete)
        [Obsolete]
        public void DeleteObject(Type type, Guid guidID)
        {
            string procName = String.Empty;
            foreach (DBObjectAttribute attribute in type.GetCustomAttributes(typeof(DBObjectAttribute), false))
            {
                procName = attribute.DeleteProcedureName;
            }
            if (procName != String.Empty)
            {
                try
                {
                    SqlHelper.ExecuteNonQuery(
                        GetConnectionString(),
                        CommandType.StoredProcedure,
                        procName,
                        CreateParameter("@ID", SqlDbType.UniqueIdentifier, guidID));
                }
                catch (Exception ex)
                {
                    Microsoft.ApplicationBlocks.ExceptionManagement.ExceptionManager.Publish(ex);
                    throw;
                }
            }
        }
        #endregion
        #region DeleteObject(object theObject)
        public void DeleteObject(object theObject)
        {
            string procName = String.Empty;
            foreach (DBObjectAttribute attribute in theObject.GetType().GetCustomAttributes(typeof(DBObjectAttribute), false))
            {
                procName = attribute.DeleteProcedureName;
            }
            if (procName != String.Empty)
            {
                // find PK field(s)
                List<SqlParameter> parameters = new List<SqlParameter>();
                foreach (PropertyInfo property in theObject.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    foreach (PrimaryKeyAttribute PKAttribute in property.GetCustomAttributes(typeof(PrimaryKeyAttribute), false))
                    {
                        foreach (DBFieldAttribute attribute in property.GetCustomAttributes(typeof(DBFieldAttribute), false))
                        {
                            parameters.Add(BaseDAL.CreateParameter("@" + attribute.Field, attribute.SqlDbType, property.GetValue(theObject, null)));
                        }
                    }
                }

                try
                {
                    SqlHelper.ExecuteNonQuery(
                        GetConnectionString(),
                        CommandType.StoredProcedure,
                        procName,
                        parameters.ToArray());
                }
                catch (Exception ex)
                {
                    Microsoft.ApplicationBlocks.ExceptionManagement.ExceptionManager.Publish(ex);
                    throw;
                }
            }
        }
        #endregion
        #region DeleteObjects(Type type, string strIDs)
        public void DeleteObjects(Type type, string strIDs)
        {
            string procName = String.Empty;
            foreach (DBObjectAttribute attribute in type.GetCustomAttributes(typeof(DBObjectAttribute), false))
            {
                procName = attribute.DeleteProcedureName;
            }
            if (procName != String.Empty)
            {
                try
                {
                    SqlHelper.ExecuteNonQuery(
                        GetConnectionString(),
                        CommandType.StoredProcedure,
                        procName,
                        CreateParameter("@ID", SqlDbType.VarChar, strIDs));
                }
                catch (Exception ex)
                {
                    Microsoft.ApplicationBlocks.ExceptionManagement.ExceptionManager.Publish(ex);
                    throw;
                }
            }
        }
        #endregion
        #endregion

        #region GetObject

        public DataSet GetObject(Type type, Guid guidID)
        {
            string procName = String.Empty;
            foreach (DBObjectAttribute attribute in type.GetCustomAttributes(typeof(DBObjectAttribute), false))
            {
                procName = attribute.ReadProcedureName;
            }
            if (procName != String.Empty)
            {
                try
                {
                    return SqlHelper.ExecuteDataset(
                                            GetConnectionString(),
                                            CommandType.StoredProcedure,
                                            procName,
                                            CreateParameter("@ID", SqlDbType.UniqueIdentifier, guidID));
                }
                catch (Exception ex)
                {
                    Microsoft.ApplicationBlocks.ExceptionManagement.ExceptionManager.Publish(ex);
                    throw;
                }
            }
            return null;
        }

        public DataSet GetObject(object theObject)
        {
            string procName = String.Empty;
            foreach (DBObjectAttribute attribute in theObject.GetType().GetCustomAttributes(typeof(DBObjectAttribute), false))
            {
                procName = attribute.ReadProcedureName;
            }
            if (procName != String.Empty)
            {
                // find PK field(s)
                List<SqlParameter> parameters = new List<SqlParameter>();
                foreach (PropertyInfo property in theObject.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    foreach (PrimaryKeyAttribute PKAttribute in property.GetCustomAttributes(typeof(PrimaryKeyAttribute), false))
                    {
                        foreach (DBFieldAttribute attribute in property.GetCustomAttributes(typeof(DBFieldAttribute), false))
                        {
                            parameters.Add(BaseDAL.CreateParameter("@" + attribute.Field, attribute.SqlDbType, property.GetValue(theObject, null)));
                        }
                    }
                }
                try
                {
                    return SqlHelper.ExecuteDataset(
                                            GetConnectionString(),
                                            CommandType.StoredProcedure,
                                            procName,
                                            parameters.ToArray());
                }
                catch (Exception ex)
                {
                    Microsoft.ApplicationBlocks.ExceptionManagement.ExceptionManager.Publish(ex);
                    throw;
                }
            }
            return null;
        }

        #endregion

        #region CreateParameter

        public static SqlParameter CreateParameter(string paramName, SqlDbType paramType, object paramValue)
        {
            SqlParameter param = new SqlParameter(paramName, paramType);
            object newParamValue = paramValue;
            if (paramValue != DBNull.Value)
            {
                switch (paramType)
                {
                    case SqlDbType.VarChar:
                        break;
                    case SqlDbType.NVarChar:
                        break;
                    case SqlDbType.Char:
                        break;
                    case SqlDbType.NChar:
                        break;
                    case SqlDbType.Text:
                        newParamValue = CheckParamValue((string)paramValue);
                        break;
                    case SqlDbType.Float:
                        newParamValue = CheckParamValue((float?)paramValue);
                        break;
                    case SqlDbType.UniqueIdentifier:
                        newParamValue = CheckParamValue((Guid?)paramValue);
                        break;
                    case SqlDbType.Int:
                        newParamValue = CheckParamValue((int?)paramValue);
                        break;
                    case SqlDbType.DateTime:
                        newParamValue = CheckParamValue((DateTime?)paramValue);
                        break;
                    case SqlDbType.Image:
                        newParamValue = CheckParamValue((byte[])paramValue);
                        break;
                }
            }
            param.Value = newParamValue;
            return param;
        }

        public static SqlParameter CreateParameter(string paramName, SqlDbType paramType, object paramValue, ParameterDirection paramDirection)
        {
            SqlParameter param = CreateParameter(paramName, paramType, paramValue);
            param.Direction = paramDirection;
            return param;
        }

        private static object CheckParamValue(string paramValue)
        {
            if (string.IsNullOrEmpty(paramValue))
            {
                return DBNull.Value;
            }
            else
            {
                return paramValue;
            }
        }

        private static object CheckParamValue(float? paramValue)
        {
            if (paramValue == null)
            {
                return DBNull.Value;
            }
            else
            {
                return paramValue;
            }
        }

        private static object CheckParamValue(Guid paramValue)
        {
            if (paramValue == Guid.Empty)
            {
                return DBNull.Value;
            }
            else
            {
                return paramValue;
            }
        }

        private static object CheckParamValue(Guid? paramValue)
        {
            if (paramValue == null)
            {
                return DBNull.Value;
            }
            else
            {
                return paramValue;
            }
        }

        private static object CheckParamValue(int? paramValue)
        {
            if (paramValue == null)
            {
                return DBNull.Value;
            }
            else
            {
                return paramValue;
            }
        }

        private static object CheckParamValue(DateTime? paramValue)
        {
            if (paramValue == null)
                return DBNull.Value;
            else
                return (DateTime)paramValue;
        }

        private static object CheckParamValue(byte[] paramValue)
        {
            if (paramValue == null)
                return DBNull.Value;
            else
                return paramValue;
        }

        #endregion
    }
}

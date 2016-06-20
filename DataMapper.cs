using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;

namespace ORM
{
    public class DataMapper
    {
        #region PopulateObject (object from data)
        public static void PopulateObject(object BaseObject, DataRow dr)
        {
            // added binding flags to access properties declared as internal (Loan.StatusCode - used for DB loading only)
            foreach (PropertyInfo property in BaseObject.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                foreach (DBFieldAttribute attribute in property.GetCustomAttributes(typeof(DBFieldAttribute), false))
                {
                    if (attribute.Direction == DBFieldDirection.Read || attribute.Direction == DBFieldDirection.ReadWrite)
                    {
                        if (dr.Table.Columns.Contains(attribute.Field))
                        {
                            if (property.PropertyType.FullName == "System.String")
                            {
                                property.SetValue(BaseObject, dr[attribute.Field].ToString(), null);
                            }
                            else if (property.PropertyType.FullName == "System.Decimal")
                            {
                                property.SetValue(BaseObject, Decimal.Parse(dr[attribute.Field].ToString()), null);
                            }
                            else if (property.PropertyType.FullName == "System.Single")
                            {
                                property.SetValue(BaseObject, Single.Parse(dr[attribute.Field].ToString()), null);
                            }
                            else if (property.PropertyType.FullName == "System.Guid")
                            {
                                property.SetValue(BaseObject, (System.Guid)dr[attribute.Field], null);
                            }
                            else if (property.PropertyType.FullName == "System.Int32")
                            {
                                property.SetValue(BaseObject, int.Parse(dr[attribute.Field].ToString()), null);
                            }
                            else if (property.PropertyType.FullName == "System.Boolean")
                            {
                                property.SetValue(BaseObject, bool.Parse(dr[attribute.Field].ToString()), null);
                            }
                            else if (property.PropertyType.FullName == "System.DateTime")
                            {
                                property.SetValue(BaseObject, DateTime.Parse(dr[attribute.Field].ToString()), null);
                            }
                            else if (property.PropertyType.FullName.IndexOf("System.Nullable") != -1 &&
                                    property.PropertyType.FullName.IndexOf("System.Single") != -1)
                            {
                                property.SetValue(
                                    BaseObject,
                                    (dr[attribute.Field] == DBNull.Value) ? null : (float?)Single.Parse(dr[attribute.Field].ToString()),
                                    null);
                            }
                            else if (property.PropertyType.FullName.IndexOf("System.Nullable") != -1 &&
                                    property.PropertyType.FullName.IndexOf("System.Guid") != -1)
                            {
                                property.SetValue(
                                    BaseObject,
                                    dr[attribute.Field] == DBNull.Value ? null : (System.Guid?)dr[attribute.Field],
                                    null);
                            }
                            else if (property.PropertyType.FullName.IndexOf("System.Nullable") != -1 &&
                               property.PropertyType.FullName.IndexOf("System.DateTime") != -1)
                            {
                                property.SetValue(
                                    BaseObject,
                                    dr[attribute.Field] == DBNull.Value ? null : (System.DateTime?)(DateTime)dr[attribute.Field],
                                    null);
                            }
                            else if (property.PropertyType.FullName.IndexOf("System.Nullable") != -1 &&
                          property.PropertyType.FullName.IndexOf("System.Int32") != -1)
                            {
                                property.SetValue(
                                    BaseObject,
                                     dr[attribute.Field] == DBNull.Value ? null : (int?)System.Int32.Parse(dr[attribute.Field].ToString()),
                                    null);
                            }
                            else if (property.PropertyType.FullName.IndexOf("System.Nullable") != -1 &&
                          property.PropertyType.FullName.IndexOf("System.Boolean") != -1)
                            {
                                object objValue = null;
                                if (dr[attribute.Field] != DBNull.Value)
                                {
                                    if (dr[attribute.Field].ToString() == "1" || String.Compare(dr[attribute.Field].ToString(), "true", true) == 0)
                                        objValue = true;
                                    else
                                        objValue = false;
                                }
                                property.SetValue(
                                    BaseObject,
                                     objValue,
                                    null);
                            }
                            else if (property.PropertyType.FullName.IndexOf("System.Byte[]") != -1)
                            {
                                property.SetValue(
                                    BaseObject,
                                    dr[attribute.Field] == DBNull.Value ? null : (byte[])dr[attribute.Field],
                                    null);
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region BuildParameters
        public static SqlParameter[] BuildParameters(object BaseObject)
        {
            List<SqlParameter> list = new List<SqlParameter>();

            // added binding flags to access properties declared as internal (Loan.StatusCode - used for DB loading only)
            foreach (PropertyInfo property in BaseObject.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                foreach (DBFieldAttribute attribute in property.GetCustomAttributes(typeof(DBFieldAttribute), false))
                {
                    if (attribute.Direction == DBFieldDirection.ReadWrite || attribute.Direction == DBFieldDirection.Write)
                    {
                        list.Add(BaseDAL.CreateParameter("@" + attribute.Field, attribute.SqlDbType, property.GetValue(BaseObject, null)));
                    }
                }
            }
            return list.ToArray();
        }
        #endregion

        #region BuildXml
        public static string BuildXml<T>(IList<T> listOfObjects)
        {
            TagNode rootNode = new TagNode("ROOT");
            TagNode objectNode = null;
            foreach (T BaseObject in listOfObjects)
            {
                objectNode = new TagNode(BaseObject.GetType().Name);

                // added binding flags to access properties declared as internal (Loan.StatusCode - used for DB loading only)
                foreach (PropertyInfo property in BaseObject.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    foreach (DBFieldAttribute attribute in property.GetCustomAttributes(typeof(DBFieldAttribute), false))
                    {
                        if (attribute.Direction == DBFieldDirection.ReadWrite || attribute.Direction == DBFieldDirection.Write)
                        {
                            objectNode.AddAttribute(attribute.Field, _GetValueForXml(property, BaseObject, attribute));
                        }
                    }
                }
                rootNode.AddChildNode(objectNode);
                objectNode = null;
            }
            return rootNode.ToString();
        }
        #endregion

        private static string _GetValueForXml(PropertyInfo property, object BaseObject, DBFieldAttribute dbFieldAttribute)
        {
            string ret = String.Empty;

            object objPropertyValue = property.GetValue(BaseObject, null);

            if (objPropertyValue == null)
                return String.Empty;

            switch (dbFieldAttribute.SqlDbType)
            {
                case SqlDbType.Bit:
                    ret = (String.Compare(objPropertyValue.ToString(), "true", true) == 0) ? "1" : "0";
                    break;
                default:
                    ret = objPropertyValue.ToString();
                    break;
            }
            return ret;
        }

        #region UTF8ByteArrayToString
        /// <summary>
        /// Convert UTF8 Byte Array to String (Used Internal Only).
        /// </summary>
        protected static string UTF8ByteArrayToString(byte[] characters)
        {
            UnicodeEncoding encoding = new UnicodeEncoding();
            string constructedString = encoding.GetString(characters);
            return constructedString;
        }
        #endregion
    }
}

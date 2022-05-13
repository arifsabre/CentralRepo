using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Web;

namespace ManufacturingManagement_V2.Models
{
    public static class clsTableToModel
    {
        //
        public static List<T> ConvertToList<T>(DataSet ds)
        {
            //to be used directly instead of createObjectListX, as-
            //return clsTableToModel.ConvertToList<PartyMdl>(ds);
            var columnNames = ds.Tables[0].Columns.Cast<DataColumn>()
                .Select(c => c.ColumnName.ToLower()).ToList();
            var properties = typeof(T).GetProperties();
            return ds.Tables[0].AsEnumerable().Select(row => 
            {
                var objT = Activator.CreateInstance<T>();
                foreach (var prop in properties)
                {
                    if (columnNames.Contains(prop.Name.ToLower()))
                    {
                        try
                        {
                            prop.SetValue(objT, row[prop.Name]);
                        }
                        catch (Exception ex) 
                        {
                            string str = ex.Message;
                        }
                    }
                }
                return objT;
            }).ToList();
        }
        //

        public static DataSet getDataForModel<T>(string kwp, DataSet ds)
        {
            DataSet dsRet = new DataSet();
            dsRet.Tables.Add();
            var columnNames = ds.Tables[0].Columns.Cast<DataColumn>()
                .Select(c => c.ColumnName.ToLower())
                .Where(s => s.StartsWith(kwp.ToLower())).ToList();
            var properties = typeof(T).GetProperties();
            foreach (var prop in properties)
            {
                if (columnNames.Contains((kwp + prop.Name).ToLower()))
                {
                    DataColumn dtc = new DataColumn();
                    dtc.ColumnName = prop.Name;
                    dtc.DataType = prop.PropertyType;
                    dsRet.Tables[0].Columns.Add(dtc);
                }
            }
            DataRow dr = dsRet.Tables[0].NewRow();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                dr = dsRet.Tables[0].NewRow();
                for (int x = 0; x < dsRet.Tables[0].Columns.Count; x++)
                {
                    dr[x] = ds.Tables[0].Rows[i][kwp + dsRet.Tables[0].Columns[x]];
                }
                dsRet.Tables[0].Rows.Add(dr);
            }
            return dsRet;
        }
        //

    }
}
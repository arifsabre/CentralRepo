using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Configuration;
using System.IO;
using System.Net.Mail;
using System.Web.Mvc;

namespace ManufacturingManagement_V2.Models
{
    public class AAA_ITAssestCell_BLL
    {
        
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ERPConnection"].ToString());
        clsMyClass mc = new clsMyClass();



        public  List<SelectListItem> PopulateDropDown(string query, string textColumn, string valueColumn)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            string constr = ConfigurationManager.ConnectionStrings["ErpConnection"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new SelectListItem
                            {
                                Text = sdr[textColumn].ToString(),
                                Value = sdr[valueColumn].ToString()
                            });
                        }
                    }
                    con.Close();

                }
            }

            return items;
        }
     public List<AAA_ITAssestCell_MDI> Get_Item_List()
        {
            //clsMyClass mc = new clsMyClass();
            string cs = mc.strconn;

            List<AAA_ITAssestCell_MDI> HList = new List<AAA_ITAssestCell_MDI>();
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                SqlCommand com = new SqlCommand("ZZZ_GetItemNameMaster_List", con);
                com.CommandType = CommandType.StoredProcedure;
                SqlDataReader rdr = com.ExecuteReader();
                while (rdr.Read())
                {
                    HList.Add(new AAA_ITAssestCell_MDI
                    {
                        Tran_Id = Convert.ToInt32(rdr["Tran_Id"]),
                        Item_Type = rdr["Item_Type"].ToString(),
                        Item_Name = rdr["Item_Name"].ToString(),
                        Serial_No = rdr["Serial_No"].ToString(),
                        Stock = Convert.ToInt32(rdr["Stock"]),
                        Purchaage_Date = Convert.ToDateTime(rdr["Purchaage_Date"]),
                        Warranty_Date = Convert.ToDateTime(rdr["Warranty_Date"]),
                        //Invoice_No = rdr["Invoice_No"].ToString(),
                       // Scrap_Date = Convert.ToDateTime(rdr["Scrap_Date"]),
                        cmpname = rdr["Location"].ToString(),
                        Supplier_Name = rdr["Supplier_Name"].ToString(),
                        //Scrap_Item = rdr["Scrap_Item"].ToString(),
                        Comment = rdr["Comment"].ToString(),
                      
                        
                      });
                }
                return HList;
            }
        }
            public List<AAA_ITAssestCell_MDI> Get_ScrapItem_List()
            {
            //clsMyClass mc = new clsMyClass();
        
            string cs = mc.strconn;
            List<AAA_ITAssestCell_MDI> HList = new List<AAA_ITAssestCell_MDI>();
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                SqlCommand com = new SqlCommand("ZZZ_GetScrapItem_List", con);
                com.CommandType = CommandType.StoredProcedure;
                SqlDataReader rdr = com.ExecuteReader();
                while (rdr.Read())
                {
                    HList.Add(new AAA_ITAssestCell_MDI
                    {
                        Tran_Id = Convert.ToInt32(rdr["Tran_Id"]),
                        Item_Name = rdr["Item_Name"].ToString(),
                        Serial_No = rdr["Serial_No"].ToString(),
                        Issue_Qty = Convert.ToInt32(rdr["Issue_Qty"]),
                        Scrap_Qty = Convert.ToInt32(rdr["Scrap_Qty"]),
                        Scrap_Item = rdr["Scrap_Item"].ToString(),
                       // Location = rdr["Location"].ToString(),
                        Scrap_Comment = rdr["Scrap_Comment"].ToString(),
                        
                                      
                    });
                }
                return HList;
            }
        }


        public List<AAA_ITAssestCell_MDI> Get_ScrapItem_ListForApproval()
        {
            //clsMyClass mc = new clsMyClass();

            string cs = mc.strconn;
            List<AAA_ITAssestCell_MDI> HList = new List<AAA_ITAssestCell_MDI>();
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                SqlCommand com = new SqlCommand("[ZZZ_GetScrapItem_ListForApproval]", con);
                com.CommandType = CommandType.StoredProcedure;
                SqlDataReader rdr = com.ExecuteReader();
                while (rdr.Read())
                {
                    HList.Add(new AAA_ITAssestCell_MDI
                    {
                        Tran_Id = Convert.ToInt32(rdr["Tran_Id"]),
                        Item_Name = rdr["Item_Name"].ToString(),
                        Serial_No = rdr["Serial_No"].ToString(),
                        Issue_Qty = Convert.ToInt32(rdr["Issue_Qty"]),
                        Scrap_Qty = Convert.ToInt32(rdr["Scrap_Qty"]),
                        Scrap_Item = rdr["Scrap_Item"].ToString(),
                        Location = rdr["Location"].ToString(),
                        Scrap_Comment = rdr["Scrap_Comment"].ToString(),
                        Approval_Comment = rdr["Approval_Comment"].ToString(),
                        Approoved = rdr["Approoved"].ToString(),




                    });
                }
                return HList;
            }
        }

         public List<AAA_ITAssestCell_MDI> Get_Issue_Item_List()
        {
            //clsMyClass mc = new clsMyClass();

            string cs = mc.strconn;
            List<AAA_ITAssestCell_MDI> HList = new List<AAA_ITAssestCell_MDI>();
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                SqlCommand com = new SqlCommand("ZZZ_USP_Get_Issue_Item_ITCell_List", con);
                com.CommandType = CommandType.StoredProcedure;
                SqlDataReader rdr = com.ExecuteReader();
                while (rdr.Read())
                {
                    HList.Add(new AAA_ITAssestCell_MDI
                    {
                        Tran_Id = Convert.ToInt32(rdr["Tran_Id"]),
                        Item_Type = rdr["Item_Type"].ToString(),
                        Item_Name = rdr["Item_Name"].ToString(),
                        Serial_No = rdr["Serial_No"].ToString(),
                        Issue_Qty = Convert.ToInt32(rdr["Issue_Qty"]),
                        Issue_Date = Convert.ToDateTime(rdr["Issue_Date"]),
                        EmpName = rdr["EmpName"].ToString(),
                        Comment = rdr["Comment"].ToString(),
           });
                }
                return HList;
            }
        }
       

        public List<AAA_ITAssestCell_MDI> Get_Item_List_Type()
        {
            clsMyClass mc = new clsMyClass();
            string cs = mc.strconn;
            List<AAA_ITAssestCell_MDI> HList = new List<AAA_ITAssestCell_MDI>();
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                SqlCommand com = new SqlCommand("[dbo].[ZZZ_Get_Item_List_Type]", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                SqlDataReader rdr = com.ExecuteReader();
                while (rdr.Read())
                {
                    HList.Add(new AAA_ITAssestCell_MDI
                    {
                        Item_Type_Id = Convert.ToInt32(rdr["Item_Type_Id"]),
                        Item_Type = rdr["Item_Type"].ToString(),


                    });
                }
                return HList;
            }
        }




        public int Insert_ItemType(AAA_ITAssestCell_MDI hld)
        {
            clsMyClass mc = new clsMyClass();
            string cs = mc.strconn;
            int i;
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                SqlCommand com = new SqlCommand("ZZZ_USP_Insert_AAA_Item Type", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@Item_Type", hld.Item_Type);
                i = com.ExecuteNonQuery();
            }
            return i;
        }

        public int Update_ItemType(AAA_ITAssestCell_MDI hld)
        {
            clsMyClass mc = new clsMyClass();
            string cs = mc.strconn;
            int i;
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                SqlCommand com = new SqlCommand("ZZZ_USP_Update_AAA_Item Type", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@Item_Type_Id", hld.Item_Type_Id);
                com.Parameters.AddWithValue("@Item_Type", hld.Item_Type);
                i = com.ExecuteNonQuery();
            }
            return i;
        }

        public List<AAA_ITAssestCell_MDI> Get_MasterItem_List_ItCell()
        {
            clsMyClass mc = new clsMyClass();
            string cs = mc.strconn;
            List<AAA_ITAssestCell_MDI> HList = new List<AAA_ITAssestCell_MDI>();
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                SqlCommand com = new SqlCommand("[dbo].[ZZZ_Get_MasterItem_List_ItCell]", con);
                com.CommandType = CommandType.StoredProcedure;
                SqlDataReader rdr = com.ExecuteReader();
                while (rdr.Read())
                {
                    HList.Add(new AAA_ITAssestCell_MDI
                    {
                        Item_Id = Convert.ToInt32(rdr["Item_Id"]),
                        Item_Name = rdr["Item_Name"].ToString(),
                        //StockMaster = Convert.ToInt32(rdr["StockMaster"]),


                    });
                }
                return HList;
            }
        }


        public int Insert_ItemMaster_ItCell(AAA_ITAssestCell_MDI hld)
        {
            clsMyClass mc = new clsMyClass();
            string cs = mc.strconn;
            int i;
            using (SqlConnection con = new SqlConnection(cs))
            {

                con.Open();
                SqlCommand com = new SqlCommand("ZZZ_USP_Insert_AAA_Item Master_ITCell", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@Item_Type_Id", hld.Item_Type_Id);
                com.Parameters.AddWithValue("@Item_Name", hld.Item_Name);
                //com.Parameters.AddWithValue("@StockMaster", hld.StockMaster);
                i = com.ExecuteNonQuery();
            }
            return i;
        }


        public int Update_ItemMaster_ItCell(AAA_ITAssestCell_MDI hld)
        {
            clsMyClass mc = new clsMyClass();
            string cs = mc.strconn;
            int i;
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                SqlCommand com = new SqlCommand("ZZZ_USP_Update_AAA_Item Master_ITCell", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@Item_Id", hld.Item_Id);
                com.Parameters.AddWithValue("@Item_Name", hld.Item_Name);
               // com.Parameters.AddWithValue("@StockMaster", hld.StockMaster);
                i = com.ExecuteNonQuery();
            }
            return i;
        }

        public int Add_Item_MasterName_ITCell(AAA_ITAssestCell_MDI hld)
        {
            clsMyClass mc = new clsMyClass();
            string cs = mc.strconn;
            int i;

            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                SqlCommand com = new SqlCommand("ZZZ_USP_Add_AAA_Item_Master_ItCell", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@Item_Type_Id1", hld.Tran_Id);
                com.Parameters.AddWithValue("@Item_Id1", hld.Item_Id);
                com.Parameters.AddWithValue("@Supplier_Id1", hld.Supplier_Id);
                com.Parameters.AddWithValue("@Serial_No1", hld.Serial_No);
                com.Parameters.AddWithValue("@Stock1", hld.Stock);
                com.Parameters.AddWithValue("@Purchaage_Date1", hld.Purchaage_Date);
                com.Parameters.AddWithValue("@Warranty_Date1", hld.Warranty_Date);
                com.Parameters.AddWithValue("@Scrap_Item1", hld.Scrap_Item);
                com.Parameters.AddWithValue("@compcode1", hld.compcode);
                com.Parameters.AddWithValue("@Invoice_No1", hld.Invoice_No);
                com.Parameters.AddWithValue("@Comment1", hld.Comment);
                i = com.ExecuteNonQuery();
            }

            return i;

        }




           public int Insert_Item_Name_ITCell(AAA_ITAssestCell_MDI hld)
        {
            clsMyClass mc = new clsMyClass();
            string cs = mc.strconn;
            int i;
         
            using (SqlConnection con = new SqlConnection(cs))
            {
                    con.Open();
                    SqlCommand com = new SqlCommand("ZZZ_USP_Insert_AAA_Item_Name", con);
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@ddlItem_Type_Id1", hld.Tran_Id);
                    com.Parameters.AddWithValue("@Item_Name1", hld.Item_Name);
                    com.Parameters.AddWithValue("@Serial_No1", hld.Serial_No);
                    com.Parameters.AddWithValue("@Stock1", hld.Stock);
                    com.Parameters.AddWithValue("@Purchaage_Date1", hld.Purchaage_Date);
                    com.Parameters.AddWithValue("@Warranty_Date1", hld.Warranty_Date);
                    com.Parameters.AddWithValue("@Supplier_Id1", hld.Supplier_Id);
                    com.Parameters.AddWithValue("@Scrap_Item1", hld.Scrap_Item);
                    com.Parameters.AddWithValue("@compcode1", hld.compcode);
                    com.Parameters.AddWithValue("@Invoice_No1", hld.Invoice_No);
                    com.Parameters.AddWithValue("@Comment1", hld.Comment);
                    i = com.ExecuteNonQuery();
                }

                return i;
            
        }

           public int Insert_Item_CreditName_ITCell(AAA_ITAssestCell_MDI hld)
           {
               clsMyClass mc = new clsMyClass();
               string cs = mc.strconn;
               int i;
                         
               using (SqlConnection con = new SqlConnection(cs))
               {
                   con.Open();
                   SqlCommand com = new SqlCommand("ZZZ_USP_Insert_AAA_Item_MasterName_ItCell", con);
                   com.CommandType = CommandType.StoredProcedure;
                   com.Parameters.AddWithValue("@Item_Type_Id1", hld.Item_Type_Id);
                   com.Parameters.AddWithValue("@Item_Id1", hld.Item_Id);
                   com.Parameters.AddWithValue("@Serial_No1", hld.Serial_No);
                   com.Parameters.AddWithValue("@Stock1", hld.Stock);
                   com.Parameters.AddWithValue("@Purchaage_Date1", hld.Purchaage_Date);
                   com.Parameters.AddWithValue("@Warranty_Date1", hld.Warranty_Date);
                   com.Parameters.AddWithValue("@Supplier_Id1", hld.Supplier_Id);
                   com.Parameters.AddWithValue("@Scrap_Item1", hld.Scrap_Item);
                   com.Parameters.AddWithValue("@compcode1", hld.compcode);
                   //com.Parameters.AddWithValue("@Invoice_No1", hld.Invoice_No);
                   com.Parameters.AddWithValue("@Comment1", hld.Comment);
                   com.Parameters.AddWithValue("@Plus1", hld.Plus);
                com.Parameters.AddWithValue("@Minus1", hld.Minus);

                i = com.ExecuteNonQuery();
               }

               return i;

           }

        public int Insert_Isssue_Item_Name_ITCELL(AAA_ITAssestCell_MDI hlu)
        {
            clsMyClass mc = new clsMyClass();
            string cs = mc.strconn;
            int i;
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                SqlCommand com = new SqlCommand("ZZZ_USP_Issue_ItemNameMaster_ITCell", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@Item_Id", hlu.Item_Id);
                com.Parameters.AddWithValue("@Item_Type_Id", hlu.Item_Type_Id);
                com.Parameters.AddWithValue("@Serial_No", hlu.Serial_No);
                com.Parameters.AddWithValue("@Issue_Date", hlu.Issue_Date);
                com.Parameters.AddWithValue("@Issue_Qty", hlu.Issue_Qty);
                com.Parameters.AddWithValue("@NewEmpId", hlu.NewEmpId);
                com.Parameters.AddWithValue("@Comment", hlu.Comment);
                com.Parameters.AddWithValue("@Minus", hlu.Minus);
                i = com.ExecuteNonQuery();
            }
            return i;
        }

        public int Update_ScrapItem_ITCELL(AAA_ITAssestCell_MDI hlu)
        {
            clsMyClass mc = new clsMyClass();
            string cs = mc.strconn;
            int i;
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                SqlCommand com = new SqlCommand("ZZZ_USP_Update_AAA_ScrapItem", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                com.Parameters.AddWithValue("@Tran_Id", hlu.Tran_Id);
                //com.Parameters.AddWithValue("@Serial_No", hlu.Serial_No);
                //com.Parameters.AddWithValue("@Stock", hlu.Stock);
                com.Parameters.AddWithValue("@Issue_Qty", hlu.Issue_Qty);
                com.Parameters.AddWithValue("@Scrap_Qty", hlu.Scrap_Qty);
                com.Parameters.AddWithValue("@Scrap_Item", hlu.Scrap_Item);
               // com.Parameters.AddWithValue("@Location", hlu.Location);
                com.Parameters.AddWithValue("@Scrap_Comment", hlu.Scrap_Comment);
                com.Parameters.AddWithValue("@Scrap1", hlu.Scrap1);
               
               

                i = com.ExecuteNonQuery();
            }
            return i;
        }
        public int Update_InvoiceDetail_ITCELL(AAA_ITAssestCell_MDI hlu)
        {
            clsMyClass mc = new clsMyClass();
            string cs = mc.strconn;
            int i;
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                SqlCommand com = new SqlCommand("AAA_Update_InvoiceDetail_ITCell", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@RecordId", hlu.RecordId);
               
                com.Parameters.AddWithValue("@OrderNo", hlu.OrderNo);
                com.Parameters.AddWithValue("@Invoice_No", hlu.Invoice_No);
                com.Parameters.AddWithValue("@BillNo", hlu.BillNo);
               
                com.Parameters.AddWithValue("@ItemQty", hlu.ItemQty);
                com.Parameters.AddWithValue("@ItemQty", hlu.ItemQty);
                com.Parameters.AddWithValue("@Remark", hlu.Remark);



                i = com.ExecuteNonQuery();
            }
            return i;
        }
        public int Update_ScrapItemForApproval_ITCELL(AAA_ITAssestCell_MDI hlu)
        {
            clsMyClass mc = new clsMyClass();
            string cs = mc.strconn;
            int i;
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                SqlCommand com = new SqlCommand("ZZZ_USP_Update_AAA_ScrapItemForApproval", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@Tran_Id", hlu.Tran_Id);
               // com.Parameters.AddWithValue("@Item_Name", hlu.Item_Name);
                com.Parameters.AddWithValue("@Serial_No", hlu.Serial_No);
                com.Parameters.AddWithValue("@Issue_Qty", hlu.Issue_Qty);
                com.Parameters.AddWithValue("@Scrap_Qty", hlu.Scrap_Qty);

                com.Parameters.AddWithValue("@Scrap_Date", hlu.Scrap_Date);
                com.Parameters.AddWithValue("@Scrap_Comment", hlu.Scrap_Comment);
                //com.Parameters.AddWithValue("@Location", hlu.Location);
               // com.Parameters.AddWithValue("@Scrap_Item", hlu.Scrap_Item);
                com.Parameters.AddWithValue("@Approoved", hlu.Approoved);
                com.Parameters.AddWithValue("@Approval_Comment", hlu.Approval_Comment);
                com.Parameters.AddWithValue("@Scrap2", hlu.Scrap2);
                com.Parameters.AddWithValue("@Scrap3", hlu.Scrap3);
              


                i = com.ExecuteNonQuery();
            }
            return i;
        }

        public int Update_Item_NameMaster_Credit_ITCELL(AAA_ITAssestCell_MDI hlu)
        {
            clsMyClass mc = new clsMyClass();
            string cs = mc.strconn;
            int i;
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                SqlCommand com = new SqlCommand("ZZZ_USP_Update_ItemNameMaster_ITCell", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@Tran_Id", hlu.Tran_Id);
                //com.Parameters.AddWithValue("@Item_Name", hlu.Item_Name);
                com.Parameters.AddWithValue("@Serial_No", hlu.Serial_No);
                com.Parameters.AddWithValue("@Stock", hlu.Stock);
               // com.Parameters.AddWithValue("@Invoice_No", hlu.Invoice_No);
                com.Parameters.AddWithValue("@Comment", hlu.Comment);
               
              
                      
               i = com.ExecuteNonQuery();
            }
            return i;
        }

        public int Update_Isssue_Item_Name_ITCELL1(AAA_ITAssestCell_MDI hlu)
        {
            clsMyClass mc = new clsMyClass();
            string cs = mc.strconn;
            int i;
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                SqlCommand com = new SqlCommand("[ZZZ_USP_Update_Issue_ItemNameMaster_ITCell]", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@Tran_Id", hlu.Tran_Id);
                com.Parameters.AddWithValue("@Serial_No", hlu.Serial_No);
                com.Parameters.AddWithValue("@Issue_Qty", hlu.Issue_Qty);
                com.Parameters.AddWithValue("@Comment", hlu.Comment);


                i = com.ExecuteNonQuery();
            }
            return i;
        }
      
       public int Update_Supplier_Name_ITAssest(AAA_ITAssestCell_MDI hld)
       {
           clsMyClass mc = new clsMyClass();
           string cs = mc.strconn;
           int i;
           using (SqlConnection con = new SqlConnection(cs))
           {
               con.Open();
               SqlCommand com = new SqlCommand("ZZZ_USP_Update_AAA_Supplier_ITAssest", con);
               com.CommandType = CommandType.StoredProcedure;
               com.Parameters.AddWithValue("@Supplier_Id", hld.Supplier_Id);
               com.Parameters.AddWithValue("@Supplier_Name", hld.Supplier_Name);
               i = com.ExecuteNonQuery();
           }
           return i;
       }
//GetList
       public List<AAA_ITAssestCell_MDI> get_ItemMasterList_ITCell()
       {
           DataSet ds = new DataSet();
           SqlCommand cmd = new SqlCommand();
           cmd.CommandType = CommandType.StoredProcedure;
           cmd.Parameters.Clear();
           cmd.CommandText = "ZZZ_Get_Item_Name_By_Category";
           mc.fillFromDatabase(ds, cmd);
           List<AAA_ITAssestCell_MDI> ls = new List<AAA_ITAssestCell_MDI> { };
           foreach (DataRow dr in ds.Tables[0].Rows)
           {
               ls.Add(new AAA_ITAssestCell_MDI
               {
                  // Item_Type_Id = Convert.ToInt32(dr["Item_Type_Id"]),
                   Item_Id = Convert.ToInt32(dr["Item_Id"]),
                   Item_Name = dr["Item_Name"].ToString()
               });
           }
           return ls;
       }
        public List<AAA_ITAssestCell_MDI> get_Item_Type_ITCell()
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "ZZZ_usp_get_ItemType_ITCELL";
            mc.fillFromDatabase(ds, cmd);
            List<AAA_ITAssestCell_MDI> l= new List<AAA_ITAssestCell_MDI> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                l.Add(new AAA_ITAssestCell_MDI
                {
                    Item_Type_Id = Convert.ToInt32(dr["Item_Type_Id"]),
                    Item_Type = dr["Item_Type"].ToString()
                });
            }
            return l;
        }

        public List<AAA_ITAssestCell_MDI> get_Item_Location_ITCell()
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "ZZZ_get_Item_Location_ITCell";
            mc.fillFromDatabase(ds, cmd);
            List<AAA_ITAssestCell_MDI> ls = new List<AAA_ITAssestCell_MDI> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                ls.Add(new AAA_ITAssestCell_MDI
                {
                    compcode = Convert.ToInt32(dr["compcode"]),
                    cmpname = dr["cmpname"].ToString()
                });
            }
            return ls;
        }

        public List<AAA_ITAssestCell_MDI> get_EMPName_To_Issue()
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "ZZZ_Get_EMPName_To_Issue";
            mc.fillFromDatabase(ds, cmd);
            List<AAA_ITAssestCell_MDI> ls = new List<AAA_ITAssestCell_MDI> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                ls.Add(new AAA_ITAssestCell_MDI
                {
                    NewEmpId = Convert.ToInt32(dr["NewEmpId"]),
                    EmpName = dr["EmpName"].ToString()
                });
            }
            return ls;
        }

        public List<AAA_ITAssestCell_MDI> Get_Item_ListITCell()
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "ZZZ_Get_Item_ListITCellCombo";
            mc.fillFromDatabase(ds, cmd);
            List<AAA_ITAssestCell_MDI> ls = new List<AAA_ITAssestCell_MDI> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                ls.Add(new AAA_ITAssestCell_MDI
                {
                    Item_Id = Convert.ToInt32(dr["Item_Id"]),
                    Item_Name = dr["Item_Name"].ToString()
                });
            }
            return ls;
        }

         public List<AAA_ITAssestCell_MDI> Get_ItemStock_ITCell()
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "ZZZ_GetStock_ITCEll";
            mc.fillFromDatabase(ds, cmd);
            List<AAA_ITAssestCell_MDI> ls = new List<AAA_ITAssestCell_MDI> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                ls.Add(new AAA_ITAssestCell_MDI
                {
                    // Item_Type_Id = Convert.ToInt32(dr["Item_Type_Id"]),
                    Item_Id = Convert.ToInt32(dr["Item_Id"]),
                    Stock = Convert.ToInt32(dr["Stock"])
                });
            }
            return ls;
        }

        public int Delete_Item_Type(int Id)
        {
            clsMyClass mc = new clsMyClass();
            string cs = mc.strconn;
            int i;
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                SqlCommand com = new SqlCommand("AAA_Delete_ItemType", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@Item_Type_Id", Id);
                i = com.ExecuteNonQuery();
            }
            return i;
        }

        public int DeleteItemType(int id)
        {
            SqlCommand cmd = new SqlCommand("DELETE FROM AAA_Item_Type WHERE Item_Type_Id = @Item_Type_Id", con)
            {
                CommandType = CommandType.Text
            };
            cmd.Parameters.AddWithValue("@Item_Type_Id", id);
            con.Open();
            int i = cmd.ExecuteNonQuery();
            con.Close();
            return i;
        }

        public int DeleteItemName(int id)
        {

            SqlCommand cmd = new SqlCommand("DELETE FROM AAA_ItemMaster_ITCell WHERE Item_Id = @Item_Id", con)
            {
                CommandType = CommandType.Text
            };
            cmd.Parameters.AddWithValue("@Item_Id", id);
            con.Open();
            int i = cmd.ExecuteNonQuery();
            con.Close();
            return i;
        }

        public int DeleteIssueItem(int id)
        {

            SqlCommand cmd = new SqlCommand("DELETE FROM AAA_Item_StockITCell WHERE Tran_Id = @Tran_Id", con)

            {
                CommandType = CommandType.Text
            };
            cmd.Parameters.AddWithValue("@Tran_Id", id);
            con.Open();
            int i = cmd.ExecuteNonQuery();
            con.Close();
            return i;
        }

        public int DeleteStockItem(int id)
        {

            SqlCommand cmd = new SqlCommand("DELETE FROM AAA_Item_StockITCell WHERE Tran_Id = @Tran_Id", con)
            {
                CommandType = CommandType.Text
            };
            cmd.Parameters.AddWithValue("@Tran_Id", id);
            con.Open();
            int i = cmd.ExecuteNonQuery();
            con.Close();
            return i;
        }

        //Supplier
        public List<AAA_ITAssestCell_MDI> get_Supplier_Name_ItCell()
        {
            string cs = mc.strconn;
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.CommandText = "ZZZ_Get_Supplier_Name_ItCellCombo";
            mc.fillFromDatabase(ds, cmd);
            List<AAA_ITAssestCell_MDI> ls = new List<AAA_ITAssestCell_MDI> { };
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                ls.Add(new AAA_ITAssestCell_MDI
                {
                    Supplier_Id = Convert.ToInt32(dr["Supplier_Id"]),
                    Supplier_Name = dr["Supplier_Name"].ToString()
                });
            }
            return ls;
        }

        public List<AAA_ITAssestCell_MDI> GetSupplier_List()
        {
            string cs = mc.strconn;
            List<AAA_ITAssestCell_MDI> lst = new List<AAA_ITAssestCell_MDI>();
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                SqlCommand com = new SqlCommand("ZZZ_Get_Supplier_Name_ItCellList", con);
                com.CommandType = CommandType.StoredProcedure;
                SqlDataReader rdr = com.ExecuteReader();
                while (rdr.Read())
                {
                    lst.Add(new AAA_ITAssestCell_MDI
                    {
                        Supplier_Id = Convert.ToInt32(rdr["Supplier_Id"]),
                        Supplier_Name = rdr["Supplier_Name"].ToString(),
                        //Age = Convert.ToInt32(rdr["Age"]),
                        //State = rdr["State"].ToString(),
                        //Country = rdr["Country"].ToString(),
                    });
                }
                return lst;
            }
        }

        //Method for Adding an Employee
        public int Add(AAA_ITAssestCell_MDI emp)
        {
            string cs = mc.strconn;
            int i;
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                SqlCommand com = new SqlCommand("ZZZ_InsertUpdateSupplier", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@Supplier_Id", emp.Supplier_Id);
                com.Parameters.AddWithValue("@Supplier_Name", emp.Supplier_Name);
                //com.Parameters.AddWithValue("@Age", emp.Age);
                //com.Parameters.AddWithValue("@State", emp.State);
                //com.Parameters.AddWithValue("@Country", emp.Country);
                //com.Parameters.AddWithValue("@Action", "Insert");
                i = com.ExecuteNonQuery();
            }
            return i;
        }

        //Method for Updating Employee record
        public int Update(AAA_ITAssestCell_MDI emp)
        {
            string cs = mc.strconn;
            int i;
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                SqlCommand com = new SqlCommand("ZZZ_UpdateSupplierIT", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@Supplier_Id", emp.Supplier_Id);
                com.Parameters.AddWithValue("@Supplier_Name", emp.Supplier_Name);
                //com.Parameters.AddWithValue("@Age", emp.Age);
                //com.Parameters.AddWithValue("@State", emp.State);
                //com.Parameters.AddWithValue("@Country", emp.Country);
                // com.Parameters.AddWithValue("@Action", "Update");
                i = com.ExecuteNonQuery();
            }
            return i;
        }

        //Method for Deleting an Employee
        public int Delete(int ID)
        {
            string cs = mc.strconn;
            int i;
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                SqlCommand com = new SqlCommand("ZZZ_DeleteSupplierITAssest", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                com.Parameters.AddWithValue("@Supplier_Id", ID);
                i = com.ExecuteNonQuery();
            }
            return i;
        }




    }
}
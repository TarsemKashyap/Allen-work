// Decompiled with JetBrains decompiler
// Type: skynapse.fbs.DataAccess
// Assembly: fbs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EE45F058-4776-4456-92BB-8311B7676A83
// Assembly location: D:\RBS\rbs\rbs\bin\fbs.dll

using log4net;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace skynapse.fbs
{
  public class DataAccess
  {
    private SqlConnection oConn;
    public SqlDataReader oDataReader;
    private SqlCommand oCommand;
    private string connStr = "";
    public DataSet resultDataSet;
    public string resultString;
    public int sqlErrorID;
    public Exception sqlErrorException;
    public Exception sqlConnectionException;
    private ILog db_log_detail = LogManager.GetLogger(nameof (db_log_detail));
    private ILog db_log_stats = LogManager.GetLogger(nameof (db_log_stats));
    private bool debug;
    private DateTime current_timestamp;

    public DataAccess(string connectionString)
    {
      this.oConn = new SqlConnection();
      this.connStr = connectionString;
      this.debug = false;
      this.current_timestamp = DateTime.Now;
    }

    public DataAccess(string connectionString, bool enable_debug)
    {
      this.oConn = new SqlConnection();
      this.connStr = connectionString;
      this.debug = enable_debug;
      this.current_timestamp = DateTime.Now;
    }

    public bool open_connection()
    {
      bool flag;
      try
      {
        if (this.oConn == null)
        {
          this.oConn = new SqlConnection();
          this.oConn.ConnectionString = this.connStr;
          this.oConn.Open();
          return true;
        }
        if (this.oConn.State == ConnectionState.Open)
        {
          flag = true;
        }
        else
        {
          this.oConn.ConnectionString = this.connStr;
          this.oConn.Open();
          flag = true;
        }
      }
      catch (Exception ex)
      {
        this.db_log_detail.Error((object) ex.ToString());
        this.sqlConnectionException = ex;
        flag = false;
      }
      return flag;
    }

    public bool close_connection()
    {
      try
      {
        this.oConn.Close();
        this.oConn.Dispose();
        return true;
      }
      catch (Exception ex)
      {
        this.db_log_detail.Error((object) ex.ToString());
        this.sqlConnectionException = ex;
        return false;
      }
      finally
      {
        this.oConn.Dispose();
        this.oConn = (SqlConnection) null;
      }
    }

    public bool get_dataset(string Sql)
    {
      SqlConnection sqlConnection = new SqlConnection(this.connStr);
      sqlConnection.InfoMessage += new SqlInfoMessageEventHandler(this.server_info_message);
      sqlConnection.StatisticsEnabled = true;
      bool dataset;
      try
      {
        sqlConnection.Open();
        dataset = true;
      }
      catch (Exception ex)
      {
        this.db_log_detail.Error((object) ("DB Connection Error: " + ex.ToString()));
        dataset = false;
      }
      if (dataset)
      {
        try
        {
          SqlDataAdapter sqlDataAdapter1 = new SqlDataAdapter();
          SqlDataAdapter sqlDataAdapter2 = new SqlDataAdapter(Sql, sqlConnection);
          this.resultDataSet = new DataSet();
          sqlDataAdapter2.Fill(this.resultDataSet);
          dataset = true;
          sqlDataAdapter2.Dispose();
        }
        catch (Exception ex)
        {
          this.db_log_detail.Error((object) Sql, ex);
          this.sqlErrorException = ex;
          dataset = false;
        }
        finally
        {
          this.write_stats(sqlConnection, Sql);
          sqlConnection.Close();
          sqlConnection.Dispose();
        }
      }
      return dataset;
    }

    private Dictionary<int, object[]> get_reader_objects(SqlDataReader reader)
    {
      Dictionary<int, object[]> readerObjects = new Dictionary<int, object[]>();
      if (reader.HasRows)
      {
        int key = 0;
        while (reader.Read())
        {
          object[] values = new object[reader.FieldCount];
          reader.GetValues(values);
          readerObjects.Add(key, values);
          ++key;
        }
      }
      return readerObjects;
    }

    public Dictionary<int, object[]> get_data_objects(string Sql)
    {
      Dictionary<int, object[]> dataObjects = new Dictionary<int, object[]>();
      SqlConnection sqlConnection = new SqlConnection(this.connStr);
      sqlConnection.InfoMessage += new SqlInfoMessageEventHandler(this.server_info_message);
      sqlConnection.StatisticsEnabled = true;
      SqlCommand sqlCommand = new SqlCommand(Sql, sqlConnection);
      bool flag1;
      try
      {
        sqlConnection.Open();
        flag1 = true;
      }
      catch (Exception ex)
      {
        this.db_log_detail.Error((object) Sql, ex);
        flag1 = false;
      }
      bool flag2;
      if (flag1)
      {
        try
        {
          dataObjects = this.get_reader_objects(sqlCommand.ExecuteReader());
          flag2 = true;
        }
        catch (Exception ex)
        {
          this.db_log_detail.Error((object) Sql, ex);
          this.sqlErrorException = ex;
          flag2 = false;
        }
        finally
        {
          sqlCommand.Dispose();
          this.write_stats(sqlConnection, Sql);
          sqlConnection.Close();
          sqlConnection.Dispose();
        }
      }
      return dataObjects;
    }

    public bool execute_command()
    {
      SqlConnection conn = new SqlConnection(this.connStr);
      conn.InfoMessage += new SqlInfoMessageEventHandler(this.server_info_message);
      conn.StatisticsEnabled = true;
      bool flag;
      try
      {
        conn.Open();
        flag = true;
      }
      catch (Exception ex)
      {
        this.db_log_detail.Error((object) ("DB Connection Error: " + ex.ToString()));
        flag = false;
      }
      if (flag)
      {
        try
        {
          SqlParameter sqlParameter1 = new SqlParameter("@Result", SqlDbType.BigInt, 100);
          sqlParameter1.Direction = ParameterDirection.Output;
          this.oCommand.Parameters.Add(sqlParameter1);
          SqlParameter sqlParameter2 = new SqlParameter("@SQLErrorID", SqlDbType.Int, 100);
          sqlParameter2.Direction = ParameterDirection.Output;
          this.oCommand.Parameters.Add(sqlParameter2);
          SqlParameter sqlParameter3 = new SqlParameter("@SQLError", SqlDbType.NVarChar, 500);
          sqlParameter3.Direction = ParameterDirection.Output;
          this.oCommand.Parameters.Add(sqlParameter3);
          this.oCommand.Connection = conn;
          this.resultString = this.oCommand.ExecuteNonQuery().ToString();
          this.sqlErrorID = Convert.ToInt32(this.oCommand.Parameters["@SQLErrorID"].Value);
          this.sqlErrorException = new Exception(this.oCommand.Parameters["@SQLError"].Value.ToString());
          this.resultString = this.oCommand.Parameters["@Result"].Value.ToString();
          flag = true;
        }
        catch (Exception ex)
        {
          this.db_log_detail.Error((object) ex);
          this.sqlErrorException = ex;
          flag = false;
        }
        finally
        {
          this.oCommand.Dispose();
          this.write_stats(conn, this.oCommand.CommandText);
          conn.Close();
          conn.Dispose();
        }
      }
      return flag;
    }

    public bool execute_command(SqlCommand oCom)
    {
      bool flag = this.open_connection();
      if (flag)
      {
        try
        {
          oCom.Connection = this.oConn;
          this.resultString = oCom.ExecuteNonQuery().ToString();
          flag = true;
        }
        catch (Exception ex)
        {
          this.db_log_detail.Error((object) ex);
          this.sqlErrorException = ex;
          flag = false;
        }
        finally
        {
          oCom.Dispose();
          this.close_connection();
        }
      }
      return flag;
    }

    public bool get_nonquery(string Sql)
    {
      bool nonquery = false;
      try
      {
        SqlCommand sqlCommand = new SqlCommand();
        nonquery = this.execute_command(new SqlCommand(Sql, this.oConn));
      }
      catch (Exception ex)
      {
        this.db_log_detail.Error((object) Sql, ex);
      }
      return nonquery;
    }

    public bool execute_procedure(string storedProcedure, Dictionary<string, object> parameters)
    {
      try
      {
        this.oCommand = new SqlCommand();
        this.oCommand.CommandType = CommandType.StoredProcedure;
        this.oCommand.CommandText = storedProcedure;
        foreach (string key in parameters.Keys)
          this.oCommand.Parameters.Add(new SqlParameter(key, parameters[key]));
        return this.execute_command();
      }
      catch (Exception ex)
      {
        this.db_log_detail.Error((object) ex);
        this.sqlErrorException = ex;
        return false;
      }
    }

    public object execute_scalar(string sql)
    {
      object obj = (object) null;
      SqlConnection sqlConnection = new SqlConnection(this.connStr);
      sqlConnection.InfoMessage += new SqlInfoMessageEventHandler(this.server_info_message);
      sqlConnection.StatisticsEnabled = true;
      bool flag;
      try
      {
        sqlConnection.Open();
        flag = true;
      }
      catch (Exception ex)
      {
        this.db_log_detail.Error((object) ("DB Connection Error: " + ex.ToString()));
        flag = false;
      }
      if (flag)
      {
        try
        {
          SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection);
          obj = sqlCommand.ExecuteScalar();
          sqlCommand.Dispose();
        }
        catch (Exception ex)
        {
          this.db_log_detail.Error((object) sql, ex);
          this.sqlErrorException = ex;
        }
        finally
        {
          this.write_stats(sqlConnection, sql);
          sqlConnection.Close();
          sqlConnection.Dispose();
        }
      }
      return obj;
    }

    public string ToString(object value) => value.ToString() == string.Empty ? "" : (string) value;

    public bool execute_bulk(string[] sqls)
    {
      StringBuilder stringBuilder = new StringBuilder();
      foreach (string sql in sqls)
      {
        stringBuilder.Append(sql);
        stringBuilder.Append(";");
      }
      return this.get_nonquery(stringBuilder.ToString());
    }

    protected void server_info_message(object sender, SqlInfoMessageEventArgs args)
    {
      foreach (SqlError error in args.Errors)
        this.db_log_detail.Info((object) ("SRC: " + error.Source + ", CLS: " + (object) error.Class + ", STATE: " + (object) error.State + ", NUM: " + (object) error.Number + ", LINE NO: " + (object) error.LineNumber + ", PROC: " + error.Procedure + ", SERVER: " + error.Server + ", MSG: " + error.Message));
    }

    protected void write_stats(SqlConnection conn, string sql)
    {
      StringBuilder stringBuilder = new StringBuilder();
      IDictionary dictionary = conn.RetrieveStatistics();
      stringBuilder.Append("<entry>");
      stringBuilder.Append("<timestamp>" + this.current_timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff") + "</timestamp>");
      stringBuilder.Append("<conn_id>" + sql + "</conn_id>");
      stringBuilder.Append("<workstation>" + conn.WorkstationId + "</workstation>");
      foreach (DictionaryEntry dictionaryEntry in dictionary)
      {
        stringBuilder.Append("<" + dictionaryEntry.Key + ">");
        stringBuilder.Append(dictionaryEntry.Value);
        stringBuilder.Append("</" + dictionaryEntry.Key + ">");
      }
      stringBuilder.Append("</entry>");
      this.db_log_stats.Info((object) stringBuilder.ToString());
    }
  }
}

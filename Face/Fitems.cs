//sabi Class like this
using Finisar.SQLite;
using System.Data;
using System.IO;
using System.Collections.Generic;
using System;
public static class Fitems
{
    private static int camx, camy, camn = 0,faceT = 5,faceR = 100,userid=0;
    private static string loc,username;
    private static bool faceRb = false, faceAb = false,boot = true,ineat = false;
    //sabi set{} and get{}
    public static bool init { get{return ineat;} set{ineat = value;} }
    public static int CamX { get { return camx; } set { camx = value; } }
    public static int CamY { get { return camy; } set { camy = value; } }
    public static int cameraN { get { return camn; } set { camn = value; } }
    public static int facethreshold { get { return faceT; } set { faceT = value; } }
    public static int faceWidth { get { return faceR; } set { faceR = value; } }
    public static bool facerotationB { get { return faceRb; } set { faceRb = value; } }
    public static bool faceAngle { get { return faceAb; } set { faceAb = value; } }
    public static bool Boot { get { return boot; } set { boot = value; } }
    public static string location { get { return loc; } set { loc = value; } }
    public static System.Drawing.Image uimg = null;
    public static System.Drawing.Image intruder_img = null;
    public static string user_name { get { return username; } set { username = value; } }
    public static int user_id { get { return userid; } set { userid = value; } }
   
    
    public static bool insert_vals(string name,string email,byte[] face)
    {
        SQLiteConnection con = new SQLiteConnection("Data Source=DB.db;Version=3;");
        SQLiteCommand com = new SQLiteCommand("INSERT INTO 'users' ('fname','email','face','date') VALUES (@name,@email,@face,@date)",con);
        string l = System.DateTime.Now.ToFileTimeUtc().ToString();
        try
        {
            con.Open();
            com.Parameters.Add("@name", DbType.String, name.Length).Value = name;
            com.Parameters.Add("@email", DbType.String, email.Length).Value = email;
            com.Parameters.Add("@face", DbType.Binary, face.Length).Value = face;
            com.Parameters.Add("@date", DbType.String, l.Length).Value = l; 
            com.ExecuteNonQuery();
            con.Close();
            return true;
        }
        catch (SQLiteException sql)
        {
          //  System.Windows.Forms.MessageBox.Show(sql.Message + "\n" + sql.StackTrace);
            return false;
        }
    }


    private static void createDB()
    {
        SQLiteConnection con = new SQLiteConnection("Data Source=DB.db;Version=3;New=True;");
        SQLiteCommand com = new SQLiteCommand("CREATE TABLE users(id INTEGER PRIMARY KEY ,fname TEXT(50),email TEXT(30),face BLOB(17000), date TEXT(30))",con);
        try
        {
            con.Open();
            com.ExecuteNonQuery();
            con.Close();
        }
        catch (SQLiteException sql)
        {
            System.Windows.Forms.MessageBox.Show(sql.Message + "\n" + sql.StackTrace);
        }
    }
    private static void createLDB()
    {
        SQLiteConnection con = new SQLiteConnection("Data Source=DB.db;Version=3;");
        SQLiteCommand com = new SQLiteCommand("CREATE TABLE log (id INTEGER PRIMARY KEY,uid INTEGER,date TEXT(30))", con);
        try
        {
            con.Open();
            com.ExecuteNonQuery();
            con.Close();
        }
        catch (SQLiteException sql)
        {
            System.Windows.Forms.MessageBox.Show(sql.Message + "\n" + sql.StackTrace);
        }
    }
    public static bool insert_log(int uid)
    {
        SQLiteConnection con = new SQLiteConnection("Data Source=DB.db;Version=3;");
        SQLiteCommand com = new SQLiteCommand("INSERT INTO 'log' ('uid','date') VALUES (@uid,@date)", con);
        string l = System.DateTime.Now.ToFileTimeUtc().ToString();
        try
        {
            con.Open();           
            com.Parameters.Add("@uid", DbType.Int32, uid.ToString().Length).Value = uid;
            com.Parameters.Add("@date", DbType.String, l.Length).Value = l;
            com.ExecuteNonQuery();
            con.Close();
            return true;
        }
        catch (SQLiteException sql)
        {
            System.Windows.Forms.MessageBox.Show(sql.Message + "\n" + sql.StackTrace);
            return false;
        }
    }
    
    public static bool deleteUser(string name)
    {
        bool deleted = false;
        SQLiteConnection con = new SQLiteConnection("Data Source=DB.db;Version=3;");
        SQLiteCommand com = new SQLiteCommand("DELETE FROM users WHERE fname = '" + name + "'", con);
        string l = System.DateTime.Now.ToFileTimeUtc().ToString();
        try
        {
            con.Open();
            com.ExecuteNonQuery();
            deleted = true;
        }
        catch (SQLiteException sql)
        {
            deleted = false;
        }
        return deleted;
    }
    public static List<string> get_user_log_vars(int uid,string fname)
    {

        SQLiteConnection con = new SQLiteConnection("Data Source=DB.db;Version=3;");
        SQLiteCommand com = new SQLiteCommand();
        try
        {
            com.Connection = con;
            con.Open();
            List<string> vars = new List<string>();
            //List<long> dateUTC = new List<long>();
            com.CommandText = "SELECT date FROM log WHERE uid = '" + uid + "'";
            SQLiteDataReader reader = com.ExecuteReader();
            while (reader.Read())
            {

                long l = 0L;
                long.TryParse(reader.GetValue(0).ToString(), out l);
                DateTime then = DateTime.FromFileTimeUtc(l);
                vars.Add(then.ToLongDateString() + " " + then.ToShortTimeString());
            }
            reader.Close();
            con.Close();
            return vars;
        }
        catch (SQLiteException sql)
        {
            System.Windows.Forms.MessageBox.Show(sql.Message + "\n" + sql.StackTrace);
        }
        return null;
    }
    public static List<string> get_log_vars()
    {

        SQLiteConnection con = new SQLiteConnection("Data Source=DB.db;Version=3;");
        SQLiteCommand com = new SQLiteCommand();
        try
        {
            com.Connection = con;
            con.Open();
            List<string> vars = new List<string>();
            com.CommandText = "SELECT users.fname, log.date FROM log INNER JOIN users ON log.uid = users.id";
            SQLiteDataReader reader = com.ExecuteReader();
            while (reader.Read())
            {   
                long l = 0L;
                long.TryParse(reader.GetValue(1).ToString(), out l);
                DateTime then = DateTime.FromFileTimeUtc(l);
                vars.Add(reader.GetValue(0).ToString() + " logged in at: " + then.ToLongDateString() +" "+ then.ToShortTimeString());   
            }
            reader.Close();
            con.Close();
            return vars;
        }
        catch (SQLiteException sql)
        {
            System.Windows.Forms.MessageBox.Show(sql.Message + "\n" + sql.StackTrace);
        }
        return null;
    }
    public static object[] get_db_vars()
    {
        if (!File.Exists("DB.db"))
        {
            createDB();
            createLDB();
        }
        object[] obj = null;
        SQLiteConnection con = new SQLiteConnection("Data Source=DB.db;Version=3;");
        SQLiteCommand com = new SQLiteCommand();
        try
        {
            com.Connection = con;
            con.Open();
            List<string> names = new List<string>();
            List<string> email = new List<string>();
            List<long> dateUTC = new List<long>();
            List<int> _userid = new List<int>();
            List<byte[]> face = new List<byte[]>();
            com.CommandText = "SELECT fname,email,face,date,id FROM users";
            SQLiteDataReader reader = com.ExecuteReader();
            while (reader.Read())
            {
                names.Add(reader.GetValue(0).ToString());
                email.Add(reader.GetValue(1).ToString());
                _userid.Add(reader.GetInt32(4));
                face.Add((byte[])reader.GetValue(2));
                long l = 0L;
                long.TryParse(reader.GetValue(3).ToString(),out l);
                dateUTC.Add(l);
            }
            reader.Close();
            con.Close();
            obj = new object[] {names,email,face,dateUTC,_userid }; 
        }
        catch (SQLiteException sql)
        {
            System.Windows.Forms.MessageBox.Show(sql.Message + "\n"+sql.StackTrace);
        }
        return obj;
    }
    
}
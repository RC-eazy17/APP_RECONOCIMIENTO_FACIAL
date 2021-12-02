using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reconocimiento_facial
{
    public class DBCon
    {
        private OleDbConnection cnx;

        public string[] Nombre;

        private byte[] cara;

        public List<byte[]> Cara = new List<byte[]>();

        public int TotalUsuario;

        public DBCon()
        {
            cnx = new OleDbConnection("Provider = Microsoft.Jet.OLEDB.4.0; Data Source = UsersFace.mdb");

            cnx.Open();
        }
        public bool GuardarImagen(string Name, string Code, string Peso, string Altura, string Equipo,string Fecha,byte[] abImagen)
        {
            cnx.Open();

            OleDbCommand comm = new OleDbCommand("INSERT INTO UserFaces (Name,Code,Peso,Altura,Equipo,Fecha,Face) VALUES ('" + Name + "','" + Code + "','" + Peso + "','" + Altura + "','" + Equipo + "','" + Fecha + "',?)", cnx); 
            
            OleDbParameter parImagen = new OleDbParameter("@Face", OleDbType.VarBinary, abImagen.Length);

            parImagen.Value = abImagen;

            comm.Parameters.Add(parImagen);   
            
            int iResultado = comm.ExecuteNonQuery();

            cnx.Close();

            return Convert.ToBoolean(iResultado);
        }

        public DataTable ObtenerBytesImagen()
        {
            string sql = "SELECT IdImage,Name,Code,Altura,Peso,Equipo,Fecha,Face FROM UserFaces";
            OleDbDataAdapter adaptador = new OleDbDataAdapter(sql, cnx);
            DataTable dt = new DataTable();
            adaptador.Fill(dt);
            int cont = dt.Rows.Count;
            Nombre = new string[cont];

            for (int i = 0; i < cont; i++)
            {
                Nombre[i] = dt.Rows[i]["Name"].ToString();
                cara = (byte[])dt.Rows[i]["Face"];
                Cara.Add(cara);
            }
            TotalUsuario = dt.Rows.Count;
            cnx.Close();
            return dt;
        }

        public void ConvertImgToBinary(string Nombre, string Codigo, string Peso, string Altura, string Equipo,string Fecha,Image Img)
        {
            Bitmap bmp = new Bitmap(Img);

            MemoryStream MyStream = new MemoryStream();

            bmp.Save(MyStream, System.Drawing.Imaging.ImageFormat.Bmp);

            byte[] aImagen = MyStream.ToArray();

            GuardarImagen(Nombre, Codigo, Peso,Altura,Equipo,Fecha,aImagen);
        }

        public Image ConvertByteToImg( int con)
        {
            Image FetImg;

            byte[] img = Cara[con];

            MemoryStream ms = new MemoryStream(img);

            FetImg = Image.FromStream(ms);

            ms.Close();

            return FetImg;

        }
    }
}

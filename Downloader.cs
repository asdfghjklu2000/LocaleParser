using System;
using System.IO;
using System.Net;


namespace LocaleParser
{
    public class Downloader
    {

        public static Boolean Dl_file(string param_url, string param_filename)
        {
            string url = "";
            string filename;

            url = param_url;
            filename = param_filename;

            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(url);
            httpRequest.Timeout = 2000; //5秒timeout

            try
            {
                HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse(); //error



                System.IO.Stream dataStream = httpResponse.GetResponseStream();

                byte[] buffer = new byte[8192];



                FileStream fs = new FileStream(filename,

                    FileMode.Create, FileAccess.Write);

                int size = 0;

                do
                {

                    size = dataStream.Read(buffer, 0, buffer.Length);

                    if (size > 0)

                        fs.Write(buffer, 0, size);

                } while (size > 0);

                fs.Close();



                httpResponse.Close();
                return true;
            }
            catch (Exception e)
            {
                //位置不存在
            }

            return false;
        }

    }
}
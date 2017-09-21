using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using WebsiteGetter.Catch;
using System.Runtime.InteropServices;
using System.Drawing;

namespace WebsiteGetter.Output
{
    class OutputController
    {
        public string savePath;
        public bool isSaveOneTxt;
        public bool isNotSmallImage;

        private static object filewrite = "writelock";

        public OutputController()
        {
            savePath = "/tmp";
            isSaveOneTxt = false;
            isNotSmallImage = false;
        }

        /// <summary>
        /// 将字符串整理成本地路径格式
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private string getPath(string str)
        {
            if (str.Length <= 0) return "\\";
            if (str[str.Length - 1] == '\\') return str;
            if (str.Split('.').Length <= 1) return str + "\\";
            if (str.Split('\\').Length <= 0) return "\\";
            return str.Split('\\')[str.Split('\\').Length - 1] + "\\";
        }

        /// <summary>
        /// 获取字符串中的文件后缀名
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private string getExt(string name)
        {
            if (name.Split('.').Length <= 0) return "";
            return name.Split('.')[name.Split('.').Length - 1];
        }

        public string cleanHtmlText(string res)
        {
            string str = "";
            str = res.Replace("<br>", "\r\n");
            string[] deleteList ={
                                    "<b>","</b>","<span>","</span>","<pre>","</pre>","<p>","</p>"
                                };
            foreach (var d in deleteList)
            {
                str.Replace(d, "");
            }
            return str;
        }

        public string formatOutputText(Dictionary<string, string> res)
        {
            string str = "";
            foreach (var a in res)
            {
                if (a.Key != "all")
                {
                    str += string.Format("{0}:\r\n{1}\r\n", a.Key, cleanHtmlText(a.Value));
                }
            }
            return str;
        }

        public void output(string title,string content)
        {
            if (isSaveOneTxt)
            {
                //one file
                title = DateTime.Today.ToShortDateString();
            }
            else
            {
                if (title.Length <= 0) title = "unname";
            }
            title = CatchController.CleanInvalidFileName(title);

            saveTxt(title, content, isSaveOneTxt);
        }



        [DllImport("kernel32.dll")]
        private static extern IntPtr _lopen(string lpPathName, int iReadWrite);
        [DllImport("kernel32.dll")]
        private static extern bool CloseHandle(IntPtr hObject);
        private const int OF_READWRITE = 2;
        private const int OF_SHARE_DENY_NONE = 0x40;
        private static readonly IntPtr HFILE_ERROR = new IntPtr(-1);
        public static int FileIsOpen(string fileFullName)
        {
            if (!File.Exists(fileFullName))
            {
                return -1;
            }
            IntPtr handle = _lopen(fileFullName, OF_READWRITE | OF_SHARE_DENY_NONE);
            if (handle == HFILE_ERROR)
            {
                return 1;
            }
            CloseHandle(handle);
            return 0;
        }

        public void saveTxt(string filename, string content, bool openOld = true)
        {
            //string tmpname = "";
            //string[] tmpstr = res.Substring(0, res.Length > 30 ? 30 : res.Length).Replace("\\", "").Replace(" ", "").Replace(".", "").Replace("\r", "").Split('\n');
            //if (tmpstr.Length >= 0 && tmpstr[0].Length > 0) tmpname = tmpstr[0];
            //else if (tmpstr.Length >= 1 && tmpstr[1].Length > 0) tmpname = tmpstr[1];
            //else tmpname = "none";
            //if (!Directory.Exists(savePath)) Directory.CreateDirectory(savePath);
            //if (title.Length <= 0) title = tmpname;

            //name = savePath + noStr + "_" + title + ".txt";

            string filepath = string.Format("{0}\\{1}.txt", savePath, filename);
            try
            {
                FileStream s;
                if (openOld)
                {
                    
                    //while (FileIsOpen(filepath) == 1) ;
                    lock (filewrite)
                    {
                        if (!File.Exists(filepath))
                        {
                            using (FileStream fs = new FileStream(filepath, FileMode.Create))
                            {
                                using (StreamWriter sw = new StreamWriter(fs, Encoding.Default))
                                {
                                    sw.WriteLine(content);
                                }
                            }
                        }
                        else
                        {
                            using (FileStream fs = new FileStream(filepath, FileMode.Append))
                            {
                                using (StreamWriter sw = new StreamWriter(fs, Encoding.Default))
                                {
                                    sw.WriteLine(content);
                                }
                            }
                        }

                    }
                   
                }
                else
                {
                    using (FileStream fs = new FileStream(filepath, FileMode.Create))
                    {
                        using (StreamWriter sw = new StreamWriter(fs, Encoding.Default))
                        {
                            sw.WriteLine(content);
                        }
                    }
                }

            }
            catch (Exception)
            {
                ;
            }
        }

        public  void savePdf(string filename,string content)
        {
            string filepath = string.Format("{0}\\{1}.pdf", savePath, filename);
            //WebConnection.saveFile(content, sessionidName + "=" + sessionidValue, filepath);
        }

        public void saveJpg(string filename, Image img)
        {
            string filepath = string.Format("{0}\\{1}.jpg", savePath, filename);
            img.Save(filepath);
            //WebConnection.saveFile(content, sessionidName + "=" + sessionidValue, filepath);
        }


        ///// <summary>
        ///// 保存文字信息
        ///// </summary>
        ///// <param name="res"></param>
        //private void saveInfo(string res, string noStr)
        //{
        //    string name = "";
        //    if (saveState == 0)
        //    {

        //    }
        //    else if (saveState == 1)
        //    {
        //        //多线程读同个文件会发生冲突，故采用委托
        //        Invoke(printToFileEvent, res);
        //    }
        //    Invoke(printEvent, (object)"save:" + name);
        //}
    }
}

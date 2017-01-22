using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebsiteGetter.Catch;
using System.Web;
using System.Net;
using System.IO;


namespace WebsiteGetter.Catch
{
    class CatchController
    {
        public int maxNum;
        public int nowNum;
        public string nowStr;
        public AddState addState;
        public EncodingState encoding;

        public string url1;
        public string url2;
        public string url3;

        //public bool useCookies;
        public string cookies;


        public CatchController()
        {
            //addState = AddState.num_staticLength;
            url1 = "";
            url2 = "";
            url3 = "";
            cookies = "";
            nowNum = 0;
            maxNum = -1;
            nowStr = "";
            encoding = EncodingState.utf8;
        }

        private Encoding getEncoding()
        {
            if (this.encoding == EncodingState.utf8) return Encoding.UTF8;
            else if (this.encoding == EncodingState.gbk) return Encoding.GetEncoding("gbk");
            else return Encoding.Default;
        }

        public string getNowUrl()
        {
            string url = url1 + nowStr + url3;
            return url;
        }

        /// <summary>
        /// 获取下一个要扫描的号码
        /// </summary>
        /// <returns></returns>
        private void getNext()
        {
            if (nowNum == 0)
            {
                //init
                nowStr = url2;
            }
            else
            {
                string beforeStr = nowStr;
                WordsIncrement incStr = new WordsIncrement();
                switch (addState)
                {
                    case AddState.num_buaaSid:
                        nowStr= NumberGetter.nextId(beforeStr);
                        break;
                    case AddState.num_staticLength:
                        //添0的自增
                        nowStr = NumberGetter.add0(incStr.getNext(beforeStr), beforeStr.Length);
                        break;
                    case AddState.num_dynamicLength:
                        //纯数字自增
                        nowStr = incStr.getNext(beforeStr);
                        break;
                    case AddState.num_underline:
                        //中间带有下划线，后一部分1-30自增，前一部分纯数字自增
                        string[] tmp = beforeStr.Split('_');
                        int num1, num2;
                        Int32.TryParse(tmp[0], out num1);
                        Int32.TryParse(tmp[1], out num2);
                        num2++;
                        if (num2 > 30)
                        {
                            num1++;
                            num2 = 1;
                        }
                        nowStr = num1 + "_" + num2;
                        break;
                    default:
                        break;
                }
            }
        }


        public string catchHtml()
        {
            getNext();
            if (nowNum != maxNum)
            {
                string html = "";
                if (string.IsNullOrEmpty(this.cookies))
                {
                    html = WebConnection.getData(getNowUrl(),getEncoding());
                }
                else
                {
                    html = WebConnection.getDataWithCookie(getNowUrl(), cookies, getEncoding());
                }
                nowNum += 1;
                return html;
            }
            else
            {
                return "";
            }
        }

        public void saveFile(string name, string path)
        {
            WebClient client = new WebClient();
            client.DownloadFile(path.Replace('\\', '/'), name);
        }

        /// <summary>
        /// 文件名非法字符
        /// </summary>
        private static readonly char[] InvalidFileNameChars = new[]
        {
            '"',
            '<',
            '>',
            '|',
            '\0',
            '\u0001',
            '\u0002',
            '\u0003',
            '\u0004',
            '\u0005',
            '\u0006',
            '\a',
            '\b',
            '\t',
            '\n',
            '\v',
            '\f',
            '\r',
            '\u000e',
            '\u000f',
            '\u0010',
            '\u0011',
            '\u0012',
            '\u0013',
            '\u0014',
            '\u0015',
            '\u0016',
            '\u0017',
            '\u0018',
            '\u0019',
            '\u001a',
            '\u001b',
            '\u001c',
            '\u001d',
            '\u001e',
            '\u001f',
            ':',
            '*',
            '?',
            '\\',
            '/'
        };

        public static string CleanInvalidFileName(string fileName)
        {
            fileName = fileName + "";
            fileName = InvalidFileNameChars.Aggregate(fileName, (current, c) => current.Replace(c + "", ""));
            if (fileName.Length > 1)
                if (fileName[0] == '.')
                    fileName = "dot" + fileName.TrimStart('.');
            if (fileName.Length > 20)
            {
                fileName = string.Format("{0}_{1}", fileName.Substring(0, 10), fileName.Substring(fileName.Length - 10, 10));
            }
            return fileName;
        }

        public static string getImagePathExt(string path)
        {
            string[] allows = { ".jpeg", ".jpg", ".png", ".gif" };
            string ext = ".jpg";
            foreach (var a in allows)
            {
                if (path.Contains(a)) 
                { 
                    ext = a; 
                    break; 
                }
            }
            return ext;
        }

        private static string rebuildUrl(string url)
        {
            return url.Replace("\\/", "/");
        }

        /// <summary>
        /// 保存图片
        /// </summary>
        /// <param name="imgPath"></param>
        /// <param name="dirPath"></param>
        /// <param name="imgTitle"></param>
        public void saveImg(List<string> imgPath, string dirPath,string imgTitle)
        {
            imgTitle = CleanInvalidFileName(imgTitle);
            for (int i = 0; i < imgPath.Count; i++)
            {
                //Invoke(printEvent, (object)imgPath[i]);
                string path = rebuildUrl(imgPath[i]);
                string name = string.Format("{0}/{1}_{2}{3}", 
                    dirPath ,
                    imgTitle,
                    NumberGetter.add0(i.ToString(),imgPath.Count.ToString().Length),
                    getImagePathExt(path)
                    );
                Directory.CreateDirectory(dirPath);

                WebClient imgClient = new WebClient();
                try
                {
                    Stream imgStream = imgClient.OpenRead(path);
                    BinaryReader r = new BinaryReader(imgStream);
                    byte[] mbyte = new byte[100000000];
                    int allmybyte = (int)mbyte.Length;
                    int startmbyte = 0;
                    while (allmybyte > 0)
                    {
                        int m = r.Read(mbyte, startmbyte, allmybyte);
                        if (m == 0)
                            break;

                        startmbyte += m;
                        allmybyte -= m;
                    }

                    if (startmbyte < 51200)
                    {
                        //Invoke(printEvent, (object)("img too small:" + startmbyte + "bytes"));
                        //continue;
                    }

                    imgStream.Dispose();
                    FileStream img = new FileStream(name, FileMode.Create, FileAccess.Write);
                    img.Write(mbyte, 0, startmbyte);
                    img.Flush();
                    img.Close();
                    //Invoke(printEvent, (object)"save:" + name);
                    imgStream.Close();
                    imgClient.Dispose();
                }
                catch (Exception e)
                {
                    //Invoke(printEvent, (object)"exception:" + e.Message);
                    continue;
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mozilla.NUniversalCharDet;
using System.Threading;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.IO;
using WebsiteGetter.Catch;
using WebsiteGetter.Analysis;

namespace WebsiteGetter.Output
{
    class JsonController
    {
        /// <summary>
        /// 返回流的编码格式
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private static Encoding getEncoding(string streamName)
        {
            Encoding encoding = Encoding.Default;
            using (Stream stream = new FileStream(streamName, FileMode.Open))
            {
                MemoryStream msTemp = new MemoryStream();
                int len = 0;
                byte[] buff = new byte[512];
                while ((len = stream.Read(buff, 0, 512)) > 0)
                {
                    msTemp.Write(buff, 0, len);
                }
                if (msTemp.Length > 0)
                {
                    msTemp.Seek(0, SeekOrigin.Begin);
                    byte[] PageBytes = new byte[msTemp.Length];
                    msTemp.Read(PageBytes, 0, PageBytes.Length);
                    msTemp.Seek(0, SeekOrigin.Begin);
                    int DetLen = 0;
                    UniversalDetector Det = new UniversalDetector(null);
                    byte[] DetectBuff = new byte[4096];
                    while ((DetLen = msTemp.Read(DetectBuff, 0, DetectBuff.Length)) > 0 && !Det.IsDone())
                    {
                        Det.HandleData(DetectBuff, 0, DetectBuff.Length);
                    }
                    Det.DataEnd();
                    if (Det.GetDetectedCharset() != null)
                    {
                        encoding = Encoding.GetEncoding(Det.GetDetectedCharset());
                    }
                }
                msTemp.Close();
                msTemp.Dispose();
                return encoding;
            }
        }

        /// <summary>
        /// 解析JSON为configInfo对象
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static void getConfigInfoFromJson(string fileName, CatchController cc)
        {
            Encoding encoding = getEncoding(fileName);
            using (FileStream file = new FileStream(fileName, FileMode.Open))
            {
                StreamReader reader = new StreamReader(file, encoding);
                JsonSerializer serializer = new JsonSerializer();
                StringReader sr = new StringReader(reader.ReadToEnd());
                object info = serializer.Deserialize(new JsonTextReader(sr), typeof(CatchController));
                CatchController info1= info as CatchController;
                cc.cookies = info1.cookies;
                cc.addState = info1.addState;
                cc.url1 = info1.url1;
                cc.url2 = info1.url2;
                cc.url3 = info1.url3;
                cc.encoding = info1.encoding;
            }
        }

        /// <summary>
        /// 将对象存储为json文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="configInfo"></param>
        public static void saveConfigAsJson(string fileName, CatchController cc)
        {
            string saveJsonString = JsonConvert.SerializeObject(cc);
            using (FileStream file = new FileStream(fileName, FileMode.Create))
            {
                StreamWriter writer = new StreamWriter(file, Encoding.UTF8);
                writer.Write(saveJsonString);
                writer.Close();
            }
        }
        /// <summary>
        /// 解析JSON为configInfo对象
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static void getOutputConfigInfoFromJson(string fileName, OutputController oc)
        {
            Encoding encoding = getEncoding(fileName);
            using (FileStream file = new FileStream(fileName, FileMode.Open))
            {
                StreamReader reader = new StreamReader(file, encoding);
                JsonSerializer serializer = new JsonSerializer();
                StringReader sr = new StringReader(reader.ReadToEnd());
                object info = serializer.Deserialize(new JsonTextReader(sr), typeof(OutputController));
                OutputController info1 = info as OutputController;
                oc.isNotSmallImage = info1.isNotSmallImage;
                oc.isSaveOneTxt = info1.isSaveOneTxt;
                oc.savePath = info1.savePath;
            }
        }

        /// <summary>
        /// 将对象存储为json文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="configInfo"></param>
        public static void saveOutputConfigAsJson(string fileName, OutputController oc)
        {
            string saveJsonString = JsonConvert.SerializeObject(oc);
            using (FileStream file = new FileStream(fileName, FileMode.Create))
            {
                StreamWriter writer = new StreamWriter(file, Encoding.UTF8);
                writer.Write(saveJsonString);
                writer.Close();
            }
        }




        /// <summary>
        /// 解析JSON为configInfo对象
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static void getAnalysisConfigInfoFromJson(string fileName, AnalysisController ac)
        {
            Encoding encoding = getEncoding(fileName);
            using (FileStream file = new FileStream(fileName, FileMode.Open))
            {
                StreamReader reader = new StreamReader(file, encoding);
                JsonSerializer serializer = new JsonSerializer();
                StringReader sr = new StringReader(reader.ReadToEnd());
                object info = serializer.Deserialize(new JsonTextReader(sr), typeof(AnalysisController));
                AnalysisController info1 = info as AnalysisController;
                ac.isWord = info1.isWord;
                ac.isImage = info1.isImage;
                ac.isFile = info1.isFile;
                ac.regexGroup = info1.regexGroup;
                ac.nowRegexGroup = info1.nowRegexGroup;
            }
        }

        /// <summary>
        /// 将对象存储为json文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="configInfo"></param>
        public static void saveAnalysisConfigAsJson(string fileName, AnalysisController ac)
        {
            ac.title = "";
            ac.content.Clear();
            ac.strList.Clear();
            string saveJsonString = JsonConvert.SerializeObject(ac);
            using (FileStream file = new FileStream(fileName, FileMode.Create))
            {
                StreamWriter writer = new StreamWriter(file, Encoding.UTF8);
                writer.Write(saveJsonString);
                writer.Close();
            }
        }
        
    }
}

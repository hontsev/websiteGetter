using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace WebsiteGetter.Analysis
{
    class ImageAnalysis
    {
        /// <summary>
        /// 根据网站来选择不同的规则，从html代码中获取需要的图片的url
        /// </summary>
        /// <param name="website"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        public static List<String> getImage(string website, string str = "")
        {
            List<String> imgs = new List<String>();
            Regex reg;
            MatchCollection m;
            switch (website)
            {
                case "wecool.socialmedia.cn":
                    reg = new Regex("img src=\\\"(.*?)\\\"");
                    m = reg.Matches(str);
                    for (int i = 0; i < m.Count; i++)
                    {
                        string path = m[i].Groups[1].ToString();
                        imgs.Add(path);
                    }
                    break;
                case "www.6188.com":
                    reg = new Regex("href=\\\"/show.php\\?pic=(.*?)\\\" title=\\\"\\\" target=\\\"_blank");
                    m = reg.Matches(str);
                    if (0 < m.Count)
                    {
                        string path1 = "http://pic.6188.com/upload_6188s" + m[0].Groups[1].ToString();
                        imgs.Add(path1);
                    }
                    break;
                case "202.112.131.80:8080":
                    reg = new Regex("<a name=\\\"(.*?)\\\" id=");
                    m = reg.Matches(str);
                    if (0 < m.Count)
                    {
                        string path1 = "http://202.112.131.80:8080/ezdcs" + m[0].Groups[1].ToString();
                        imgs.Add(path1);
                    }
                    break;
                case "www.fengrubei.net":
                    reg = new Regex("<a href=\\\"(.*?)\\\" >点击下载</a>");
                    m = reg.Matches(str);
                    if (0 < m.Count)
                    {
                        for (int i = 0; i < m.Count; i++)
                        {
                            string path1 = "http://www.fengrubei.net" + m[i].Groups[1].ToString();
                            imgs.Add(path1);
                        }
                    }
                    break;
                case "photo.blog.sina.com.cn":
                    reg = new Regex("target=\\\"_blank\\\"><img src=\\\"(.*?)\\\"");
                    m = reg.Matches(str);
                    for (int i = 0; i < m.Count; i++)
                    {
                        string path1 = m[i].Groups[1].ToString();
                        if (path1.IndexOf("sinajs") > 0) continue;
                        path1 = path1.Replace("small", "origin");
                        string path = path1 + ".jpg";
                        imgs.Add(path);
                    }
                    break;
                case "tieba.baidu.com":
                    reg = new Regex("src=\\\"(.*?)\\\"");
                    m = reg.Matches(str);
                    for (int i = 0; i < m.Count; i++)
                    {
                        string path1 = m[i].Groups[1].ToString();
                        if (path1.IndexOf("imgsrc") < 0) continue;
                        if (path1.IndexOf("tiebaimg") >= 0) continue;
                        imgs.Add(path1);
                    }
                    break;
                default:
                    //reg = new Regex("img src=\\\"(.*?)\\\"");
                    //m = reg.Matches(str);
                    //for (int i = 0; i < m.Count; i++)
                    //{
                    //    string path = m[i].Groups[1].ToString();
                    //    imgs.Add(path);
                    //}

                    reg = new Regex("src=\\\"([^\"]*?\\.(jpeg|jpg|png|gif)[^\"]*?)\\\"", RegexOptions.IgnoreCase);
                    m = reg.Matches(str);
                    for (int i = 0; i < m.Count; i++)
                    {
                        string path = m[i].Groups[1].ToString();
                        if (path.IndexOf("http") < 0)
                        {
                            Regex reg2 = new Regex("(http://.*?/)");
                            string site = reg2.Matches(website)[0].Groups[0].ToString();
                            path = site + path;
                        }
                        imgs.Add(path);
                    }
                    break;
            }
            return imgs;
        }
    }
}

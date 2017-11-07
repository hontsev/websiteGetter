using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.Web;
using System.Net;
using System.Text.RegularExpressions;
using CCWin;
using System.Diagnostics;
using WebsiteGetter.Analysis;
using WebsiteGetter.Catch;
using WebsiteGetter.Output;

namespace WebsiteGetter
{
    public partial class WSForm : CCSkinMain
    {
        private AnalysisController ac;
        private CatchController cc;
        private OutputController oc;

        private const string catchConfigPath = "cconfig";
        private const string outputConfigPath = "oconfig";
        private const string analysisConfigPath = "aconfig";

        bool run = false;

        private List<Thread> td;
        MyDelegate.sendStringDelegate printEvent;
        MyDelegate.sendStringDelegate updateNoEvent;
        MyDelegate.sendStringDelegate updateStateEvent;


        public WSForm()
        {
            InitializeComponent();
            cc = new CatchController();
            ac = new AnalysisController();
            oc = new OutputController();
            initConfig();
            //comboBox1.SelectedIndex = 0;
            //comboBox2.SelectedIndex = 0;
        }

        private void initConfig()
        {
            loadConfig();
            updateUI();
        }

        private void loadConfig()
        {
            try
            {
                JsonController.getConfigInfoFromJson(catchConfigPath, cc);
            }
            catch (Exception e)
            {
                print("读取配置文件失败："+e.Message);
            }

            try
            {
                JsonController.getOutputConfigInfoFromJson(outputConfigPath, oc);
            }
            catch (Exception e)
            {
                print("读取配置文件失败：" + e.Message);
            }

            try
            {
                JsonController.getAnalysisConfigInfoFromJson(analysisConfigPath, ac);
            }
            catch (Exception e)
            {
                print("读取配置文件失败：" + e.Message);
            }
        }

        private void saveConfig()
        {
            try
            {
                JsonController.saveConfigAsJson(catchConfigPath, cc);
            }
            catch (Exception e)
            {
                print("保存配置文件失败：" + e.Message);
            }

            try
            {
                JsonController.saveOutputConfigAsJson(outputConfigPath, oc);
            }
            catch (Exception e)
            {
                print("保存配置文件失败：" + e.Message);
            }

            try
            {
                JsonController.saveAnalysisConfigAsJson(analysisConfigPath, ac);
            }
            catch (Exception e)
            {
                print("保存配置文件失败：" + e.Message);
            }
        }

        /// <summary>
        /// 根据配置记录来刷新界面元素显示
        /// </summary>
        private void updateUI()
        {
            comboBox1.SelectedIndex = (int)cc.encoding;
            comboBox2.SelectedIndex = (int)cc.addState;
            comboBox3.SelectedIndex = (int)cc.conntype;
            //numericUpDown1.Value = config.threadNum;
            textBox2.Text = cc.url1;
            textBox3.Text = cc.url2;
            textBox7.Text = cc.url3;
            checkBox4.Checked = ac.isWord;
            checkBox5.Checked = ac.isImage;
            checkBox6.Checked = ac.isFile;
            checkBox1.Checked = oc.isSaveOneTxt;
            checkBox2.Checked = oc.isNotSmallImage;
            textBox4.Text = oc.savePath;
            textBox8.Text = cc.cookies;
            updateRegexItemView();

        }

        /// <summary>
        /// 根据界面元素来刷新配置属性
        /// </summary>
        private void updateConfig()
        {
            cc.encoding = (EncodingState)comboBox1.SelectedIndex;
            cc.addState = (AddState)comboBox2.SelectedIndex;
            cc.conntype = (ConnectType)comboBox3.SelectedIndex;
            cc.setNumberList(textBox9.Text);
            //.threadNum = Convert.ToInt32(numericUpDown1.Value);
            cc.url1 = textBox2.Text;
            cc.url2 = textBox3.Text;
            cc.url3 = textBox7.Text;
            ac.isWord = checkBox4.Checked;
            ac.isImage = checkBox5.Checked;
            ac.isFile = checkBox6.Checked;
            cc.isFile = ac.isFile;
            oc.isSaveOneTxt = checkBox1.Checked;
            oc.isNotSmallImage = checkBox2.Checked;
            oc.savePath = textBox4.Text;
            cc.cookies = textBox8.Text;
            saveRegexGroup();
        }




        /// <summary>
        /// 在界面输出log信息
        /// </summary>
        /// <param name="str"></param>
        public void print(string str)
        {
            if (textBox6.InvokeRequired)
            {
                printEvent = new MyDelegate.sendStringDelegate(print);
                Invoke(printEvent, (object)str);
            }
            else
            {
                if (textBox6.Text.Length > 500000) textBox6.Text = "";
                textBox6.AppendText(str + "\r\n");
            }
            
        }

        private void updateState(string str)
        {
            if (label6.InvokeRequired)
            {
                updateStateEvent = new MyDelegate.sendStringDelegate(updateState);
                Invoke(updateStateEvent, (object)str);
            }
            else
            {
                label6.Text = str;
            }
        }

        /// <summary>
        /// 更新当前进度显示
        /// </summary>
        /// <param name="str"></param>
        public void updateNo(string str)
        {
            if (textBox1.InvokeRequired)
            {
                updateNoEvent = new MyDelegate.sendStringDelegate(updateNo);
                Invoke(updateNoEvent, (object)str);
            }
            else
            {
                textBox1.Text = str;
            }
        }

        /// <summary>
        /// 将内容输出到文件
        /// </summary>
        /// <param name="str"></param>
        public void printToFile(string str)
        {
            //string fileName = textBox5.Text;
            string fileName = "";
            using(FileStream s = File.Open(fileName, FileMode.OpenOrCreate))
            {
                s.Position = s.Length;
                StreamWriter w = new StreamWriter(s);
                w.WriteLine(str);
                w.Close();
            }
        }
        private string ToGBK(string str)
        {
            return System.Text.Encoding.GetEncoding("UTF-8").GetString(System.Text.Encoding.Default.GetBytes(str));
        }

        private void startWorkOnce()
        {
            try
            {
                if (!run)
                {
                    cc.nowNum = 0;
                    //开始
                    updateConfig();
                    //this.domainName = config.url1.Split('/')[2];
                    run = false;
                    int tdnum = Int32.Parse(numericUpDown1.Value.ToString());
                    print("program begin. thread number:" + tdnum);
                    Thread td = new Thread(work);
                    td.Start(0);
                }
            }
            catch (Exception e)
            {
                print(e.Message);
            }
        }

        private void startTestWorkOnce()
        {
            try
            {
                if (!run)
                {
                    cc.nowNum = 0;
                    updateConfig();
                    run = false;
                    //print("program begin. ");
                    Thread td = new Thread(testwork);
                    td.Start();
                }
            }
            catch (Exception e)
            {
                print(e.Message);
            }
        }

        private void startWork()
        {
            try
            {
                if (!run)
                {
                    updateState("爬取中");
                    //开始
                    updateConfig();
                    //this.domainName = config.url1.Split('/')[2];
                    run = true;
                    int tdnum = Int32.Parse(numericUpDown1.Value.ToString());
                    print("program begin. thread number:" + tdnum);
                    td = new List<Thread>();
                    for (int i = 1; i <= tdnum; i++)
                    {
                        Thread ttd = new Thread(work);
                        td.Add(ttd);
                        ttd.Start(i);
                    }
                }
            }
            catch (Exception e)
            {
                print(e.Message);
            }
        }

        private void stopWork()
        {
            run = false;
            updateState("停止");
        }

        private void testwork()
        {

            CatchResult res = cc.catchHtml();
            html = res.text;
            if (ac.isFile && res.img!=null)
            {
                oc.saveJpg("test", res.img);
            }
            else
            {
                ac.analysis(cc.getNowUrl(), html, cc.url1);
                print("分析结果：");
                print(ac.title);
                foreach (var a in ac.content)
                {
                    string outputstr = "";
                    foreach (var v in a.Value) outputstr += v + "\r\n";
                    print(string.Format("【【【{0}】】】:\r\n\r\n\r\n\r\n{1}\r\n\r\n\r\n\r\n", a.Key, outputstr));
                }
                if (ac.strList.Count > 0) print(ac.strList[0].ToString());
            }

            

        }

        private void dealAnalysisResult(Dictionary<string,List<string>> res)
        {
            foreach (var item in res)
            {
                if (item.Key.ToLower() == "title" && item.Value != null && !string.IsNullOrWhiteSpace(item.Value.First()))
                {
                    ac.title = item.Value.First();
                }
                else if (!item.Key.StartsWith("!") && item.Key != "all")
                {
                    //惊叹号开头的那些不进行输出
                    if (item.Key.ToLower().StartsWith("img"))
                    {
                        //以img开头，视为图片来下载
                        if (item.Value != null)
                        {
                            foreach(var imgstr in item.Value)
                                if(!string.IsNullOrWhiteSpace(imgstr))ac.strList.Add(imgstr);
                        }
                    }
                    else
                    {
                        //储存为txt。
                        string savestr="";
                        //以下划线开头的，不打印其key名
                        if(!item.Key.ToLower().StartsWith("_"))
                             savestr+=string.Format("{0}:\r\n",item.Key);
                        foreach (var s in item.Value)
                        {
                            savestr += string.Format("{0}\r\n", s);
                        }
                        //savestr += "\r\n";
                        oc.output(ac.title, savestr);
                    }
                }
            }
        }

        /// <summary>
        /// 主循环工作函数
        /// </summary>
        private void work(object threadNum)
        {
            do
            {
                updateNo(cc.nowNum.ToString());
                CatchResult res = cc.catchHtml();
                html = res.text;

                if (ac.isFile && res.img != null )
                {
                    //save jpg
                    
                    //cc.getNext();
                    //cc.nowNum++;
                    string filename = cc.nowStr;
                    oc.saveJpg(filename, res.img);
                    print("output:" + filename);

                }
                else
                {
                    ac.analysis(cc.getNowUrl(), html, cc.url1);

                    //处理正则匹配结果
                    dealAnalysisResult(ac.content);
                    //if (ac.content.Count > 1 && !string.IsNullOrWhiteSpace(ac.content.ElementAt(1).Value)) oc.output(ac.title, oc.formatOutputText(ac.content), OutputType.txt);

                    //处理图片链接
                    if (ac.strList.Count > 0) print(ac.strList[0].ToString());
                    cc.saveImg(ac.strList, oc.savePath, ac.title + "_" + cc.nowNum.ToString());


                }

                if (cc.nowNum == cc.maxNum)
                {
                    run = false;
                }
            } while (run);
            if (threadNum != null) print("thread " + threadNum.ToString() + " end.");
        }


        private void workLoop()
        {
            for (int i = 0; i < 300; i++)
            {
                print(string.Format("loop:{0}", i.ToString()));
                WebConnection.getData(html, Encoding.Default);
                Thread.Sleep(100);
            }
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
        

       
        

        private void openAddress(string filePath)
        {
            Process proc = new Process();
            proc.StartInfo.FileName = "explorer";
            proc.StartInfo.Arguments = @"/select," + filePath;
            proc.Start();
        }
       



        private void button2_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox4.Text = folderBrowserDialog1.SelectedPath;
            }
        }


        private void button13_Click(object sender, EventArgs e)
        {
            startTestWorkOnce();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            startWorkOnce();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            startWork();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            stopWork();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openAddress(textBox4.Text.ToString());
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox4.Text = @"tmp\";
        }

        string html;
        private void button7_Click(object sender, EventArgs e)
        {
            html = textBox5.Text;
            new Thread(workLoop).Start();

        }

        private void WSForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            updateConfig();
            saveConfig();
            Environment.Exit(0);
        }

        private void WSForm_Shown(object sender, EventArgs e)
        {
            updateUI();
        }

        private void button15_Click(object sender, EventArgs e)
        {
            string str = @"E:\北航\网络存储\新建文件夹";
            string[] res = Directory.GetDirectories(str, "提交作业的附件", SearchOption.AllDirectories);
            foreach (var a in res)
            {
                if (Directory.GetFiles(a).Length <= 0) Directory.Delete(a);

                //string uppath = a.Replace(@"提交作业的附件", "");
                //string[] files = Directory.GetFiles(a);
                //foreach (var f in files)
                //{
                //    print(f);
                //    //print(uppath + Path.GetFileName(f));
                //    File.Move(f, uppath + Path.GetFileName(f));
                //}
            }
        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            cc.cookies = textBox8.Text;
        }

        private void updateRegexItemView()
        {
            dataGridView1.Rows.Clear();
            foreach (var r in ac.nowRegexGroup.regex)
            {
                int index = dataGridView1.Rows.Add();
                this.dataGridView1.Rows[index].Cells[0].Value = r.sourcename;
                this.dataGridView1.Rows[index].Cells[1].Value = r.regex;
                this.dataGridView1.Rows[index].Cells[2].Value = r.name;
            }
        }

        private void saveRegexGroup()
        {
            ac.nowRegexGroup.regex.Clear();
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                RegexItem ri = new RegexItem();
                if (dataGridView1.Rows[i].Cells[0].Value==null||dataGridView1.Rows[i].Cells[1].Value==null||dataGridView1.Rows[i].Cells[2].Value==null) continue;
                ri.sourcename = dataGridView1.Rows[i].Cells[0].Value.ToString();
                ri.regex = dataGridView1.Rows[i].Cells[1].Value.ToString();
                ri.name = dataGridView1.Rows[i].Cells[2].Value.ToString();
                ac.nowRegexGroup.regex.Add(ri);
            }
            ac.nowRegexGroup.target = cc.url1;
            ac.regexGroup.Remove(ac.regexGroup.Find(item => item.target == ac.nowRegexGroup.target));
            ac.regexGroup.Add(ac.nowRegexGroup);
        }

        private void textBox6_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(MousePosition);
            }
        }

        private void 清空ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox6.Text = "";
        }

        private void workClearMultiImages()
        {
            print("clear multi-images");
            string path2 = "tmp_sames/";
            try
            {
                string path = textBox4.Text;
                List<string> files = new List<string>(Directory.GetFiles(path));
                List<FileInfo> fileinfos = new List<FileInfo>();
                foreach (var f in files) fileinfos.Add(new FileInfo(f));
                for (int i = 0; i < files.Count; i++)
                {
                    print(string.Format("{0}/{1}", i, files.Count));
                    //if (!isImage(files[i])) continue;
                    for (int j = i + 1; j < files.Count; j++)
                    {
                        if (j >= files.Count) break;
                        //if (!isImage(files[j])) continue;
                        try
                        {
                            //print(string.Format("at{0}", j));
                            if (fileinfos[i].Length == fileinfos[j].Length)
                            {
                                //if (!Directory.Exists(path2)) Directory.CreateDirectory(path2);
                                //File.Move(files[j], string.Format("{0}{1}", path2, Path.GetFileName(files[j])));
                                File.Delete(files[j]);
                                fileinfos.RemoveAt(j);
                                files.RemoveAt(j);
                                j--;
                            }
                        }catch(Exception e)
                        {

                        }

                    }
                }
            }
            catch(Exception e)
            {
                print("error");
            }
            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            new Thread(workClearMultiImages).Start();
        }
       
    }
}

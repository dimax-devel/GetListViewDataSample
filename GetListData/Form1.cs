using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace GetListData
{

    public partial class Form1 : Form
    {    
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr FindWindowEx(IntPtr hWndParent, IntPtr hWndChildAfter, string lpClassName, string lpWindowName);
 
        public Form1()
        {
            InitializeComponent();
            const int CHECK_INTERVAL_MSEC = 10000; // 10sec(sample)
            timer1.Interval = CHECK_INTERVAL_MSEC;
            // 定期的にチェックするためのタイマを起動
            timer1.Start();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CheckTrigger("");
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            var triggerON = CheckTrigger("");
            if (triggerON == true)
            {
                System.Diagnostics.Debug.WriteLine("Trigger on");
            }
            else;
            {
                System.Diagnostics.Debug.WriteLine("Trigger off");
            }
        }

        private bool CheckTrigger(string keyword)
        {
            try
            {
                const string SEARCHNAME = "シグナル履歴";
                // SHIFT_JISを使えるようにする呪文
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                // ウィンドウ一覧からシグナル履歴のウィンドウを取得
                var targetWindow = ManagedWinapi.Windows.SystemWindow.AllToplevelWindows.FirstOrDefault(_ => _.Title == SEARCHNAME);
                if(targetWindow == null)
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("{0} is not found.", SEARCHNAME));
                    return false;
                }
                // SysListViewクラスののウィンドウを取得
                var listWindow = targetWindow.AllDescendantWindows.FirstOrDefault(_ => _.ClassName.Contains("SysListView"));
                if (listWindow == null)
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("listwindow is not found."));
                    return false;
                }
                // DAO（SystemListViewオブジェクト）を作成
                var listView = ManagedWinapi.Windows.SystemListView.FromSystemWindow(listWindow);
                if (listView == null)
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("listview is not found."));
                    return false;
                }
                for (var row = 0; row < listView.Count; row++)
                {
                    for (var col = 0; col < listView.Columns.Length; col++)
                    {
                        // LISTVIEWのデータを取得（ライブラリのロジックで文字化けするので、一部改造）
                        var text = listView[row, col].Title;
                        System.Diagnostics.Debug.WriteLine(text);
                        if (text.Contains(keyword))
                        {
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
            return false;
        }
    }
}

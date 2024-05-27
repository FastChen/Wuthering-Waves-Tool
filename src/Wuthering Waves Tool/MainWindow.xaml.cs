using Microsoft.Data.Sqlite;
using Microsoft.Win32;
using System.Diagnostics;
using System.Text.Json;
using System.Windows;

namespace Wuthering_Waves_Tool
{
    /// <summary>
    /// 作者：FASTCHEN
    /// 网站：FASTCHEN.COM
    /// Github：https://github.com/fastchen
    /// 本工具已开源，请遵循开源协议使用。
    /// 用时20分钟制作，未优化，未作防呆设计，仅固定流程使用。
    /// </summary>
    public partial class MainWindow : Window
    {
        private GameQualitySettingEntity gameQualitySettingEntity = new GameQualitySettingEntity();
        private string db_LocalStorage_Path;
        public MainWindow()
        {
            InitializeComponent();
            MessageBox.Show("此工具已开源至 Github：https://github.com/fastchen\r\n本人用时20分钟制作，未优化，未作防呆设计，仅固定流程使用。\r\n不能保证修改文件是否会导致游戏封禁，修改后一切后果自行承担。", "提示" ,MessageBoxButton.OK, MessageBoxImage.Warning);

            this.Title = "鸣潮自定义帧率解锁工具 - FASTCHEN.COM -v0.0.1.528";
            this.MinHeight = this.Height;
            this.MinWidth = this.Width;
        }

        private void Button_Test_Click(object sender, RoutedEventArgs e)
        {
            gameQualitySettingEntity.KeyCustomFrameRate = Convert.ToInt32(TextBox_FrameRate.Text);

            using (var connection = new SqliteConnection(@$"Data Source={db_LocalStorage_Path}"))
            {
                connection.Open();
                string updateQuery = $"UPDATE LocalStorage SET Value = @Value WHERE Key = @Key";
                using (var command = new SqliteCommand(updateQuery, connection))
                {
                    command.Parameters.AddWithValue("@Value", JsonSerializer.Serialize(gameQualitySettingEntity));
                    command.Parameters.AddWithValue("@Key", "GameQualitySetting");
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("已成功修改帧率，登录游戏后查看，\r\n如无效检查游戏是否开启了垂直同步。");
                    }
                    else
                    {
                        MessageBox.Show("修改失败，表内未找到键名。");
                    }
                }
            }

        }

        private void Button_Read_Click(object sender, RoutedEventArgs e)
        {
            // 本地 SQLite 数据库文件路径
            string dataSource = @$"Data Source={db_LocalStorage_Path}";

            // 创建一个连接到数据库
            using (var connection = new SqliteConnection(dataSource))
            {
                connection.Open();

                // 创建一个 SQL 命令
                string sql = "SELECT * FROM LocalStorage where key=\"GameQualitySetting\"";
                using (var command = new SqliteCommand(sql, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        // 读取数据
                        while (reader.Read())
                        {
                            string key = reader.GetString(0);
                            string value = reader.GetString(1);

                            Trace.WriteLine($"key: {key} value: {value}");

                            gameQualitySettingEntity = JsonSerializer.Deserialize<GameQualitySettingEntity>(value);

                            Button_Test.IsEnabled = true;

                            MessageBox.Show($"当前游戏设置帧率为：{gameQualitySettingEntity.KeyCustomFrameRate}");
                            
                            //Trace.WriteLine($"当前游戏设置帧率为：{gameQualitySettingEntity.KeyCustomFrameRate}");

                        }
                    }
                }
            }
        }

        private void Button_Path_Game_Click(object sender, RoutedEventArgs e)
        {
            // 创建一个新的 OpenFileDialog 实例
            OpenFileDialog openFileDialog = new OpenFileDialog();

            // 设置过滤器，便于用户只选择特定类型的文件
            openFileDialog.Title = @"路径：游戏目录\Wuthering Waves Game\Client\Saved\LocalStorage\LocalStorage.db";
            openFileDialog.Filter = "LocalStorage.db (*.db)|*.db";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;

            // 显示对话框，并检查用户是否选择了一个文件
            if (openFileDialog.ShowDialog() == true)
            {
                // 获取选中的文件路径
                string selectedFilePath = openFileDialog.FileName;

                // 在文本框中显示选中的文件路径
                TextBox_Path_Game.Text = selectedFilePath;
                db_LocalStorage_Path = selectedFilePath;

                Button_Read.IsEnabled = true;
            }
        }

        private void Button_Use_Click(object sender, RoutedEventArgs e)
        {
            StartLink("https://space.bilibili.com/23945424");
        }

        public static void StartLink(string? url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                //Process.Start("explorer", url);
                var psi = new ProcessStartInfo
                {
                    FileName = url,
                    WorkingDirectory = "",
                    UseShellExecute = true
                };
                Process.Start(psi);
            }
        }
    }
}
using System.IO;
using Fpath = System.IO.Path;
using System.Windows;
using Microsoft.Win32;
using System.Text.RegularExpressions;

namespace EasyFileMover
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Boolean firstState1;
        private Boolean firstState2;
        public MainWindow()
        {
            InitializeComponent();
            FilePathTextBox1.Text = "中身を移動したいフォルダパスを選択してください(ドラッグ&ドロップも可)";
            FilePathTextBox2.Text = "移動先のフォルダパスを選択してください(ドラッグ&ドロップも可)";
            AlertTextBox1.Content = "移動元フォルダを選択してください(指定できるフォルダは一つまで)";
            AlertTextBox2.Content = "移動先フォルダを選択してください(指定できるフォルダは一つまで)";
            firstState1 = true;
            firstState2 = true;
        }

        private void Button_Click1(object sender, RoutedEventArgs e)
        {
            //移動元ファイルパスの指定
            OpenFolderDialog d = new OpenFolderDialog();
            if (d.ShowDialog() == true)
            {
                FilePathTextBox1.Text = d.FolderName;
            }
        }
        private void Button_Click2(object sender, RoutedEventArgs e)
        {
            //移動先ファイルパスの指定
            OpenFolderDialog d = new OpenFolderDialog();
            if (d.ShowDialog() == true)
            {
                FilePathTextBox2.Text = d.FolderName;
            }
        }

        private async void Run_Move(object sender, RoutedEventArgs e)
        {
            //警告の有無を確認してから実行する
            if (String.IsNullOrEmpty(AlertTextBox1.Content.ToString()) && String.IsNullOrEmpty(AlertTextBox2.Content.ToString()))
            {



                //ファイルの移動前実行確認
                MessageBoxResult r = MessageBox.Show("ファイルの移動を実行します。本当に開始しますか？", "確認", MessageBoxButton.YesNo);


                if (r == MessageBoxResult.Yes)
                {
                    if (Check_folderExist(FilePathTextBox1.Text) || Check_folderExist(FilePathTextBox2.Text))
                    {
                        Boolean removeFlag;
                        removeFlag = false;
                        //フォルダ削除フラグにチェックが入っていた場合の処理確認
                        if (checkremove.IsChecked == true)
                        {
                            //サブフォルダーが含まれない場合は削除を実行する
                            if (Check_SubFolder())
                            {
                                MessageBoxResult r2 = MessageBox.Show("ファイルの移動後フォルダは削除されますが本当によろしいですか？", "確認", MessageBoxButton.YesNo);
                                if (r2 == MessageBoxResult.Yes)
                                {
                                    removeFlag = true;
                                }
                                else if (r2 == MessageBoxResult.No)
                                {
                                    MessageBox.Show("処理を実行せずに中断します", "メッセージ", MessageBoxButton.OK);
                                    return;
                                }
                                else
                                {
                                    return;
                                }

                            }
                            //サブフォルダーが含まれる場合は削除は実行しない
                            else
                            {
                                MessageBox.Show("移動先が移動元のサブフォルダに含まれるためフォルダの削除は行えません。", "メッセージ", MessageBoxButton.OK);
                                removeFlag = false;
                            }

                        }
                        else
                        {
                            //チェックが入っていない場合は削除を実行しない
                            removeFlag = false;

                        }
                        //ファイルの移動を実行する
                        foreach (string FilePath in Directory.GetFiles(FilePathTextBox1.Text))
                        {
                            try
                            {
                                string fileName = Fpath.GetFileName(FilePath);
                                string destinationFilePath = Fpath.Combine(FilePathTextBox2.Text, fileName);
                                File.Move(FilePath, destinationFilePath);
                            }
                            catch (Exception ex)
                            {
                                //移動が失敗した場合の例外処理
                                MessageBox.Show(FilePath + "について処理が行えませんでした\r\n【詳細】\r\n" + ex.Message, "メッセージ", MessageBoxButton.OK);
                                removeFlag = false;
                            }

                        }
                        //フォルダファイルの移動を実行する
                        foreach (string dirPath in Directory.GetDirectories(FilePathTextBox1.Text))
                        {
                            try
                            {
                                string dirName = Fpath.GetFileName(dirPath);
                                string destinationdirPath = Fpath.Combine(FilePathTextBox2.Text, dirName);
                                Directory.Move(dirPath, destinationdirPath);
                            }
                            catch (Exception ex)
                            {
                                //移動が失敗した場合の例外処理
                                MessageBox.Show(dirPath + "について移動処理が行えませんでした\r\n【詳細】\r\n" + ex.Message, "メッセージ", MessageBoxButton.OK);
                                removeFlag = false;
                            }
                        }
                        //削除フラグが立っている場合は移動の元となったフォルダを削除する
                        if (removeFlag == true)
                        {
                            if (!Directory.EnumerateFileSystemEntries(FilePathTextBox1.Text).Any())
                            {
                                Directory.Delete(FilePathTextBox1.Text);
                            }
                            else
                            {
                                //フォルダが空でない場合(例外処理等が発生して処理できないファイルがあった場合)の処理
                                MessageBox.Show("フォルダの中身が空でないため削除できません。", "メッセージ", MessageBoxButton.OK);
                            }
                        }
                        else
                        {
                            //削除フラグが立っていない場合は何もしない
                        }

                        //処理が完了したらメッセージを出力
                        MessageBox.Show("移動元フォルダ内のファイル・フォルダを移動先フォルダへ移動しました。" ,"メッセージ", MessageBoxButton.OK);

                    }
                    else
                    {
                        //記載不備のメッセージ
                        MessageBox.Show("フォルダパス未指定・フォルダが存在しない・移動元と移動先が同じである可能性があります。確認して再実行してください。", "メッセージ", MessageBoxButton.OK);
                        return;
                    }
                }
                else if (r == MessageBoxResult.No)
                {
                    //中断した場合のメッセージ
                    MessageBox.Show("処理を実行せずに中断します", "メッセージ", MessageBoxButton.OK);
                    return;
                }
                else
                {
                    //それ以外の理由で中断された時の処理
                    return;
                }
            }
            else
            {
                MessageBox.Show("ファイルが選択されていないか、パスが存在しないため処理を実行できません。", "メッセージ", MessageBoxButton.OK);
                return;
            }
        }
        //フォルダ存在確認・入力チェック・同じフォルダの有無確認
        private Boolean Check_folderExist(String folderPath)
        {
            if (Directory.Exists(folderPath) && !String.IsNullOrEmpty(folderPath) && (FilePathTextBox1.Text != FilePathTextBox2.Text)){
                return true;
            }
            else
            {
                return false;
            }
        }
        //サブフォルダ関係判定
        private Boolean Check_SubFolder()
        {
            var uri1 = new Uri(FilePathTextBox1.Text);
            var uri2 = new Uri(FilePathTextBox2.Text);
            if(uri1.IsBaseOf(uri2))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //ドラッグオーバーしたときの処理
        private void textBoxDragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop, true))
            {
                e.Effects = System.Windows.DragDropEffects.Copy;
            }
            else
            {
                e.Effects = System.Windows.DragDropEffects.None;
            }
            e.Handled = true;
        }
        //ドロップしたときの処理
        private void textBox1Drop(object sender, DragEventArgs e)
        {
            var dropFile = e.Data.GetData(System.Windows.DataFormats.FileDrop) as String[];
            if (dropFile != null && dropFile.Length == 1)
            {
                FilePathTextBox1.Text = dropFile[0];
            }
            else
            {
                MessageBox.Show("一度に指定できるフォルダは一つまでです。", "メッセージ", MessageBoxButton.OK);
                return;
            }
        }
        private void textBox2Drop(object sender, DragEventArgs e)
        {
            var dropFile = e.Data.GetData(System.Windows.DataFormats.FileDrop) as String[];
            if (dropFile != null && dropFile.Length == 1)
            {
                FilePathTextBox2.Text = dropFile[0];
            }
            else
            {
                MessageBox.Show("一度に指定できるフォルダは一つまでです。", "メッセージ", MessageBoxButton.OK);
                return;
            }
        }
//入力欄のテキストの内容が変わった時の処理

        private void FilePathTextBox1_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            //内容変更時のフォルダのフォーマットのチェック
            if (String.IsNullOrEmpty(FilePathTextBox1.Text))
            {

                AlertTextBox1.Content = "移動元フォルダにフォルダパスが入力されていません";
            }
            else if (!Directory.Exists(FilePathTextBox1.Text))
            {
                AlertTextBox1.Content = "移動元フォルダに存在しないパスが指定されています";
            }
            else if (FilePathTextBox1.Text == FilePathTextBox2.Text)
            {
                AlertTextBox1.Content = "移動元フォルダと移動先フォルダに同じフォルダが指定されています。";
                AlertTextBox2.Content = "移動元フォルダと移動先フォルダに同じフォルダが指定されています。";
            }
            else
            {
                AlertTextBox1.Content = "";
            }

            //初期状態の変更
            if(firstState1 == true)
            {
                firstState1 = false;
            }
            //初期状態じゃない場合はファイル判定をする
            if (firstState2 == false)
            {
                if (String.IsNullOrEmpty(FilePathTextBox2.Text))
                {

                    AlertTextBox2.Content = "移動元フォルダにフォルダパスが入力されていません";
                }
                else if (!Directory.Exists(FilePathTextBox2.Text))
                {
                    AlertTextBox2.Content = "移動元フォルダに存在しないパスが指定されています";
                }
                else if (FilePathTextBox1.Text == FilePathTextBox2.Text)
                {
                    AlertTextBox1.Content = "移動元フォルダと移動先フォルダに同じフォルダが指定されています。";
                    AlertTextBox2.Content = "移動元フォルダと移動先フォルダに同じフォルダが指定されています。";
                }
                else
                {
                    AlertTextBox2.Content = "";
                }
            }
            //サブフォルダーの場合チェックボックスをOFFにする
            if ((String.IsNullOrEmpty(AlertTextBox1.Content.ToString()) && String.IsNullOrEmpty(AlertTextBox2.Content.ToString())) && Check_SubFolder())
            {
                checkremove.IsChecked = false;
                checkremove.IsEnabled = false;
            }
        }
        private void FilePathTextBox2_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            //内容変更時のフォルダのフォーマットのチェック
            if (String.IsNullOrEmpty(FilePathTextBox2.Text))
            {

                AlertTextBox2.Content = "移動先フォルダにフォルダパスが入力されていません";
            }
            else if (!Directory.Exists(FilePathTextBox2.Text))
            {
                AlertTextBox2.Content = "移動先フォルダに存在しないパスが指定されています";
            }
            else if (FilePathTextBox1.Text == FilePathTextBox2.Text)
            {
                AlertTextBox1.Content = "移動元フォルダと移動先フォルダに同じフォルダが指定されています。";
                AlertTextBox2.Content = "移動元フォルダと移動先フォルダに同じフォルダが指定されています。";
            }
            else
            {
                AlertTextBox2.Content = "";
            }

            //初期状態の変更
            if (firstState2 == true)
            {
                firstState2 = false;
            }
            //初期状態じゃない場合はファイル判定をする
            if (firstState1 == false)
            {
                if (String.IsNullOrEmpty(FilePathTextBox1.Text))
                {

                    AlertTextBox1.Content = "移動元フォルダにフォルダパスが入力されていません";
                }
                else if (!Directory.Exists(FilePathTextBox1.Text))
                {
                    AlertTextBox1.Content = "移動元フォルダに存在しないパスが指定されています";
                }
                else if (FilePathTextBox1.Text == FilePathTextBox2.Text)
                {
                    AlertTextBox1.Content = "移動元フォルダと移動先フォルダに同じフォルダが指定されています。";
                    AlertTextBox2.Content = "移動元フォルダと移動先フォルダに同じフォルダが指定されています。";
                }
                else
                {
                    AlertTextBox1.Content = "";
                }
            }
            //サブフォルダーの場合チェックボックスをOFFにする
            if ((String.IsNullOrEmpty(AlertTextBox1.Content.ToString()) && String.IsNullOrEmpty(AlertTextBox2.Content.ToString())) && Check_SubFolder())
            {
                checkremove.IsChecked = false;
                checkremove.IsEnabled = false;
            }
        }
    }
}
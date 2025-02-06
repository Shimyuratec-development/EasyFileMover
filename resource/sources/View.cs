using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Text.RegularExpressions;
using EasyFileMover;
using System.Threading.Tasks;
using System.Diagnostics.Eventing.Reader;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.Xaml.Behaviors.Core;


/// <summary>
/// ビューモデルの定義
/// </summary>
public class MainViewModel : INotifyPropertyChanged
{
    /// <summary>
    /// メンバ変数の定義
    /// </summary>
    public ObservableCollection<Item> FilePathItems {
		get { return _FilePathItems!; }
		set
		{
			_FilePathItems = value;
			OnPropertyChanged(nameof(FilePathItems));
		} 
	}
	private ObservableCollection<Item>? _FilePathItems;
	public Item FilePathIteminTextBox
	{
		get { return _FilePathIteminTextBox!; }
		set
		{
			if (_FilePathIteminTextBox != value)
			{
				_FilePathIteminTextBox = value;
				OnPropertyChanged(nameof(FilePathIteminTextBox));
			}
		}

	}
	bool customDialogIsOpen;

	private Item? _FilePathIteminTextBox;

	public AbortItem AbortItem
	{
		get { return _abortitem!; }
		set
		{
			if (_abortitem != value)
			{
				_abortitem = value;
				OnPropertyChanged(nameof(AbortItem));
			}
		}
	}
	private AbortItem? _abortitem;
    public AbortItem AbortItem2
    {
        get { return _abortitem2!; }
        set
        {
            if (_abortitem2 != value)
            {
                _abortitem2 = value;
                OnPropertyChanged(nameof(AbortItem));
            }
        }
    }
	private AbortItem? _abortitem2;
    public bool EmptyState;
	private RunnableState? _state;
	public RunnableState state
	{
		get { return _state!; }
		set
		{
			if(_state != value)
			{
				_state = value;
				OnPropertyChanged(nameof(state));
			}
		}
	}
	private NowProcessing? _NowProcessing;
	public NowProcessing NowProcessing
	{
		get { return _NowProcessing!; }
		set
		{
			if (_NowProcessing != value)
			{
				_NowProcessing = value;
				OnPropertyChanged(nameof(NowProcessing));
			}
		}
	}
	private CustomDialog? _customDialog;
	public CustomDialog customDialog
	{
		get { return _customDialog!; }
		set
		{
			if (_customDialog != value)
			{
				_customDialog = value;
				OnPropertyChanged(nameof(customDialog));
			}
		}
	}
    private NortifiWindow? _nortifi;
    public NortifiWindow nortifi
    {
        get { return _nortifi!; }
        set
        {
            if (_nortifi != value)
            {
                _nortifi = value;
                OnPropertyChanged(nameof(nortifi));
            }
        }
    }
    private WindowState? _WindowStateMain;
    public WindowState WindowStateMain
    {
        get { return _WindowStateMain!; }
        set
        {
            if (_WindowStateMain != value)
            {
                _WindowStateMain = value;
                OnPropertyChanged(nameof(WindowStateMain));

            }
        }
    }
    public string[] Sysfolder = { @"^[A-Z]:\\\\$",@"^C:\\[^\\]+\\$",@"^C:\\Windows\\.*$", @"^C:\\ProgramData\\.*$",@"^C:\\Program Files\\[^\\]+\\$", @"^C:\\Program Files (x86)\\[^\\]+\\$", @"^C:\\Users\\$", @"^C:\\Users\\[^\\]+\\$", @"^C:\\System32\\.*$", @"^C:\\System Volume Information\\.*" };
	private CancellationTokenSource? CancellToken;
	//private static SynchronizationContext? _SynchronizationContext;

    /// <summary>
    /// ICommandの設定
    /// </summary>

    public ICommand AddItemCommand { get; set; }
    public ICommand AddItemToTextBoxByDropCommand{ get; set; }
    public ICommand AddItemToTextBoxByFileLauncherCommand { get; set; }
    public ICommand AddItemsCommand { get; set; }
	public ICommand ChangeDragOver {  get; set; }
	public ICommand DeleteListBoxContentCommand {  get; set; }
	public ICommand ResetTextBoxCommand {  get; set; }
	public ICommand RunMoveCommand {  get; set; }
	public ICommand CloseWindowCommand { get; set; }
	public ICommand WindowCloseMangageCommand {  get; set; }


    /// <summary>
    /// コンストラクタ
    /// </summary>

    public MainViewModel()
	{
		FilePathItems = new ObservableCollection<Item>();
		FilePathIteminTextBox = new Item
		{
			FilePath = "",
			IsFolder = true,
			Mode = 0
		};
		state = new RunnableState { IsRunnable = false };
        AbortItem = new AbortItem { Item = "移動元のファイル・フォルダを選択してください(複数選択可)(ドラッグ&ドロップ可)" };
		AbortItem2 = new AbortItem { Item = "移動先となるフォルダを選択してください(ドラッグ&ドロップ可)" };
		NowProcessing = new NowProcessing
		{
			Item = "",
			Progress = 0,
			CanFinish = false,
			CanStop = true
		};
		
		
		EmptyState = false;
		AddItemCommand = new RelayCommand(AddItem, canExecute:() => true);
        AddItemToTextBoxByDropCommand = new RelayCommand2<object>(AddItemToTextBoxByDrop, canExecute: (AddItemToTextBoxByDrop) => true);
		AddItemToTextBoxByFileLauncherCommand = new RelayCommand(AddItemToTextBoxByFileLauncher, canExecute: () => true);
        AddItemsCommand = new RelayCommand2<object>(AddItems, canExecute: (AddItems) => true);
		ChangeDragOver = new RelayCommand2<object>(DragOverEvent, canExecute: (DragOverEvent) => true);
		DeleteListBoxContentCommand = new RelayCommand2<object>(DeleteListBoxContent, canExecute: (DeleteListBoxContent) => true);
		ResetTextBoxCommand = new RelayCommand(ResetTextBox,canExecute:() => true);
		RunMoveCommand = new RelayCommand(RunMove, canExecute: () => true);
		CloseWindowCommand = new RelayCommand2<Window>(CloseWindow, canExecute: (CloseWindow) => true);
		WindowCloseMangageCommand = new RelayCommand2<Window>(WindowCloseMangage, canExecute: (indowCloseMangage) => true);
		WindowStateMain = new WindowState
		{
			IsEnable = true,
			IsClosable = true
		};
		customDialogIsOpen = false;
    }
    /// <summary>
    /// リストビューへのアイテム追加
    /// </summary>
    private void AddItem()
	{
		MessageBox.Show("移動元となるフォルダーを選択してください(複数選択可)");
		OpenFolderDialog OpenFolderDialog = new OpenFolderDialog
		{
			FolderName = "SelectFolder",
			Multiselect = true,
			InitialDirectory=@"C:\"

		};

        if (OpenFolderDialog.ShowDialog() == true)
		{
			foreach(string folder in OpenFolderDialog.FolderNames)
			{
                if (Directory.Exists(folder))
                {

					Item item = new Item()
					{
						FilePath = folder,
						IsFolder = true,
						Mode = 0
					};

					if (FilePathItems.Where(x => x.FilePath == item.FilePath).Count() == 0)
					{
						if (isReadOnly(item.FilePath))
						{
							MessageBox.Show(item.FilePath + "は読み取り専用のため追加できませんでした。");
						}
						else if(isSysFile(item.FilePath))
						{
                            MessageBox.Show(item.FilePath + "はシステムフォルダのため追加できませんでした。");
                           
						}
						else　if(isUnVisible(item.FilePath))
						{
                            MessageBox.Show(item.FilePath + "は隠しフォルダ属性のため追加できませんでした");
						}
						else
						{
                            FilePathItems.Add(item);
                        }
					}

                    
                }
            }

        }
        MessageBox.Show("移動したいファイルを選択してください(複数選択可)");
		OpenFileDialog OpenFileDialog = new OpenFileDialog
		{
			FileName = "SelectFile",
			Multiselect = true,
			Filter="全てのファイル(*.*)|*.*",
            InitialDirectory = @"C:\"

        };
		if (OpenFileDialog.ShowDialog() == true)
		{
			foreach (string file in OpenFileDialog.FileNames)
			{
				if (File.Exists(file))
				{

                    Item item = new Item()
                    {
                        FilePath = file,
                        IsFolder = false,
						Mode = 0
                    };

                    if (FilePathItems.Where(x => x.FilePath == item.FilePath).Count() == 0)
					{
                        if (isReadOnly(item.FilePath))
                        {
                            MessageBox.Show(item.FilePath + "は読み取り専用のため追加できません。");
                        }
                        else if (isSysFile(item.FilePath) || isSysFile(item.FilePath))
                        {
                            MessageBox.Show(item.FilePath + "はシステムファイルのため追加できません。");

                        }
                        else if (isUnVisible(item.FilePath))
                        {
                            MessageBox.Show(item.FilePath + "は隠しフォルダ属性のため追加できません。");
                        }
                        else
                        {
                            FilePathItems.Add(item);
                        }

                    }
				}
			}
		}

        if (FilePathItems.Count > 0)
		{
			AbortItem.Item = "";
			if (!string.IsNullOrEmpty(FilePathIteminTextBox.FilePath))
			{
				if (isContain(FilePathItems, FilePathIteminTextBox.FilePath))
				{
					state.IsRunnable = false;
					MessageBox.Show("移動先に移動元の子フォルダ・もしくは同じフォルダが含まれます。当該のアイテムを一覧から削除するか移動先フォルダを変更してください。");
				}
				else
				{
					state.IsRunnable = true;
				}
			}
		}

		
	}
    private void AddItems(object parameter)
    {


        if (parameter != null && parameter is DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files != null)
            {
                try
                {
                    foreach (string file in files)
                    {
                        if (File.Exists(file) || Directory.Exists(file))
                        {
                            bool isdir = false;
                            if (File.Exists(file))
                            {
                                isdir = false;
                            }
                            else
                            {
                                isdir = true;
                            }

                            Item item = new Item()
                            {
                                FilePath = file,
                                IsFolder = isdir,
                                Mode = 0
                            };


                            if (FilePathItems.Where(x => x.FilePath == item.FilePath).Count() == 0)
                            {
								if (isReadOnly(item.FilePath))
								{
									MessageBox.Show(item.FilePath + "は読み取り専用のため追加できません。");
								}
								else if (isSysFile(item.FilePath))
								{
									MessageBox.Show(item.FilePath + "はシステムファイルのため追加できません。");
								}
								else if (isUnVisible(item.FilePath))
								{
									MessageBox.Show(item.FilePath + "は隠しフォルダ属性のため追加できません。");
								}
								else
								{
									FilePathItems.Add(item);
								}
                            }
                        }
                    }

                }
                catch (Exception Exception)
                {
                    string directoryPath = AppDomain.CurrentDomain.BaseDirectory;

                    string filePath = Path.Combine(directoryPath, "log.txt");

                    using (StreamWriter writer = new StreamWriter(filePath, true))
                    {
                        writer.WriteLine(Exception.Message);
                    }
                }
            }
        }
        if (FilePathItems.Count > 0)
        {

            this.AbortItem.Item = "";
            if (isContain(FilePathItems, FilePathIteminTextBox.FilePath))
            {
                state.IsRunnable = false;
                MessageBox.Show("移動先に移動元の子フォルダ・もしくは同じフォルダが含まれます。当該のアイテムを一覧から削除するか移動先フォルダを変更してください。");
            }else if(!string.IsNullOrEmpty(FilePathIteminTextBox.FilePath))
            {
                state.IsRunnable = true;
            }
        }


    }
    /// <summary>
    /// テキストボックスへの入力
    /// </summary>
    private void AddItemToTextBoxByDrop(object parameter)
    {
		if (parameter != null && parameter is DragEventArgs e)
		{
			string[] files =(string[])e.Data.GetData(DataFormats.FileDrop);
			if (files != null)
			{
				try
				{
					if(files.Count() == 1)
					{
						foreach (string file in files)
						{
							if (File.Exists(file))
							{
								MessageBox.Show("フォルダを指定してください。");
							}
							else
							{
								if (isSysFile(file))
								{
									MessageBox.Show("システムフォルダであるため指定できません。");
								}else if (isReadOnly(file))
								{
                                    MessageBox.Show("読み取り専用であるため指定できません。");
                                }else if (isUnVisible(file))
								{
									MessageBox.Show("隠しフォルダ属性であるため指定できません");
								}else {
									bool IsOK = true;
                                    if (isContain(FilePathItems, file))
                                    {
                                        IsOK = false;
                                    }
                                    if (IsOK)
									{
										FilePathIteminTextBox.FilePath = file;
										this.AbortItem2.Item = "";
										if (FilePathItems.Count > 0)
										{
											state.IsRunnable = true;
										}
									}
									else
									{
										MessageBox.Show("移動元に親フォルダ・もしくは同じフォルダが含まれます。他のパスを指定してください。");
									}
								}
				            }
						}
							
					}
					else
					{
						MessageBox.Show("移動先として指定可能なのは1フォルダまでです。");
					}

				}
				catch (Exception Exception)
				{
                    string directoryPath = AppDomain.CurrentDomain.BaseDirectory;

                    string filePath = Path.Combine(directoryPath, "log.txt");

                    using (StreamWriter writer = new StreamWriter(filePath, true))
                    {
                        writer.WriteLine(Exception.Message);
                    }
                }
			}
        }
    }

    private void AddItemToTextBoxByFileLauncher()
    {
        MessageBox.Show("移動先となるフォルダーを選択してください");
        OpenFolderDialog OpenFolderDialog = new OpenFolderDialog
        {
            FolderName = "SelectFolder",
            Multiselect = false,
            InitialDirectory = @"C:\"

        };

        if (OpenFolderDialog.ShowDialog() == true)
		{
			if (Directory.Exists(OpenFolderDialog.FolderName))
			{
				if (isSysFile(OpenFolderDialog.FolderName))
				{
                    MessageBox.Show("システムフォルダであるため指定できません。");
                }
				else if (isReadOnly(OpenFolderDialog.FolderName)){
                    MessageBox.Show("読み取り専用であるため指定できません。");
                }
                else if (isUnVisible(OpenFolderDialog.FolderName))
				{
                    MessageBox.Show("隠しフォルダ属性であるため指定できません");
                }
                else{
					bool IsOK = true;
					
					if (isContain(FilePathItems,OpenFolderDialog.FolderName))
					{
						IsOK = false;
					}
					if (IsOK == true)
					{
						FilePathIteminTextBox.FilePath = OpenFolderDialog.FolderName;
						this.AbortItem2.Item = "";
						if (FilePathItems.Count > 0)
						{

							state.IsRunnable = true;
						}
					}
					else
					{
						MessageBox.Show("移動元に親フォルダ・もしくは同じフォルダが含まれます。他のパスを指定してください。");
					}
				}

			}
		}
    }

    /// <summary>
    /// テキストボックスへの入力
    /// </summary>

    private void DragOverEvent(object parameter)
	{
		if (parameter is  DragEventArgs e)
		{
			e.Handled = true;
		}
	}
    /// <summary>
    /// リストからの削除処理
    /// </summary>
    private void DeleteListBoxContent(object parameter)
	{
		var SelectedItems = parameter as IList<object>;
		if (SelectedItems != null && SelectedItems.Count >0)
		{

			foreach (var item in SelectedItems.ToList())
			{
				if (item is Item i)
				{
					FilePathItems.Remove(i);
				}

			}
		}

        if (FilePathItems.Count == 0)
        {
            this.AbortItem.Item = "移動元のファイル・フォルダを選択してください(複数選択可)(ドラッグ&ドロップ可)";
			state.IsRunnable = false;

		}
		else if (isContain(FilePathItems,FilePathIteminTextBox.FilePath))
		{
            state.IsRunnable = false;
		}
		else
		{
            state.IsRunnable = true;
        }
    }
    /// <summary>
    /// テキストボックスからの削除処理
    /// </summary>
    private void ResetTextBox()
    {
        FilePathIteminTextBox.FilePath = string.Empty;
        this.AbortItem2.Item = "移動先となるフォルダを選択してください(ドラッグ&ドロップ可)";
        state.IsRunnable = false;
        
    }
    /// <summary>
    /// ウインドウを閉じる処理
    /// </summary>
	private void CloseWindow(Window w)
	{
		w.Close();
	}
    /// <summary>
    //　処理実行
    /// </summary>
    public async void RunMove()
	{
		
		if (FilePathItems.Count() > 0 && !string.IsNullOrEmpty(FilePathIteminTextBox.FilePath))
		{
            WindowStateMain.IsEnable = false;
			WindowStateMain.IsClosable = false;
            if (MessageBox.Show("実行しますか？", "確認", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
			{
				CancellToken = new CancellationTokenSource();
				var token = CancellToken.Token;
                
                var task = ProcessDataAsync(FilePathItems, FilePathItems.Count(),token);


                Progressbar newWindow = new Progressbar(this);
				newWindow.Closing += (s, e) => {
					if (customDialogIsOpen == true)
					{
						e.Cancel = true;
					}
					else
					{
						CancellToken.Cancel();
					}
				};
				
                newWindow.Show();


                try
				{
					await task;
					

                }
                catch (OperationCanceledException)
				{

					MessageBox.Show("処理がキャンセルされました");
                    WindowStateMain.IsEnable = true;
                    WindowStateMain.IsClosable = true;


				}
				finally
				{
                    FilePathItems.Clear();
                    FilePathIteminTextBox.FilePath = string.Empty;
                    state.IsRunnable = false;
                    this.AbortItem.Item = "移動元のファイル・フォルダを選択してください(複数選択可)(ドラッグ&ドロップ可)";
                    this.AbortItem2.Item = "移動先となるフォルダを選択してください(ドラッグ&ドロップ可)";
                    newWindow.Closed += (s, e) =>
                    {
                        WindowStateMain.IsEnable = true;
                        WindowStateMain.IsClosable = true;
                    };
                }


            }
			else{

			}
        }
		else
		{
			MessageBox.Show("移動元・移動先を選択してから実行してください");
		}


	}
    /// <summary>
    //　判定系処理
    /// </summary>

	public bool isSysFile(string path)
	{
        foreach (string sysfolder in Sysfolder)
        {
            Match match = Regex.Match(path + @"\", sysfolder);
            if (match.Success)
            {
                return true;
            }
        }
        FileInfo filinfo = new FileInfo(path);
		if ((filinfo.Attributes & FileAttributes.System) == FileAttributes.System)
		{
			return true;
		}
		else if (isReadOnly(path))
		{
			return true;
		}
		else if (isUnVisible(path))
		{
			return true;
		}
		else{
			return false;
		}
    }
	public bool isReadOnly(string path)
	{

		FileInfo filinfo = new FileInfo(path);
		if ((filinfo.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly){
			return true;
		}
		else
		{
			return false;
		}
	}
    public bool isUnVisible(string path)
    {
        FileInfo filinfo = new FileInfo(path);
        if ((filinfo.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
	public bool isContain(ObservableCollection<Item> o,string s)
	{
		foreach (Item item in o)
		{
			if (item.IsFolder == true)
			{
				if (((s + "\\").StartsWith(item.FilePath + "\\", StringComparison.OrdinalIgnoreCase)))
				{
					return true;
				}
			}
		}
		return false;
	}

    /// <summary>
    /// プロパティチェンジの検知
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

	protected void OnPropertyChanged(string propertyName)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
    /// <summary>
    /// 非同期処理の実装
    /// </summary>
	public async Task ProcessDataAsync(ObservableCollection<Item> Item,int ItemCount,CancellationToken token)
	{
		int NowItem = 0;
		string fpath = FilePathIteminTextBox.FilePath;
		NowProcessing.Progress = 0;
        NowProcessing.CanFinish = false;
        NowProcessing.CanStop = true;
        ObservableCollection<Item> OrderedItem = new ObservableCollection<Item>(Item.OrderByDescending(n => n.FilePath));



        //テンポラリディレクトリを作成
        string TempDir = "C:\\EFM_temp";

        Directory.CreateDirectory(TempDir);

        foreach (var item in OrderedItem)
        {
			token.ThrowIfCancellationRequested();
			NowItem++;

            /// <summary>
            /// ファイルを移動する時の処理
            /// </summary>
            if (!item.IsFolder)
			{
				token.ThrowIfCancellationRequested();
				NowProcessing.Item = item.FilePath;
                string TempFile = TempDir + "\\" + Path.GetFileName(item.FilePath);
                string Moveto = fpath + "\\" + Path.GetFileName(item.FilePath);
                try
				{


                    //移動先に同名コンテンツがある場合の処理
                    if (File.Exists(Moveto)|| Directory.Exists(Moveto))
					{
						if (File.Exists(Moveto))
						{

                            bool overwrite = await ShowCustomDialogAsync(item.FilePath, Moveto);
                            if (overwrite)
							{

								if (isSysFile(Moveto) || isSysFile(item.FilePath))
								{
									//システムファイル例外
									throw new Exception( Moveto + "または" + item.FilePath + "はシステムファイルです。");
								}
								else if (isReadOnly(Moveto) || isReadOnly(item.FilePath))
								{
									//読み取り専用例外
									throw new Exception(Moveto + "または" + item.FilePath + "が読み込み専用になっています。");
								}
								else if (isUnVisible(Moveto) || isUnVisible(item.FilePath))
								{
									//非表示状態例外
									throw new Exception(Moveto + "または" + item.FilePath + "が非表示状態になっています。");
								}
								else
								{
									//元ファイルをテンポラリディレクトリに移動してからファイルを目的の場所へ移動
									File.Move(Moveto, TempFile);
									File.Move(item.FilePath, Moveto);
									File.Delete(TempFile);
								}
							}
						}
						//移動先に同名コンテンツがある場合の処理
						else if(Directory.Exists(Moveto))

                        {
                            bool overwrite = await ShowCustomDialogAsync(item.FilePath, Moveto);
                            if (overwrite)
                            {

                                if (isSysFile(Moveto) || isSysFile(item.FilePath))
                                {
                                    //システムファイル例外
                                    throw new Exception(Moveto + "または" + item.FilePath + "はシステムファイルです。");
                                }
                                else if (isReadOnly(Moveto) || isReadOnly(item.FilePath))
                                {
                                    //読み取り専用例外
                                    throw new Exception(Moveto + "または" + item.FilePath + "が読み込み専用になっています。");
                                }
                                else if (isUnVisible(Moveto) || isUnVisible(item.FilePath))
                                {
                                    //非表示状態例外
                                    throw new Exception(Moveto + "または" + item.FilePath + "が非表示状態になっています。");
                                }
                                else
                                {
                                    //元ファイルをテンポラリディレクトリに移動してからファイルを目的の場所へ移動
                                    Directory.Move(Moveto, TempFile);
                                    File.Move(item.FilePath, Moveto);
									Directory.Delete(TempFile);
                                }
                            }
                        }
					}
					else
					{
						//移動先にコンテンツがない場合の処理
                        if (isSysFile(item.FilePath))
                        {
                            throw new Exception(item.FilePath + "はシステムファイルです。");
                        }
						else if (isReadOnly(item.FilePath))
						{
                            throw new Exception(item.FilePath + "が読み込み専用になっています。");
                        }
                        else if (isUnVisible(item.FilePath))
                        {
                            throw new Exception(item.FilePath + "が非表示状態になっています。");
                        }
                        else
                        {


                            if (File.Exists(item.FilePath))
							{
								File.Move(item.FilePath, Moveto);
							}
							else
							{
                                Directory.Move(item.FilePath, Moveto);
                            }

							
                        }
                    }
					//アイテムをリストから削除して進行度を更新
                    FilePathItems.Remove(item);
                    NowProcessing.Progress = (int)(100 * NowItem / ItemCount);
                } catch (IOException exc)
				{

                    string directoryPath = AppDomain.CurrentDomain.BaseDirectory;
                    string filePath = Path.Combine(directoryPath, "log.txt");
                    
                    bool overwrite = await ShowNortifiDialogAsync(item.FilePath+"は移動できませんでした。原因を"+directoryPath+"log.txtに出力します。");
					if (File.Exists(TempFile))
					{
						File.Move(TempFile,Moveto);
					}
					else if(Directory.Exists(TempFile))
					{
						Directory.Move(TempFile, Moveto);
					}
                    DateTime dt = DateTime.Now;
                    //ログ出力
                    using (StreamWriter writer = new StreamWriter(filePath, true))
                    {
                        writer.WriteLine(dt + "\t:\t" + exc.Message);
                    }
                }
				catch (Exception e)
				{

                    string directoryPath = AppDomain.CurrentDomain.BaseDirectory;
					string filePath = Path.Combine(directoryPath, "log.txt");
                    bool overwrite = await ShowNortifiDialogAsync(item.FilePath + "は移動できませんでした。原因を" + directoryPath + "log.txtに出力します。");
                    if (File.Exists(TempFile))
                    {
                        File.Move(TempFile, Moveto);
                    }
                    else if (Directory.Exists(TempFile))
                    {
                        Directory.Move(TempFile, Moveto);
                    }
					DateTime dt = DateTime.Now;
                    //ログ出力
                    using (StreamWriter writer = new StreamWriter(filePath, true))
					{
						writer.WriteLine(dt + "\t:\t" +e.Message);
					}
				}

				token.ThrowIfCancellationRequested();
			}
			else
			{
				token.ThrowIfCancellationRequested();
                

                NowProcessing.Item = item.FilePath;
                /// <summary>
                /// ディレクトリの場合の処理 Mode0 フォルダを移動
                /// </summary>
                if (item.Mode == 0)
				{
                    string TempFile = TempDir + "\\" + Path.GetFileName(item.FilePath);
                    string Moveto = fpath + "\\" + Path.GetFileName(item.FilePath);
                    try
					{
						if (File.Exists(Moveto) || Directory.Exists(Moveto)) {

							if (File.Exists(Moveto))
							{

                                bool overwrite = await ShowCustomDialogAsync(item.FilePath, Moveto);
                                if (overwrite)
								{
									if (isSysFile(Moveto) || isSysFile(item.FilePath))
									{
                                        throw new Exception(Moveto + "または" + item.FilePath + "はシステムファイルです。");
                                    }
									else if (isReadOnly(Moveto) || isReadOnly(item.FilePath))
									{
                                        throw new Exception(Moveto + "または" + item.FilePath + "が読み込み専用になっています。");
                                    }
									else if (isUnVisible(Moveto) || isUnVisible(item.FilePath))
									{
                                        throw new Exception(Moveto + "または" + item.FilePath + "が非表示状態になっています。");
                                    }
									else
									{

										File.Move(Moveto, TempFile);
										Directory.Move(item.FilePath, Moveto);
										File.Delete(TempFile);
									}
								}
							}
							else
							{
                                bool overwrite = await ShowCustomDialogAsync(item.FilePath, Moveto);
                                if (overwrite)
                                {
                                    if (isSysFile(Moveto) || isSysFile(item.FilePath))
                                    {
                                        throw new Exception(Moveto + "または" + item.FilePath + "はシステムファイルです。");
                                    }
                                    else if (isReadOnly(Moveto) || isReadOnly(item.FilePath))
                                    {
                                        throw new Exception(Moveto + "または" + item.FilePath + "が読み込み専用になっています。");
                                    }
                                    else if (isUnVisible(Moveto) || isUnVisible(item.FilePath))
                                    {
                                        throw new Exception(Moveto + "または" + item.FilePath + "が非表示状態になっています。");
                                    }
                                    else
                                    {
										Directory.Move(Moveto, TempFile);
                                        Directory.Move(item.FilePath, Moveto);
										Directory.Delete(TempFile);
                                    }
                                }
                            }
						}
						else
						{

                            if (isSysFile(item.FilePath))
                            {
                                throw new Exception(item.FilePath + "はシステムファイルです。");
                            }
                            else if (isReadOnly(item.FilePath))
                            {
                                throw new Exception(item.FilePath + "が読み込み専用になっています。");
                            }
                            else if (isUnVisible(item.FilePath))
                            {
                                throw new Exception(item.FilePath + "が非表示状態になっています。");
                            }
                            else
                            {
								
                                Directory.Move(item.FilePath, Moveto);
                            }
                        }
					}
					catch (IOException exc)
					{
                        string directoryPath = AppDomain.CurrentDomain.BaseDirectory;
                        string filePath = Path.Combine(directoryPath, "log.txt");
                        bool overwrite = await ShowNortifiDialogAsync(item.FilePath + "は移動できませんでした。原因を" + directoryPath + "log.txtに出力します。");
                        if (File.Exists(TempFile))
                        {
                            File.Move(TempFile, Moveto);
                        }
                        else if (Directory.Exists(TempFile))
                        {
                            Directory.Move(TempFile, Moveto);
                        }

                        DateTime dt = DateTime.Now;

                        using (StreamWriter writer = new StreamWriter(filePath, true))
                        {
                            writer.WriteLine(dt + "\t:\t" + exc.Message);
                        }
                    }
					catch (Exception e)
					{
                        string directoryPath = AppDomain.CurrentDomain.BaseDirectory;
                        string filePath = Path.Combine(directoryPath, "log.txt");
                        bool overwrite = await ShowNortifiDialogAsync(item.FilePath + "は移動できませんでした。原因を" + directoryPath + "log.txtに出力します。");
                        if (File.Exists(TempFile))
                        {
                            File.Move(TempFile, Moveto);
                        }
                        else if (Directory.Exists(TempFile))
                        {
                            Directory.Move(TempFile, Moveto);
                        }
                        DateTime dt = DateTime.Now;
                        using (StreamWriter writer = new StreamWriter(filePath, true))
						{
                            writer.WriteLine(dt + "\t:\t" + e.Message);
                        }
					}
					FilePathItems.Remove(item);
                    NowProcessing.Progress = (int)(100 * NowItem / ItemCount);
                    token.ThrowIfCancellationRequested();
                }
                /// <summary>
                /// ディレクトリの場合の処理 Mode1,2 フォルダ内部のファイルを移動
                /// </summary>
                else if (item.Mode == 1 || item.Mode == 2)
				{

					foreach (string file in Directory.GetFileSystemEntries(item.FilePath))
					{
						NowProcessing.Item = file;
						bool isfile =false;
						bool isdir = false;
                        string TempFile = TempDir + "\\" + Path.GetFileName(file);
                        string Moveto = fpath + "\\" + Path.GetFileName(file);
                        isfile = (File.Exists(file));
                        isdir = (Directory.Exists(file));
                        try
						{
							if (isfile)
							{
								if (Directory.Exists(Moveto) || File.Exists(Moveto))
								{
                                    bool overwrite = await ShowCustomDialogAsync(file, Moveto);
                                    if (overwrite)
									{
										if (isSysFile(Moveto) || isSysFile(file))
										{
                                            throw new Exception(Moveto + "または" + file + "はシステムファイルです。");
                                        }
										else if (isReadOnly(Moveto) || isReadOnly(file))
										{
                                            throw new Exception(Moveto + "または" + file + "が読み込み専用になっています。");
                                        }
										else if (isUnVisible(Moveto) || isUnVisible(file))
										{
                                            throw new Exception(Moveto + "または" + file + "が非表示状態になっています。");
                                        }
										else
										{
                                            if (File.Exists(Moveto))
                                            {
												File.Move(Moveto, TempFile);
												File.Move(file, Moveto);
                                                File.Delete(TempFile);
                                            }
                                            else
                                            {
												Directory.Move(Moveto, TempFile);
                                                Directory.Move(file, Moveto);
												Directory.Delete(TempFile);
                                            }
                                        }
									}
								}
								else
								{
									if (isSysFile(file))
									{
                                        throw new Exception(file + "はシステムファイルです。");
                                    }
									else if (isReadOnly(file))
									{
                                        throw new Exception(file + "が読み込み専用になっています。");
                                    }
									else if (isUnVisible(file))
									{
                                        throw new Exception(file + "が非表示状態になっています。");
                                    }
									else
									{
										File.Move(file, Moveto);
									}
								}
							}
							else if(isdir)
							{
                                if (Directory.Exists(Moveto) || File.Exists(Moveto))
                                {
                                    bool overwrite = await ShowCustomDialogAsync(file, Moveto);
                                    
                                    if (overwrite)
                                    {
                                        if (isSysFile(Moveto) || isSysFile(file))
                                        {
                                            throw new Exception(Moveto + "または" + file + "はシステムファイルです。");
                                        }
                                        else if (isReadOnly(Moveto) || isReadOnly(file))
                                        {
                                            throw new Exception(Moveto + "または" + file + "が読み込み専用になっています。");
                                        }
                                        else if (isUnVisible(Moveto) || isUnVisible(file))
                                        {
                                            throw new Exception(Moveto + "または" + file + "が非表示状態になっています。");
                                        }
                                        else
                                        {
											if (File.Exists(Moveto))
											{
                                                File.Move(Moveto, TempFile);
												Directory.Move(file, Moveto);
                                                File.Delete(TempFile);
                                            }
											else
											{
                                                Directory.Move(Moveto, TempFile);
                                                Directory.Move(file, Moveto);
                                                Directory.Delete(TempFile);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (isSysFile(file))
                                    {
                                        throw new Exception(file + "はシステムファイルです。");
                                    }
                                    else if (isReadOnly(file))
                                    {
                                        throw new Exception(file + "が読み込み専用になっています。");
                                    }
                                    else if (isUnVisible(file))
                                    {
                                        throw new Exception(file + "が非表示状態になっています。");
                                    }
                                    else
                                    {
										if (File.Exists(file))
										{
											File.Move(file, Moveto);
										}
										else
										{
											Directory.Move(file, Moveto);
										}
                                    }
                                }
                            }
							else
							{
								throw new IOException();
							}
						}
						catch (IOException exc)
						{
                            string directoryPath = AppDomain.CurrentDomain.BaseDirectory;
                            string filePath = Path.Combine(directoryPath, "log.txt");
                            bool overwrite = await ShowNortifiDialogAsync(file+ "は移動できませんでした。原因を" + directoryPath + "log.txtに出力します。");

                            if (File.Exists(TempFile))
                            {
                                File.Move(TempFile, Moveto);
                            }
                            else if (Directory.Exists(TempFile))
                            {
                                Directory.Move(TempFile, Moveto);
                            }
                            DateTime dt = DateTime.Now;
                            using (StreamWriter writer = new StreamWriter(filePath, true))
                            {
                                writer.WriteLine(dt + "\t:\t" + exc.Message);
                            }
                        }
                        catch (Exception e)
						{
                            string directoryPath = AppDomain.CurrentDomain.BaseDirectory;
                            string filePath = Path.Combine(directoryPath, "log.txt");
                            bool overwrite = await ShowNortifiDialogAsync(file + "は移動できませんでした。原因を" + directoryPath + "log.txtに出力します。");

                            if (File.Exists(TempFile))
                            {
                                File.Move(TempFile, Moveto);
                            }
                            else if (Directory.Exists(TempFile))
                            {
                                Directory.Move(TempFile, Moveto);
                            }
                            DateTime dt = DateTime.Now;
                            using (StreamWriter writer = new StreamWriter(filePath, true))
                            {
                                writer.WriteLine(dt + "\t:\t" + e.Message);
                            }
                        }
                        FilePathItems.Remove(item);
                        NowProcessing.Progress = (int)(100 * NowItem / ItemCount);
                        token.ThrowIfCancellationRequested();

                    }
                    /// <summary>
                    ///　フォルダを移動後に削除
                    /// </summary>
                    if (item.Mode == 2)
					{

						if(Directory.GetFileSystemEntries(item.FilePath).Count() > 0)
						{
                            bool overwrite = await ShowNortifiDialogAsync("フォルダが空ではないため削除できません。");
                        }
						else
						{
							try
							{
                                token.ThrowIfCancellationRequested();
								if (isReadOnly(item.FilePath)){
                                    bool overwrite = await ShowNortifiDialogAsync("読み取り専用のため削除できません。");

								}
								else if (isSysFile(item.FilePath)) {
                                    bool overwrite = await ShowNortifiDialogAsync("システムフォルダのため削除できません。");

								} else if (isUnVisible(item.FilePath)) {
                                    bool overwrite = await ShowNortifiDialogAsync("隠しフォルダのため削除できません。");
                                }
								else
								{
                                    Directory.Delete(item.FilePath);
                                }
                            }
							catch (IOException exc)
							{
                                bool overwrite = await ShowNortifiDialogAsync(item.FilePath+"は削除出来ませんでした。");
                                string directoryPath = AppDomain.CurrentDomain.BaseDirectory;

                                string filePath = Path.Combine(directoryPath, "log.txt");

                                using (StreamWriter writer = new StreamWriter(filePath, true))
                                {
                                    writer.WriteLine(exc.Message);
                                }
                            }catch (Exception e)
							{
                                bool overwrite = await ShowNortifiDialogAsync(item.FilePath + "は削除出来ませんでした。");
                                string directoryPath = AppDomain.CurrentDomain.BaseDirectory;

                                string filePath = Path.Combine(directoryPath, "log.txt");

                                using (StreamWriter writer = new StreamWriter(filePath, true))
                                {
                                    writer.WriteLine(e.Message);
                                }
                            }
                            token.ThrowIfCancellationRequested();
                        }
					}
				}
				else
				{

				}
			
            }
            token.ThrowIfCancellationRequested();
            await Task.Delay(500);
            FilePathIteminTextBox.FilePath = string.Empty;
        }
		NowProcessing.Item = "処理が完了しました。";
		NowProcessing.CanFinish = true;
		NowProcessing.CanStop = false;
    }
	private Task<bool> ShowCustomDialogAsync(string sourcePath, string destinationPath)
	{
		var tcs = new TaskCompletionSource<bool>();

		Application.Current.Dispatcher.BeginInvoke(new Action(() =>
		{
			var dialog = new CustomDialog
			{
				Message = $"同名の"+ (File.Exists(destinationPath)? "ファイル" : "フォルダ") +$"があります。上書きしますか？\r\n【移動元】{sourcePath}\r\n【移動先】{destinationPath}"
			};
			dialog.ResultSubmitted += (result) =>
			{
				tcs.SetResult(result);
			};


			customDialogIsOpen = true;
            dialog.Closed += (s, e) => { customDialogIsOpen = false; };
            dialog.Show();
            dialog.Activate();
            dialog.Topmost = true;


        }));
		return tcs.Task; 
	}
    private Task<bool> ShowNortifiDialogAsync(string text)
    {
        var tcs = new TaskCompletionSource<bool>();

        Application.Current.Dispatcher.BeginInvoke(new Action(() =>
        {
			var dialog = new NortifiWindow
			{
				Message = text
			};
            dialog.Closed += (sender, args) =>
            {
                tcs.SetResult(dialog.DialogResult == true);
            };

			dialog.ResutlReceived += Nortifi_FormClosed!;
            dialog.Closed += (s, e) => { customDialogIsOpen = false; };
			customDialogIsOpen =true;
            dialog.Show();
            dialog.Activate();
            dialog.Topmost = true;


        }));
        return tcs.Task;
    }
    private void WindowCloseMangage(object parameter)
	{
        if (parameter is Window window)
		{
			if(window != null && window.Title == "EasyFileMover")
			{
				if (WindowStateMain.IsClosable == false)
				{
					return;
				}
				else
				{
					Application.Current.Shutdown();
                }

			}
			else if(window != null && window.Title == "進行率")
			{
                return;
            }
			else if(window != null) 
            {
                window.Close();
				
			}
            else
            {
				return;
			}
		}
    }
	private void CustomDialog_FormClosed(object sender,bool result)
	{
		
	}
    private void Nortifi_FormClosed(object sender, bool result)
    {

    }

}

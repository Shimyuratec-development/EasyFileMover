using System;
using System.ComponentModel;
using System.Windows;
/// <summary>
/// ファイルパスを定義するモデル
/// </summary>
public class Item : INotifyPropertyChanged
{
	private string? _FilePath;
	public string FilePath {
        get { return _FilePath!; }
        set
        {
            if (_FilePath != value)
            {
                _FilePath = value;
                OnPropertyChanged(nameof(FilePath));

            }
        }
    }
    private Boolean _IsFolder;
    public Boolean IsFolder
    {
        get { return _IsFolder; }
        set
        {
            if (_IsFolder != value)
            {
                _IsFolder = value;
                OnPropertyChanged(nameof(IsFolder));
            }
        }
    }
    private int _Mode;
    public int Mode
    {
        get { return _Mode; }
        set
        {
            if (_Mode != value)
            {
                _Mode = value;
                OnPropertyChanged(nameof(Mode));
                HandleSelectionChanged(_Mode);
            }
        }
    }
    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged(string v)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(v));
    }

    private void HandleSelectionChanged(int mode)
    {

    }
}

using System;
using System.ComponentModel;
using System.Windows;
/// <summary>
/// 起動可能状態を管理する
/// </summary>
public class WindowState : INotifyPropertyChanged
{
	private Boolean _IsEnable;
	public Boolean IsEnable {
        get { return _IsEnable; }
        set
        {
            if (_IsEnable != value)
            {
                _IsEnable = value;
                OnPropertyChanged(nameof(IsEnable));

            }
        }
    }
    private Boolean _IsClosable;
    public Boolean IsClosable
    {
        get { return _IsClosable; }
        set
        {
            if (_IsClosable != value)
            {
                _IsClosable = value;
                OnPropertyChanged(nameof(IsClosable));

            }
        }
    }
    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged(string v)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(v));
    }
}

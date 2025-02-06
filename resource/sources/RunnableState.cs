using System;
using System.ComponentModel;
using System.Windows;
/// <summary>
/// 起動可能状態を管理する
/// </summary>
public class RunnableState : INotifyPropertyChanged
{
	private Boolean _IsRunnable;
	public Boolean IsRunnable {
        get { return _IsRunnable; }
        set
        {
            if (_IsRunnable != value)
            {
                _IsRunnable = value;
                OnPropertyChanged(nameof(IsRunnable));

            }
        }
    }
  
    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged(string v)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(v));
    }
}

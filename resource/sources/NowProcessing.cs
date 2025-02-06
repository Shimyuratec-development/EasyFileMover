using System;
using System.ComponentModel;
using System.Diagnostics;
/// <summary>
/// 処理状態を管理するモデル
/// </summary>
public class NowProcessing : INotifyPropertyChanged
{
	private String? item;
	public string Item
	{
		get { return item!; }
		set
		{
			if (item != value)
			{
				item = value;
				OnPropertyChanged(nameof(Item));

			}
		}
	}
	private int _Progress;
	public int Progress
	{
		get { return _Progress; }
		set
		{
			if (_Progress != value)
			{
				_Progress = value;
				OnPropertyChanged(nameof(Progress));
			}
		}
	}
	private Boolean _CanFinish;
	public Boolean CanFinish
	{
		get { return _CanFinish; }
		set
		{
			if (_CanFinish != value)
			{
				_CanFinish = value;
				OnPropertyChanged(nameof(CanFinish));
			}
		}
	}
    private Boolean _CanStop;
    public Boolean CanStop
    {
        get { return _CanStop; }
        set
        {
            if (_CanStop != value)
            {
                _CanStop = value;
                OnPropertyChanged(nameof(CanStop));
            }
        }
    }


    public event PropertyChangedEventHandler? PropertyChanged;


    protected virtual void OnPropertyChanged(string v)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(v));
    }
}

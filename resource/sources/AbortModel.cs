using System;
using System.ComponentModel;
using System.Diagnostics;
/// <summary>
/// アボート出力を定義するモデル
/// </summary>
public class AbortItem : INotifyPropertyChanged
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


	public event PropertyChangedEventHandler? PropertyChanged;


    protected virtual void OnPropertyChanged(string v)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(v));
    }
}

using System;

public interface ISaveable
{
    public event EventHandler<EventArgs> OnSaveEvent;
    public void Save();
}

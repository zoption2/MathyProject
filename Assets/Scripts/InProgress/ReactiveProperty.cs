using System;

public class ReactiveProperty<T> : IDisposable
{
    public event Action<T> ON_VALUE_CHANGED;

    private T value;
    public T Value
    {
        get { return value; }
        set 
        {
            if(!Equals(value, this.value))
            {
                this.value = value;
                ON_VALUE_CHANGED?.Invoke(value);
            }
        }
    }

    public ReactiveProperty(T initValue)
    {
        value = initValue;
    }

    public void Dispose()
    {
        ON_VALUE_CHANGED = null;
        value = default;
    }
}

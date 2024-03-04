using System;

namespace APGMPCSSIntegration.Constant.Helpers;

public sealed class WriteOnce<T>
{
    private T value;
    private bool hasValue;
    
    public override string ToString() => value?.ToString();

    /// <summary>
    /// 
    /// </summary>
    /// <exception cref="InvalidOperationException">When setting a value that is already set
    /// OR reading a value that has not been set.</exception>
    public T Value
    {
        get
        {
            if (hasValue == false)
                throw new InvalidOperationException("Value not set");
            return value;
        }
        set
        {
            if (hasValue)
                throw new InvalidOperationException("Value already set");
            this.value = value;
            this.hasValue = true;
        }
    }
    public T ValueOrDefault => value;

    public static implicit operator T(WriteOnce<T> value) { return value.Value; }
}
namespace ContentHider.Core.Functions;

public class RequestValidationResult<T> where T : class
{
    private T _value;
    
    public T Value
    {
        get => _value ?? throw new Exception();

        set => _value = value;
    }
}
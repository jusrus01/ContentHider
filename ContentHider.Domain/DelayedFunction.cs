using System.Diagnostics.CodeAnalysis;

namespace ContentHider.Domain;

public class DelayedFunction
{
    private protected class DelayedFunctionNode
    {
        public Func<object, object> F { get; set; }
    }
    
    private protected DelayedFunction Next { get; private set; }
    private protected DelayedFunction Head { get; private set; }


    private readonly object _obj;
    private protected readonly DelayedFunctionNode FunctionNode;

    public DelayedFunction(object obj)
    {
        _obj = obj;
        
        Head = this;
    }
    
    private DelayedFunction(DelayedFunction head, DelayedFunctionNode functionNode)
    {
        Head = head;
        FunctionNode = functionNode;
    }

    public TResult ExecuteAsync<TResult>()
    {
        var current = Head;
        var arg = Head._obj;
        
        while (current != null)
        {
            if (current.FunctionNode == null)
            {
                current = current.Next;
                continue;
            }
            
            var result = current.FunctionNode.F(arg);
            
            current = current.Next;
            arg = result;
        }
        
        return (TResult)arg;
    }

    public DelayedFunction Map<TFrom, TTo>(Func<TFrom, TTo> mapFunc)
    {
        var node = new DelayedFunctionNode
        {
            F = requestObject =>
            {
                return mapFunc((TFrom)requestObject);
            }
        };
        
        Next = new DelayedFunction(Head, node);
        return Next;
    }
    
    public DelayedFunction Validate<TRequestObject>(Func<TRequestObject, bool> validationFunc)
    {
        var node = new DelayedFunctionNode
        {
            F = requestObject =>
            {
                var isValid = validationFunc((TRequestObject)requestObject);
                if (!isValid)
                {
                    throw new Exception();
                }

                return requestObject;
            }
        };
        
        Next = new DelayedFunction(Head, node);
        return Next;
    }
}
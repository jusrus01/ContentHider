using System;
using System.Threading.Tasks;
using ContentHider.Core.Repositories;

namespace ContentHider.Domain;

public class DelayedFunction
{
    private protected class DelayedFunctionNode
    {
        public Func<object, Task<object>> F { get; set; }
    }
    
    private protected DelayedFunction Next { get; private set; }
    private protected DelayedFunction Head { get; private set; }


    private object _obj;
    private protected readonly DelayedFunctionNode FunctionNode;
    
    private readonly IUnitOfWork _uow;

    public DelayedFunction(IUnitOfWork uow)
    {
        _uow = uow;
    }
    
    // public DelayedFunction(object obj)
    // {
    //     _obj = obj;
    //     
    //     Head = this;
    // }
    
    private DelayedFunction(DelayedFunction head, DelayedFunctionNode functionNode)
    {
        Head = head;
        FunctionNode = functionNode;
    }

    public DelayedFunction Begin(object obj)
    {
        _obj = obj;
        
        Head = this;

        return this;
    }
    
    public async Task<TResult> ExecuteAsync<TResult>()
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
            
            var result = await current.FunctionNode.F(arg).ConfigureAwait(false);
            
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
                return Task.FromResult<object>(mapFunc((TFrom)requestObject));
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

                return Task.FromResult(requestObject);
            }
        };
        
        Next = new DelayedFunction(Head, node);
        return Next;
    }
}
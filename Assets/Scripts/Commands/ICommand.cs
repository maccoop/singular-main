using System;
using Cysharp.Threading.Tasks;
using TriLibCore;

namespace Commands
{
    public interface ICommand
    {
        UniTask ExecuteAsync();
    }
    
    public interface ICommand<T>
    {
        T Result { get; }
        UniTask<T> ExecuteAsync();
    }
}
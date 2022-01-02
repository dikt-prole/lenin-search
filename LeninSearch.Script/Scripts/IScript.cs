using System.Collections.Generic;

namespace LeninSearch.Script.Scripts
{
    public interface IScript
    {
        string Id { get; }
        string Arguments { get; }
        void Execute(params string[] input);
    }
}
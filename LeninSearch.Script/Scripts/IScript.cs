namespace LeninSearch.Script.Scripts
{
    public interface IScript
    {
        string Id { get; }
        void Execute(params string[] input);
    }
}
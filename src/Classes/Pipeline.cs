using System.Collections;

namespace CodeCrafters.Shell.Classes;

public class Pipeline(IEnumerable<PipelineStage> stages) : IReadOnlyList<PipelineStage>
{
    private readonly PipelineStage[] _stages = stages.ToArray();

    public int Count => _stages.Length;
    public PipelineStage this[int index] => _stages[index];

    public IEnumerator<PipelineStage> GetEnumerator()
        => ((IEnumerable<PipelineStage>)_stages).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
    
    public bool BackgroundJob { get; init; }
    public required string Invocation { get; init; }
}
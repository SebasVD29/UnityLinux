using UnityEngine;

public interface ICounterable 
{

    public bool CanBeParry {  get; }
    public bool CanBePerfectParry {  get; }
    public void HandleCounter();
    public void HandlePerfectCounter();
}

using UnityEngine;

[CreateAssetMenu(
    fileName = "IdManager",
    menuName = "ScriptableObjects/IdManagerScriptableObject")]
public class IdManagerScriptableObject : ScriptableObject
{
    [SerializeField]
    private int _nextId = 0;

    public int NextId
    {
        get => _nextId++;
        set => _nextId = value;
    }
}

using System;

public class SpecialNodeInstance
{
    public string InstanceID { get; private set; }
    public SpecialNodeData Data { get; private set; }

    public SpecialNodeInstance(SpecialNodeData data) 
    {
        Data = data;
        InstanceID = Guid.NewGuid().ToString();
    }
    public SpecialNodeInstance(SpecialNodeData data, SpecialNodeSaveData save)
    {
        Data = data;
        InstanceID = string.IsNullOrEmpty(save.instanceID) 
            ?Guid.NewGuid().ToString()
            : save.instanceID;
        
    }
    public SpecialNodeSaveData ToSave() => new SpecialNodeSaveData
    {
        nodeID = Data.nodeId,
        instanceID = InstanceID,

    };

}

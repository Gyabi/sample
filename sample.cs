using System.Collections.Generic;
using UnityEngine;

public class IDAssigner {

    private int maxID;
    private List<int> assignedIDs;
    private List<int> unassignedIDs;
    private Dictionary<int, int> instanceIDToID;
    private HashSet<int> previousFrameInstances;

    public IDAssigner(int maxID) {
        this.maxID = maxID;
        assignedIDs = new List<int>();
        unassignedIDs = new Queue<int>();
        instanceIDToID = new Dictionary<int, int>();
        previousFrameInstances = new HashSet<int>();

        for (int i = 0; i < maxID; i++) {
            unassignedIDs.Add(i);
        }
    }

    public void RegisterID(int[] instances) {
        HashSet<int> currentFrameInstances = new HashSet<int>();
        for (int i = 0; i < instances.Length; i++) {
            int instance = instances[i];
            currentFrameInstances.Add(instance);
            if (instanceIDToID.ContainsKey(instance)) {
                continue;
            }
            else if(unassignedIDs.Length != 0){
                int newID = unassignedIDs.Min();
                unassignedIDs.Remove(newID);
                assignedIDs.Add(newID);
                instanceIDToID.Add(instance, newID);
            }
        }
        HashSet<int> removedInstances = new HashSet<int>(previousFrameInstances);
        removedInstances.ExceptWith(currentFrameInstances);
        foreach (int removedInstance in removedInstances) {
            RemoveID(removedInstance);
        }
        previousFrameInstances = currentFrameInstances;
    }

    public int GetID(int instance) {
        if (instanceIDToID.ContainsKey(instance)) {
            return instanceIDToID[instance];
        }
        else {
            return -1;
        }
    }

    public void RemoveID(int instance) {
        if (instanceIDToID.ContainsKey(instance)) {
            unassignedIDs.Add(instanceIDToID[instance]);
            assignedIDs.Remove(instanceIDToID[instance]);
            instanceIDToID.Remove(instance);
        }
    }
}

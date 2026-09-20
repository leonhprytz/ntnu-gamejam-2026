using UnityEngine;

namespace QuestLines
{
    public class IntroBonfireQuest : InteractionCheckpoint
    {
        public BonfireManager bonfireManager;
        
        public override void Interact()
        {
            bonfireManager.size = BonfireSize.Medium;
        }
    }
}


using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Quest/Quest")]

public class Quest : ScriptableObject
{
    
    public string questID;
    public string questName;
    public string description;
    public List<QuestObjetive> objetives;
    public List<QuestReward> questRewards;

    private void OnValidate()
    {
        if(string.IsNullOrEmpty(questID))
        {
            questID = questName + Guid.NewGuid().ToString();
        }
    }

    [System.Serializable]

    public class QuestObjetive
    {
        public string objetiveID;// si coicide con el ID a encontrar 
        public string description;
        public ObjectiveType type;
        public int requiredAmount;
        public int currentAmount;

        public bool IsCompleted => currentAmount >= requiredAmount;
    }

    public enum ObjectiveType { Collection,DefeatEnemy, ReachLocation,TalkNPC,Custom}


    [System.Serializable]
    public class QuestProgress
    {
        public Quest quest;
        public List<QuestObjetive> objetives;

        public QuestProgress(Quest quest)
        {
            this.quest = quest;
            objetives = new List<QuestObjetive>();

            foreach(var obj in quest.objetives)
            {
                objetives.Add(new QuestObjetive
                {
                    objetiveID = obj.objetiveID,
                    description = obj.description,
                    type = obj.type,
                    requiredAmount = obj.requiredAmount,
                    currentAmount = 0
                });
            }
        }

        public bool IsCompleted => objetives.TrueForAll(o => o.IsCompleted);

        public string QuestID => quest.questID;


    }


}

[System.Serializable]
public class QuestReward
{
    public RewardType type;
    public int rewardID; //itemID etc
    public int amount = 1;
}

public enum RewardType { Item,Gold,Experience,Custom}

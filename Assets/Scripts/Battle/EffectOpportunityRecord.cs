using System.Collections.Generic;

public class EffectOpportunityRecord
{
    public Dictionary<string, int> record = new();

    public void Add(string opportunity)
    {
        if (record.ContainsKey(opportunity))
        {
            record[opportunity] += 1;
        }
        else
        {
            record.Add(opportunity, 1);
        }
    }

    public void Remove(string opportunity)
    {
        if (record.ContainsKey(opportunity))
        {
            record[opportunity] -= 1;

            if (record[opportunity] < 1)
            {
                record.Remove(opportunity);
            }
        }
    }
}

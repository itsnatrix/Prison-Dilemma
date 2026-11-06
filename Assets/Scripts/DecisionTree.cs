using System;
using System.Collections.Generic;
using System.Linq; 

[Serializable]
public class SavedExample{
    public string contradicts;
    public string detail;
    public string selfMentioned;
    public string confess;
    public string result;

    public string Get(string n){
        switch(n){
        case "contradicts": return contradicts;
        case "detail" : return detail;
        case "selfMentioned": return selfMentioned;
        case "confess" : return confess;
        default: throw new ArgumentException("Unknown attribute: " + n);

        }
    }
}

[Serializable]
public class ExampleList{
    public List<SavedExample> examples = new List<SavedExample>();
}

 public abstract class Node{
    public abstract string Classify(SavedExample situation);
}

public class Leaf : Node{
    public string ai;
    public override string Classify(SavedExample s) => ai;
}


public class Split : Node{
    public string Attribute;
    public Dictionary<string, Node> Children = new Dictionary<string, Node>();
    public string Default;

    public override string Classify(SavedExample s){
        return Children.TryGetValue(s.Get(Attribute), out Node child)
            ? child.Classify(s)
            : Default;
    }
}

    
public static class ID3{
    static double Entropy(List<SavedExample> examples){
        return examples
            .GroupBy(e => e.result)
            .Sum(g => {
                double p = (double)g.Count() / examples.Count;
                return -p * Math.Log(p, 2);
            });
    }

    static double InfoGain(List<SavedExample> examples, string attribute){
        double after = examples
        .GroupBy(e => e.Get(attribute))
        .Sum(g => (double)g.Count() / examples.Count * Entropy(g.ToList()));
        return Entropy(examples) - after;
    }

    static string Majority(List<SavedExample> examples) =>
        examples.GroupBy(e => e.result)
        .OrderByDescending(g => g.Count())
        .First().Key;

    public static Node Build(List<SavedExample> examples, List<string> attributes){
        if (examples.Select(e => e.result).Distinct().Count() == 1)
            return new Leaf { ai = examples[0].result };

        if (attributes.Count == 0){
            return new Leaf { ai = Majority(examples) };
        }

        string best = attributes.OrderByDescending(a => InfoGain(examples, a)).First();

        var node = new Split { Attribute = best, Default = Majority(examples) };
        var remaining = attributes.Where(a => a != best).ToList();

        foreach (var group in examples.GroupBy(e => e.Get(best))){
            node.Children[group.Key] = Build(group.ToList(), remaining);
        }

        return node;
    }
}
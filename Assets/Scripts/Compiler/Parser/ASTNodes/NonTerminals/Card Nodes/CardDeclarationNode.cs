using System;
using System.Collections.Generic;
#nullable enable

namespace Compiler
{
    public class CardDeclarationNode : StatementNodes
    {
        public StringNode Name { get; set; }
        public StringNode Type { get; set; }
        public StringNode Faction { get; set; }
        public NumberNode Power { get; set; }
        public StringNode[] Ranges { get; set; }
        public OnActivationNode OnActivation { get; set; }

        public StringNode? artName { get; set; }
        public StringNode? description { get; set;}

        public CardDeclarationNode(StringNode name, StringNode type, StringNode faction, NumberNode power, StringNode[] ranges, OnActivationNode onActivation, StringNode? artName, StringNode? description)
        {
            this.Name = name;
            this.Type = type;
            this.Faction = faction;
            this.Power = power;
            this.Ranges = ranges;
            this.OnActivation = onActivation;
            this.artName = artName;
            this.description = description;
        }

        public override IEnumerable<ASTNode> GetChildren()
        {
            yield return Name;
            yield return Type;
            yield return Faction;
            yield return Power;
            foreach (var range in Ranges)
            {
                yield return range;
            } 
            yield return  OnActivation;
        }
    }
}
<Program> ::= <Card> 
            | <Effect>

<Card> ::= "card" "{" <Attributes> "}"

<Attributes> ::= <Attribute> 
                |<Attribute> "," <Attributes>

<Attribute> ::= "Type" ":" <String>
		   | "Name" ":" <String>
		   | "Faction" ":" <String>
		   | "Power" ":" <Number>
		   | "Range" ":" "[" <Ranges> "]"
		   | "OnActivation" ":" "[" <Actions> "]"

<Ranges> ::= <Range> 
            |<Range> "," <Ranges>

<Range> ::= <String>

<Actions> ::= <Action> 
            |<Action "," <Actions>

<Action> ::= "{" <Effect> "," <Selector> "," <PostAction> "}"

<Effect> ::= "Effect" ":" "{" "Name" ":" <String> "," 
                            "Amount" ":" <Number> "}"

<Selector> ::= "Selector" ":" "{" "Source" ":" <String> ","
                                  "Single" ":" <Boolean> "," 
                               "Predicate" ":" "(" <Unit> ")" "}"

<PostAction> ::= "PostAction" ":" "{" "Type" ":" <String> ","
                 "Selector" ":" "{" "Source" ":" <String> "," 
                                    "Single" ":" <Boolean> ","
                                 "Predicate" ":" "(" <Unit> ")" "}" "}"

<Unit> ::= <String>

<Number> ::= [0-9]+

<String> ::= '"' [a-zA-Z0-9\s]+ '"'

<Boolean> ::= "true" 
            | "false"
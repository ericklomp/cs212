using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace $safeprojectname$
{
    class Program
    {
        private static RelationshipGraph rg;

        // Read RelationshipGraph whose filename is passed in as a parameter.
        // Build a RelationshipGraph in RelationshipGraph rg
        private static void ReadRelationshipGraph(string filename)
        {
            rg = new RelationshipGraph();                           // create a new RelationshipGraph object

            string name = "";                                       // name of person currently being read
            int numPeople = 0;
            string[] values;
            Console.Write("Reading file " + filename + "\n");
            try
            {
                string input = System.IO.File.ReadAllText(filename);// read file
                input = input.Replace("\r", ";");                   // get rid of nasty carriage returns 
                input = input.Replace("\n", ";");                   // get rid of nasty new lines
                string[] inputItems = Regex.Split(input, @";\s*");  // parse out the relationships (separated by ;)
                foreach (string item in inputItems) 
		{
                    if (item.Length > 2)                            // don't bother with empty relationships
                    {
                        values = Regex.Split(item, @"\s*:\s*");     // parse out relationship:name
                        if (values[0] == "name")                    // name:[personname] indicates start of new person
                        {
                            name = values[1];                       // remember name for future relationships
                            rg.AddNode(name);                       // create the node
                            numPeople++;
                        }
                        else
                        {               
                            rg.AddEdge(name, values[1], values[0]); // add relationship (name1, name2, relationship)

                            // handle symmetric relationships -- add the other way
                            if (values[0] == "hasSpouse" || values[0] == "hasFriend")
                                rg.AddEdge(values[1], name, values[0]);

                            // for parent relationships add child as well
                            else if (values[0] == "hasParent")
                                rg.AddEdge(values[1], name, "hasChild");
                            else if (values[0] == "hasChild")
                                rg.AddEdge(values[1], name, "hasParent");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.Write("Unable to read file {0}: {1}\n", filename, e.ToString());
            }
            Console.WriteLine(numPeople + " people read");
        }

        // Show the relationships a person is involved in
        private static void ShowPerson(string name)
        {
            GraphNode n = rg.GetNode(name);
            if (n != null)
                Console.Write(n.ToString());
            else
                Console.WriteLine("{0} not found", name);
        }

        // Show a person's friends
        private static void ShowFriends(string name)
        {
            GraphNode n = rg.GetNode(name);
            if (n != null)
            {
                Console.Write("{0}'s friends: ",name);
                List<GraphEdge> friendEdges = n.GetEdges("hasFriend");
                foreach (GraphEdge e in friendEdges) {
                    Console.Write("{0} ",e.To());
                }
                Console.WriteLine();
            }
            else
                Console.WriteLine("{0} not found", name);     
        }

        //Show all the people without parents
        private static void ShowOrphans()
        {
            Console.Write("Orphans: ");
            foreach (GraphNode n in rg.nodes)
            {
                if (n.GetEdges("hasParent").Count == 0)
                    Console.Write("\t" + n.Name);
            }
        }

        //Show a Persons Siblings
        private static void ShowSiblings(string name)
        {
            GraphNode node = rg.GetNode(name);
            List<GraphNode> parents = rg.GetParentNodes(name);
            List<GraphNode> siblings = new List<GraphNode>();

            foreach (GraphNode parent in parents)
            {

                foreach (GraphNode sibling in rg.GetChildNodes(parent.Name))
                {
                    if (sibling != node)
                        siblings.Add(sibling);
                }
            }
                if (siblings.Count() == 0)
                {
                    Console.WriteLine(name + " has no siblings.");
                    return;
                }
            Console.WriteLine("Sibling of " + node.Name + ": ");
            foreach (GraphNode sibling in siblings)
                Console.Write(sibling.Name + " ");

            Console.WriteLine();
        }

        //Show a persons Decendants
        private static void ShowDecendants(string name)
        {
            if (rg.GetNode(name) == null)
            {
                Console.WriteLine(name + " not found");
                return;
            }

            if (rg.GetChildNodes(name).Count < 1)
            {
                Console.WriteLine(name + " has no decendants");
                return;
            }

            List<GraphNode> current_gen = new List<GraphNode>();
            List<GraphNode> next_gen = rg.GetChildNodes(name);
            int gen_num = 1;

            Console.WriteLine("*children: ");

            while(next_gen.Count > 0)
            {
                gen_num++;

                current_gen.Clear();
                copy_list(current_gen, next_gen);
                next_gen.Clear();

                if(gen_num > 2)
                {
                    Console.WriteLine();
                    Console.Write("*great ");
                    for(int i = 2; i < gen_num; i++)
                    {
                        Console.Write("great ");
                    }
                    Console.WriteLine("grandchildren: ");
                }

                foreach(GraphNode child in current_gen)
                {
                    Console.Write(child.Name + " ");
                    child.Label = "visited";
                    foreach(GraphNode next_kid in rg.GetChildNodes(child.Name))
                    {
                        if(node_visited(next_kid))
                            {
                            Console.WriteLine("Cycle detected");
                            return;
                        }
                        next_gen.Add(next_kid);
                    }
                }
                Console.WriteLine();
            }
            reset_label();
            return;
        }

        //Show find the shortest path of connects between two people
        static private void $safeprojectname$(string person_1, string person_2)
        {
            GraphNode person1 = rg.GetNode(person_1);
            person1.Label = "visited";
            GraphNode person2 = rg.GetNode(person_2);
            Dictionary<GraphNode, GraphNode> dict = new Dictionary<GraphNode, GraphNode>();
            Dictionary<GraphNode, GraphEdge> dict2 = new Dictionary<GraphNode, GraphEdge>();
            Queue<GraphEdge> queue = new Queue<GraphEdge>();
            List<GraphEdge> first = person1.GetEdges();

            foreach (GraphEdge e in first)
            {
                if(rg.GetNode(e.To()) == person2)
                {
                    Console.WriteLine(e.ToString());
                    return;
                }
                queue.Enqueue(e);
                rg.GetNode(e.To()).Label = "visited";
                dict.Add(rg.GetNode(e.To()), person1);
                dict2.Add(rg.GetNode(e.To()), e);
            }

            bool broken = false;
            while (queue.Count > 0)
            {
                GraphEdge next_edge = queue.Dequeue();
                GraphNode next_person = rg.GetNode(next_edge.To());
                foreach (GraphEdge e in next_person.GetEdges())
                {
                    if (rg.GetNode(e.To()) == person2)
                    {
                        dict.Add(person2, next_person);
                        dict2.Add(person2, e);
                        broken = true;
                        break;
                    }
                    else
                    {
                        if(rg.GetNode(e.To()).Label != "visited")
                        {
                            queue.Enqueue(e);
                            dict.Add(rg.GetNode(e.To()), next_person);
                            dict2.Add(rg.GetNode(e.To()), e);
                            rg.GetNode(e.To()).Label = "visited";
                        }
                    }
                }
                if (broken)
                    break;
            }
            GraphNode parent = dict[person2];
            GraphNode child = person2;

            if (child.GetRelationship(parent.Name) == null)
                Console.WriteLine(child.GetRelationship(parent.Name).ToString());
            else
                Console.WriteLine(child.GetRelationship(parent.Name).ToString());

            while (parent != person1)
            {
                child = parent;
                parent = dict[parent];

                if (child.GetRelationship(parent.Name) == null)
                    Console.WriteLine(parent.GetRelationship(child.Name));
                else
                    Console.WriteLine(child.GetRelationship(parent.Name).ToString());
            }

            reset_label();
        }
 
        //copy a list and add it too a new one
        private static void copy_list(List<GraphNode> current, List<GraphNode> next)
        {
            foreach (GraphNode person in next)
                current.Add(person);
        }

        //determine whether a node had been visited
        private static bool node_visited(GraphNode node)
        {
            return node.Label == "visited";
        }

        //reset the labels of nodes and change them for a future search
        private static void reset_label()
        {
            foreach (GraphNode person in rg.nodes)
                person.Label = "Unvisited";
        }
        // accept, parse, and execute user commands
        private static void CommandLoop()
        {
            string command = "";
            string[] commandWords;
            Console.Write("Welcome to Eric's Dutch $safeprojectname$ Parlor!\n");
            Console.Write("Please Read a File then choose your Command you want to do! \n");

            while (command != "exit")
            {
                Console.Write("\nEnter a command: ");
                command = Console.ReadLine();
                commandWords = Regex.Split(command, @"\s+");        // split input into array of words
                command = commandWords[0];

                if (command == "exit")
                    ;                                               // do nothing

                // read a relationship graph from a file
                else if (command == "read" && commandWords.Length > 1)
                    ReadRelationshipGraph(commandWords[1]);

                // show information for one person
                else if (command == "show" && commandWords.Length > 1)
                    ShowPerson(commandWords[1]);

                else if (command == "friends" && commandWords.Length > 1)
                    ShowFriends(commandWords[1]);

                else if (command == "Orphans")
                    ShowOrphans();

                else if (command == "Siblings" && commandWords.Length > 1)
                    ShowSiblings(commandWords[1]);

                else if (command == "Decendants" && commandWords.Length > 1)
                    ShowDecendants(commandWords[1]);

                else if (command == "$safeprojectname$ && commmandWords.Length > 1")
                    $safeprojectname$(commandWords[1], commandWords[2]);

                // dump command prints out the graph
                else if (command == "dump")
                    rg.Dump();

                // illegal command
                else
                    Console.Write("\nLegal commands: read [filename], dump, show [personname],\n  friends [personname], exit\n");
            }
        }

        static void Main(string[] args)
        {
            CommandLoop();
        }
    }
}

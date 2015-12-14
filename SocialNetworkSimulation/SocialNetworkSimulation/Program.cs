using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuickGraph;
using QuickGraph.Algorithms;
using System.IO;
using CsvHelper;
using System.Diagnostics.Contracts;
using System.Diagnostics;
using QuickGraph.Collections;

namespace SocialNetworkSimulation
{
    class Program
    {
        static void Main(string[] args)
        {

            // Initial Directory Setup
            var appPath = Environment.CurrentDirectory;
            Directory.CreateDirectory(appPath + "/Data/");

            // Project Input
            var graph = ImportGraphFromFile();
            var undirectedGraph = ImportUndirectedGraphFromFile();

            // Calculate Weighted/Unweighted In-Out Degrees
            calculateInOutDegrees(graph);

            // Unweighted Closeness Centrality Calculation Directed
            calculateClosenessCentrality(graph);

            // Unweighted Closeness Centrality Calculation Undirected
            calculateClosenessCentrality(undirectedGraph);





        }



        static BidirectionalGraph<string, Edge<string>> ImportGraphFromFile()
        {
            // Create Graph To Be Returned
            var graph = new BidirectionalGraph<string, Edge<string>>(false);

            // Initial Directory Setup
            var dataPath = Environment.CurrentDirectory + "/Data/";
            var file = new StreamReader(dataPath + "Data.txt");
            var csv = new CsvReader(file);

            var edges = new List<Edge<Vertex>>();

            while (csv.Read())
            {
                // Read CSV Data
                var source = csv.GetField<string>(0);
                var target = csv.GetField<string>(1);
                var weight = csv.GetField<int>(2);

                // Vertices And Edges To Graph
                var v1 = source;
                if (!graph.ContainsVertex(v1))
                    graph.AddVertex(v1);

                var v2 = target;
                if (!graph.ContainsVertex(v2))
                    graph.AddVertex(v2);

                var e1 = new Edge<string>(v1, v2, weight);
                if (!graph.ContainsEdge(e1))
                    graph.AddEdge(e1);
            }

            return graph;
        }

        static BidirectionalGraph<string, Edge<string>> ImportUndirectedGraphFromFile()
        {
            // Create Graph To Be Returned
            var graph = new BidirectionalGraph<string, Edge<string>>(true);

            // Initial Directory Setup
            var dataPath = Environment.CurrentDirectory + "/Data/";
            var file = new StreamReader(dataPath + "Data.txt");
            var csv = new CsvReader(file);

            var edges = new List<Edge<Vertex>>();

            while (csv.Read())
            {
                // Read CSV Data
                var source = csv.GetField<string>(0);
                var target = csv.GetField<string>(1);
                var weight = csv.GetField<int>(2);

                // Vertices And Edges To Graph
                var v1 = source;
                graph.AddVertex(v1);

                var v2 = target;
                graph.AddVertex(v2);

                // Duplicate Each Edge Both Ways
                var e1 = new Edge<string>(v1, v2, weight);
                var e2 = new Edge<string>(v2, v1, weight);
                graph.AddEdge(e1);
                graph.AddEdge(e2);
            }

            return graph;
        }

        /** Data Calculation Functions **/

        static void calculateInOutDegrees(BidirectionalGraph<string, Edge<string>> graph)
        {

            // Calculate In-Degree Unweighted Graph
            // TODO: Create Graph Off Of Data
            var numberOfVerticesByInDegree = new Dictionary<int, int>();

            foreach (var v in graph.Vertices)
            {
                var degree = graph.InDegree(v);

                if (numberOfVerticesByInDegree.ContainsKey(degree))
                {
                    int tempValue;
                    numberOfVerticesByInDegree.TryGetValue(degree, out tempValue);
                    numberOfVerticesByInDegree.Remove(degree);
                    numberOfVerticesByInDegree.Add(degree, tempValue + 1);
                }
                else
                {
                    numberOfVerticesByInDegree.Add(degree, 1);
                }
            }

            // Calculate Out-Degree Unweighted Graph
            var numberOfVerticesByOutDegree = new Dictionary<int, int>();

            foreach (var v in graph.Vertices)
            {

                var degree = graph.OutDegree(v);

                if (numberOfVerticesByOutDegree.ContainsKey(degree))
                {
                    int tempValue;
                    numberOfVerticesByOutDegree.TryGetValue(degree, out tempValue);
                    numberOfVerticesByOutDegree.Remove(degree);
                    numberOfVerticesByOutDegree.Add(degree, tempValue + 1);
                }
                else
                {
                    numberOfVerticesByOutDegree.Add(degree, 1);
                }
            }

            // Calculate Im-Degree Weighted Graph
            var numberOfVerticesByWeightedInDegree = new Dictionary<int, int>();

            foreach (var v in graph.Vertices)
            {
                var edges = graph.Edges.Where(e => e.Target == v);
                var edgeSum = 0;

                foreach (var e in edges)
                    edgeSum += e.weight;

                if (numberOfVerticesByWeightedInDegree.ContainsKey(edgeSum))
                {
                    int tempValue;
                    numberOfVerticesByWeightedInDegree.TryGetValue(edgeSum, out tempValue);
                    numberOfVerticesByWeightedInDegree.Remove(edgeSum);
                    numberOfVerticesByWeightedInDegree.Add(edgeSum, tempValue + 1);
                }
                else
                {
                    numberOfVerticesByWeightedInDegree.Add(edgeSum, 1);
                }
            }

            // Calculate Out-Degree Weighted Graph
            var numberOfVerticesByWeightedOutDegree = new Dictionary<int, int>();

            foreach (var v in graph.Vertices)
            {
                var edges = graph.Edges.Where(e => e.Source == v);
                var edgeSum = 0;

                foreach (var e in edges)
                    edgeSum += e.weight;

                if (numberOfVerticesByWeightedOutDegree.ContainsKey(edgeSum))
                {
                    int tempValue;
                    numberOfVerticesByWeightedOutDegree.TryGetValue(edgeSum, out tempValue);
                    numberOfVerticesByWeightedOutDegree.Remove(edgeSum);
                    numberOfVerticesByWeightedOutDegree.Add(edgeSum, tempValue + 1);
                }
                else
                {
                    numberOfVerticesByWeightedOutDegree.Add(edgeSum, 1);
                }
            }
        }

        static void calculateClosenessCentrality(BidirectionalGraph<string, Edge<string>> graph)
        {
            var averageDistanceByStartingVertex = new Dictionary<string, int>();
            var distancesByVertex = new Dictionary<string, int>();

            foreach (var v in graph.Vertices)
            {
                // Calculate Paths
                distancesByVertex = dijkstras(graph, v);

                // Remove Infinity
                var distances = distancesByVertex.Values.ToList();
                distances.RemoveAll(d => d == int.MaxValue);

                // Calculate Average
                var average = 0;
                foreach (var d in distances)
                    average += d;

                average = average / distances.Count;

                // Add To Result Dictionary
                averageDistanceByStartingVertex.Add(v, average);
            }
        }


        public static Dictionary<string, int> dijkstras(BidirectionalGraph<string, Edge<string>> g, string startVertex)
        {
            // Gives a enumerable object of the adjacent edges to startVertex
            IEnumerable<Edge<string>> adjEdges = g.OutEdges(startVertex);

            //			// Dictionary to hold the distances from startVertex to all verticies.
            //			Dictionary<string,int> distances = new Dictionary<string,int> ();
            ////			foreach (var v in g.Vertices)
            ////				distances.Add (v, -1);
            //
            //
            //			// Dictionary to store the previous node in shortest path.
            //			Dictionary<string,string> prevNode = new Dictionary<string,string>();
            ////			foreach (var v in g.Vertices)
            ////				prevNode.Add (v, null);

            // FibonacciQueue to be used in place of a Priority Queue.
            // Stores the unvisted nodes during runtime.
            // var unvisitedNodes = new FibonacciQueue<string,int>(distances);

            var unvistedNodes = new FibonacciHeap<int, string>();
            Dictionary<string, int> distances = new Dictionary<string, int>();


            // The distance from the start to itself is zero.
            if (!distances.ContainsKey(startVertex))
                distances.Add(startVertex, 0);

            foreach (var v in g.Vertices)
            {
                if (v != startVertex)
                {
                    if (!distances.ContainsKey(v))
                    {
                        distances.Add(v, int.MaxValue);
                    }

                    //unvistedNodes.Enqueue (distances [v], v);
                }
            }
            unvistedNodes.Enqueue(distances[startVertex], startVertex);

            //Console.WriteLine (unvistedNodes);


            while (!unvistedNodes.IsEmpty)
            {
                string u = unvistedNodes.Top.Value;
                unvistedNodes.Dequeue();

                foreach (var e in g.OutEdges(u))
                {
                    //					int edgeWeight = findEdgeWeight (g, u, v)
                    //Console.WriteLine(e.Target);
                    var alt = distances[u] + e.weight;
                    if (alt < distances[e.Target])
                    {
                        distances[e.Target] = alt;
                        //var tmp = new KeyValuePair<int,string>;

                        unvistedNodes.Enqueue(alt, e.Target);
                        //unvistedNodes.ChangeKey (unvistedNodes., alt);
                        //						unvistedNodes.ElementAt(
                        int temp = distances[e.Target];
                        //Console.WriteLine (e);
                    }
                }
            }

            return distances;
        }

        /** Custom Data Structure Helper Classes **/

        public class Vertex
        {
            public Vertex()
            {
                name = string.Empty;
            }

            public Vertex(string id)
            {
                name = id;
            }

            public string name;
        }

        [DebuggerDisplay("{Source}->{Target}")]
        public class Edge<TVertex>
            : IEdge<TVertex>
        {
            private readonly TVertex source;
            private readonly TVertex target;
            public int weight;

            /// <summary>
            /// Initializes a new instance of the <see cref="Edge&lt;TVertex&gt;"/> class.
            /// </summary>
            /// <param name="source">The source.</param>
            /// <param name="target">The target.</param>
            public Edge(TVertex source, TVertex target, int weight)
            {
                Contract.Requires(source != null);
                Contract.Requires(target != null);
                Contract.Ensures(this.Source.Equals(source));
                Contract.Ensures(this.Target.Equals(target));

                this.source = source;
                this.target = target;
                this.weight = weight;
            }

            /// <summary>
            /// Gets the source vertex
            /// </summary>
            /// <value></value>
            public TVertex Source
            {
                get { return this.source; }
            }

            /// <summary>
            /// Gets the target vertex
            /// </summary>
            /// <value></value>
            public TVertex Target
            {
                get { return this.target; }
            }

            /// <summary>
            /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
            /// </summary>
            /// <returns>
            /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
            /// </returns>
            public override string ToString()
            {
                return this.Source + "->" + this.Target;
            }
        }
    }
}
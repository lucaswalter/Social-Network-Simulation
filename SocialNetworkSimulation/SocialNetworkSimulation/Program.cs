﻿using System;
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

            // Calculate Weighted/Unweighted In-Out Degrees
            calculateInOutDegrees(graph);

        }

        static BidirectionalGraph<string, Edge<string>> ImportGraphFromFile()
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
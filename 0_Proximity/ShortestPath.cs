using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace Zachitect_GH
{
    public class ShortestPath : GH_Component
    {

        public ShortestPath()
          : base(
                "Shortest Path",//Full Name
                "S Path",//Nick Name
                "Find the shortest path from A to B in street network using Dijkstra algorithm.\n\n© Zach X.G. Zheng | Zachitect.com",//Description
                "Zachitect.com",//Tab
                "Proximity"//Category
                )
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Network Curves", "Network", "Segmented curves as street network", GH_ParamAccess.list);
            pManager.AddPointParameter("Departure", "Departure", "Departure point", GH_ParamAccess.item);
            pManager.AddPointParameter("Destination", "Destination", "Destination point", GH_ParamAccess.item);
            pManager.AddNumberParameter("Arbitrary Length", "Length", "A list of Length for network curves, actual length used if null", GH_ParamAccess.list);
            pManager[3].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Shortest Path", "Shortest", "The shortest path from departure to destination", GH_ParamAccess.item);
            pManager.AddNumberParameter("Shortest Distance", "Distance", "The shortest distance from departure to destination", GH_ParamAccess.item);
            pManager.AddNumberParameter("Index of Curves", "Index", "Index of input network curves in the path", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Curve> InputCrvs = new List<Curve>();
            if (!DA.GetDataList(0, InputCrvs)) return;
            Point3d SP = new Point3d();
            if (!DA.GetData(1, ref SP)) return;
            Point3d EP = new Point3d();
            if (!DA.GetData(2, ref EP)) return;
            List<Double> SubjectiveLength = new List<Double>();
            DA.GetDataList(3, SubjectiveLength);


            if (RoundPoint(SP).DistanceTo(RoundPoint(EP)) < MTolerance)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Departure and destination are overlapped, do you expect some sort of worm hole travel?");
                return;
            }

            if (SubjectiveLength.Count > 0 && SubjectiveLength.Count != InputCrvs.Count)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Network curves and provided arbitrary lengths provided are of different numbers, that is too arbitrary!");
                return;
            }

            bool TooSmall = false;
            foreach (Double len in SubjectiveLength)
            {
                if (len < MTolerance)
                {
                    TooSmall = true;
                }
            }
            if (TooSmall)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Some of the length values are too small or negative, please fix them before proceed.");
                return;
            }

            if (SubjectiveLength.Count == InputCrvs.Count)
            {
                Subject_Dist = SubjectiveLength;
            }

            List<Curve> ProcessedNetwork = CleanNetwork(InputCrvs);
            GetPathInfo(ProcessedNetwork);//Create Adjacency Matrix

            int SPIndex = GetSource(UniqueNodes, SP);
            source_index = SPIndex;
            int EPIndex = GetSource(UniqueNodes, EP);

            Dijkstra(AdjacentMatrix);//Implement Dijkstra
            
            //OUTPUT
            List<int>[] NodePathRecords = GetNodePathRecord();
            int[][] EdgePathRecords = NodeRecordToEdgeRecord(NodePathRecords, EdgeNodePairs);
            Curve[] OuputShortest = ShortestCurves(SP, EdgePathRecords, ProcessedNetwork.ToArray());
            
            DA.SetData(0, OuputShortest[EPIndex]);
            DA.SetData(1, Distances[EPIndex]);
            DA.SetDataList(2, EdgePathRecords[EPIndex].ToList());
        }
        //====================
        //Document Settings

        Double MTolerance = Rhino.RhinoDoc.ActiveDoc.ModelAbsoluteTolerance;
        List<Double> Subject_Dist { get; set; }
        
        //Output GetPathInfo - Adjacency Matrix
        public List<Point3d> UniqueNodes { get; set; }
        public double[,] AdjacentMatrix { get; set; }
        public int[][] EdgeNodePairs { get; set; }
        public int[] pathStarts { get; set; }
        public int[] pathEnds { get; set; }

        //Output Dijkstra
        public double[] Distances { get; set; }
        public int[] NodeRecord { get; set; }
        List<int> NodePathHistory { get; set; }

        //Common Variables
        static int source_index { get; set; }

        //====================
        // Custom Functions
        private Curve[] ShortestCurves(Point3d SP, int[][]EdgeIndex, Curve[] InputNetworks)
        {
            Curve[] PathCollection = new Curve[UniqueNodes.Count];
            for(int h = 0; h < EdgeIndex.Length; h++)
            {
                List<Curve> pc = new List<Curve>();
                foreach (int i in EdgeIndex[h])
                {
                    try
                    {
                        pc.Add(InputNetworks[i]);
                    }
                    catch
                    {
                    }
                    
                }
                if (pc.Count > 0)
                {
                    try
                    {
                        Curve crvPath = Curve.JoinCurves(pc)[0];
                        if (crvPath.PointAtStart.DistanceTo(SP) > crvPath.PointAtEnd.DistanceTo(SP))
                        {
                            crvPath.Reverse();
                        }
                        PathCollection[h] = crvPath;
                    }
                    catch
                    {
                        PathCollection[h] = null;
                    }
                }
                else
                {
                    PathCollection[h] = null;
                }
            }
            return PathCollection;
        }
        private int[][] NodeRecordToEdgeRecord(List<int>[] NodeRecordIndex, int[][] EdgeNodePairIndex)
        {
            int[][] OutputEdgeRecord = new int[NodeRecordIndex.Length][];
            for (int i = 0; i < NodeRecordIndex.Length;i++)
            {
                if (NodeRecordIndex[i].Count > 1)
                {
                    int[] EdgeRecords = new int[NodeRecordIndex[i].Count - 1];
                    for (int j = 0; j < NodeRecordIndex[i].Count - 1; j++)
                    {
                        int[] EdgeNodePair = new int[2] { (NodeRecordIndex[i])[j], (NodeRecordIndex[i])[j + 1] };
                        for (int k = 0; k < EdgeNodePairIndex.Length; k++)
                        {
                            if (EdgeNodePairIndex[k].Except(EdgeNodePair).Count() == 0)
                            {
                                EdgeRecords[j] = k;
                            }
                        }
                    }
                    OutputEdgeRecord[i] = EdgeRecords;
                }
                else
                {
                    OutputEdgeRecord[i] = new int[1] { -1 };
                }
                
            }
            return OutputEdgeRecord;
        }

        //Get Node Index of the Path from a Start Node to All Nodes
        private List<int>[] GetNodePathRecord()
        {
            List<int>[] Record = new List<int>[UniqueNodes.Count];
            for (int i = 0; i < UniqueNodes.Count; i++)
            {
                NodePathHistory = new List<int>();
                UpdateNodePath(i, NodeRecord);
                Record[i] = NodePathHistory;
            }
            return Record;
        }

        //Round Point3d XYZ with 6 Decimal Places
        private Point3d RoundPoint(Point3d Pt)
        {
            double x = Math.Round(Pt.X, 6);
            double y = Math.Round(Pt.Y, 6);
            double z = Math.Round(Pt.Z, 6);
            Point3d RoundPt = new Point3d(x, y, z);
            return RoundPt;
        }
        //Establish Adjacency Matrix
        private void GetPathInfo(List<Curve> Network)
        {
            int IniCount = 0;//Index for Unique Pt
            List<Point3d> UniquePts = new List<Point3d>();
            int[] NetIndex = new int[Network.Count];
            int[] UIndexSP = new int[Network.Count];
            int[] UIndexEP = new int[Network.Count];
            double[] NetLength = new double[Network.Count];

            for (int i = 0; i < Network.Count; i++)
            {
                NetIndex[i] = i;
                NetLength[i] = Math.Round(Network[i].GetLength(), 6);
                Point3d sp = RoundPoint(Network[i].PointAtStart);
                Point3d ep = RoundPoint(Network[i].PointAtEnd);

                if (UniquePts.Contains(sp) == false)
                {
                    UniquePts.Add(sp);
                    UIndexSP[i] = IniCount;
                    IniCount = IniCount + 1;
                }
                else
                {
                    UIndexSP[i] = UniquePts.IndexOf(sp);
                }
                if (UniquePts.Contains(ep) == false)
                {
                    UniquePts.Add(ep);
                    UIndexEP[i] = IniCount;
                    IniCount = IniCount + 1;
                }
                else
                {
                    UIndexEP[i] = UniquePts.IndexOf(ep);
                }
            }
            if(Subject_Dist != null)
            { 
                NetLength = Subject_Dist.ToArray(); 
            }

            //Create Matrix of Unique Nodes After Getting Unique Points
            int UniquePtSize = UniquePts.Count;
            double[,] NodeMatrix = new double[UniquePtSize, UniquePtSize];

            for (int i = 0; i < UniquePtSize; i++)
            {
                for (int j = 0; j < UniquePtSize; j++)
                {
                    NodeMatrix[i, j] = double.PositiveInfinity;
                }
            }

            for (int i = 0; i < Network.Count; i++)
            {
                int u = UIndexSP[i];
                int v = UIndexEP[i];
                double dist = NetLength[i];
                NodeMatrix[u, v] = dist;
                NodeMatrix[v, u] = dist;
                NodeMatrix[u, u] = 0;
                NodeMatrix[v, v] = 0;
            }
            UniqueNodes = UniquePts;
            AdjacentMatrix = NodeMatrix;
            pathStarts = UIndexSP;
            pathEnds = UIndexEP;

            //Represent an edge by node index in a pair
            int[][] EdgePairSet = new int[Network.Count][];
            for (int i = 0; i < Network.Count; i++)
            {
                if(pathStarts[i] != pathEnds[i])
                {
                    int[] EdgeNodePair = new int[2] { pathStarts[i], pathEnds[i] };
                    EdgePairSet[i] = EdgeNodePair;
                }
                else
                {
                    EdgePairSet[i] = new int[1] { pathStarts[i]};
                }
            }
            EdgeNodePairs = EdgePairSet;
        }

        //Dijkstra Core Implementation ==========
        private void Dijkstra(double[,] NodeMatrix)
        {
            int source = source_index;
            int rowsize = NodeMatrix.GetLength(0);
            bool[] Visited = new bool[rowsize];
            double[] Distance = new double[rowsize];

            //This is to record travel nodes index
            int[] PtHistory = new int[rowsize];
            bool[] Added = new bool[rowsize];

            for (int i = 0; i < rowsize; i++)
            {
                Visited[i] = false;
                Added[i] = false;
                Distance[i] = double.PositiveInfinity;
            }
            Distance[source] = 0;
            PtHistory[source] = -1;

            for (int count = 0; count < rowsize; count++)
            {
                int NeighbourIndex = -1;
                double ShortestDist = double.PositiveInfinity;

                //Min Index Row/Column to Visit
                //int min_index = MinDistIndex(Distance, Visited);
                //Visited[min_index] = true;

                for (int col = 0; col < rowsize; col++)
                {
                    /*if(Visited[col] == false
                      && NodeMatrix[min_index, col] != 0
                      && NodeMatrix[min_index, col] != Double.PositiveInfinity
                      && (Distance[min_index] + NodeMatrix[min_index, col]) < Distance[col])
                    {
                      Distance[col] = Distance[min_index] + NodeMatrix[min_index, col];
                    }*/

                    if (Added[col] == false
                      && Distance[col] < ShortestDist)
                    {
                        NeighbourIndex = col;
                        ShortestDist = Distance[col];
                    }
                }
                Added[NeighbourIndex] = true;
                for (int col = 0; col < rowsize; col++)
                {
                    double edgeDist = NodeMatrix[NeighbourIndex, col];
                    if (edgeDist > 0 && ((ShortestDist + edgeDist) < Distance[col]))
                    {
                        PtHistory[col] = NeighbourIndex;
                        Distance[col] = ShortestDist + edgeDist;
                    }

                }
            }
            Distances = Distance;
            NodeRecord = PtHistory;
        }
        //Get Min Distance Index from Each Row/Colomn of Adjcency Matrix
        private int MinDistIndex(double[] dist, bool[] visit)
        {
            int size = dist.Length;
            double min = double.PositiveInfinity;
            int min_index = -1;
            for (int i = 0; i < size; i++)
            {
                if (visit[i] == false && dist[i] <= min)
                {
                    min = dist[i];
                    min_index = i;
                }
            }
            return min_index;
        }
        //Get Source Index Based on Point Proximity
        int GetSource(List<Point3d> PointList, Point3d Pt)
        {
            double dist = double.PositiveInfinity;
            int index = -1;
            for (int i = 0; i < PointList.Count; i++)
            {
                double DS = Pt.DistanceTo(PointList[i]);
                if (DS < dist)
                {
                    dist = DS;
                    index = i;
                }
            }
            return index;
        }

        //
        private void UpdateNodePath(int currentVertex, int[] parents)
        {
            // Base case : Source node has
            // been processed
            if (currentVertex == -1)
            {
                return;
            }
            UpdateNodePath(parents[currentVertex], parents);
            NodePathHistory.Add(currentVertex);
        }

        //Eliminate Minimal Curves
        private List<Curve> CleanNetwork(List<Curve> Path_Network)
        {
            List<Curve> Network = new List<Curve>();
            foreach (Curve c in Path_Network)
            {
                if (c.GetLength() > MTolerance)
                {
                    Network.Add(c);
                }
            }
            return Network;
        }


protected override System.Drawing.Bitmap Icon
        {
            get
            {
                // You can add image files to your project resources and access them like this:
                //return Resources.IconForThisComponent;
                return Properties.Resources.ShortestPath;
            }
        }
        public override Guid ComponentGuid
        {
            get { return new Guid("3a89f4c5-326a-4684-8818-427c53794213"); }
        }
    }
}

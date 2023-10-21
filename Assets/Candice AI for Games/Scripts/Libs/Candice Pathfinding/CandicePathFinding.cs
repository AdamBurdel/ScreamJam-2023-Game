using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System.Linq;
using System;
using CandiceAIforGames.data;

namespace CandiceAIforGames.AI.Pathfinding
{
    public class CandicePathFinding
    {
        CandiceGrid grid;
        GameObject[] tiles;

        public CandicePathFinding(CandiceGrid _grid)
        {
            grid = _grid;
            ComputeAdjacencyList(1, null);
        }
        /// <summary>
        /// Computes the adjacency list for each CandiceTile in the scene, based on the specified jump height and target tile.
        /// </summary>
        /// <param name="jumpHeight">The maximum height that the player can jump when traversing between tiles.</param>
        /// <param name="target">The target tile that the player is trying to reach.</param>
        /// <remarks>
        /// This function finds all CandiceTile game objects in the scene that have the "CandiceTile" tag, and calls the FindNeighbors method on each CandiceTile component to compute its adjacency list. The adjacency list is a list of other CandiceTile components that are adjacent to the current tile, meaning that they can be reached by jumping from the current tile with a height of no more than jumpHeight. The adjacency list is stored as a public property of the CandiceTile component, and can be accessed by other scripts to implement pathfinding or other AI behaviors. If no CandiceTile game objects are found in the scene, an empty array is created and used instead. 
        /// </remarks>
        public void ComputeAdjacencyList(float jumpHeight, CandiceTile target)
        {
            // Find all game objects in the scene with the "CandiceTile" tag
            GameObject[] tiles;
            try
            {
                tiles = GameObject.FindGameObjectsWithTag("CandiceTile");
            }
            catch
            {
                // If no game objects are found, use an empty array instead
                tiles = new GameObject[0];
            }

            // Compute the adjacency list for each CandiceTile in the scene
            foreach (GameObject tile in tiles)
            {
                // Get the CandiceTile component of the current game object
                CandiceTile t = tile.GetComponent<CandiceTile>();

                // Compute the adjacency list for the current CandiceTile component
                t.FindNeighbors(jumpHeight, target);
            }
        }

        /// <summary>
        /// Finds all selectable tiles within a given maximum move distance, starting from a given tile. Make sure to call ComputeAdjacencyList() first.
        /// </summary>
        /// <param name="currentTile">The tile from which to start the search for selectable tiles.</param>
        /// <param name="maxMovePoints">The maximum distance that can be traveled from the starting tile.</param>
        /// <param name="callback">The function to call when the selectable tiles have been found.</param>
        /// <remarks>
        /// This function uses the Breadth-First Search algorithm to find all tiles that can be reached
        /// from the starting tile within a given maximum move distance. The resulting list of selectable
        /// tiles is returned to the specified callback function.
        /// </remarks>
        public void FindSelectableTiles(CandiceTile currentTile, float maxMovePoints, Action<List<CandiceTile>> callback)
        {
            // Create an empty list of selectable tiles
            List<CandiceTile> selectableTiles = new List<CandiceTile>();

            // Create a queue for BFS traversal
            Queue<CandiceTile> process = new Queue<CandiceTile>();

            // Enqueue the current tile and mark it as visited
            process.Enqueue(currentTile);
            currentTile.visited = true;

            // BFS traversal to find all selectable tiles
            while (process.Count > 0)
            {
                CandiceTile t = process.Dequeue();

                // Add the tile to the list of selectable tiles and mark it as selectable
                selectableTiles.Add(t);
                t.selectable = true;

                // If the distance from the current tile is less than maxMovePoints, add all its adjacent tiles to the queue
                if (t.distance < maxMovePoints)
                {
                    foreach (CandiceTile adjacentTile in t.adjacencyList)
                    {
                        if (!adjacentTile.visited)
                        {
                            adjacentTile.parent = t;
                            adjacentTile.visited = true;
                            adjacentTile.distance = 1 + t.distance;
                            process.Enqueue(adjacentTile);
                        }
                    }
                }
            }

            // Call the callback function with the list of selectable tiles
            callback(selectableTiles);
        }
        /// <summary>
        /// Finds the path to a target tile using a Breadth-First Search algorithm.
        /// </summary>
        /// <param name="targetTile">The target tile to find the path to.</param>
        /// <param name="callback">The method to call with the resulting path.</param>
        public void FindBFSPath(CandiceTile targetTile, Action<Stack<CandiceTile>> callback)
        {
            // Create a new stack to hold the path.
            Stack<CandiceTile> path = new Stack<CandiceTile>();

            // Mark the target tile as the destination of the path.
            targetTile.target = true;

            // Traverse the tree from the target tile to the starting tile, adding each tile to the path stack.
            CandiceTile nextTile = targetTile;
            while (nextTile != null)
            {
                path.Push(nextTile);
                nextTile = nextTile.parent;
            }

            // Invoke the callback method with the resulting path stack.
            callback(path);
        }
        /// <summary>
        /// Calculates the shortest path between two points using the A* algorithm. Calls the specified callback function with the result.
        /// </summary>
        /// <param name="request">The pathfinding request, which contains information about the start point, end point, and callback function.</param>
        /// <param name="callback">The callback function to call with the result of the pathfinding operation.</param>
        /// <remarks>
        /// The A* algorithm uses a heuristic function to estimate the cost of the remaining path from a node to the target node. 
        /// The algorithm maintains two sets of nodes: the open set, which contains nodes that have been discovered but not yet explored, 
        /// and the closed set, which contains nodes that have already been explored. 
        /// The algorithm selects the node with the lowest fCost (the sum of the gCost and hCost) from the open set, and adds it to the closed set. 
        /// It then checks the neighbours of the current node, and for each neighbour that is walkable and not in the closed set, 
        /// it calculates the cost of moving from the current node to the neighbour node, and updates the neighbour node's gCost, hCost, 
        /// and parent attributes if it's cheaper to get to the neighbour node through the current node. 
        /// If the neighbour node is not in the open set, it is added to the open set. 
        /// If the neighbour node is already in the open set, its fCost is updated to reflect the new cost. 
        /// If the target node is found, the algorithm stops, and the function retraces the path by following the parent links back to the start node.
        /// </remarks>
        public void FindASTARPath(PathRequest request, Action<PathResult> callback)
        {
            //Starts a timer
            Stopwatch sw = new Stopwatch();
            sw.Start();

            //Initializes variables
            Vector3[] waypoints = new Vector3[0];
            bool pathSuccess = false;
            //Finds the start and end node from the given world points
            Node startNode = grid.NodeFromWorldPoint(request.pathStart);
            Node targetNode = grid.NodeFromWorldPoint(request.pathEnd);

            //Checks if start and end nodes are walkable
            if (startNode.walkable && targetNode.walkable)
            {
                //Initializes open and closed sets
                CandiceHeap<Node> openSet = new CandiceHeap<Node>(grid.MaxSize);
                HashSet<Node> closedSet = new HashSet<Node>();
                openSet.Add(startNode);

                //Starts A* pathfinding algorithm
                while (openSet.Count > 0)
                {
                    //Takes the node with the lowest fCost from the open set
                    Node currentNode = openSet.RemoveFirst();
                    //Adds current node to the closed set
                    closedSet.Add(currentNode);
                    //If the path has been found
                    if (currentNode == targetNode)
                    {
                        //Stops the timer, sets path success flag to true, and breaks the loop
                        sw.Stop();
                        pathSuccess = true;
                        break;
                    }
                    //Checks the neighbours of the current node
                    foreach (Node neighbour in grid.GetNeighbours(currentNode))
                    {
                        //Ignores non-walkable nodes or nodes already in the closed set
                        if (!neighbour.walkable || closedSet.Contains(neighbour))
                        {
                            continue;
                        }
                        //Calculates the new cost to get to the neighbour node
                        int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour) + neighbour.movementPenalty;
                        //If it's cheaper to get to the neighbour node through the current node, or if the neighbour node is not in the open set
                        if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                        {
                            //Updates the neighbour node
                            neighbour.gCost = newMovementCostToNeighbour;
                            neighbour.hCost = GetDistance(neighbour, targetNode);
                            neighbour.parent = currentNode;
                            //If the neighbour node is not in the open set, adds it
                            if (!openSet.Contains(neighbour))
                            {
                                openSet.Add(neighbour);
                            }
                            //Otherwise, updates the neighbour node in the open set
                            else
                            {
                                openSet.UpdateItem(neighbour);
                            }
                        }
                    }
                }
            }
            //Retraces the path and sets path success flag to true if path has been found
            if (pathSuccess)
            {
                waypoints = RetracePath(startNode, targetNode);
                pathSuccess = waypoints.Length > 0;
            }
            //Calls the callback function with the path result
            callback(new PathResult(waypoints, pathSuccess, request.callback));
        }

        /// <summary>
        /// Backtracks through the path from endNode to startNode and returns an array of Vector3 waypoints
        /// </summary>
        /// <param name="startNode">The starting node of the path</param>
        /// <param name="endNode">The ending node of the path</param>
        /// <returns>An array of Vector3 waypoints from the start to end of the path</returns>
        Vector3[] RetracePath(Node startNode, Node endNode)
        {
            // Initialize an empty list to store the nodes in the path
            List<Node> path = new List<Node>();

            // Set the current node to the end node
            Node currentNode = endNode;

            // Loop through the nodes from the end node to the start node
            while (currentNode != startNode)
            {
                // Add the current node to the path list
                path.Add(currentNode);

                // Set the current node to its parent node
                currentNode = currentNode.parent;
            }

            // Convert and simplify the path to an array of Vector3 waypoints
            Vector3[] waypoints = ConvertAndSimplifyPath(path);

            // Reverse the order of the waypoints to create a path from the start to end
            Array.Reverse(waypoints);

            // Return the array of waypoints
            return waypoints;
        }
        Vector3[] ConvertPath(List<Node> path)
        {
            List<Vector3> waypoints = new List<Vector3>();

            for (int i = 1; i < path.Count; i++)
            {
                waypoints.Add(path[i].worldPosition);
            }
            return waypoints.ToArray();
        }


        /// <summary>
        /// Converts a path of nodes to an array of waypoints (as Vector3).
        /// The path is simplified by removing waypoints that are on the same line (and same direction) as the previous waypoint.
        /// </summary>
        /// <param name="path">A list of nodes representing a path.</param>
        /// <returns>An array of Vector3 representing a simplified version of the path.</returns>
        Vector3[] ConvertAndSimplifyPath(List<Node> path)
        {
            // Create an empty list to hold the waypoints
            List<Vector3> waypoints = new List<Vector3>();

            // Keep track of the previous direction to determine when to add a new waypoint
            Vector2 directionOld = Vector2.zero;

            // Loop over the nodes in the path (starting from the second node)
            for (int i = 1; i < path.Count; i++)
            {
                // Calculate the direction between the current node and the previous node
                Vector2 directionNew = new Vector2(path[i - 1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY);

                // If the direction has changed since the last node, add the current node's position as a new waypoint
                if (directionNew != directionOld)
                {
                    waypoints.Add(path[i].worldPosition);
                }

                // Update the previous direction
                directionOld = directionNew;
            }

            // Convert the list of waypoints to an array and return it
            return waypoints.ToArray();
        }
        /// <summary>
        /// Calculates the heuristic cost between two nodes using Manhattan distance.
        /// </summary>
        /// <param name="nodeA">The starting node.</param>
        /// <param name="nodeB">The ending node.</param>
        /// <returns>The heuristic cost between the two nodes.</returns>
        int GetDistance(Node nodeA, Node nodeB)
        {
            int distX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
            int distY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

            // The heuristic cost is based on the difference between the nodes' x and y positions on a two-dimensional grid.
            // The cost is calculated as 10 for every horizontal or vertical step and 14 for every diagonal step.
            if (distX > distY)
            {
                return 14 * distY + 10 * (distX - distY);
            }
            else
            {
                return 14 * distX + 10 * (distY - distX);
            }
        }

    }

}

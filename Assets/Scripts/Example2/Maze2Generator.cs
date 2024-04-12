using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maze2Generator : MonoBehaviour
{
	//Prefab database
	[SerializeField]
    PrefabDatabase prefabDB;
	//Map size
	[SerializeField]
    int mazeX = 59;
    [SerializeField]
    int mazeY = 59;
	//Container
	[SerializeField]
    Transform mazeGroup;

	//Prefab instances 2D array
	Maze2Cell[,] mazeCellMap;

	//List for Prim's algorithm
	List<Maze2Cell> unvisitCells = new List<Maze2Cell>();

    // Start is called before the first frame update
    void Start()
    {
        GenerateMaze();
    }

    void GenerateMaze()
    {
        mazeCellMap = new Maze2Cell[mazeX, mazeY];
        //Build all cells
        for (int x = 0; x < mazeX; x++)
        {
            for (int y = 0; y < mazeY; y++)
            {
                Maze2Cell cell = Instantiate(prefabDB.prefabList[1], mazeGroup).GetComponent<Maze2Cell>();
                cell.transform.position = new Vector3(cell.mazeSize * x, 0, cell.mazeSize * y);

                mazeCellMap[x, y] = cell;

                //Assign the current position to cell
                cell.Init(x, y);
            }
        }

        Maze2Cell startCell = mazeCellMap[1, 1];
        unvisitCells.Add(startCell);

		//Start Recursive at cell [1,1] ([0,0] is a wall)
		RecursiveRandomPrim(startCell);
    }

    void RecursiveRandomPrim(Maze2Cell startCell)
    {
        unvisitCells.Remove(startCell);
        if (!startCell.isVisited)
        {
            startCell.isVisited = true;
            //This cell will not be a wall cell
            startCell.wall.SetActive(false);

            //Instant connect from main path, hide the wall cell in between
            if (startCell.tunnelDirection == Maze2TunnelDirectionIndicator.Right)
            {
                mazeCellMap[startCell.locX - 1, startCell.locY].wall.SetActive(false);
            }
            else if (startCell.tunnelDirection == Maze2TunnelDirectionIndicator.Left)
            {
                mazeCellMap[startCell.locX + 1, startCell.locY].wall.SetActive(false);
            }
            else if (startCell.tunnelDirection == Maze2TunnelDirectionIndicator.Up)
            {
                mazeCellMap[startCell.locX, startCell.locY + 1].wall.SetActive(false);
            }
            else if (startCell.tunnelDirection == Maze2TunnelDirectionIndicator.Down)
            {
                mazeCellMap[startCell.locX, startCell.locY - 1].wall.SetActive(false);
            }

            //Standard connection, check unvisited cells nearby start cell
            List<Maze2Cell> neighborUnvisitedCells = CheckCellSurroundings(startCell);


            if (neighborUnvisitedCells.Count > 0)
            {
                //Connect to one of the nearby unvisit cells
                Maze2Cell endCell = neighborUnvisitedCells[Random.Range(0, neighborUnvisitedCells.Count)];
                endCell.isVisited = true;
                endCell.wall.SetActive(false);

                if (endCell.locX < startCell.locX)
                {
                    mazeCellMap[startCell.locX - 1, startCell.locY].wall.SetActive(false);
                }
                else if (endCell.locX > startCell.locX)
                {
                    mazeCellMap[startCell.locX + 1, startCell.locY].wall.SetActive(false);
                }
                else if (endCell.locY < startCell.locY)
                {
                    mazeCellMap[startCell.locX, startCell.locY - 1].wall.SetActive(false);
                }
                else if (endCell.locY > startCell.locY)
                {
                    mazeCellMap[startCell.locX, startCell.locY + 1].wall.SetActive(false);
                }

                //Remove visited endCell from unvisited cell list
                neighborUnvisitedCells.Remove(endCell);

				//Get all unvisited cells around startCell & endCell and add to the unvisitCells list
				unvisitCells.AddRange(neighborUnvisitedCells);
                //Since end cell is also changed to visited status, add unvisited cells nearby end cell as well
                unvisitCells.AddRange(CheckCellSurroundings(endCell));

            }
        }
        if (unvisitCells.Count > 0)
        {
            //As long as there is unvisited cell in the list, keep the recursive progress
            //Randomly choose one cell and continue
            RecursiveRandomPrim(unvisitCells[Random.Range(0, unvisitCells.Count)]);
        }
        else
        {
            Debug.Log("Generation Done");
        }
    }
    List<Maze2Cell> CheckCellSurroundings(Maze2Cell cell)
    {
        List<Maze2Cell> neighborUnvisitedCells = new List<Maze2Cell>();

        //Check if surrounding cells are unvisited and add them to the list, skip over the wall cell (-2/+2)
        if (cell.locX - 2 > 0)
        {
            Maze2Cell checkingNeighborCell = mazeCellMap[cell.locX - 2, cell.locY];
            if (!checkingNeighborCell.isVisited)
            {
                neighborUnvisitedCells.Add(checkingNeighborCell);
                checkingNeighborCell.tunnelDirection = Maze2TunnelDirectionIndicator.Left;
            }
        }
        if (cell.locX + 2 < mazeX - 1)
        {
            Maze2Cell checkingNeighborCell = mazeCellMap[cell.locX + 2, cell.locY];
            if (!checkingNeighborCell.isVisited)
            {
                neighborUnvisitedCells.Add(checkingNeighborCell);
                checkingNeighborCell.tunnelDirection = Maze2TunnelDirectionIndicator.Right;
            }
        }
        if (cell.locY - 2 > 0)
        {
            Maze2Cell checkingNeighborCell = mazeCellMap[cell.locX, cell.locY - 2];
            if (!checkingNeighborCell.isVisited)
            {
                neighborUnvisitedCells.Add(checkingNeighborCell);
                checkingNeighborCell.tunnelDirection = Maze2TunnelDirectionIndicator.Up;
            }
        }
        if (cell.locY + 2 < mazeY - 1)
        {
            Maze2Cell checkingNeighborCell = mazeCellMap[cell.locX, cell.locY + 2];
            if (!checkingNeighborCell.isVisited)
            {
                neighborUnvisitedCells.Add(checkingNeighborCell);
                checkingNeighborCell.tunnelDirection = Maze2TunnelDirectionIndicator.Down;
            }
        }

        return neighborUnvisitedCells;
    }
}

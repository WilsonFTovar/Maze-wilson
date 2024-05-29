using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class MazeGenerator : MonoBehaviour
{
    [SerializeField]
    private MazeCell _mazeCellPrefab;

    [SerializeField]
    private GameObject _torchPrefab; // Prefab de la antorcha o lámpara

    [SerializeField]
    private GameObject _zombiePrefab; // Prefab del zombi

    [SerializeField]
    private GameObject _starPrefab; // Prefab de la estrella

    [SerializeField]
    private GameObject _winPanel; // Panel de "WIN"

    [SerializeField]
    private int _mazeWidth;

    [SerializeField]
    private int _mazeDepth;

    [SerializeField]
    private int _numberOfZombies = 10;

    private MazeCell[,] _mazeGrid;

    void Start()
    {
        _mazeGrid = new MazeCell[_mazeWidth, _mazeDepth];

        for (int x = 0; x < _mazeWidth; x++)
        {
            for (int z = 0; z < _mazeDepth; z++)
            {
                _mazeGrid[x, z] = Instantiate(_mazeCellPrefab, new Vector3(x, 0, z), Quaternion.identity);
            }
        }

        GenerateMaze(null, _mazeGrid[0, 0]);
        PlaceTorches();
        PlaceZombies();
        PlaceStar();
    }

    private void GenerateMaze(MazeCell previousCell, MazeCell currentCell)
    {
        currentCell.Visit();
        ClearWalls(previousCell, currentCell);

        MazeCell nextCell;

        do
        {
            nextCell = GetNextUnvisitedCell(currentCell);

            if (nextCell != null)
            {
                GenerateMaze(currentCell, nextCell);
            }
        } while (nextCell != null);
    }

    private MazeCell GetNextUnvisitedCell(MazeCell currentCell)
    {
        var unvisitedCells = GetUnvisitedCells(currentCell);

        return unvisitedCells.OrderBy(_ => Random.Range(1, 10)).FirstOrDefault();
    }

    private IEnumerable<MazeCell> GetUnvisitedCells(MazeCell currentCell)
    {
        int x = (int)currentCell.transform.position.x;
        int z = (int)currentCell.transform.position.z;

        if (x + 1 < _mazeWidth)
        {
            var cellToRight = _mazeGrid[x + 1, z];

            if (cellToRight.IsVisited == false)
            {
                yield return cellToRight;
            }
        }

        if (x - 1 >= 0)
        {
            var cellToLeft = _mazeGrid[x - 1, z];

            if (cellToLeft.IsVisited == false)
            {
                yield return cellToLeft;
            }
        }

        if (z + 1 < _mazeDepth)
        {
            var cellToFront = _mazeGrid[x, z + 1];

            if (cellToFront.IsVisited == false)
            {
                yield return cellToFront;
            }
        }

        if (z - 1 >= 0)
        {
            var cellToBack = _mazeGrid[x, z - 1];

            if (cellToBack.IsVisited == false)
            {
                yield return cellToBack;
            }
        }
    }

    private void ClearWalls(MazeCell previousCell, MazeCell currentCell)
    {
        if (previousCell == null)
        {
            return;
        }

        if (previousCell.transform.position.x < currentCell.transform.position.x)
        {
            previousCell.ClearRightWall();
            currentCell.ClearLeftWall();
            return;
        }

        if (previousCell.transform.position.x > currentCell.transform.position.x)
        {
            previousCell.ClearLeftWall();
            currentCell.ClearRightWall();
            return;
        }

        if (previousCell.transform.position.z < currentCell.transform.position.z)
        {
            previousCell.ClearFrontWall();
            currentCell.ClearBackWall();
            return;
        }

        if (previousCell.transform.position.z > currentCell.transform.position.z)
        {
            previousCell.ClearBackWall();
            currentCell.ClearFrontWall();
            return;
        }
    }

    private void PlaceTorches()
    {
        foreach (MazeCell cell in _mazeGrid)
        {
            if (Random.value > 0.95f) // Probabilidad del 5% de colocar una antorcha
            {
                if (cell.HasLeftWall)
                {
                    Instantiate(_torchPrefab, cell.transform.position + new Vector3(-0.5f, 1f, 0), Quaternion.identity);
                }
                if (cell.HasRightWall)
                {
                    Instantiate(_torchPrefab, cell.transform.position + new Vector3(0.5f, 1f, 0), Quaternion.identity);
                }
                if (cell.HasFrontWall)
                {
                    Instantiate(_torchPrefab, cell.transform.position + new Vector3(0, 1f, 0.5f), Quaternion.identity);
                }
                if (cell.HasBackWall)
                {
                    Instantiate(_torchPrefab, cell.transform.position + new Vector3(0, 1f, -0.5f), Quaternion.identity);
                }
            }
        }
    }

    private void PlaceZombies()
    {
        int zombiesPlaced = 0;
        while (zombiesPlaced < _numberOfZombies)
        {
            int x = Random.Range(0, _mazeWidth);
            int z = Random.Range(0, _mazeDepth);
            MazeCell randomCell = _mazeGrid[x, z];

            // Ensure the cell is walkable
            if (randomCell.IsWalkable())
            {
                GameObject zombie = Instantiate(_zombiePrefab, randomCell.transform.position + new Vector3(0, 1, 0), Quaternion.identity);
                NavMeshAgent navMeshAgent = zombie.GetComponent<NavMeshAgent>();

                if (navMeshAgent != null)
                {
                    navMeshAgent.Warp(randomCell.transform.position);
                }
                else
                {
                    Debug.LogError("El prefab de zombi no tiene un componente NavMeshAgent.");
                }

                zombiesPlaced++;
            }
        }
    }

    private void PlaceStar()
    {
        int x = Random.Range(0, _mazeWidth);
        int z = Random.Range(0, _mazeDepth);
        MazeCell randomCell = _mazeGrid[x, z];

        // Asegura que la celda sea transitable
        if (randomCell.IsWalkable())
        {
            GameObject star = Instantiate(_starPrefab, randomCell.transform.position + new Vector3(0, 1, 0), Quaternion.identity);

            // Pasar el panel de "WIN" a la estrella
            Star starScript = star.GetComponent<Star>();
            if (starScript != null)
            {
                starScript.winPanel = _winPanel;
            }
        }
    }
}
 
using System;
using System.Collections;
using System.Collections.Generic;
using FungiScripts;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FungalGridHandler : MonoBehaviour
{
    private FungusNetwork network;
    private ResourceNetwork _resourceNetwork;
    private int _internalCounter = 0;
    private TextureAssigner _textureAssigner;
    private GridLayout _gridLayout;
    private Camera _camera;
    private bool _simulationStarted = false;
    private bool _drawingEnabled = false;
    private bool _feedingEnabled = false;
    [SerializeField] private Tilemap _tilemap;
    [SerializeField] private Tile _resourceTileTexture;
    [SerializeField] [Range(0f, 1f)] private float _refreshTimer = 1f;


    private float timer;

    private void Awake()
    {
        _resourceTileTexture = Resources.Load<Tile>("FungiAssets/WhiteBase");
        _gridLayout = GetComponentInParent<GridLayout>();
        _textureAssigner = GetComponent<TextureAssigner>();
        _camera = Camera.main;
    }

    private void OnDrawGizmos()
    {
        if (network == null) return;
        foreach (var cell in network.GetAllCells())
        {
            var pos = _gridLayout.CellToWorld(cell.GetCoordsAsVector());
            Gizmos.color = new Color(0, 0.2f, 0.6f, 0.5f);
            var size = Mathf.Log(cell.GetResourceAmount() + 1, 10) / 2f;
            Gizmos.DrawSphere(pos, size);
        }
    }

    private void HandleObstacleDrawing()
    {
        if (!_drawingEnabled) return;
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) return;
        if (Input.GetMouseButton(0))
        {
            var mousePos = _camera.ScreenToWorldPoint(Input.mousePosition);
            var cellPos = _gridLayout.WorldToCell(mousePos);
            cellPos.z = 0;
            var hasTile = _tilemap.HasTile(cellPos);
            if (!hasTile)
            {
                _tilemap.SetTile(cellPos, _textureAssigner.GetObstacleTile());
            }
        }
        else if (Input.GetMouseButton(1))
        {
            var mousePos = _camera.ScreenToWorldPoint(Input.mousePosition);
            var cellPos = _gridLayout.WorldToCell(mousePos);
            cellPos.z = 0;
            var hasTile = _tilemap.HasTile(cellPos);
            if (!hasTile) return;
            if (_tilemap.GetTile(cellPos) == _textureAssigner.GetObstacleTile())
                _tilemap.SetTile(cellPos, null);
        }
    }

    private void HandleFeeding()
    {
        if(!_feedingEnabled) return;
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) return;
        if (Input.GetMouseButton(0))
        {
            var mousePos = _camera.ScreenToWorldPoint(Input.mousePosition);
            var cellPos = _gridLayout.WorldToCell(mousePos);
            cellPos.z = 0;
            var hasTile = _tilemap.HasTile(cellPos);
            if (!hasTile) return;
            var cell = network.GetCellAt(cellPos);
            if (cell == null) return;
            cell.AddResource(100);
        }
    }

    void Start()
    {
        _resourceNetwork = new ResourceNetwork(_tilemap);
    }

    public void ExitSimulation()
    {
        Application.Quit();
    }
    
    public void StartSimulation()
    {
        _simulationStarted = true;
        network = new FungusNetwork(_tilemap, _resourceNetwork);
        _tilemap = GetComponent<Tilemap>();
        network.SetEnviroment(_tilemap);
        timer = _refreshTimer;
    }

    public void ResetSimulation()
    {
        _simulationStarted = false;
        _internalCounter = 0;
        _tilemap.ClearAllTiles();
        _resourceNetwork = new ResourceNetwork(_tilemap);
    }
    
    public void ToggleDrawing(bool value)
    {
        _drawingEnabled = value;
    }
    
    public void ToggleFeeding(bool value)
    {
        _feedingEnabled = value;
    }
    
    public void PauseSimulation()
    {
        _simulationStarted = !_simulationStarted;
    }
    
    public void SetSimulationSpeed(System.Single speed)
    {
        _refreshTimer = speed;
    }
    
    public void RemoveAllObstacles()
    {
        var allTiles = _tilemap.cellBounds.allPositionsWithin;
        foreach (var pos in allTiles)
        {
            if (_tilemap.GetTile(pos) == _textureAssigner.GetObstacleTile())
            {
                _tilemap.SetTile(pos, null);
            }
        }
    }

    void Update()
    {
        HandleObstacleDrawing();
        HandleFeeding();
        if(!_simulationStarted) return;
        if (timer < 0)
        {
            timer = _refreshTimer;
            network.FungiStep(_internalCounter);
            DrawAllCellsDebug();
            _internalCounter++;
        }
        else
        {
            timer -= Time.deltaTime;
        }
    }

    private void DrawAllCells()
    {
        var cells = network.GetActiveCells();
        var dying = network.GetDyingCells();
        var resourceCells = _resourceNetwork.GetActiveCells();
        foreach (var cell in cells)
        {
            var newpos = cell.GetCoordsAsVector();
            _tilemap.SetTile(newpos, MapCellToTile(cell));
        }
        foreach (var cell in dying)
        {
            var newpos = cell.GetCoordsAsVector();
            _tilemap.SetTile(newpos, MapCellToTile(cell));
        }
        foreach (var cell in resourceCells)
        {
            var newpos = cell.GetCoordsAsVector();
            _tilemap.SetTile(newpos, MapCellToTile(cell));
        }
    }

    private void DrawAllCellsDebug()
    {
        var cells = network.GetAllCells();
        var resourceCells = _resourceNetwork.GetActiveCells();
        foreach (var cell in cells)
        {
            var newpos = cell.GetCoordsAsVector();
            _tilemap.SetTile(newpos, MapCellToTile(cell));
        }
        foreach (var cell in resourceCells)
        {
            var newpos = cell.GetCoordsAsVector();
            _tilemap.SetTile(newpos, MapCellToTile(cell));
        }
    }

    private Tile MapCellToTile(FungusCell cell)
    {
        return _textureAssigner.GetTileForCell(cell);
    }
    
    private Tile MapCellToTile(ResourceCell cell)
    {
        var color = new Color(0.24f, 0.45f, 0.18f, cell.GetResourceAmount() / ResourceParameters.maxResourceAmount);
        _resourceTileTexture.color = color;

        return _resourceTileTexture;
    }
}

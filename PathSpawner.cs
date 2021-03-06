using Godot;
using System;
using System.Threading.Tasks;

public class PathSpawner : StaticBody
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    [Export]
    public PackedScene PathObject;
    [Export]
    public PackedScene PathTObject;
    [Export]
    public PackedScene PathTurnObject;
    [Export]
    public PackedScene PathFourwayObject;
    public Spatial SpawnUnder;
    private PathSpawner zRaycastObject;
    private PathSpawner xRaycastObject;
    private PathSpawner minusZRaycastObject;
    private PathSpawner minusXRaycastObject;
    private bool pathConnectX = false;
    private bool pathConnectZ = false;
    private bool pathConnectMinusX = false;
    private bool pathConnectMinusZ = false;
    bool updating = true;
    bool removing = false;
    bool readyToRemove = false;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        SpawnUnder = GetNode<Spatial>("SpawnUnder");
        
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);
        if(updating){
            InitialSetPath();
        }if(readyToRemove){
            QueueFree();
        }else if(removing){
            GetNode<CollisionShape>("CollisionShape").Disabled = true;
            setObjectsAroundSpawner();
            updateObjectsAroundSpawner();
        }
        
    }

    public async void InitialSetPath(){
        await ToSignal(GetTree(), "physics_frame");
        setPath();
        updateObjectsAroundSpawner();
        updating = false;
        if(removing)
            readyToRemove = true;
        
    }

    private void updateObjectsAroundSpawner(){
        if(zRaycastObject != null){
            zRaycastObject.setPath();
        }
        if(xRaycastObject != null){
            xRaycastObject.setPath();
        }
        if(minusXRaycastObject != null){
            minusXRaycastObject.setPath();
        }
        if(minusZRaycastObject != null){
            minusZRaycastObject.setPath();
        }
    }

    private void setObjectsAroundSpawner(){
        zRaycastObject = null;
        xRaycastObject = null;
        minusXRaycastObject = null;
        minusZRaycastObject = null;
        zRaycastObject = raycastForObject(new Vector3(0,0,2), out pathConnectZ);
        xRaycastObject = raycastForObject(new Vector3(2,0,0), out pathConnectX);
        minusXRaycastObject = raycastForObject(new Vector3(-2,0,0), out pathConnectMinusX);
        minusZRaycastObject = raycastForObject(new Vector3(0,0,-2), out pathConnectMinusZ);
    }

    private void setPath(){  

        foreach (Spatial item in SpawnUnder.GetChildren())
        {
            item.QueueFree();
        }

        setObjectsAroundSpawner();

        GD.Print(pathConnectMinusX, pathConnectMinusZ, pathConnectX, pathConnectZ);

        if(pathConnectMinusX && pathConnectMinusZ && pathConnectX && pathConnectZ){
             Spatial path = PathFourwayObject.Instance() as Spatial;
            SpawnUnder.AddChild(path);
        }else if(pathConnectMinusX && pathConnectMinusZ && pathConnectX){
            Spatial path = PathTObject.Instance() as Spatial;
            SpawnUnder.AddChild(path);
        }else if(pathConnectMinusX && pathConnectMinusZ && pathConnectZ){
            Spatial path = PathTObject.Instance() as Spatial;
            SpawnUnder.AddChild(path);
            path.RotationDegrees = new Vector3(0,90,0);
        }else if(pathConnectMinusX && pathConnectZ && pathConnectX){
             Spatial path = PathTObject.Instance() as Spatial;
            SpawnUnder.AddChild(path);
            path.RotationDegrees = new Vector3(0,-180,0);
        }else if( pathConnectZ && pathConnectMinusZ && pathConnectX){
             Spatial path = PathTObject.Instance() as Spatial;
            SpawnUnder.AddChild(path);
            path.RotationDegrees = new Vector3(0,-90,0);
        }else if(pathConnectMinusX && pathConnectMinusZ){
            Spatial path = PathTurnObject.Instance() as Spatial;
            SpawnUnder.AddChild(path);
            path.RotationDegrees = new Vector3(0,90,0);
        }else if(pathConnectMinusX && pathConnectZ){
            Spatial path = PathTurnObject.Instance() as Spatial;
            SpawnUnder.AddChild(path);
            path.RotationDegrees = new Vector3(0,-180,0);
        }else if(pathConnectX && pathConnectZ){
            Spatial path = PathTurnObject.Instance() as Spatial;
            SpawnUnder.AddChild(path);
            path.RotationDegrees = new Vector3(0,-90,0);
        }else if(pathConnectX && pathConnectMinusZ){
            Spatial path = PathTurnObject.Instance() as Spatial;
            SpawnUnder.AddChild(path);
        }else if(pathConnectMinusX || pathConnectX){
            Spatial path = PathObject.Instance() as Spatial;
            SpawnUnder.AddChild(path);
            path.RotationDegrees = new Vector3(0,90,0);
        }else if(pathConnectZ || pathConnectMinusZ){
            Spatial path = PathObject.Instance() as Spatial;
            SpawnUnder.AddChild(path);
        }else
        {
            Spatial path = PathObject.Instance() as Spatial;
            SpawnUnder.AddChild(path);
        }

        
    }

    private PathSpawner raycastForObject(Vector3 rayCastpos, out bool objectConnected){
        var spaceState = GetWorld().DirectSpaceState;
        Godot.Collections.Dictionary result = spaceState.IntersectRay(Translation, Translation + rayCastpos,
                        new Godot.Collections.Array { this });
        GD.Print(result);
        if(result.Contains("collider")){
            if(result["collider"] is PathSpawner){
                objectConnected = true;
                return result["collider"] as PathSpawner;
            }
        }
        objectConnected = false;
        return null;   
    }

    public async void RemoveObject(){
       updating = true;
        removing = true;
    }

    public void MoveObject(Vector3 position){
        updating = true;
        Translation = position;
    }

}

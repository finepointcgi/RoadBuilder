using Godot;
using System;

public class Camera : Godot.Camera
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    // Called when the node enters the scene tree for the first time.
    [Export]
    public PackedScene PathSpawnerObj;
    private PathSpawner selectedNode;
    private bool moving;
    public override void _Ready()
    {
        
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
     public override void _Process(float delta)
     {

        if (selectedNode != null && moving)
        {
            Vector3 positionToMove = (Vector3)getRayCast()["position"];
            Vector3 snappedPositionToMove = new Vector3(Mathf.Round(positionToMove.x), positionToMove.y, Mathf.Round(positionToMove.z));
            selectedNode.MoveObject(snappedPositionToMove);
        }
     
     }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if(@event is InputEventMouseButton && @event.IsPressed()){
            Godot.Collections.Dictionary intersection = getRayCast();
            if(intersection.Contains("collider")){
                if(intersection["collider"] is PathSpawner){
                    selectedNode = intersection["collider"] as PathSpawner;
                    selectedNode.GetNode<CollisionShape>("CollisionShape").Disabled = true;
                    moving = true;
                }else{
                    PathSpawner spawner = PathSpawnerObj.Instance() as PathSpawner;
                    GetTree().GetRoot().AddChild(spawner);
                    Vector3 positionToMove = (Vector3)intersection["position"];
                    GD.Print(SnapTo(positionToMove, 1));
                    spawner.Translation = SnapTo(positionToMove, 1);
                    selectedNode = spawner;
                    selectedNode.GetNode<CollisionShape>("CollisionShape").Disabled = true;
                    moving = true;
                    
                }
            }
            
        }else if (@event is InputEventMouseButton && !@event.IsPressed())
        {
            if ((@event as InputEventMouseButton).ButtonIndex == 1)
            {
                if (selectedNode != null)
                {
                    selectedNode.GetNode<CollisionShape>("CollisionShape").Disabled = false;
                    selectedNode = null;
                    moving = false;
                }
            }
        }
    }

    public Vector3 SnapTo(Vector3 value, int increment) => new Vector3(SnapTo(value.x, increment), SnapTo(value.y, increment), SnapTo(value.z, increment));
    public float SnapTo(float value, int increment) => Mathf.Round(value/increment) * increment;

    public void _on_Button_button_down(){
        if(selectedNode != null){
            selectedNode.RemoveObject();
        }
    }

    public Godot.Collections.Dictionary getRayCast(){
        PhysicsDirectSpaceState spaceState = GetWorld().DirectSpaceState;
        Vector2 mousePos = GetViewport().GetMousePosition();
        Vector3 rayOrigin = ProjectRayOrigin(mousePos);
        Vector3 rayEnd = rayOrigin + ProjectRayNormal(mousePos) * 2000;
        return spaceState.IntersectRay(rayOrigin, rayEnd);
    }
}   

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
    public override void _Ready()
    {
        
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if(@event is InputEventMouseButton && @event.IsPressed()){
            PhysicsDirectSpaceState spaceState = GetWorld().DirectSpaceState;
            Vector2 mousePos = GetViewport().GetMousePosition();
            Vector3 rayOrigin = ProjectRayOrigin(mousePos);
            Vector3 rayEnd = rayOrigin + ProjectRayNormal(mousePos) * 2000;
            Godot.Collections.Dictionary intersection = spaceState.IntersectRay(rayOrigin, rayEnd);
            if(intersection.Contains("collider")){
                if(intersection["collider"] is PathSpawner){
                    selectedNode = intersection["collider"] as PathSpawner;
                }else{
                    PathSpawner spawner = PathSpawnerObj.Instance() as PathSpawner;
                    GetTree().GetRoot().AddChild(spawner);
                    Vector3 positionToMove = (Vector3)intersection["position"];
                    GD.Print(SnapTo(positionToMove, 1));
                    spawner.Translation = SnapTo(positionToMove, 1);
                    selectedNode = spawner;
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
}   

using Fusion;

public struct PlayerInputData : INetworkInput
{

    //X AND Z POS FOR MOVEMENT
    public float x;
    public float z;
    //MOUSE DIRECTIONS FOR CAM MOVEMENT 
    public float mouseX;
    public float mouseY;
    //ACTIONS BOOLS
    public bool jump;
    public bool crouch;
    public bool run;
    public bool flashlight;
}
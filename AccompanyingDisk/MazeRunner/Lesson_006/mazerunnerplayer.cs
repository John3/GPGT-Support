//------------------------------------------------------
// Copyright 2000-2005, GarageGames.com, Inc.
// Written, modfied, or otherwise interpreted by Ed Maurina, Hall Of Worlds, LLC
//------------------------------------------------------
echo("\n\c3--------- Loading MazeRunner Player Datablock Definitions and Scripts ---------");

function rldplayer() {
   exec("./mazerunnerplayer.cs");
}

//-------------------------------------------------------------------------
//             MazeRunner Datablock Definition
//-------------------------------------------------------------------------
datablock PlayerData( MazeRunner : BasePlayer )
{
    shapeFile            = "~/data/MazeRunner/Shapes/Players/MazeRunner.dts";
    boundingBox          = "1.6 1.6 2.3"; 
    useEyePoint          = false;   
    pickupRadius         = 0.75;
    runForce             = 60;
    jumpForce            = 60; 
    drag                 = 0;
    mass                 = 1;

    invincible           = true;

    groundImpactMinSpeed = 1000; 
    ImpactMinSpeed       = 1000; 

    renderFirstPerson    = false; 

    observeThroughObject = true; 
    firstPersonOnly      = false;

    cameraDefaultFov     = 90.0;
    cameraMinFov         = 45.0;
    cameraMaxFov         = 120.0;
    cameraMinDist        = 18.0;
    cameraMaxDist        = 25.0;
    minLookAngle         = -1.73;
    maxLookAngle         = 1.73; 
    maxFreelookAngle     = 2.1;	
};

// ******************************************************************
//					onAdd() 
// ******************************************************************
//
function MazeRunner::onAdd( %DB , %Obj )
{
   Parent::onAdd( %DB , %Obj );
}

// ******************************************************************
//					onRemove() 
// ******************************************************************
//
function MazeRunner::onRemove( %DB , %Obj )
{
   Parent::onRemove( %DB , %Obj );
}
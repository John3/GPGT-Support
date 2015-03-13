//------------------------------------------------------
// Copyright 2000-2006, GarageGames.com, Inc.
// Written by Ed Maurina, Hall Of Worlds, LLC
//------------------------------------------------------
echo("\c3--------- Loading GuiProgressCtrl Samples ---------");
//--------------------------------------------------------------------------
// gsProgress.cs
//--------------------------------------------------------------------------
//--------------------------------------------------------------------------
// 
// In this file, we demonstrate the use of the GuiProgressCtrl.
//
// This is a pretty trivial example.  Basically, there are serveral bars with 
// differnt profile settings.  
//
// Each bar will initialize using the same onAdd() and onWake() methods.
//
// Each bar will go into a schedule loop, updating itself
// with both a 'filled value' and a text value.
//
// This repeats forever....till the sample is closed.
//

//--------------------------------------------------------------------------
// Profiles
//--------------------------------------------------------------------------
if(!isObject(gsTestProgressProfile0)) new GuiControlProfile (gsTestProgressProfile0)
{
   opaque			= true;
   fillColor		= "44 152 162 255";

   border			= 0;
   borderThickness	= 6;
   borderColor		= "78 88 120";
};

if(!isObject(gsTestProgressProfile1)) new GuiControlProfile (gsTestProgressProfile1 : gsTestProgressProfile0)
{
   border			= 2;
};

if(!isObject(gsTestProgressProfile2)) new GuiControlProfile (gsTestProgressProfile2 : gsTestProgressProfile0)
{
   opaque			= false;
   border			= 4;
   fillColor		= "44 152 162 64";
};

if(!isObject(gsTestProgressProfile3)) new GuiControlProfile (gsTestProgressProfile3 : gsTestProgressProfile0)
{
   fillColor		= "255 255 255 255";
};

if(!isObject(gsTestProgressProfile4)) new GuiControlProfile (gsTestProgressProfile4 : gsTestProgressProfile0)
{
   opaque			= false;
   fillColor		= "255 255 255 100";
};

if(!isObject(gsTestProgressProfile5)) new GuiControlProfile (gsTestProgressProfile5 : gsTestProgressProfile0)
{
   fillColor		= "255 33 33 255";
};

if(!isObject(gsTestProgressProfile6)) new GuiControlProfile (gsTestProgressProfile6 : gsTestProgressProfile0)
{
   opaque			= false;
   fillColor		= "33 255 33 200";
};

if(!isObject(gsTestProgressProfile7)) new GuiControlProfile (gsTestProgressProfile7 : gsTestProgressProfile0)
{
   fillColor		= "33 33 255 255";
};

if(!isObject(gsTestProgressProfile8)) new GuiControlProfile (gsTestProgressProfile8 : gsTestProgressProfile0)
{
   opaque			= false;
   fillColor		= "255 33 255 200";
};


//--------------------------------------------------------------------------
// Scripts
//--------------------------------------------------------------------------
//TestProgress


function TestProgress::onWake( %this ) {
   %this.lastSchedule = %this.schedule( 32, incrementMe );
}

function TestProgress::onSleep( %this ) {
	cancel( %this.lastSchedule );
}

function TestProgress::incrementMe( %this ) {
	%this.setValue( %this.getValue() + 0.01 );

	if(%this.getValue() >= 1.0) %this.setValue( 0.0 ); // actually limits at 1.0
	
	%this.lastSchedule = %this.schedule( 32, incrementMe );
}


//--------------------------------------------------------------------------
// Load Interface Definition
//--------------------------------------------------------------------------
exec("./gsProgress.gui");


//------------------------------------------------------
// Copyright 2000-2006, GarageGames.com, Inc.
// Written by Ed Maurina, Hall Of Worlds, LLC
//------------------------------------------------------
echo("\c3--------- Loading GuiFadeInBitmapCtrl Samples ---------");
//--------------------------------------------------------------------------
// gsFadeInBitmap.cs
//--------------------------------------------------------------------------
//--------------------------------------------------------------------------
// 
// In this file, we demonstrate, the following GuiFadeInBitmapCtrl features:
//
// 1. Fade Times
// 2. Re-fading
//

//--------------------------------------------------------------------------
// Profiles
//--------------------------------------------------------------------------

//--------------------------------------------------------------------------
// Scripts
//--------------------------------------------------------------------------

//
// 1. Fade Times - In order to test fade times, we have used some sliders
//                 to gather data.  Then we use a common method to update
//                 the times stored in the actual fade-in bitmap.
//                 For feedback sake, we have some images showing the state
//                 of the fade and showing when it completes.  
//
// 


function gsTestFadeInBitmap::onWake( %this ) {

	// Need to clear this as it only gets set to true by the control
	%this.done = false; 
	
	// Set feedback images to the proper state
	gsIsFading.setBitmap(gsIsFading.lightOn);
	gsIsDoneFading.setBitmap(gsIsDoneFading.lightOff);
}


//
// This function is used to gather slider information and 
//

function GuiSliderCtrl::updateFade( %this ) {
	gsTestFadeInBitmap.fadeInTime	= 1000 * gsFadeInAdjust.getValue();
	gsTestFadeInBitmap.fadeOutTime	= 1000 * gsFadeOutAdjust.getValue();
	gsTestFadeInBitmap.waitTime		= 1000 * gsWaitAdjust.getValue();
}


function gsTestFadeInBitmap::onSleep( %this ) {
	cancel(%this.lastSchedule);
}


function gsTestFadeInBitmap::checkIsDone( %this ) {
	if( %this.done ) {

		gsIsFading.setBitmap(gsIsFading.lightOff);
		gsIsDoneFading.setBitmap(gsIsDoneFading.lightOn);
	}
	else {
	   %this.lastSchedule = %this.schedule( 100, checkIsDone );
	}
}


// 2. Re-fading - Because there is no direct way to re-fade a GuiFadeInBitmapCtrl
//                we have to make the control go to sleep and wake-up again.
//                This is done by making the gsFadeInBitmap sample the content
//                of the canvas again.  i.e. all elements go to sleep, then 
//                wake up again. See the definition of TestFadeButton in the .gui 
//                file for this sample.
//
// Button does this --> command = "Canvas.setContent(gsFadeInBitmap);";


//--------------------------------------------------------------------------
// Load Interface Definition
//--------------------------------------------------------------------------
exec("./gsFadeInBitmap.gui");

//--------------------------------------------------------------------------
// More Scripts
//--------------------------------------------------------------------------

// Note: This is not the best way to initialize GUIs.  It is better to use the 
//       onAdd() or onWake() callbacks, but if need be, you can do initializations
//       in a flat script as below:

gsIsFading.lightOn = expandFilename("./lighton");
gsIsFading.lightOff = expandFilename("./lightoff");
gsIsDoneFading.lightOn = expandFilename("./lighton");
gsIsDoneFading.lightOff = expandFilename("./lightoff");



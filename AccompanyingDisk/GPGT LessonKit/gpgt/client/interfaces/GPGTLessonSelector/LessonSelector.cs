//------------------------------------------------------
// Copyright 2000-2006, GarageGames.com, Inc.
// Written by Ed Maurina, Hall Of Worlds, LLC
//------------------------------------------------------
function rldls() // Reloader for lesson lesson selector scripts
{
   exec("./lessonSelector.cs");
}

function toggleLessonSelectorDialogMAC( ){
   toggleLessonSelectorDialog();
}
function toggleLessonSelectorDialog( ){

   if( lessonSelector.isOpen )
   {
      lessonSelector.setActive(false);
      Canvas.popDialog(lessonSelector);
      lessonSelector.isOpen = false;
   } 
   else 
   {
      lessonSelector.setActive(true);
      Canvas.pushDialog(lessonSelector);
      lessonSelector.isOpen = true;
   }
}


function clientCmdloadActionMap( %path ) {
   echo("\c3clientCmdloadActionMap( ", %path , " )" );
   exec( %path );
}

//// ------------------------------------------------------------------------
////	LessonSelector Methods
//// ------------------------------------------------------------------------
function LessonSelector::onAdd( %this ) 
{
   // Start off w/ no lessons enabled
   if( isObject( Vol1EnableLessonButton ) && Vol1EnableLessonButton.getValue() ) Vol1EnableLessonButton.performClick();
   if( isObject( Vol2EnableLessonButton ) && Vol2EnableLessonButton.getValue() ) Vol2EnableLessonButton.performClick();
   if( isObject( QAEnableLessonButton ) && QAEnableLessonButton.getValue() ) QAEnableLessonButton.performClick();
   if( isObject( CommunityEnableLessonButton ) && CommunityEnableLessonButton.getValue() ) CommunityEnableLessonButton.performClick();

   %this.tileBitmap[0,0]   = expandFilename("./blanklessontile.png");
   %this.tileBitmap[0,1]   = expandFilename("./blanklessontile_sel.png");
   %this.tileBitmap[1,0]   = expandFilename("./blanklessontile.png");
   %this.tileBitmap[1,1]   = expandFilename("./blanklessontile_sel.png");
   %this.tileBitmap[2,0]   = expandFilename("./blanklessontile.png");
   %this.tileBitmap[2,1]   = expandFilename("./blanklessontile_sel.png");

}
//
// Load the currently selected lesson...
//
function LessonSelector::GotoLesson( %this ) 
{
   if( "" !$= %this.lessonObject[%this.currentSelectedLesson] )
   {
      commandToServer('LoadLesson', %this.lessonObject[%this.currentSelectedLesson]);

      // Load any action map associated with this lesson
      if( isObject(lessonActionMap) )
      {
         lessonActionMap.pop();
         lessonActionMap.delete();
      }

      if("" !$= %this.clientScriptPath[%this.currentSelectedLesson])
      {
         commandToServer( 'AllClientsLoadActionMap', 
         %this.clientScriptPath[%this.currentSelectedLesson] );
      }
         
      return true;
   }
   return false;
}

function LessonSelector::onWake( %this ) 
{

   // Temporarily disable the movemap
   movemap.pop();

   // Be sure a lesson 'tile' is selected, if none is, select 0
   if(tile0.Selected ==
      tile1.Selected ==	   
      tile2.Selected == false) {

         tile0.Selected = true;

         %this.schedule(100, setCurrentSelectedLesson, 0 );
         //%this.setCurrentSelectedLesson( 0 );

      }
}   

function LessonSelector::onSleep( %this ) {
   // We're leaving.  Turn the movemap back on.
   movemap.push();

   lessonSelector.isOpen = false;
}   

// Helper function to set currently selected lesson ID
function LessonSelector::setCurrentSelectedLesson( %this , %lessonID ) {
   %this.currentSelectedLesson = %lessonID;
}


//// ------------------------------------------------------------------------
////	Mouse Event Tile Methods (guiMouseCtrl controls)
//// ------------------------------------------------------------------------
function tileMouseEvent::onMouseUp( %this , %eventModifier , %XY, %numMouseClicks ) {
   
   if( %this.tileNum >= lessonSelector.totalLessons ) return;
   
   %tile = (tile@%this.tileNum).getid();

   %tile.selectTile();
   
   if(%numMouseClicks > 1){
      if( true == LessonSelector.GotoLesson() )
      {
         Canvas.popDialog(LessonSelector);
         %this.setActive(false);
      }
   }
}

//// ------------------------------------------------------------------------
////	Lesson Image Tile Methods (guiBitmap controls)
//// ------------------------------------------------------------------------

//
// Named 'onWake' methods to update the images for each tile. I could have
// made these in the GuiBitmapCtrl namespace, but I didn't want to 'pollute'.
// 
function tile0::onWake( %this  ) {
   %this.updateTileImage();
}
function tile1::onWake( %this  ) {
   %this.updateTileImage();
}
function tile2::onWake( %this  ) {
   %this.updateTileImage();
}

//
// This method draws the proper image (i.e. sets the bitmap), or if no 
// image is provided, uses the blank tile image and sets the value
// of the tile's guiMLText control (a child) to the lesson name.
// This allows the end-user (that's you) to not supply a image).
// Cool eh?
//
// Yes, I polluted in this case, but hey it was more clear this way.
//
function GuiBitmapCtrl::updateTileImage( %this ) {

   // Display 'TileText' for blank lesson tiles so we know what the lesson is
   // This is for user added lessons w/ no tile image
   if( -1 != strstr( lessonSelector.tileBitmap[%this.currentLesson,%this.Selected] , "blank" ) ) 
   {
      %this.getObject(0).setVisible(true);
   }	
   else  
   {
      %this.getObject(0).setVisible(false);
   }

   %this.getObject(0).setText(lessonSelector.lessonName[%this.currentLesson]);
   %this.setBitmap(lessonSelector.tileBitmap[%this.currentLesson,%this.Selected]);

   if(%this.Selected) updateLessonDescriptionPane( %this.currentLesson );

   if(%this.Selected) LessonSelector.setCurrentSelectedLesson( %this.currentLesson );

}

//
//  This method is a helper, called by the guiMouseCtrl controls to
//  'select' a tile.  We could have skipped all this and used guiButtonCtrl, but
//  then four images would have been required, which is way too much work.
//  
//
function GuiBitmapCtrl::selectTile( %this ) {
   
   %maxCount = (lessonSelector.totalLessons > 2) ? 3 : lessonSelector.totalLessons;
  
   //for(%count = 0; %count < 3; %count++) {
   for(%count = 0; %count < %maxCount; %count++) {
      %tile = (tile@%count).getid();

      if(%this == %tile) {
         %tile.Selected = true;
      } else {
         %tile.Selected = false;
      }
      %tile.updateTileImage();
   }
}


//// ------------------------------------------------------------------------
////	Utility Functions
//// ------------------------------------------------------------------------

//
// This function updates the text for our lesson selector's lessonDescription
// control, which is a guiMLText control.  I could have made this a method,
// but there was no point.  Updates can be adequately done either way.
//
function updateLessonDescriptionPane( %LessonID ){
   LessonDescription.setValue("");

   //echo("\c3", lessonSelector.MLPath[%LessonID]);

   %file = new FileObject();
   if( %file.openForRead( lessonSelector.MLPath[%LessonID]) ) 
   {
      while( !%file.isEOF() )
      {
         LessonDescription.addText( %file.readline() , true );
      }
   }
   %file.delete();

   //LessonDescription.forceReflow();	
}

//
// This rather complicated (looking) function simply scrolls the image tiles.
// Since we actually only have three tiles, what it really does is update the 
// image info, selected info, and current lesson info for each tile.  
// You may wish to make similar controls to the lesson selector and this code
// will probably be the hardest part to write.  Whatever the case, you should
// feel free to use this as a basis for your own code unless you can think of
// a more efficient way to do it (and there are more efficient ways).
//
//
// Note:  If you write your own version of this code, remember the order of
//        increments vs. selection updates is important.
//
function scrollLessons(%direction) {
   //echo("scrollLessons(",%direction,")");

   if(%direction) { // Down Button (scrolls tiles up)
      if( tile2.currentLesson >= (lessonSelector.totalLessons-1) ) {
         return;
      }
      tile0.Selected = tile1.Selected;
      tile1.Selected = tile2.Selected;
      tile2.Selected = false;

      tile0.currentLesson++;
      tile1.currentLesson++;
      tile2.currentLesson++;

      // Don't allow selection to move 'off screen'
      if(false == tile0.Selected == tile1.Selected == tile2.Selected) {
         tile0.Selected = true;
      }

   } else { // Up Button (scrolls tiles down)
      if(0 == tile0.currentLesson) {
         return;
      }
      tile2.Selected = tile1.Selected;
      tile1.Selected = tile0.Selected;
      tile0.Selected = false;

      tile0.currentLesson--;
      tile1.currentLesson--;
      tile2.currentLesson--;

      // Don't allow selection to move 'off screen'
      if(false == tile0.Selected == tile1.Selected == tile2.Selected) {
         tile2.Selected = true;
      }
   }

   //echo("tile0.currentLesson ==", tile0.currentLesson);
   //echo("tile1.currentLesson ==", tile1.currentLesson);
   //echo("tile2.currentLesson ==", tile2.currentLesson);

   tile0.updateTileImage();
   tile1.updateTileImage();
   tile2.updateTileImage();
}

function lessonSelector::reInitLessons( %this ) 
{
   
   %minCount = (lessonSelector.totalLessons > 2) ? lessonSelector.totalLessons : 3;

   for(%count = 0; %count < %minCount; %count++)
   {
      %this.lessonName[%count]     = "";
      %this.lessonObject[%count]   = "";
      %this.MLPath[%count]         = "";
      %this.tileBitmap[%count,0]   = expandFilename("./blanklessontile.png");
      %this.tileBitmap[%count,1]   = expandFilename("./blanklessontile_sel.png");
      %this.clientScriptPath[%count]  = "";
   }

   lessonSelector.totalLessons = 0;

   tile0.Selected = true;
   tile1.Selected = false;
   tile2.Selected = false;

   tile0.currentLesson=0;
   tile1.currentLesson=1;
   tile2.currentLesson=2;

   %this.initialize();

   tile0.updateTileImage();
   tile1.updateTileImage();
   tile2.updateTileImage();
}



function lessonSelector::initialize( %this ) 
{
   //echo("\n\c3--------- LessonSelector Interface Initializing.... ---------");
   //// 1. Build list of lesson maps
   //
   if(isObject(%this.mapArray)) 
      %this.mapArray.delete();

   %this.mapArray = new ScriptObject(arrayObject);

   %lastMap = findFirstFile("*/lesson.map.txt");

   while("" !$= %lastMap) 
   {
      %this.mapArray.addEntry( %lastMap );

      %lastMap = findNextFile("*/lesson.map.txt");
   }

   %this.mapArray.sort();

   //// 2. Open the lesson maps and initializae the lessonSelector.
   //

   lessonSelector.totalLessons = 0;
   
   %lessonFileCount = %this.mapArray.getCount();
   %tileCount = 0;
   for(%count=0; %count < %lessonFileCount ; %count++) 
   {
      %file  = new FileObject();
      %map = %this.mapArray.getEntry(%count);
      if( %file.openForRead( %map ) ) 
      {
         %lessonFile    = %file.readLine();

         // *****************************************************************************
         // Note: This was placed here to allow the author a quick way to turn off lesson
         //       maps while preparing the guides.  It has been left here for your use, 
         //       but the best way to remove a lesson from a kit is to move the lesson
         //       directory to some offline storage location.
         // *****************************************************************************
         //
         // Skip files w/ skipme as first line
         //
         if("skipme" $= %lessonFile) 
         {
            echo("\c5 ..... Skipping lesson map: ", %map );
            continue;
         } 
         //
         // Skip Volume 1 Lessons if not enabled
         //
         else if ( (false == Vol1EnableLessonButton.getValue()) && 
                   ( -1 != strstr( strlwr( filePath( %map ) ) , "standard" ) ) )
         {
            echo("\c5 ..... Skipping VOLUME 1 lesson map: ", %map );
         }
         ////
         //// Skip Volume 2 Lessons if not enabled
         ////
         //else if ( (false == Vol2EnableLessonButton.getValue()) && 
                   //( -1 != strstr( strlwr( filePath( %map ) ) , "volume2" ) ) )
         //{
            //echo("\c5 ..... Skipping VOLUME 2 lesson map: ", %map );
         //}
         //
         // Skip QA Lessons if not enabled
         //
         else if ( (false == QAEnableLessonButton.getValue()) && 
                   ( -1 != strstr( strlwr( filePath( %map ) ) , "qa" ) ) )
         {
            echo("\c5 ..... Skipping Q & A lesson map: ", %map );
         }
         //
         // Skip Community Lessons if not enabled
         //
         else if ( (false == CommunityEnableLessonButton.getValue()) && 
                   ( -1 != strstr( strlwr( filePath( %map ) ) , "community" ) ) )
         {
            echo("\c5 ..... Skipping COMMUNITY lesson map: ", %map );
         }
         else 
         {
            // Increment 'actual' lesson count
            lessonSelector.totalLessons++;

            echo("\c4 ..... Processing lesson map: ", %map );
            %lessonName    = %file.readLine();
            %lessonObject  = %file.readLine();
            %MLPath        = filePath( %map  ) @ "/" @ %file.readLine();
            %tileName      = %file.readLine();
            %clientScriptPath = %file.readLine();

            if ("" $= %tileName)
            {
               %tileName    = expandFilename("./blanklessontile.png");
               %tileNameSel = expandFilename("./blanklessontile_sel.png");
            }
            else 
            {
               %tileNameSel = filePath( %map ) @ "/" @ %tileName @ "_sel.png";
               %tileName    = filePath( %map ) @ "/" @ %tileName @ ".png";
            }

            if ("" !$= %clientScriptPath)
            {
               %clientScriptPath = filePath(%map ) @ "/" @ %clientScriptPath;
            }

            //echo("\c4 lessonName=> ", %lessonName); 
            //echo("\c4 lessonObject=> ", %lessonObject); 
            //echo("\c4 MLPath=> ", %MLPath); 
            //echo("\c4 tileName=> ", %tileName); 
            //echo("\c4 tileNameSel=> ", %tileNameSel); 

            %this.lessonName[%tileCount]     = %lessonName;
            %this.lessonObject[%tileCount]   = %lessonObject;
            %this.MLPath[%tileCount]         = %MLPath;
            %this.tileBitmap[%tileCount,0]   = %tileName;
            %this.tileBitmap[%tileCount,1]   = %tileNameSel;
            %this.clientScriptPath[%tileCount]  = %clientScriptPath;
            %tileCount++;
         }
      } 
      else 
      {
         error("lessonSelector::initialize() - Failed to open lesson map:", %map );
      }
      %file.delete();
   }
}


//
// This command will either:
// 1. Create and apply and action map, enabling 
//    the lessonSelector, 
//
// OR
//
// 2. Destroy the map
//
function clientCmdToggleSelectorBinding( %enable )
{

   //echo("\c3clientCmdToggleSelectorBinding..." );   
   if( %enable )
   {
      if ( isObject( lessonSelectorMap ) )   lessonSelectorMap.delete();
      new ActionMap(lessonSelectorMap);
      lessonSelectorMap.bindCmd(mouse0, "button1", "toggleLessonSelectorDialog();", "");
      lessonSelectorMap.bindCmd(keyboard, "F2", "toggleLessonSelectorDialogMAC();", "");
      
      
      lessonSelectorMap.push();
      MissionCleanup.add( lessonSelectorMap );
   } 
   else 
   {
      if ( isObject( lessonSelectorMap ) )   lessonSelectorMap.delete();
   }
}


//--------------------------------------------------------------------------
// Load Interface Definition
//--------------------------------------------------------------------------
if( isObject( LessonSelector ) )
{
   LessonSelector.delete();
}
exec("./lessonSelector.gui");

//--------------------------------------------------------------------------
// Post-Load Scripts
//--------------------------------------------------------------------------


function LessonSelector::atLeastOnButtonCheck( %this) 
{
   if( 
      !Vol1EnableLessonButton.getValue() && 
      !Vol2EnableLessonButton.getValue() && 
      !QAEnableLessonButton.getValue()   && 
      !CommunityEnableLessonButton.getValue() 
     )
   {
      Vol1EnableLessonButton.performClick();
   }
}

Vol1EnableLessonButton.command       = "LessonSelector.atLeastOnButtonCheck();lessonSelector.reInitLessons();";
Vol2EnableLessonButton.command       = "LessonSelector.atLeastOnButtonCheck();lessonSelector.reInitLessons();";
QAEnableLessonButton.command         = "LessonSelector.atLeastOnButtonCheck();lessonSelector.reInitLessons();";
CommunityEnableLessonButton.command  = "LessonSelector.atLeastOnButtonCheck();lessonSelector.reInitLessons();";
LessonSelector.atLeastOnButtonCheck();
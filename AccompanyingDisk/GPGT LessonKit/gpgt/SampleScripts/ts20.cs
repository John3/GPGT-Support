
////
//		TS20
////
function ts20() {

   %test = "Torque cool!";

   echo( %test , " has " , getWordCount( %test ) , " words." );

   %test = setWord( %test , 0 , "Torque is is" );

   echo( %test , " has " , getWordCount( %test ) , " words." );

   %test = removeWord( %test, 1 );

   echo( %test , " has " , getWordCount( %test ) , " words." );

   while ( "" !$= %test ) {

      echo( firstWord( %test ) );

      %test = restWords( %test ) ;

   }


}

ts20();
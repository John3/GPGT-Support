

////
//		TS33
////
function testRecords( %recordString ) {

   %tmpRecord = %recordString;

   echo( %tmpRecord, "\n" );


   for( %count = 0; %count < getRecordCount( %tmpRecord ); %count++ )
   {
      %theRecord = getRecord( %tmpRecord , %count );

      echo( "Current record: ", %theRecord );

      if ( %theRecord $= "test" ) 
      {
         echo("\c3Replacing records...");

         %tmpRecord = setRecord ( %tmpRecord , %count , %theRecord NL "of" NL "records." );
      }

   }

   while ( getRecordCount( %tmpRecord ) )
   {
      %concatRecordString = %concatRecordString SPC getRecord( %tmpRecord , 0 );

      %tmpRecord = removeRecord( %tmpRecord , 0 );
   }

   echo( "\n", %concatRecordString );

}


function ts33() {

   testRecords( "This" NL "is" NL "a" NL "test" );
}


ts33();
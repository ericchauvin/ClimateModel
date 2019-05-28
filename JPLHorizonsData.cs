// Copyright Eric Chauvin 2018 - 2019.


// Get ephemeris data from:
// JPL Horizons:
// https://ssd.jpl.nasa.gov/horizons.cgi

// Use settings like this:
// Ephemeris Type: VECTORS
// Coordinate Origin: Solar System Barycenter (SSB) [500@0]
// Table Settings: output units=KM-S; CSV format=YES
// Display/Output: plain text (or to a file).



using System;
using System.Text;
using System.IO;
// using System.Drawing;
// using System.Drawing.Imaging;
using System.Windows.Forms;


namespace ClimateModel
{
  class JPLHorizonsData
  {
  private MainForm MForm;
  private JPLRec[] JPLRecArray;
  private int JPLRecArrayLast = 0;
  private string FileName = "";



  public struct JPLRec
    {
    // This would be at midnight Universal Time
    // because of the .5.
    // 2458282.500000000

    public double JulianDayNumber; // JDTDB, Julian Day Number, Barycentric Dynamical Time
    public ulong CalDate; // Calendar Date (TDB),

    // Original distances in kilometers get converted
    // to meters when the data file is read.
    public double PositionX; //  X,
    public double PositionY; //  Y,
    public double PositionZ; //  Z,
    public double VelocityX; // VX,
    public double VelocityY; // VY,
    public double VelocityZ; // VZ,
    public double LightTime; //  LT
    public double Range;   //  RG Range
    public double RangeRate; //  RR Range-rate
    }



  private JPLHorizonsData()
    {
    }



  internal JPLHorizonsData( MainForm UseForm )
    {
    MForm = UseForm;

    JPLRecArray = new JPLRec[2];
    }



  private void ShowStatus( string ToShow )
    {
    if( MForm == null )
      return;

    MForm.ShowStatus( ToShow );
    }



  internal void ReadFromTextFile( string UseFileName )
    {
    FileName = UseFileName;
    if( FileName.Length < 2 )
      return;

    if( !File.Exists( FileName ))
      {
      ShowStatus( "The file does not exist." );
      ShowStatus( FileName );
      return;
      }

    try
    {
    bool IsInsideData = false;
    using( StreamReader SReader = new StreamReader( FileName ))
      {
      while( SReader.Peek() >= 0 )
        {
        string Line = SReader.ReadLine();
        if( Line == null )
          continue;

        Line = Line.Trim();
        if( Line == "" )
          continue;

        // Start of data marker:
        if( Line.StartsWith( "$$SOE" ))
          {
          IsInsideData = true;
          continue;
          }

        // End marker:
        if( Line.StartsWith( "$$EOE" ))
          {
          IsInsideData = false;
          continue;
          }

        if( !IsInsideData )
          continue;

        // ShowStatus( Line );

        JPLRec Rec = new JPLRec();

        if( !Line.Contains( "," ))
          continue;

        string[] SplitS = Line.Split( new Char[] { ',' } );

        if( SplitS.Length < 11 )
          continue;

        Rec.JulianDayNumber = GetDoubleValue( SplitS[0].Trim());
        Rec.CalDate = GetDateTimeULong( SplitS[1].Trim());
        Rec.PositionX = GetDoubleValue( SplitS[2].Trim());

        // Convert them to meters:
        Rec.PositionX *= 1000.0d;

        Rec.PositionY = GetDoubleValue( SplitS[3].Trim());
        Rec.PositionY *= 1000.0d;

        Rec.PositionZ = GetDoubleValue( SplitS[4].Trim());
        Rec.PositionZ *= 1000.0d;

        Rec.VelocityX = GetDoubleValue( SplitS[5].Trim());
        Rec.VelocityX *= 1000.0d;

        Rec.VelocityY = GetDoubleValue( SplitS[6].Trim());
        Rec.VelocityY *= 1000.0d;

        Rec.VelocityZ = GetDoubleValue( SplitS[7].Trim());
        Rec.VelocityZ *= 1000.0d;

        Rec.LightTime = GetDoubleValue( SplitS[8].Trim());

        Rec.Range = GetDoubleValue( SplitS[9].Trim());
        Rec.Range *= 1000.0d;

        Rec.RangeRate = GetDoubleValue( SplitS[10].Trim());
        Rec.RangeRate *= 1000.0d;

        AddJPLRec( Rec );
        }
      }

    // ShowStatus( " " );
    // ShowStatus( "Loaded file:" );
    // ShowStatus( FileName );
    // ShowStatus( "Records: " + JPLRecArrayLast.ToString( "N0" ));
    }
    catch( Exception Except )
      {
      ShowStatus( "Could not read the file: \r\n" + FileName );
      ShowStatus( Except.Message );
      }
    }



  private ulong GetDateTimeULong( string DateS )
    {
    // ShowStatus( " " );
    // ShowStatus( "DateS: " + DateS );
    // A.D. 2018-Jun-13 00:00:00.0000

    DateS = DateS.Replace( "A.D.", "" ).Trim();
    string[] SplitS = DateS.Split( new Char[] { ' ' } );

    if( SplitS.Length < 2 )
      return 0;

    string DatePart = SplitS[0].Trim();
    string TimePart = SplitS[1].Trim();

    // ShowStatus( "DatePart: " + DatePart );
    // ShowStatus( "TimePart: " + TimePart );

    // DatePart: 2018-Jun-13

    SplitS = DatePart.Split( new Char[] { '-' } );

    int Year = GetIntegerValue( SplitS[0] );
    int Month = MonthNameToInt( SplitS[1] );
    int Day = GetIntegerValue( SplitS[2] );

    SplitS = TimePart.Split( new Char[] { ':' } );
    int Hour = GetIntegerValue( SplitS[0] );
    int Minute = GetIntegerValue( SplitS[1] );
    double SecondsD = GetDoubleValue( SplitS[2] );

    int Second = (int)Math.Truncate( SecondsD );

    // SecondsD -= Second;
    // SecondsD = SecondsD * 1000;
    int Millisecond = 0; // (int)Math.Truncate( SecondsD );

    ECTime RecTime = new ECTime();
    RecTime.SetUTCTime( Year,
                        Month,
                        Day,
                        Hour,
                        Minute,
                        Second,
                        Millisecond );

    // ShowStatus( RecTime.ToCrudeString());

    // SetFromIndex( ulong Index )
    return RecTime.GetIndex();
    }



  private int MonthNameToInt( string MonthName )
    {
    if( MonthName == "Jan" )
      return 1;

    if( MonthName == "Feb" )
      return 2;

    if( MonthName == "Mar" )
      return 3;

    if( MonthName == "Apr" )
      return 4;

    if( MonthName == "May" )
      return 5;

    if( MonthName == "Jun" )
      return 6;

    if( MonthName == "Jul" )
      return 7;

    if( MonthName == "Aug" )
      return 8;

    if( MonthName == "Sep" )
      return 9;

    if( MonthName == "Oct" )
      return 10;

    if( MonthName == "Nov" )
      return 11;

    if( MonthName == "Dec" )
      return 12;

    return 99;
    }



  private double GetDoubleValue( string ValueS )
    {
    try
    {
    // Numbers look like: -2.179945319184925E+07,

    return Double.Parse( ValueS );
    }
    catch( Exception Except )
      {
      ShowStatus( "Exception in GetDoubleValue():" );
      ShowStatus( Except.Message );
      return 0;
      }
    }



  private int GetIntegerValue( string ValueS )
    {
    try
    {
    return Int32.Parse( ValueS );
    }
    catch( Exception Except )
      {
      ShowStatus( "Exception in GetIntegerValue():" );
      ShowStatus( Except.Message );
      return 0;
      }
    }



  internal void AddJPLRec( JPLRec ToAdd )
    {
    JPLRecArray[JPLRecArrayLast] = ToAdd;
    JPLRecArrayLast++;
    if( JPLRecArrayLast >= JPLRecArray.Length )
      {
      Array.Resize( ref JPLRecArray, JPLRecArray.Length + 1024 );
      }
    }



  internal JPLRec GetNearestRecByDateTime( ulong ToMatch )
    {
    if( JPLRecArrayLast < 1 )
      {
      JPLRec RecNone = new JPLRec();
      RecNone.CalDate = 0;
      return RecNone;
      }

    ECTime BigDate = new ECTime();
    BigDate.SetToYear2099();
    // Set NearestValue to be bigger than any
    // reasonable value.
    long NearestValue = (long)BigDate.GetIndex();

    // This should be a binary search on sorted data.
    int NearestIndex = 0;
    for( int Count = 0; Count < JPLRecArrayLast; Count++ )
      {
      JPLRec Rec = JPLRecArray[Count];
      if( Rec.CalDate > ToMatch )
        continue;

      long Diff = (long)ToMatch - (long)Rec.CalDate;
      if( Diff < NearestValue )
        {
        NearestValue = Diff;
        NearestIndex = Count;
        }
      }

    if( NearestIndex > (JPLRecArrayLast - 10))
      MessageBox.Show( "Need new data for: " + FileName, "Need Data", MessageBoxButtons.OK, MessageBoxIcon.Stop );

    return JPLRecArray[NearestIndex];
    }



  }
}

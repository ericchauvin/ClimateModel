// Copyright Eric Chauvin 2018 - 2019.


// The coordinates (the reference frame) used here are centered
// at the Solar System Barycenter.  The coordinates match up with
// NASA's JPL Horizon's Ephemeris data.  (See the JPLHorizonsData.cs
// file.)  The X, Y plane is mostly in the ecliptic plane.
// Both the Earth and the Sun have Z coordinates that are not
// zero, so they are not exactly in that Zero-Z plane.  But Z
// coordinates for all the planets are relatively small
// compared with the X, Y coordinates.

// It is a rectangular coordinate system in meters and seconds.
// It is a right-handed coordinate system.

// JPL data uses the International Celestial Reference Frame
// ICRF/J2000.0.

// On May 29th 2019 the velocity of the Earth in this system was
// Velocity.X: 27,081.0
// Velocity.Y: -11,385.5
// Velocity.Z: 0.9

// It is moving mostly in the positive X direction as it
// approaches the Summer Solstice on June 21st.  But the Earth's
// X coordinate on May 29th was still a negative number.
// Position.X: -57,625,169,877.3

// If you think of the X coordinate line as going from left to
// right, and the Y coordinate line as being vertical, then
// at the Vernal (Spring) Equinox back on March 20th 2019, the
// Y coordinate was zero, and it was as far to the left (negative)
// on the X coordinate line as it goes.  But on June 21st at
// the Summer Solstice the X value will be zero and the Y coordinate
// will be as far negative as it goes.

// The Earth is tilted about 23 degrees around the X axis.
// The Positive direction of the X axis at the Vernal Equinox
// is from the Earth toward the sun.  Because of how rotations
// are defined around an axis, it is a negative angle of tilt.
// The Earth is tilted about negative 23 degrees around the X axis.

// See the QuaternionEC.cs file for notes about the direction
// of rotation around an axis.




using System;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Media.Imaging;


namespace ClimateModel
{
  class SolarSystem
  {
  private MainForm MForm;
  private Model3DGroup Main3DGroup;
  private SpaceObject[] SpaceObjectArray;
  private int SpaceObjectArrayLast = 0;
  private const double RadiusScale = 300.0;
  private PlanetSphere Sun;
  private EarthGeoid Earth;
  private PlanetSphere Moon;
  // private PlanetSphere SpaceStation;
  // Mark Leadville's position:
  // private PlanetSphere Leadville;

  private ECTime SunTime; // Local time.
  // private ECTime SpringTime; // Spring Equinox.



  private SolarSystem()
    {
    }



  internal SolarSystem( MainForm UseForm,
                        Model3DGroup Use3DGroup )
    {
    MForm = UseForm;
    Main3DGroup = Use3DGroup;

    // The local time for the sun.
    SunTime = new ECTime();
    SunTime.SetToNow();

    // SpringTime = new ECTime();
    // InitializeTimes();

    SpaceObjectArray = new SpaceObject[2];
    AddInitialSpaceObjects();
    }



  internal void AddMinutesToSunTime( int Minutes )
    {
    SunTime.AddMinutes( Minutes );
    }



/*
  private void InitializeTimes()
    {
    SunTime.SetToNow();

    // https://en.wikipedia.org/wiki/March_equinox

    // "Spring Equinox 2018 was at 10:15 AM on
    // March 20."
    SpringTime.SetUTCTime( 2018,
                            3,
                            20,
                            10,
                            15,
                            0,
                            0 );


    }
*/





/*
  private void SetPositionsAndTime( ECTime SetTime )
    {
    SunTime.Copy( SetTime );

    }
*/




  private void AddInitialSpaceObjects()
    {
    try
    {
    // Sun:
    string JPLFileName = "C:\\Eric\\ClimateModel\\EphemerisData\\JPLSun.txt";
    Sun = new PlanetSphere( MForm, "Sun", true, JPLFileName );
    Sun.Radius = 695700 * ModelConstants.TenTo3;
    Sun.Mass = ModelConstants.MassOfSun;
    Sun.TextureFileName = "C:\\Eric\\ClimateModel\\bin\\Release\\sun.jpg";
    AddSpaceObject( Sun );

    // Earth:
    JPLFileName = "C:\\Eric\\ClimateModel\\EphemerisData\\JPLEarth.txt";
    Earth = new EarthGeoid( MForm, "Earth", JPLFileName );
    Earth.Mass = ModelConstants.MassOfEarth;
    Earth.TextureFileName = "C:\\Eric\\ClimateModel\\bin\\Release\\earth.jpg";
    AddSpaceObject( Earth );

    // Moon:
    JPLFileName = "C:\\Eric\\ClimateModel\\EphemerisData\\JPLMoon.txt";
    Moon = new PlanetSphere( MForm, "Moon", false, JPLFileName );
    // Radius: About 1,737.1 kilometers.
    Moon.Radius = 1737100;
    Moon.Mass = ModelConstants.MassOfMoon;
    Moon.TextureFileName = "C:\\Eric\\ClimateModel\\bin\\Release\\moon.jpg";
    AddSpaceObject( Moon );

    // Space Station:
    // Both Earth and the Space Station need to be
    // using the same time intervals for the JPL
    // data.
    // JPLFileName = "C:\\Eric\\ClimateModel\\EphemerisData\\JPLSpaceStation.txt";
    // SpaceStation = new PlanetSphere( MForm, "Space Station", false, JPLFileName );
    // It's about 418 kilometers above the Earth.
    // SpaceStation.Radius = 400000; // 418000;
    // SpaceStation.Mass = 1.0;
    // SpaceStation.TextureFileName = "C:\\Eric\\ClimateModel\\bin\\Release\\TestImage2.jpg";
    // AddSpaceObject( SpaceStation );

/*
I would need to set this position after getting
Earth rotation angle and all that.
    // Leadville marker:
    JPLFileName = "";
    Leadville = new PlanetSphere( MForm, "Leadville", false, JPLFileName );
    Leadville.Radius = 1000000;
    Leadville.Mass = 0;
    Leadville.TextureFileName = "C:\\Eric\\ClimateModel\\bin\\Release\\TestImage.jpg";
    AddSpaceObject( Leadville );
*/

    // Mercury:
    JPLFileName = "C:\\Eric\\ClimateModel\\EphemerisData\\JPLMercury.txt";
    PlanetSphere Mercury = new PlanetSphere(
              MForm, "Mercury", false, JPLFileName );

    Mercury.Radius = 2440000d * RadiusScale;
    Mercury.Mass = ModelConstants.MassOfMercury;
    Mercury.TextureFileName = "C:\\Eric\\ClimateModel\\bin\\Release\\Mercury.jpg";
    AddSpaceObject( Mercury );

    // Venus:
    JPLFileName = "C:\\Eric\\ClimateModel\\EphemerisData\\JPLVenus.txt";
    PlanetSphere Venus = new PlanetSphere(
                MForm, "Venus", false, JPLFileName );

    Venus.Radius = 6051000 * RadiusScale; // 6,051 km
    Venus.Mass = ModelConstants.MassOfVenus;
    Venus.TextureFileName = "C:\\Eric\\ClimateModel\\bin\\Release\\Venus.jpg";
    AddSpaceObject( Venus );

    // Mars:
    JPLFileName = "C:\\Eric\\ClimateModel\\EphemerisData\\JPLMars.txt";
    PlanetSphere Mars = new PlanetSphere(
                 MForm, "Mars", false, JPLFileName );

    Mars.Radius = 3396000 * RadiusScale;
    Mars.Mass = ModelConstants.MassOfMars;
    Mars.TextureFileName = "C:\\Eric\\ClimateModel\\bin\\Release\\mars.jpg";
    AddSpaceObject( Mars );


    // Jupiter:
    JPLFileName = "C:\\Eric\\ClimateModel\\EphemerisData\\JPLJupiter.txt";
    PlanetSphere Jupiter = new PlanetSphere(
              MForm, "Jupiter", false, JPLFileName );

    //                m  t
    Jupiter.Radius = 69911000d * RadiusScale; // 69,911 km
    Jupiter.Mass = ModelConstants.MassOfJupiter;
    Jupiter.TextureFileName = "C:\\Eric\\ClimateModel\\bin\\Release\\Jupiter.jpg";
    AddSpaceObject( Jupiter );

    // Saturn:
    JPLFileName = "C:\\Eric\\ClimateModel\\EphemerisData\\JPLSaturn.txt";
    PlanetSphere Saturn = new PlanetSphere(
                MForm, "Saturn", false, JPLFileName );

    //               m  t
    Saturn.Radius = 58232000d * RadiusScale; // 58,232 km
    Saturn.Mass = ModelConstants.MassOfSaturn;
    Saturn.TextureFileName = "C:\\Eric\\ClimateModel\\bin\\Release\\Saturn.jpg";
    AddSpaceObject( Saturn );

    // North Pole:
    PlanetSphere NorthPole = new PlanetSphere(
                MForm, "North Pole", false, "" );

    NorthPole.Radius = 400000d;
    NorthPole.Mass = 0;
    NorthPole.TextureFileName = "C:\\Eric\\ClimateModel\\bin\\Release\\TestImage2.jpg";
    AddSpaceObject( NorthPole );

    // Moon Axis:
    PlanetSphere MoonAxis = new PlanetSphere(
                MForm, "Moon Axis", false, "" );

    MoonAxis.Radius = 200000d;
    MoonAxis.Mass = 0;
    MoonAxis.TextureFileName = "C:\\Eric\\ClimateModel\\bin\\Release\\TestImage2.jpg";
    AddSpaceObject( MoonAxis );


    // Solar North:
    PlanetSphere SolarNorth = new PlanetSphere(
                MForm, "Solar North", false, "" );

    SolarNorth.Radius = 100000d;
    SolarNorth.Mass = 0;
    SolarNorth.TextureFileName = "C:\\Eric\\ClimateModel\\bin\\Release\\TestImage2.jpg";
    AddSpaceObject( SolarNorth );


    ///////////////////////////////////
    // Set the positions of the objects from the JPL data.
    SetJPLTimes();


    // Now that the Earth's position has been set...

    Vector3.Vector NorthPoleUnit = Earth.GetNorthPoleVector();

    Vector3.Vector NorthPolePos =
          Vector3.MultiplyWithScalar( NorthPoleUnit,
         ModelConstants.EarthRadiusMinor + NorthPole.Radius );

    NorthPolePos = Vector3.Add( NorthPolePos, Earth.Position );
    NorthPole.Position = NorthPolePos;


    // Set the moon axis.
    Vector3.Vector MoonFirstPosition = Moon.Position;
    // Subtract the Earth's position so this is just in relation
    // to the Earth.
    MoonFirstPosition = Vector3.Subtract( MoonFirstPosition, Earth.Position );

    ECTime OneWeek = new ECTime( SunTime.GetIndex());
    int SevenDays = 7 * 24 * 60;
    OneWeek.AddMinutes( SevenDays );

    JPLHorizonsData.JPLRec Rec =
           Moon.JPLData.GetNearestRecByDateTime( OneWeek.GetIndex());

    // if( Rec.CalDate == 0 ) then it's not found.

    Vector3.Vector MoonSecondPosition;
    MoonSecondPosition.X = Rec.PositionX;
    MoonSecondPosition.Y = Rec.PositionY;
    MoonSecondPosition.Z = Rec.PositionZ;

    // Get the Earth's position a week from now.
    Rec = Earth.JPLData.GetNearestRecByDateTime( OneWeek.GetIndex());

    Vector3.Vector EarthSecondPosition;
    EarthSecondPosition.X = Rec.PositionX;
    EarthSecondPosition.Y = Rec.PositionY;
    EarthSecondPosition.Z = Rec.PositionZ;


    MoonSecondPosition = Vector3.Subtract( MoonSecondPosition, EarthSecondPosition );

    // MoonFirstPosition = Vector3.Normalize( MoonFirstPosition );
    // MoonSecondPosition = Vector3.Normalize( MoonSecondPosition );

    Vector3.Vector MoonAxisPos = Vector3.CrossProduct(
                                           MoonFirstPosition,
                                           MoonSecondPosition );

    MoonAxisPos = Vector3.Normalize( MoonAxisPos );
    MoonAxisPos = Vector3.MultiplyWithScalar( MoonAxisPos,
              ModelConstants.EarthRadiusMinor + MoonAxis.Radius );

    MoonAxisPos = Vector3.Add( MoonAxisPos, Earth.Position );

    MoonAxis.Position = MoonAxisPos;


    Vector3.Vector SolarNorthPos;
    SolarNorthPos.X = 0;
    SolarNorthPos.Y = 0;
    SolarNorthPos.Z = 1;
    SolarNorthPos = Vector3.MultiplyWithScalar( SolarNorthPos,
              ModelConstants.EarthRadiusMinor + SolarNorth.Radius );

    SolarNorthPos = Vector3.Add( SolarNorthPos, Earth.Position );
    SolarNorth.Position = SolarNorthPos;



    }
    catch( Exception Except )
      {
      MForm.ShowStatus( "Exception in SolarSystem.AddMercury(): " + Except.Message );
      }
    }



  internal void SetJPLTimes()
    {
    SetToJPLTimePosition( SunTime.GetIndex() );

    SetEarthRotationAngle();

    MakeNewGeometryModels();
    }



  private void SetEarthRotationAngle()
    {
    // Earth: Sidereal period, hr  = 23.93419

    Vector3.Vector AlongX;
    AlongX.X = 1;
    AlongX.Y = 0;
    AlongX.Z = 0;

    Vector3.Vector EarthToSun = Earth.Position;

    // Make a vector that goes from the Earth to
    // the center of the coordinate system.
    EarthToSun = Vector3.Negate( EarthToSun );

    // Add the vector from the center of the
    // coordinate system to the sun.
    EarthToSun = Vector3.Add( EarthToSun, Sun.Position );

    // This is now the vector from the Earth to the
    // sun.

    // Set Z to zero so it's only the rotation
    // around the Z axis.
    EarthToSun.Z = 0;

    ShowStatus( " " );
    ShowStatus( "EarthToSun.X: " + EarthToSun.X.ToString( "N2" ));
    ShowStatus( "EarthToSun.Y: " + EarthToSun.Y.ToString( "N2" ));
    // ShowStatus( "EarthToSun.Z: " + EarthToSun.Z.ToString( "N2" ));

    EarthToSun = Vector3.Normalize( EarthToSun );

    // The dot product of two normalized vectors.
    double Dot = Vector3.DotProduct(
                              AlongX,
                              EarthToSun );

    double SunAngle = Math.Acos( Dot );
    double HalfPi = Math.PI / 2.0;
    ShowStatus( "Dot: " + Dot.ToString( "N2" ));
    ShowStatus( "SunAngle: " + SunAngle.ToString( "N2" ));
    ShowStatus( "HalfPi: " + HalfPi.ToString( "N2" ));

    // EarthToSun.X: -68,463,078,802.05
    // EarthToSun.Y: 135,732,403,641.45
    // Dot: -0.45
    // SunAngle: 2.04
    // HalfPi: 1.57
    // Hours: 6.93

    double Hours = SunTime.GetHour();
    double Minutes = SunTime.GetMinute();
    Minutes = Minutes / 60.0d;
    Hours = Hours + Minutes;
    Hours -= 12.0;
    ShowStatus( "Hours: " + Hours.ToString( "N2" ));

    double HoursInRadians = NumbersEC.DegreesToRadians( Hours * (360.0d / 24.0d) );
    Earth.UTCTimeRadians = HoursInRadians + SunAngle;

    // Make a new Earth geometry model before
    // calling reset.
    Earth.MakeNewGeometryModel();
    ResetGeometryModels();

    // MakeNewGeometryModels();
    }




  internal void AddSpaceObject( SpaceObject ToAdd )
    {
    SpaceObjectArray[SpaceObjectArrayLast] = ToAdd;
    SpaceObjectArrayLast++;
    if( SpaceObjectArrayLast >= SpaceObjectArray.Length )
      {
      Array.Resize( ref SpaceObjectArray, SpaceObjectArray.Length + 16 );
      }
    }



  private void ShowStatus( string ToShow )
    {
    if( MForm == null )
      return;

    MForm.ShowStatus( ToShow );
    }



  internal void SetToJPLTimePosition( ulong TimeIndex )
    {
    for( int Count = 0; Count < SpaceObjectArrayLast; Count++ )
      {
      SpaceObjectArray[Count].
                 SetToNearestJPLPosition( TimeIndex );

      }
    }



  internal void MakeNewGeometryModels()
    {
    Main3DGroup.Children.Clear();

    for( int Count = 0; Count < SpaceObjectArrayLast; Count++ )
      {
      SpaceObjectArray[Count].MakeNewGeometryModel();
      GeometryModel3D GeoMod = SpaceObjectArray[Count].GetGeometryModel();
      if( GeoMod == null )
        continue;

      Main3DGroup.Children.Add( GeoMod );
      }

    SetupAmbientLight();
    SetupSunlight();
    }



  internal void ResetGeometryModels()
    {
    Main3DGroup.Children.Clear();

    for( int Count = 0; Count < SpaceObjectArrayLast; Count++ )
      {
      GeometryModel3D GeoMod = SpaceObjectArray[Count].GetGeometryModel();
      if( GeoMod == null )
        continue;

      Main3DGroup.Children.Add( GeoMod );
      }

    SetupAmbientLight();
    SetupSunlight();
    }



  private void SetupSunlight()
    {
    // Lights are Model3D objects.
    // System.Windows.Media.Media3D.Model3D
    //   System.Windows.Media.Media3D.Light

    // double OuterDistance = 1.5;

    double X = Sun.Position.X * ModelConstants.ThreeDSizeScale;
    double Y = Sun.Position.Y * ModelConstants.ThreeDSizeScale;
    double Z = Sun.Position.Z * ModelConstants.ThreeDSizeScale;
    // double RadiusScaled = Sun.Radius * ModelConstants.ThreeDSizeScale;

    SetupPointLight( X,
                     Y,
                     Z );

    }



  private void SetupPointLight( double X,
                                double Y,
                                double Z )
    {
    PointLight PLight1 = new PointLight();
    PLight1.Color = System.Windows.Media.Colors.White;

    Point3D Location = new  Point3D( X, Y, Z );
    PLight1.Position = Location;
    PLight1.Range = 100000000.0;

    // Attenuation with distance D is like:
    // Attenuation = C + L*D + Q*D^2
    PLight1.ConstantAttenuation = 1;
    // PLight.LinearAttenuation = 1;
    // PLight.QuadraticAttenuation = 1;

    Main3DGroup.Children.Add( PLight1 );
    }



  private void SetupAmbientLight()
    {
    byte RGB = 0x0F;
    SetupAmbientLightColors( RGB, RGB, RGB );
    }



  private void SetupAmbientLightColors( byte Red,
                                        byte Green,
                                        byte Blue )
    {
    try
    {
    AmbientLight AmbiLight = new AmbientLight();
    // AmbiLight.Color = System.Windows.Media.Colors.Gray; // AliceBlue

    Color AmbiColor = new Color();
    AmbiColor.R = Red;
    AmbiColor.G = Green;
    AmbiColor.B = Blue;

    AmbiLight.Color = AmbiColor;

    Main3DGroup.Children.Add( AmbiLight );
    }
    catch( Exception Except )
      {
      ShowStatus( "Exception in ThreeDScene.SetupAmbientLight(): " + Except.Message );
      }
    }




  internal void RotateView()
    {
    double AddHours = NumbersEC.DegreesToRadians( 0.5 * (360.0d / 24.0d) );
    Earth.UTCTimeRadians = Earth.UTCTimeRadians + AddHours;
    Earth.MakeNewGeometryModel();
    ResetGeometryModels();
    }




  internal void DoTimeStep()
    {
    const double TimeDelta = 60 * 10; // seconds.
    for( int Count = 0; Count < SpaceObjectArrayLast; Count++ )
      {
      SpaceObject SpaceObj = SpaceObjectArray[Count];
      SpaceObj.SetNextPositionFromVelocity(
                                    TimeDelta );
      }

    Vector3.Vector AccelVector = new Vector3.Vector();
    for( int Count = 0; Count < SpaceObjectArrayLast; Count++ )
      {
      SpaceObject SpaceObj = SpaceObjectArray[Count];
      SpaceObj.Acceleration = Vector3.MakeZero();

      for( int Count2 = 0; Count2 < SpaceObjectArrayLast; Count2++ )
        {
        SpaceObject FarAwaySpaceObj = SpaceObjectArray[Count2];
        if( FarAwaySpaceObj.Mass < 1 )
          throw( new Exception( "The space object has no mass." ));

        AccelVector = FarAwaySpaceObj.Position;
        AccelVector = Vector3.Subtract( AccelVector, SpaceObj.Position );

        double Distance = Vector3.Norm( AccelVector );

        // Check if it's the same planet at zero
        // distance.
        if( Distance < 1.0 )
          continue;

        double Acceleration =
             (ModelConstants.GravitationConstant *
             FarAwaySpaceObj.Mass) /
             (Distance * Distance);

        AccelVector = Vector3.Normalize( AccelVector );
        AccelVector = Vector3.MultiplyWithScalar( AccelVector, Acceleration );
        SpaceObj.Acceleration = Vector3.Add( SpaceObj.Acceleration, AccelVector );
        }

      // Add the new Acceleration vector to the
      // velocity vector.
      SpaceObj.Velocity = Vector3.Add( SpaceObj.Velocity, SpaceObj.Acceleration );
      }

    ShowStatus( " " );
    ShowStatus( "Velocity.X: " + Earth.Velocity.X.ToString( "N2" ));
    ShowStatus( "Velocity.Y: " + Earth.Velocity.Y.ToString( "N2" ));
    ShowStatus( "Velocity.Z: " + Earth.Velocity.Z.ToString( "N2" ));
    ShowStatus( " " );


    Earth.AddTimeStepRotateAngle();

    // Earth.SetPlanetGravityAcceleration( this );

    // Move Earth only:
    // Earth.MakeNewGeometryModel();
    // ResetGeometryModels();

    // Move all of the planets:
    MakeNewGeometryModels();
    }



  internal Vector3.Vector GetEarthScaledPosition()
    {
    Vector3.Vector ScaledPos;

    ScaledPos.X = Earth.Position.X * ModelConstants.ThreeDSizeScale;
    ScaledPos.Y = Earth.Position.Y * ModelConstants.ThreeDSizeScale;
    ScaledPos.Z = Earth.Position.Z * ModelConstants.ThreeDSizeScale;

    return ScaledPos;
    }



  internal void SetEarthPositionToZero()
    {
    Earth.Position.X = 0;
    Earth.Position.Y = 0;
    Earth.Position.Z = 0;

    // Make a new Earth geometry model before
    // calling this:
    // ResetGeometryModels();

    MakeNewGeometryModels();
    }




  }
}

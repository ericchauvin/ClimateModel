// Copyright Eric Chauvin 2018 - 2019.



using System;
using System.Text;



namespace ClimateModel
{
  class ReferenceVertex
  {
  private MainForm MForm;
  private int ArraySize = 0;
  private double RadiusMinor = 0;
  private double RadiusMajor = 0;
  private double ApproximateLatitude = 0;
  private double GeodeticLatitude = 0;
  private double LatRadians = 0;
  private double CosLatRadians = 0;
  private double SinLatRadians = 0;
  private double CosLatRadiansPlusDelta = 0;
  private double SinLatRadiansPlusDelta = 0;
  private QuaternionEC.QuaternionRec RotationQ;
  private QuaternionEC.QuaternionRec InverseRotationQ;

  private Vector3.Vector[] PositionArray;
  private Vector3.Vector[] SurfaceNormalArray;

  private const double LatitudeRadiansDelta =
                       ModelConstants.TenToMinus6 *
                       ModelConstants.TenToMinus1;



  private ReferenceVertex()
    {
    }



  internal ReferenceVertex( MainForm UseForm,
                                 int UseArraySize )
    {
    MForm = UseForm;

    ArraySize = UseArraySize;

    try
    {
    // Some of these don't need to be allocated.
    // Like SurfaceNormalArray is only used on the
    // surface at index zero.
    PositionArray = new Vector3.Vector[ArraySize];
    SurfaceNormalArray = new Vector3.Vector[ArraySize];

    SetupEarthTiltRotations();
    }
    catch( Exception Except )
      {
      ShowStatus( "Exception in ReferenceVertexArray constructor: " + Except.Message );
      }
    }



  private void ShowStatus( string ToShow )
    {
    if( MForm == null )
      return;

    MForm.ShowStatus( ToShow );
    }


  internal int GetArraySize()
    {
    return ArraySize;
    }




  private void SetupEarthTiltRotations()
    {
    // RotationQ = new QuaternionEC.QuaternionRec();
    // InverseRotationQ = new QuaternionEC.QuaternionRec();

    double Angle = ModelConstants.EarthTiltAngleDegrees;
    Angle = NumbersEC.DegreesToRadians( Angle );

    QuaternionEC.QuaternionRec Axis;
    Axis.X = 1; // Rotate around the X axis.
    Axis.Y = 0;
    Axis.Z = 0;
    Axis.W = 0;

    RotationQ = QuaternionEC.SetAsRotation( Axis,
                                            Angle );

    InverseRotationQ = QuaternionEC.Inverse( RotationQ );
    }



  internal void DoAllEarthTiltRotations()
    {
    DoEarthTiltRotations( ref PositionArray );
    DoEarthTiltRotations( ref SurfaceNormalArray );
    }



  internal void DoEarthTiltRotations(
            ref Vector3.Vector[] InArray )
    {
    int Last = InArray.Length;
    for( int Count = 0; Count < Last; Count++ )
      {
      Vector3.Vector Vec = InArray[Count];

      Vector3.Vector ResultPoint = QuaternionEC.RotateVector3(
                     RotationQ,
                     InverseRotationQ,
                     Vec );

      InArray[Count] = ResultPoint;
      }
    }



  private void MakePoleRow( double ApproxLatitude )
    {
    Vector3.Vector Position = new Vector3.Vector();
    Vector3.Vector Normal = new Vector3.Vector();

    Position.X = 0;
    Position.Y = 0;
    Normal.X = 0;
    Normal.Y = 0;

    if( ApproxLatitude > 0 )
      {
      Position.Z = RadiusMinor;
      Normal.Z = 1;
      }
    else
      {
      Position.Z = -RadiusMinor;
      Normal.Z = -1;
      }

    PositionArray[0] = Position;
    SurfaceNormalArray[0] = Normal;
    }



  internal void MakeVertexRow(
                       double ApproxLatitude,
                       double LongitudeHoursRadians )
    {
    try
    {
    if( ArraySize < 2 )
      {
      MakePoleRow( ApproxLatitude );
      return;
      }

    double LonStart = -180.0;

    // There is a beginning vertex at -180 longitude
    // and there is an ending vertex at 180
    // longitude, which is the same place, but they
    // are associated with different texture
    // coordinates.  One at the left end of the
    // texture and one at the right end.
    // So this is minus 1:
    double LonDelta = 360.0d / (double)(ArraySize - 1);

    for( int Count = 0; Count < ArraySize; Count++ )
      {
      Vector3.Vector Position = new Vector3.Vector();
      Vector3.Vector Normal = new Vector3.Vector();

      double Longitude = LonStart + (LonDelta * Count);
      double LonRadians = NumbersEC.DegreesToRadians(
                                          Longitude );

      // Higher hours make the sun set in the west.
      LonRadians += LongitudeHoursRadians;

      double CosLonRadians = Math.Cos( LonRadians );
      double SinLonRadians = Math.Sin( LonRadians );

      // Along the equatorial axis:
      Position.X = RadiusMajor * (CosLatRadians * CosLonRadians );
      Position.Y = RadiusMajor * (CosLatRadians * SinLonRadians );

      // Along the polar axis:
      Position.Z = RadiusMinor * SinLatRadians;

      PositionArray[Count] = Position;

      // if( It needs to be calculated... )
        SetSurfaceNormal( Position,
                   ref Normal,
                   CosLonRadians,
                   SinLonRadians,
                   ApproxLatitude );

        SurfaceNormalArray[Count] = Normal;
      //
      }
    }
    catch( Exception Except )
      {
      ShowStatus( "Exception in ReferenceVertex.MakeVertexRow(): " + Except.Message );
      }
    }




  private void SetSurfaceNormal(
                      Vector3.Vector Position,
                      ref Vector3.Vector Normal,
                      double CosLonRadians,
                      double SinLonRadians,
                      double ApproxLatitude )
    {
    if( ApproxLatitude < -89.999 )
      throw( new Exception( "ApproxLatitude < -89.999 in SetSurfaceNormal()." ));

    if( ApproxLatitude > 89.999 )
      throw( new Exception( "ApproxLatitude > 89.999 in SetSurfaceNormal()." ));

    // If it's at the equator.
    if( (ApproxLatitude < 0.00001) &&
        (ApproxLatitude > -0.00001))
      {
      // This avoids calculating a perpendicular
      // with two vectors _very_ close to each other.
      // It avoids normalizing a very small vector.
      Normal.X = Position.X;
      Normal.Y = Position.Y;
      Normal.Z = 0;
      Normal = Vector3.Normalize( Normal );
      return;
      }

    Vector3.Vector PositionAtDelta;
    PositionAtDelta.X = RadiusMajor * (CosLatRadiansPlusDelta * CosLonRadians );
    PositionAtDelta.Y = RadiusMajor * (CosLatRadiansPlusDelta * SinLonRadians );
    PositionAtDelta.Z = RadiusMinor * SinLatRadiansPlusDelta;

    Vector3.Vector Flat;
    Flat = PositionAtDelta;
    Flat = Vector3.Subtract( Flat, Position );

    if( Position.Z > PositionAtDelta.Z )
      throw( new Exception( "Position.Z > PositionAtDelta.Z." ));

    if( Flat.Z < 0 )
      throw( new Exception( "Flat.Z < 0." ));

    Flat = Vector3.Normalize( Flat );

    // Straight up through the north pole.
    Vector3.Vector StraightUp;
    StraightUp.X = 0;
    StraightUp.Y = 0;
    StraightUp.Z = 1;

    // The dot product of two normalized vectors.
    double Dot = Vector3.DotProduct( StraightUp, Flat );

    if( Dot < 0 )
      throw( new Exception( "Dot < 0." ));

    // Make a vector perpendicular to FlatVector and
    // toward StraightUp.
    Vector3.Vector PerpenVector =
              Vector3.MakePerpendicular( Flat,
                                         StraightUp );

    if( Position.Z < 0 )
      PerpenVector = Vector3.Negate( PerpenVector );

    Normal = PerpenVector;
    }




  internal double GetGeodeticLatitude()
    {
    return GeodeticLatitude;
    }




  internal void SetupLatitude(
                     double UseApproximateLatitude,
                     double UseRadiusMinor,
                     double UseRadiusMajor )
    {
    ApproximateLatitude = UseApproximateLatitude;
    RadiusMinor = UseRadiusMinor;
    RadiusMajor = UseRadiusMajor;

    LatRadians = NumbersEC.DegreesToRadians( ApproximateLatitude );
    CosLatRadians = Math.Cos( LatRadians );
    SinLatRadians = Math.Sin( LatRadians );
    CosLatRadiansPlusDelta = Math.Cos( LatRadians + LatitudeRadiansDelta );
    SinLatRadiansPlusDelta = Math.Sin( LatRadians + LatitudeRadiansDelta );

    // If it's at the north or south pole.
    if( (ApproximateLatitude >  89.9999) ||
        (ApproximateLatitude < -89.9999))
      {
      GeodeticLatitude = ApproximateLatitude;
      return;
      }

    // If it's pretty much at the equator at
    // zero latitude.
    if( (ApproximateLatitude >  -0.0001) &&
        (ApproximateLatitude < 0.0001))
      {
      GeodeticLatitude = ApproximateLatitude;
      return;
      }

    // Straight up through the north pole.
    Vector3.Vector StraightUp = new Vector3.Vector();
    StraightUp.X = 0;
    StraightUp.Y = 0;
    StraightUp.Z = 1;

    double CosLonRadians = 1;
    double SinLonRadians = 0;

    Vector3.Vector Position = new Vector3.Vector();
    Position.X = RadiusMajor * (CosLatRadians * CosLonRadians );
    Position.Y = RadiusMajor * (CosLatRadians * SinLonRadians );
    Position.Z = RadiusMinor * SinLatRadians;

    Vector3.Vector PositionAtDelta = new Vector3.Vector();
    PositionAtDelta.X = RadiusMajor * (CosLatRadiansPlusDelta * CosLonRadians );
    PositionAtDelta.Y = RadiusMajor * (CosLatRadiansPlusDelta * SinLonRadians );
    PositionAtDelta.Z = RadiusMinor * SinLatRadiansPlusDelta;

    Vector3.Vector Flat = PositionAtDelta;
    Flat = Vector3.Subtract( Flat, Position );

    if( Position.Z > PositionAtDelta.Z )
      throw( new Exception( "Position.Z > PositionAtDelta.Z." ));

    if( Flat.Z < 0 )
      throw( new Exception( "Flat.Z < 0." ));

    Flat = Vector3.Normalize( Flat );

    // The dot product of two normalized vectors.
    double Dot = Vector3.DotProduct(
                              StraightUp,
                              Flat );

    if( Dot < 0 )
      throw( new Exception( "Dot < 0." ));

    double StraightUpAngle = Math.Acos( Dot );
    double AngleToEquator =
               (Math.PI / 2.0) - StraightUpAngle;

    double Degrees = NumbersEC.RadiansToDegrees( StraightUpAngle );

    if( ApproximateLatitude >= 0 )
      GeodeticLatitude = Degrees;
    else
      GeodeticLatitude = -Degrees;

    }



  internal Vector3.Vector GetPosition( int Where )
    {
    // if (Where >= ArraySize )

    return PositionArray[Where];
    }



  internal Vector3.Vector GetSurfaceNormal( int Where )
    {
    // if (Where >= ArraySize )

    return SurfaceNormalArray[Where];
    }



  }
}

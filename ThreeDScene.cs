// Copyright Eric Chauvin 2018 - 2019.


using System;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Media.Imaging;

// For testing.
// using System.Windows.Forms;


namespace ClimateModel
{
  class ThreeDScene
  {
  private MainForm MForm;
  private PerspectiveCamera PCamera = new PerspectiveCamera();
  private Model3DGroup Main3DGroup = new Model3DGroup();
  private ModelVisual3D MainModelVisual3D = new ModelVisual3D();
  internal SolarSystem SolarS;



  private ThreeDScene()
    {
    }



  internal ThreeDScene( MainForm UseForm )
    {
    try
    {
    MForm = UseForm;

    // RefFrame = new ReferenceFrame( MForm, Main3DGroup );
    SolarS = new SolarSystem( MForm, Main3DGroup );

    SetupCamera();
    MainModelVisual3D.Content = Main3DGroup;

    MoveToEarthView();

    }
    catch( Exception Except )
      {
      MForm.ShowStatus( "Exception in ThreeScene constructor: " + Except.Message );
      return;
      }
    }



  internal PerspectiveCamera GetCamera()
    {
    return PCamera;
    }



  internal ModelVisual3D GetMainModelVisual3D()
    {
    return MainModelVisual3D;
    }



  internal void SetCameraTo( double X,
                             double Y,
                             double Z,
                             double LookX,
                             double LookY,
                             double LookZ,
                             double UpX,
                             double UpY,
                             double UpZ )
    {
    PCamera.Position = new Point3D( X, Y, Z );
    PCamera.LookDirection = new Vector3D( LookX, LookY, LookZ );
    PCamera.UpDirection = new Vector3D( UpX, UpY, UpZ );
    }



  internal void SetCameraToOriginal()
    {
    PCamera.Position = new Point3D( 0, 15, 0 );
    PCamera.LookDirection = new Vector3D( 0, -1, 0 );
    // Up is toward the North Pole.
    PCamera.UpDirection = new Vector3D( 0, 0, 1 );
    }



  private void SetupCamera()
    {
    // Positive Z values go toward the viewer.
    PCamera.FieldOfView = 60;
    // Clipping planes:
    // Too much of a range for clipping will cause
    // problems with the Depth buffer.
    // Saturn is at about 1,498,032 thousand
    // kilometers from Earth.  And 1,505,795 from
    // the sun.  Twice the radius of Saturn's
    // orbit: 1,600,000 * 2.
    PCamera.FarPlaneDistance = 1600000 * 2;
    PCamera.NearPlaneDistance = 0.5;

    SetCameraToOriginal();
    }



  internal void MoveForwardBack( double HowFar )
    {
    Vector3D LookAt = PCamera.LookDirection;
    Point3D Position = PCamera.Position;
    Vector3D MoveBy = new Vector3D();
    MoveBy = Vector3D.Multiply( HowFar, LookAt );
    Point3D MoveTo = new Point3D();
    MoveTo = Point3D.Add( Position, MoveBy );
    PCamera.Position = MoveTo;
    }



  internal void MoveLeftRight( double Angle )
    {
    Vector3D LookDirection = PCamera.LookDirection;
    Vector3D UpDirection = PCamera.UpDirection;

    QuaternionEC.QuaternionRec Axis;
    Axis.X = UpDirection.X;
    Axis.Y = UpDirection.Y;
    Axis.Z = UpDirection.Z;
    Axis.W = 0;

    QuaternionEC.QuaternionRec StartPoint;
    StartPoint.X = LookDirection.X;
    StartPoint.Y = LookDirection.Y;
    StartPoint.Z = LookDirection.Z;
    StartPoint.W = 0;

    QuaternionEC.QuaternionRec RotationQ =
                   QuaternionEC.SetAsRotation( Axis,
                                               Angle );

    QuaternionEC.QuaternionRec InverseRotationQ =
                    QuaternionEC.Inverse( RotationQ );

    QuaternionEC.QuaternionRec ResultPoint =
                  QuaternionEC.Rotate( RotationQ,
                                       InverseRotationQ,
                                       StartPoint );

    LookDirection.X = ResultPoint.X;
    LookDirection.Y = ResultPoint.Y;
    LookDirection.Z = ResultPoint.Z;
    PCamera.LookDirection = LookDirection;
    }



  // For Yaw, Pitch and Roll, this is Roll.
  internal void RotateLeftRight( double Angle )
    {
    Vector3D LookDirection = PCamera.LookDirection;
    Vector3D UpDirection = PCamera.UpDirection;

    QuaternionEC.QuaternionRec Axis;
    Axis.X = LookDirection.X;
    Axis.Y = LookDirection.Y;
    Axis.Z = LookDirection.Z;
    Axis.W = 0;

    Vector3.Vector Up;
    Up.X = UpDirection.X;
    Up.Y = UpDirection.Y;
    Up.Z = UpDirection.Z;

    QuaternionEC.QuaternionRec RotationQ =
                QuaternionEC.SetAsRotation( Axis,
                                            Angle );

    QuaternionEC.QuaternionRec InverseRotationQ =
                   QuaternionEC.Inverse( RotationQ );

    Vector3.Vector ResultPoint =
          QuaternionEC.RotateVector3( RotationQ,
                                      InverseRotationQ,
                                      Up );

    UpDirection.X = ResultPoint.X;
    UpDirection.Y = ResultPoint.Y;
    UpDirection.Z = ResultPoint.Z;
    PCamera.UpDirection = UpDirection;
    }



  internal void MoveUpDown( double Angle )
    {
    Vector3D LookDirection = PCamera.LookDirection;
    Vector3D UpDirection = PCamera.UpDirection;

    QuaternionEC.QuaternionRec Look;
    Look.X = LookDirection.X;
    Look.Y = LookDirection.Y;
    Look.Z = LookDirection.Z;
    Look.W = 0;

    QuaternionEC.QuaternionRec Up;
    Up.X = UpDirection.X;
    Up.Y = UpDirection.Y;
    Up.Z = UpDirection.Z;
    Up.W = 0;

    // X Cross Y = Z.  The Right-hand rule.

    QuaternionEC.QuaternionRec Cross =
                QuaternionEC.CrossProduct( Look, Up );

    QuaternionEC.QuaternionRec RotationQ =
           QuaternionEC.SetAsRotation( Cross, Angle );

    QuaternionEC.QuaternionRec InverseRotationQ =
                    QuaternionEC.Inverse( RotationQ );

    /////////////////
    // Rotate Up around Cross.
    QuaternionEC.QuaternionRec StartPoint;
    StartPoint.X = Up.X;
    StartPoint.Y = Up.Y;
    StartPoint.Z = Up.Z;
    StartPoint.W = 0;

    QuaternionEC.QuaternionRec ResultPoint =
              QuaternionEC.Rotate( RotationQ,
                                   InverseRotationQ,
                                   StartPoint );

    UpDirection.X = ResultPoint.X;
    UpDirection.Y = ResultPoint.Y;
    UpDirection.Z = ResultPoint.Z;
    PCamera.UpDirection = UpDirection;

    /////////////////
    // Rotate Look around Cross.
    StartPoint.X = Look.X;
    StartPoint.Y = Look.Y;
    StartPoint.Z = Look.Z;
    StartPoint.W = 0;

    ResultPoint = QuaternionEC.Rotate( RotationQ,
                                       InverseRotationQ,
                                       StartPoint );

    LookDirection.X = ResultPoint.X;
    LookDirection.Y = ResultPoint.Y;
    LookDirection.Z = ResultPoint.Z;
    PCamera.LookDirection = LookDirection;
    }



  internal void ShiftLeftRight( double HowFar )
    {
    Vector3D LookDirection = PCamera.LookDirection;
    Vector3D UpDirection = PCamera.UpDirection;

    QuaternionEC.QuaternionRec Look;
    Look.X = LookDirection.X;
    Look.Y = LookDirection.Y;
    Look.Z = LookDirection.Z;
    Look.W = 0;

    QuaternionEC.QuaternionRec Up;
    Up.X = UpDirection.X;
    Up.Y = UpDirection.Y;
    Up.Z = UpDirection.Z;
    Up.W = 0;

    QuaternionEC.QuaternionRec Cross =
                QuaternionEC.CrossProduct( Look, Up );

    Vector3D CrossVect = new Vector3D();
    CrossVect.X = Cross.X;
    CrossVect.Y = Cross.Y;
    CrossVect.Z = Cross.Z;

    Point3D Position = PCamera.Position;

    Vector3D MoveBy = Vector3D.Multiply( HowFar, CrossVect );
    Point3D MoveTo = Point3D.Add( Position, MoveBy );
    PCamera.Position = MoveTo;
    }



  internal void ShiftUpDown( double HowFar )
    {
    Vector3D UpDirection = PCamera.UpDirection;

    Point3D Position = PCamera.Position;
    Vector3D MoveBy = new Vector3D();
    Point3D MoveTo = new Point3D();

    MoveBy = Vector3D.Multiply( HowFar, UpDirection );
    MoveTo = Point3D.Add( Position, MoveBy );
    PCamera.Position = MoveTo;
    }



/*
  private void MakeSurface()
    {
    try
    {
    MeshGeometry3D TriSurface = new MeshGeometry3D();

    // Texture Coordinates:
    // https://msdn.microsoft.com/en-us/library/system.windows.media.media3d.meshgeometry3d.texturecoordinates(v=vs.110).aspx

    // For texture coordinates in 2D "Bitmap image
    // space", so to speak, Y starts at the top and
    // goes downward.
    Point TopLeft = new Point( 0, 0 );
    Point BottomRight = new Point( 1, 1 );
    Point TopRight = new Point( 1, 0 );
    Point BottomLeft = new Point( 0, 1 );

    // In this 3D space Y goes upward.
    Point3D TopLeft3D = new Point3D( 0, 1, 0 );
    Point3D BottomRight3D = new Point3D( 1, 0, 0 );
    Point3D TopRight3D = new Point3D( 1, 1, 0 );
    Point3D BottomLeft3D = new Point3D( 0, 0, 0 );

    TriSurface.Positions.Add( BottomLeft3D );
    TriSurface.Positions.Add( BottomRight3D );
    TriSurface.Positions.Add( TopLeft3D );
    TriSurface.Positions.Add( TopRight3D );

    TriSurface.TextureCoordinates.Add( BottomLeft );
    TriSurface.TextureCoordinates.Add( BottomRight );
    TriSurface.TextureCoordinates.Add( TopLeft );
    TriSurface.TextureCoordinates.Add( TopRight );

    // Counterclockwise winding goes toward the viewer.
    TriSurface.TriangleIndices.Add( 0 );
    TriSurface.TriangleIndices.Add( 1 );
    TriSurface.TriangleIndices.Add( 2 );

    TriSurface.TriangleIndices.Add( 1 );
    TriSurface.TriangleIndices.Add( 3 );
    TriSurface.TriangleIndices.Add( 2 );

    // Positive Z values go toward the viewer.
    // So the normal is toward the viewer.
    Vector3D BasicNormal = new Vector3D( 0, 0, 1 );
    TriSurface.Normals.Add( BasicNormal );
    TriSurface.Normals.Add( BasicNormal );
    TriSurface.Normals.Add( BasicNormal );
    TriSurface.Normals.Add( BasicNormal );

    DiffuseMaterial SolidMat = new DiffuseMaterial();
    // SolidMat.Brush = Brushes.Blue;
    SolidMat.Brush = SetTextureImageBrush();

    GeometryModel3D GeoMod = new GeometryModel3D();
    GeoMod.Geometry = TriSurface;
    GeoMod.Material = SolidMat;

    Main3DGroup.Children.Add( GeoMod );

    }
    catch( Exception Except )
      {
      MForm.ShowStatus( "Exception in ThreeDScene.MakeSurface(): " + Except.Message );
      return;
      }
    }
*/


/*
  private ImageBrush SetTextureImageBrush()
    {
    // Imaging Overview:
    // https://docs.microsoft.com/en-us/dotnet/framework/wpf/graphics-multimedia/imaging-overview

    // Imaging Namespace:
    // https://docs.microsoft.com/en-us/dotnet/api/system.windows.media.imaging?view=netframework-4.7.1

    // ImageDrawing:
    // https://docs.microsoft.com/en-us/dotnet/api/system.windows.media.imagedrawing?view=netframework-4.7.1

    BitmapImage BMapImage = new BitmapImage();

    // Things have to be in this Begin-end block.
    BMapImage.BeginInit();

    BMapImage.UriSource = new Uri( "C:\\Eric\\ClimateModel\\bin\\Release\\earth.jpg" );
    // BMapImage.UriSource = new Uri( "C:\\Eric\\ClimateModel\\bin\\Release\\TestImage.jpg" );

    // BMapImage.DecodePixelWidth = 200;

    BMapImage.EndInit();

    // ImageBrush:
    // https://msdn.microsoft.com/en-us/library/system.windows.media.imagebrush(v=vs.110).aspx
    ImageBrush ImgBrush = new ImageBrush();
    ImgBrush.ImageSource = BMapImage;
    return ImgBrush;
    }
*/


  internal void RotateView()
    {
    SolarS.RotateView();
    }



  internal void DoTimeStep()
    {
    SolarS.DoTimeStep();
    }



  internal void MoveToEarthView()
    {
    Vector3.Vector Pos = SolarS.GetEarthScaledPosition();

    SetCameraTo( Pos.X,
                 Pos.Y,
                 Pos.Z,
                     0,  // LookAt vector.
                     -1,
                     0,
                     0,
                     0,
                     1 );

    MoveForwardBack( -30.0 );
    }



  internal void SetEarthPositionToZero()
    {
    SolarS.SetEarthPositionToZero();
    }



  }
}

// Copyright Eric Chauvin 2018 - 2019.


// Constants in physics:
// https://en.wikipedia.org/wiki/Physical_constant#Table_of_physical_constants



using System;
using System.Text;


namespace ClimateModel
{

  static class ModelConstants
  {
  // Double precision format:
  // 53-bit "significand precision".
  // https://en.wikipedia.org/wiki/Double-precision_floating-point_format

  // Quad precision:
  // 113-bit significand precision.
  // https://en.wikipedia.org/wiki/Quadruple-precision_floating-point_format


  /////////////////////////////////////////////////
  // Float128
  // https://gcc.gnu.org/onlinedocs/libquadmath.pdf
  /////////////////////////////////////////////////


  // binary256:
  // 237-bit significand precision.

  internal const double TenTo1 = 10.0d;
  internal const double TenTo3 = 10.0d * 10.0d * 10.0d;
  internal const double TenTo6 = TenTo3 * TenTo3;
  internal const double TenTo9 = TenTo6 * TenTo3;
  internal const double TenTo10 = TenTo9 * TenTo1;
  internal const double TenTo20 = TenTo10 * TenTo10;

  internal const double TenToMinus1 = 1.0d / 10.0d;
  internal const double TenToMinus3 = 1.0d / TenTo3;
  internal const double TenToMinus6 = 1.0d / TenTo6;
  internal const double TenToMinus9 = 1.0d / TenTo9;
  internal const double TenToMinus10 = 1.0d / TenTo10;
  internal const double TenToMinus20 = 1.0d / TenTo20;


  internal const double ThreeDSizeScale = TenToMinus6;

  internal const double SpeedOfLight = 299792458.0d;
  internal const double SpeedOfLightSqr =
             SpeedOfLight * SpeedOfLight;

  // The speed of light in a vacuum is
  // 299,792,458 meters per second.
  // One meter is actually _defined_ by how long it
  // takes light to travel in 1 / 299,792,458 seconds.
  // It takes about 8.3 minutes for light to travel
  // from the sun to the Earth.


  // Meters^3  Kilograms^-1  Seconds^-2
  // https://en.wikipedia.org/wiki/Gravitational_constant
// Check this value:
  internal const double GravitationConstant =
               6.6740831d *
               (TenToMinus10 * TenToMinus1);


  internal const double EarthRadiusMajor = 6378137d; // Equator
  internal const double EarthRadiusMinor = 6356752d; // poles
  // Test squished ellipsoid:
  // internal const double EarthRadiusMinor = 3000000d;

  // Mass in kilograms.
  internal const double MassOfSun = 1.98855d *
                                      TenTo20 *
                                      TenTo10;

  internal const double MassOfEarth = 5.97237d *
                                      TenTo20 *
                                      TenTo3 *
                                      TenTo1;


  internal const double MassOfMoon = 7.342d *
                                      TenTo20 *
                                      TenTo1 *
                                      TenTo1;

  internal const double MassOfEarthPlusMoon =
             MassOfEarth + MassOfMoon;

  internal const double MassOfMercury = 3.3011d *
                                      TenTo20 *
                                      TenTo3;

  internal const double MassOfVenus = 4.8675d *
                                      TenTo20 *
                                      TenTo3 *
                                      TenTo1;

  internal const double MassOfMars = 6.4171d *
                                      TenTo20 *
                                      TenTo3;

  internal const double MassOfJupiter = 1.8982d *
                                      TenTo20 *
                                      TenTo6 *
                                      TenTo1;

  internal const double MassOfSaturn = 5.6834d *
                                      TenTo20 *
                                      TenTo6;




  // Need sidereal?
  internal const double EarthRotationAnglePerSecond =
              (2.0d  * Math.PI) / (24 * 60 * 60);


  // See the QuaternionEC.cs and SolarSystem.cs files for notes
  // about rotations with quaternions and why this is a negative
  // angle.
  internal const double EarthTiltAngleDegrees = -23.439;

  // From JPL Earth file: Obliquity to orbit, deg  = 23.4392911


  }
}

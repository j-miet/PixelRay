namespace PixelRay.Core.Mathematics;

/// <summary>
/// 4x4 matrix struct for creating linear transformation matrices
/// </summary>
public readonly struct Matrix4x4(
    double m00, double m01, double m02, double m03,
    double m10, double m11, double m12, double m13,
    double m20, double m21, double m22, double m23,
    double m30, double m31, double m32, double m33
)
{
    public readonly double M00 = m00, M01 = m01, M02 = m02, M03 = m03;
    public readonly double M10 = m10, M11 = m11, M12 = m12, M13 = m13;
    public readonly double M20 = m20, M21 = m21, M22 = m22, M23 = m23;
    public readonly double M30 = m30, M31 = m31, M32 = m32, M33 = m33;

    /// <summary>
    /// Apply matrix transformation to point
    /// </summary>
    public Vec3 TransformPoint(Vec3 p)
    {
        // vectors require use of translation vector Mx3
        double x = M00 * p.X + M01 * p.Y + M02 * p.Z + M03;
        double y = M10 * p.X + M11 * p.Y + M12 * p.Z + M13;
        double z = M20 * p.X + M21 * p.Y + M22 * p.Z + M23;
        double w = M30 * p.X + M31 * p.Y + M32 * p.Z + M33;

        if (w != 0.0) // if affine component not 0, normalize all coordinates by it's value
        {
            x /= w;
            y /= w;
            z /= w;
        }

        return new(x, y, z);
    }

    /// <summary>
    /// Apply matrix transformation to vector
    /// </summary>
    public Vec3 TransformVector(Vec3 v)
    {
        return new Vec3(
            M00 * v.X + M01 * v.Y + M02 * v.Z,
            M10 * v.X + M11 * v.Y + M12 * v.Z,
            M20 * v.X + M21 * v.Y + M22 * v.Z
        );
    }

    /// <summary>
    /// Apply matrix transformation to ray
    /// </summary>
    public Ray TransformRay(Ray r)
    {
        return new Ray(
            TransformPoint(r.Origin),
            TransformVector(r.Direction)
        );
    }

    /// <summary>
    /// Matrix m inverse for affine transforms. These include translation, rotation, scaling and any combination
    /// of these. 
    /// Upper-left 3x3 rotation matrix must be orthonormal. If not, use general Inverse method instead which will be
    /// slower.
    /// </summary>
    public Matrix4x4 InverseAffine()
    {
        // affine transform matric have simple square form
        // [ R     T ]
        // [ 0 0 0 1 ]
        // where R is 3x3 rotation matrix (orthonormal) and T the translation vector [tx, ty, tz]^T
        // Inverse of this is then simply
        // [ R^-1 -(R^1)*T]
        // [ 0   0   0  1 ]
        // Furthermore: because R is a rotation matrix thus orthonormal, its inverse is equal to transpose.
        Matrix4x4 rInv = new( // R^-1 = R^T
            M00, M10, M20, 0,
            M01, M11, M21, 0,
            M02, M12, M22, 0,
            0, 0, 0, 1
        );

        Vec3 t = new(M03, M13, M23);
        Vec3 tInverse = -rInv.TransformVector(t); // -(R^1)*T

        return new Matrix4x4(
            rInv.M00, rInv.M01, rInv.M02, tInverse.X,
            rInv.M10, rInv.M11, rInv.M12, tInverse.Y,
            rInv.M20, rInv.M21, rInv.M22, tInverse.Z,
            0, 0, 0, 1
        );
    }

    /// <summary>
    /// Matrix inverse. For affine transformations always use the faster InverseAffine method.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public Matrix4x4 Inverse()
    {
        double[] m =
        [
            M00,M01,M02,M03,
            M10,M11,M12,M13,
            M20,M21,M22,M23,
            M30,M31,M32,M33
        ];

        double[] dets = new double[16]; // determinants of all inner 2x2 matrices

        dets[0] = M11 * M22 * M33 - M11 * M23 * M32 - M21 * M12 * M33
               + M21 * M13 * M32 + M31 * M12 * M23 - M31 * M13 * M22;

        dets[4] = -M10 * M22 * M33 + M10 * M23 * M32 + M20 * M12 * M33
               - M20 * M13 * M32 - M30 * M12 * M23 + M30 * M13 * M22;

        dets[8] = M10 * M21 * M33 - M10 * M23 * M31 - M20 * M11 * M33
               + M20 * M13 * M31 + M30 * M11 * M23 - M30 * M13 * M21;

        dets[12] = -M10 * M21 * M32 + M10 * M22 * M31 + M20 * M11 * M32
                - M20 * M12 * M31 - M30 * M11 * M22 + M30 * M12 * M21;

        dets[1] = -M01 * M22 * M33 + M01 * M23 * M32 + M21 * M02 * M33
               - M21 * M03 * M32 - M31 * M02 * M23 + M31 * M03 * M22;

        dets[5] = M00 * M22 * M33 - M00 * M23 * M32 - M20 * M02 * M33
               + M20 * M03 * M32 + M30 * M02 * M23 - M30 * M03 * M22;

        dets[9] = -M00 * M21 * M33 + M00 * M23 * M31 + M20 * M01 * M33
               - M20 * M03 * M31 - M30 * M01 * M23 + M30 * M03 * M21;

        dets[13] = M00 * M21 * M32 - M00 * M22 * M31 - M20 * M01 * M32
                + M20 * M02 * M31 + M30 * M01 * M22 - M30 * M02 * M21;

        dets[2] = M01 * M12 * M33 - M01 * M13 * M32 - M11 * M02 * M33
               + M11 * M03 * M32 + M31 * M02 * M13 - M31 * M03 * M12;

        dets[6] = -M00 * M12 * M33 + M00 * M13 * M32 + M10 * M02 * M33
               - M10 * M03 * M32 - M30 * M02 * M13 + M30 * M03 * M12;

        dets[10] = M00 * M11 * M33 - M00 * M13 * M31 - M10 * M01 * M33
                + M10 * M03 * M31 + M30 * M01 * M13 - M30 * M03 * M11;

        dets[14] = -M00 * M11 * M32 + M00 * M12 * M31 + M10 * M01 * M32
                - M10 * M02 * M31 - M30 * M01 * M12 + M30 * M02 * M11;

        dets[3] = -M01 * M12 * M23 + M01 * M13 * M22 + M11 * M02 * M23
               - M11 * M03 * M22 - M21 * M02 * M13 + M21 * M03 * M12;

        dets[7] = M00 * M12 * M23 - M00 * M13 * M22 - M10 * M02 * M23
               + M10 * M03 * M22 + M20 * M02 * M13 - M20 * M03 * M12;

        dets[11] = -M00 * M11 * M23 + M00 * M13 * M21 + M10 * M01 * M23
                - M10 * M03 * M21 - M20 * M01 * M13 + M20 * M03 * M11;

        dets[15] = M00 * M11 * M22 - M00 * M12 * M21 - M10 * M01 * M22
                + M10 * M02 * M21 + M20 * M01 * M12 - M20 * M02 * M11;

        double determinant = M00 * dets[0] + M01 * dets[4] + M02 * dets[8] + M03 * dets[12];

        if (Math.Abs(determinant) < 1e-12)
            throw new InvalidOperationException("Matrix not detsertible");

        double inverseDet = 1.0 / determinant;

        for (int i = 0; i < 16; i++)
            dets[i] *= inverseDet;

        return new Matrix4x4(
            dets[0], dets[1], dets[2], dets[3],
            dets[4], dets[5], dets[6], dets[7],
            dets[8], dets[9], dets[10], dets[11],
            dets[12], dets[13], dets[14], dets[15]
        );
    }

    /// <summary>
    /// Matrix m transposed
    /// </summary>
    public Matrix4x4 Transpose()
    {
        return new Matrix4x4(
            M00, M10, M20, M30,
            M01, M11, M21, M31,
            M02, M12, M22, M32,
            M03, M13, M23, M33
        );
    }

    /// <summary>
    /// Identity matrix
    /// </summary>
    public static Matrix4x4 Identity()
    {
        return new(
            1, 0, 0, 0,
            0, 1, 0, 0,
            0, 0, 1, 0,
            0, 0, 0, 1
        );
    }

    /// <summary>
    /// Translation/shifting matrix to direction v
    /// </summary>
    public static Matrix4x4 Translate(Vec3 v)
    {
        return new Matrix4x4(
            1, 0, 0, v.X,
            0, 1, 0, v.Y,
            0, 0, 1, v.Z,
            0, 0, 0, 1
        );
    }

    /// <summary>
    /// Translation/shifting matrix to direction v
    /// </summary>
    public static Matrix4x4 Shift(Vec3 v) => Translate(v); // alias for Translate

    /// <summary>
    /// Scaling matrix using vector v components as scalars
    /// </summary>
    public static Matrix4x4 Scale(Vec3 v)
    {
        return new Matrix4x4(
            v.X, 0, 0, 0,
            0, v.Y, 0, 0,
            0, 0, v.Z, 0,
            0, 0, 0, 1
        );
    }

    /// <summary>
    /// Uniform matrix scaling
    /// </summary>
    public static Matrix4x4 Scale(double t)
    {
        return new Matrix4x4(
            t, 0, 0, 0,
            0, t, 0, 0,
            0, 0, t, 0,
            0, 0, 0, 1
        );
    }

    /// <summary>
    /// Rotation matrix for given axis and angle (in radians)
    /// </summary>
    public static Matrix4x4 Rotate(Vec3 axis, double angle)
    {
        axis = axis.Unit();

        double x = axis.X, y = axis.Y, z = axis.Z;
        double cos = Math.Cos(angle);
        double sin = Math.Sin(angle);
        double a = 1 - cos;

        return new Matrix4x4(
            a * x * x + cos, a * x * y - sin * z, a * x * z + sin * y, 0,
            a * x * y + sin * z, a * y * y + cos, a * y * z - sin * x, 0,
            a * x * z - sin * y, a * y * z + sin * x, a * z * z + cos, 0,
            0, 0, 0, 1
        );
    }

    /// <summary>
    /// Matrix multiplication M1*M2
    /// </summary>
    public static Matrix4x4 operator *(Matrix4x4 m1, Matrix4x4 m2)
    {
        return new Matrix4x4(
            m1.M00 * m1.M00 + m1.M01 * m2.M10 + m1.M02 * m2.M20 + m1.M03 * m2.M30,
            m1.M00 * m2.M01 + m1.M01 * m2.M11 + m1.M02 * m2.M21 + m1.M03 * m2.M31,
            m1.M00 * m2.M02 + m1.M01 * m2.M12 + m1.M02 * m2.M22 + m1.M03 * m2.M32,
            m1.M00 * m2.M03 + m1.M01 * m2.M13 + m1.M02 * m2.M23 + m1.M03 * m2.M33,

            m1.M10 * m2.M00 + m1.M11 * m2.M10 + m1.M12 * m2.M20 + m1.M13 * m2.M30,
            m1.M10 * m2.M01 + m1.M11 * m2.M11 + m1.M12 * m2.M21 + m1.M13 * m2.M31,
            m1.M10 * m2.M02 + m1.M11 * m2.M12 + m1.M12 * m2.M22 + m1.M13 * m2.M32,
            m1.M10 * m2.M03 + m1.M11 * m2.M13 + m1.M12 * m2.M23 + m1.M13 * m2.M33,

            m1.M20 * m2.M00 + m1.M21 * m2.M10 + m1.M22 * m2.M20 + m1.M23 * m2.M30,
            m1.M20 * m2.M01 + m1.M21 * m2.M11 + m1.M22 * m2.M21 + m1.M23 * m2.M31,
            m1.M20 * m2.M02 + m1.M21 * m2.M12 + m1.M22 * m2.M22 + m1.M23 * m2.M32,
            m1.M20 * m2.M03 + m1.M21 * m2.M13 + m1.M22 * m2.M23 + m1.M23 * m2.M33,

            m1.M30 * m2.M00 + m1.M31 * m2.M10 + m1.M32 * m2.M20 + m1.M33 * m2.M30,
            m1.M30 * m2.M01 + m1.M31 * m2.M11 + m1.M32 * m2.M21 + m1.M33 * m2.M31,
            m1.M30 * m2.M02 + m1.M31 * m2.M12 + m1.M32 * m2.M22 + m1.M33 * m2.M32,
            m1.M30 * m2.M03 + m1.M31 * m2.M13 + m1.M32 * m2.M23 + m1.M33 * m2.M33
        );
    }
}
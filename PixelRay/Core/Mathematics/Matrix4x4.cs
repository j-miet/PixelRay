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
        Vec3 O = TransformPoint(r.Origin);
        Vec3 D = TransformVector(r.Direction);

        return new Ray(O, D);
    }

    /// <summary>
    /// Matrix m inverse for affine transforms. These include translation, rotation, scaling (including non-uniform) 
    /// and any combination of these.
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
        // However this version cannot be applied to scaling transforms because orthonormal condition breaks. Therefore
        // orthonormal assumption cannot be used, but final inverse has same structure as stated above
        double determinant = M00 * (M11 * M22 - M12 * M21) - M01 * (M10 * M22 - M12 * M20)
                            + M02 * (M10 * M21 - M11 * M20);

        if (Math.Abs(determinant) < 1e-12)
            throw new InvalidOperationException("Matrix not invertible");

        double invDet = 1.0 / determinant;

        // A^-1 = (1 / det) * Adj(A) where Adj(A) is the adjugate matrix.
        double r00 = (M11 * M22 - M12 * M21) * invDet;
        double r01 = (M02 * M21 - M01 * M22) * invDet;
        double r02 = (M01 * M12 - M02 * M11) * invDet;

        double r10 = (M12 * M20 - M10 * M22) * invDet;
        double r11 = (M00 * M22 - M02 * M20) * invDet;
        double r12 = (M02 * M10 - M00 * M12) * invDet;

        double r20 = (M10 * M21 - M11 * M20) * invDet;
        double r21 = (M01 * M20 - M00 * M21) * invDet;
        double r22 = (M00 * M11 - M01 * M10) * invDet;

        // -(A⁻¹ * t); this applies transform properly so coordinate x becomes x - a after applying transform vector a
        double t0 = -(r00 * M03 + r01 * M13 + r02 * M23);
        double t1 = -(r10 * M03 + r11 * M13 + r12 * M23);
        double t2 = -(r20 * M03 + r21 * M13 + r22 * M23);

        return new Matrix4x4(
            r00, r01, r02, t0,
            r10, r11, r12, t1,
            r20, r21, r22, t2,
            0, 0, 0, 1
        );
    }

    /// <summary>
    /// Matrix inverse. For affine transformations always use the faster InverseAffine method.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public Matrix4x4 Inverse()
    {
        double b00 = M00 * M11 - M01 * M10;
        double b01 = M00 * M12 - M02 * M10;
        double b02 = M00 * M13 - M03 * M10;
        double b03 = M01 * M12 - M02 * M11;
        double b04 = M01 * M13 - M03 * M11;
        double b05 = M02 * M13 - M03 * M12;
        double b06 = M20 * M31 - M21 * M30;
        double b07 = M20 * M32 - M22 * M30;
        double b08 = M20 * M33 - M23 * M30;
        double b09 = M21 * M32 - M22 * M31;
        double b10 = M21 * M33 - M23 * M31;
        double b11 = M22 * M33 - M23 * M32;

        double det =
            b00 * b11 - b01 * b10 + b02 * b09 +
            b03 * b08 - b04 * b07 + b05 * b06;

        if (Math.Abs(det) < 1e-12)
            throw new InvalidOperationException("Matrix not invertible");

        double invDet = 1.0 / det;

        // final inverse by direct calculation using shared subdeterminants b_ij
        return new Matrix4x4(
            (M11 * b11 - M12 * b10 + M13 * b09) * invDet,
            (-M01 * b11 + M02 * b10 - M03 * b09) * invDet,
            (M31 * b05 - M32 * b04 + M33 * b03) * invDet,
            (-M21 * b05 + M22 * b04 - M23 * b03) * invDet,

            (-M10 * b11 + M12 * b08 - M13 * b07) * invDet,
            (M00 * b11 - M02 * b08 + M03 * b07) * invDet,
            (-M30 * b05 + M32 * b02 - M33 * b01) * invDet,
            (M20 * b05 - M22 * b02 + M23 * b01) * invDet,

            (M10 * b10 - M11 * b08 + M13 * b06) * invDet,
            (-M00 * b10 + M01 * b08 - M03 * b06) * invDet,
            (M30 * b04 - M31 * b02 + M33 * b00) * invDet,
            (-M20 * b04 + M21 * b02 - M23 * b00) * invDet,

            (-M10 * b09 + M11 * b07 - M12 * b06) * invDet,
            (M00 * b09 - M01 * b07 + M02 * b06) * invDet,
            (-M30 * b03 + M31 * b01 - M32 * b00) * invDet,
            (M20 * b03 - M21 * b01 + M22 * b00) * invDet
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

    // translate aliases
    public static Matrix4x4 Translate(double x, double y, double z) => Translate(new Vec3(x, y, z));
    public static Matrix4x4 Shift(Vec3 v) => Translate(v);
    public static Matrix4x4 Shift(double x, double y, double z) => Translate(new Vec3(x, y, z));

    /// <summary>
    /// Scaling matrix to direction (x, y, z)
    /// </summary>
    public static Matrix4x4 Scale(double x, double y, double z)
    {
        return new Matrix4x4(
            x, 0, 0, 0,
            0, y, 0, 0,
            0, 0, z, 0,
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
            m1.M00 * m2.M00 + m1.M01 * m2.M10 + m1.M02 * m2.M20 + m1.M03 * m2.M30,
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
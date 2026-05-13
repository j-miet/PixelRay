namespace PixelRay.Core.Mathematics;

/// <summary>
/// 4x4 matrix struct for creating linear/affine transformation matrices
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
        // apply also translation component Mx3
        double x = M00 * p.X + M01 * p.Y + M02 * p.Z + M03;
        double y = M10 * p.X + M11 * p.Y + M12 * p.Z + M13;
        double z = M20 * p.X + M21 * p.Y + M22 * p.Z + M23;

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
    /// Matrix m inverse for 3-dimensional affine transforms. These include translation, rotation, scaling (including 
    /// non-uniform) and any combination of these.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public Matrix4x4 InverseAffine()
    {
        // Assume affine transform matrix with simple square form
        // [ A     T ]
        // [ 0 0 0 1 ]
        // where A is 3x3 matrix and T is a vector [tx, ty, tz]^T
        // Inverse of this is then simply
        // [ A^-1 -(A^1)*T]
        // [ 0   0   0  1 ]
        // If A would be orthonormal, R^1 = R^T meaning inverse equals to transpose. Thus this version cannot be 
        // applied to scaling transforms and inverse must be calculated manually, but final inverse has still the same 
        // structure as stated above

        // determinant of matrix 3x3 A
        double _a00 = M11 * M22 - M12 * M21;
        double _a01 = M10 * M22 - M12 * M20;
        double _a02 = M10 * M21 - M11 * M20;
        double det = M00 * _a00 - M01 * _a01 + M02 * _a02;

        if (Utils.IsEqual(det, 0, MathConst.MatrixEpsilon))
            throw new InvalidOperationException("Rotation matrix not invertible");

        double invDet = 1.0 / det;

        // nxn matrix inverse can be calculated as A^-1 = (1 / det) * Adj(A) where Adj(A) is the adjugate matrix.
        double a00 = _a00 * invDet;
        double a01 = _a01 * invDet;
        double a02 = _a02 * invDet;

        double a10 = (M12 * M20 - M10 * M22) * invDet;
        double a11 = (M00 * M22 - M02 * M20) * invDet;
        double a12 = (M02 * M10 - M00 * M12) * invDet;

        double a20 = (M10 * M21 - M11 * M20) * invDet;
        double a21 = (M01 * M20 - M00 * M21) * invDet;
        double a22 = (M00 * M11 - M01 * M10) * invDet;

        // -(A⁻¹ * t); this applies transform properly so vector x is shifted to x - (A^-1)*t
        double t0 = -(a00 * M03 + a01 * M13 + a02 * M23);
        double t1 = -(a10 * M03 + a11 * M13 + a12 * M23);
        double t2 = -(a20 * M03 + a21 * M13 + a22 * M23);

        return new Matrix4x4(
            a00, a01, a02, t0,
            a10, a11, a12, t1,
            a20, a21, a22, t2,
            0, 0, 0, 1
        );
    }

    /// <summary>
    /// Matrix inverse. For affine 3D transformations always use the faster InverseAffine method. Therefore this method 
    /// is not really required
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

        if (Utils.IsEqual(det, 0, MathConst.MatrixEpsilon))
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

    // scaling alias
    public static Matrix4x4 Scale(Vec3 v) => Scale(v.X, v.Y, v.Z);

    /// <summary>
    /// Uniform matrix scaling
    /// </summary>
    public static Matrix4x4 Scale(double t)
    {
        return Scale(t, t, t);
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
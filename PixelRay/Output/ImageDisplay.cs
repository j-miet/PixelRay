using Silk.NET.OpenGL;
using Silk.NET.Windowing;

using PixelRay.Core;
using PixelRay.Mathematics;

namespace PixelRay.Output;

/// <summary>
/// For rendering saved PPM images in a new window using SILK.NET library
/// </summary>
public class ImageDisplay
{
    private readonly int _width;
    private readonly int _height;
    private readonly FrameBuffer _buffer;
    private readonly int _renderSpeed;
    private readonly byte[] _frameBuffer;
    private readonly byte[] _emptyBuffer;
    private volatile bool _dirty;

    private uint _vao;
    private uint _vbo;
    private uint _shaderProgram;

    private WindowOptions _options;
    private IWindow? _window;
    private GL? _gl;
    private uint _texture;

    /// <summary>
    /// Create a new ImageDisplay instance. Then call Display(...) to render image in a new window.
    /// </summary>
    /// <param name="width">Image width</param>
    /// <param name="height">Image height</param>
    /// <param name="buffer">PPM image data</param>
    /// <param name="renderSpeed">How fast it takes to render a single pixel, in milliseconds. Default value 1.
    /// Use higher values if you'd like to see the image generated progressively or use 0 for max speed.</param>
    public ImageDisplay(int width, int height, FrameBuffer buffer, int renderSpeed = 1)
    {
        _width = width;
        _height = height;
        _buffer = buffer;
        _renderSpeed = renderSpeed < 0 ? 1 : renderSpeed;

        _emptyBuffer = new byte[_width * _height * 3]; // for creating initial black background image
        _frameBuffer = new byte[_width * _height * 3]; // rendered image
        _dirty = true;
    }

    /// <summary>
    /// Open a separate window to display image rendering process
    /// </summary>
    public void Display()
    {
        _options = WindowOptions.Default;
        _options.Title = "PixelRay.Image";
        _options.Size = new Silk.NET.Maths.Vector2D<int>(_width, _height);
        _options.API = new GraphicsAPI(
            ContextAPI.OpenGL,
            ContextProfile.Core,
            ContextFlags.Default,
            new APIVersion(3, 3));

        _window = Window.Create(_options);

        _window.Load += OnLoad;
        _window.Render += OnRender;
        _window.Closing += OnClose;

        _window.Run();
    }

    private void OnLoad()
    {
        if (_window == null)
            throw new Exception("Null window object ref");

        _gl = GL.GetApi(_window);

        // Base texture
        _texture = _gl.GenTexture();
        _gl.BindTexture(TextureTarget.Texture2D, _texture);
        _gl.TexImage2D(
            TextureTarget.Texture2D,
            0,
            InternalFormat.Rgb8,
            (uint)_width,
            (uint)_height,
            0,
            PixelFormat.Rgb,
            PixelType.UnsignedByte,
            _emptyBuffer);

        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.Linear);
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Linear);

        // a square screen can be represented with 2 triangles
        // position coordinates range from -1 to 1 whereas OpenGL texture coordinates are from 0 to 1
        // texture coordinates start from bottom-left (0, 0)
        // so in order to map two triangles to form a square, we can do
        // 1. (0, 0) -> (1, 0) -> (1, 1) then connect (1, 1) -> (0, 0) but this doesn't need to be explicitly stated
        // 2. (0, 0) -> (1, 0) -> (0, 1) then connect (0, 1) -> (0, 0), same as above
        float[] vertices =
        [
            // positions   // texcoords
            -1f, -1f,      0f, 0f,
            1f, -1f,      1f, 0f,
            1f,  1f,      1f, 1f,

            -1f, -1f,      0f, 0f,
            1f,  1f,      1f, 1f,
            -1f,  1f,      0f, 1f,
        ];

        _vao = _gl.GenVertexArray();
        _vbo = _gl.GenBuffer();

        _gl.BindVertexArray(_vao);
        _gl.BindBuffer(BufferTargetARB.ArrayBuffer, _vbo);
        _gl.BufferData(BufferTargetARB.ArrayBuffer, vertices, BufferUsageARB.StaticDraw);
        _gl.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
        _gl.EnableVertexAttribArray(0);
        _gl.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 2 * sizeof(float));
        _gl.EnableVertexAttribArray(1);

        _shaderProgram = CreateShader();

        // start a separate thread which reads data into render buffer
        new Thread(RenderLoop).Start();
    }

    private void OnRender(double deltaTime)
    {
        if (_gl == null)
            throw new Exception("GL object is null");

        if (_dirty)
        {
            // Rendered texture
            _gl.BindTexture(TextureTarget.Texture2D, _texture);
            _gl.TexSubImage2D(
                TextureTarget.Texture2D,
                0,
                0,
                0,
                (uint)_width,
                (uint)_height,
                PixelFormat.Rgb,
                PixelType.UnsignedByte,
                _frameBuffer);

            _dirty = false;
        }

        _gl.Clear((uint)ClearBufferMask.ColorBufferBit);
        _gl.UseProgram(_shaderProgram);
        _gl.BindVertexArray(_vao);
        _gl.BindTexture(TextureTarget.Texture2D, _texture);
        _gl.DrawArrays(PrimitiveType.Triangles, 0, 6);
    }

    private void OnClose()
    {
        if (_gl == null)
            throw new Exception("GL object is null");

        _gl.DeleteTexture(_texture);
    }

    private uint CreateShader()
    {
        if (_gl == null)
            throw new Exception("GL object is null");

        string vertexShaderSource = @"
        #version 330 core
        layout (location = 0) in vec2 aPos;
        layout (location = 1) in vec2 aTex;

        out vec2 TexCoord;

        void main()
        {
            TexCoord = aTex;
            gl_Position = vec4(aPos, 0.0, 1.0);
        }";

        // here texture coordinates y-coordinate is set to 1-y so image gets rendered from top-left; otherwise final
        // image is upside-down
        string fragmentShaderSource = @"
        #version 330 core
        out vec4 FragColor;
        in vec2 TexCoord;

        uniform sampler2D screenTexture;

        void main()
        {
            FragColor = texture(screenTexture, vec2(TexCoord.x, 1.0 - TexCoord.y));
        }";

        uint vertex = _gl.CreateShader(ShaderType.VertexShader);
        _gl.ShaderSource(vertex, vertexShaderSource);
        _gl.CompileShader(vertex);

        uint fragment = _gl.CreateShader(ShaderType.FragmentShader);
        _gl.ShaderSource(fragment, fragmentShaderSource);
        _gl.CompileShader(fragment);

        uint program = _gl.CreateProgram();
        _gl.AttachShader(program, vertex);
        _gl.AttachShader(program, fragment);
        _gl.LinkProgram(program);

        _gl.DeleteShader(vertex);
        _gl.DeleteShader(fragment);

        return program;
    }

    private void RenderLoop()
    {
        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                int i = (y * _width + x) * 3;

                ColorRGB pixelColor = _buffer.GetPixel(x, y);
                _frameBuffer[i] = (byte)(pixelColor.R * 255);
                _frameBuffer[i + 1] = (byte)(pixelColor.G * 255);
                _frameBuffer[i + 2] = (byte)(pixelColor.B * 255);
            }

            _dirty = true;
            Thread.Sleep(_renderSpeed);
        }
    }
}
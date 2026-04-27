using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.GLControl;

namespace VirtualRubiksCube
{
    public class RubiksCubeRenderer : IDisposable
    {
        #region Constants
        private const int FacesPerCubelet = 6;
        private const int VerticesPerFace = 4;
        private const int TriIndicesPerFace = 6;
        private const int LineIndicesPerFace = 8;

        // 4 vertex indices into Cubelet.Vertices for each of the 6 faces, matching Cubelet.cs.
        private static readonly int[][] FaceVertexIndices = new[]
        {
            new[] { 0, 1, 2, 3 }, // top
            new[] { 4, 5, 6, 7 }, // bottom
            new[] { 0, 3, 7, 4 }, // left
            new[] { 1, 2, 6, 5 }, // right
            new[] { 3, 2, 6, 7 }, // front
            new[] { 0, 1, 5, 4 }, // back
        };
        #endregion

        #region Vertex
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct VertexData
        {
            public float X, Y, Z;
            public int ColorIndex;
            public int SelectionMode;
            public int HoverFlag;

            public const int SizeBytes = 3 * sizeof(float) + 3 * sizeof(int);
        }
        #endregion

        #region Fields
        private readonly RubiksCube rubiksCube;
        private readonly GLControl glControl;
        private RenderInfo currentRenderInfo;

        private int program;
        private int vao;
        private int vbo;
        private int triEbo;
        private int lineEbo;

        private int uViewLoc;
        private int uProjectionLoc;
        private int uColorsLoc;
        private int uIsLineLoc;

        private VertexData[] vertexBuffer = Array.Empty<VertexData>();
        private uint[] triangleIndices = Array.Empty<uint>();
        private uint[] lineIndices = Array.Empty<uint>();

        private bool initialized;
        private bool disposed;
        #endregion

        #region Public state
        public bool IsRunning => initialized && !disposed;
        public void SetRenderInfo(RenderInfo renderInfo) => currentRenderInfo = renderInfo;
        public Face3D? HoveredFace { get; set; }
        #endregion

        #region Constructor
        public RubiksCubeRenderer(RubiksCube rubiksCube, GLControl glControl, RenderInfo initialRenderInfo)
        {
            this.rubiksCube = rubiksCube;
            this.glControl = glControl;
            currentRenderInfo = initialRenderInfo;
            rubiksCube.Renderer = this;
        }
        #endregion

        #region Initialization
        public void Initialize()
        {
            if (initialized) return;
            glControl.MakeCurrent();

            program = CreateProgram();
            uViewLoc = GL.GetUniformLocation(program, "uView");
            uProjectionLoc = GL.GetUniformLocation(program, "uProjection");
            uColorsLoc = GL.GetUniformLocation(program, "uColors");
            uIsLineLoc = GL.GetUniformLocation(program, "uIsLine");

            BuildIndexBuffers();
            BuildVertexBuffer();

            vao = GL.GenVertexArray();
            vbo = GL.GenBuffer();
            triEbo = GL.GenBuffer();
            lineEbo = GL.GenBuffer();

            GL.BindVertexArray(vao);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertexBuffer.Length * VertexData.SizeBytes,
                vertexBuffer, BufferUsageHint.StreamDraw);

            // location 0 = position (vec3)
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, VertexData.SizeBytes, 0);
            GL.EnableVertexAttribArray(0);
            // location 1 = colorIndex (int)
            GL.VertexAttribIPointer(1, 1, VertexAttribIntegerType.Int, VertexData.SizeBytes, (IntPtr)(3 * sizeof(float)));
            GL.EnableVertexAttribArray(1);
            // location 2 = selectionMode (int)
            GL.VertexAttribIPointer(2, 1, VertexAttribIntegerType.Int, VertexData.SizeBytes, (IntPtr)(3 * sizeof(float) + sizeof(int)));
            GL.EnableVertexAttribArray(2);
            // location 3 = hoverFlag (int)
            GL.VertexAttribIPointer(3, 1, VertexAttribIntegerType.Int, VertexData.SizeBytes, (IntPtr)(3 * sizeof(float) + 2 * sizeof(int)));
            GL.EnableVertexAttribArray(3);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, triEbo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, triangleIndices.Length * sizeof(uint),
                triangleIndices, BufferUsageHint.StaticDraw);

            GL.BindVertexArray(0);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, lineEbo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, lineIndices.Length * sizeof(uint),
                lineIndices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Lequal);
            GL.Disable(EnableCap.CullFace);
            GL.ClearColor(0.93f, 0.93f, 0.93f, 1.0f);

            initialized = true;
        }

        private void BuildIndexBuffers()
        {
            int cubeletCount = rubiksCube.Cubelets.Count;
            triangleIndices = new uint[cubeletCount * FacesPerCubelet * TriIndicesPerFace];
            lineIndices = new uint[cubeletCount * FacesPerCubelet * LineIndicesPerFace];

            uint vertexOffset = 0;
            int triWrite = 0;
            int lineWrite = 0;
            for (int c = 0; c < cubeletCount; c++)
            {
                for (int f = 0; f < FacesPerCubelet; f++)
                {
                    uint a = vertexOffset;
                    uint b = vertexOffset + 1;
                    uint cc = vertexOffset + 2;
                    uint d = vertexOffset + 3;

                    triangleIndices[triWrite++] = a;
                    triangleIndices[triWrite++] = b;
                    triangleIndices[triWrite++] = cc;
                    triangleIndices[triWrite++] = a;
                    triangleIndices[triWrite++] = cc;
                    triangleIndices[triWrite++] = d;

                    lineIndices[lineWrite++] = a; lineIndices[lineWrite++] = b;
                    lineIndices[lineWrite++] = b; lineIndices[lineWrite++] = cc;
                    lineIndices[lineWrite++] = cc; lineIndices[lineWrite++] = d;
                    lineIndices[lineWrite++] = d; lineIndices[lineWrite++] = a;

                    vertexOffset += VerticesPerFace;
                }
            }
        }

        private void BuildVertexBuffer()
        {
            int cubeletCount = rubiksCube.Cubelets.Count;
            int vertCount = cubeletCount * FacesPerCubelet * VerticesPerFace;
            if (vertexBuffer.Length != vertCount)
                vertexBuffer = new VertexData[vertCount];
        }
        #endregion

        #region Rendering
        public void Render(RenderInfo renderInfo)
        {
            if (!initialized) return;
            currentRenderInfo = renderInfo;

            UpdateVertexBuffer();

            GL.Viewport(0, 0, glControl.ClientSize.Width, glControl.ClientSize.Height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.UseProgram(program);

            Matrix4 view = BuildViewMatrix(renderInfo);
            Matrix4 projection = BuildProjectionMatrix(renderInfo, glControl.ClientSize.Width, glControl.ClientSize.Height);

            GL.UniformMatrix4(uViewLoc, true, ref view);
            GL.UniformMatrix4(uProjectionLoc, true, ref projection);

            float[] colors = BuildColorsArray(renderInfo);
            GL.Uniform4(uColorsLoc, 7, colors);

            GL.BindVertexArray(vao);

            // Upload current vertex data
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertexBuffer.Length * VertexData.SizeBytes,
                vertexBuffer, BufferUsageHint.StreamDraw);

            // Pass 1: filled triangles
            GL.Uniform1(uIsLineLoc, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, triEbo);
            GL.DrawElements(PrimitiveType.Triangles, triangleIndices.Length, DrawElementsType.UnsignedInt, 0);

            // Pass 2: black face borders
            GL.Enable(EnableCap.PolygonOffsetLine);
            GL.PolygonOffset(-1f, -1f);
            GL.LineWidth(1.5f);
            GL.Uniform1(uIsLineLoc, 1);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, lineEbo);
            GL.DrawElements(PrimitiveType.Lines, lineIndices.Length, DrawElementsType.UnsignedInt, 0);
            GL.Disable(EnableCap.PolygonOffsetLine);

            GL.BindVertexArray(0);

            glControl.SwapBuffers();
        }

        private void UpdateVertexBuffer()
        {
            Cubelet? hoverCubelet = HoveredFace?.Cubelet;
            int hoverFaceIndex = HoveredFace?.CubeletFaceIndex ?? -1;

            int write = 0;
            foreach (Cubelet cubelet in rubiksCube.Cubelets)
            {
                bool sameCubelet = cubelet == hoverCubelet;
                for (int f = 0; f < FacesPerCubelet; f++)
                {
                    int colorIndex = GetColorIndex(cubelet, f);
                    int selection = (int)cubelet.GetSelectionMode((byte)f);
                    int hover = (sameCubelet && f == hoverFaceIndex) ? 1 : 0;
                    int[] vi = FaceVertexIndices[f];
                    for (int v = 0; v < VerticesPerFace; v++)
                    {
                        Point3D p = cubelet.Vertices[vi[v]];
                        vertexBuffer[write].X = (float)p.X;
                        vertexBuffer[write].Y = (float)p.Y;
                        vertexBuffer[write].Z = (float)p.Z;
                        vertexBuffer[write].ColorIndex = colorIndex;
                        vertexBuffer[write].SelectionMode = selection;
                        vertexBuffer[write].HoverFlag = hover;
                        write++;
                    }
                }
            }
        }

        private static int GetColorIndex(Cubelet cubelet, int faceIndex)
        {
            RubiksCube.Layer layer = faceIndex switch
            {
                0 => RubiksCube.Layer.Up,
                1 => RubiksCube.Layer.Down,
                2 => RubiksCube.Layer.Left,
                3 => RubiksCube.Layer.Right,
                4 => RubiksCube.Layer.Front,
                5 => RubiksCube.Layer.Back,
                _ => RubiksCube.Layer.None,
            };
            if (!cubelet.OriginalLayers.HasFlag(layer)) return 0;
            return faceIndex + 1; // 1..6
        }

        private static Matrix4 BuildViewMatrix(RenderInfo renderInfo)
        {
            // Match Point3D.Rotate order: Z, then X, then Y.
            // OpenTK row-vector convention: leftmost matrix applied first.
            float zRad = (float)(renderInfo.RotationZ * Math.PI / 180.0);
            float xRad = (float)(renderInfo.RotationX * Math.PI / 180.0);
            float yRad = (float)(renderInfo.RotationY * Math.PI / 180.0);
            return Matrix4.CreateRotationZ(zRad)
                 * Matrix4.CreateRotationX(xRad)
                 * Matrix4.CreateRotationY(yRad);
        }

        private static Matrix4 BuildProjectionMatrix(RenderInfo renderInfo, int width, int height)
        {
            // Match the original CPU projection:
            //   x_screen = X * (imageDistance/viewDistance) + width/2
            //   y_screen = Y * (imageDistance/viewDistance) + height/2  (Y down)
            // In NDC after viewport: x_ndc = (x_screen / width)*2 - 1, but GL window Y is up.
            float scale = (float)renderInfo.ImageDistance / Math.Max(1, renderInfo.ViewDistance);
            float orthoWidth = width / scale;
            float orthoHeight = height / scale;
            // Negate Z range so larger world-Z maps to smaller depth (closer to viewer).
            // Cubelets span ~±200 in world Z; allow generous range.
            const float depthRange = 4000f;
            return Matrix4.CreateOrthographicOffCenter(
                -orthoWidth / 2f,
                orthoWidth / 2f,
                orthoHeight / 2f,    // bottom (positive Y is "down" in our model -> map to negative NDC Y after flip)
                -orthoHeight / 2f,   // top
                -depthRange,
                depthRange);
        }

        private static float[] BuildColorsArray(RenderInfo renderInfo)
        {
            // 7 vec4s: 0=none(black), 1=top, 2=bottom, 3=left, 4=right, 5=front, 6=back
            Color[] cs = new[]
            {
                Color.Black,
                renderInfo.TopFaceColor,
                renderInfo.BottomFaceColor,
                renderInfo.LeftFaceColor,
                renderInfo.RightFaceColor,
                renderInfo.FrontFaceColor,
                renderInfo.BackFaceColor,
            };
            float[] result = new float[7 * 4];
            for (int i = 0; i < 7; i++)
            {
                result[i * 4 + 0] = cs[i].R / 255f;
                result[i * 4 + 1] = cs[i].G / 255f;
                result[i * 4 + 2] = cs[i].B / 255f;
                result[i * 4 + 3] = 1f;
            }
            return result;
        }
        #endregion

        #region Hit testing
        public Face3D? HitTest(Point mouse, RenderInfo renderInfo)
        {
            // CPU projection of every face, point-in-polygon test, frontmost wins.
            // Frontmost = largest min-Z (matches the original painter's algorithm sort).
            int width = glControl.ClientSize.Width;
            int height = glControl.ClientSize.Height;
            Face3D? hit = null;
            double hitMinZ = double.NegativeInfinity;

            foreach (Face3D face in rubiksCube.Faces)
            {
                Point3D[] rotated = new Point3D[face.Vertices.Length];
                for (int i = 0; i < face.Vertices.Length; i++)
                    rotated[i] = face.Vertices[i].Rotate(renderInfo.RotationX, renderInfo.RotationY, renderInfo.RotationZ);

                PointF[] screen = new PointF[rotated.Length];
                for (int i = 0; i < rotated.Length; i++)
                {
                    Point3D p = rotated[i].Project(width, height, renderInfo.ImageDistance, renderInfo.ViewDistance);
                    screen[i] = new PointF((float)p.X, (float)p.Y);
                }

                using GraphicsPath path = new();
                path.AddPolygon(screen);
                if (!path.IsVisible(mouse)) continue;

                double minZ = rotated.Min(p => p.Z);
                if (minZ > hitMinZ)
                {
                    hitMinZ = minZ;
                    hit = face;
                }
            }
            return hit;
        }
        #endregion

        #region Shaders
        private static int CreateProgram()
        {
            const string vsSrc = @"
                #version 330 core
                layout (location = 0) in vec3 aPos;
                layout (location = 1) in int aColorIndex;
                layout (location = 2) in int aSelectionMode;
                layout (location = 3) in int aHoverFlag;
                flat out int vColorIndex;
                flat out int vSelectionMode;
                flat out int vHoverFlag;
                uniform mat4 uView;
                uniform mat4 uProjection;
                void main()
                {
                    gl_Position = uProjection * uView * vec4(aPos, 1.0);
                    vColorIndex = aColorIndex;
                    vSelectionMode = aSelectionMode;
                    vHoverFlag = aHoverFlag;
                }";

            const string fsSrc = @"
                #version 330 core
                flat in int vColorIndex;
                flat in int vSelectionMode;
                flat in int vHoverFlag;
                out vec4 FragColor;
                uniform vec4 uColors[7];
                uniform int uIsLine;
                void main()
                {
                    if (uIsLine == 1)
                    {
                        if (vHoverFlag == 1) FragColor = vec4(1.0, 0.84, 0.0, 1.0);
                        else FragColor = vec4(0.0, 0.0, 0.0, 1.0);
                        return;
                    }
                    vec4 c = uColors[vColorIndex];
                    if (vSelectionMode == 1)       c.rgb *= 0.30;
                    else if (vSelectionMode == 2)  c.rgb *= 0.55;
                    if (vHoverFlag == 1)           c.rgb = mix(c.rgb, vec3(1.0, 0.84, 0.0), 0.35);
                    FragColor = c;
                }";

            int vs = CompileShader(ShaderType.VertexShader, vsSrc);
            int fs = CompileShader(ShaderType.FragmentShader, fsSrc);
            int prog = GL.CreateProgram();
            GL.AttachShader(prog, vs);
            GL.AttachShader(prog, fs);
            GL.LinkProgram(prog);
            GL.GetProgram(prog, GetProgramParameterName.LinkStatus, out int linked);
            if (linked == 0)
            {
                string log = GL.GetProgramInfoLog(prog);
                throw new InvalidOperationException("Shader link failed: " + log);
            }
            GL.DetachShader(prog, vs);
            GL.DetachShader(prog, fs);
            GL.DeleteShader(vs);
            GL.DeleteShader(fs);
            return prog;
        }

        private static int CompileShader(ShaderType type, string source)
        {
            int s = GL.CreateShader(type);
            GL.ShaderSource(s, source);
            GL.CompileShader(s);
            GL.GetShader(s, ShaderParameter.CompileStatus, out int ok);
            if (ok == 0)
            {
                string log = GL.GetShaderInfoLog(s);
                throw new InvalidOperationException($"{type} compile failed: {log}");
            }
            return s;
        }
        #endregion

        #region Disposal
        public void Dispose()
        {
            if (disposed) return;
            disposed = true;
            try
            {
                if (initialized && !glControl.IsDisposed)
                {
                    glControl.MakeCurrent();
                    if (vbo != 0) GL.DeleteBuffer(vbo);
                    if (triEbo != 0) GL.DeleteBuffer(triEbo);
                    if (lineEbo != 0) GL.DeleteBuffer(lineEbo);
                    if (vao != 0) GL.DeleteVertexArray(vao);
                    if (program != 0) GL.DeleteProgram(program);
                }
            }
            catch { }
        }
        #endregion
    }
}

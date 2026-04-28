namespace VirtualRubiksCube.Solver
{
    // 54-sticker representation of a 3x3 cube, Singmaster/Kociemba layout:
    //   indices  0..8  = U face,  9..17 = R,  18..26 = F,  27..35 = D,  36..44 = L,  45..53 = B
    //
    // Stickers are stored as char labels 'U','D','L','R','F','B' meaning "this sticker came from the
    // original face whose center has that label". This is the only color information the solver needs:
    // the 6 center stickers' labels never change because centers don't move.
    //
    // Apply moves with ApplyMove(face, quarter). The 6 quarter-CW permutations are precomputed once
    // in the static constructor by simulating each move on a uniquely labeled cube.
    public class FaceletCube
    {
        public const int FaceletCount = 54;

        public const int U0 = 0;
        public const int R0 = 9;
        public const int F0 = 18;
        public const int D0 = 27;
        public const int L0 = 36;
        public const int B0 = 45;

        public char[] Facelets { get; }

        private FaceletCube(char[] facelets) { Facelets = facelets; }

        public FaceletCube Clone() => new FaceletCube((char[])Facelets.Clone());

        public bool IsSolved()
        {
            for (int f = 0; f < 6; f++)
            {
                int start = f * 9;
                char center = Facelets[start + 4];
                for (int i = 0; i < 9; i++)
                    if (Facelets[start + i] != center)
                        return false;
            }
            return true;
        }

        public static FaceletCube Solved()
        {
            char[] f = new char[FaceletCount];
            for (int i = 0; i < 9; i++) f[U0 + i] = 'U';
            for (int i = 0; i < 9; i++) f[R0 + i] = 'R';
            for (int i = 0; i < 9; i++) f[F0 + i] = 'F';
            for (int i = 0; i < 9; i++) f[D0 + i] = 'D';
            for (int i = 0; i < 9; i++) f[L0 + i] = 'L';
            for (int i = 0; i < 9; i++) f[B0 + i] = 'B';
            return new FaceletCube(f);
        }

        // Build a facelet cube from the live controller state.
        public static FaceletCube FromControllerState(RubiksCube cube)
        {
            char[] facelets = new char[FaceletCount];
            for (int i = 0; i < FaceletCount; i++) facelets[i] = '?';

            foreach (Cubelet cubelet in cube.Cubelets)
            {
                (sbyte x, sbyte y, sbyte z) = cubelet.CurrentPosition;
                if (y == -1) EmitSticker(facelets, cubelet, x, y, z, RubiksCube.Face.Top);
                if (y ==  1) EmitSticker(facelets, cubelet, x, y, z, RubiksCube.Face.Bottom);
                if (x == -1) EmitSticker(facelets, cubelet, x, y, z, RubiksCube.Face.Left);
                if (x ==  1) EmitSticker(facelets, cubelet, x, y, z, RubiksCube.Face.Right);
                if (z ==  1) EmitSticker(facelets, cubelet, x, y, z, RubiksCube.Face.Front);
                if (z == -1) EmitSticker(facelets, cubelet, x, y, z, RubiksCube.Face.Back);
            }

            return new FaceletCube(facelets);
        }

        private static void EmitSticker(char[] facelets, Cubelet cubelet, sbyte x, sbyte y, sbyte z, RubiksCube.Face worldDir)
        {
            // Find the slot whose current value matches the world direction the sticker is exposed in.
            byte slot = 255;
            for (byte s = 0; s < 6; s++)
                if (cubelet.CurrentFaces[s] == worldDir) { slot = s; break; }
            if (slot == 255) return;

            // Slot 0 was initially Top, slot 1 Bottom, etc. The slot's "color label" is fixed —
            // it identifies which original cube face's color this side of the cubelet shows.
            char color = SlotToColor(slot);
            int idx = WorldToFaceletIndex(worldDir, x, y, z);
            if (idx >= 0) facelets[idx] = color;
        }

        // Slot index → label of the original face that slot's sticker came from.
        // (Cubelet.cs:174-179 initializes currentFaces[0..5] = Top, Bottom, Left, Right, Front, Back.)
        private static char SlotToColor(byte slot) => slot switch
        {
            0 => 'U',
            1 => 'D',
            2 => 'L',
            3 => 'R',
            4 => 'F',
            5 => 'B',
            _ => '?',
        };

        // Map (world face direction, cubelet world position) → facelet array index.
        // Layout chosen so that:
        //   U: row index = z+1 (back→front), col index = x+1 (left→right)
        //   D: row index = 1-z (front→back), col = x+1
        //   F: row = y+1 (top→bottom), col = x+1
        //   B: row = y+1, col = 1-x
        //   R: row = y+1, col = 1-z (front→back)
        //   L: row = y+1, col = z+1 (back→front)
        // Adjacent-face stickers wrap consistently (the 4 stickers around any corner cubelet
        // sit at (row, col) coordinates that line up after physical rotation).
        public static int WorldToFaceletIndex(RubiksCube.Face face, sbyte x, sbyte y, sbyte z)
        {
            return face switch
            {
                RubiksCube.Face.Top    => U0 + (z + 1) * 3 + (x + 1),
                RubiksCube.Face.Right  => R0 + (y + 1) * 3 + (1 - z),
                RubiksCube.Face.Front  => F0 + (y + 1) * 3 + (x + 1),
                RubiksCube.Face.Bottom => D0 + (1 - z) * 3 + (x + 1),
                RubiksCube.Face.Left   => L0 + (y + 1) * 3 + (z + 1),
                RubiksCube.Face.Back   => B0 + (y + 1) * 3 + (1 - x),
                _ => -1,
            };
        }

        // ---------- Apply moves ----------

        // perms[face] gives the CW quarter-turn permutation: new[i] = old[perms[face][i]].
        private static readonly Dictionary<char, int[]> CwPerms = BuildPerms();
        // Inverse permutations (= CCW quarter-turns). Computed alongside.
        private static readonly Dictionary<char, int[]> CcwPerms = BuildInverses(CwPerms);
        // Initialized after CwPerms/CcwPerms so BuildOrientations can call ApplyMove during type init.
        public static readonly OrientationEntry[] Orientations = BuildOrientations();

        public void ApplyMove(SolverMove move)
        {
            if (move.Quarter == 1) ApplyPerm(CwPerms[move.Face]);
            else if (move.Quarter == -1) ApplyPerm(CcwPerms[move.Face]);
            else { ApplyPerm(CwPerms[move.Face]); ApplyPerm(CwPerms[move.Face]); }
        }

        public void ApplyMoves(IEnumerable<SolverMove> moves)
        {
            foreach (var m in moves) ApplyMove(m);
        }

        public FaceletCube ApplyOrientation(int[] perm)
        {
            char[] next = new char[FaceletCount];
            for (int i = 0; i < FaceletCount; i++) next[i] = Facelets[perm[i]];
            return new FaceletCube(next);
        }

        // Apply orientation AND relabel stickers so the solver's hardcoded face names
        // (D=bottom, U=top, F/R/B/L=sides) are correct for this orientation.
        // Without relabeling, the solver would place the wrong colors in Phase 1's D-cross etc.
        public FaceletCube ApplyOrientationWithRelabel(OrientationEntry oe)
        {
            // Build relabel map: original face label -> solver face label.
            // oe.OrigFace[fi] = original face for solver face fi, so
            //   original label X becomes solver label faces[fi] where oe.OrigFace[fi] == X.
            char[] faces = { 'U', 'R', 'F', 'D', 'L', 'B' };
            var relabel = new char['Z' + 1]; // small ASCII range; 'B'..'U' covers all labels
            for (int fi = 0; fi < 6; fi++)
                relabel[oe.OrigFace[fi]] = faces[fi];

            char[] next = new char[FaceletCount];
            for (int i = 0; i < FaceletCount; i++)
                next[i] = relabel[Facelets[oe.Perm[i]]];
            return new FaceletCube(next);
        }

        public static SolverMove TranslateMove(SolverMove m, OrientationEntry oe)
        {
            int fi = FaceIndex(m.Face);
            return new SolverMove(oe.OrigFace[fi], m.Quarter == 2 ? 2 : m.Quarter * oe.DirFlip[fi]);
        }

        private void ApplyPerm(int[] perm)
        {
            char[] next = new char[FaceletCount];
            for (int i = 0; i < FaceletCount; i++) next[i] = Facelets[perm[i]];
            Array.Copy(next, Facelets, FaceletCount);
        }

        // ---------- Permutation construction ----------
        // Build the 6 CW permutations once at startup by simulating each move on a uniquely
        // labeled cube using the same coordinate math the model uses. This keeps the solver
        // and the model perfectly aligned without hand-typed permutation tables.
        private static Dictionary<char, int[]> BuildPerms()
        {
            var result = new Dictionary<char, int[]>();
            foreach (char face in new[] { 'U', 'D', 'L', 'R', 'F', 'B' })
                result[face] = ComputeFacePerm(face);
            return result;
        }

        private static Dictionary<char, int[]> BuildInverses(Dictionary<char, int[]> perms)
        {
            var result = new Dictionary<char, int[]>();
            foreach (var kv in perms)
            {
                int[] inv = new int[FaceletCount];
                for (int i = 0; i < FaceletCount; i++) inv[kv.Value[i]] = i;
                result[kv.Key] = inv;
            }
            return result;
        }

        // ---------- Whole-cube orientation support ----------

        private static int FaceIndex(char c) => c switch
        {
            'U' => 0, 'R' => 1, 'F' => 2, 'D' => 3, 'L' => 4, 'B' => 5,
            _ => throw new ArgumentException($"Unknown face '{c}'"),
        };

        // result[i] = a[b[i]]: applying perm a then perm b in sequence.
        private static int[] ComposePerms(int[] a, int[] b)
        {
            var r = new int[FaceletCount];
            for (int i = 0; i < FaceletCount; i++) r[i] = a[b[i]];
            return r;
        }

        private static int[] BuildInversePerm(int[] perm)
        {
            var inv = new int[FaceletCount];
            for (int i = 0; i < FaceletCount; i++) inv[perm[i]] = i;
            return inv;
        }

        // Like ComputeFacePerm but rotates all 27 cubies (whole-cube rotation).
        private static int[] ComputeWholeRotPerm(RubiksCube.Axis axis, int targetAngle)
        {
            var cubies = new List<SimCubie>();
            for (sbyte x = -1; x <= 1; x++)
                for (sbyte y = -1; y <= 1; y++)
                    for (sbyte z = -1; z <= 1; z++)
                    {
                        var c = new SimCubie { X = x, Y = y, Z = z };
                        if (y == -1) c.Up    = WorldToFaceletIndex(RubiksCube.Face.Top,    x, y, z);
                        if (y ==  1) c.Down  = WorldToFaceletIndex(RubiksCube.Face.Bottom, x, y, z);
                        if (x == -1) c.Left  = WorldToFaceletIndex(RubiksCube.Face.Left,   x, y, z);
                        if (x ==  1) c.Right = WorldToFaceletIndex(RubiksCube.Face.Right,  x, y, z);
                        if (z ==  1) c.Front = WorldToFaceletIndex(RubiksCube.Face.Front,  x, y, z);
                        if (z == -1) c.Back  = WorldToFaceletIndex(RubiksCube.Face.Back,   x, y, z);
                        cubies.Add(c);
                    }

            foreach (var c in cubies) RotateCubie(c, axis, targetAngle);

            int[] post = new int[FaceletCount];
            for (int i = 0; i < FaceletCount; i++) post[i] = -1;
            foreach (var c in cubies)
            {
                if (c.Y == -1 && c.Up    >= 0) post[WorldToFaceletIndex(RubiksCube.Face.Top,    c.X, c.Y, c.Z)] = c.Up;
                if (c.Y ==  1 && c.Down  >= 0) post[WorldToFaceletIndex(RubiksCube.Face.Bottom, c.X, c.Y, c.Z)] = c.Down;
                if (c.X == -1 && c.Left  >= 0) post[WorldToFaceletIndex(RubiksCube.Face.Left,   c.X, c.Y, c.Z)] = c.Left;
                if (c.X ==  1 && c.Right >= 0) post[WorldToFaceletIndex(RubiksCube.Face.Right,  c.X, c.Y, c.Z)] = c.Right;
                if (c.Z ==  1 && c.Front >= 0) post[WorldToFaceletIndex(RubiksCube.Face.Front,  c.X, c.Y, c.Z)] = c.Front;
                if (c.Z == -1 && c.Back  >= 0) post[WorldToFaceletIndex(RubiksCube.Face.Back,   c.X, c.Y, c.Z)] = c.Back;
            }
            return post;
        }

        // For each of the 6 solver face letters, compute which original face and direction
        // corresponds to turning that face CW in the reoriented cube.
        private static (char[], int[]) ComputeMoveTranslation(int[] rotPerm, int[] invRotPerm)
        {
            char[] faces = { 'U', 'R', 'F', 'D', 'L', 'B' };
            char[] origFace = new char[6];
            int[] dirFlip  = new int[6];

            for (int fi = 0; fi < 6; fi++)
            {
                char f = faces[fi];

                // Reorient solved cube, apply CW turn of f, undo reorientation.
                var rotated = Solved().ApplyOrientation(rotPerm);
                rotated.ApplyMove(new SolverMove(f, 1));
                var unrotated = rotated.ApplyOrientation(invRotPerm);

                // Find the original face move that produces the same result from a solved cube.
                bool found = false;
                foreach (char of in faces)
                {
                    foreach (int q in new[] { 1, -1 })
                    {
                        var test = Solved();
                        test.ApplyMove(new SolverMove(of, q));
                        bool match = true;
                        for (int i = 0; i < FaceletCount; i++)
                            if (test.Facelets[i] != unrotated.Facelets[i]) { match = false; break; }
                        if (match) { origFace[fi] = of; dirFlip[fi] = q; found = true; break; }
                    }
                    if (found) break;
                }
                if (!found)
                    throw new InvalidOperationException($"No original-face match for reoriented face '{f}'");
            }
            return (origFace, dirFlip);
        }

        private static OrientationEntry[] BuildOrientations()
        {
            int[] id   = Enumerable.Range(0, FaceletCount).ToArray();
            // Elementary whole-cube rotations (same axis/angle convention as RotateCubie).
            int[] x90  = ComputeWholeRotPerm(RubiksCube.Axis.X, +90); // B→D
            int[] x90n = ComputeWholeRotPerm(RubiksCube.Axis.X, -90); // F→D
            int[] y90  = ComputeWholeRotPerm(RubiksCube.Axis.Y, +90);
            int[] z90  = ComputeWholeRotPerm(RubiksCube.Axis.Z, +90); // R→D
            int[] z90n = ComputeWholeRotPerm(RubiksCube.Axis.Z, -90); // L→D
            int[] x180 = ComposePerms(x90, x90);                      // U→D

            // 6 base orientations: each puts a different face at the bottom.
            int[][] bases = { id, x90n, x180, x90, z90, z90n }; // D, F, U, B, R, L on bottom

            int[] y180 = ComposePerms(y90, y90);
            int[] y270 = ComposePerms(y180, y90);
            int[][] yCycles = { id, y90, y180, y270 };

            var entries = new List<OrientationEntry>(24);
            foreach (var bp in bases)
                foreach (var yp in yCycles)
                {
                    int[] perm = ComposePerms(bp, yp);
                    int[] inv  = BuildInversePerm(perm);
                    var (oFace, dFlip) = ComputeMoveTranslation(perm, inv);
                    entries.Add(new OrientationEntry(perm, inv, oFace, dFlip));
                }
            return entries.ToArray();
        }

        private static int[] ComputeFacePerm(char face)
        {
            // Build a labeled cube where each sticker has a unique index ID.
            // Each cubie carries 6 chars/ints — but to simulate, we use a Cubie list
            // with positions and per-direction sticker IDs.
            var cubies = new List<SimCubie>();
            for (sbyte x = -1; x <= 1; x++)
                for (sbyte y = -1; y <= 1; y++)
                    for (sbyte z = -1; z <= 1; z++)
                    {
                        var c = new SimCubie { X = x, Y = y, Z = z };
                        // Assign sticker IDs equal to facelet indices for each exposed direction.
                        if (y == -1) c.Up    = WorldToFaceletIndex(RubiksCube.Face.Top, x, y, z);
                        if (y ==  1) c.Down  = WorldToFaceletIndex(RubiksCube.Face.Bottom, x, y, z);
                        if (x == -1) c.Left  = WorldToFaceletIndex(RubiksCube.Face.Left, x, y, z);
                        if (x ==  1) c.Right = WorldToFaceletIndex(RubiksCube.Face.Right, x, y, z);
                        if (z ==  1) c.Front = WorldToFaceletIndex(RubiksCube.Face.Front, x, y, z);
                        if (z == -1) c.Back  = WorldToFaceletIndex(RubiksCube.Face.Back, x, y, z);
                        cubies.Add(c);
                    }

            // Apply one CW quarter turn for the requested face.
            (RubiksCube.Layer layer, int targetAngle) = FaceCwParams(face);
            RubiksCube.Axis axis = LayerAxis(layer);
            foreach (var c in cubies)
                if (CubieInLayer(c, layer))
                    RotateCubie(c, axis, targetAngle);

            // Reconstruct facelet array (each cubie writes its currently exposed stickers).
            int[] post = new int[FaceletCount];
            for (int i = 0; i < FaceletCount; i++) post[i] = -1;
            foreach (var c in cubies)
            {
                if (c.Y == -1 && c.Up    >= 0) post[WorldToFaceletIndex(RubiksCube.Face.Top, c.X, c.Y, c.Z)]    = c.Up;
                if (c.Y ==  1 && c.Down  >= 0) post[WorldToFaceletIndex(RubiksCube.Face.Bottom, c.X, c.Y, c.Z)] = c.Down;
                if (c.X == -1 && c.Left  >= 0) post[WorldToFaceletIndex(RubiksCube.Face.Left, c.X, c.Y, c.Z)]   = c.Left;
                if (c.X ==  1 && c.Right >= 0) post[WorldToFaceletIndex(RubiksCube.Face.Right, c.X, c.Y, c.Z)]  = c.Right;
                if (c.Z ==  1 && c.Front >= 0) post[WorldToFaceletIndex(RubiksCube.Face.Front, c.X, c.Y, c.Z)]  = c.Front;
                if (c.Z == -1 && c.Back  >= 0) post[WorldToFaceletIndex(RubiksCube.Face.Back, c.X, c.Y, c.Z)]   = c.Back;
            }

            // post[i] is the original sticker ID now sitting at facelet index i.
            // Since IDs were chosen equal to the original facelet index, post[i] is the perm: new[i] = old[post[i]].
            return post;
        }

        private static (RubiksCube.Layer, int) FaceCwParams(char face) => face switch
        {
            // Pair (layer, target angle) matches Move.cs:32-53 for RotationType.Clockwise.
            'U' => (RubiksCube.Layer.Up, -90),
            'D' => (RubiksCube.Layer.Down, +90),
            'L' => (RubiksCube.Layer.Left, -90),
            'R' => (RubiksCube.Layer.Right, +90),
            'F' => (RubiksCube.Layer.Front, +90),
            'B' => (RubiksCube.Layer.Back, -90),
            _ => throw new ArgumentException($"Unknown face '{face}'"),
        };

        private static RubiksCube.Axis LayerAxis(RubiksCube.Layer layer) => layer switch
        {
            RubiksCube.Layer.Up or RubiksCube.Layer.Down or RubiksCube.Layer.MiddleY => RubiksCube.Axis.Y,
            RubiksCube.Layer.Left or RubiksCube.Layer.Right or RubiksCube.Layer.MiddleX => RubiksCube.Axis.X,
            _ => RubiksCube.Axis.Z,
        };

        private static bool CubieInLayer(SimCubie c, RubiksCube.Layer layer) => layer switch
        {
            RubiksCube.Layer.Up    => c.Y == -1,
            RubiksCube.Layer.Down  => c.Y ==  1,
            RubiksCube.Layer.Left  => c.X == -1,
            RubiksCube.Layer.Right => c.X ==  1,
            RubiksCube.Layer.Front => c.Z ==  1,
            RubiksCube.Layer.Back  => c.Z == -1,
            _ => false,
        };

        // Rotate the cubie's position and permute its sticker directions, matching the model's
        // RubiksCubeController.StartRotation math (lines 116-130, 157-240).
        private static void RotateCubie(SimCubie c, RubiksCube.Axis axis, int targetAngle)
        {
            int sin = targetAngle == 90 ? 1 : targetAngle == -90 ? -1 : 0;
            sbyte oldX = c.X, oldY = c.Y, oldZ = c.Z;
            int up = c.Up, down = c.Down, left = c.Left, right = c.Right, front = c.Front, back = c.Back;
            switch (axis)
            {
                case RubiksCube.Axis.X:
                    // newY = -oldZ * sin; newZ = oldY * sin
                    c.Y = (sbyte)(-oldZ * sin);
                    c.Z = (sbyte)( oldY * sin);
                    if (sin == +1)
                    {
                        // +90° about X: +Y → +Z → -Y → -Z → +Y
                        c.Front = down;
                        c.Up    = front;
                        c.Back  = up;
                        c.Down  = back;
                    }
                    else
                    {
                        // -90° about X: +Y → -Z → -Y → +Z → +Y
                        c.Back  = down;
                        c.Up    = back;
                        c.Front = up;
                        c.Down  = front;
                    }
                    break;
                case RubiksCube.Axis.Y:
                    // newX = oldZ * sin; newZ = -oldX * sin
                    c.X = (sbyte)( oldZ * sin);
                    c.Z = (sbyte)(-oldX * sin);
                    if (sin == +1)
                    {
                        // +90° about Y: +Z → +X → -Z → -X → +Z
                        c.Right = front;
                        c.Back  = right;
                        c.Left  = back;
                        c.Front = left;
                    }
                    else
                    {
                        // -90° about Y: +Z → -X → -Z → +X → +Z
                        c.Left  = front;
                        c.Back  = left;
                        c.Right = back;
                        c.Front = right;
                    }
                    break;
                case RubiksCube.Axis.Z:
                    // newX = -oldY * sin; newY = oldX * sin
                    c.X = (sbyte)(-oldY * sin);
                    c.Y = (sbyte)( oldX * sin);
                    if (sin == +1)
                    {
                        // +90° about Z (F-CW): Top → Right → Down → Left → Top
                        c.Right = up;
                        c.Down  = right;
                        c.Left  = down;
                        c.Up    = left;
                    }
                    else
                    {
                        // -90° about Z (B-CW): Top → Left → Down → Right → Top
                        c.Left  = up;
                        c.Down  = left;
                        c.Right = down;
                        c.Up    = right;
                    }
                    break;
            }
        }

        // -1 means "no sticker on this side".
        private class SimCubie
        {
            public sbyte X, Y, Z;
            public int Up = -1, Down = -1, Left = -1, Right = -1, Front = -1, Back = -1;
        }

        // ---------- Sticker accessors used by solver phases ----------

        public char U(int i) => Facelets[U0 + i];
        public char R(int i) => Facelets[R0 + i];
        public char F(int i) => Facelets[F0 + i];
        public char D(int i) => Facelets[D0 + i];
        public char L(int i) => Facelets[L0 + i];
        public char B(int i) => Facelets[B0 + i];

        public char this[int i] => Facelets[i];

        public string ToDisplayString()
        {
            // Compact unfolded layout for debugging.
            string s(int i) => Facelets[i].ToString();
            string row1 = $"      {s(0)}{s(1)}{s(2)}";
            string row2 = $"      {s(3)}{s(4)}{s(5)}";
            string row3 = $"      {s(6)}{s(7)}{s(8)}";
            string row4 = $"{s(36)}{s(37)}{s(38)} {s(18)}{s(19)}{s(20)} {s(9)}{s(10)}{s(11)} {s(45)}{s(46)}{s(47)}";
            string row5 = $"{s(39)}{s(40)}{s(41)} {s(21)}{s(22)}{s(23)} {s(12)}{s(13)}{s(14)} {s(48)}{s(49)}{s(50)}";
            string row6 = $"{s(42)}{s(43)}{s(44)} {s(24)}{s(25)}{s(26)} {s(15)}{s(16)}{s(17)} {s(51)}{s(52)}{s(53)}";
            string row7 = $"      {s(27)}{s(28)}{s(29)}";
            string row8 = $"      {s(30)}{s(31)}{s(32)}";
            string row9 = $"      {s(33)}{s(34)}{s(35)}";
            return string.Join("\n", new[] { row1, row2, row3, row4, row5, row6, row7, row8, row9 });
        }
    }

    // Describes one of the 24 whole-cube orientations used by the solver retry loop.
    public readonly struct OrientationEntry
    {
        public readonly int[]  Perm;      // 54-element sticker permutation
        public readonly int[]  InvPerm;   // inverse permutation (to undo the rotation)
        public readonly char[] OrigFace;  // OrigFace[fi] = original face letter for solver face index fi
        public readonly int[]  DirFlip;   // ±1; multiply solver quarter by this to get original quarter

        public OrientationEntry(int[] perm, int[] invPerm, char[] origFace, int[] dirFlip)
        {
            Perm = perm; InvPerm = invPerm; OrigFace = origFace; DirFlip = dirFlip;
        }
    }
}

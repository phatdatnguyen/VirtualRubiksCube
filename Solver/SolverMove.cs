namespace VirtualRubiksCube.Solver
{
    // Face letter is U/D/L/R/F/B (Singmaster). Quarter is +1 (clockwise looking at the face from outside),
    // -1 (counterclockwise), or 2 (180°). The mapping from face letter to RubiksCube.Layer + Move.RotationType
    // is verified empirically by SolverConvention; if a face letter rotates the wrong way visually, only the
    // mapping in this class needs to flip.
    public readonly struct SolverMove
    {
        public char Face { get; }
        public int Quarter { get; }

        public SolverMove(char face, int quarter)
        {
            Face = face;
            Quarter = quarter;
        }

        public static SolverMove Parse(string token)
        {
            char f = token[0];
            int q = 1;
            if (token.Length == 2)
                q = token[1] == '\'' ? -1 : 2;
            return new SolverMove(f, q);
        }

        public static List<SolverMove> ParseSequence(string sequence)
        {
            List<SolverMove> result = new();
            foreach (string token in sequence.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                result.Add(Parse(token));
            return result;
        }

        public SolverMove Inverse()
        {
            int q = Quarter == 2 ? 2 : -Quarter;
            return new SolverMove(Face, q);
        }

        public IEnumerable<Move> ToControllerMoves()
        {
            (RubiksCube.Layer layer, bool flipDirection) = FaceToLayer(Face);
            int times = Quarter == 2 ? 2 : 1;
            Move.RotationType type;
            if (Quarter == -1)
                type = flipDirection ? Move.RotationType.Clockwise : Move.RotationType.Counterclockwise;
            else
                type = flipDirection ? Move.RotationType.Counterclockwise : Move.RotationType.Clockwise;

            for (int i = 0; i < times; i++)
                yield return new Move(layer, type);
        }

        // Returns (layer, flipDirection). When flipDirection is true, what Singmaster calls "clockwise"
        // maps to RubiksCube.RotationType.Counterclockwise. SolverConvention.Verify() chooses the flag.
        private static (RubiksCube.Layer, bool) FaceToLayer(char face)
        {
            return face switch
            {
                'U' => (RubiksCube.Layer.Up, SolverConvention.FlipU),
                'D' => (RubiksCube.Layer.Down, SolverConvention.FlipD),
                'L' => (RubiksCube.Layer.Left, SolverConvention.FlipL),
                'R' => (RubiksCube.Layer.Right, SolverConvention.FlipR),
                'F' => (RubiksCube.Layer.Front, SolverConvention.FlipF),
                'B' => (RubiksCube.Layer.Back, SolverConvention.FlipB),
                _ => throw new ArgumentException($"Unknown face '{face}'"),
            };
        }

        public override string ToString()
        {
            string suffix = Quarter == 1 ? "" : Quarter == -1 ? "'" : "2";
            return Face + suffix;
        }
    }

    // Per-face direction-flip flags, set once at startup by comparing the model's Move semantics
    // against the solver's facelet permutations on a solved cube. See SolverConvention.cs.
    public static class SolverConvention
    {
        public static bool FlipU = false;
        public static bool FlipD = false;
        public static bool FlipL = false;
        public static bool FlipR = false;
        public static bool FlipF = false;
        public static bool FlipB = false;
    }
}

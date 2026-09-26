namespace VirtualRubiksCube
{
    public class RubiksCubeState
    {
        #region Property
        public Dictionary<Cubelet, (sbyte, sbyte, sbyte)> CubeletsPosition { get; private set; }
        public Dictionary<Cubelet, RubiksCube.Face[]> CubeletsFaces { get; private set; }
        #endregion

        #region Contructor
        public RubiksCubeState(Dictionary<Cubelet, (sbyte, sbyte, sbyte)> cubeletsPosition)
        {
            CubeletsPosition = new(cubeletsPosition);
            CubeletsFaces = cubeletsPosition.Keys.ToDictionary(
                cubelet => cubelet,
                cubelet => Enumerable.Range(0, 6).Select(index => cubelet.CurrentFaces[(byte)index]).ToArray());
        }
        #endregion

        #region Method
        public RubiksCubeState Clone()
        {
            var clone = (RubiksCubeState)MemberwiseClone();
            clone.CubeletsPosition = new(CubeletsPosition);
            clone.CubeletsFaces = CubeletsFaces.ToDictionary(
                entry => entry.Key, entry => (RubiksCube.Face[])entry.Value.Clone());
            return clone;
        }
        #endregion
    }
}

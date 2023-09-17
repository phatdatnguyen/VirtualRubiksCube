namespace VirtualRubiksCube
{
    public class RubiksCubeState
    {
        #region Property
        public Dictionary<Cubelet, (sbyte, sbyte, sbyte)> CubeletsPosition { get; private set; }
        #endregion

        #region Contructor
        public RubiksCubeState(Dictionary<Cubelet, (sbyte, sbyte, sbyte)> cubeletsPosition)
        {
            CubeletsPosition = cubeletsPosition;
        }
        #endregion

        #region Method
        public RubiksCubeState Clone()
        {
            Dictionary<Cubelet, (sbyte, sbyte, sbyte)> cubeletsPosition = new();
            foreach (KeyValuePair<Cubelet, (sbyte, sbyte, sbyte)> cubeletPosition in CubeletsPosition)
            {
                (sbyte x, sbyte y, sbyte z) = cubeletPosition.Value;
                cubeletsPosition.Add(cubeletPosition.Key, (x, y, z));
            }
               
            return new RubiksCubeState(cubeletsPosition);
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualRubiksCube
{
    public class RubiksCubeState
    {
        // Property
        public Dictionary<Cubelet, Tuple<sbyte, sbyte, sbyte>> CubeletsPosition { get; private set; }

        // Contructor
        public RubiksCubeState(Dictionary<Cubelet, Tuple<sbyte, sbyte, sbyte>> cubeletsPosition)
        {
            CubeletsPosition = cubeletsPosition;
        }

        // Method
        public RubiksCubeState Clone()
        {
            Dictionary<Cubelet, Tuple<sbyte, sbyte, sbyte>> cubeletsPosition = new Dictionary<Cubelet, Tuple<sbyte, sbyte, sbyte>>();
            foreach (KeyValuePair<Cubelet, Tuple<sbyte, sbyte, sbyte>> cubeletPosition in CubeletsPosition)
            {
                Tuple<sbyte, sbyte, sbyte> position = cubeletPosition.Value;
                Tuple<sbyte, sbyte, sbyte> clonedPosition = new Tuple<sbyte, sbyte, sbyte>(position.Item1, position.Item2, position.Item3);
                cubeletsPosition.Add(cubeletPosition.Key, clonedPosition);
            }
               
            return new RubiksCubeState(cubeletsPosition);
        }
    }
}

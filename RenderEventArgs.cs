namespace VirtualRubiksCube
{
    public class RenderEventArgs : EventArgs
    {
        #region Properties
        public RenderInfo RenderInfo { get; private set; }
        public RenderEventArgs(RenderInfo renderInfo)
        {
            RenderInfo = renderInfo;
        }
        #endregion
    }
}

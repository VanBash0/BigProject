using UnityEngine;

namespace BigProject.Systems.QuestSystem
{
    /// <summary>
    /// Controller for set up quest objects in init, start and end points.
    /// </summary>
    public interface IQuestBoundariesController
    {
        public int QuestId {  get; }
        /// <summary>
        /// Initiallization on scene entry point.
        /// </summary>
        public void InitOnSceneEntry() { }

        /// <summary>
        /// Deinitialization on scene entry point.
        /// </summary>
        public void DeinitOnSceneEntry() { }

        /// <summary>
        /// Invoke when quest change state to active.
        /// </summary>
        public void Begin() { }

        /// <summary>
        /// Invoke when quest change state to completed/failed.
        /// </summary>
        public void End() { }
    }
}
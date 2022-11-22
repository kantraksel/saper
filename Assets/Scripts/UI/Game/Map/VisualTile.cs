using TheGame.GameModes.Saper;
using UnityEngine;
using UnityEngine.UI;

namespace TheGame.UI.Game
{
    public class VisualTile : MonoBehaviour
    {
        [Header("Setup")]
        //icon to set up tile
        [SerializeField] private Sprite bombIcon;
        [SerializeField] private Sprite flagIcon;
        [SerializeField] private Sprite guessIcon;

        //renderer of this objects
        [SerializeField] private Image baseLayer;
        [SerializeField] private Image holeLayer;
        [SerializeField] private Image iconLayer;
        [SerializeField] private Text neighbours;

        //color array
        [SerializeField] private Color[] numberColors = new Color[8];

        //state value container
        public Vector2Int Position { get; set; }
        public TileState State { get; private set; }
        public int Value { get; private set; }

        /// <summary>
        /// Initialize this tile
        /// </summary>
        public void Initialize()
        {
            SetState(TileState.Hidden);
        }

        /// <summary>
        /// Sets state value of tile.
        /// </summary>
        /// <param name="stateSet">State value</param>
        public void SetState(TileState stateSet)
        {
            //sets state of this tile
            State = stateSet;

            Color color;
            if (stateSet == TileState.Bombed)
                color = Color.black;
            else
                color = Color.white;

            baseLayer.color = color;
            holeLayer.color = color;

            //sets visuals of this tile
            switch (stateSet)
            {
                case TileState.Hidden:
                    //tile without icon
                    holeLayer.enabled = false;
                    iconLayer.enabled = false;
                    iconLayer.sprite = null;
                    neighbours.enabled = false;
                    break;
                case TileState.Flagged:
                    //tile with flag icon
                    holeLayer.enabled = false;
                    iconLayer.enabled = true;
                    iconLayer.sprite = flagIcon;
                    neighbours.enabled = false;
                    break;
                case TileState.QuestionMarked:
                    //tile with question icon
                    holeLayer.enabled = false;
                    iconLayer.enabled = true;
                    iconLayer.sprite = guessIcon;
                    neighbours.enabled = false;
                    break;

                case TileState.Shown:
                    //clicked without icon
                    holeLayer.enabled = true;
                    iconLayer.enabled = false;
                    iconLayer.sprite = null;
                    neighbours.enabled = true;
                    break;
                case TileState.Bombed:
                    //clicked with bomb icon
                    holeLayer.enabled = true;
                    iconLayer.enabled = true;
                    iconLayer.sprite = bombIcon;
                    neighbours.enabled = false;
                    break;
            }
        }

        /// <summary>
        /// Sets visual number filtered by tile status.
        /// </summary>
        /// <param name="value">Field value</param>
        public void SetFieldValue(int value)
        {
            Value = value;

            if (value == 0)
            {
                neighbours.text = "";
                return;
            }

            var color = Mathf.Clamp(value, 1, numberColors.Length);
            neighbours.color = numberColors[color - 1];
            neighbours.text = value.ToString();
        }
    }
}

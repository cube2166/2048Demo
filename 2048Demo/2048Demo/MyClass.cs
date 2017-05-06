using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2048Demo
{
    public enum KeyArrow
    {
        UP,
        DOWN,
        LEFT,
        RIGHT,
    }

    public class MyObject
    {
        #region Property
        public float X { get; set; }
        public float Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public object Texture { get; set; }
        public Color thisClolor { get; set; }
        #endregion

        #region Event
        #endregion

        #region Create

        #endregion

        #region Method

        public virtual void Update(float elapsedTime)
        {

        }

        public virtual void Show(SpriteBatch spriteBatch)
        {

        }

        #endregion
    }

    public class Board : MyObject
    {
        #region Property
        private bool prepareAdd { get; set; }
        #endregion
        #region Private
        private int _MaxCol, _MaxRow;
        private PieceList _PieceList;
        private int[,,] _PieceInfo;
        private int _PieceWidth;
        private int _PieceHeight;
        private int _PieceInterval;
        private List<Texture2D> _TextureList;
        private Rectangle _bg1, _bg2;
        private Random rand;
        #endregion
        #region Create

        public Board(List<Texture2D>List, int size)
        {
            _PieceList = new PieceList();
            rand = new Random(DateTime.Now.Second);
            _MaxCol = _MaxRow = size;
            _TextureList = List;
            Height = Game1.window_Height;
            Width = Height;
            X = (Game1.window_Width - Game1.window_Height) / 2;
            Y = 0;
            int tempVal = Width/40;
            _bg1 = new Rectangle((int)X, (int)Y, Width, Height);
            _bg2 = new Rectangle((int)X + tempVal, (int)Y + tempVal, Width - (tempVal * 2), Height - (tempVal * 2));
            prepareAdd = false;
            _PieceInfo = new int[_MaxCol,_MaxRow,3];

            _PieceInterval = (_bg2.Width / _MaxCol) / 20;
            _PieceWidth = (_bg2.Width - _PieceInterval * 2) / _MaxCol;
            _PieceHeight = (_bg2.Height - _PieceInterval * 2) / _MaxRow;

            for (int col = 0; col < _MaxCol; col++)
            {
                for (int row = 0; row < _MaxRow; row++)
                {
                    _PieceInfo[col, row, 0] = (_bg2.X + _PieceWidth * col) + _PieceInterval * 2;
                    _PieceInfo[col, row, 1] = (_bg2.Y + _PieceHeight * row) + _PieceInterval * 2;
                    _PieceInfo[col, row, 2] = -1;
                    _PieceList.Add(new Piece(_TextureList, _PieceWidth, _PieceHeight, _PieceInterval));
                }
            }
            InitPiece();
        }

        #endregion
        #region Method
        public override void Update(float elapsedTime)
        {
            foreach (var item in _PieceList)
            {
                item.Update(elapsedTime);
            }
            if(prepareAdd == true)
            {
                prepareAdd = false;
                foreach (var item in _PieceList)
                {
                    if (item.canMove == false)
                    {
                        prepareAdd = true;
                        break;
                    }
                }
                if(prepareAdd == false)
                {
                    addPiece();
                }
            }
        }

        public void Move(KeyArrow ka)
        {
            foreach (var item in _PieceList)
            {
                if (item.canMove == false) return;
            }
            switch (ka)
            {
                case KeyArrow.UP:
                    for (int col = 0; col < _MaxCol; col++)
                    {
                        for (int row = 0; row < _MaxRow;)
                        {
                            if (_PieceInfo[col,row,2] == -1)
                            {
                                for (int insRow = row+1; insRow < _MaxRow; insRow++)
                                {
                                    if (_PieceInfo[col, insRow, 2] != -1)
                                    {
                                        _PieceInfo[col, row, 2] = _PieceInfo[col, insRow, 2];
                                        _PieceInfo[col, insRow, 2] = -1;
                                        _PieceList[_PieceInfo[col, row, 2]].distPosition = new Vector2(_PieceInfo[col, row, 0], _PieceInfo[col, row, 1]);
                                        prepareAdd = true;
                                        break;
                                    }
                                    if((insRow+1) == _MaxRow)
                                    {
                                        row = _MaxRow;
                                    }
                                }
                            }
                            else
                            {
                                for (int insRow = row+1; insRow < _MaxRow; insRow++)
                                {
                                    if (_PieceInfo[col, insRow, 2] != -1)
                                    {
                                        if (_PieceList[_PieceInfo[col, row, 2]].Value == _PieceList[_PieceInfo[col, insRow, 2]].Value)
                                        {
                                            _PieceList[_PieceInfo[col, row, 2]].Value *= 2;
                                            _PieceList[_PieceInfo[col, insRow, 2]].Value *= 2;
                                            _PieceList[_PieceInfo[col, insRow, 2]].distPosition = new Vector2(_PieceInfo[col, row, 0], _PieceInfo[col, row, 1]);
                                            _PieceList[_PieceInfo[col, insRow, 2]].preparHide = true;
                                            _PieceInfo[col, insRow, 2] = -1;
                                            prepareAdd = true;
                                        }
                                        break;
                                    }
                                }
                                row++;
                            }
                            if ((row + 1) == _MaxRow)
                            {
                                row++;
                            }
                        }
                    }
                    break;
                case KeyArrow.DOWN:
                    for (int col = 0; col < _MaxCol; col++)
                    {
                        for (int row = _MaxRow-1; row >= 0;)
                        {
                            if (_PieceInfo[col,row,2] == -1)
                            {
                                for (int insRow = row-1; insRow >=0; insRow--)
                                {
                                    if (_PieceInfo[col, insRow, 2] != -1)
                                    {
                                        _PieceInfo[col, row, 2] = _PieceInfo[col, insRow, 2];
                                        _PieceInfo[col, insRow, 2] = -1;
                                        _PieceList[_PieceInfo[col, row, 2]].distPosition = new Vector2(_PieceInfo[col, row, 0], _PieceInfo[col, row, 1]);
                                        prepareAdd = true;
                                        break;
                                    }
                                    if((insRow-1) < 0)
                                    {
                                        row = -1;
                                    }
                                }
                            }
                            else
                            {
                                for (int insRow = row - 1; insRow >= 0; insRow--)
                                {
                                    if (_PieceInfo[col, insRow, 2] != -1)
                                    {
                                        if (_PieceList[_PieceInfo[col, row, 2]].Value == _PieceList[_PieceInfo[col, insRow, 2]].Value)
                                        {
                                            _PieceList[_PieceInfo[col, row, 2]].Value *= 2;
                                            _PieceList[_PieceInfo[col, insRow, 2]].Value *= 2;
                                            _PieceList[_PieceInfo[col, insRow, 2]].distPosition = new Vector2(_PieceInfo[col, row, 0], _PieceInfo[col, row, 1]);
                                            _PieceList[_PieceInfo[col, insRow, 2]].preparHide = true;
                                            _PieceInfo[col, insRow, 2] = -1;
                                            prepareAdd = true;
                                        }
                                        break;
                                    }
                                }
                                row--;
                            }
                            if ((row - 1) < 0)
                            {
                                row--;
                            }
                        }
                    }
                    break;
                case KeyArrow.LEFT:
                    for (int row = 0; row < _MaxRow; row++)
                    {
                        for (int col = 0; col < _MaxCol;)
                        {
                            if (_PieceInfo[col, row, 2] == -1)
                            {
                                for (int insCol = col + 1; insCol < _MaxCol; insCol++)
                                {
                                    if (_PieceInfo[insCol, row, 2] != -1)
                                    {
                                        _PieceInfo[col, row, 2] = _PieceInfo[insCol, row, 2];
                                        _PieceInfo[insCol, row, 2] = -1;
                                        _PieceList[_PieceInfo[col, row, 2]].distPosition = new Vector2(_PieceInfo[col, row, 0], _PieceInfo[col, row, 1]);
                                        prepareAdd = true;
                                        break;
                                    }
                                    if ((insCol + 1) == _MaxCol)
                                    {
                                        col = _MaxCol;
                                    }
                                }
                            }
                            else
                            {
                                for (int insCol = col + 1; insCol < _MaxCol; insCol++)
                                {
                                    if (_PieceInfo[insCol, row, 2] != -1)
                                    {
                                        if (_PieceList[_PieceInfo[col, row, 2]].Value == _PieceList[_PieceInfo[insCol, row, 2]].Value)
                                        {
                                            _PieceList[_PieceInfo[col, row, 2]].Value *= 2;
                                            _PieceList[_PieceInfo[insCol, row, 2]].Value *= 2;
                                            _PieceList[_PieceInfo[insCol, row, 2]].distPosition = new Vector2(_PieceInfo[col, row, 0], _PieceInfo[col, row, 1]);
                                            _PieceList[_PieceInfo[insCol, row, 2]].preparHide = true;
                                            _PieceInfo[insCol, row, 2] = -1;
                                            prepareAdd = true;
                                        }
                                        break;
                                    }
                                }
                                col++;
                            }
                            if ((col + 1) == _MaxCol)
                            {
                                col++;
                            }
                        }
                    }
                    break;
                case KeyArrow.RIGHT:
                    for (int row = 0; row < _MaxRow; row++)
                    {
                        for (int col = _MaxCol-1; col >= 0;)
                        {
                            if (_PieceInfo[col, row, 2] == -1)
                            {
                                for (int insCol = col - 1; insCol >= 0; insCol--)
                                {
                                    if (_PieceInfo[insCol, row, 2] != -1)
                                    {
                                        _PieceInfo[col, row, 2] = _PieceInfo[insCol, row, 2];
                                        _PieceInfo[insCol, row, 2] = -1;
                                        _PieceList[_PieceInfo[col, row, 2]].distPosition = new Vector2(_PieceInfo[col, row, 0], _PieceInfo[col, row, 1]);
                                        prepareAdd = true;
                                        break;
                                    }
                                    if ((insCol - 1) < 0)
                                    {
                                        col = -1;
                                    }
                                }
                            }
                            else
                            {
                                for (int insCol = col - 1; insCol >= 0; insCol--)
                                {
                                    if (_PieceInfo[insCol, row, 2] != -1)
                                    {
                                        if (_PieceList[_PieceInfo[col, row, 2]].Value == _PieceList[_PieceInfo[insCol, row, 2]].Value)
                                        {
                                            _PieceList[_PieceInfo[col, row, 2]].Value *= 2;
                                            _PieceList[_PieceInfo[insCol, row, 2]].Value *= 2;
                                            _PieceList[_PieceInfo[insCol, row, 2]].distPosition = new Vector2(_PieceInfo[col, row, 0], _PieceInfo[col, row, 1]);
                                            _PieceList[_PieceInfo[insCol, row, 2]].preparHide = true;
                                            _PieceInfo[insCol, row, 2] = -1;
                                            prepareAdd = true;
                                        }
                                        break;
                                    }
                                }
                                col--;
                            }
                            if ((col - 1) < 0)
                            {
                                col--;
                            }
                        }
                    }
                    break;
                default:
                    break;
            }           
        }

        public override void Show(SpriteBatch spriteBatch)
        {
            showBackground(spriteBatch);
            foreach (var item in _PieceList)
            {
                item.Show(spriteBatch);
            }
        }

        public void InitPiece()
        {
            foreach (var item in _PieceList)
            {
                item.Value = 0;
            }
            for (int col = 0; col < _MaxCol; col++)
            {
                for (int row = 0; row < _MaxRow; row++)
                {
                    _PieceInfo[col, row, 2] = -1;
                }
            }
            for (int ii = 0; ii < 2; ii++)
            {
                addPiece();
            }
        }

        private void addPiece()
        {
            int count = 0;
            List<Vector2> tempList = new List<Vector2>();
            for (int col = 0; col < _MaxCol; col++)
            {
                for (int row = 0; row < _MaxRow; row++)
                {
                    if (_PieceInfo[col, row, 2] == -1)
                    {
                        count++;
                        Vector2 temp = new Vector2(col, row);
                        tempList.Add(temp);
                    }
                }
            }
            if (count < 1) return;
            int randTemp = rand.Next(0, tempList.Count - 1);
            int randCol = (int)tempList[randTemp].X;
            int randRow = (int)tempList[randTemp].Y;
            while (true)
            {
                if (_PieceInfo[randCol, randRow, 2] == -1)
                {
                    foreach (var item in _PieceList)
                    {
                        if (item.Value == 0)
                        {
                            _PieceInfo[randCol, randRow, 2] = _PieceList.IndexOf(item);
                            item.distPosition = new Vector2(_PieceInfo[randCol, randRow, 0], _PieceInfo[randCol, randRow, 1]);
                            item.srcPosition = new Vector2(_PieceInfo[randCol, randRow, 0], _PieceInfo[randCol, randRow, 1]);
                            item.X = item.srcPosition.X;
                            item.Y = item.srcPosition.Y;
                            item.Value = 2;
                            break;
                        }
                    }
                    break;
                }
                else
                {
                    randTemp = rand.Next(0, tempList.Count - 1);
                    randCol = (int)tempList[randTemp].X;
                    randRow = (int)tempList[randTemp].Y;

                }
            }
        }

        private void showBackground(SpriteBatch spriteBatch)
        {
            Texture2D bg1 = searchTexture("bg1", _TextureList);
            Texture2D bg2 = searchTexture("bg2", _TextureList);
            Texture2D block_0 = searchTexture("block_0", _TextureList);
            if (bg1 == null || bg2 == null || block_0 == null) return;
            spriteBatch.Draw(bg1, _bg1, Color.White);
            spriteBatch.Draw(bg2, _bg2, Color.White);

            for (int col = 0; col < _MaxCol; col++)
            {
                for (int row = 0; row < _MaxRow; row++)
                {
                    spriteBatch.Draw(block_0, new Rectangle(_PieceInfo[col, row, 0], _PieceInfo[col, row, 1],
                                                            _PieceWidth -(_PieceInterval * 2) , _PieceHeight - (_PieceInterval * 2))
                                                            , Color.White);
                }
            }
        }

        private Texture2D searchTexture(string name, List<Texture2D> List)
        {
            foreach (var item in List)
            {
                if (String.Equals(item.Name, name)) return item;
            }
            return null;
        }

        #endregion
        #region Event

        #endregion
        #region Claee
        private class Piece : MyObject
        {
            #region Property
            public bool canMove
            {
                get { return _canMove; }
                set
                {
                    if(_canMove != value)
                    _canMove = value;
                }
            }

            public bool canShow
            {
                get { return _canShow; }
                set
                {
                    if (_canShow != value)
                        _canShow = value;
                }
            }
            public int Value
            {
                get { return _value; }
                set
                {
                    if (_value != value)
                    {
                        _value = value;
                        string temp = string.Format("block_{0}", _value);
                        Texture = searchTexture(temp, _TextureList);
                        if (_value == 0) canShow = false;
                        else canShow = true;
                    }
                }
            }
            public Vector2 distPosition
            {
                get { return _distPosition; }
                set
                {
                    if (_distPosition != value && canMove == true)
                    {
                        _distPosition = value;
                        canMove = false;
                    }
                }
            }
            public Vector2 srcPosition
            {
                get { return _srcPosition; }
                set
                {
                    if (_srcPosition != value)
                    {
                        _srcPosition = value;
                        if (_srcPosition == _distPosition)
                        {
                                canMove = true;
                            if(preparHide == true)
                            {
                                preparHide = false;
                                Value = 0;
                            }
                        }

                    }
                }
            }
            public bool preparHide
            {
                get { return _preparHide; }
                set
                {
                    if(_preparHide != value)
                    {
                        _preparHide = value;
                    }
                }
            }
            #endregion
            #region Private
            private bool _preparHide;
            private bool _canMove;
            private bool _canShow;
            private int _value;
            private List<Texture2D> _TextureList;
            private Vector2 _distPosition;
            private Vector2 _srcPosition;
            private int _PieceWidth;
            private int _PieceHeight;
            private int _PieceInterval;
            #endregion
            #region Method
            private Texture2D searchTexture(string name, List<Texture2D> List)
            {
                foreach (var item in List)
                {
                    if (String.Equals(item.Name, name)) return item;
                }
                return null;
            }
            public override void Update(float elapsedTime)
            {
                double len = Math.Sqrt(Math.Pow((distPosition.X - srcPosition.X),2) + Math.Pow((distPosition.Y - srcPosition.Y), 2));
                X += (distPosition.X - srcPosition.X)  * elapsedTime * 10;
                Y += (distPosition.Y - srcPosition.Y)  * elapsedTime * 10;
                srcPosition = new Vector2(X, Y);
                if (len < 1) srcPosition = distPosition;
            }
            public override void Show(SpriteBatch spriteBatch)
            {
                if(canShow == true)
                {
                    int tempX = (int)X;
                    int tempY = (int)Y;
                    Rectangle temp = new Rectangle(tempX, tempY, _PieceWidth - (_PieceInterval * 2), _PieceHeight - (_PieceInterval * 2));
                    spriteBatch.Draw((Texture2D)Texture, temp, Color.White);
                }
            }
            #endregion
            #region Create
            public Piece(List<Texture2D> List, int pw, int ph, int pi)
            {
                _TextureList = List;
                canMove = true;
                preparHide = false;

                _PieceWidth = pw;
                _PieceHeight = ph;
                _PieceInterval = pi;
            }
            #endregion
            #region Event

            #endregion

        }
        private class PieceList : ObservableCollection<Piece>
        {

        }
        #endregion
    }
}

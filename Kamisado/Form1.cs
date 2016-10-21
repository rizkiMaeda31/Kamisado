using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kamisado
{
    public partial class Form1 : Form
    {
        int[,] indexBoard = {{1,2,3,4,5,6,7,8},
                             {6,1,4,7,2,5,8,3},
                             {7,4,1,6,3,8,5,2},
                             {4,3,2,1,8,7,6,5},
                             {5,6,7,8,1,2,3,4},
                             {2,5,8,3,6,1,4,7},
                             {3,8,5,2,7,4,1,6},
                             {8,7,6,5,4,3,2,1}
                            };
        public Enemy[] enemy;
        public Player[] player;
        public Form1()
        {
            InitializeComponent();
            enemy = new Enemy[8];
            player = new Player[8];
            for (int i = 0; i < 8; i++)
            {
                enemy[i] = new Enemy(i, 0, indexBoard[i, 0]);
                player[i] = new Player(i, 7, indexBoard[i, 7]);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    g.DrawRectangle(new Pen(Color.Black), j * 40, i * 40, 40, 40);
                }
            }
        }
    }
    public abstract class Pawn
    {
        public int x;
        public int y;
        public int indexPawn;
        public int levelSumo;
        public int countPushedTower;
        public int maxMove;
        public int point;
        public Pawn previous;
        public Pawn next;
        public Pawn parent;
    }
    public interface Ability
    {
        bool Promotion();//promosi ke sumo
        bool RecoverPawn(int x);//jika round selesai maka pawn akan kembali seperti semula
        bool SumoMove(Pawn[] enemy);//check bisa melakukan sumo atau tidak
        bool Move(int toX, int toY, int[,] board, Pawn[] enemy);
        bool PossibleMove(int indexBoard);
    }
    public class Player : Pawn, Ability
    {
        public Player()
        {
            this.x = 0;
            this.y = 0;
            this.indexPawn = 0;
            this.levelSumo = 0;
            this.countPushedTower = 0;
            this.maxMove = 7;
            this.point = 0;
            this.previous = null;
            this.next = null;
            this.parent = null;
        }
        public Player(int x, int y, int indexPawn)
        {
            this.x = x;
            this.y = y;
            this.indexPawn = indexPawn;
            this.levelSumo = 0;
            this.countPushedTower = 0;
            this.maxMove = 7;
            this.point = 0;
            this.previous = null;
            this.next = null;
            this.parent = null;
        }
        public bool Promotion()
        {
            if (this.y == 0 && this.levelSumo <= 2)
            {
                this.levelSumo++;
                switch (this.levelSumo)
                {
                    case 1:
                        this.countPushedTower = 1;
                        this.maxMove = 5;
                        this.point = 1;
                        return true;
                    case 2:
                        this.countPushedTower = 2;
                        this.maxMove = 3;
                        this.point = 3;
                        return true;
                    case 3:
                        this.countPushedTower = 3;
                        this.maxMove = 1;
                        this.point = 7;
                        return true;
                    case 4:
                        this.point = 15;
                        return true;
                    default:
                        return false;
                }
            }
            return false;
        }
        public bool RecoverPawn(int x)
        {
            this.x = x;
            this.y = 7;
            return true;
        }
        public bool SumoMove(Pawn[] enemy)
        {
            Queue<int> indexEnemy = new Queue<int>();
            switch (this.levelSumo)
            {
                case 1:
                    if (this.y >= 2)
                    {
                        return false;
                    }
                    break;
                case 2:
                    if (this.y >= 3)
                    {
                        return false;
                    }
                    break;
                case 3:
                    if (this.y >= 4)
                    {
                        return false;
                    }
                    break;
                default:
                    return false;
            }
            for (int i = 0, pointer = 1; i < 8; i++)
            {
                if (enemy[i].x == this.x && enemy[i].y == this.y - pointer)
                {
                    indexEnemy.Enqueue(i);
                    pointer++;
                }
            }
            if (indexEnemy.Count > 0 && indexEnemy.Count == this.countPushedTower)
            {
                for (int j = 0; j < indexEnemy.Count; j++)
                {
                    enemy[indexEnemy.Dequeue()].y--;
                }
                this.y--;
                return true;
            }
            return false;
        }
        public bool Move(int toX, int toY, int[,] board, Pawn[] enemy)
        {
            if (toX < 0 || toX > 7 || toY < 0 || toY > 7)
            {
                return false;
            }
            if (board[toX,toY] == this.indexPawn)
            {
                for (int i = 0; i < 8; i++)
                {
                    if (toX == enemy[i].x && toY == enemy[i].y)
                    {
                        return false;
                    }
                }
                this.x = x;
                this.y = y;
                return true;
            }
            return false;
        }
        public bool PossibleMove(int indexBoard)
        {
            if (indexBoard == this.indexPawn)
            {
                int count = this.maxMove;
                //diagonal right
                for (int i = this.y, j = this.x; i < 8 && j < 8 && i >= 0 && j >= 0 && count != 0; i--, j++)
                {
                    count--;
                }
                //diagonal left
                count = this.maxMove;
                for (int i = this.y, j = this.x; i < 8 && j < 8 && i >= 0 && j >= 0 && count != 0; i--, j--)
                {
                    count--;
                } count = this.maxMove;
                //straight
                for (int i = this.y, j = this.x; i < 8 && i >= 0 && count != 0; i--)
                {
                    count--;
                }
            }
            return false;
        }
    }
    public class Enemy : Pawn, Ability
    { 
        public Enemy()
        {
            this.x = 0;
            this.y = 0;
            this.indexPawn = 0;
            this.levelSumo = 0;
            this.countPushedTower = 0;
            this.maxMove = 7;
            this.point = 0;
            this.previous = null;
            this.next = null;
            this.parent = null;
            this.diagonalLeft = new Queue<string>();
            this.diagonalRight = new Queue<string>();
            this.straight = new Queue<string>();
        }
        public Enemy(int x, int y, int indexPawn)
        {
            this.x = x;
            this.y = y;
            this.indexPawn = indexPawn;
            this.levelSumo = 0;
            this.countPushedTower = 0;
            this.maxMove = 7;
            this.point = 0;
            this.previous = null;
            this.next = null;
            this.parent = null;
            this.diagonalLeft = new Queue<string>();
            this.diagonalRight = new Queue<string>();
            this.straight = new Queue<string>();
        }
        public bool Promotion()
        {
            if (this.y == 7 && this.levelSumo <= 2)
            {
                this.levelSumo++;
                switch (this.levelSumo)
                {
                    case 1:
                        this.countPushedTower = 1;
                        this.maxMove = 5;
                        this.point = 1;
                        return true;
                    case 2:
                        this.countPushedTower = 2;
                        this.maxMove = 3;
                        this.point = 3;
                        return true;
                    case 3:
                        this.countPushedTower = 3;
                        this.maxMove = 1;
                        this.point = 7;
                        return true;
                    case 4:
                        this.point = 15;
                        return true;
                    default:
                        return false;
                }
            }
            return false;
        }
        public bool RecoverPawn(int x)
        {
            this.x = x;
            this.y = 0;
            return true;
        }
        public bool SumoMove(Pawn[] enemy)
        {
            Queue<int> indexEnemy = new Queue<int>();
            switch (this.levelSumo)
            {
                case 1:
                    if (this.y >= 2)
                    {
                        return false;
                    }
                    break;
                case 2:
                    if (this.y >= 3)
                    {
                        return false;
                    }
                    break;
                case 3:
                    if (this.y >= 4)
                    {
                        return false;
                    }
                    break;
                default:
                    return false;
            }
            for (int i = 0, pointer = 1; i < 8; i++)
            {
                if (enemy[i].x == this.x && enemy[i].y == this.y - pointer)
                {
                    indexEnemy.Enqueue(i);
                    pointer++;
                }
            }
            if (indexEnemy.Count > 0 && indexEnemy.Count == this.countPushedTower)
            {
                for (int j = 0; j < indexEnemy.Count; j++)
                {
                    enemy[indexEnemy.Dequeue()].y--;
                }
                this.y--;
                return true;
            }
            return false;
        }
        public bool Move(int toX, int toY, int[,] board, Pawn[] enemy)
        {
            if (toX < 0 || toX > 7 || toY < 0 || toY > 7)
            {
                return false;
            }
            if (board[toX, toY] == this.indexPawn)
            {
                for (int i = 0; i < 8; i++)
                {
                    if (toX == enemy[i].x && toY == enemy[i].y)
                    {
                        return false;
                    }
                }
                this.x = x;
                this.y = y;
                return true;
            }
            return false;
        }
        public bool PossibleMove(int indexBoard)
        {
            if (indexBoard == this.indexPawn)
            {
                int count = this.maxMove;
                for (int i = this.y, j = this.x; i < 8 && j < 8 && i >= 0 && j >= 0 && count != 0; i++, j++)
                {
                    count--;
                    diagonalRight.Enqueue(j + "-" + i);
                }
                count = this.maxMove;
                for (int i = this.y, j = this.x; i < 8 && j < 8 && i >= 0 && j >= 0 && count != 0; i++, j--)
                {
                    count--;
                    diagonalLeft.Enqueue(j + "-" + i);
                } count = this.maxMove;
                for (int i = this.y, j = this.x; i < 8 && i >= 0 && count != 0; i++)
                {
                    count--;
                    straight.Enqueue(j + "-" + i);
                }
                if (diagonalLeft.Count + diagonalRight.Count + straight.Count > 0)
                {
                    return true;
                }
            }
            return false;
        }
    }
}

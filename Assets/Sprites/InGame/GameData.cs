using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.IO;
namespace Data
{
    public static class GameData
    {
        //  CSVes
        public static string basicCsv;
        public static string levelStage;

        //Player
        public static float playerShootCd;
        public static float playerMoveTime;

        //  Bullet 
        public static float bulletMaxAmount;

        //  Boom's Flame and WallBoom Spread
        public static float spreadSpeed;
        public static float flameMaxRadius { get; set; }

        // 


        //Block Puts Area
        public static Vector3 playAreaPosition { get; set; }
        public static Vector3[] worldCorners { get; set; }


        // Block Built
        public static float blockSize;
        public static float blockSizeY;

        public static float distance { get; set; }

        // Player Move Area
        public static float[] moveDelta { get; set; }

        //Get The Target Level's Stage
        static public string SetLevelStage(int level,int stage)
        {
            levelStage = $"CSVes/Level_0{level}/Stage_0{stage}";
            return levelStage;
        }

        #region GameObject Prefab

        //Prefab's Code
        static string basicBlock;
        static string boomBlock;
        static string wallBlock;
        static string wallBoomBlock;
        static string canBreakBallBlock;

        public static GameObject TargetGameObject(string targetCode)
        {
            GameObject targetGameObject = null;
            if (targetCode == basicBlock) targetGameObject = Resources.Load<GameObject>(CSVReader.instance.ReadTargetCellString(basicCsv, "C", 3));
            else if (targetCode == boomBlock) targetGameObject = Resources.Load<GameObject>(CSVReader.instance.ReadTargetCellString(basicCsv, "C", 4));
            else if (targetCode == wallBlock) targetGameObject = Resources.Load<GameObject>(CSVReader.instance.ReadTargetCellString(basicCsv, "C", 5));
            else if (targetCode == wallBoomBlock) targetGameObject = Resources.Load<GameObject>(CSVReader.instance.ReadTargetCellString(basicCsv, "C", 6));
            else if (targetCode == canBreakBallBlock) targetGameObject = Resources.Load<GameObject>(CSVReader.instance.ReadTargetCellString(basicCsv, "C", 7));


            return targetGameObject;


        }

        //static Vector3[] worldCorners = new Vector3[4];
        //0->L.U, 1->L.D, 2->R.U, 3->R.D

        public static int rowNumber { get; private set; }
        public static int colNumber { get; private set; }

        public const int deltaLine = 8;

        static int BlockSizeRedia()
        {
            int row = 7;

            switch (rowNumber)
            {
                case 8:
                    row *= 1; break;
                case 15:
                    row *= 2; break;
                case 22:
                    row *= 3; break;
                case 29:
                    row *= 4; break;
                case 36:
                    row *= 5; break;
                default:            return 0;
            }

            return row;
        }


        static void CaluBlockSize()
        {
            float dx = Mathf.Abs(worldCorners[0].x - worldCorners[2].x);
            dx /= BlockSizeRedia();
            float dy = Mathf.Abs(worldCorners[1].y - worldCorners[3].y);

            //float xLong = dx / rowNumber;
            float yLong = dy / colNumber;

            blockSize = dx;
            blockSizeY = yLong;
        }

        static public float Width(int index)
        {
            float posX;

            posX = worldCorners[0].x;
            if (index < 0 || index > rowNumber) Debug.LogError("Out of range");
            posX += blockSize * index;
            //posX -= blockSize / 2;
            return posX;
        }

        static public float Height(int index)  
        {
            float posY;
            posY = worldCorners[1].y;
            if (index < 0 || index > colNumber) Debug.LogError("Out of range");
            posY -= blockSize * index;
            posY -= blockSize/2;
            return posY;

        }

        #endregion

        /// <summary>
        /// Init In GameStart
        /// </summary>
        public static void GameBasicDataInit()
        {
            basicCsv = "CSVes/BasicInform";

            basicBlock = CSVReader.instance.ReadTargetCellString(basicCsv, "B", 3);
            boomBlock = CSVReader.instance.ReadTargetCellString(basicCsv, "B", 4);
            wallBlock = CSVReader.instance.ReadTargetCellString(basicCsv, "B", 5);
            wallBoomBlock = CSVReader.instance.ReadTargetCellString(basicCsv, "B", 6);
            canBreakBallBlock = CSVReader.instance.ReadTargetCellString(basicCsv, "B", 7);

            //bulletMoveSpeed = CSVReader.instance.ReadTargetCellIndex(basicCsv, "B", 4);


        }

        /// <summary>
        /// Init In InGame
        /// </summary>
        public static void GameDataInit(int level, int stage)
        {
            SetLevelStage(level, stage);

            playerMoveTime = 60 / CSVReader.instance.ReadTargetCellIndex(levelStage, "B", 13);

            playerShootCd = CSVReader.instance.ReadTargetCellIndex(levelStage, "B", 16);
            bulletMaxAmount = CSVReader.instance.ReadTargetCellIndex(levelStage, "B", 17);

            float speed = CSVReader.instance.ReadTargetCellIndex(levelStage, "B", 19);

            spreadSpeed = playerMoveTime * (1 / speed);

            rowNumber = (int)CSVReader.instance.ReadTargetCellIndex(levelStage, "B", 3);
            colNumber = (int)CSVReader.instance.ReadTargetCellIndex(levelStage, "B", 4);
            if (colNumber > ((rowNumber / 2) - 1)) colNumber = (int)(rowNumber / 2) - 1;

            float posX = Mathf.Abs(worldCorners[1].x - worldCorners[2].x);
            float posY = Mathf.Abs(worldCorners[1].y - worldCorners[2].y);

            distance = Vector3.Distance(worldCorners[1], worldCorners[2]);
            //distance = Mathf.Sqrt(posX * posX + posY * posY);
            flameMaxRadius = distance * 2;

            CaluBlockSize();
        }

    }

    [System.Serializable]
    public class UI 
    {
        public Vector2Int canvasSize;

        public Vector2 playerInformArea;
        public Vector2 playArea;
        public Vector2 playerArea;

        public Vector3 playInformAreaPos;
        public Vector3 playAreaPos;
        public Vector3 playerAreaPos;

    }

    [System.Serializable]
    public class UI_Size
    {
        public Vector2Int size;
        public UI ui;

        public UI_Size()
        {
            size = Vector2Int.zero;
            ui = new UI(); 
        }
    }

    [System.Serializable]
    public class ScoreData
    {
        public int level;
        public int stage;
        public int score;

    }

    [System.Serializable]
    public class RankData
    {
        public string playerName;

        public List<ScoreData> scoreData = new List<ScoreData>();

    }
    [System.Serializable]
    public class RankList
    {
        public List<RankData> ranks = new List<RankData>();
    }


    [System.Serializable]
    public class LevelAndStage
    {
        public int level;
        public int stage;
        public LevelAndStage(int level, int stage)
        {
            this.level = level;
            this.stage = stage;
        }

        // 判断两个关卡是否相同
        public bool Equals(LevelAndStage other)
        {
            return other != null && level == other.level && stage == other.stage;
        }
    }
    [System.Serializable]
    public class BestScore
    {
        public int best;
    }
    [System.Serializable]
    public class StageScore
    {
        public LevelAndStage levelAndStage;
        public BestScore bestScore;

    }
    [System.Serializable]
    public class StageBestScoreList
    {
        public List<StageScore> bestScoreList = new List<StageScore>();
    }

}




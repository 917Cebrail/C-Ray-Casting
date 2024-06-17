using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3D_Ray_Casting
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            Console.SetWindowSize(w, h);
            Console.SetBufferSize(w, h);

            var currdate = DateTime.Now;

            while (true)
            {
                var newTime = DateTime.Now;
                var deltatime = (newTime - currdate).TotalSeconds;
                currdate = DateTime.Now;
                Input(deltatime);
                EnemyFire(deltatime);

                double[] dists = new double[w];
                for (int x = 0; x < w; x++)
                {
                    double raydir = angle + fov / 2 - x * fov / w;
                    double rayx = Math.Sin(raydir);
                    double rayy = Math.Cos(raydir);

                    double dist = 0;
                    bool hit = false;
                    double deph = 20;

                    while (!hit && dist < deph)
                    {
                        dist += raySpeed;

                        int tx = (int)(px + rayx * dist);
                        int ty = (int)(py + rayy * dist);

                        if (tx < 0 || tx >= deph + px || ty < 0 || ty >= deph + py)
                        {
                            hit = true;
                            dist = deph;
                        }
                        else
                        {
                            if (map[ty * mapW + tx] == '1')
                            {
                                hit = true;
                            }
                            else if (map[ty * mapW + tx] == '2')
                            {
                                hit = true;
                                RenderEnemy(x, dist);
                            }
                        }
                    }
                    dists[x] = dist;

                    int wall = (int)(h / 2d - h * fov / dist);
                    int floor = h - wall;

                    for (int y = 0; y < h; y++)
                    {
                        if (y <= wall)
                        {
                            buffer[y * w + x] = ' ';
                        }
                        else if (y > wall && y <= floor)
                        {
                            char wallColour = ' ';

                            if (dist <= deph / 4f)
                                wallColour = '█';
                            else if (dist <= deph / 3f)
                                wallColour = '▓';
                            else if (dist <= deph / 2f)
                                wallColour = '▒';
                            else
                                wallColour = '░';

                            buffer[y * w + x] = wallColour;
                        }
                        else
                        {
                            double floorDist = 1 - (y - h / 2d) / (h / 2f);
                            char floorChar = ' ';
                            if (floorDist <= 0.2f)
                                floorChar = '*';
                            else if (floorDist <= 0.50f)
                                floorChar = '_';
                            else if (floorDist <= 0.75f)
                                floorChar = '-';
                            else
                                floorChar = '.';

                            buffer[y * w + x] = floorChar;
                        }
                    }

                }

                Console.SetCursorPosition(0, 0);
                Console.Out.Write(buffer);
            }
        }
        public static string map =
            "1111111111111111111111111" +
            "1000100001000000000000001" +
            "1000100001000000000000001" +
            "1000100001001111110000001" +
            "1000100001001000010000001" +
            "1000100001001000010002001" +
            "1000111111001000010000001" +
            "1020000000001000010200001" +
            "1000000000001000010000001" +
            "1111111111111111111111111";

        public static double px = 2, py = 2, angle = 45;
        public static int w = 80, h = 40;
        public static double fov = Math.PI / 2.5;
        public static char[] buffer = new char[w * h];
        public static int mapW = 25, mapH = 10;
        public static double playerSpeed = 10;
        public static double raySpeed = 0.00060;

        public static void Input(double delta)
        {
            if (Console.KeyAvailable)
            {
                var k = Console.ReadKey(true).Key;
                switch (k)
                {
                    case ConsoleKey.A:
                        angle += 1f * delta;
                        break;
                    case ConsoleKey.D:
                        angle -= 1f * delta;
                        break;
                    case ConsoleKey.W:
                        px += Math.Sin(angle) * playerSpeed * delta;
                        py += Math.Cos(angle) * playerSpeed * delta;
                        if (map[(int)py * mapW + (int)px] == '1')
                        {
                            px -= Math.Sin(angle) * playerSpeed * delta;
                            py -= Math.Cos(angle) * playerSpeed * delta;
                        }
                        break;
                    case ConsoleKey.S:
                        px -= Math.Sin(angle) * playerSpeed * delta;
                        py -= Math.Cos(angle) * playerSpeed * delta;
                        if (map[(int)py * mapW + (int)px] == '1')
                        {
                            px += Math.Sin(angle) * playerSpeed * delta;
                            py += Math.Cos(angle) * playerSpeed * delta;
                        }
                        break;
                    case ConsoleKey.K:
                        Shoot();
                        break;
                }
            }
        }
        public static void Shoot()
        {
            double rayx = Math.Sin(angle);
            double rayy = Math.Cos(angle);
            double dist = 0;
            double deph = 20;
            bool hit = false;

            while (!hit && dist < deph)
            {
                dist += raySpeed;

                int tx = (int)(px + rayx * dist);
                int ty = (int)(py + rayy * dist);

                if (tx < 0 || tx >= mapW || ty < 0 || ty >= mapH)
                {
                    hit = true;
                }
                else
                {
                    if (map[ty * mapW + tx] == '2')
                    {
                        map = map.Remove(ty * mapW + tx, 1).Insert(ty * mapW + tx, "0");
                        hit = true;
                    }
                    else if (map[ty * mapW + tx] == '1')
                    {
                        hit = true;
                    }
                }
            }
        }
        public static void RenderEnemy(int x, double dist)
        {
            char enemyChar = 'E';
            int wall = (int)(h / 2d - h * fov / dist);
            int floor = h - wall;
            for (int y = 0; y < h; y++)
            {
                if (y > wall && y <= floor)
                {

                    if (dist <= 20 / 4f)
                        enemyChar = 'E';
                    else if (dist <= 20 / 3f)
                        enemyChar = 'e';
                    else if (dist <= 20 / 2f)
                        enemyChar = 'o';
                    else
                        enemyChar = '.';

                    buffer[y * w + x] = enemyChar;
                }
            }
        }
        public static void EnemyFire(double delta)
        {
            double enemyAngle = 0;
            for (int y = 0; y < mapH; y++)
            {
                for (int x = 0; x < mapW; x++)
                {
                    if (map[y * mapW + x] == '2')
                    {
                        double dx = px - x;
                        double dy = py - y;
                        double dist = Math.Sqrt(dx * dx + dy * dy);

                        if (dist < 10)
                        {
                            enemyAngle = Math.Atan2(dy, dx);
                            double rayx = Math.Sin(enemyAngle);
                            double rayy = Math.Cos(enemyAngle);

                            double rayDist = 0;
                            bool hit = false;

                            while (!hit && rayDist < 20)
                            {
                                rayDist += raySpeed;

                                int tx = (int)(x + rayx * rayDist);
                                int ty = (int)(y + rayy * rayDist);

                                if (tx < 0 || tx >= mapW || ty < 0 || ty >= mapH)
                                {
                                    hit = true;
                                }
                                else
                                {
                                    if (((int)px >= tx && (int)py >= ty) || ((int)px <= tx && (int)py <= ty))
                                    {
                                        Console.ForegroundColor = ConsoleColor.DarkRed;
                                        hit = true;
                                    }
                                    else if (map[ty * mapW + tx] == '1')
                                    {
                                        hit = true;
                                    }
                                    else
                                    {
                                        Console.ForegroundColor = ConsoleColor.White;
                                        hit = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}

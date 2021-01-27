using System.Collections.Generic;
using System.Numerics;

namespace Coordinates
{
    public class maps
    {
        public map Acidlake { get; set; }
        public map Acidlakehalloween { get; set; }
        public map Area159 { get; set; }
        public map Boombox { get; set; }
        public map Iran { get; set; }
        public map Kungur { get; set; }
        public map Massacre { get; set; }
        public map MassacremarsBG { get; set; }
        public map Nightiran { get; set; }
        public map Repin { get; set; }
        public map Rio { get; set; }
        public map Sandbox { get; set; }
        public map Silence { get; set; }
        public map Silencemoon { get; set; }
        public map Testbox { get; set; }
        public map Westprime { get; set; }
    }

    public class map
    {
        public bonusRegions bonusRegions { get; set; }
        public flags flags { get; set; }
        public IList<puntativeGeoms> puntativeGeoms { get; set; }
        public spawnPoints spawnPoints { get; set; }
    }

    public class bonusRegions
    {
        public deathmatchBonusRegions deathmatch { get; set; }
        public captureTheFlagBonusRegions captureTheFlag { get; set; }
    }
    public class deathmatchBonusRegions
    {
        public IList<bonus> armor { get; set; }
        public IList<bonus> damage { get; set; }
        public IList<bonus> gold { get; set; }
        public IList<bonus> repair { get; set; }
        public IList<bonus> speed { get; set; }
    }
    public class teamDeathmatchBonusRegions
    {
        public IList<bonus> armor { get; set; }
        public IList<bonus> damage { get; set; }
        public IList<bonus> gold { get; set; }
        public IList<bonus> repair { get; set; }
        public IList<bonus> speed { get; set; }
    }
    public class captureTheFlagBonusRegions
    {
        public IList<bonus> armor { get; set; }
        public IList<bonus> damage { get; set; }
        public IList<bonus> gold { get; set; }
        public IList<bonus> repair { get; set; }
        public IList<bonus> speed { get; set; }
    }
    public class bonus
    {
        public int number { get; set; }
        public bool parachute { get; set; }
        public position position { get; set; }
    }

    public class flags
    {
        public flag flagBlue { get; set; }
        public flag flagRed { get; set; }
    }
    public class flag
    {
        public position position { get; set; }
    }

    public class puntativeGeoms
    {
        public int number { get; set; }
        public position position { get; set; }
        public rotation rotation { get; set; }
        public size size { get; set; }
        public center center { get; set; }
    }

    public class spawnPoints
    {
        public IList<deathmatchSpawnPoints> deathmatch { get; set; }
        public teamDeathmatchSpawnPoints teamDeathmatch { get; set; }
        public captureTheFlagSpawnPoints captureTheFlag { get; set; }
    }
    public class deathmatchSpawnPoints
    {
        public int number { get; set; }
        public position position { get; set; }
        public rotation rotation { get; set; }
    }
    public class teamDeathmatchSpawnPoints
    {
        public IList<blueTeam> blueTeam { get; set; }
        public IList<redTeam> redTeam { get; set; }
    }
    public class captureTheFlagSpawnPoints
    {
        public IList<blueTeam> blueTeam { get; set; }
        public IList<redTeam> redTeam { get; set; }
    }
    public class blueTeam
    {
        public int number { get; set; }
        public position position { get; set; }
        public rotation rotation { get; set; }
    }
    public class redTeam
    {
        public int number { get; set; }
        public position position { get; set; }
        public rotation rotation { get; set; }
    }

    public class position
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public void Fill(Vector3 v3)
        {
            X = v3.X;
            Y = v3.Y;
            Z = v3.Z;
        }

        public Vector3 V3 { get { return new Vector3(X, Y, Z); } set { Fill(value); } }
    }
    public class rotation
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float W { get; set; }
    }
    public class size
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
    }
    public class center
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
    }
}
